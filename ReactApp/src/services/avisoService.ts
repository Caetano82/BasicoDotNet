import axios from 'axios';
import { Aviso, CreateAvisoRequest, UpdateAvisoRequest, PagedResult, ApiResponse } from '../types/Aviso';

const API_BASE_URL = process.env.REACT_APP_API_URL || 'https://localhost:5001/api/v1';

const api = axios.create({
  baseURL: API_BASE_URL,
  headers: {
    'Content-Type': 'application/json',
  },
});

// Interceptor para ignorar erros de certificado SSL em desenvolvimento
if (process.env.NODE_ENV === 'development') {
  // Em desenvolvimento, o navegador pode mostrar aviso de certificado
  // O usuário precisa aceitar o certificado manualmente no navegador
}

export const avisoService = {
  async getAvisos(page: number = 1, pageSize: number = 10): Promise<ApiResponse<PagedResult<Aviso>>> {
    const response = await api.get<ApiResponse<PagedResult<Aviso>>>('/avisos', {
      params: { page, pageSize }
    });
    return response.data;
  },

  async getAvisoById(id: number): Promise<ApiResponse<Aviso>> {
    const response = await api.get<ApiResponse<Aviso>>(`/avisos/${id}`);
    return response.data;
  },

  async createAviso(titulo: string, mensagem: string): Promise<Aviso> {
    const request: CreateAvisoRequest = { titulo, mensagem };
    const response = await api.post<Aviso>('/avisos', request);
    return response.data;
  },

  async updateAviso(id: number, mensagem: string): Promise<void> {
    const request: UpdateAvisoRequest = { mensagem };
    await api.put(`/avisos/${id}`, request);
  },

  async deleteAviso(id: number): Promise<void> {
    await api.delete(`/avisos/${id}`);
  }
};

