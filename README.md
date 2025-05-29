# averbacao-arcdemo
Demonstração de estrutura de projeto com vertical-slice para um serviço que faz averbação de empréstimo consignado.

Dividido em dois projetos:
- AverbacaoService: Detêm o domínio de averbação e regras, ele é somente uma WebAPI.
- AverbacaoWorkflow: Um serviço/micro-serviço que faz a orquestração dos fluxos da averbação, ele tem diversos consumers (Consumidores Kafka) que ativam o Workflow.Core para fazer a orquestração.

## Run with Docker Compose

```bash
docker compose up -d
# Or if running from root folder
docker compose -f backend/docker-compose.yaml up -d
# If you want to rebuild images
docker compose up -d --build
# If you want to run averbacao-db separately
docker compose up -d --no-deps sqlserver
docker compose -f backend/docker-compose.yaml up -d --no-deps sqlserver
```

Rodando via docker-compose você pode acessar o averbacao-service pelo link [`http://localhost:8081/swagger`](http://localhost:8081/swagger).

> Importante: para executar pela IDE configure suas aplicações como Development através das variáveis ASPNETCORE_ENVIRONMENT=Development - para AverbacaoService; e DOTNET_ENVIRONMENT=Development - para AverbacaoWorkflowService.

## Migrations

```bash
# 1. Create initial migration
dotnet ef migrations add InitialCreate --output-dir Infrastructure/Migrations

# 2. Run the migrations on DB
dotnet ef database update

# 3. IF NECESSARY - Drop the database (if it exists and you want to reset the migrations). Note that you will need to repeat the first two steps after this
dotnet ef database drop --force
```

## Running

1. Para executar um fluxo de teste para INSS basta você executar `docker compose -f backend/docker-compose.yaml up -d`.
2. Usando qualquer Kafka cliente insira a mensagem abaixo no tópico `local.averbacao-service.incluir-averbacao`.
Mensagem:
```json
{
    "Codigo": 12345,
    "Proponente": {
        "Cpf": "11111111111",
        "Nome": "Test",
        "Sobrenome": "Testers",
        "DataNascimento": "1990-08-10"
    },
    "Valor": 1000,
    "PrazoEmMeses": 30
}
```
> 💡 Você pode utilizar o [OffsetExplorer](https://www.kafkatool.com/) para enviar as mensagens Kafka.

> Se quiser rodar um teste de carga na aplicação execute o comando `k6 run backend/AverbacaoService/tst/k6-load-test.js`. Importante: você precisa estar com o [k6](k6.io/) instalado na sua máquina.

## Roadmap

- Adicionar Polly no Flurl
- Estuturar strategy pattern na AverbacaoService de forma a separar o comportamento das conveniadas?
- Adicionar StatePattern na Averbacao?
- Rever real necessidade da interface IAverbacoesRepository pois ela foi inclusa inicialmente pela necessidade nos testes unitários, porém os testes de integração já fazem o teste do repositório
- Implementar feature de cancelamento similar a implementação da formalização