import React, { useState, useEffect, useRef, useCallback } from 'react';
import './App.css';
import AvisosBoard from './components/AvisosBoard';
import CreateAvisoModal from './components/CreateAvisoModal';
import { Aviso } from './types/Aviso';
import { avisoService } from './services/avisoService';
import { signalRService } from './services/signalRService';

function App() {
  const [avisos, setAvisos] = useState<Aviso[]>([]);
  const [loading, setLoading] = useState(true);
  const [showCreateModal, setShowCreateModal] = useState(false);
  const [pageSize] = useState(50); // Buscar muitos para o board
  const [isConnected, setIsConnected] = useState(false);
  const signalRInitialized = useRef(false);

  const loadAvisos = React.useCallback(async () => {
    try {
      setLoading(true);
      const response = await avisoService.getAvisos(1, pageSize);
      if (response && response.Data && response.Data.Data) {
        // Ordenar por ID decrescente (mais novo primeiro)
        const sortedAvisos = [...response.Data.Data].sort((a, b) => b.Id - a.Id);
        setAvisos(sortedAvisos);
      } else {
        setAvisos([]);
      }
    } catch (error) {
      console.error('Erro ao carregar avisos:', error);
      setAvisos([]);
    } finally {
      setLoading(false);
    }
  }, [pageSize]);

  // Efeito separado para carregar avisos
  useEffect(() => {
    loadAvisos();
  }, [loadAvisos]);

  // Ref para rastrear se o componente ainda está montado
  const isMountedRef = useRef(true);

  // Callbacks usando useCallback - setAvisos é estável e não precisa estar nas dependências
  const handleAvisoCriado = useCallback((aviso: Aviso) => {
    console.log('=== onAvisoCriado CHAMADO ===');
    console.log('Aviso recebido:', aviso);
    console.log('Tipo:', typeof aviso);
    console.log('Chaves:', aviso ? Object.keys(aviso) : 'null/undefined');
    
    // Normalizar novamente para garantir (pode vir com propriedades em minúsculas)
    const avisoAny = aviso as any;
    const avisoId = aviso?.Id ?? avisoAny?.id ?? 0;
    
    if (!aviso || !avisoId || avisoId === 0) {
      console.warn('Aviso inválido recebido:', aviso);
      return;
    }

    // Garantir que o aviso tem todos os campos necessários
    const avisoCompleto: Aviso = {
      Id: avisoId,
      Titulo: aviso.Titulo ?? avisoAny?.titulo ?? 'Sem título',
      Mensagem: aviso.Mensagem ?? avisoAny?.mensagem ?? '',
      Ativo: aviso.Ativo ?? avisoAny?.ativo ?? true
    };
    
    console.log('Aviso completo preparado:', avisoCompleto);
    
    // Atualizar estado diretamente usando função callback (React garante que setAvisos é estável)
    setAvisos(prev => {
      console.log('=== DENTRO DO setAvisos ===');
      console.log('Estado anterior:', prev);
      console.log('Tamanho do array anterior:', prev.length);
      
      // Verificar se já existe (evitar duplicatas)
      const existe = prev.find(a => a && a.Id === avisoCompleto.Id);
      if (existe) {
        console.log('Aviso já existe, ignorando duplicata. ID:', avisoCompleto.Id);
        return prev;
      }
      
      const novoEstado = [avisoCompleto, ...prev].sort((a, b) => b.Id - a.Id);
      console.log('Novo estado criado:', novoEstado);
      console.log('Tamanho do novo array:', novoEstado.length);
      console.log('=== FIM DO setAvisos ===');
      return novoEstado;
    });
    
    console.log('setAvisos chamado');
  }, []);

  const handleAvisoAtualizado = useCallback((data: { Id?: number; Mensagem?: string; id?: number; mensagem?: string } | any) => {
    console.log('=== onAvisoAtualizado CHAMADO ===');
    console.log('Data recebido (raw):', data);
    console.log('Tipo:', typeof data);
    console.log('Chaves:', data ? Object.keys(data) : 'null/undefined');
    
    // Normalizar PRIMEIRO (pode vir com propriedades em minúsculas)
    // Aceitar tanto maiúsculas quanto minúsculas
    const dataAny = data as any;
    const dataId = data?.Id ?? data?.id ?? dataAny?.Id ?? dataAny?.id ?? 0;
    const dataMensagem = data?.Mensagem ?? data?.mensagem ?? dataAny?.Mensagem ?? dataAny?.mensagem ?? '';
    
    console.log('Dados normalizados - ID:', dataId, 'Mensagem:', dataMensagem);
    
    if (!data || !dataId || dataId === 0) {
      console.warn('Dados de atualização inválidos:', data);
      return;
    }

    if (!dataMensagem || dataMensagem.trim() === '') {
      console.warn('Mensagem vazia na atualização:', data);
      return;
    }

    console.log('ID para atualizar:', dataId);
    console.log('Nova mensagem:', dataMensagem);

    // Atualizar aviso existente
    setAvisos(prev => {
      console.log('=== DENTRO DO setAvisos (update) ===');
      console.log('Estado anterior:', prev);
      console.log('Tamanho do array anterior:', prev.length);
      console.log('Buscando aviso com ID:', dataId);
      
      const avisoEncontrado = prev.find(aviso => aviso.Id === dataId);
      if (!avisoEncontrado) {
        console.warn('Aviso não encontrado para atualizar. ID:', dataId);
        return prev;
      }
      
      console.log('Aviso encontrado para atualizar:', avisoEncontrado);
      
      const novoEstado = prev.map(aviso => {
        if (aviso.Id === dataId) {
          const avisoAtualizado = { ...aviso, Mensagem: dataMensagem };
          console.log('Aviso atualizado:', avisoAtualizado);
          return avisoAtualizado;
        }
        return aviso;
      });
      
      console.log('Estado após atualização:', novoEstado);
      console.log('Tamanho do novo array:', novoEstado.length);
      console.log('=== FIM DO setAvisos (update) ===');
      return novoEstado;
    });
    
    console.log('setAvisos chamado para atualizar');
  }, []);

  const handleAvisoDeletado = useCallback((data: { Id: number }) => {
    console.log('=== onAvisoDeletado CHAMADO ===');
    console.log('Data recebido:', data);
    console.log('Tipo:', typeof data);
    console.log('Chaves:', data ? Object.keys(data) : 'null/undefined');
    
    // Normalizar novamente para garantir (pode vir com propriedades em minúsculas)
    const dataAny = data as any;
    const dataId = data?.Id ?? dataAny?.id ?? 0;
    
    if (!data || !dataId || dataId === 0) {
      console.warn('Dados de exclusão inválidos:', data);
      return;
    }

    console.log('ID para deletar:', dataId);

    // Remover aviso do board
    setAvisos(prev => {
      console.log('=== DENTRO DO setAvisos (delete) ===');
      console.log('Estado anterior:', prev);
      console.log('Tamanho do array anterior:', prev.length);
      console.log('Buscando aviso com ID:', dataId);
      
      const novoEstado = prev.filter(aviso => {
        const match = aviso.Id !== dataId;
        if (!match) {
          console.log('Removendo aviso com ID:', aviso.Id);
        }
        return match;
      });
      
      console.log('Estado após exclusão:', novoEstado);
      console.log('Tamanho do novo array:', novoEstado.length);
      console.log('=== FIM DO setAvisos (delete) ===');
      return novoEstado;
    });
    
    console.log('setAvisos chamado para deletar');
  }, []);

  // Efeito separado para SignalR (sem dependências que mudam)
  useEffect(() => {
    isMountedRef.current = true;
    let cleanupTimer: NodeJS.Timeout | null = null;
    
    // Aguardar um pouco para garantir que não é um remount do StrictMode
    const initTimer = setTimeout(() => {
      // Verificar se ainda está montado antes de iniciar
      if (!isMountedRef.current || signalRInitialized.current) {
        return;
      }

      signalRInitialized.current = true;

      const startSignalR = async () => {
        // Verificar novamente antes de iniciar
        if (!isMountedRef.current) {
          return;
        }

        await signalRService.start({
          onConnected: (message) => {
            if (isMountedRef.current) {
              console.log('SignalR conectado:', message);
              setIsConnected(true);
            }
          },
          onError: (error) => {
            if (isMountedRef.current) {
              console.error('Erro no SignalR:', error);
              setIsConnected(false);
            }
          },
          onAvisoCriado: handleAvisoCriado,
          onAvisoAtualizado: handleAvisoAtualizado,
          onAvisoDeletado: handleAvisoDeletado
        });
      };

      startSignalR();
    }, 100); // Pequeno delay para evitar race condition do StrictMode

    // Cleanup ao desmontar
    return () => {
      isMountedRef.current = false;
      clearTimeout(initTimer);
      
      if (cleanupTimer) {
        clearTimeout(cleanupTimer);
      }
      
      // Aguardar um pouco antes de parar para garantir que não é um remount do StrictMode
      cleanupTimer = setTimeout(() => {
        // Só para se realmente estiver desmontando (após um tempo)
        if (!isMountedRef.current) {
          signalRService.stop().catch((err) => {
            // Ignora erros no cleanup (a conexão pode já estar desconectada)
            console.warn('Erro durante cleanup do SignalR (pode ser ignorado):', err);
          });
        }
      }, 200);
    };
  }, [handleAvisoCriado, handleAvisoAtualizado, handleAvisoDeletado]); // Dependências dos callbacks

  const handleCreateAviso = async (titulo: string, mensagem: string) => {
    try {
      await avisoService.createAviso(titulo, mensagem);
      // SignalR vai notificar automaticamente todos os clientes conectados
      setShowCreateModal(false);
    } catch (error) {
      console.error('Erro ao criar aviso:', error);
      throw error;
    }
  };

  const handleUpdateAviso = async (id: number, mensagem: string) => {
    try {
      await avisoService.updateAviso(id, mensagem);
      // Não precisa recarregar - SignalR vai notificar automaticamente
    } catch (error) {
      console.error('Erro ao atualizar aviso:', error);
      throw error;
    }
  };

  const handleDeleteAviso = async (id: number) => {
    try {
      await avisoService.deleteAviso(id);
      // Não precisa recarregar - SignalR vai notificar automaticamente
    } catch (error) {
      console.error('Erro ao deletar aviso:', error);
      throw error;
    }
  };

  return (
    <div className="App">
      <header className="App-header">
        <div>
          <h1>📋 Board de Avisos</h1>
          <div className="connection-status">
            <span className={`status-indicator ${isConnected ? 'connected' : 'disconnected'}`}>
              {isConnected ? '🟢 Conectado' : '🔴 Desconectado'}
            </span>
          </div>
        </div>
        <button 
          className="btn-create" 
          onClick={() => setShowCreateModal(true)}
        >
          + Novo Aviso
        </button>
      </header>

      {loading ? (
        <div className="loading">Carregando avisos...</div>
      ) : (
        <AvisosBoard
          avisos={avisos}
          onUpdate={handleUpdateAviso}
          onDelete={handleDeleteAviso}
        />
      )}

      {showCreateModal && (
        <CreateAvisoModal
          onClose={() => setShowCreateModal(false)}
          onCreate={handleCreateAviso}
        />
      )}
    </div>
  );
}

export default App;

