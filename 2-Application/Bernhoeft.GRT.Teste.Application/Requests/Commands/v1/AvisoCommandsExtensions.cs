using MediatR;

namespace Bernhoeft.GRT.Teste.Application.Requests.Commands.v1
{
 public class GetAvisoByIdRequest : IRequest<object>
 {
 public int Id { get; set; }
 }

 public class UpdateAvisoCommand : IRequest<object>
 {
 public int Id { get; set; }
 public string Mensagem { get; set; }
 }

 public class DeleteAvisoCommand : IRequest<bool>
 {
 public int Id { get; set; }
 }
}