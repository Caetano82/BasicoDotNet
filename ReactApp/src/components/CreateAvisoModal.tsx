import React, { useState } from 'react';
import './CreateAvisoModal.css';

interface CreateAvisoModalProps {
  onClose: () => void;
  onCreate: (titulo: string, mensagem: string) => Promise<void>;
}

const CreateAvisoModal: React.FC<CreateAvisoModalProps> = ({ onClose, onCreate }) => {
  const [titulo, setTitulo] = useState('');
  const [mensagem, setMensagem] = useState('');
  const [isSubmitting, setIsSubmitting] = useState(false);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();

    if (!titulo.trim()) {
      alert('O título é obrigatório!');
      return;
    }

    if (!mensagem.trim()) {
      alert('A mensagem é obrigatória!');
      return;
    }

    try {
      setIsSubmitting(true);
      await onCreate(titulo.trim(), mensagem.trim());
      setTitulo('');
      setMensagem('');
    } catch (error) {
      alert('Erro ao criar aviso. Tente novamente.');
    } finally {
      setIsSubmitting(false);
    }
  };

  return (
    <div className="modal-overlay" onClick={onClose}>
      <div className="modal-content" onClick={(e) => e.stopPropagation()}>
        <div className="modal-header">
          <h2>Criar Novo Aviso</h2>
          <button className="modal-close" onClick={onClose}>×</button>
        </div>

        <form onSubmit={handleSubmit} className="modal-form">
          <div className="form-group">
            <label htmlFor="titulo">Título *</label>
            <input
              id="titulo"
              type="text"
              value={titulo}
              onChange={(e) => setTitulo(e.target.value)}
              placeholder="Digite o título do aviso"
              disabled={isSubmitting}
              required
            />
          </div>

          <div className="form-group">
            <label htmlFor="mensagem">Mensagem *</label>
            <textarea
              id="mensagem"
              value={mensagem}
              onChange={(e) => setMensagem(e.target.value)}
              placeholder="Digite a mensagem do aviso"
              rows={5}
              disabled={isSubmitting}
              required
            />
          </div>

          <div className="modal-actions">
            <button
              type="button"
              className="btn-cancel"
              onClick={onClose}
              disabled={isSubmitting}
            >
              Cancelar
            </button>
            <button
              type="submit"
              className="btn-submit"
              disabled={isSubmitting || !titulo.trim() || !mensagem.trim()}
            >
              {isSubmitting ? 'Criando...' : 'Criar Aviso'}
            </button>
          </div>
        </form>
      </div>
    </div>
  );
};

export default CreateAvisoModal;

