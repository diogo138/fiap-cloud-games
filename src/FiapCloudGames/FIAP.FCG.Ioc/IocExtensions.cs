using FIAP.FCG.Application.Carrinhos.Services;
using FIAP.FCG.Domain.Carrinhos;
using FIAP.FCG.Infrastructure.Dados;
using Microsoft.Extensions.DependencyInjection;

namespace FIAP.FCG.Ioc
{
	public static class IocExtensions
	{

		public static void ConfigurarInjecaoDeDependencias(this IServiceCollection services)
		{
			ConfigurarDinamicamenteClassesQuePossuemInterfaceNoProjeto<ICarrinhoService>(services);   // Application
			ConfigurarDinamicamenteClassesQuePossuemInterfaceNoProjeto<ICarrinhoBusiness>(services);  // Domain
			ConfigurarDinamicamenteClassesQuePossuemInterfaceNoProjeto<IUnidadeDeTrabalho>(services); // Infrastructure
		}

		private static void ConfigurarDinamicamenteClassesQuePossuemInterfaceNoProjeto<T>(IServiceCollection services)
		{
			var assembly = typeof(T).Assembly;

			if (assembly == null)
				return;

			var tipos = assembly.GetTypes()
				.Where(t => t.IsClass && !t.IsAbstract && t.IsPublic);

			foreach (var tipo in tipos)
			{
				var inter = tipo.GetInterface($"I{tipo.Name}");
				if (inter != null)
					services.AddScoped(inter, tipo);
			}
		}
	}
}
