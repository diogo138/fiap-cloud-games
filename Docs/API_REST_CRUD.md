# API REST - Endpoints CRUD (FCG)

Documento proposto de endpoints REST cobrindo **CRUD** para todas as tabelas do script `FCG.sql` (SQL Server).

- Base URL (sugestao): `/api/v1`
- Content-Type: `application/json`
- Paginacao (sugestao): `page` (1..N), `pageSize` (1..200)
- Ordenacao (sugestao): `sort=campo` (asc), `sort=-campo` (desc)
- Respostas HTTP (sugestao): `200`, `201`, `204`, `400`, `404`, `409`
- `DELETE` em tabelas com coluna `ativo`: preferir **soft delete** (setar `ativo=false`)

---

# Resumo dos Endpoints

## Diogo
- Autenticacao (`dbo.Acessos`)
- Usuarios (`dbo.Usuarios`) (`dbo.Administradores`) 

## Roberson
- Categorias (`dbo.Categorias`)
- Jogos (`dbo.Jogos`) (`dbo.PrecosJogos`)

## Jhonatan
- Promocoes (`dbo.Promocoes`)
- ListasDeDesejos (`dbo.ListasDeDesejos`)

## Glayson
- Carrinhos (`dbo.Carrinhos`) (`dbo.JogosAdquiridos`) (`dbo.Transacoes`)

# Endpoints

## Categorias (`dbo.Categorias`)

| Metodo | Endpoint | Operacao |
|---|---|---|
| GET | `/categorias` | Lista categorias (filtros: `ativo`, `q`, `page`, `pageSize`, `sort`) |
| POST | `/categorias` | Cria categoria |
| GET | `/categorias/{id}` | Obtem categoria por `id` |
| PUT/PATCH | `/categorias/{id}` | Atualiza categoria |
| DELETE | `/categorias/{id}` | Remove (soft delete: `ativo=false`) |

---


## Promocoes (`dbo.Promocoes`)

| Metodo | Endpoint | Operacao |
|---|---|---|
| GET | `/promocoes` | Lista promocoes (filtros: `dataInicio`, `dataFim`, `q`, `page`, `pageSize`, `sort`) |
| POST | `/promocoes` | Cria promocao |
| GET | `/promocoes/{id}` | Obtem promocao por `id` |
| PUT/PATCH | `/promocoes/{id}` | Atualiza promocao |
| DELETE | `/promocoes/{id}` | Remove promocao |
| POST | `/promocoes/{id}/jogo/{jogoId}` | Incluir um determinado jogo na promoção |
| DELETE | `/promocoes/{id}/jogo/{jogoId}` | Remoer um determinado jogo da promoção |

---

## Usuarios (`dbo.Usuarios`)

| Metodo | Endpoint | Operacao |
|---|---|---|
| GET | `/usuarios` | Lista usuarios (filtros: `ativo`, `email`, `q`, `page`, `pageSize`, `sort`) |
| POST | `/usuarios` | Cria usuario |
| GET | `/usuarios/{id}` | Obtem usuario por `id` |
| PUT/PATCH | `/usuarios/{id}` | Atualiza usuario (ex.: `nomeUsuario`, `email`, `hashSenha`, `ativo`) |
| DELETE | `/usuarios/{id}` | Remove (soft delete: `ativo=false`) |

Endpoints correlatos (tabelas dependentes):

| Metodo | Endpoint | Operacao |
|---|---|---|
| PUT | `/usuarios/{usuarioId}/admin` | Concede admin |
| DELETE | `/usuarios/{usuarioId}/admin` | Revoga admin |

---

## Jogos (`dbo.Jogos`)

| Metodo | Endpoint | Operacao |
|---|---|---|
| GET | `/jogos` | Lista jogos (filtros: `categoriaId`, `visivel`, `ativo`, `q`, `page`, `pageSize`, `sort`) |
| POST | `/jogos` | Cria jogo |
| GET | `/jogos/{id}` | Obtem jogo por `id` |
| PUT/PATCH | `/jogos/{id}` | Atualiza jogo |
| DELETE | `/jogos/{id}` | Remove (soft delete: `ativo=false`) |
| POST | `/jogos/{id}/ajustar-preco` | Obtem jogo por `id` |

---

## ListasDeDesejos (`dbo.ListasDeDesejos`)

Tabela N:N (PK composta = `idUsuario`, `idJogo`). Como so existem chaves, o "update" normalmente vira um **upsert** idempotente.

| Metodo | Endpoint | Operacao |
|---|---|---|
| GET | `/usuarios/{idUsuario}/wishlist` | Lista wishlist do usuario |
| POST | `/usuarios/{idUsuario}/wishlist/{idJogo}` | Adiciona jogo |
| DELETE | `/usuarios/{idUsuario}/wishlist/{idJogo}` | Remove jogo |

---

## Carrinhos (`dbo.Carrinhos`)

Itens de carrinho (PK = `id`, com unicidade em `(usuarioId, jogoId)`).

| Metodo | Endpoint | Operacao |
|---|---|---|
| GET | `/carrinhos` | Lista itens do usuário atual (filtros: `jogoId`, `page`, `pageSize`, `sort`) |
| GET | `/carrinhos/{id}` | Obtem item por `id` do usuário atual |
| POST | `/carrinhos` | Cria item no carrinho do usuário atual |
| PATCH | `/carrinhos/{id}` | Atualiza item (ex.: `quantidade`) do usuário atual |
| DELETE | `/carrinhos/{id}` | Remove item do carrinho do usuário atual |
| GET | `/usuarios/{id}/biblioteca` | Lista jogos adquiridos (tabela `JogosAdquiridos`) |
| GET | `/usuarios/{id}/biblioteca/{jogoId}` | Obtem um jogo da biblioteca (por `jogoId`) |



















# Fazer depois ... se der ...

## Resources (`dbo.Resources`)

| Metodo | Endpoint | Operacao |
|---|---|---|
| GET | `/resources` | Lista resources (filtros: `jogoId`, `tipoResource`, `ativo`, `page`, `pageSize`, `sort`) |
| POST | `/resources` | Cria resource |
| GET | `/resources/{id}` | Obtem resource por `id` |
| PUT/PATCH | `/resources/{id}` | Atualiza resource |
| DELETE | `/resources/{id}` | Remove (soft delete: `ativo=false`) |

Opcional (nested):

| Metodo | Endpoint | Operacao |
|---|---|---|
| POST | `/jogos/{jogoId}/resources` | Cria resource para um jogo |
| GET | `/jogos/{jogoId}/resources/{id}` | Obtem resource do jogo |

---

## Tags (`dbo.Tags`)

| Metodo | Endpoint | Operacao |
|---|---|---|
| GET | `/tags` | Lista tags (filtros: `ativo`, `q`, `page`, `pageSize`, `sort`) |
| POST | `/tags` | Cria tag |
| GET | `/tags/{id}` | Obtem tag por `id` |
| PUT/PATCH | `/tags/{id}` | Atualiza tag |
| DELETE | `/tags/{id}` | Remove (soft delete: `ativo=false`) |

---

## TagsPorJogo (`dbo.TagsPorJogo`)

Tabela de relacionamento N:N (PK composta = `idJogo`, `idTag`). Como so existem chaves, o "update" normalmente vira um **upsert** idempotente.

| Metodo | Endpoint | Operacao |
|---|---|---|
| GET | `/tags-por-jogo` | Lista relacionamentos (filtros: `idJogo`, `idTag`, `page`, `pageSize`) |
| POST | `/tags-por-jogo` | Cria relacionamento (vincula tag ao jogo) |
| GET | `/tags-por-jogo/{idJogo}/{idTag}` | Obtem relacionamento |
| PUT | `/tags-por-jogo/{idJogo}/{idTag}` | Upsert idempotente (cria se nao existir) |
| DELETE | `/tags-por-jogo/{idJogo}/{idTag}` | Remove relacionamento |

Opcional (nested, mais REST):

| Metodo | Endpoint | Operacao |
|---|---|---|
| POST | `/jogos/{idJogo}/tags/{idTag}` | Vincula |
| DELETE | `/jogos/{idJogo}/tags/{idTag}` | Desvincula |

---

## Jogos (`dbo.Jogos`)

Sub-recursos do jogo (tabelas dependentes):

| Metodo | Endpoint | Operacao |
|---|---|---|
| GET | `/jogos/{id}/resources` | Lista resources do jogo (tabela `Resources`) |
| GET | `/jogos/{id}/precos` | Lista precos do jogo (tabela `PrecosJogos`) |
| GET | `/jogos/{id}/tags` | Lista tags do jogo (tabela `TagsPorJogo`) |

## Usuarios (`dbo.Usuarios`)

| GET | `/usuarios/{id}/acessos` | Lista acessos do usuario (tabela `Acessos`) |
