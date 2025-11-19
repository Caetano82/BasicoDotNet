using FluentValidation;
using Bernhoeft.GRT.Teste.Application.Requests.Commands.v1;

namespace Bernhoeft.GRT.Teste.Application.Validators.Commands.v1
{
    public class DeleteAvisoCommandValidator : AbstractValidator<DeleteAvisoCommand>
    {
        public DeleteAvisoCommandValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0)
                .WithMessage("O ID deve ser maior que zero.");
        }
    }
}

