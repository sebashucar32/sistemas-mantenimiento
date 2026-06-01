export interface LoginResponse {
  token: string;
  expiraEn: string;
  usuario: {
    id: number;
    nombreUsuario: string;
    nombreCompleto: string;
  };
}
