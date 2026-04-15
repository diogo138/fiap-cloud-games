# Documentação do Banco de Dados — GameStore

> Plataforma de loja e biblioteca de jogos digitais.  
> Gerado em: 2026-04-06

---

## Sumário

1. [Visão Geral](#visão-geral)
2. [Diagrama de Entidades](#diagrama-de-entidades)
3. [Tabelas](#tabelas)
   - [Usuarios](#usuarios)
   - [Administradores](#administradores)
   - [Acessos](#acessos)
   - [Jogos](#jogos)
   - [Categorias](#categorias)
   - [Tags](#tags)
   - [TagsPorJogo](#tagsprojogo)
   - [Resources](#resources)
   - [PrecosJogos](#precosjogos)
   - [Promocoes](#promocoes)
   - [ListasDeDesejos](#listasdedesejos)
   - [Carrinhos](#carrinhos)
   - [Transacoes](#transacoes)
   - [JogosAdquiridos](#jogosadquiridos)
   - [IntegradorasPagamento](#integradoraspagamento)
4. [Relacionamentos](#relacionamentos)
5. [Regras de Negócio](#regras-de-negócio)
6. [Script DDL](#script-ddl)

---

## Visão Geral

O banco de dados suporta uma plataforma de distribuição digital de jogos com os seguintes domínios:

| Domínio | Responsabilidade |
|---|---|
| **Identidade** | Cadastro, autenticação e controle de acesso de usuários |
| **Catálogo** | Gerenciamento de jogos, categorias, tags e recursos (assets) |
| **Precificação** | Preços históricos e promoções por jogo |
| **Aquisição** | Carrinho, transações financeiras e biblioteca do usuário |
| **Integração** | Gateway de pagamento externo |

---

## Diagrama de Entidades

```
Usuarios ──────────────── Administradores (1:1)
   │
   ├──────────────────── Acessos (1:N)
   ├──────────────────── ListasDeDesejos (N:N via Jogos)
   ├──────────────────── Carrinhos (1:N)
   └──────────────────── JogosAdquiridos (1:N)

Jogos ─────────────────── Categorias (N:1)
   │
   ├──────────────────── TagsPorJogo ──── Tags (N:N)
   ├──────────────────── Resources (1:N)
   ├──────────────────── PrecosJogos ─── Promocoes (N:1)
   ├──────────────────── ListasDeDesejos (N:N via Usuarios)
   ├──────────────────── Carrinhos (1:N)
   └──────────────────── JogosAdquiridos (1:N)

Transacoes ─────────────── IntegradorasPagamento (N:1)
   └──────────────────── JogosAdquiridos (1:N)
```

---

## Tabelas

---

### Usuarios

Armazena todos os usuários da plataforma (clientes e administradores).

| Coluna | Tipo sugerido | Nulável | Descrição |
|---|---|---|---|
| `id` | `INT IDENTITY` | NÃO | Chave primária |
| `nomeUsuario` | `NVARCHAR(100)` | NÃO | Nome de exibição do usuário |
| `email` | `NVARCHAR(255)` | NÃO | E-mail único — usado no login |
| `hashSenha` | `NVARCHAR(512)` | NÃO | Hash bcrypt/Argon2 da senha |
| `dataCadastro` | `DATETIME2` | NÃO | Data e hora do cadastro |
| `ativo` | `BIT` | NÃO | Soft delete — `1` ativo, `0` inativo |

**Constraints:**
- `PK_Usuarios` → `id`
- `UQ_Usuarios_Email` → `email` (unicidade)

**Índices recomendados:**
- `IX_Usuarios_Email` em `email` — filtro de autenticação
- `IX_Usuarios_Ativo` em `ativo` — filtros de listagem

**Observações:**
> `hashSenha` jamais deve armazenar senha em texto plano. A aplicação é responsável pelo hashing antes da persistência. Recomenda-se uso de Argon2id com salt único por usuário.

---

### Administradores

Marca quais usuários possuem papel de administrador. Extensão de `Usuarios` com relação 1:1.

| Coluna | Tipo sugerido | Nulável | Descrição |
|---|---|---|---|
| `usuarioId` | `INT` | NÃO | PK e FK para `Usuarios.id` |
| `dataCadastro` | `DATETIME2` | NÃO | Data em que o papel foi concedido |

**Constraints:**
- `PK_Administradores` → `usuarioId`
- `FK_Administradores_Usuarios` → `usuarioId` → `Usuarios(id)`

**Observações:**
> O papel de administrador é verificado pela presença do `usuarioId` nesta tabela, alinhado ao controle de roles JWT da aplicação (`ROLE_ADMIN`).

---

### Acessos

Log de autenticações bem-sucedidas. Rastreabilidade de sessões por usuário.

| Coluna | Tipo sugerido | Nulável | Descrição |
|---|---|---|---|
| `id` | `INT IDENTITY` | NÃO | Chave primária |
| `usuarioId` | `INT` | NÃO | FK para `Usuarios.id` |
| `dataHora` | `DATETIME2` | NÃO | Timestamp do acesso |

**Constraints:**
- `PK_Acessos` → `id`
- `FK_Acessos_Usuarios` → `usuarioId` → `Usuarios(id)`

**Índices recomendados:**
- `IX_Acessos_UsuarioId_DataHora` em `(usuarioId, dataHora DESC)` — consultas por usuário e período

**Observações:**
> Tabela de alta gravação. Avaliar particionamento por `dataHora` em produção com volume elevado. Implementar política de retenção (ex: 90 dias via SQL Server Agent Job).

---

### Jogos

Catálogo central de jogos da plataforma.

| Coluna | Tipo sugerido | Nulável | Descrição |
|---|---|---|---|
| `id` | `INT IDENTITY` | NÃO | Chave primária |
| `nome` | `NVARCHAR(255)` | NÃO | Nome do jogo |
| `categoriaId` | `INT` | NÃO | FK para `Categorias.id` |
| `descricao` | `NVARCHAR(MAX)` | SIM | Descrição longa do jogo |
| `dataCadastro` | `DATETIME2` | NÃO | Data de cadastro no sistema |
| `dataLancamento` | `DATE` | SIM | Data oficial de lançamento do jogo |
| `visivel` | `BIT` | NÃO | Controla visibilidade na loja (`1` = visível) |
| `ativo` | `BIT` | NÃO | Soft delete — `1` ativo, `0` excluído |

**Constraints:**
- `PK_Jogos` → `id`
- `FK_Jogos_Categorias` → `categoriaId` → `Categorias(id)`

**Índices recomendados:**
- `IX_Jogos_CategoriaId` em `categoriaId`
- `IX_Jogos_Visivel_Ativo` em `(visivel, ativo)` — filtro principal da loja
- `IX_Jogos_Nome` em `nome` — pesquisa por nome

**Observações:**
> Exclusão de jogos é lógica (`ativo = 0`), conforme regra de negócio definida no Event Storming. Jogos inativos não devem aparecer na loja nem ser adquiridos, mas são mantidos no histórico de `JogosAdquiridos`.

---

### Categorias

Classificação dos jogos (ex: RPG, Ação, Estratégia).

| Coluna | Tipo sugerido | Nulável | Descrição |
|---|---|---|---|
| `id` | `INT IDENTITY` | NÃO | Chave primária |
| `nome` | `NVARCHAR(100)` | NÃO | Nome da categoria |
| `descricao` | `NVARCHAR(500)` | SIM | Descrição da categoria |
| `dataCadastro` | `DATETIME2` | NÃO | Data de cadastro |
| `ativo` | `BIT` | NÃO | Soft delete |

**Constraints:**
- `PK_Categorias` → `id`
- `UQ_Categorias_Nome` → `nome`

---

### Tags

Palavras-chave associadas aos jogos para filtragem e descoberta.

| Coluna | Tipo sugerido | Nulável | Descrição |
|---|---|---|---|
| `id` | `INT IDENTITY` | NÃO | Chave primária |
| `nome` | `NVARCHAR(100)` | NÃO | Nome da tag (ex: `multiplayer`, `open-world`) |
| `dataCadastro` | `DATETIME2` | NÃO | Data de cadastro |
| `ativo` | `BIT` | NÃO | Soft delete |

**Constraints:**
- `PK_Tags` → `id`
- `UQ_Tags_Nome` → `nome`

---

### TagsPorJogo

Tabela de associação N:N entre `Jogos` e `Tags`.

| Coluna | Tipo sugerido | Nulável | Descrição |
|---|---|---|---|
| `idJogo` | `INT` | NÃO | FK para `Jogos.id` (parte da PK composta) |
| `idTag` | `INT` | NÃO | FK para `Tags.id` (parte da PK composta) |

**Constraints:**
- `PK_TagsPorJogo` → `(idJogo, idTag)`
- `FK_TagsPorJogo_Jogos` → `idJogo` → `Jogos(id)`
- `FK_TagsPorJogo_Tags` → `idTag` → `Tags(id)`

**Índices recomendados:**
- `IX_TagsPorJogo_IdTag` em `idTag` — busca de jogos por tag

---

### Resources

Recursos digitais associados a um jogo: imagens, trailers, executáveis, etc.

| Coluna | Tipo sugerido | Nulável | Descrição |
|---|---|---|---|
| `id` | `INT IDENTITY` | NÃO | Chave primária |
| `jogoId` | `INT` | NÃO | FK para `Jogos.id` |
| `tipoResource` | `NVARCHAR(50)` | NÃO | Tipo do recurso (ex: `imagem`, `trailer`, `executavel`) |
| `descricao` | `NVARCHAR(255)` | SIM | Descrição do recurso |
| `endereco` | `NVARCHAR(1000)` | NÃO | URL ou caminho do arquivo |
| `dataCadastro` | `DATETIME2` | NÃO | Data de cadastro |
| `ativo` | `BIT` | NÃO | Soft delete |

**Constraints:**
- `PK_Resources` → `id`
- `FK_Resources_Jogos` → `jogoId` → `Jogos(id)`

**Índices recomendados:**
- `IX_Resources_JogoId_Tipo` em `(jogoId, tipoResource)` — carregamento de assets por jogo

---

### PrecosJogos

Histórico de preços de cada jogo, com suporte a promoções.

| Coluna | Tipo sugerido | Nulável | Descrição |
|---|---|---|---|
| `id` | `INT IDENTITY` | NÃO | Chave primária |
| `jogoId` | `INT` | NÃO | FK para `Jogos.id` |
| `valor` | `DECIMAL(10,2)` | NÃO | Preço base (sem desconto) |
| `promocaoId` | `INT` | SIM | FK para `Promocoes.id` — nulo se sem promoção ativa |
| `percDesconto` | `DECIMAL(5,2)` | SIM | Percentual de desconto efetivo (0.00 a 100.00) |
| `dataInicio` | `DATETIME2` | NÃO | Início da vigência deste preço |

**Constraints:**
- `PK_PrecosJogos` → `id`
- `FK_PrecosJogos_Jogos` → `jogoId` → `Jogos(id)`
- `FK_PrecosJogos_Promocoes` → `promocaoId` → `Promocoes(id)`

**Índices recomendados:**
- `IX_PrecosJogos_JogoId_DataInicio` em `(jogoId, dataInicio DESC)` — preço vigente por jogo

**Observações:**
> Para obter o preço atual de um jogo, selecionar o registro com maior `dataInicio` para o `jogoId`. O preço final é `valor * (1 - percDesconto / 100)` quando `promocaoId` não é nulo.

---

### Promocoes

Campanhas de desconto aplicáveis aos preços dos jogos.

| Coluna | Tipo sugerido | Nulável | Descrição |
|---|---|---|---|
| `id` | `INT IDENTITY` | NÃO | Chave primária |
| `nome` | `NVARCHAR(200)` | NÃO | Nome da promoção (ex: `Promoção de Verão`) |
| `dataCadastro` | `DATETIME2` | NÃO | Data de criação |
| `dataInicio` | `DATETIME2` | NÃO | Início da vigência |
| `dataFim` | `DATETIME2` | NÃO | Fim da vigência |
| `percDesconto` | `DECIMAL(5,2)` | NÃO | Percentual de desconto padrão da promoção |

**Constraints:**
- `PK_Promocoes` → `id`
- `CK_Promocoes_Datas` → `dataFim > dataInicio`

**Índices recomendados:**
- `IX_Promocoes_Vigencia` em `(dataInicio, dataFim)` — filtro de promoções ativas

---

### ListasDeDesejos

Lista de desejos (wishlist) de cada usuário — N:N entre `Usuarios` e `Jogos`.

| Coluna | Tipo sugerido | Nulável | Descrição |
|---|---|---|---|
| `idUsuario` | `INT` | NÃO | FK para `Usuarios.id` (parte da PK composta) |
| `idJogo` | `INT` | NÃO | FK para `Jogos.id` (parte da PK composta) |

**Constraints:**
- `PK_ListasDeDesejos` → `(idUsuario, idJogo)`
- `FK_ListasDeDesejos_Usuarios` → `idUsuario` → `Usuarios(id)`
- `FK_ListasDeDesejos_Jogos` → `idJogo` → `Jogos(id)`

**Índices recomendados:**
- `IX_ListasDeDesejos_IdJogo` em `idJogo` — contagem de interesse por jogo

---

### Carrinhos

Itens no carrinho de compras de cada usuário (estado pré-transação).

| Coluna | Tipo sugerido | Nulável | Descrição |
|---|---|---|---|
| `id` | `INT IDENTITY` | NÃO | Chave primária |
| `usuarioId` | `INT` | NÃO | FK para `Usuarios.id` |
| `jogoId` | `INT` | NÃO | FK para `Jogos.id` |
| `quantidade` | `SMALLINT` | NÃO | Quantidade do item (normalmente `1` para jogos digitais) |
| `dataCadastro` | `DATETIME2` | NÃO | Data em que o item foi adicionado |

**Constraints:**
- `PK_Carrinhos` → `id`
- `FK_Carrinhos_Usuarios` → `usuarioId` → `Usuarios(id)`
- `FK_Carrinhos_Jogos` → `jogoId` → `Jogos(id)`
- `UQ_Carrinhos_Usuario_Jogo` → `(usuarioId, jogoId)` — evita duplicidade por usuário

**Índices recomendados:**
- `IX_Carrinhos_UsuarioId` em `usuarioId` — carregamento do carrinho

---

### Transacoes

Registro financeiro de cada compra concluída.

| Coluna | Tipo sugerido | Nulável | Descrição |
|---|---|---|---|
| `id` | `INT IDENTITY` | NÃO | Chave primária |
| `integradoraPagamentoId` | `INT` | NÃO | FK para `IntegradorasPagamento.id` |
| `dataCompra` | `DATETIME2` | NÃO | Timestamp da transação aprovada |
| `valorTotal` | `DECIMAL(10,2)` | NÃO | Valor total pago na transação |
| `comprovante` | `NVARCHAR(500)` | SIM | Código/ID de comprovante retornado pelo gateway |

**Constraints:**
- `PK_Transacoes` → `id`
- `FK_Transacoes_IntegradorasPagamento` → `integradoraPagamentoId` → `IntegradorasPagamento(id)`

**Índices recomendados:**
- `IX_Transacoes_DataCompra` em `dataCompra DESC` — relatórios financeiros
- `IX_Transacoes_Comprovante` em `comprovante` — reconciliação com gateway

---

### JogosAdquiridos

Biblioteca permanente do usuário — registra cada item adquirido em uma transação.

| Coluna | Tipo sugerido | Nulável | Descrição |
|---|---|---|---|
| `id` | `INT IDENTITY` | NÃO | Chave primária |
| `transacaoId` | `INT` | NÃO | FK para `Transacoes.id` |
| `usuarioId` | `INT` | NÃO | FK para `Usuarios.id` |
| `jogoId` | `INT` | NÃO | FK para `Jogos.id` |
| `quantidade` | `SMALLINT` | NÃO | Quantidade adquirida |
| `valorUnitario` | `DECIMAL(10,2)` | NÃO | Preço unitário no momento da compra |
| `valorTotal` | `DECIMAL(10,2)` | NÃO | `quantidade * valorUnitario` |

**Constraints:**
- `PK_JogosAdquiridos` → `id`
- `FK_JogosAdquiridos_Transacoes` → `transacaoId` → `Transacoes(id)`
- `FK_JogosAdquiridos_Usuarios` → `usuarioId` → `Usuarios(id)`
- `FK_JogosAdquiridos_Jogos` → `jogoId` → `Jogos(id)`
- `UQ_JogosAdquiridos_Usuario_Jogo` → `(usuarioId, jogoId)` — impede duplicata de jogo na biblioteca

**Índices recomendados:**
- `IX_JogosAdquiridos_UsuarioId` em `usuarioId` — carregamento da biblioteca
- `IX_JogosAdquiridos_JogoId` em `jogoId` — verificação de posse por jogo

**Observações:**
> `valorUnitario` deve ser gravado no momento da compra, independente de alterações futuras de preço. Esta é a fonte de verdade histórica do valor pago.

---

### IntegradorasPagamento

Cadastro dos gateways de pagamento disponíveis na plataforma (ex: Stripe, PayPal, PagSeguro).

| Coluna | Tipo sugerido | Nulável | Descrição |
|---|---|---|---|
| `id` | `INT IDENTITY` | NÃO | Chave primária |
| `nome` | `NVARCHAR(100)` | NÃO | Nome do gateway |
| `dataCadastro` | `DATETIME2` | NÃO | Data de cadastro |
| `ativo` | `BIT` | NÃO | Soft delete — `1` disponível para uso |

**Constraints:**
- `PK_IntegradorasPagamento` → `id`
- `UQ_IntegradorasPagamento_Nome` → `nome`

---

## Relacionamentos

| Tabela Filho | Coluna(s) FK | Tabela Pai | Coluna PK | Cardinalidade |
|---|---|---|---|---|
| `Administradores` | `usuarioId` | `Usuarios` | `id` | 1:1 |
| `Acessos` | `usuarioId` | `Usuarios` | `id` | N:1 |
| `ListasDeDesejos` | `idUsuario` | `Usuarios` | `id` | N:1 |
| `ListasDeDesejos` | `idJogo` | `Jogos` | `id` | N:1 |
| `Carrinhos` | `usuarioId` | `Usuarios` | `id` | N:1 |
| `Carrinhos` | `jogoId` | `Jogos` | `id` | N:1 |
| `JogosAdquiridos` | `usuarioId` | `Usuarios` | `id` | N:1 |
| `JogosAdquiridos` | `jogoId` | `Jogos` | `id` | N:1 |
| `JogosAdquiridos` | `transacaoId` | `Transacoes` | `id` | N:1 |
| `Jogos` | `categoriaId` | `Categorias` | `id` | N:1 |
| `TagsPorJogo` | `idJogo` | `Jogos` | `id` | N:1 |
| `TagsPorJogo` | `idTag` | `Tags` | `id` | N:1 |
| `Resources` | `jogoId` | `Jogos` | `id` | N:1 |
| `PrecosJogos` | `jogoId` | `Jogos` | `id` | N:1 |
| `PrecosJogos` | `promocaoId` | `Promocoes` | `id` | N:1 |
| `Transacoes` | `integradoraPagamentoId` | `IntegradorasPagamento` | `id` | N:1 |

---

## Regras de Negócio

| # | Regra | Implementação |
|---|---|---|
| RN01 | E-mail único por usuário | `UQ_Usuarios_Email` |
| RN02 | Senha com requisitos mínimos | Validação na aplicação — hash antes de persistir |
| RN03 | Usuário não pode adquirir o mesmo jogo duas vezes | `UQ_JogosAdquiridos_Usuario_Jogo` |
| RN04 | Jogo excluído mantém histórico de aquisições | Soft delete (`ativo = 0`) em `Jogos` |
| RN05 | Preço no momento da compra é imutável | `valorUnitario` gravado em `JogosAdquiridos` |
| RN06 | Promoção tem período de vigência obrigatório | `CK_Promocoes_Datas` → `dataFim > dataInicio` |
| RN07 | Jogo invisível não aparece na loja, mas pode ser mantido no catálogo | Filtro `visivel = 1 AND ativo = 1` nas queries da loja |
| RN08 | Pagamento realizado com sucesso antes de inserir em JogosAdquiridos | Fluxo transacional na camada de aplicação |

---

## Script DDL

```sql
-- ============================================================
-- GameStore — DDL completo
-- SQL Server 2019+ | Compatibility Level 150
-- ============================================================

CREATE TABLE dbo.Categorias (
    id           INT            NOT NULL IDENTITY(1,1),
    nome         NVARCHAR(100)  NOT NULL,
    descricao    NVARCHAR(500)  NULL,
    dataCadastro DATETIME2      NOT NULL CONSTRAINT DF_Categorias_DataCadastro DEFAULT GETUTCDATE(),
    ativo        BIT            NOT NULL CONSTRAINT DF_Categorias_Ativo        DEFAULT 1,
    CONSTRAINT PK_Categorias    PRIMARY KEY (id),
    CONSTRAINT UQ_Categorias_Nome UNIQUE (nome)
);

CREATE TABLE dbo.Tags (
    id           INT            NOT NULL IDENTITY(1,1),
    nome         NVARCHAR(100)  NOT NULL,
    dataCadastro DATETIME2      NOT NULL CONSTRAINT DF_Tags_DataCadastro DEFAULT GETUTCDATE(),
    ativo        BIT            NOT NULL CONSTRAINT DF_Tags_Ativo        DEFAULT 1,
    CONSTRAINT PK_Tags     PRIMARY KEY (id),
    CONSTRAINT UQ_Tags_Nome UNIQUE (nome)
);

CREATE TABLE dbo.Promocoes (
    id           INT             NOT NULL IDENTITY(1,1),
    nome         NVARCHAR(200)   NOT NULL,
    dataCadastro DATETIME2       NOT NULL CONSTRAINT DF_Promocoes_DataCadastro DEFAULT GETUTCDATE(),
    dataInicio   DATETIME2       NOT NULL,
    dataFim      DATETIME2       NOT NULL,
    percDesconto DECIMAL(5,2)    NOT NULL,
    CONSTRAINT PK_Promocoes        PRIMARY KEY (id),
    CONSTRAINT CK_Promocoes_Datas  CHECK (dataFim > dataInicio),
    CONSTRAINT CK_Promocoes_Perc   CHECK (percDesconto BETWEEN 0 AND 100)
);

CREATE TABLE dbo.IntegradorasPagamento (
    id           INT           NOT NULL IDENTITY(1,1),
    nome         NVARCHAR(100) NOT NULL,
    dataCadastro DATETIME2     NOT NULL CONSTRAINT DF_IntegradorasPagamento_DataCadastro DEFAULT GETUTCDATE(),
    ativo        BIT           NOT NULL CONSTRAINT DF_IntegradorasPagamento_Ativo        DEFAULT 1,
    CONSTRAINT PK_IntegradorasPagamento    PRIMARY KEY (id),
    CONSTRAINT UQ_IntegradorasPagamento_Nome UNIQUE (nome)
);

CREATE TABLE dbo.Usuarios (
    id           INT            NOT NULL IDENTITY(1,1),
    nomeUsuario  NVARCHAR(100)  NOT NULL,
    email        NVARCHAR(255)  NOT NULL,
    hashSenha    NVARCHAR(512)  NOT NULL,
    dataCadastro DATETIME2      NOT NULL CONSTRAINT DF_Usuarios_DataCadastro DEFAULT GETUTCDATE(),
    ativo        BIT            NOT NULL CONSTRAINT DF_Usuarios_Ativo        DEFAULT 1,
    CONSTRAINT PK_Usuarios    PRIMARY KEY (id),
    CONSTRAINT UQ_Usuarios_Email UNIQUE (email)
);

CREATE INDEX IX_Usuarios_Email ON dbo.Usuarios (email);
CREATE INDEX IX_Usuarios_Ativo  ON dbo.Usuarios (ativo);

CREATE TABLE dbo.Administradores (
    usuarioId    INT       NOT NULL,
    dataCadastro DATETIME2 NOT NULL CONSTRAINT DF_Administradores_DataCadastro DEFAULT GETUTCDATE(),
    CONSTRAINT PK_Administradores         PRIMARY KEY (usuarioId),
    CONSTRAINT FK_Administradores_Usuarios FOREIGN KEY (usuarioId) REFERENCES dbo.Usuarios(id)
);

CREATE TABLE dbo.Acessos (
    id        INT       NOT NULL IDENTITY(1,1),
    usuarioId INT       NOT NULL,
    dataHora  DATETIME2 NOT NULL CONSTRAINT DF_Acessos_DataHora DEFAULT GETUTCDATE(),
    CONSTRAINT PK_Acessos         PRIMARY KEY (id),
    CONSTRAINT FK_Acessos_Usuarios FOREIGN KEY (usuarioId) REFERENCES dbo.Usuarios(id)
);

CREATE INDEX IX_Acessos_UsuarioId_DataHora ON dbo.Acessos (usuarioId, dataHora DESC);

CREATE TABLE dbo.Jogos (
    id              INT            NOT NULL IDENTITY(1,1),
    nome            NVARCHAR(255)  NOT NULL,
    categoriaId     INT            NOT NULL,
    descricao       NVARCHAR(MAX)  NULL,
    dataCadastro    DATETIME2      NOT NULL CONSTRAINT DF_Jogos_DataCadastro    DEFAULT GETUTCDATE(),
    dataLancamento  DATE           NULL,
    visivel         BIT            NOT NULL CONSTRAINT DF_Jogos_Visivel         DEFAULT 1,
    ativo           BIT            NOT NULL CONSTRAINT DF_Jogos_Ativo           DEFAULT 1,
    CONSTRAINT PK_Jogos          PRIMARY KEY (id),
    CONSTRAINT FK_Jogos_Categorias FOREIGN KEY (categoriaId) REFERENCES dbo.Categorias(id)
);

CREATE INDEX IX_Jogos_CategoriaId   ON dbo.Jogos (categoriaId);
CREATE INDEX IX_Jogos_Visivel_Ativo ON dbo.Jogos (visivel, ativo);
CREATE INDEX IX_Jogos_Nome          ON dbo.Jogos (nome);

CREATE TABLE dbo.TagsPorJogo (
    idJogo INT NOT NULL,
    idTag  INT NOT NULL,
    CONSTRAINT PK_TagsPorJogo      PRIMARY KEY (idJogo, idTag),
    CONSTRAINT FK_TagsPorJogo_Jogos FOREIGN KEY (idJogo) REFERENCES dbo.Jogos(id),
    CONSTRAINT FK_TagsPorJogo_Tags  FOREIGN KEY (idTag)  REFERENCES dbo.Tags(id)
);

CREATE INDEX IX_TagsPorJogo_IdTag ON dbo.TagsPorJogo (idTag);

CREATE TABLE dbo.Resources (
    id           INT             NOT NULL IDENTITY(1,1),
    jogoId       INT             NOT NULL,
    tipoResource NVARCHAR(50)    NOT NULL,
    descricao    NVARCHAR(255)   NULL,
    endereco     NVARCHAR(1000)  NOT NULL,
    dataCadastro DATETIME2       NOT NULL CONSTRAINT DF_Resources_DataCadastro DEFAULT GETUTCDATE(),
    ativo        BIT             NOT NULL CONSTRAINT DF_Resources_Ativo        DEFAULT 1,
    CONSTRAINT PK_Resources      PRIMARY KEY (id),
    CONSTRAINT FK_Resources_Jogos FOREIGN KEY (jogoId) REFERENCES dbo.Jogos(id)
);

CREATE INDEX IX_Resources_JogoId_Tipo ON dbo.Resources (jogoId, tipoResource);

CREATE TABLE dbo.PrecosJogos (
    id           INT          NOT NULL IDENTITY(1,1),
    jogoId       INT          NOT NULL,
    valor        DECIMAL(10,2) NOT NULL,
    promocaoId   INT          NULL,
    percDesconto DECIMAL(5,2)  NULL,
    dataInicio   DATETIME2    NOT NULL,
    CONSTRAINT PK_PrecosJogos          PRIMARY KEY (id),
    CONSTRAINT FK_PrecosJogos_Jogos     FOREIGN KEY (jogoId)     REFERENCES dbo.Jogos(id),
    CONSTRAINT FK_PrecosJogos_Promocoes FOREIGN KEY (promocaoId) REFERENCES dbo.Promocoes(id),
    CONSTRAINT CK_PrecosJogos_Valor     CHECK (valor >= 0),
    CONSTRAINT CK_PrecosJogos_Perc      CHECK (percDesconto IS NULL OR percDesconto BETWEEN 0 AND 100)
);

CREATE INDEX IX_PrecosJogos_JogoId_DataInicio ON dbo.PrecosJogos (jogoId, dataInicio DESC);

CREATE TABLE dbo.ListasDeDesejos (
    idUsuario INT NOT NULL,
    idJogo    INT NOT NULL,
    CONSTRAINT PK_ListasDeDesejos          PRIMARY KEY (idUsuario, idJogo),
    CONSTRAINT FK_ListasDeDesejos_Usuarios  FOREIGN KEY (idUsuario) REFERENCES dbo.Usuarios(id),
    CONSTRAINT FK_ListasDeDesejos_Jogos     FOREIGN KEY (idJogo)    REFERENCES dbo.Jogos(id)
);

CREATE INDEX IX_ListasDeDesejos_IdJogo ON dbo.ListasDeDesejos (idJogo);

CREATE TABLE dbo.Carrinhos (
    id           INT       NOT NULL IDENTITY(1,1),
    usuarioId    INT       NOT NULL,
    jogoId       INT       NOT NULL,
    quantidade   SMALLINT  NOT NULL CONSTRAINT DF_Carrinhos_Quantidade DEFAULT 1,
    dataCadastro DATETIME2 NOT NULL CONSTRAINT DF_Carrinhos_DataCadastro DEFAULT GETUTCDATE(),
    CONSTRAINT PK_Carrinhos           PRIMARY KEY (id),
    CONSTRAINT FK_Carrinhos_Usuarios   FOREIGN KEY (usuarioId) REFERENCES dbo.Usuarios(id),
    CONSTRAINT FK_Carrinhos_Jogos      FOREIGN KEY (jogoId)    REFERENCES dbo.Jogos(id),
    CONSTRAINT UQ_Carrinhos_Usr_Jogo   UNIQUE (usuarioId, jogoId)
);

CREATE INDEX IX_Carrinhos_UsuarioId ON dbo.Carrinhos (usuarioId);

CREATE TABLE dbo.Transacoes (
    id                      INT            NOT NULL IDENTITY(1,1),
    integradoraPagamentoId  INT            NOT NULL,
    dataCompra              DATETIME2      NOT NULL CONSTRAINT DF_Transacoes_DataCompra DEFAULT GETUTCDATE(),
    valorTotal              DECIMAL(10,2)  NOT NULL,
    comprovante             NVARCHAR(500)  NULL,
    CONSTRAINT PK_Transacoes                       PRIMARY KEY (id),
    CONSTRAINT FK_Transacoes_IntegradorasPagamento  FOREIGN KEY (integradoraPagamentoId)
        REFERENCES dbo.IntegradorasPagamento(id)
);

CREATE INDEX IX_Transacoes_DataCompra  ON dbo.Transacoes (dataCompra DESC);
CREATE INDEX IX_Transacoes_Comprovante ON dbo.Transacoes (comprovante);

CREATE TABLE dbo.JogosAdquiridos (
    id             INT           NOT NULL IDENTITY(1,1),
    transacaoId    INT           NOT NULL,
    usuarioId      INT           NOT NULL,
    jogoId         INT           NOT NULL,
    quantidade     SMALLINT      NOT NULL,
    valorUnitario  DECIMAL(10,2) NOT NULL,
    valorTotal     DECIMAL(10,2) NOT NULL,
    CONSTRAINT PK_JogosAdquiridos            PRIMARY KEY (id),
    CONSTRAINT FK_JogosAdquiridos_Transacoes  FOREIGN KEY (transacaoId) REFERENCES dbo.Transacoes(id),
    CONSTRAINT FK_JogosAdquiridos_Usuarios    FOREIGN KEY (usuarioId)   REFERENCES dbo.Usuarios(id),
    CONSTRAINT FK_JogosAdquiridos_Jogos       FOREIGN KEY (jogoId)      REFERENCES dbo.Jogos(id),
    CONSTRAINT UQ_JogosAdquiridos_Usr_Jogo    UNIQUE (usuarioId, jogoId)
);

CREATE INDEX IX_JogosAdquiridos_UsuarioId ON dbo.JogosAdquiridos (usuarioId);
CREATE INDEX IX_JogosAdquiridos_JogoId    ON dbo.JogosAdquiridos (jogoId);
```

---

*Documentação gerada por DBA Sênior — SQL Server | GameStore v1.0*
