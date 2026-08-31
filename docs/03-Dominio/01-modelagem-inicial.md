# Fase 2 — Modelagem inicial do domínio agrícola

## Objetivo

Definir uma linguagem comum para o sistema antes de criar banco de dados, controllers e integrações.

Nesta etapa começamos pelo menor recorte que já representa valor de negócio:

1. cadastrar um insumo agrícola;
2. identificar sua categoria e unidade de medida;
3. registrar lotes com fabricação, validade e saldo;
4. impedir operações que gerem saldo negativo;
5. permitir desativação sem apagar histórico.

## Decisões de modelagem

### Insumo agrícola

`AgriculturalInput` representa o catálogo do produto utilizado ou armazenado pela operação rural.

Dados iniciais:

- nome;
- categoria;
- unidade de medida;
- estoque mínimo;
- fabricante;
- número de registro externo, quando aplicável;
- situação ativa ou inativa;
- datas de criação e alteração.

O campo de registro externo é propositalmente genérico nesta fase. Fertilizantes, sementes, defensivos e produtos veterinários possuem cadastros e regras distintas. O AgroControl armazenará a referência necessária para rastreabilidade, mas não substituirá os sistemas oficiais.

### Lote de estoque

`InventoryLot` representa o saldo rastreável de um insumo.

Cada lote possui:

- vínculo com o insumo;
- número do lote;
- saldo atual;
- data de fabricação opcional;
- data de validade opcional;
- histórico temporal básico.

## Regras implementadas

- Identificadores não podem ser vazios.
- O nome do insumo é obrigatório.
- Estoque mínimo não pode ser negativo.
- Quantidades de entrada e saída devem ser maiores que zero.
- Uma saída não pode ultrapassar o saldo atual.
- A validade não pode ser anterior à fabricação.
- Um insumo é desativado em vez de ser removido fisicamente.

## Por que ainda não usamos Entity Framework?

A ordem de aprendizagem é intencional:

1. primeiro entendemos as regras de negócio;
2. depois criamos testes unitários para essas regras;
3. somente então mapeamos as entidades no Entity Framework;
4. por último expomos os casos de uso pela API.

Assim evitamos misturar regra de domínio com detalhes de banco de dados.

## Próximos passos

- criar testes unitários para `AgriculturalInput` e `InventoryLot`;
- definir propriedades rurais, depósitos e locais de armazenamento;
- modelar movimentações de entrada, saída, transferência e ajuste;
- criar o primeiro caso de uso da camada Application;
- mapear o domínio com Entity Framework Core e SQL Server.

## Exercícios

1. Explique por que um lote não deve aceitar saldo negativo.
2. Identifique quais categorias podem exigir número de registro externo.
3. Avalie se o estoque mínimo pertence ao catálogo do insumo ou ao depósito.
4. Pense em como controlar o mesmo lote armazenado em dois depósitos diferentes.
5. Liste quais informações adicionais seriam necessárias para sementes e defensivos.

## Checklist

- [ ] Compreendi a diferença entre catálogo de insumos e lote de estoque.
- [ ] Compreendi por que as regras estão dentro das entidades.
- [ ] Consigo explicar a diferença entre desativação e exclusão.
- [ ] Executei `dotnet build AgroControl.sln` localmente.
- [ ] Revisei os exercícios desta etapa.
