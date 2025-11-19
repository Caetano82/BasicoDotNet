using Bernhoeft.GRT.ContractWeb.Domain.SqlServer.ContractStore.Entities;
using Bernhoeft.GRT.ContractWeb.Domain.SqlServer.ContractStore.Interfaces.Repositories;
using Bernhoeft.GRT.Core.Attributes;
using Bernhoeft.GRT.Core.EntityFramework.Infra;
using Bernhoeft.GRT.Core.Enums;
using Microsoft.EntityFrameworkCore;

namespace Bernhoeft.GRT.ContractWeb.Infra.Persistence.SqlServer.ContractStore.Repositories
{
    [InjectService(Interface: typeof(IAvisoRepository))]
    public class AvisoRepository : Repository<AvisoEntity>, IAvisoRepository
    {
        private readonly DbContext _dbContext;

        public AvisoRepository(IServiceProvider serviceProvider, DbContext dbContext) : base(serviceProvider)
        {
            _dbContext = dbContext;
        }

        public Task<List<AvisoEntity>> ObterTodosAvisosAsync(TrackingBehavior tracking = TrackingBehavior.Default, CancellationToken cancellationToken = default)
        {
            var query = tracking is TrackingBehavior.NoTracking ? Set.AsNoTrackingWithIdentityResolution() : Set;
            return query.Where(x => !x.IsDeleted).ToListAsync(cancellationToken);
        }

        public async Task<(List<AvisoEntity> Items, int TotalCount)> ObterAvisosPaginadosAsync(int page, int pageSize, TrackingBehavior tracking = TrackingBehavior.Default, CancellationToken cancellationToken = default)
        {
            var query = tracking is TrackingBehavior.NoTracking ? Set.AsNoTrackingWithIdentityResolution() : Set;
            var filteredQuery = query.Where(x => !x.IsDeleted);
            
            var totalCount = await filteredQuery.CountAsync(cancellationToken);
            var items = await filteredQuery
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            return (items, totalCount);
        }

        public async Task<AvisoEntity> ObterAvisoPorIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return await Set.FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, cancellationToken);
        }

        public async Task<AvisoEntity> CriarAvisoAsync(string titulo, string mensagem, CancellationToken cancellationToken = default)
        {
            var aviso = new AvisoEntity
            {
                Titulo = titulo,
                Mensagem = mensagem
            };
            Add(aviso);
            await _dbContext.SaveChangesAsync(cancellationToken);
            return aviso;
        }

        public async Task SoftDeleteAvisoAsync(AvisoEntity aviso, CancellationToken cancellationToken = default)
        {
            aviso.MarkAsDeleted();
            Update(aviso);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }

        public async Task<bool> AtualizarAvisoAsync(int id, string mensagem, CancellationToken cancellationToken = default)
        {
            var aviso = await ObterAvisoPorIdAsync(id, cancellationToken);
            if (aviso == null)
                return false;

            aviso.UpdateMessage(mensagem);
            Update(aviso);
            await _dbContext.SaveChangesAsync(cancellationToken);
            return true;
        }

        public async Task<bool> DeletarAvisoAsync(int id, CancellationToken cancellationToken = default)
        {
            var aviso = await ObterAvisoPorIdAsync(id, cancellationToken);
            if (aviso == null)
                return false;

            await SoftDeleteAvisoAsync(aviso, cancellationToken);
            return true;
        }
    }
}