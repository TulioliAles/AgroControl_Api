# Fase 1 — Bootstrap da solução

## Objetivo

Criar uma base mínima, compilável e organizada para a AgroControl API antes da inclusão de banco de dados, cache, mensageria ou autenticação.

## O que foi criado

- `global.json`: fixa o SDK .NET 10 usado pelo projeto.
- `Directory.Build.props`: centraliza configurações comuns de compilação.
- `.editorconfig`: padroniza formatação e estilo do código.
- `AgroControl.sln`: agrupa os projetos da solução.
- `AgroControl.Domain`: regras e conceitos centrais do negócio.
- `AgroControl.Application`: casos de uso e contratos de aplicação.
- `AgroControl.Infrastructure`: implementações técnicas e integrações.
- `AgroControl.Contracts`: contratos públicos de entrada e saída da API.
- `AgroControl.Api`: ponto de entrada HTTP e composição da aplicação.

## Regra de dependências

```text
Api -> Application
Api -> Infrastructure
Api -> Contracts
Infrastructure -> Application
Infrastructure -> Domain
Application -> Domain
Domain -> nenhuma outra camada
```

A camada de domínio deve permanecer independente de banco, frameworks web, Redis e RabbitMQ.

## Passo a passo para executar localmente

1. Instale o SDK .NET 10.
2. Clone o repositório.
3. Troque para a branch `feature/001-bootstrap-solution`.
4. Execute `dotnet --version` e confirme que o SDK 10 está ativo.
5. Execute `dotnet restore AgroControl.sln`.
6. Execute `dotnet build AgroControl.sln`.
7. Execute `dotnet run --project src/AgroControl.Api`.
8. Acesse `/` para verificar o status da API.
9. Acesse `/health` para verificar o health check básico.

## O que estudar nesta fase

- Diferença entre solução e projeto no .NET.
- Referências entre projetos.
- Inversão de dependência.
- Responsabilidade de cada camada.
- Nullable Reference Types.
- Tratamento de warnings como erros.
- Minimal APIs e pipeline HTTP.

## Exercícios

1. Explique por que `Domain` não referencia `Infrastructure`.
2. Remova temporariamente uma referência necessária e observe o erro de compilação.
3. Altere a resposta do endpoint raiz e execute novamente.
4. Crie um endpoint `/version` retornando a versão inicial `0.1.0`.

## Critérios de conclusão

- A solução restaura sem erros.
- A solução compila sem warnings.
- A API inicia localmente.
- Os endpoints `/` e `/health` respondem.
- A regra de dependências entre camadas foi compreendida.

## Próxima fase

Na Fase 2 serão definidos o domínio agrícola, os módulos do sistema, entidades, agregados, regras de negócio e o primeiro recorte funcional vertical.
