
using Microsoft.EntityFrameworkCore;

namespace FIAP.FCG.Infrastructure.Dados
{
	public interface IUnidadeDeTrabalho
	{

		DbSet<T> Repositorio<T>() where T : class;

		Task SalvarAsync();

	}
}