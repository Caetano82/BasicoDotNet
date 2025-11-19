namespace Bernhoeft.GRT.ContractWeb.Domain.SqlServer.ContractStore.Entities
{
    public partial class AvisoEntity
    {
        public int Id { get; private set; }
        public bool Ativo { get; set; } = true;
        public string Titulo { get; set; }
        public string Mensagem { get; set; }

        // New fields for business requirements
        public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; private set; }
        public bool IsDeleted { get; private set; } = false;

        // Method to mark as deleted (soft delete)
        public void MarkAsDeleted()
        {
            IsDeleted = true;
            UpdatedAt = DateTime.UtcNow;
        }

        // Method to update the message
        public void UpdateMessage(string newMessage)
        {
            Mensagem = newMessage;
            UpdatedAt = DateTime.UtcNow;
        }

        // Method to update both title and message
        public void UpdateTituloEMensagem(string newTitulo, string newMensagem)
        {
            Titulo = newTitulo;
            Mensagem = newMensagem;
            UpdatedAt = DateTime.UtcNow;
        }
    }
}