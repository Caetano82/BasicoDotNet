using Bernhoeft.GRT.ContractWeb.Domain.SqlServer.ContractStore.Interfaces.Repositories;
using Bernhoeft.GRT.Core.EntityFramework.Domain.Interfaces;
using Bernhoeft.GRT.Core.Enums;
using Bernhoeft.GRT.Core.Extensions;
using Bernhoeft.GRT.Core.Interfaces.Results;
using Bernhoeft.GRT.Core.Models;
using Bernhoeft.GRT.Teste.Application.Requests.Queries.v1;
using Bernhoeft.GRT.Teste.Application.Responses.Queries.v1;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Bernhoeft.GRT.Teste.Application.Handlers.Queries.v1
{
    public class GetAvisosHandler : IRequestHandler<GetAvisosRequest, IOperationResult<PagedResult<GetAvisosResponse>>>
    {
        private readonly IServiceProvider _serviceProvider;

        private IContext _context => _serviceProvider.GetRequiredService<IContext>();
        private IAvisoRepository _avisoRepository => _serviceProvider.GetRequiredService<IAvisoRepository>();

        public GetAvisosHandler(IServiceProvider serviceProvider) => _serviceProvider = serviceProvider;

        public async Task<IOperationResult<PagedResult<GetAvisosResponse>>> Handle(GetAvisosRequest request, CancellationToken cancellationToken)
        {
            var (items, totalCount) = await _avisoRepository.ObterAvisosPaginadosAsync(
                request.Page, 
                request.PageSize, 
                TrackingBehavior.NoTracking, 
                cancellationToken);

            if (totalCount == 0)
                return OperationResult<PagedResult<GetAvisosResponse>>.ReturnNoContent();

            var pagedResult = new PagedResult<GetAvisosResponse>
            {
                Data = items.Select(x => (GetAvisosResponse)x),
                Page = request.Page,
                PageSize = request.PageSize,
                TotalCount = totalCount
            };

            return OperationResult<PagedResult<GetAvisosResponse>>.ReturnOk(pagedResult);
        }
    }
}