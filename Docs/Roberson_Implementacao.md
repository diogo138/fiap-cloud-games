# Implementação Roberson — Categorias + Jogos

## Escopo
- `dbo.Categorias`
- `dbo.Jogos`
- `dbo.PrecosJogos`

## Padrões Seguidos
- Sem AutoMapper — mapeamento manual nos Services
- IoC dinâmico — `IocExtensions.cs` NÃO alterado (registra por convenção `I{NomeClasse}`)
- Unit of Work — `SalvarAsync()` chamado no Controller, nunca no Service
- Testes com NUnit + Shouldly + banco in-memory

---

## Arquivos Criados

### `FIAP.FCG.Infrastructure` — Repositórios (6 arquivos)

| Arquivo | Conteúdo |
|---|---|
| `Dados/Repositorios/ICategoriaRepository.cs` | Interface: `ConsultarAsync`, `ConsultarTodosAsync`, `ConsultarPorNomeAsync`, `AdicionarAsync`, `AtualizarAsync` |
| `Dados/Repositorios/CategoriaRepository.cs` | Implementação com filtros de `ativo` e `q` (busca em nome/descrição) |
| `Dados/Repositorios/IJogoRepository.cs` | Interface: `ConsultarAsync`, `ConsultarTodosAsync`, `ConsultarPorNomeAsync`, `AdicionarAsync`, `AtualizarAsync` |
| `Dados/Repositorios/JogoRepository.cs` | Implementação com filtros de `categoriaId`, `visivel`, `ativo`, `q` |
| `Dados/Repositorios/IPrecoJogoRepository.cs` | Interface: `ConsultarPorJogoAsync`, `AdicionarAsync` |
| `Dados/Repositorios/PrecoJogoRepository.cs` | Implementação ordenando por `DataInicio DESC` |

---

### `FIAP.FCG.Domain` — Regras de Negócio (4 arquivos)

| Arquivo | Regra |
|---|---|
| `Categorias/ICategoriaBusiness.cs` | `NomeJaExiste(nome, excluirId?)` |
| `Categorias/CategoriaBusiness.cs` | Verifica unicidade do nome (ignora o próprio id no update) |
| `Jogos/IJogoBusiness.cs` | `NomeJaExiste(nome, excluirId?)` |
| `Jogos/JogoBusiness.cs` | Verifica unicidade do nome (ignora o próprio id no update) |

---

### `FIAP.FCG.Application` — DTOs + Services (11 arquivos)

**Categorias:**

| Arquivo | Campos |
|---|---|
| `Categorias/Dtos/CategoriaDto.cs` | `Id`, `Nome`, `Descricao`, `DataCadastro`, `Ativo` |
| `Categorias/Dtos/CategoriaNovoDto.cs` | `Nome`, `Descricao` |
| `Categorias/Dtos/CategoriaAtualizadoDto.cs` | `Nome`, `Descricao`, `Ativo` |
| `Categorias/Services/ICategoriaService.cs` | `ListarAsync`, `ConsultarAsync`, `AdicionarAsync`, `AtualizarAsync`, `RemoverAsync` |
| `Categorias/Services/CategoriaService.cs` | Soft delete (`Ativo = false`), lança `InvalidOperationException` para nome duplicado, `KeyNotFoundException` para não encontrado |

**Jogos:**

| Arquivo | Campos |
|---|---|
| `Jogos/Dtos/JogoDto.cs` | `Id`, `Nome`, `CategoriaId`, `Descricao`, `DataCadastro`, `DataLancamento`, `Visivel`, `Ativo` |
| `Jogos/Dtos/JogoNovoDto.cs` | `Nome`, `CategoriaId`, `Descricao`, `DataLancamento`, `Visivel` |
| `Jogos/Dtos/JogoAtualizadoDto.cs` | `Nome`, `CategoriaId`, `Descricao`, `DataLancamento`, `Visivel`, `Ativo` |
| `Jogos/Dtos/AjustarPrecoDto.cs` | `Valor`, `PromocaoId?`, `PercDesconto?` |
| `Jogos/Services/IJogoService.cs` | `ListarAsync`, `ConsultarAsync`, `AdicionarAsync`, `AtualizarAsync`, `RemoverAsync`, `AjustarPrecoAsync` |
| `Jogos/Services/JogoService.cs` | Soft delete, regra de nome único, cria novo `PrecoJogo` no `AjustarPrecoAsync` |

---

### `FIAP.FCG.Web.API` — Controllers (2 arquivos)

**`Controllers/CategoriasController.cs`**

| Método | Rota | HTTP Status |
|---|---|---|
| `Get` | `GET /api/categorias?ativo=&q=` | 200 |
| `GetById` | `GET /api/categorias/{id}` | 200 / 404 |
| `Post` | `POST /api/categorias` | 201 / 409 |
| `Put` | `PUT /api/categorias/{id}` | 200 / 404 / 409 |
| `Delete` | `DELETE /api/categorias/{id}` | 204 / 404 |

**`Controllers/JogosController.cs`**

| Método | Rota | HTTP Status |
|---|---|---|
| `Get` | `GET /api/jogos?categoriaId=&visivel=&ativo=&q=` | 200 |
| `GetById` | `GET /api/jogos/{id}` | 200 / 404 |
| `Post` | `POST /api/jogos` | 201 / 409 |
| `Put` | `PUT /api/jogos/{id}` | 200 / 404 / 409 |
| `Delete` | `DELETE /api/jogos/{id}` | 204 / 404 |
| `AjustarPreco` | `POST /api/jogos/{id}/ajustar-preco` | 204 / 404 |

---

### `FIAP.FCG.Domain.Tests` — Testes de Domínio (2 arquivos)

| Arquivo | Testes |
|---|---|
| `Categorias/CategoriaBusinessTest.cs` | `NomeJaExiste` → true, false, false (mesmo id) |
| `Jogos/JogoBusinessTest.cs` | `NomeJaExiste` → true, false, false (mesmo id) |

---

### `FIAP.FCG.Application.Tests` — Testes de Serviço (2 arquivos)

| Arquivo | Testes |
|---|---|
| `Categorias/Services/CategoriaServiceTest.cs` | `ListarAsync`, `AdicionarAsync` (nome duplicado), `AdicionarAsync` (sucesso), `AtualizarAsync` (não encontrado), `AtualizarAsync` (sucesso), `RemoverAsync` (soft delete) |
| `Jogos/Services/JogoServiceTest.cs` | `ListarAsync`, `AdicionarAsync` (nome duplicado), `AdicionarAsync` (sucesso), `AtualizarAsync`, `RemoverAsync`, `AjustarPrecoAsync` |

---

### `FIAP.FCG.Tests` — Alteração (1 arquivo)

**`DataFakeFactory.cs`** — adicionados:
- `NovaCategoria(string sufixo)` — sobrecarga com nome único para testes de unicidade
- `NovoPrecoJogo(Jogo jogo, decimal valor)` — para testes de `AjustarPreco`

---

## Resumo

| Camada | Arquivos |
|---|---|
| Infrastructure (Repositórios) | 6 novos |
| Domain (Business) | 4 novos |
| Application (DTOs + Services) | 11 novos |
| Web.API (Controllers) | 2 novos |
| Domain.Tests | 2 novos |
| Application.Tests | 2 novos |
| Tests (DataFakeFactory) | 1 alterado |
| **Total** | **27 novos + 1 alteração** |

`IocExtensions.cs` e `Program.cs` **não foram alterados**.  
Nenhuma migration nova foi necessária (entidades já existiam).
