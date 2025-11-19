using Bernhoeft.GRT.ContractWeb.Domain.SqlServer.ContractStore.Interfaces.Repositories;
using Bernhoeft.GRT.Core.EntityFramework.Domain.Interfaces;
using Bernhoeft.GRT.Teste.Application.Requests.Commands.v1;
using Bernhoeft.GRT.Teste.Application.Responses.Queries.v1;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Bernhoeft.GRT.Teste.Application.Handlers.Commands.v1
{
    public class GetAvisoByIdHandler : IRequestHandler<GetAvisoByIdRequest, object>
    {
        private readonly IServiceProvider _serviceProvider;

        private IAvisoRepository _avisoRepository => _serviceProvider.GetRequiredService<IAvisoRepository>();

        public GetAvisoByIdHandler(IServiceProvider serviceProvider) => _serviceProvider = serviceProvider;

        public async Task<object> Handle(GetAvisoByIdRequest request, CancellationToken cancellationToken)
        {
            var result = await _avisoRepository.ObterAvisoPorIdAsync(request.Id, cancellationToken);
            if (result == null)
                return null;

            return (GetAvisosResponse)result;
        }
    }
}

