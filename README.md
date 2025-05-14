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
docker compose up -d --no-deps averbacao-sqlserver
docker compose -f backend/docker-compose.yaml up -d --no-deps averbacao-sqlserver
```

Rodando via docker-compose você pode acessar o averbacao-service pelo link [`http://localhost:8081/swagger`](http://localhost:8081/swagger). 

## Migrations

```bash
# 1. Create initial migration
dotnet ef migrations add InitialCreate --output-dir Infrastructure/Migrations

# 2. Run the migrations on DB
dotnet ef database update

# 3. IF NECESSARY - Drop the database (if it exists and you want to reset the migrations). Note that you will need to repeat the first two steps after this
dotnet ef database drop --force
```

## Tasks

- Corrigir testes unitários
- Adicionar StatePattern na averbacao
- Adicionar Workflow.Core no AverbacaoWorkflowService
- Rever real necessidade da interface IAverbacoesRepository pois ela foi inclusa inicialmente pela necessidade nos testes unitários, porém os testes de integração já fazem o teste do repositório
- Implementar feature de cancelamento similar a implementação da formalização