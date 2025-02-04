# MyFood-Products

## Descrição

O MyFood-Carts-Payments é um microserviço de gerenciamento de carrinho e pagamento desenvolvido utilizando ASP.NET Core Minimal API com uma arquitetura Clean Architecture. Ele permite que os clientes montem combos personalizados e acompanhem o status de seus pedidos. O sistema também oferece funcionalidades administrativas para gerenciar produtos, categorias e campanhas promocionais.

## Sobre o Projeto

Este projeto faz parte do Tech Challenge da Pós Tech da FIAP do curso de Software Architecture. Trata-se de uma atividade obrigatória que deve ser desenvolvida em grupo e vale 90% da nota de todas as disciplinas da fase. É importante atentar-se ao prazo de entrega.

O repositório do projeto pode ser encontrado [https://github.com/themisterbondy/postech-tech-challenge-api-carts-payments.git](https://github.com/themisterbondy/postech-tech-challenge-api-carts-payments.git).

## Tecnologias Utilizadas

- **ASP.NET Core Minimal API**
- **Entity Framework Core**
- **PostgreSQL**
- **Docker e Docker Compose**
- **Swagger**
- **OpenTelemetry**
- **Serilog**
- **MediatR**
- **FluentValidation**
- **HealthChecks**
- **Quartz.NET** (para gerenciamento de jobs)

## Estrutura do Projeto

- **Arquitetura Hexagonal**
- **Feature Folder**
- **Documentação com Event Storming**

## Documentação

A documentação do sistema foi desenvolvida seguindo os padrões de Domain-Driven Design (DDD) com Event Storming, cobrindo os fluxos de realização do pedido e pagamento, preparação e entrega do pedido.

Os desenhos e diagramas do Event Storming podem ser encontrados [https://miro.com/app/board/uXjVK06l1is=/](https://miro.com/app/board/uXjVK06l1is=/).

### Postman

Link do Postman [https://www.postman.com/blue-crater-21969/workspace/postech/collection/389375-9e6deac9-fe83-4f67-9072-a55063ff590d?action=share&creator=38874442](https://www.postman.com/blue-crater-21969/workspace/postech/collection/389375-9e6deac9-fe83-4f67-9072-a55063ff590d?action=share&creator=38874442)

## Migrações e Dados Pré-Incluídos

O sistema utiliza migrações do Entity Framework Core para gerenciar o esquema do banco de dados. As migrações são aplicadas automaticamente durante a inicialização da aplicação.


## Instruções para Configuração

### Requisitos

- Docker
- Docker Compose

### Passos para Configuração

1. Clone o repositório:
    ```shell
    git clone https://github.com/themisterbondy/postech-tech-challenge-api-carts-payments.git
    cd postech-tech-challenge
    ```

2. Configure o certificado HTTPS executando os seguintes comandos:
    ```shell
    dotnet dev-certs https --clean
    dotnet dev-certs https -ep $env:userprofile\.aspnet\https\aspnetapp.pfx -p password123
    dotnet dev-certs https --trust
    ```

3. Suba os containers:
    ```shell
    docker-compose up --build
    ```

4. Acesse o Swagger para explorar as APIs: [https://localhost:8081/swagger](https://localhost:8081/swagger)


### Passos para Utilização HELM


1. Cria pods de aplicação: 
    ```shell
    helm install myfood-webapi .\charts\webapi\ --namespace myfood-namespace
    ```

2. Valida estado dos Pods
    ```shell
    kubectl get pods --namespace myfood-namespace --watch
    ```

3. Url de acesso a aplicação 
    ```shell
    http://localhost:30000
    ```

4. Visualizar Logs de pods 
    ```shell
      kubectl describe pod {{myfood-webapi}} --namespace myfood-namespace
    ```

5. Deletar NameSpace ( deleta todos os recursos criados )
    ```shell
    kubectl delete namespace myfood-namespace
    ```       


## Validação da POC

## HealthChecks

O sistema inclui configurações de HealthChecks para monitorar a saúde do sistema, incluindo o banco de dados PostgreSQL. Os endpoints de HealthCheck estão configurados para fornecer informações sobre o status do sistema e detalhes dos monitores de saúde.

### Endpoints de HealthCheck

- **Status Text**: [https://localhost:8081/status-text](https://localhost:8081/status-text)
- **Health Status**: [https://localhost:8081/health](https://localhost:8081/health)

## Middleware de Logging de Contexto de Requisição

O sistema inclui um middleware de logging de contexto de requisição utilizando Serilog. Este middleware adiciona um ID de correlação a cada requisição para melhor rastreamento e correlação de logs.

## Manipulador Global de Exceções

O sistema inclui um manipulador global de exceções que registra erros e retorna uma resposta JSON padronizada com detalhes do problema.
