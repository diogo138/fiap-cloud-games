namespace FIAP.FCG.Application.Autenticacao.Dtos
{
	public record ContextoAtual
	{

		public int Usuario { get; }

		public string Email { get; }

		public bool Administrador { get; }

		public ContextoAtual(int usuario, string email, bool administrador)
		{
			Usuario = usuario;
			Email = email;
			Administrador = administrador;
		}

	}
}
