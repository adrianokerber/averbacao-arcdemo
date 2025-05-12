# averbacao-arcdemo
Demonstração de estrutura de projeto com vertical-slice para um serviço que faz averbação de empréstimo consignado

Dividido em dois projetos:
- AverbacaoService: Detêm o domínio de averbação e regras, ele é somente uma WebAPI.
- AverbacaoWorkflow: Um micro-serviço que faz a orquestração dos fluxos da averbação, ele tem diversos consumers (Consumidores Kafka) que ativam o Workflow.Core para fazer a orquestração

## Run with Docker Compose

```bash
docker compose up -d
# Or if running from root folder
docker compose -f backend/docker-compose.yaml up -d
# If you want to rebuild images
docker compose up -d --build
# If you want to run averbacao-db separately
docker compose up -d --no-deps averbacao-service-database
docker compose -f backend/docker-compose.yaml up -d --no-deps averbacao-service-database
```

## Tasks

- Corrigir DI de CommandHandler
- Garantir que Averbacao está funcional e gravando no banco de dados
- Adicionar Workflow.Core no AverbacaoWorkflowService