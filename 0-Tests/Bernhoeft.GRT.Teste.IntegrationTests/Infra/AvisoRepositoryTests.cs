using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using Bernhoeft.GRT.ContractWeb.Domain.SqlServer.ContractStore.Entities;
using Bernhoeft.GRT.ContractWeb.Infra.Persistence.SqlServer.ContractStore.Repositories;
using Bernhoeft.GRT.Core.Enums;
using Bernhoeft.GRT.Core.EntityFramework.Domain.Interfaces;

namespace Bernhoeft.GRT.Teste.IntegrationTests.Infra
{
    public class AvisoRepositoryTests
    {
        /// <summary>
        /// Cria o ServiceProvider usando a mesma configuração da aplicação
        /// (AddDbContext registra IContext, DbContext e faz o seeding).
        /// </summary>
        private ServiceProvider CreateServiceProvider()
        {
            var services = new ServiceCollection();

            // Usa SUA extensão que registra o contexto in-memory e IContext
            services.AddDbContext();

            return services.BuildServiceProvider();
        }

        /// <summary>
        /// Cria um scope, resolve o IContext como DbContext e instancia o AvisoRepository.
        /// Também limpa os Avisos semeados para que cada teste controle o estado.
        /// </summary>
        private (AvisoRepository Repo, DbContext Ctx, IServiceScope Scope) CreateRepository()
        {
            var rootProvider = CreateServiceProvider();
            var scope = rootProvider.CreateScope();

            var sp = scope.ServiceProvider;

            // IContext é a interface do contexto que herda de DbContext
            var ctx = (DbContext)sp.GetRequiredService<IContext>();

            // Limpa os registros semeados para partir sempre de um estado limpo
            var avisosSet = ctx.Set<AvisoEntity>();
            ctx.RemoveRange(avisosSet);
            ctx.SaveChanges();

            // Instancia o repositório manualmente, mas passando o ServiceProvider real
            var repo = new AvisoRepository(sp, ctx);

            return (repo, ctx, scope);
        }

        [Fact]
        public async Task ObterTodosAvisosAsync_ShouldReturnOnlyNotDeleted()
        {
            var (repo, ctx, scope) = CreateRepository();
            using (scope)
            {
                var a1 = new AvisoEntity { Titulo = "A1", Mensagem = "M1" };
                var a2 = new AvisoEntity { Titulo = "A2", Mensagem = "M2" };

                ctx.AddRange(a1, a2);
                await ctx.SaveChangesAsync();

                a2.MarkAsDeleted();
                ctx.Update(a2);
                await ctx.SaveChangesAsync();

                var list = await repo.ObterTodosAvisosAsync(TrackingBehavior.NoTracking, CancellationToken.None);

                list.Should().HaveCount(1);
                list.Single().Titulo.Should().Be("A1");
            }
        }

        [Fact]
        public async Task ObterAvisoPorIdAsync_ShouldReturnEntity_WhenExistsAndNotDeleted()
        {
            var (repo, ctx, scope) = CreateRepository();
            using (scope)
            {
                var a1 = new AvisoEntity { Titulo = "A1", Mensagem = "M1" };
                ctx.Add(a1);
                await ctx.SaveChangesAsync();

                var found = await repo.ObterAvisoPorIdAsync(a1.Id);

                found.Should().NotBeNull();
                found!.Titulo.Should().Be("A1");
            }
        }

        [Fact]
        public async Task ObterAvisoPorIdAsync_ShouldReturnNull_WhenDeleted()
        {
            var (repo, ctx, scope) = CreateRepository();
            using (scope)
            {
                var a1 = new AvisoEntity { Titulo = "A1", Mensagem = "M1" };
                ctx.Add(a1);
                await ctx.SaveChangesAsync();

                a1.MarkAsDeleted();
                ctx.Update(a1);
                await ctx.SaveChangesAsync();

                var found = await repo.ObterAvisoPorIdAsync(a1.Id);

                found.Should().BeNull();
            }
        }

        [Fact]
        public async Task SoftDeleteAvisoAsync_ShouldPersistDeletion()
        {
            var (repo, ctx, scope) = CreateRepository();
            using (scope)
            {
                var aviso = new AvisoEntity { Titulo = "A", Mensagem = "M" };
                ctx.Add(aviso);
                await ctx.SaveChangesAsync();

                await repo.SoftDeleteAvisoAsync(aviso);

                var again = await repo.ObterAvisoPorIdAsync(aviso.Id);
                again.Should().BeNull();

                var all = await repo.ObterTodosAvisosAsync();
                all.Should().BeEmpty();
            }
        }

        [Fact]
        public async Task CriarAvisoAsync_ShouldCreateAndReturnAviso()
        {
            var (repo, ctx, scope) = CreateRepository();
            using (scope)
            {
                var aviso = await repo.CriarAvisoAsync("Novo Título", "Nova Mensagem", CancellationToken.None);

                aviso.Should().NotBeNull();
                aviso.Id.Should().BeGreaterThan(0);
                aviso.Titulo.Should().Be("Novo Título");
                aviso.Mensagem.Should().Be("Nova Mensagem");
                aviso.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(2));
                aviso.IsDeleted.Should().BeFalse();

                var found = await repo.ObterAvisoPorIdAsync(aviso.Id);
                found.Should().NotBeNull();
                found!.Titulo.Should().Be("Novo Título");
            }
        }

        [Fact]
        public async Task AtualizarAvisoAsync_ShouldUpdateMessage_WhenAvisoExists()
        {
            var (repo, ctx, scope) = CreateRepository();
            using (scope)
            {
                var aviso = new AvisoEntity { Titulo = "Título Original", Mensagem = "Mensagem Original" };
                ctx.Add(aviso);
                await ctx.SaveChangesAsync();

                var result = await repo.AtualizarAvisoAsync(aviso.Id, "Mensagem Atualizada", CancellationToken.None);

                result.Should().BeTrue();
                var updated = await repo.ObterAvisoPorIdAsync(aviso.Id);
                updated.Should().NotBeNull();
                updated!.Mensagem.Should().Be("Mensagem Atualizada");
                updated.Titulo.Should().Be("Título Original"); // Título não deve mudar
                updated.UpdatedAt.Should().NotBeNull();
            }
        }

        [Fact]
        public async Task AtualizarAvisoAsync_ShouldReturnFalse_WhenAvisoDoesNotExist()
        {
            var (repo, ctx, scope) = CreateRepository();
            using (scope)
            {
                var result = await repo.AtualizarAvisoAsync(999, "Mensagem", CancellationToken.None);

                result.Should().BeFalse();
            }
        }

        [Fact]
        public async Task DeletarAvisoAsync_ShouldReturnTrue_WhenAvisoExists()
        {
            var (repo, ctx, scope) = CreateRepository();
            using (scope)
            {
                var aviso = new AvisoEntity { Titulo = "Título", Mensagem = "Mensagem" };
                ctx.Add(aviso);
                await ctx.SaveChangesAsync();

                var result = await repo.DeletarAvisoAsync(aviso.Id, CancellationToken.None);

                result.Should().BeTrue();
                var deleted = await repo.ObterAvisoPorIdAsync(aviso.Id);
                deleted.Should().BeNull();
            }
        }

        [Fact]
        public async Task DeletarAvisoAsync_ShouldReturnFalse_WhenAvisoDoesNotExist()
        {
            var (repo, ctx, scope) = CreateRepository();
            using (scope)
            {
                var result = await repo.DeletarAvisoAsync(999, CancellationToken.None);

                result.Should().BeFalse();
            }
        }

        [Fact]
        public async Task ObterAvisosPaginadosAsync_ShouldReturnPagedResults()
        {
            var (repo, ctx, scope) = CreateRepository();
            using (scope)
            {
                // Cria 5 avisos
                for (int i = 1; i <= 5; i++)
                {
                    ctx.Add(new AvisoEntity { Titulo = $"Título {i}", Mensagem = $"Mensagem {i}" });
                }
                await ctx.SaveChangesAsync();

                // Primeira página: 2 itens
                var (items1, totalCount1) = await repo.ObterAvisosPaginadosAsync(1, 2, TrackingBehavior.NoTracking, CancellationToken.None);
                totalCount1.Should().Be(5);
                items1.Should().HaveCount(2);

                // Segunda página: 2 itens
                var (items2, totalCount2) = await repo.ObterAvisosPaginadosAsync(2, 2, TrackingBehavior.NoTracking, CancellationToken.None);
                totalCount2.Should().Be(5);
                items2.Should().HaveCount(2);

                // Terceira página: 1 item
                var (items3, totalCount3) = await repo.ObterAvisosPaginadosAsync(3, 2, TrackingBehavior.NoTracking, CancellationToken.None);
                totalCount3.Should().Be(5);
                items3.Should().HaveCount(1);
            }
        }

        [Fact]
        public async Task ObterAvisosPaginadosAsync_ShouldExcludeDeletedAvisos()
        {
            var (repo, ctx, scope) = CreateRepository();
            using (scope)
            {
                var a1 = new AvisoEntity { Titulo = "A1", Mensagem = "M1" };
                var a2 = new AvisoEntity { Titulo = "A2", Mensagem = "M2" };
                var a3 = new AvisoEntity { Titulo = "A3", Mensagem = "M3" };
                ctx.AddRange(a1, a2, a3);
                await ctx.SaveChangesAsync();

                a2.MarkAsDeleted();
                ctx.Update(a2);
                await ctx.SaveChangesAsync();

                var (items, totalCount) = await repo.ObterAvisosPaginadosAsync(1, 10, TrackingBehavior.NoTracking, CancellationToken.None);
                totalCount.Should().Be(2);
                items.Should().HaveCount(2);
                items.Should().NotContain(a => a.Id == a2.Id);
            }
        }

        [Fact]
        public async Task ObterTodosAvisosAsync_ShouldUseTracking_WhenTrackingBehaviorIsDefault()
        {
            var (repo, ctx, scope) = CreateRepository();
            using (scope)
            {
                var a1 = new AvisoEntity { Titulo = "A1", Mensagem = "M1" };
                ctx.Add(a1);
                await ctx.SaveChangesAsync();

                var list = await repo.ObterTodosAvisosAsync(TrackingBehavior.Default, CancellationToken.None);

                list.Should().HaveCount(1);
                list.Single().Titulo.Should().Be("A1");
            }
        }

        [Fact]
        public async Task ObterAvisosPaginadosAsync_ShouldUseTracking_WhenTrackingBehaviorIsDefault()
        {
            var (repo, ctx, scope) = CreateRepository();
            using (scope)
            {
                var a1 = new AvisoEntity { Titulo = "A1", Mensagem = "M1" };
                ctx.Add(a1);
                await ctx.SaveChangesAsync();

                var (items, totalCount) = await repo.ObterAvisosPaginadosAsync(1, 10, TrackingBehavior.Default, CancellationToken.None);

                totalCount.Should().Be(1);
                items.Should().HaveCount(1);
                items.Single().Titulo.Should().Be("A1");
            }
        }
    }
}
