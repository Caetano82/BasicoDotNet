using Bernhoeft.GRT.ContractWeb.Domain.SqlServer.ContractStore.Interfaces.Repositories;
using Bernhoeft.GRT.Core.EntityFramework.Domain.Interfaces;
using Bernhoeft.GRT.Teste.Application.Requests.Commands.v1;
using Bernhoeft.GRT.Teste.Application.Responses.Commands.v1;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Bernhoeft.GRT.Teste.Application.Handlers.Commands.v1
{
    public class CreateAvisoHandler : IRequestHandler<CreateAvisoRequest, CreateAvisoResponse>
    {
        private readonly IServiceProvider _serviceProvider;

        private IAvisoRepository _avisoRepository => _serviceProvider.GetRequiredService<IAvisoRepository>();

        public CreateAvisoHandler(IServiceProvider serviceProvider) => _serviceProvider = serviceProvider;

        public async Task<CreateAvisoResponse> Handle(CreateAvisoRequest request, CancellationToken cancellationToken)
        {
            var aviso = await _avisoRepository.CriarAvisoAsync(request.Titulo, request.Mensagem, cancellationToken);
            
            return new CreateAvisoResponse
            {
                Id = aviso.Id,
                Titulo = aviso.Titulo,
                Mensagem = aviso.Mensagem
            };
        }
    }
}

