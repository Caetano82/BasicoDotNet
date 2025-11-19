using FluentAssertions;
using Microsoft.AspNetCore.SignalR;
using Xunit;
using Bernhoeft.GRT.Teste.Api.Hubs;
using System.Reflection;

namespace Bernhoeft.GRT.Teste.IntegrationTests.Hubs
{
    public class AvisosHubTests
    {
        [Fact]
        public void AvisosHub_Should_BeInstantiable()
        {
            // Arrange & Act
            var hub = new AvisosHub();

            // Assert
            hub.Should().NotBeNull();
            hub.Should().BeAssignableTo<Hub>();
        }

        [Fact]
        public void AvisosHub_Should_HaveJoinBoardMethod()
        {
            // Arrange
            var hubType = typeof(AvisosHub);

            // Act
            var method = hubType.GetMethod("JoinBoard", BindingFlags.Public | BindingFlags.Instance);

            // Assert
            method.Should().NotBeNull();
            method!.ReturnType.Should().Be(typeof(Task));
        }

        [Fact]
        public void AvisosHub_Should_HaveLeaveBoardMethod()
        {
            // Arrange
            var hubType = typeof(AvisosHub);

            // Act
            var method = hubType.GetMethod("LeaveBoard", BindingFlags.Public | BindingFlags.Instance);

            // Assert
            method.Should().NotBeNull();
            method!.ReturnType.Should().Be(typeof(Task));
        }

        [Fact]
        public void AvisosHub_Should_OverrideOnDisconnectedAsync()
        {
            // Arrange
            var hubType = typeof(AvisosHub);
            var baseMethod = typeof(Hub).GetMethod("OnDisconnectedAsync", BindingFlags.Public | BindingFlags.Instance);

            // Act
            var method = hubType.GetMethod("OnDisconnectedAsync", BindingFlags.Public | BindingFlags.Instance);

            // Assert
            method.Should().NotBeNull();
            method!.ReturnType.Should().Be(typeof(Task));
            method.Should().NotBeSameAs(baseMethod); // Deve ser um override
        }

        [Fact]
        public void AvisosHub_JoinBoard_Should_BeAsync()
        {
            // Arrange
            var hubType = typeof(AvisosHub);
            var method = hubType.GetMethod("JoinBoard", BindingFlags.Public | BindingFlags.Instance);

            // Assert
            method.Should().NotBeNull();
            method!.ReturnType.Should().Be(typeof(Task));
        }

        [Fact]
        public void AvisosHub_LeaveBoard_Should_BeAsync()
        {
            // Arrange
            var hubType = typeof(AvisosHub);
            var method = hubType.GetMethod("LeaveBoard", BindingFlags.Public | BindingFlags.Instance);

            // Assert
            method.Should().NotBeNull();
            method!.ReturnType.Should().Be(typeof(Task));
        }

        [Fact]
        public void AvisosHub_OnDisconnectedAsync_Should_AcceptException()
        {
            // Arrange
            var hubType = typeof(AvisosHub);
            var method = hubType.GetMethod("OnDisconnectedAsync", BindingFlags.Public | BindingFlags.Instance);

            // Assert
            method.Should().NotBeNull();
            var parameters = method!.GetParameters();
            parameters.Should().HaveCount(1);
            parameters[0].ParameterType.Should().Be(typeof(Exception));
        }

        [Fact]
        public void AvisosHub_Should_InheritFromHub()
        {
            // Arrange & Act
            var hubType = typeof(AvisosHub);
            var baseType = hubType.BaseType;

            // Assert
            baseType.Should().Be(typeof(Hub));
        }

        [Fact]
        public void AvisosHub_Should_BeInHubsNamespace()
        {
            // Arrange & Act
            var hubType = typeof(AvisosHub);

            // Assert
            hubType.Namespace.Should().Be("Bernhoeft.GRT.Teste.Api.Hubs");
        }
    }
}