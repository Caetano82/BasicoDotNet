using System;
using FluentAssertions;
using Xunit;
using Bernhoeft.GRT.ContractWeb.Domain.SqlServer.ContractStore.Entities;

namespace Bernhoeft.GRT.Teste.IntegrationTests.Domain
{
 public class AvisoEntityTests
 {
 [Fact]
 public void Constructor_ShouldInitializeDefaults()
 {
 var e = new AvisoEntity();
 e.Id.Should().Be(0); // not persisted yet
 e.Ativo.Should().BeTrue();
 e.IsDeleted.Should().BeFalse();
 e.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(2));
 e.UpdatedAt.Should().BeNull();
 }

 [Fact]
 public void MarkAsDeleted_ShouldSetFlagsAndTimestamp()
 {
 var e = new AvisoEntity();
 e.MarkAsDeleted();
 e.IsDeleted.Should().BeTrue();
 e.UpdatedAt.Should().NotBeNull();
 e.UpdatedAt.Should().BeAfter(e.CreatedAt);
 }

 [Fact]
 public void UpdateMessage_ShouldChangeMessageAndTimestamp()
 {
 var e = new AvisoEntity { Mensagem = "Old" };
 var before = e.UpdatedAt;
 e.UpdateMessage("New message");
 e.Mensagem.Should().Be("New message");
 e.UpdatedAt.Should().NotBe(before);
 e.UpdatedAt.Should().NotBeNull();
 }

    [Fact]
    public void MarkAsDeleted_AfterUpdateMessage_ShouldKeepDeletedFlag()
    {
        var e = new AvisoEntity { Mensagem = "X" };
        e.UpdateMessage("Y");
        var tsAfterUpdate = e.UpdatedAt;
        e.MarkAsDeleted();
        e.IsDeleted.Should().BeTrue();
        e.UpdatedAt.Should().NotBeNull();
        e.UpdatedAt.Should().BeAfter(tsAfterUpdate!.Value);
    }

    [Fact]
    public void UpdateTituloEMensagem_ShouldUpdateBothFieldsAndTimestamp()
    {
        var e = new AvisoEntity { Titulo = "Título Antigo", Mensagem = "Mensagem Antiga" };
        var before = e.UpdatedAt;
        e.UpdateTituloEMensagem("Novo Título", "Nova Mensagem");
        e.Titulo.Should().Be("Novo Título");
        e.Mensagem.Should().Be("Nova Mensagem");
        e.UpdatedAt.Should().NotBe(before);
        e.UpdatedAt.Should().NotBeNull();
        e.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(2));
    }
}
}
