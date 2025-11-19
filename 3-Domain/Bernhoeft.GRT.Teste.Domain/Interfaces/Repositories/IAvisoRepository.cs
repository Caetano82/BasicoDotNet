using Bernhoeft.GRT.ContractWeb.Domain.SqlServer.ContractStore.Entities;
using Bernhoeft.GRT.Core.EntityFramework.Domain.Interfaces;
using Bernhoeft.GRT.Core.Enums;

namespace Bernhoeft.GRT.ContractWeb.Domain.SqlServer.ContractStore.Interfaces.Repositories
{
    public interface IAvisoRepository : IRepository<AvisoEntity>
    {
        Task<List<AvisoEntity>> ObterTodosAvisosAsync(TrackingBehavior tracking = TrackingBehavior.Default, CancellationToken cancellationToken = default);
        Task<AvisoEntity> ObterAvisoPorIdAsync(int id, CancellationToken cancellationToken = default);
        Task<(List<AvisoEntity> Items, int TotalCount)> ObterAvisosPaginadosAsync(int page, int pageSize, TrackingBehavior tracking = TrackingBehavior.Default, CancellationToken cancellationToken = default);
        Task<AvisoEntity> CriarAvisoAsync(string titulo, string mensagem, CancellationToken cancellationToken = default);
        Task<bool> AtualizarAvisoAsync(int id, string mensagem, CancellationToken cancellationToken = default);
        Task<bool> DeletarAvisoAsync(int id, CancellationToken cancellationToken = default);
    }
}