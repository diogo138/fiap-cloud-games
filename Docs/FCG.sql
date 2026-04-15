-- ============================================================
-- FCG (FIAP Challenge Game) — Script completo
-- Criação do banco, DDL e dados de exemplo
-- SQL Server 2019+ | Compatibility Level 150
-- ============================================================

USE master;
GO

IF EXISTS (SELECT 1 FROM sys.databases WHERE name = N'FCG')
BEGIN
    ALTER DATABASE FCG SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE FCG;
END
GO

CREATE DATABASE FCG
    COLLATE Latin1_General_CI_AI;
GO

USE FCG;
GO

-- ============================================================
-- DDL — Tabelas independentes (sem FK)
-- ============================================================

CREATE TABLE dbo.Categorias (
    id           INT           NOT NULL IDENTITY(1,1),
    nome         NVARCHAR(100) NOT NULL,
    descricao    NVARCHAR(500) NULL,
    dataCadastro DATETIME2     NOT NULL CONSTRAINT DF_Categorias_DataCadastro DEFAULT GETUTCDATE(),
    ativo        BIT           NOT NULL CONSTRAINT DF_Categorias_Ativo        DEFAULT 1,
    CONSTRAINT PK_Categorias      PRIMARY KEY (id),
    CONSTRAINT UQ_Categorias_Nome UNIQUE (nome)
);

CREATE TABLE dbo.Tags (
    id           INT           NOT NULL IDENTITY(1,1),
    nome         NVARCHAR(100) NOT NULL,
    dataCadastro DATETIME2     NOT NULL CONSTRAINT DF_Tags_DataCadastro DEFAULT GETUTCDATE(),
    ativo        BIT           NOT NULL CONSTRAINT DF_Tags_Ativo        DEFAULT 1,
    CONSTRAINT PK_Tags      PRIMARY KEY (id),
    CONSTRAINT UQ_Tags_Nome UNIQUE (nome)
);

CREATE TABLE dbo.Promocoes (
    id           INT          NOT NULL IDENTITY(1,1),
    nome         NVARCHAR(200) NOT NULL,
    dataCadastro DATETIME2     NOT NULL CONSTRAINT DF_Promocoes_DataCadastro DEFAULT GETUTCDATE(),
    dataInicio   DATETIME2     NOT NULL,
    dataFim      DATETIME2     NOT NULL,
    percDesconto DECIMAL(5,2)  NOT NULL,
    CONSTRAINT PK_Promocoes       PRIMARY KEY (id),
    CONSTRAINT CK_Promocoes_Datas CHECK (dataFim > dataInicio),
    CONSTRAINT CK_Promocoes_Perc  CHECK (percDesconto BETWEEN 0 AND 100)
);

CREATE TABLE dbo.IntegradorasPagamento (
    id           INT           NOT NULL IDENTITY(1,1),
    nome         NVARCHAR(100) NOT NULL,
    dataCadastro DATETIME2     NOT NULL CONSTRAINT DF_IntegradorasPagamento_DataCadastro DEFAULT GETUTCDATE(),
    ativo        BIT           NOT NULL CONSTRAINT DF_IntegradorasPagamento_Ativo        DEFAULT 1,
    CONSTRAINT PK_IntegradorasPagamento     PRIMARY KEY (id),
    CONSTRAINT UQ_IntegradorasPagamento_Nome UNIQUE (nome)
);

-- ============================================================
-- DDL — Usuarios e dependentes de identidade
-- ============================================================

CREATE TABLE dbo.Usuarios (
    id           INT            NOT NULL IDENTITY(1,1),
    nomeUsuario  NVARCHAR(100)  NOT NULL,
    email        NVARCHAR(255)  NOT NULL,
    hashSenha    NVARCHAR(512)  NOT NULL,
    dataCadastro DATETIME2      NOT NULL CONSTRAINT DF_Usuarios_DataCadastro DEFAULT GETUTCDATE(),
    ativo        BIT            NOT NULL CONSTRAINT DF_Usuarios_Ativo        DEFAULT 1,
    CONSTRAINT PK_Usuarios       PRIMARY KEY (id),
    CONSTRAINT UQ_Usuarios_Email UNIQUE (email)
);

CREATE INDEX IX_Usuarios_Email ON dbo.Usuarios (email);
CREATE INDEX IX_Usuarios_Ativo  ON dbo.Usuarios (ativo);

CREATE TABLE dbo.Administradores (
    usuarioId    INT       NOT NULL,
    dataCadastro DATETIME2 NOT NULL CONSTRAINT DF_Administradores_DataCadastro DEFAULT GETUTCDATE(),
    CONSTRAINT PK_Administradores          PRIMARY KEY (usuarioId),
    CONSTRAINT FK_Administradores_Usuarios  FOREIGN KEY (usuarioId) REFERENCES dbo.Usuarios(id)
);

CREATE TABLE dbo.Acessos (
    id        INT       NOT NULL IDENTITY(1,1),
    usuarioId INT       NOT NULL,
    dataHora  DATETIME2 NOT NULL CONSTRAINT DF_Acessos_DataHora DEFAULT GETUTCDATE(),
    CONSTRAINT PK_Acessos          PRIMARY KEY (id),
    CONSTRAINT FK_Acessos_Usuarios  FOREIGN KEY (usuarioId) REFERENCES dbo.Usuarios(id)
);

CREATE INDEX IX_Acessos_UsuarioId_DataHora ON dbo.Acessos (usuarioId, dataHora DESC);

-- ============================================================
-- DDL — Catálogo de Jogos
-- ============================================================

CREATE TABLE dbo.Jogos (
    id             INT           NOT NULL IDENTITY(1,1),
    nome           NVARCHAR(255) NOT NULL,
    categoriaId    INT           NOT NULL,
    descricao      NVARCHAR(MAX) NULL,
    dataCadastro   DATETIME2     NOT NULL CONSTRAINT DF_Jogos_DataCadastro  DEFAULT GETUTCDATE(),
    dataLancamento DATE          NULL,
    visivel        BIT           NOT NULL CONSTRAINT DF_Jogos_Visivel       DEFAULT 1,
    ativo          BIT           NOT NULL CONSTRAINT DF_Jogos_Ativo         DEFAULT 1,
    CONSTRAINT PK_Jogos           PRIMARY KEY (id),
    CONSTRAINT FK_Jogos_Categorias FOREIGN KEY (categoriaId) REFERENCES dbo.Categorias(id)
);

CREATE INDEX IX_Jogos_CategoriaId   ON dbo.Jogos (categoriaId);
CREATE INDEX IX_Jogos_Visivel_Ativo ON dbo.Jogos (visivel, ativo);
CREATE INDEX IX_Jogos_Nome          ON dbo.Jogos (nome);

CREATE TABLE dbo.TagsPorJogo (
    idJogo INT NOT NULL,
    idTag  INT NOT NULL,
    CONSTRAINT PK_TagsPorJogo       PRIMARY KEY (idJogo, idTag),
    CONSTRAINT FK_TagsPorJogo_Jogos  FOREIGN KEY (idJogo) REFERENCES dbo.Jogos(id),
    CONSTRAINT FK_TagsPorJogo_Tags   FOREIGN KEY (idTag)  REFERENCES dbo.Tags(id)
);

CREATE INDEX IX_TagsPorJogo_IdTag ON dbo.TagsPorJogo (idTag);

CREATE TABLE dbo.Resources (
    id           INT            NOT NULL IDENTITY(1,1),
    jogoId       INT            NOT NULL,
    tipoResource NVARCHAR(50)   NOT NULL,
    descricao    NVARCHAR(255)  NULL,
    endereco     NVARCHAR(1000) NOT NULL,
    dataCadastro DATETIME2      NOT NULL CONSTRAINT DF_Resources_DataCadastro DEFAULT GETUTCDATE(),
    ativo        BIT            NOT NULL CONSTRAINT DF_Resources_Ativo        DEFAULT 1,
    CONSTRAINT PK_Resources       PRIMARY KEY (id),
    CONSTRAINT FK_Resources_Jogos  FOREIGN KEY (jogoId) REFERENCES dbo.Jogos(id)
);

CREATE INDEX IX_Resources_JogoId_Tipo ON dbo.Resources (jogoId, tipoResource);

CREATE TABLE dbo.PrecosJogos (
    id           INT          NOT NULL IDENTITY(1,1),
    jogoId       INT          NOT NULL,
    valor        DECIMAL(10,2) NOT NULL,
    promocaoId   INT          NULL,
    percDesconto DECIMAL(5,2)  NULL,
    dataInicio   DATETIME2    NOT NULL,
    CONSTRAINT PK_PrecosJogos           PRIMARY KEY (id),
    CONSTRAINT FK_PrecosJogos_Jogos      FOREIGN KEY (jogoId)     REFERENCES dbo.Jogos(id),
    CONSTRAINT FK_PrecosJogos_Promocoes  FOREIGN KEY (promocaoId) REFERENCES dbo.Promocoes(id),
    CONSTRAINT CK_PrecosJogos_Valor      CHECK (valor >= 0),
    CONSTRAINT CK_PrecosJogos_Perc       CHECK (percDesconto IS NULL OR percDesconto BETWEEN 0 AND 100)
);

CREATE INDEX IX_PrecosJogos_JogoId_DataInicio ON dbo.PrecosJogos (jogoId, dataInicio DESC);

-- ============================================================
-- DDL — Loja (wishlist, carrinho, transações, biblioteca)
-- ============================================================

CREATE TABLE dbo.ListasDeDesejos (
    idUsuario INT NOT NULL,
    idJogo    INT NOT NULL,
    CONSTRAINT PK_ListasDeDesejos           PRIMARY KEY (idUsuario, idJogo),
    CONSTRAINT FK_ListasDeDesejos_Usuarios   FOREIGN KEY (idUsuario) REFERENCES dbo.Usuarios(id),
    CONSTRAINT FK_ListasDeDesejos_Jogos      FOREIGN KEY (idJogo)    REFERENCES dbo.Jogos(id)
);

CREATE INDEX IX_ListasDeDesejos_IdJogo ON dbo.ListasDeDesejos (idJogo);

CREATE TABLE dbo.Carrinhos (
    id           INT      NOT NULL IDENTITY(1,1),
    usuarioId    INT      NOT NULL,
    jogoId       INT      NOT NULL,
    quantidade   SMALLINT NOT NULL CONSTRAINT DF_Carrinhos_Quantidade  DEFAULT 1,
    dataCadastro DATETIME2 NOT NULL CONSTRAINT DF_Carrinhos_DataCadastro DEFAULT GETUTCDATE(),
    CONSTRAINT PK_Carrinhos           PRIMARY KEY (id),
    CONSTRAINT FK_Carrinhos_Usuarios   FOREIGN KEY (usuarioId) REFERENCES dbo.Usuarios(id),
    CONSTRAINT FK_Carrinhos_Jogos      FOREIGN KEY (jogoId)    REFERENCES dbo.Jogos(id),
    CONSTRAINT UQ_Carrinhos_Usr_Jogo   UNIQUE (usuarioId, jogoId)
);

CREATE INDEX IX_Carrinhos_UsuarioId ON dbo.Carrinhos (usuarioId);

CREATE TABLE dbo.Transacoes (
    id                     INT           NOT NULL IDENTITY(1,1),
    integradoraPagamentoId INT           NOT NULL,
    dataCompra             DATETIME2     NOT NULL CONSTRAINT DF_Transacoes_DataCompra DEFAULT GETUTCDATE(),
    valorTotal             DECIMAL(10,2) NOT NULL,
    comprovante            NVARCHAR(500) NULL,
    CONSTRAINT PK_Transacoes                        PRIMARY KEY (id),
    CONSTRAINT FK_Transacoes_IntegradorasPagamento   FOREIGN KEY (integradoraPagamentoId)
        REFERENCES dbo.IntegradorasPagamento(id)
);

CREATE INDEX IX_Transacoes_DataCompra  ON dbo.Transacoes (dataCompra DESC);
CREATE INDEX IX_Transacoes_Comprovante ON dbo.Transacoes (comprovante);

CREATE TABLE dbo.JogosAdquiridos (
    id            INT           NOT NULL IDENTITY(1,1),
    transacaoId   INT           NOT NULL,
    usuarioId     INT           NOT NULL,
    jogoId        INT           NOT NULL,
    quantidade    SMALLINT      NOT NULL,
    valorUnitario DECIMAL(10,2) NOT NULL,
    valorTotal    DECIMAL(10,2) NOT NULL,
    CONSTRAINT PK_JogosAdquiridos             PRIMARY KEY (id),
    CONSTRAINT FK_JogosAdquiridos_Transacoes   FOREIGN KEY (transacaoId) REFERENCES dbo.Transacoes(id),
    CONSTRAINT FK_JogosAdquiridos_Usuarios     FOREIGN KEY (usuarioId)   REFERENCES dbo.Usuarios(id),
    CONSTRAINT FK_JogosAdquiridos_Jogos        FOREIGN KEY (jogoId)      REFERENCES dbo.Jogos(id),
    CONSTRAINT UQ_JogosAdquiridos_Usr_Jogo     UNIQUE (usuarioId, jogoId)
);

CREATE INDEX IX_JogosAdquiridos_UsuarioId ON dbo.JogosAdquiridos (usuarioId);
CREATE INDEX IX_JogosAdquiridos_JogoId    ON dbo.JogosAdquiridos (jogoId);

GO

-- ============================================================
-- DADOS DE EXEMPLO
-- ============================================================

-- ------------------------------------------------------------
-- Categorias
-- ------------------------------------------------------------
INSERT INTO dbo.Categorias (nome, descricao) VALUES
    (N'RPG',        N'Jogos de interpretação de papéis com progressão de personagem'),
    (N'Ação',       N'Jogos com combate dinâmico e ritmo acelerado'),
    (N'Aventura',   N'Jogos focados em exploração e narrativa'),
    (N'Estratégia', N'Jogos de planejamento tático ou gerenciamento'),
    (N'Esportes',   N'Simulações esportivas'),
    (N'Terror',     N'Jogos de horror e suspense');

-- ------------------------------------------------------------
-- Tags
-- ------------------------------------------------------------
INSERT INTO dbo.Tags (nome) VALUES
    (N'multiplayer'),
    (N'single-player'),
    (N'open-world'),
    (N'indie'),
    (N'online'),
    (N'cooperativo'),
    (N'pixel-art'),
    (N'sci-fi'),
    (N'fantasia'),
    (N'sobrevivência');

-- ------------------------------------------------------------
-- Integradora de Pagamento
-- ------------------------------------------------------------
INSERT INTO dbo.IntegradorasPagamento (nome) VALUES
    (N'Stripe'),
    (N'PagSeguro'),
    (N'PayPal');

-- ------------------------------------------------------------
-- Promoções
-- ------------------------------------------------------------
INSERT INTO dbo.Promocoes (nome, dataInicio, dataFim, percDesconto) VALUES
    (N'Promoção de Férias',      '2026-01-10', '2026-01-31', 30.00),
    (N'Semana do Consumidor',    '2026-03-15', '2026-03-21', 25.00),
    (N'Mega Sale Abril',         '2026-04-01', '2026-04-30', 40.00),
    (N'Lançamento Especial',     '2026-05-01', '2026-05-07', 15.00);

-- ------------------------------------------------------------
-- Usuários
-- Senhas fictícias — hash Argon2id simulado (nunca texto plano)
-- ------------------------------------------------------------
INSERT INTO dbo.Usuarios (nomeUsuario, email, hashSenha) VALUES
    (N'admin_fcg',    N'admin@fcg.com',      N'$argon2id$v=19$m=65536,t=3,p=4$YWRtaW5zYWx0$hashadmin000001'),
    (N'joao.silva',   N'joao@email.com',     N'$argon2id$v=19$m=65536,t=3,p=4$am9hb3NhbHQ$hashjoao000002'),
    (N'maria.souza',  N'maria@email.com',    N'$argon2id$v=19$m=65536,t=3,p=4$bWFyaWFzYWx0$hashmaria00003'),
    (N'pedro.costa',  N'pedro@email.com',    N'$argon2id$v=19$m=65536,t=3,p=4$cGVkcm9zYWx0$hashpedro00004'),
    (N'ana.lima',     N'ana@email.com',      N'$argon2id$v=19$m=65536,t=3,p=4$YW5hc2FsdA$hashana0000005'),
    (N'lucas.mendes', N'lucas@email.com',    N'$argon2id$v=19$m=65536,t=3,p=4$bHVjYXNzYWx0$hashlucas00006');

-- Promover usuário 1 a administrador
INSERT INTO dbo.Administradores (usuarioId) VALUES (1);

-- ------------------------------------------------------------
-- Acessos (log de logins)
-- ------------------------------------------------------------
INSERT INTO dbo.Acessos (usuarioId, dataHora) VALUES
    (1, '2026-04-01 08:00:00'),
    (2, '2026-04-01 09:15:00'),
    (3, '2026-04-01 10:30:00'),
    (2, '2026-04-02 14:00:00'),
    (4, '2026-04-03 18:45:00'),
    (5, '2026-04-04 20:00:00'),
    (6, '2026-04-05 11:20:00'),
    (1, '2026-04-06 08:05:00');

-- ------------------------------------------------------------
-- Jogos
-- ------------------------------------------------------------
INSERT INTO dbo.Jogos (nome, categoriaId, descricao, dataLancamento, visivel, ativo) VALUES
    -- RPG (categoriaId = 1)
    (N'Realm of Eternity',    1, N'RPG de mundo aberto com mais de 100 horas de conteúdo e sistema de crafting profundo.',          '2024-03-15', 1, 1),
    (N'Chronicles of Ashan',  1, N'RPG tático em turnos com narrativa ramificada e 12 classes de personagem.',                       '2023-11-01', 1, 1),

    -- Ação (categoriaId = 2)
    (N'Steel Nemesis',        2, N'Jogo de ação frenética com combates estilo hack-and-slash e modo co-op online para 4 jogadores.', '2025-06-20', 1, 1),
    (N'Neon Blade',           2, N'Ação cyberpunk em terceira pessoa ambientada em megalópole futurista.',                            '2025-01-10', 1, 1),

    -- Aventura (categoriaId = 3)
    (N'Lost Expedition',      3, N'Aventura narrativa com puzzles ambientais e exploração de ruínas antigas.',                       '2024-08-05', 1, 1),
    (N'Echoes of the Deep',   3, N'Aventura de exploração submarina com elementos de terror suave.',                                  '2023-05-18', 1, 1),

    -- Estratégia (categoriaId = 4)
    (N'Empire Builders',      4, N'Grand strategy com diplomacia, economia e guerras em mapa global.',                               '2022-10-30', 1, 1),
    (N'Tactics Reborn',       4, N'Estratégia tática em grade com campanha single-player e ranking online.',                         '2024-12-01', 1, 1),

    -- Esportes (categoriaId = 5)
    (N'ProLeague Football',   5, N'Simulação realista de futebol com modos carreira, ultimate team e ligas online.',                  '2025-09-01', 1, 1),

    -- Terror (categoriaId = 6)
    (N'Haunted Manor',        6, N'Horror psicológico em primeira pessoa ambientado em mansão vitoriana.',                            '2023-10-31', 1, 1),

    -- Jogo inativo (excluído logicamente)
    (N'Abandoned Project',    2, N'Jogo cancelado antes do lançamento completo.',                                                    '2022-01-01', 0, 0);

-- ------------------------------------------------------------
-- Tags por Jogo
-- ------------------------------------------------------------
INSERT INTO dbo.TagsPorJogo (idJogo, idTag) VALUES
    -- Realm of Eternity: open-world, single-player, fantasia
    (1, 2), (1, 3), (1, 9),
    -- Chronicles of Ashan: single-player, fantasia, turn-based (sem tag específica, reusa fantasia)
    (2, 2), (2, 9),
    -- Steel Nemesis: multiplayer, online, cooperativo
    (3, 1), (3, 5), (3, 6),
    -- Neon Blade: single-player, sci-fi
    (4, 2), (4, 8),
    -- Lost Expedition: single-player, aventura
    (5, 2),
    -- Echoes of the Deep: single-player, sobrevivência
    (6, 2), (6, 10),
    -- Empire Builders: multiplayer, online, estratégia
    (7, 1), (7, 5),
    -- Tactics Reborn: single-player, online
    (8, 2), (8, 5),
    -- ProLeague Football: multiplayer, online, esportes
    (9, 1), (9, 5),
    -- Haunted Manor: single-player
    (10, 2);

-- ------------------------------------------------------------
-- Resources (assets dos jogos)
-- ------------------------------------------------------------
INSERT INTO dbo.Resources (jogoId, tipoResource, descricao, endereco) VALUES
    (1,  N'imagem',      N'Capa do jogo',         N'https://cdn.fcg.com/jogos/1/capa.jpg'),
    (1,  N'trailer',     N'Trailer de lançamento',N'https://cdn.fcg.com/jogos/1/trailer.mp4'),
    (2,  N'imagem',      N'Capa do jogo',         N'https://cdn.fcg.com/jogos/2/capa.jpg'),
    (3,  N'imagem',      N'Capa do jogo',         N'https://cdn.fcg.com/jogos/3/capa.jpg'),
    (3,  N'trailer',     N'Gameplay trailer',     N'https://cdn.fcg.com/jogos/3/trailer.mp4'),
    (3,  N'screenshot',  N'Screenshot combate',   N'https://cdn.fcg.com/jogos/3/ss01.jpg'),
    (4,  N'imagem',      N'Capa do jogo',         N'https://cdn.fcg.com/jogos/4/capa.jpg'),
    (5,  N'imagem',      N'Capa do jogo',         N'https://cdn.fcg.com/jogos/5/capa.jpg'),
    (6,  N'imagem',      N'Capa do jogo',         N'https://cdn.fcg.com/jogos/6/capa.jpg'),
    (7,  N'imagem',      N'Capa do jogo',         N'https://cdn.fcg.com/jogos/7/capa.jpg'),
    (8,  N'imagem',      N'Capa do jogo',         N'https://cdn.fcg.com/jogos/8/capa.jpg'),
    (9,  N'imagem',      N'Capa do jogo',         N'https://cdn.fcg.com/jogos/9/capa.jpg'),
    (10, N'imagem',      N'Capa do jogo',         N'https://cdn.fcg.com/jogos/10/capa.jpg'),
    (10, N'trailer',     N'Teaser de terror',     N'https://cdn.fcg.com/jogos/10/trailer.mp4');

-- ------------------------------------------------------------
-- Preços dos Jogos
-- Preço base + versão com promoção ativa (Mega Sale Abril = id 3)
-- ------------------------------------------------------------
INSERT INTO dbo.PrecosJogos (jogoId, valor, promocaoId, percDesconto, dataInicio) VALUES
    -- Realm of Eternity
    (1,  199.90, NULL, NULL,  '2024-03-15'),   -- preço original lançamento
    (1,  199.90, 3,    40.00, '2026-04-01'),   -- com Mega Sale Abril (R$119,94)

    -- Chronicles of Ashan
    (2,  149.90, NULL, NULL,  '2023-11-01'),
    (2,  149.90, 3,    40.00, '2026-04-01'),   -- (R$89,94)

    -- Steel Nemesis
    (3,  129.90, NULL, NULL,  '2025-06-20'),
    (3,  129.90, 3,    40.00, '2026-04-01'),   -- (R$77,94)

    -- Neon Blade
    (4,  119.90, NULL, NULL,  '2025-01-10'),

    -- Lost Expedition
    (5,   79.90, NULL, NULL,  '2024-08-05'),

    -- Echoes of the Deep
    (6,   59.90, NULL, NULL,  '2023-05-18'),
    (6,   59.90, 3,    40.00, '2026-04-01'),   -- (R$35,94)

    -- Empire Builders
    (7,   99.90, NULL, NULL,  '2022-10-30'),

    -- Tactics Reborn
    (8,   89.90, NULL, NULL,  '2024-12-01'),
    (8,   89.90, 3,    40.00, '2026-04-01'),   -- (R$53,94)

    -- ProLeague Football
    (9,  249.90, NULL, NULL,  '2025-09-01'),

    -- Haunted Manor
    (10,  49.90, NULL, NULL,  '2023-10-31'),
    (10,  49.90, 3,    40.00, '2026-04-01');   -- (R$29,94)

-- ------------------------------------------------------------
-- Wishlist
-- ------------------------------------------------------------
INSERT INTO dbo.ListasDeDesejos (idUsuario, idJogo) VALUES
    (2, 1),  -- joao quer Realm of Eternity
    (2, 4),  -- joao quer Neon Blade
    (3, 3),  -- maria quer Steel Nemesis
    (3, 9),  -- maria quer ProLeague Football
    (4, 1),  -- pedro quer Realm of Eternity
    (4, 7),  -- pedro quer Empire Builders
    (5, 10), -- ana quer Haunted Manor
    (6, 2);  -- lucas quer Chronicles of Ashan

-- ------------------------------------------------------------
-- Transações (compras concluídas)
-- ------------------------------------------------------------
INSERT INTO dbo.Transacoes (integradoraPagamentoId, dataCompra, valorTotal, comprovante) VALUES
    (1, '2026-01-20 14:32:10', 199.90, N'STR-2026012001-ABC123'),  -- joao via Stripe
    (2, '2026-02-05 09:10:45', 209.80, N'PGS-2026020501-DEF456'),  -- maria via PagSeguro (2 jogos)
    (1, '2026-03-18 18:55:20',  99.90, N'STR-2026031801-GHI789'),  -- pedro via Stripe
    (3, '2026-04-02 21:00:00', 153.88, N'PPL-2026040201-JKL012'),  -- ana via PayPal (2 jogos com desconto)
    (1, '2026-04-03 11:30:00',  89.94, N'STR-2026040301-MNO345');  -- lucas via Stripe (com desconto)

-- ------------------------------------------------------------
-- Jogos Adquiridos (biblioteca)
-- valorUnitario = preço pago no momento da compra
-- ------------------------------------------------------------
INSERT INTO dbo.JogosAdquiridos (transacaoId, usuarioId, jogoId, quantidade, valorUnitario, valorTotal) VALUES
    -- Transação 1: joao comprou Realm of Eternity (preço cheio)
    (1, 2, 1,  1, 199.90, 199.90),

    -- Transação 2: maria comprou Steel Nemesis + Echoes of the Deep (preço cheio)
    (2, 3, 3,  1, 149.90, 149.90),
    (2, 3, 6,  1,  59.90,  59.90),

    -- Transação 3: pedro comprou Empire Builders (preço cheio)
    (3, 4, 7,  1,  99.90,  99.90),

    -- Transação 4: ana comprou Haunted Manor + Tactics Reborn (com Mega Sale 40%)
    (4, 5, 10, 1,  29.94,  29.94),
    (4, 5, 8,  1,  53.94,  53.94),

    -- Transação 5: lucas comprou Chronicles of Ashan (com Mega Sale 40%)
    (5, 6, 2,  1,  89.94,  89.94);

-- ------------------------------------------------------------
-- Carrinho atual (itens ainda não comprados)
-- ------------------------------------------------------------
INSERT INTO dbo.Carrinhos (usuarioId, jogoId, quantidade) VALUES
    (2, 4,  1),  -- joao tem Neon Blade no carrinho
    (3, 9,  1),  -- maria tem ProLeague Football no carrinho
    (4, 1,  1),  -- pedro tem Realm of Eternity no carrinho
    (5, 3,  1),  -- ana tem Steel Nemesis no carrinho
    (6, 5,  1);  -- lucas tem Lost Expedition no carrinho

GO

-- ============================================================
-- CONSULTAS DE VERIFICAÇÃO
-- ============================================================

-- Loja: jogos visíveis com preço atual e desconto ativo
SELECT
    j.id,
    j.nome                                                              AS jogo,
    c.nome                                                              AS categoria,
    p.valor                                                             AS precoBase,
    p.percDesconto,
    CAST(p.valor * (1.0 - ISNULL(p.percDesconto, 0) / 100.0)
         AS DECIMAL(10,2))                                              AS precoFinal,
    pr.nome                                                             AS promocao
FROM dbo.Jogos j
INNER JOIN dbo.Categorias c ON c.id = j.categoriaId
CROSS APPLY (
    SELECT TOP 1 pj.valor, pj.percDesconto, pj.promocaoId
    FROM   dbo.PrecosJogos pj
    WHERE  pj.jogoId = j.id
    ORDER  BY pj.dataInicio DESC
) p
LEFT JOIN dbo.Promocoes pr ON pr.id = p.promocaoId
WHERE j.visivel = 1 AND j.ativo = 1
ORDER BY j.nome;

GO

-- Biblioteca de um usuário específico (usuarioId = 2 — joao)
SELECT
    j.nome           AS jogo,
    c.nome           AS categoria,
    ja.valorUnitario AS valorPago,
    t.dataCompra,
    i.nome           AS gateway
FROM   dbo.JogosAdquiridos ja
INNER JOIN dbo.Jogos                  j ON j.id  = ja.jogoId
INNER JOIN dbo.Categorias             c ON c.id  = j.categoriaId
INNER JOIN dbo.Transacoes             t ON t.id  = ja.transacaoId
INNER JOIN dbo.IntegradorasPagamento  i ON i.id  = t.integradoraPagamentoId
WHERE  ja.usuarioId = 2
ORDER  BY t.dataCompra DESC;

GO

-- Relatório: receita total por gateway de pagamento
SELECT
    i.nome          AS gateway,
    COUNT(t.id)     AS totalTransacoes,
    SUM(t.valorTotal) AS receitaTotal
FROM   dbo.Transacoes            t
INNER JOIN dbo.IntegradorasPagamento i ON i.id = t.integradoraPagamentoId
GROUP BY i.nome
ORDER BY receitaTotal DESC;

GO

-- Top 5 jogos mais adquiridos
SELECT TOP 5
    j.nome,
    COUNT(ja.id)       AS totalVendas,
    SUM(ja.valorTotal) AS receitaGerada
FROM   dbo.JogosAdquiridos ja
INNER JOIN dbo.Jogos j ON j.id = ja.jogoId
GROUP BY j.nome
ORDER BY totalVendas DESC, receitaGerada DESC;

GO

-- Jogos na wishlist que já entraram em promoção (usuário pode ser notificado)
SELECT
    u.nomeUsuario,
    u.email,
    j.nome              AS jogo,
    pj.percDesconto      AS descontoAtual,
    CAST(pj.valor * (1.0 - pj.percDesconto / 100.0)
         AS DECIMAL(10,2)) AS precoFinal,
    pr.dataFim          AS fimPromocao
FROM   dbo.ListasDeDesejos ld
INNER JOIN dbo.Usuarios  u  ON u.id  = ld.idUsuario
INNER JOIN dbo.Jogos     j  ON j.id  = ld.idJogo
CROSS APPLY (
    SELECT TOP 1 pj2.valor, pj2.percDesconto, pj2.promocaoId
    FROM   dbo.PrecosJogos pj2
    WHERE  pj2.jogoId = j.id
    ORDER  BY pj2.dataInicio DESC
) pj
INNER JOIN dbo.Promocoes pr ON pr.id = pj.promocaoId
WHERE  pj.percDesconto > 0
  AND  GETUTCDATE() BETWEEN pr.dataInicio AND pr.dataFim
  AND  j.visivel = 1 AND j.ativo = 1
ORDER BY u.nomeUsuario, j.nome;

GO
