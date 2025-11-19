import React, { useState } from 'react';
import './PostItCard.css';
import { Aviso } from '../types/Aviso';

interface PostItCardProps {
  aviso: Aviso;
  onUpdate: (id: number, mensagem: string) => Promise<void>;
  onDelete: (id: number) => Promise<void>;
}

const PostItCard: React.FC<PostItCardProps> = ({ aviso, onUpdate, onDelete }) => {
  // Hooks devem ser chamados antes de qualquer return condicional
  const [isEditing, setIsEditing] = useState(false);
  const [editedMensagem, setEditedMensagem] = useState(aviso?.Mensagem || '');
  const [isDeleting, setIsDeleting] = useState(false);

  // Cores diferentes para os post-its (rotaciona entre 6 cores)
  const colors = [
    { bg: '#ffeb3b', shadow: '#fbc02d' },
    { bg: '#ff9800', shadow: '#f57c00' },
    { bg: '#4caf50', shadow: '#388e3c' },
    { bg: '#2196f3', shadow: '#1976d2' },
    { bg: '#9c27b0', shadow: '#7b1fa2' },
    { bg: '#f44336', shadow: '#d32f2f' }
  ];
  
  // Proteção contra avisos inválidos
  if (!aviso || !aviso.Id || typeof aviso.Id !== 'number' || aviso.Id <= 0) {
    console.warn('PostItCard: Aviso inválido recebido:', aviso);
    return null;
  }

  // Garantir que Id existe e é um número válido
  const avisoId = aviso.Id;
  const colorIndex = Math.abs(avisoId) % colors.length;
  const color = colors[colorIndex] ?? colors[0]; // Fallback garantido

  const handleEdit = () => {
    setIsEditing(true);
  };

  const handleSave = async () => {
    if (editedMensagem.trim() === '') {
      alert('A mensagem não pode estar vazia!');
      return;
    }

    if (!aviso?.Id) {
      alert('Erro: ID do aviso não encontrado.');
      return;
    }

    try {
      await onUpdate(aviso.Id, editedMensagem);
      setIsEditing(false);
    } catch (error) {
      alert('Erro ao atualizar aviso. Tente novamente.');
    }
  };

  const handleCancel = () => {
    setEditedMensagem(aviso.Mensagem || '');
    setIsEditing(false);
  };

  const handleDelete = async () => {
    if (!aviso?.Id) {
      alert('Erro: ID do aviso não encontrado.');
      return;
    }

    if (!window.confirm('Tem certeza que deseja excluir este aviso?')) {
      return;
    }

    try {
      setIsDeleting(true);
      await onDelete(aviso.Id);
    } catch (error) {
      alert('Erro ao excluir aviso. Tente novamente.');
      setIsDeleting(false);
    }
  };

  return (
    <div 
      className="postit-card" 
      style={{ 
        '--postit-bg': color.bg,
        '--postit-shadow': color.shadow
      } as React.CSSProperties}
    >
      <div className="postit-header">
        <h3 className="postit-title">{aviso.Titulo || 'Sem título'}</h3>
        <div className="postit-actions">
          {!isEditing && (
            <>
              <button 
                className="btn-edit" 
                onClick={handleEdit}
                title="Editar"
              >
                ✏️
              </button>
              <button 
                className="btn-delete" 
                onClick={handleDelete}
                disabled={isDeleting}
                title="Excluir"
              >
                {isDeleting ? '⏳' : '🗑️'}
              </button>
            </>
          )}
        </div>
      </div>

      <div className="postit-content">
        {isEditing ? (
          <div className="edit-mode">
            <textarea
              value={editedMensagem}
              onChange={(e) => setEditedMensagem(e.target.value)}
              className="edit-textarea"
              rows={4}
              autoFocus
            />
            <div className="edit-actions">
              <button className="btn-save" onClick={handleSave}>
                Salvar
              </button>
              <button className="btn-cancel" onClick={handleCancel}>
                Cancelar
              </button>
            </div>
          </div>
        ) : (
          <p className="postit-mensagem">{aviso.Mensagem || ''}</p>
        )}
      </div>

      <div className="postit-footer">
        <span className="postit-id">#{avisoId}</span>
        {aviso.Ativo && <span className="postit-status">Ativo</span>}
      </div>
    </div>
  );
};

export default PostItCard;

