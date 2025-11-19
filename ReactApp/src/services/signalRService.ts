import * as signalR from '@microsoft/signalr';
import { Aviso } from '../types/Aviso';

const HUB_URL = process.env.REACT_APP_API_URL?.replace('/api/v1', '/hub/avisos') || 'https://localhost:5001/hub/avisos';

export interface SignalRCallbacks {
  onAvisoCriado?: (aviso: Aviso) => void;
  onAvisoAtualizado?: (data: { Id: number; Mensagem: string }) => void;
  onAvisoDeletado?: (data: { Id: number }) => void;
  onConnected?: (message: string) => void;
  onError?: (error: Error) => void;
}

class SignalRService {
  private connection: signalR.HubConnection | null = null;
  private callbacks: SignalRCallbacks = {};
  private isStarting: boolean = false;
  private isStopping: boolean = false;

  async start(callbacks: SignalRCallbacks): Promise<void> {
    // Verificar se está parando antes de tudo
    if (this.isStopping) {
      console.log('SignalR está parando, abortando start()');
      return;
    }

    // Se já está iniciando, aguarda e retorna
    if (this.isStarting) {
      console.log('SignalR já está iniciando, aguardando...');
      // Aguarda até terminar de iniciar (ou timeout)
      let waitCount = 0;
      while (this.isStarting && waitCount < 10 && !this.isStopping) {
        await new Promise(resolve => setTimeout(resolve, 100));
        waitCount++;
      }
      if (this.connection && this.connection.state !== signalR.HubConnectionState.Disconnected) {
        this.callbacks = callbacks; // Atualiza callbacks
        return;
      }
    }

    // Se já está conectado ou conectando, não precisa iniciar novamente
    if (this.connection && this.connection.state !== signalR.HubConnectionState.Disconnected) {
      console.log('SignalR já está conectado ou conectando');
      this.callbacks = callbacks; // Atualiza callbacks
      return;
    }

    this.isStarting = true;
    this.callbacks = callbacks;

    try {
      // Verificar novamente se não está parando
      if (this.isStopping) {
        console.log('SignalR foi marcado para parar durante setup, abortando');
        return;
      }

      // Para conexão anterior se existir e não estiver parando
      if (this.connection && !this.isStopping) {
        try {
          // Não aguarda stop() se estiver parando para evitar bloqueio
          this.connection.stop().catch(() => {
            // Ignora erro
          });
        } catch (error) {
          // Ignora erro ao parar conexão anterior
        }
        this.connection = null;
      }

      // Verificar novamente antes de criar nova conexão
      if (this.isStopping) {
        console.log('SignalR foi marcado para parar antes de criar conexão');
        return;
      }

      this.connection = new signalR.HubConnectionBuilder()
        .withUrl(HUB_URL, {
          skipNegotiation: false,
          transport: signalR.HttpTransportType.WebSockets | signalR.HttpTransportType.LongPolling
        })
        .withAutomaticReconnect({
          nextRetryDelayInMilliseconds: (retryContext) => {
            if (retryContext.elapsedMilliseconds < 60000) {
              // Retry a cada 2 segundos por até 60 segundos
              return 2000;
            }
            // Depois disso, retry a cada 10 segundos
            return 10000;
          }
        })
        .configureLogging(signalR.LogLevel.Information)
        .build();

      // Registrar callbacks
      this.connection.on('AvisoCriado', (aviso: any) => {
        console.log('SignalR: AvisoCriado recebido (raw):', aviso);
        console.log('SignalR: Tipo do aviso:', typeof aviso);
        console.log('SignalR: Chaves do aviso:', aviso ? Object.keys(aviso) : 'null/undefined');
        console.log('SignalR: aviso.Id:', aviso?.Id);
        console.log('SignalR: aviso.id:', aviso?.id);
        console.log('SignalR: isStopping =', this.isStopping);
        console.log('SignalR: callbacks.onAvisoCriado existe?', !!this.callbacks.onAvisoCriado);
        
        if (!this.isStopping && this.callbacks.onAvisoCriado) {
          console.log('SignalR: Chamando callback onAvisoCriado');
          try {
            // Normalizar o objeto recebido (pode vir com propriedades em diferentes casos)
            const avisoNormalizado: Aviso = {
              Id: aviso?.Id ?? aviso?.id ?? 0,
              Titulo: aviso?.Titulo ?? aviso?.titulo ?? '',
              Mensagem: aviso?.Mensagem ?? aviso?.mensagem ?? '',
              Ativo: aviso?.Ativo ?? aviso?.ativo ?? true
            };
            
            console.log('SignalR: Aviso normalizado:', avisoNormalizado);
            
            // Usar setTimeout para garantir que seja executado no próximo tick
            setTimeout(() => {
              if (!this.isStopping) {
                this.callbacks.onAvisoCriado?.(avisoNormalizado);
              }
            }, 0);
          } catch (error) {
            console.error('SignalR: Erro ao chamar onAvisoCriado:', error);
          }
        } else {
          console.log('SignalR: Ignorando AvisoCriado - isStopping:', this.isStopping, 'callback existe:', !!this.callbacks.onAvisoCriado);
        }
      });

      this.connection.on('AvisoAtualizado', (data: any) => {
        console.log('SignalR: AvisoAtualizado recebido (raw):', data);
        console.log('SignalR: Tipo do data:', typeof data);
        console.log('SignalR: Chaves do data:', data ? Object.keys(data) : 'null/undefined');
        console.log('SignalR: data.Id:', data?.Id);
        console.log('SignalR: data.id:', data?.id);
        console.log('SignalR: data.Mensagem:', data?.Mensagem);
        console.log('SignalR: data.mensagem:', data?.mensagem);
        console.log('SignalR: isStopping =', this.isStopping);
        console.log('SignalR: callbacks.onAvisoAtualizado existe?', !!this.callbacks.onAvisoAtualizado);
        
        if (!this.isStopping && this.callbacks.onAvisoAtualizado) {
          console.log('SignalR: Chamando callback onAvisoAtualizado');
          try {
            // Normalizar o objeto recebido (pode vir com propriedades em diferentes casos)
            const dataNormalizado = {
              Id: data?.Id ?? data?.id ?? 0,
              Mensagem: data?.Mensagem ?? data?.mensagem ?? ''
            };
            
            console.log('SignalR: Data normalizado:', dataNormalizado);
            
            // Usar setTimeout para garantir que seja executado no próximo tick
            setTimeout(() => {
              if (!this.isStopping) {
                this.callbacks.onAvisoAtualizado?.(dataNormalizado);
              }
            }, 0);
          } catch (error) {
            console.error('SignalR: Erro ao chamar onAvisoAtualizado:', error);
          }
        } else {
          console.log('SignalR: Ignorando AvisoAtualizado - isStopping:', this.isStopping, 'callback existe:', !!this.callbacks.onAvisoAtualizado);
        }
      });

      this.connection.on('AvisoDeletado', (data: any) => {
        console.log('SignalR: AvisoDeletado recebido (raw):', data);
        console.log('SignalR: Tipo do data:', typeof data);
        console.log('SignalR: Chaves do data:', data ? Object.keys(data) : 'null/undefined');
        console.log('SignalR: data.Id:', data?.Id);
        console.log('SignalR: data.id:', data?.id);
        console.log('SignalR: isStopping =', this.isStopping);
        console.log('SignalR: callbacks.onAvisoDeletado existe?', !!this.callbacks.onAvisoDeletado);
        
        if (!this.isStopping && this.callbacks.onAvisoDeletado) {
          console.log('SignalR: Chamando callback onAvisoDeletado');
          try {
            // Normalizar o objeto recebido (pode vir com propriedades em diferentes casos)
            const dataNormalizado = {
              Id: data?.Id ?? data?.id ?? 0
            };
            
            console.log('SignalR: Data normalizado:', dataNormalizado);
            
            // Usar setTimeout para garantir que seja executado no próximo tick
            setTimeout(() => {
              if (!this.isStopping) {
                this.callbacks.onAvisoDeletado?.(dataNormalizado);
              }
            }, 0);
          } catch (error) {
            console.error('SignalR: Erro ao chamar onAvisoDeletado:', error);
          }
        } else {
          console.log('SignalR: Ignorando AvisoDeletado - isStopping:', this.isStopping, 'callback existe:', !!this.callbacks.onAvisoDeletado);
        }
      });

      this.connection.on('Connected', (message: string) => {
        if (!this.isStopping) {
          console.log('SignalR conectado:', message);
          this.callbacks.onConnected?.(message);
        }
      });

      this.connection.onreconnecting((error) => {
        console.log('SignalR reconectando...', error);
      });

      this.connection.onreconnected((connectionId) => {
        if (!this.isStopping) {
          console.log('SignalR reconectado:', connectionId);
          this.joinBoard();
        }
      });

      this.connection.onclose((error) => {
        console.log('SignalR desconectado:', error);
        if (!this.isStopping) {
          // Se não está parando intencionalmente, pode ser um erro
          this.callbacks.onError?.(new Error('Conexão SignalR fechada'));
        }
      });

      // Verificar uma última vez antes de iniciar
      if (this.isStopping) {
        console.log('SignalR foi marcado para parar antes de start()');
        return;
      }

      await this.connection.start();
      console.log('SignalR conectado com sucesso');
      
      // Verificar novamente antes de entrar no grupo
      if (!this.isStopping && this.connection.state === signalR.HubConnectionState.Connected) {
        await this.joinBoard();
      }
    } catch (error) {
      if (!this.isStopping) {
        console.error('Erro ao conectar SignalR:', error);
        this.callbacks.onError?.(error as Error);
      } else {
        console.log('Erro ao conectar SignalR (já estava parando):', error);
      }
    } finally {
      this.isStarting = false;
    }
  }

  private async joinBoard(): Promise<void> {
    if (this.connection?.state === signalR.HubConnectionState.Connected) {
      try {
        await this.connection.invoke('JoinBoard');
        console.log('Entrou no grupo Board');
      } catch (error) {
        console.error('Erro ao entrar no grupo Board:', error);
      }
    }
  }

  async stop(): Promise<void> {
    // Marca que está parando para evitar que start() continue
    this.isStopping = true;

    if (this.connection) {
      try {
        const connectionState = this.connection.state;

        // Só tenta fazer LeaveBoard se a conexão estiver conectada
        if (connectionState === signalR.HubConnectionState.Connected) {
          try {
            await this.connection.invoke('LeaveBoard');
            console.log('Saiu do grupo Board');
          } catch (error) {
            // Ignora erros ao sair do grupo (pode já ter sido desconectado)
            console.warn('Erro ao sair do grupo Board (pode ser ignorado):', error);
          }
        }
        
        // Para a conexão se não estiver desconectada
        if (connectionState !== signalR.HubConnectionState.Disconnected) {
          try {
            await this.connection.stop();
            console.log('SignalR desconectado');
          } catch (error) {
            // Ignora erros de desconexão (a conexão pode já estar desconectada)
            console.warn('Erro ao parar conexão SignalR (pode ser ignorado):', error);
          }
        }
      } catch (error) {
        // Ignora erros de desconexão
        console.warn('Erro ao desconectar SignalR (pode ser ignorado):', error);
      } finally {
        this.connection = null;
        this.isStopping = false;
      }
    } else {
      this.isStopping = false;
    }
  }

  isConnected(): boolean {
    return this.connection?.state === signalR.HubConnectionState.Connected;
  }
}

export const signalRService = new SignalRService();

