using Microsoft.EntityFrameworkCore;

namespace FIAP.FCG.Infrastructure.Dados
{
	public class UnidadeDeTrabalho : IUnidadeDeTrabalho
	{

		private readonly ContextoBancoDadosFCG _contexto;

		public DbSet<T> Repositorio<T>() where T : class
		{
			return _contexto.Set<T>();
		}

		public UnidadeDeTrabalho(ContextoBancoDadosFCG contexto)
		{
			_contexto = contexto;
		}

		public async Task SalvarAsync()
		{
			await _contexto.SaveChangesAsync();
		}

	}
}
