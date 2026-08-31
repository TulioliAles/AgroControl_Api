# Regra de dependência

A solução segue uma separação em camadas orientada ao domínio.

- `Domain` contém regras de negócio e não depende de outras camadas.
- `Application` coordena casos de uso e depende apenas de `Domain`.
- `Infrastructure` implementa persistência, cache, mensageria e integrações.
- `Contracts` define modelos públicos da API sem expor entidades internas.
- `Api` configura o processo, o pipeline HTTP e a injeção de dependências.

Dependências técnicas futuras, como Entity Framework Core, SQL Server, Redis e RabbitMQ, serão mantidas fora do domínio.
