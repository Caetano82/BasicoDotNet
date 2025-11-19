using MediatR;
using Bernhoeft.GRT.Teste.Application.Responses.Commands.v1;

namespace Bernhoeft.GRT.Teste.Application.Requests.Commands.v1
{
 public class CreateAvisoRequest : IRequest<CreateAvisoResponse>
 {
 public string Titulo { get; set; }
 public string Mensagem { get; set; }
 }

 public class UpdateAvisoRequest
 {
 public string Mensagem { get; set; }
 }
}