import React from 'react';
import './AvisosBoard.css';
import PostItCard from './PostItCard';
import { Aviso } from '../types/Aviso';

interface AvisosBoardProps {
  avisos: Aviso[];
  onUpdate: (id: number, mensagem: string) => Promise<void>;
  onDelete: (id: number) => Promise<void>;
}

const AvisosBoard: React.FC<AvisosBoardProps> = ({ avisos, onUpdate, onDelete }) => {
  // Log para debug
  React.useEffect(() => {
    console.log('AvisosBoard renderizado com', avisos.length, 'avisos');
    console.log('IDs dos avisos:', avisos.map(a => a.Id));
  }, [avisos]);

  if (avisos.length === 0) {
    return (
      <div className="board-empty">
        <p>Nenhum aviso encontrado. Crie o primeiro aviso!</p>
      </div>
    );
  }

  // Filtrar e validar avisos antes de renderizar
  const avisosValidos = avisos.filter(aviso => 
    aviso && 
    typeof aviso.Id === 'number' && 
    aviso.Id > 0 &&
    aviso.Titulo !== undefined &&
    aviso.Mensagem !== undefined
  );

  return (
    <div className="avisos-board">
      {avisosValidos.map((aviso) => (
        <PostItCard
          key={aviso.Id}
          aviso={aviso}
          onUpdate={onUpdate}
          onDelete={onDelete}
        />
      ))}
    </div>
  );
};

export default AvisosBoard;

