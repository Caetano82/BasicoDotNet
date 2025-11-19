using Bernhoeft.GRT.Core.Interfaces.Results;
using Bernhoeft.GRT.Teste.Application.Responses.Queries.v1;
using MediatR;

namespace Bernhoeft.GRT.Teste.Application.Requests.Queries.v1
{
    public class GetAvisosRequest : IRequest<IOperationResult<PagedResult<GetAvisosResponse>>>
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;

        public GetAvisosRequest()
        {
        }

        public GetAvisosRequest(int page, int pageSize)
        {
            Page = page > 0 ? page : 1;
            PageSize = pageSize > 0 && pageSize <= 100 ? pageSize : 10;
        }
    }
}