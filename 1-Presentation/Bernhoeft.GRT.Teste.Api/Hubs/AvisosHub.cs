using Microsoft.AspNetCore.SignalR;

namespace Bernhoeft.GRT.Teste.Api.Hubs
{
    /// <summary>
    /// Hub SignalR para notificações em tempo real de avisos
    /// </summary>
    public class AvisosHub : Hub
    {
        public async Task JoinBoard()
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, "Board");
            await Clients.Caller.SendAsync("Connected", "Conectado ao board de avisos");
        }

        public async Task LeaveBoard()
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, "Board");
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, "Board");
            await base.OnDisconnectedAsync(exception);
        }
    }
}

