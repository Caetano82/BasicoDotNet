using Bernhoeft.GRT.ContractWeb.Domain.SqlServer.ContractStore.Entities;
using Bernhoeft.GRT.ContractWeb.Domain.SqlServer.ContractStore.Interfaces.Repositories;
using Bernhoeft.GRT.Core.EntityFramework.Domain.Interfaces;
using Bernhoeft.GRT.Teste.Application.Handlers.Commands.v1;
using Bernhoeft.GRT.Teste.Application.Requests.Commands.v1;
using Bernhoeft.GRT.Teste.Application.Responses.Queries.v1;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;

namespace Bernhoeft.GRT.Teste.IntegrationTests.Application.Handlers
{
    public class GetAvisoByIdHandlerTests
    {
        private (GetAvisoByIdHandler handler, Mock<IAvisoRepository> repoMock) CreateSut()
        {
            var services = new ServiceCollection();
            var repoMock = new Mock<IAvisoRepository>();
            services.AddSingleton(repoMock.Object);
            var contextMock = new Mock<IContext>();
            services.AddSingleton<IContext>(contextMock.Object);
            var sp = services.BuildServiceProvider();
            var handler = new GetAvisoByIdHandler(sp);
            return (handler, repoMock);
        }

        [Fact]
        public async Task Handle_ShouldReturnGetAvisosResponse_WhenAvisoExists()
        {
            // Arrange
            var (handler, repoMock) = CreateSut();
            var request = new GetAvisoByIdRequest { Id = 1 };
            var aviso = new AvisoEntity
            {
                Titulo = "Título Teste",
                Mensagem = "Mensagem Teste",
                Ativo = true
            };

            repoMock.Setup(r => r.ObterAvisoPorIdAsync(request.Id, It.IsAny<CancellationToken>()))
                    .ReturnsAsync(aviso);

            // Act
            var result = await handler.Handle(request, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<GetAvisosResponse>();
            var response = result as GetAvisosResponse;
            response!.Titulo.Should().Be("Título Teste");
            response.Mensagem.Should().Be("Mensagem Teste");
            response.Ativo.Should().BeTrue();
            repoMock.Verify(r => r.ObterAvisoPorIdAsync(request.Id, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldReturnNull_WhenAvisoDoesNotExist()
        {
            // Arrange
            var (handler, repoMock) = CreateSut();
            var request = new GetAvisoByIdRequest { Id = 999 };

            repoMock.Setup(r => r.ObterAvisoPorIdAsync(request.Id, It.IsAny<CancellationToken>()))
                    .ReturnsAsync((AvisoEntity?)null);

            // Act
            var result = await handler.Handle(request, CancellationToken.None);

            // Assert
            result.Should().BeNull();
            repoMock.Verify(r => r.ObterAvisoPorIdAsync(request.Id, It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}

