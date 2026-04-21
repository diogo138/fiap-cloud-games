using FIAP.FCG.Infrastructure.Dados.Entidades;

namespace FIAP.FCG.Tests
{
	public static class DataFakeFactory
	{

		public static Categoria NovaCategoria() => new()
		{
			Nome = "Categoria Teste",
			Ativo = true,
			DataCadastro = DateTime.UtcNow
		};

		public static Usuario NovoUsuario(string sufixo) => new()
		{
			NomeUsuario = $"user_{sufixo}",
			Email = $"user_{sufixo}@test.com",
			HashSenha = "hash",
			Ativo = true,
			DataCadastro = DateTime.UtcNow
		};

		public static Jogo NovoJogo(string sufixo, Categoria categoria) => new()
		{
			Nome = $"Jogo {sufixo}",
			Categoria = categoria,
			Ativo = true,
			Visivel = true,
			DataCadastro = DateTime.UtcNow
		};

		public static Carrinho NovoCarrinho(Jogo jogo, Usuario usuario) => new()
		{
			Usuario = usuario,
			Jogo = jogo,
			Quantidade = 1,
			DataCadastro = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
		};

		public static Categoria NovaCategoria(string sufixo) => new()
		{
			Nome = $"Categoria {sufixo}",
			Ativo = true,
			DataCadastro = DateTime.UtcNow
		};

		public static PrecoJogo NovoPrecoJogo(Jogo jogo, decimal valor) => new()
		{
			Jogo = jogo,
			Valor = valor,
			DataInicio = DateTime.UtcNow
		};

	}
}
