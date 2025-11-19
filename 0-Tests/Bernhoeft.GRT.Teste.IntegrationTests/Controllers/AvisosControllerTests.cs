using Moq;
using Xunit;
using FluentAssertions;
using Bernhoeft.GRT.Teste.Api.Controllers.v1;
using Bernhoeft.GRT.Teste.Api.Hubs;
using Bernhoeft.GRT.Teste.Application.Requests.Commands.v1;
using Bernhoeft.GRT.Teste.Application.Responses.Commands.v1;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using MediatR;
using Bernhoeft.GRT.Teste.Application.Requests.Queries.v1;
using Bernhoeft.GRT.Teste.Application.Responses.Queries.v1;
using Bernhoeft.GRT.Core.Models;
using Bernhoeft.GRT.Core.Interfaces.Results;
using PagedResult = Bernhoeft.GRT.Teste.Application.Responses.Queries.v1.PagedResult<Bernhoeft.GRT.Teste.Application.Responses.Queries.v1.GetAvisosResponse>;

namespace Bernhoeft.GRT.Teste.IntegrationTests.Controllers
{
 public class AvisosControllerTests
 {
 private readonly Mock<IMediator> _mediatorMock;
 private readonly Mock<IHubContext<AvisosHub>> _hubContextMock;
 private readonly AvisosController _controller;

 public AvisosControllerTests()
 {
 _mediatorMock = new Mock<IMediator>();
 _hubContextMock = new Mock<IHubContext<AvisosHub>>();
 
 // Mock do IHubContext para os métodos necessários
 var clientsMock = new Mock<IHubClients>();
 var groupMock = new Mock<IClientProxy>();
 clientsMock.Setup(c => c.Group(It.IsAny<string>())).Returns(groupMock.Object);
 _hubContextMock.Setup(h => h.Clients).Returns(clientsMock.Object);
 
 _controller = new AvisosController(_mediatorMock.Object, _hubContextMock.Object);
 }

 [Fact]
 public async Task GetAvisoById_ShouldReturnOk_WhenAvisoExists()
 {
 // Arrange
 var avisoId =1;
 var avisoResponse = new GetAvisosResponse { Id = avisoId, Titulo = "Teste", Mensagem = "Mensagem de Teste", Ativo = true };
 _mediatorMock.Setup(m => m.Send(It.IsAny<GetAvisoByIdRequest>(), It.IsAny<CancellationToken>()))
 .ReturnsAsync(avisoResponse);

 // Act
 var result = await _controller.GetAvisoById(avisoId, CancellationToken.None);

 // Assert
 result.Should().BeOfType<OkObjectResult>();
 var okResult = result as OkObjectResult;
 okResult!.Value.Should().NotBeNull();
 }

 [Fact]
 public async Task GetAvisoById_ShouldReturnNotFound_WhenAvisoDoesNotExist()
 {
 // Arrange
 var avisoId =1;
 _mediatorMock.Setup(m => m.Send(It.IsAny<GetAvisoByIdRequest>(), It.IsAny<CancellationToken>()))
 .ReturnsAsync((object?)null);

 // Act
 var result = await _controller.GetAvisoById(avisoId, CancellationToken.None);

 // Assert
 result.Should().BeOfType<NotFoundObjectResult>();
 }

 [Theory]
 [InlineData(0)]
 [InlineData(-5)]
 public async Task GetAvisoById_ShouldReturnBadRequest_ForInvalidId(int invalidId)
 {
 var result = await _controller.GetAvisoById(invalidId, CancellationToken.None);
 result.Should().BeOfType<BadRequestObjectResult>();
 }

 [Fact]
 public async Task CreateAviso_ShouldReturnCreated_WhenRequestIsValid()
 {
 // Arrange
 var createRequest = new CreateAvisoRequest { Titulo = "Novo Aviso", Mensagem = "Mensagem do aviso" };
 var createResponse = new CreateAvisoResponse { Id =1, Titulo = createRequest.Titulo, Mensagem = createRequest.Mensagem };
 _mediatorMock.Setup(m => m.Send(It.IsAny<CreateAvisoRequest>(), It.IsAny<CancellationToken>()))
 .ReturnsAsync(createResponse);

 // Act
 var result = await _controller.CreateAviso(createRequest, CancellationToken.None);

 // Assert
 result.Should().BeOfType<CreatedAtActionResult>();
 var createdResult = result as CreatedAtActionResult;
 createdResult!.Value.Should().BeEquivalentTo(createResponse);
 }

 [Fact]
 public async Task CreateAviso_ShouldReturnBadRequest_WhenModelStateInvalid()
 {
 // Arrange
 _controller.ModelState.AddModelError("Titulo", "Titulo é obrigatório");

 // Act
 var result = await _controller.CreateAviso(new CreateAvisoRequest(), CancellationToken.None);

 // Assert
 result.Should().BeOfType<BadRequestObjectResult>();
 }

 [Fact]
 public async Task CreateAviso_ShouldReturnBadRequest_WhenMediatorReturnsUnexpectedType()
 {
 // Arrange
 var req = new CreateAvisoRequest { Titulo = "T", Mensagem = "M" };
 _mediatorMock.Setup(m => m.Send(It.IsAny<CreateAvisoRequest>(), It.IsAny<CancellationToken>()))
 .ReturnsAsync((CreateAvisoResponse?)null);

 // Act
 var result = await _controller.CreateAviso(req, CancellationToken.None);

 // Assert
 result.Should().BeOfType<BadRequestObjectResult>();
 }

 [Fact]
 public async Task UpdateAviso_ShouldReturnOk_WhenAvisoIsUpdated()
 {
 // Arrange
 var avisoId =1;
 var updateRequest = new UpdateAvisoRequest { Mensagem = "Mensagem atualizada" };
 _mediatorMock.Setup(m => m.Send(It.IsAny<UpdateAvisoCommand>(), It.IsAny<CancellationToken>()))
 .ReturnsAsync(true);

 // Act
 var result = await _controller.UpdateAviso(avisoId, updateRequest, CancellationToken.None);

 // Assert
 result.Should().BeOfType<OkObjectResult>();
 }

 [Fact]
 public async Task UpdateAviso_ShouldReturnNotFound_WhenAvisoDoesNotExist()
 {
 // Arrange
 var avisoId =1;
 var updateRequest = new UpdateAvisoRequest { Mensagem = "Mensagem atualizada" };
 _mediatorMock.Setup(m => m.Send(It.IsAny<UpdateAvisoCommand>(), It.IsAny<CancellationToken>()))
 .ReturnsAsync(false);

 // Act
 var result = await _controller.UpdateAviso(avisoId, updateRequest, CancellationToken.None);

 // Assert
 result.Should().BeOfType<NotFoundObjectResult>();
 }

 [Theory]
 [InlineData(0)]
 [InlineData(-9)]
 public async Task UpdateAviso_ShouldReturnBadRequest_ForInvalidId(int invalidId)
 {
 var updateRequest = new UpdateAvisoRequest { Mensagem = "Mensagem" };
 var result = await _controller.UpdateAviso(invalidId, updateRequest, CancellationToken.None);
 result.Should().BeOfType<BadRequestObjectResult>();
 }

 [Fact]
 public async Task DeleteAviso_ShouldReturnNoContent_WhenAvisoIsDeleted()
 {
 // Arrange
 var avisoId =1;
 _mediatorMock.Setup(m => m.Send(It.IsAny<DeleteAvisoCommand>(), It.IsAny<CancellationToken>()))
 .ReturnsAsync(true);

 // Act
 var result = await _controller.DeleteAviso(avisoId, CancellationToken.None);

 // Assert
 result.Should().BeOfType<NoContentResult>();
 }

 [Fact]
 public async Task DeleteAviso_ShouldReturnNotFound_WhenAvisoDoesNotExist()
 {
 // Arrange
 var avisoId =1;
 _mediatorMock.Setup(m => m.Send(It.IsAny<DeleteAvisoCommand>(), It.IsAny<CancellationToken>()))
 .ReturnsAsync(false);

 // Act
 var result = await _controller.DeleteAviso(avisoId, CancellationToken.None);

 // Assert
 result.Should().BeOfType<NotFoundResult>();
 }

 [Theory]
 [InlineData(0)]
 [InlineData(-3)]
 public async Task DeleteAviso_ShouldReturnBadRequest_ForInvalidId(int invalidId)
 {
 var result = await _controller.DeleteAviso(invalidId, CancellationToken.None);
 result.Should().BeOfType<BadRequestObjectResult>();
 }

    [Fact]
    public async Task GetAvisos_ShouldReturnOk_WhenAvisosExist()
    {
        // Arrange
        var avisosResponse = new[] { new GetAvisosResponse { Id =1, Titulo = "Aviso1", Mensagem = "Mensagem1", Ativo = true } };
        var pagedResult = new PagedResult<GetAvisosResponse>
        {
            Data = avisosResponse,
            Page = 1,
            PageSize = 10,
            TotalCount = 1
        };
        var operationResult = OperationResult<PagedResult<GetAvisosResponse>>.ReturnOk(pagedResult);
        _mediatorMock.Setup(m => m.Send(It.IsAny<GetAvisosRequest>(), It.IsAny<CancellationToken>()))
        .ReturnsAsync((IOperationResult<PagedResult<GetAvisosResponse>>)operationResult);

        // Act
        var result = await _controller.GetAvisos(1, 10, CancellationToken.None);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        var okResult = result as OkObjectResult;
        okResult!.Value.Should().NotBeNull();
        
        // Verifica propriedades calculadas do PagedResult
        pagedResult.TotalPages.Should().Be(1);
        pagedResult.HasPreviousPage.Should().BeFalse();
        pagedResult.HasNextPage.Should().BeFalse();
    }

    [Fact]
    public async Task GetAvisos_ShouldReturnNoContent_WhenNoAvisosExist()
    {
        // Arrange
        var operationResult = OperationResult<PagedResult<GetAvisosResponse>>.ReturnNoContent();
        _mediatorMock.Setup(m => m.Send(It.IsAny<GetAvisosRequest>(), It.IsAny<CancellationToken>()))
        .ReturnsAsync((IOperationResult<PagedResult<GetAvisosResponse>>)operationResult);

        // Act
        var result = await _controller.GetAvisos(1, 10, CancellationToken.None);

        // Assert
        result.Should().BeOfType<NoContentResult>();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public async Task GetAvisos_ShouldNormalizePage_WhenPageIsInvalid(int invalidPage)
    {
        // Arrange
        var operationResult = OperationResult<PagedResult<GetAvisosResponse>>.ReturnNoContent();
        _mediatorMock.Setup(m => m.Send(It.IsAny<GetAvisosRequest>(), It.IsAny<CancellationToken>()))
        .ReturnsAsync((IOperationResult<PagedResult<GetAvisosResponse>>)operationResult);

        // Act
        var result = await _controller.GetAvisos(invalidPage, 10, CancellationToken.None);

        // Assert
        _mediatorMock.Verify(m => m.Send(It.Is<GetAvisosRequest>(r => r.Page == 1), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(101)]
    public async Task GetAvisos_ShouldNormalizePageSize_WhenPageSizeIsInvalid(int invalidPageSize)
    {
        // Arrange
        var operationResult = OperationResult<PagedResult<GetAvisosResponse>>.ReturnNoContent();
        _mediatorMock.Setup(m => m.Send(It.IsAny<GetAvisosRequest>(), It.IsAny<CancellationToken>()))
        .ReturnsAsync((IOperationResult<PagedResult<GetAvisosResponse>>)operationResult);

        // Act
        var result = await _controller.GetAvisos(1, invalidPageSize, CancellationToken.None);

        // Assert
        _mediatorMock.Verify(m => m.Send(It.Is<GetAvisosRequest>(r => r.PageSize == 10), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetAvisos_ShouldReturnBadRequest_WhenResultIsNotOperationResult()
    {
        // Arrange - Simula um resultado que não é OperationResult
        var invalidResult = new { Invalid = "data" };
        _mediatorMock.Setup(m => m.Send(It.IsAny<GetAvisosRequest>(), It.IsAny<CancellationToken>()))
        .ReturnsAsync((IOperationResult<PagedResult<GetAvisosResponse>>)null!);

        // Act
        var result = await _controller.GetAvisos(1, 10, CancellationToken.None);

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task CreateAviso_ShouldReturnBadRequest_WhenResultIsNotCreateAvisoResponse()
    {
        // Arrange - Simula um resultado null (não é CreateAvisoResponse)
        var createRequest = new CreateAvisoRequest { Titulo = "Novo Aviso", Mensagem = "Mensagem do aviso" };
        _mediatorMock.Setup(m => m.Send(It.IsAny<CreateAvisoRequest>(), It.IsAny<CancellationToken>()))
        .ReturnsAsync((CreateAvisoResponse)null!);

        // Act
        var result = await _controller.CreateAviso(createRequest, CancellationToken.None);

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
        var badRequest = result as BadRequestObjectResult;
        badRequest!.Value.Should().Be("Invalid response from Mediator");
    }

    [Fact]
    public async Task UpdateAviso_ShouldReturnBadRequest_WhenModelStateInvalid()
    {
        // Arrange
        var updateRequest = new UpdateAvisoRequest { Mensagem = "Mensagem atualizada" };
        _controller.ModelState.AddModelError("Mensagem", "Mensagem é obrigatória");

        // Act
        var result = await _controller.UpdateAviso(1, updateRequest, CancellationToken.None);

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
    }
}
}