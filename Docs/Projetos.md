# Divisão Técnica dos Projetos

Todos os projetos foram desenvolvidos em .NET 9 com C#. O projeto principal foi feito em ASP.net Core Web Api.

## Ordem de Dependência
A relação de dependência está definida de "cima para baixo", indicando qual projeto "conhece" outro projeto.

```
  FIAP.FCG.Web.API *Start* (Endpoints, Controllers)
  ├── FIAP.FCG.Application (Serviços, DTOs)
  ├── ├── FIAP.FCG.Domain (Regras de Negócios)
  └── └── └── FIAP.FCG.Infrastructure (Migration, Configuração do EFCore, Banco, Entities, Repositório)
```

* Utilizado como referência o livro: Domain-Drive-Design de Eric Evans;

## Significado das Camadas

### API

Este projeto é responsável por expor o conteúdo que é de interesse "público", de modo a organizar o formato de entrada/saída das informações.

Utilizamos os padrões REST para documentação das APIS, sendo:

- ~/autenticacao
- ~/usuarios
- ~/jogos
- ~/categorias
- ~/promocoes
- ~/carrinhos

Também é responsabilidade deste projeto controlar a transação do banco e carregar 1 ou N serviços que irão realizar um determinado trabalho. Caso haja falha de algum serviço, toda a transação é cancelada.

---

### Application

Este projeto é responsável por disponibilizar os **Serviços**, estes fazem o "orquestramento" de 1 ou N regras de negócio. Também é responsável por gerir os DTO's, de modo que caso duas regras de negócios gerem informações, as mesmas podem ser combinadas em apenas um DTO de saída.

---

### Domain

Este projeto é responsável pelas regras de negócio, ou seja, conhecimentos que devem ser validados, verificados, calculados,  gerados. Deve ter sempre como base a entidade ou outra regra de negócio, tendo como saída as entidades ou alguma classe específica que "nasce" no próprio projeto Domain.

---

### Infrastructure

Este projeto é responsável por armazenar as configurações de banco, entidades, repositórios, IOC, cálculos de chaves de criptografia, ferramentas básicas para facilitar o próprio uso do .NET.