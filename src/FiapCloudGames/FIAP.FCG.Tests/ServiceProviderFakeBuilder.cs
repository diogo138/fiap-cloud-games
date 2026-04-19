using FIAP.FCG.Application.Carrinhos.Services;
using FIAP.FCG.Domain.Carrinhos;
using FIAP.FCG.Infrastructure.Dados;
using Microsoft.Extensions.DependencyInjection;

namespace FIAP.FCG.Tests
{
	public class ServiceProviderFakeBuilder
	{

		private class GeradorDeContextosInterceptor
		{

			private readonly Func<ContextoBancoDadosFCG> _factory;

			public GeradorDeContextosInterceptor(Func<ContextoBancoDadosFCG> factory)
			{
				_factory = factory;
			}

			public ContextoBancoDadosFCG Criar()
			{
				var contexto = _factory();
				if (contexto == null)
					throw new InvalidOperationException("O contexto não foi configurado corretamente. Verifique se o método Setup foi chamado antes de acessar o contexto.");
				return contexto;
			}

		}

		private readonly ServiceCollection _services;

		private IServiceProvider? _provider;

		public ServiceProviderFakeBuilder()
		{
			_services = new ServiceCollection();
			ConfigurarTodosOsTiposExistentes<CarrinhoService>();
			ConfigurarTodosOsTiposExistentes<CarrinhoBusiness>();
			ConfigurarTodosOsTiposExistentes<ContextoBancoDadosFCG>();
		}

		private void ConfigurarTodosOsTiposExistentes<T>()
		{
			typeof(T).Assembly.GetExportedTypes()
				.Where(tipo => tipo.IsClass && !tipo.IsAbstract && tipo.GetInterfaces().Any())
				.ToList()
				.ForEach(tipo =>
				{
					foreach (var inter in tipo.GetInterfaces())
					{
						if (inter.Name == "I" + tipo.Name)
						{
							_services.AddScoped(inter, tipo);
							break;
						}
					}
				});
		}

		public ServiceProviderFakeBuilder Remover<T>() where T : class
		{
			return Remover(typeof(T));
		}

		public ServiceProviderFakeBuilder Remover(Type tipo)
		{
			var servico = ConsultarServico(tipo);
			if (servico != null)
				_services.Remove(servico);
			return this;
		}

		private ServiceDescriptor? ConsultarServico(Type tipo)
		{
			return _services.FirstOrDefault(item => item.ServiceType == tipo);
		}

		public ServiceProviderFakeBuilder Substituir<T>(T instancia) where T : class
		{
			var servico = ConsultarServico<T>();

			if (servico != null)
			{
				var vida = servico.Lifetime;

				Remover<T>();

				if (vida == ServiceLifetime.Scoped)
					_services.AddScoped<T>(c => instancia);

				else if (vida == ServiceLifetime.Singleton)
					_services.AddSingleton<T>(c => instancia);

				else if (vida == ServiceLifetime.Transient)
					_services.AddTransient<T>(c => instancia);
			}

			return this;
		}

		private ServiceDescriptor? ConsultarServico<T>() where T : class
		{
			return _services.FirstOrDefault(item => item.ServiceType == typeof(T));
		}

		public ServiceProviderFakeBuilder Substituir<TServico, TImplementacao>()
			where TServico : class
			where TImplementacao : class, TServico
		{
			var servico = ConsultarServico<TServico>();

			if (servico != null)
			{
				var vida = servico.Lifetime;

				if (vida == ServiceLifetime.Scoped)
					AddScoped<TServico, TImplementacao>();

				else if (vida == ServiceLifetime.Singleton)
					AddSingleton<TServico, TImplementacao>();

				else if (vida == ServiceLifetime.Transient)
					AddTransient<TServico, TImplementacao>();
			}

			return this;
		}

		public ServiceProviderFakeBuilder AddSingleton<TServico, TImplementacao>()
			where TServico : class
			where TImplementacao : class, TServico
		{
			Remover<TServico>();
			_services.AddSingleton<TServico, TImplementacao>();
			return this;
		}

		public ServiceProviderFakeBuilder AddScoped<TServico, TImplementacao>()
			where TServico : class
			where TImplementacao : class, TServico
		{
			Remover<TServico>();
			_services.AddScoped<TServico, TImplementacao>();
			return this;
		}

		public ServiceProviderFakeBuilder AddScoped<TImplementacao>(Func<TImplementacao> instancia)
			where TImplementacao : class
		{
			Remover<TImplementacao>();
			_services.AddScoped(_ => instancia());
			return this;
		}

		public ServiceProviderFakeBuilder AddTransient<TServico, TImplementacao>()
			where TServico : class
			where TImplementacao : class, TServico
		{
			Remover<TServico>();
			_services.AddTransient<TServico, TImplementacao>();
			return this;
		}

		public IServiceProvider Gerar()
		{
			return (_provider = _services.BuildServiceProvider());
		}

	}
}
