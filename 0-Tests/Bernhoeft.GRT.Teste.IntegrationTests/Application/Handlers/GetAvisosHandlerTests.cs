using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Bernhoeft.GRT.ContractWeb.Domain.SqlServer.ContractStore.Entities;
using Bernhoeft.GRT.ContractWeb.Domain.SqlServer.ContractStore.Interfaces.Repositories;
using Bernhoeft.GRT.Core.EntityFramework.Domain.Interfaces;
using Bernhoeft.GRT.Core.Interfaces.Results;
using Bernhoeft.GRT.Teste.Application.Handlers.Queries.v1;
using Bernhoeft.GRT.Teste.Application.Requests.Queries.v1;
using Bernhoeft.GRT.Teste.Application.Responses.Queries.v1;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;
using Bernhoeft.GRT.Core.Enums;

namespace Bernhoeft.GRT.Teste.IntegrationTests.Application.Handlers
{
 public class GetAvisosHandlerTests
 {
 private (GetAvisosHandler handler, Mock<IAvisoRepository> repoMock) CreateSut(List<AvisoEntity> seed, int totalCount = 0)
 {
 var services = new ServiceCollection();
 var repoMock = new Mock<IAvisoRepository>();
 if (totalCount == 0)
 totalCount = seed.Count;
 repoMock.Setup(r => r.ObterAvisosPaginadosAsync(It.IsAny<int>(), It.IsAny<int>(), It.Is<TrackingBehavior>(t => t == TrackingBehavior.NoTracking), It.IsAny<CancellationToken>()))
 .ReturnsAsync((seed, totalCount));
 services.AddSingleton(repoMock.Object);
 var contextMock = new Mock<IContext>();
 services.AddSingleton<IContext>(contextMock.Object);
 var sp = services.BuildServiceProvider();
 var handler = new GetAvisosHandler(sp);
 return (handler, repoMock);
 }

 [Fact]
 public async Task Handle_ShouldReturnNoContent_WhenRepositoryReturnsEmpty()
 {
 var (handler, _) = CreateSut(new List<AvisoEntity>(), 0);
 IOperationResult<PagedResult<GetAvisosResponse>> result = await handler.Handle(new GetAvisosRequest(), CancellationToken.None);
 ((int)result.StatusCode).Should().Be(204);
 result.Data.Should().BeNull();
 }

    [Fact]
    public async Task Handle_ShouldReturnOk_WithMappedResponses_WhenRepositoryHasData()
    {
        var seed = new List<AvisoEntity>
        {
            new AvisoEntity { Titulo = "A1", Mensagem = "M1", Ativo = true },
            new AvisoEntity { Titulo = "A2", Mensagem = "M2", Ativo = false }
        };
        var (handler, _) = CreateSut(seed, 2);
        IOperationResult<PagedResult<GetAvisosResponse>> result = await handler.Handle(new GetAvisosRequest(1, 10), CancellationToken.None);
        ((int)result.StatusCode).Should().Be(200);
        result.Data.Should().NotBeNull();
        result.Data!.TotalCount.Should().Be(2);
        result.Data.Page.Should().Be(1);
        result.Data.PageSize.Should().Be(10);
        result.Data.TotalPages.Should().Be(1);
        result.Data.HasPreviousPage.Should().BeFalse();
        result.Data.HasNextPage.Should().BeFalse();
        var list = result.Data.Data.ToList();
        list.Should().HaveCount(2);
        list[0].Titulo.Should().Be("A1");
        list[0].Ativo.Should().BeTrue();
        list[1].Titulo.Should().Be("A2");
        list[1].Ativo.Should().BeFalse();
    }

    [Fact]
    public async Task Handle_ShouldSetPaginationProperties_WhenMultiplePages()
    {
        var seed = new List<AvisoEntity>
        {
            new AvisoEntity { Titulo = "A1", Mensagem = "M1" }
        };
        var (handler, _) = CreateSut(seed, 25);
        IOperationResult<PagedResult<GetAvisosResponse>> result = await handler.Handle(new GetAvisosRequest(2, 10), CancellationToken.None);
        
        result.Data.Should().NotBeNull();
        result.Data!.TotalCount.Should().Be(25);
        result.Data.Page.Should().Be(2);
        result.Data.PageSize.Should().Be(10);
        result.Data.TotalPages.Should().Be(3);
        result.Data.HasPreviousPage.Should().BeTrue();
        result.Data.HasNextPage.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_ShouldUseCorrectPaginationParameters()
    {
        var seed = new List<AvisoEntity>
        {
            new AvisoEntity { Titulo = "A1", Mensagem = "M1" }
        };
        var (handler, repoMock) = CreateSut(seed, 1);
        
        var request = new GetAvisosRequest(2, 5);
        await handler.Handle(request, CancellationToken.None);

        repoMock.Verify(r => r.ObterAvisosPaginadosAsync(2, 5, TrackingBehavior.NoTracking, It.IsAny<CancellationToken>()), Times.Once);
    }
}
}
