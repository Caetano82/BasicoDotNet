using Bernhoeft.GRT.ContractWeb.Domain.SqlServer.ContractStore.Interfaces.Repositories;
using Bernhoeft.GRT.Core.EntityFramework.Domain.Interfaces;
using Bernhoeft.GRT.Teste.Application.Requests.Commands.v1;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Bernhoeft.GRT.Teste.Application.Handlers.Commands.v1
{
    public class UpdateAvisoHandler : IRequestHandler<UpdateAvisoCommand, object>
    {
        private readonly IServiceProvider _serviceProvider;

        private IAvisoRepository _avisoRepository => _serviceProvider.GetRequiredService<IAvisoRepository>();

        public UpdateAvisoHandler(IServiceProvider serviceProvider) => _serviceProvider = serviceProvider;

        public async Task<object> Handle(UpdateAvisoCommand request, CancellationToken cancellationToken)
        {
            var result = await _avisoRepository.AtualizarAvisoAsync(request.Id, request.Mensagem, cancellationToken);
            return result;
        }
    }
}

