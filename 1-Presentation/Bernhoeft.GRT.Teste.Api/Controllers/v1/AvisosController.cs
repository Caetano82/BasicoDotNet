using Bernhoeft.GRT.Teste.Application.Requests.Commands.v1;
using Bernhoeft.GRT.Teste.Application.Requests.Queries.v1;
using Bernhoeft.GRT.Teste.Application.Responses.Commands.v1;
using Bernhoeft.GRT.Teste.Application.Responses.Queries.v1;
using Bernhoeft.GRT.Core.Models;
using Bernhoeft.GRT.Teste.Api.Hubs;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace Bernhoeft.GRT.Teste.Api.Controllers.v1
{
    /// <response code="401">Não Autenticado.</response>
    /// <response code="403">Não Autorizado.</response>
    /// <response code="500">Erro Interno no Servidor.</response>
    [AllowAnonymous]
    [ApiVersion("1.0")]
    [Produces("application/json")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = null)]
    [ProducesResponseType(StatusCodes.Status403Forbidden, Type = null)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = null)]
    public class AvisosController : RestApiController
    {
        private readonly IMediator _mediator;
        private readonly IHubContext<AvisosHub> _hubContext;

        public AvisosController(IMediator mediator, IHubContext<AvisosHub> hubContext)
        {
            _mediator = mediator;
            _hubContext = hubContext;
        }

        /// <summary>
        /// Retorna um Aviso por ID.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>Aviso.</returns>
        /// <response code="200">Sucesso.</response>
        /// <response code="400">Id inválido.</response>
        /// <response code="404">Aviso Não Encontrado.</response>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetAvisosResponse))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetAvisoById(int id, CancellationToken cancellationToken)
        {
            if (id <= 0)
                return BadRequest("Id inválido.");

            var result = await _mediator.Send(new GetAvisoByIdRequest { Id = id }, cancellationToken);
            return result != null ? Ok(new { Mensagem = "Aviso encontrado.", Data = result }) : NotFound(new { Mensagem = "Aviso não encontrado." });
        }

        /// <summary>
        /// Cria um novo Aviso.
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>Aviso Criado.</returns>
        /// <response code="201">Criado.</response>
        /// <response code="400">Dados Inválidos.</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateAviso([FromBody] CreateAvisoRequest request, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _mediator.Send(request, cancellationToken);
            if (result is CreateAvisoResponse response)
            {
                // Notificar todos os clientes conectados via SignalR
                await _hubContext.Clients.Group("Board").SendAsync("AvisoCriado", new
                {
                    Id = response.Id,
                    Titulo = response.Titulo,
                    Mensagem = response.Mensagem,
                    Ativo = true
                });

                return CreatedAtAction(nameof(GetAvisoById), new { id = response.Id }, response);
            }

            return BadRequest("Invalid response from Mediator");
        }

        /// <summary>
        /// Atualiza um Aviso existente.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>Aviso Atualizado.</returns>
        /// <response code="200">Sucesso.</response>
        /// <response code="400">Id inválido ou Dados Inválidos.</response>
        /// <response code="404">Aviso Não Encontrado.</response>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAviso(int id, [FromBody] UpdateAvisoRequest request, CancellationToken cancellationToken)
        {
            if (id <= 0)
                return BadRequest("Id inválido.");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _mediator.Send(new UpdateAvisoCommand { Id = id, Mensagem = request.Mensagem }, cancellationToken);
            if (result is bool success && success)
            {
                // Notificar todos os clientes conectados via SignalR
                await _hubContext.Clients.Group("Board").SendAsync("AvisoAtualizado", new
                {
                    Id = id,
                    Mensagem = request.Mensagem
                });

                return Ok(new { Mensagem = "Aviso atualizado com sucesso." });
            }

            return NotFound(new { Mensagem = "Aviso não encontrado." });
        }

        /// <summary>
        /// Exclui um Aviso.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>Resultado da Exclusão.</returns>
        /// <response code="204">Sem Conteúdo.</response>
        /// <response code="400">Id inválido.</response>
        /// <response code="404">Aviso Não Encontrado.</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteAviso(int id, CancellationToken cancellationToken)
        {
            if (id <= 0)
                return BadRequest("Id inválido.");

            var result = await _mediator.Send(new DeleteAvisoCommand { Id = id }, cancellationToken);
            if (result)
            {
                // Notificar todos os clientes conectados via SignalR
                await _hubContext.Clients.Group("Board").SendAsync("AvisoDeletado", new { Id = id });

                return NoContent();
            }

            return NotFound();
        }

        /// <summary>
        /// Retorna Todos os Avisos Cadastrados para Tela de Edição (com paginação).
        /// </summary>
        /// <param name="page">Número da página (padrão: 1)</param>
        /// <param name="pageSize">Tamanho da página (padrão: 10, máximo: 100)</param>
        /// <param name="cancellationToken"></param>
        /// <returns>Lista paginada com Todos os Avisos.</returns>
        /// <response code="200">Sucesso.</response>
        /// <response code="204">Sem Avisos.</response>
        [HttpGet]
        public async Task<IActionResult> GetAvisos([FromQuery] int page = 1, [FromQuery] int pageSize = 10, CancellationToken cancellationToken = default)
        {
            if (page < 1)
                page = 1;
            if (pageSize < 1 || pageSize > 100)
                pageSize = 10;

            var result = await _mediator.Send(new GetAvisosRequest(page, pageSize), cancellationToken);
            if (result is OperationResult<PagedResult<GetAvisosResponse>> operationResult)
            {
                if ((int)operationResult.StatusCode == 204)
                    return NoContent();

                if (operationResult.Data != null)
                    return Ok(new { Mensagem = "Avisos encontrados.", Data = operationResult.Data });
            }

            return BadRequest(new { Mensagem = "Unexpected result format." });
        }
    }
}