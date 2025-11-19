using System;
using FluentAssertions;
using Xunit;
using Bernhoeft.GRT.Teste.Application.Requests.Commands.v1;
using Bernhoeft.GRT.Teste.Application.Validators.Commands.v1;

namespace Bernhoeft.GRT.Teste.IntegrationTests.Application.Validators
{
 public class AvisoValidatorsTests
 {
 [Theory]
 [InlineData(null)]
 [InlineData("")]
 public void CreateAvisoValidator_Should_Fail_When_Titulo_Invalid(string? titulo)
 {
 var v = new CreateAvisoValidator();
 var r = v.Validate(new CreateAvisoRequest { Titulo = titulo!, Mensagem = "ok" });
 r.IsValid.Should().BeFalse();
 r.Errors.Should().Contain(e => e.PropertyName == nameof(CreateAvisoRequest.Titulo)
 && e.ErrorMessage == "O título é obrigatório.");
 }

 [Theory]
 [InlineData(null)]
 [InlineData("")]
 public void CreateAvisoValidator_Should_Fail_When_Mensagem_Invalid(string? msg)
 {
 var v = new CreateAvisoValidator();
 var r = v.Validate(new CreateAvisoRequest { Titulo = "ok", Mensagem = msg! });
 r.IsValid.Should().BeFalse();
 r.Errors.Should().Contain(e => e.PropertyName == nameof(CreateAvisoRequest.Mensagem)
 && e.ErrorMessage == "A mensagem é obrigatória.");
 }

 [Fact]
 public void CreateAvisoValidator_Should_Pass_When_Valid()
 {
 var v = new CreateAvisoValidator();
 var r = v.Validate(new CreateAvisoRequest { Titulo = "t", Mensagem = "m" });
 r.IsValid.Should().BeTrue();
 }

 [Theory]
 [InlineData(null)]
 [InlineData("")]
 public void UpdateAvisoValidator_Should_Fail_When_Mensagem_Invalid(string? msg)
 {
 var v = new UpdateAvisoValidator();
 var r = v.Validate(new UpdateAvisoRequest { Mensagem = msg! });
 r.IsValid.Should().BeFalse();
 r.Errors.Should().Contain(e => e.PropertyName == nameof(UpdateAvisoRequest.Mensagem)
 && e.ErrorMessage == "A mensagem é obrigatória.");
 }

    [Fact]
    public void UpdateAvisoValidator_Should_Pass_When_Valid()
    {
        var v = new UpdateAvisoValidator();
        var r = v.Validate(new UpdateAvisoRequest { Mensagem = "m" });
        r.IsValid.Should().BeTrue();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-5)]
    public void GetAvisoByIdRequestValidator_Should_Fail_When_Id_Invalid(int id)
    {
        var v = new GetAvisoByIdRequestValidator();
        var r = v.Validate(new GetAvisoByIdRequest { Id = id });
        r.IsValid.Should().BeFalse();
        r.Errors.Should().Contain(e => e.PropertyName == nameof(GetAvisoByIdRequest.Id)
            && e.ErrorMessage == "O ID deve ser maior que zero.");
    }

    [Fact]
    public void GetAvisoByIdRequestValidator_Should_Pass_When_Valid()
    {
        var v = new GetAvisoByIdRequestValidator();
        var r = v.Validate(new GetAvisoByIdRequest { Id = 1 });
        r.IsValid.Should().BeTrue();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-5)]
    public void DeleteAvisoCommandValidator_Should_Fail_When_Id_Invalid(int id)
    {
        var v = new DeleteAvisoCommandValidator();
        var r = v.Validate(new DeleteAvisoCommand { Id = id });
        r.IsValid.Should().BeFalse();
        r.Errors.Should().Contain(e => e.PropertyName == nameof(DeleteAvisoCommand.Id)
            && e.ErrorMessage == "O ID deve ser maior que zero.");
    }

    [Fact]
    public void DeleteAvisoCommandValidator_Should_Pass_When_Valid()
    {
        var v = new DeleteAvisoCommandValidator();
        var r = v.Validate(new DeleteAvisoCommand { Id = 1 });
        r.IsValid.Should().BeTrue();
    }
}
}
