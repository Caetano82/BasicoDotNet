export interface Aviso {
  Id: number;
  Titulo: string;
  Mensagem: string;
  Ativo: boolean;
}

export interface CreateAvisoRequest {
  titulo: string;
  mensagem: string;
}

export interface UpdateAvisoRequest {
  mensagem: string;
}

export interface PagedResult<T> {
  Data: T[];
  Page: number;
  PageSize: number;
  TotalCount: number;
  TotalPages: number;
  HasPreviousPage: boolean;
  HasNextPage: boolean;
}

export interface ApiResponse<T> {
  Mensagem: string;
  Data: T;
}

