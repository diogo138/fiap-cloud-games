using FIAP.FCG.Infrastructure.Dados.Entidades;
using Microsoft.EntityFrameworkCore;

namespace FIAP.FCG.Infrastructure.Dados;

public partial class ContextoBancoDadosFCG : DbContext
{

	public DbSet<Acesso> Acessos => Set<Acesso>();
	public DbSet<Administrador> Administradores => Set<Administrador>();
	public DbSet<Carrinho> Carrinhos => Set<Carrinho>();
	public DbSet<Categoria> Categorias => Set<Categoria>();
	public DbSet<IntegradoraPagamento> IntegradorasPagamentos => Set<IntegradoraPagamento>();
	public DbSet<Jogo> Jogos => Set<Jogo>();
	public DbSet<JogoAdquirido> JogosAdquiridos => Set<JogoAdquirido>();
	public DbSet<PrecoJogo> PrecosJogos => Set<PrecoJogo>();
	public DbSet<Promocao> Promocoes => Set<Promocao>();
	public DbSet<Resource> Resources => Set<Resource>();
	public DbSet<Tag> Tags => Set<Tag>();
	public DbSet<Transacao> Transacoes => Set<Transacao>();
	public DbSet<Usuario> Usuarios => Set<Usuario>();

	public ContextoBancoDadosFCG()
	{
	}

	public ContextoBancoDadosFCG(DbContextOptions<ContextoBancoDadosFCG> options)
		: base(options)
	{
	}


	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		ConfigurarLogsDeAcesso(modelBuilder);
		ConfigurarAdministradoresDoSistema(modelBuilder);
		ConfigurarCarrinhoDeCompras(modelBuilder);
		ConfigurarCategoriasDeJogos(modelBuilder);
		ConfigurarIntegradorasDePagamento(modelBuilder);
		ConfigurarJogos(modelBuilder);
		ConfigurarJogosAdquiridos(modelBuilder);
		ConfigurarPrecosJogos(modelBuilder);
		ConfigurarPromocoes(modelBuilder);
		ConfigurarResources(modelBuilder);
		ConfigurarTags(modelBuilder);
		ConfigurarTransacoes(modelBuilder);
		ConfigurarUsuarios(modelBuilder);

		OnModelCreatingPartial(modelBuilder);
	}

	private static void ConfigurarLogsDeAcesso(ModelBuilder modelBuilder)
	{
		modelBuilder.Entity<Acesso>(entity =>
		{
			entity.ToTable("Acessos");
			entity.HasKey(e => e.Id);

			entity.HasIndex(e => new { e.UsuarioId, e.DataHora }, "IX_Acessos_UsuarioId_DataHora").IsDescending(false, true);

			entity.Property(e => e.Id).HasColumnName("id");
			entity.Property(e => e.DataHora)
				.HasDefaultValueSql("(getutcdate())")
				.HasColumnName("dataHora");
			entity.Property(e => e.UsuarioId).HasColumnName("usuarioId");

			entity.HasOne(d => d.Usuario).WithMany(p => p.Acessos)
				.HasForeignKey(d => d.UsuarioId)
				.OnDelete(DeleteBehavior.ClientSetNull)
				.HasConstraintName("FK_Acessos_Usuarios");
		});
	}

	private static void ConfigurarAdministradoresDoSistema(ModelBuilder modelBuilder)
	{
		modelBuilder.Entity<Administrador>(entity =>
		{
			entity.ToTable("Administradores");
			entity.HasKey(e => e.UsuarioId);

			entity.Property(e => e.UsuarioId)
				.ValueGeneratedNever()
				.HasColumnName("usuarioId");
			entity.Property(e => e.DataCadastro)
				.HasDefaultValueSql("(getutcdate())")
				.HasColumnName("dataCadastro");

			entity.HasOne(d => d.Usuario).WithOne(p => p.Administradore)
				.HasForeignKey<Administrador>(d => d.UsuarioId)
				.OnDelete(DeleteBehavior.ClientSetNull)
				.HasConstraintName("FK_Administradores_Usuarios");
		});
	}

	private static void ConfigurarCarrinhoDeCompras(ModelBuilder modelBuilder)
	{
		modelBuilder.Entity<Carrinho>(entity =>
		{
			entity.ToTable("Carrinhos");
			entity.HasKey(e => e.Id);

			entity.HasIndex(e => e.UsuarioId, "IX_Carrinhos_UsuarioId");

			entity.HasIndex(e => new { e.UsuarioId, e.JogoId }, "UQ_Carrinhos_Usr_Jogo").IsUnique();

			entity.Property(e => e.Id).HasColumnName("id");
			entity.Property(e => e.DataCadastro)
				.HasDefaultValueSql("(getutcdate())")
				.HasColumnName("dataCadastro");
			entity.Property(e => e.JogoId).HasColumnName("jogoId");
			entity.Property(e => e.Quantidade)
				.HasDefaultValue((short)1)
				.HasColumnName("quantidade");
			entity.Property(e => e.UsuarioId).HasColumnName("usuarioId");

			entity.HasOne(d => d.Jogo).WithMany(p => p.Carrinhos)
				.HasForeignKey(d => d.JogoId)
				.OnDelete(DeleteBehavior.ClientSetNull)
				.HasConstraintName("FK_Carrinhos_Jogos");

			entity.HasOne(d => d.Usuario).WithMany(p => p.Carrinhos)
				.HasForeignKey(d => d.UsuarioId)
				.OnDelete(DeleteBehavior.ClientSetNull)
				.HasConstraintName("FK_Carrinhos_Usuarios");
		});
	}

	private static void ConfigurarCategoriasDeJogos(ModelBuilder modelBuilder)
	{
		modelBuilder.Entity<Categoria>(entity =>
		{
			entity.ToTable("Categorias");
			entity.HasKey(e => e.Id);

			entity.HasIndex(e => e.Nome, "UQ_Categorias_Nome").IsUnique();

			entity.Property(e => e.Id).HasColumnName("id");
			entity.Property(e => e.Ativo)
				.HasDefaultValue(true)
				.HasColumnName("ativo");
			entity.Property(e => e.DataCadastro)
				.HasDefaultValueSql("(getutcdate())")
				.HasColumnName("dataCadastro");
			entity.Property(e => e.Descricao)
				.HasMaxLength(500)
				.HasColumnName("descricao");
			entity.Property(e => e.Nome)
				.HasMaxLength(100)
				.HasColumnName("nome");
		});
	}

	private static void ConfigurarIntegradorasDePagamento(ModelBuilder modelBuilder)
	{
		modelBuilder.Entity<IntegradoraPagamento>(entity =>
		{
			entity.ToTable("IntegradorasPagamento");
			entity.HasKey(e => e.Id);

			entity.HasIndex(e => e.Nome, "UQ_IntegradorasPagamento_Nome").IsUnique();

			entity.Property(e => e.Id).HasColumnName("id");
			entity.Property(e => e.Ativo)
				.HasDefaultValue(true)
				.HasColumnName("ativo");
			entity.Property(e => e.DataCadastro)
				.HasDefaultValueSql("(getutcdate())")
				.HasColumnName("dataCadastro");
			entity.Property(e => e.Nome)
				.HasMaxLength(100)
				.HasColumnName("nome");
		});
	}

	private static void ConfigurarJogos(ModelBuilder modelBuilder)
	{
		modelBuilder.Entity<Jogo>(entity =>
		{
			entity.ToTable("Jogos");
			entity.HasKey(e => e.Id);

			entity.HasIndex(e => e.CategoriaId, "IX_Jogos_CategoriaId");

			entity.HasIndex(e => e.Nome, "IX_Jogos_Nome");

			entity.HasIndex(e => new { e.Visivel, e.Ativo }, "IX_Jogos_Visivel_Ativo");

			entity.Property(e => e.Id).HasColumnName("id");
			entity.Property(e => e.Ativo)
				.HasDefaultValue(true)
				.HasColumnName("ativo");
			entity.Property(e => e.CategoriaId).HasColumnName("categoriaId");
			entity.Property(e => e.DataCadastro)
				.HasDefaultValueSql("(getutcdate())")
				.HasColumnName("dataCadastro");
			entity.Property(e => e.DataLancamento).HasColumnName("dataLancamento");
			entity.Property(e => e.Descricao).HasColumnName("descricao");
			entity.Property(e => e.Nome)
				.HasMaxLength(255)
				.HasColumnName("nome");
			entity.Property(e => e.Visivel)
				.HasDefaultValue(true)
				.HasColumnName("visivel");

			entity.HasOne(d => d.Categoria).WithMany(p => p.Jogos)
				.HasForeignKey(d => d.CategoriaId)
				.OnDelete(DeleteBehavior.ClientSetNull)
				.HasConstraintName("FK_Jogos_Categorias");

			entity.HasMany(d => d.IdTags).WithMany(p => p.IdJogos)
				.UsingEntity<Dictionary<string, object>>(
					"TagsPorJogo",
					r => r.HasOne<Tag>().WithMany()
						.HasForeignKey("IdTag")
						.OnDelete(DeleteBehavior.ClientSetNull)
						.HasConstraintName("FK_TagsPorJogo_Tags"),
					l => l.HasOne<Jogo>().WithMany()
						.HasForeignKey("IdJogo")
						.OnDelete(DeleteBehavior.ClientSetNull)
						.HasConstraintName("FK_TagsPorJogo_Jogos"),
					j =>
					{
						j.HasKey("IdJogo", "IdTag");
						j.ToTable("TagsPorJogo");
						j.HasIndex(new[] { "IdTag" }, "IX_TagsPorJogo_IdTag");
						j.IndexerProperty<int>("IdJogo").HasColumnName("idJogo");
						j.IndexerProperty<int>("IdTag").HasColumnName("idTag");
					});
		});
	}

	private static void ConfigurarJogosAdquiridos(ModelBuilder modelBuilder)
	{
		modelBuilder.Entity<JogoAdquirido>(entity =>
		{
			entity.ToTable("JogosAdquiridos");
			entity.HasKey(e => e.Id);

			entity.HasIndex(e => e.JogoId, "IX_JogosAdquiridos_JogoId");

			entity.HasIndex(e => e.UsuarioId, "IX_JogosAdquiridos_UsuarioId");

			entity.HasIndex(e => new { e.UsuarioId, e.JogoId }, "UQ_JogosAdquiridos_Usr_Jogo").IsUnique();

			entity.Property(e => e.Id).HasColumnName("id");
			entity.Property(e => e.JogoId).HasColumnName("jogoId");
			entity.Property(e => e.Quantidade).HasColumnName("quantidade");
			entity.Property(e => e.TransacaoId).HasColumnName("transacaoId");
			entity.Property(e => e.UsuarioId).HasColumnName("usuarioId");
			entity.Property(e => e.ValorTotal)
				.HasColumnType("decimal(10, 2)")
				.HasColumnName("valorTotal");
			entity.Property(e => e.ValorUnitario)
				.HasColumnType("decimal(10, 2)")
				.HasColumnName("valorUnitario");

			entity.HasOne(d => d.Jogo).WithMany(p => p.JogosAdquiridos)
				.HasForeignKey(d => d.JogoId)
				.OnDelete(DeleteBehavior.ClientSetNull)
				.HasConstraintName("FK_JogosAdquiridos_Jogos");

			entity.HasOne(d => d.Transacao).WithMany(p => p.JogosAdquiridos)
				.HasForeignKey(d => d.TransacaoId)
				.OnDelete(DeleteBehavior.ClientSetNull)
				.HasConstraintName("FK_JogosAdquiridos_Transacoes");

			entity.HasOne(d => d.Usuario).WithMany(p => p.JogosAdquiridos)
				.HasForeignKey(d => d.UsuarioId)
				.OnDelete(DeleteBehavior.ClientSetNull)
				.HasConstraintName("FK_JogosAdquiridos_Usuarios");
		});
	}

	private static void ConfigurarPrecosJogos(ModelBuilder modelBuilder)
	{
		modelBuilder.Entity<PrecoJogo>(entity =>
		{
			entity.ToTable("PrecosJogos");
			entity.HasKey(e => e.Id);

			entity.HasIndex(e => new { e.JogoId, e.DataInicio }, "IX_PrecosJogos_JogoId_DataInicio").IsDescending(false, true);

			entity.Property(e => e.Id).HasColumnName("id");
			entity.Property(e => e.DataInicio).HasColumnName("dataInicio");
			entity.Property(e => e.JogoId).HasColumnName("jogoId");
			entity.Property(e => e.PercDesconto)
				.HasColumnType("decimal(5, 2)")
				.HasColumnName("percDesconto");
			entity.Property(e => e.PromocaoId).HasColumnName("promocaoId");
			entity.Property(e => e.Valor)
				.HasColumnType("decimal(10, 2)")
				.HasColumnName("valor");

			entity.HasOne(d => d.Jogo).WithMany(p => p.PrecosJogos)
				.HasForeignKey(d => d.JogoId)
				.OnDelete(DeleteBehavior.ClientSetNull)
				.HasConstraintName("FK_PrecosJogos_Jogos");

			entity.HasOne(d => d.Promocao).WithMany(p => p.PrecosJogos)
				.HasForeignKey(d => d.PromocaoId)
				.HasConstraintName("FK_PrecosJogos_Promocoes");
		});
	}

	private static void ConfigurarPromocoes(ModelBuilder modelBuilder)
	{
		modelBuilder.Entity<Promocao>(entity =>
		{
			entity.ToTable("Promocoes");
			entity.HasKey(e => e.Id);

			entity.Property(e => e.Id).HasColumnName("id");
			entity.Property(e => e.DataCadastro)
				.HasDefaultValueSql("(getutcdate())")
				.HasColumnName("dataCadastro");
			entity.Property(e => e.DataFim).HasColumnName("dataFim");
			entity.Property(e => e.DataInicio).HasColumnName("dataInicio");
			entity.Property(e => e.Nome)
				.HasMaxLength(200)
				.HasColumnName("nome");
			entity.Property(e => e.PercDesconto)
				.HasColumnType("decimal(5, 2)")
				.HasColumnName("percDesconto");
		});
	}

	private static void ConfigurarResources(ModelBuilder modelBuilder)
	{
		modelBuilder.Entity<Resource>(entity =>
		{
			entity.ToTable("Resources");
			entity.HasKey(e => e.Id);

			entity.HasIndex(e => new { e.JogoId, e.TipoResource }, "IX_Resources_JogoId_Tipo");

			entity.Property(e => e.Id).HasColumnName("id");
			entity.Property(e => e.Ativo)
				.HasDefaultValue(true)
				.HasColumnName("ativo");
			entity.Property(e => e.DataCadastro)
				.HasDefaultValueSql("(getutcdate())")
				.HasColumnName("dataCadastro");
			entity.Property(e => e.Descricao)
				.HasMaxLength(255)
				.HasColumnName("descricao");
			entity.Property(e => e.Endereco)
				.HasMaxLength(1000)
				.HasColumnName("endereco");
			entity.Property(e => e.JogoId).HasColumnName("jogoId");
			entity.Property(e => e.TipoResource)
				.HasMaxLength(50)
				.HasColumnName("tipoResource");

			entity.HasOne(d => d.Jogo).WithMany(p => p.Resources)
				.HasForeignKey(d => d.JogoId)
				.OnDelete(DeleteBehavior.ClientSetNull)
				.HasConstraintName("FK_Resources_Jogos");
		});
	}

	private static void ConfigurarTags(ModelBuilder modelBuilder)
	{
		modelBuilder.Entity<Tag>(entity =>
		{
			entity.ToTable("Tags");
			entity.HasKey(e => e.Id);

			entity.HasIndex(e => e.Nome, "UQ_Tags_Nome").IsUnique();

			entity.Property(e => e.Id).HasColumnName("id");
			entity.Property(e => e.Ativo)
				.HasDefaultValue(true)
				.HasColumnName("ativo");
			entity.Property(e => e.DataCadastro)
				.HasDefaultValueSql("(getutcdate())")
				.HasColumnName("dataCadastro");
			entity.Property(e => e.Nome)
				.HasMaxLength(100)
				.HasColumnName("nome");
		});
	}

	private static void ConfigurarTransacoes(ModelBuilder modelBuilder)
	{
		modelBuilder.Entity<Transacao>(entity =>
		{
			entity.ToTable("Transacoes");
			entity.HasKey(e => e.Id);

			entity.HasIndex(e => e.Comprovante, "IX_Transacoes_Comprovante");

			entity.HasIndex(e => e.DataCompra, "IX_Transacoes_DataCompra").IsDescending();

			entity.Property(e => e.Id).HasColumnName("id");
			entity.Property(e => e.Comprovante)
				.HasMaxLength(500)
				.HasColumnName("comprovante");
			entity.Property(e => e.DataCompra)
				.HasDefaultValueSql("(getutcdate())")
				.HasColumnName("dataCompra");
			entity.Property(e => e.IntegradoraPagamentoId).HasColumnName("integradoraPagamentoId");
			entity.Property(e => e.ValorTotal)
				.HasColumnType("decimal(10, 2)")
				.HasColumnName("valorTotal");

			entity.HasOne(d => d.IntegradoraPagamento).WithMany(p => p.Transacos)
				.HasForeignKey(d => d.IntegradoraPagamentoId)
				.OnDelete(DeleteBehavior.ClientSetNull)
				.HasConstraintName("FK_Transacoes_IntegradorasPagamento");
		});
	}

	private static void ConfigurarUsuarios(ModelBuilder modelBuilder)
	{
		modelBuilder.Entity<Usuario>(entity =>
		{
			entity.ToTable("Usuarios");
			entity.HasKey(e => e.Id);

			entity.HasIndex(e => e.Ativo, "IX_Usuarios_Ativo");

			entity.HasIndex(e => e.Email, "IX_Usuarios_Email");

			entity.HasIndex(e => e.Email, "UQ_Usuarios_Email").IsUnique();

			entity.Property(e => e.Id).HasColumnName("id");
			entity.Property(e => e.Ativo)
				.HasDefaultValue(true)
				.HasColumnName("ativo");
			entity.Property(e => e.DataCadastro)
				.HasDefaultValueSql("(getutcdate())")
				.HasColumnName("dataCadastro");
			entity.Property(e => e.Email)
				.HasMaxLength(255)
				.HasColumnName("email");
			entity.Property(e => e.HashSenha)
				.HasMaxLength(512)
				.HasColumnName("hashSenha");
			entity.Property(e => e.NomeUsuario)
				.HasMaxLength(100)
				.HasColumnName("nomeUsuario");

			entity.HasMany(d => d.IdJogos).WithMany(p => p.IdUsuarios)
				.UsingEntity<Dictionary<string, object>>(
					"ListasDeDesejo",
					r => r.HasOne<Jogo>().WithMany()
						.HasForeignKey("IdJogo")
						.OnDelete(DeleteBehavior.ClientSetNull)
						.HasConstraintName("FK_ListasDeDesejos_Jogos"),
					l => l.HasOne<Usuario>().WithMany()
						.HasForeignKey("IdUsuario")
						.OnDelete(DeleteBehavior.ClientSetNull)
						.HasConstraintName("FK_ListasDeDesejos_Usuarios"),
					j =>
					{
						j.HasKey("IdUsuario", "IdJogo");
						j.ToTable("ListasDeDesejos");
						j.HasIndex(new[] { "IdJogo" }, "IX_ListasDeDesejos_IdJogo");
						j.IndexerProperty<int>("IdUsuario").HasColumnName("idUsuario");
						j.IndexerProperty<int>("IdJogo").HasColumnName("idJogo");
					});
		});
	}

	partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
