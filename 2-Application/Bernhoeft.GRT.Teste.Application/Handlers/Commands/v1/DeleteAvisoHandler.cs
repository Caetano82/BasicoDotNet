using Bernhoeft.GRT.ContractWeb.Domain.SqlServer.ContractStore.Interfaces.Repositories;
using Bernhoeft.GRT.Core.EntityFramework.Domain.Interfaces;
using Bernhoeft.GRT.Teste.Application.Requests.Commands.v1;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Bernhoeft.GRT.Teste.Application.Handlers.Commands.v1
{
    public class DeleteAvisoHandler : IRequestHandler<DeleteAvisoCommand, bool>
    {
        private readonly IServiceProvider _serviceProvider;

        private IAvisoRepository _avisoRepository => _serviceProvider.GetRequiredService<IAvisoRepository>();

        public DeleteAvisoHandler(IServiceProvider serviceProvider) => _serviceProvider = serviceProvider;

        public async Task<bool> Handle(DeleteAvisoCommand request, CancellationToken cancellationToken)
        {
            return await _avisoRepository.DeletarAvisoAsync(request.Id, cancellationToken);
        }
    }
}

