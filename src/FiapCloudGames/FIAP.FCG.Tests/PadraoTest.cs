using FIAP.FCG.Infrastructure.Dados;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace FIAP.FCG.Tests
{
	[FixtureLifeCycle(LifeCycle.InstancePerTestCase)]
	public abstract class PadraoTest
	{

		private readonly IDictionary<Type, object> _dicionario = new Dictionary<Type, object>();

		private ContextoBancoDadosFCG _contexto = null!;

		protected ContextoBancoDadosFCG Contexto {
			get
			{
				if (_contexto == null)
				{
					var options = new DbContextOptionsBuilder<ContextoBancoDadosFCG>()
						.UseInMemoryDatabase($"FCG-Test-{Guid.NewGuid():N}")
						.EnableSensitiveDataLogging()
						.Options;
					_contexto = new ContextoBancoDadosFCG(options);
					_contexto.Database.EnsureCreated();
				}
				return _contexto;
			}
		}

		private ServiceProviderFakeBuilder Builder { get; set; } = null!;

		protected IServiceProvider Provider { get; set; } = null!;

		[SetUp]
		public void PadraoTestSetup()
		{
			Builder = new ServiceProviderFakeBuilder();
			Builder.AddScoped(() => Contexto);
			Provider = Builder.Gerar();
		}

		protected T CarregarInstancia<T>()
		{
			if (!_dicionario.ContainsKey(typeof(T)))
				_dicionario[typeof(T)] = Provider.GetService<T>()!;
			return (T)_dicionario[typeof(T)];
		}

		protected async Task AdicionarAsync(params object[] entidades)
		{
			Contexto.AddRange(entidades);
			await Contexto.SaveChangesAsync();
			Contexto.ChangeTracker.Clear();
		}

		protected async Task PersistirAsync()
		{
			await Contexto.SaveChangesAsync();
			Contexto.ChangeTracker.Clear();
		}
	}
}
