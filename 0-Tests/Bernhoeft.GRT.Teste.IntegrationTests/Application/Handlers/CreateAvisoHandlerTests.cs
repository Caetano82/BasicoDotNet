using Bernhoeft.GRT.ContractWeb.Domain.SqlServer.ContractStore.Entities;
using Bernhoeft.GRT.ContractWeb.Domain.SqlServer.ContractStore.Interfaces.Repositories;
using Bernhoeft.GRT.Core.EntityFramework.Domain.Interfaces;
using Bernhoeft.GRT.Teste.Application.Handlers.Commands.v1;
using Bernhoeft.GRT.Teste.Application.Requests.Commands.v1;
using Bernhoeft.GRT.Teste.Application.Responses.Commands.v1;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;

namespace Bernhoeft.GRT.Teste.IntegrationTests.Application.Handlers
{
    public class CreateAvisoHandlerTests
    {
        private (CreateAvisoHandler handler, Mock<IAvisoRepository> repoMock) CreateSut()
        {
            var services = new ServiceCollection();
            var repoMock = new Mock<IAvisoRepository>();
            services.AddSingleton(repoMock.Object);
            var contextMock = new Mock<IContext>();
            services.AddSingleton<IContext>(contextMock.Object);
            var sp = services.BuildServiceProvider();
            var handler = new CreateAvisoHandler(sp);
            return (handler, repoMock);
        }

        [Fact]
        public async Task Handle_ShouldReturnCreateAvisoResponse_WhenAvisoIsCreated()
        {
            // Arrange
            var (handler, repoMock) = CreateSut();
            var request = new CreateAvisoRequest { Titulo = "Novo Título", Mensagem = "Nova Mensagem" };
            var createdAviso = new AvisoEntity
            {
                Titulo = request.Titulo,
                Mensagem = request.Mensagem
            };

            repoMock.Setup(r => r.CriarAvisoAsync(request.Titulo, request.Mensagem, It.IsAny<CancellationToken>()))
                    .ReturnsAsync(createdAviso);

            // Act
            var result = await handler.Handle(request, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().BeGreaterThanOrEqualTo(0);
            result.Titulo.Should().Be(request.Titulo);
            result.Mensagem.Should().Be(request.Mensagem);
            repoMock.Verify(r => r.CriarAvisoAsync(request.Titulo, request.Mensagem, It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}

