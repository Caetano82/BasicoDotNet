using Bernhoeft.GRT.ContractWeb.Domain.SqlServer.ContractStore.Interfaces.Repositories;
using Bernhoeft.GRT.Core.EntityFramework.Domain.Interfaces;
using Bernhoeft.GRT.Teste.Application.Handlers.Commands.v1;
using Bernhoeft.GRT.Teste.Application.Requests.Commands.v1;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;

namespace Bernhoeft.GRT.Teste.IntegrationTests.Application.Handlers
{
    public class UpdateAvisoHandlerTests
    {
        private (UpdateAvisoHandler handler, Mock<IAvisoRepository> repoMock) CreateSut()
        {
            var services = new ServiceCollection();
            var repoMock = new Mock<IAvisoRepository>();
            services.AddSingleton(repoMock.Object);
            var contextMock = new Mock<IContext>();
            services.AddSingleton<IContext>(contextMock.Object);
            var sp = services.BuildServiceProvider();
            var handler = new UpdateAvisoHandler(sp);
            return (handler, repoMock);
        }

        [Fact]
        public async Task Handle_ShouldReturnTrue_WhenAvisoIsUpdated()
        {
            // Arrange
            var (handler, repoMock) = CreateSut();
            var request = new UpdateAvisoCommand { Id = 1, Mensagem = "Mensagem Atualizada" };

            repoMock.Setup(r => r.AtualizarAvisoAsync(request.Id, request.Mensagem, It.IsAny<CancellationToken>()))
                    .ReturnsAsync(true);

            // Act
            var result = await handler.Handle(request, CancellationToken.None);

            // Assert
            result.Should().Be(true);
            repoMock.Verify(r => r.AtualizarAvisoAsync(request.Id, request.Mensagem, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldReturnFalse_WhenAvisoDoesNotExist()
        {
            // Arrange
            var (handler, repoMock) = CreateSut();
            var request = new UpdateAvisoCommand { Id = 999, Mensagem = "Mensagem Atualizada" };

            repoMock.Setup(r => r.AtualizarAvisoAsync(request.Id, request.Mensagem, It.IsAny<CancellationToken>()))
                    .ReturnsAsync(false);

            // Act
            var result = await handler.Handle(request, CancellationToken.None);

            // Assert
            result.Should().Be(false);
            repoMock.Verify(r => r.AtualizarAvisoAsync(request.Id, request.Mensagem, It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}

