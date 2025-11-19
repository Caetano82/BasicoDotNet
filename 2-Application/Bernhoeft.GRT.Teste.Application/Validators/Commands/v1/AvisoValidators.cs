using FluentValidation;

namespace Bernhoeft.GRT.Teste.Application.Requests.Commands.v1
{
 public class CreateAvisoValidator : AbstractValidator<CreateAvisoRequest>
 {
 public CreateAvisoValidator()
 {
 RuleFor(x => x.Titulo).NotEmpty().WithMessage("O título é obrigatório.");
 RuleFor(x => x.Mensagem).NotEmpty().WithMessage("A mensagem é obrigatória.");
 }
 }

 public class UpdateAvisoValidator : AbstractValidator<UpdateAvisoRequest>
 {
 public UpdateAvisoValidator()
 {
 RuleFor(x => x.Mensagem).NotEmpty().WithMessage("A mensagem é obrigatória.");
 }
 }
}