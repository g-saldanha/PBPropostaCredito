# 🏦 PBPropostaCredito - Microsserviço de Proposta

## 🛠 Tecnologias Utilizadas
- **.NET 10.0** (ASP.NET Core Web API)
- **Entity Framework Core** (In-Memory Database para ambiente local)
- **MassTransit + RabbitMQ** (Consumo e Publicação de eventos)
- **xUnit + Moq** (Testes de Unidade)
- **Swagger / OpenAPI** (Documentação interativa)
- **Asp.Versioning.Http** (Versionamento de API)

## 🏗 Decisões Técnicas
- **Consumidor Assíncrono:** A classe `ClienteCadastradoConsumer` é responsável por ler os eventos da fila do RabbitMQ em segundo plano, garantindo que o MS1 não fique bloqueado esperando o cálculo do *score*.
- **Regra de Negócio (Score):** O sistema gera um *score* de 0 a 1000. Se o valor for até 100, a proposta recebe o status "Negado". Acima disso, recebe "Aprovado".
- **Encadeamento de Eventos:** Assim que a proposta é salva no banco de dados, o serviço imediatamente publica o próximo evento (`PropostaGerada`) no RabbitMQ, passando o bastão para o Microsserviço de Cartões.

## 📡 Mensageria (Eventos)
- **Consome / Ouve:** `ClienteCadastrado` (Fila: `fila-cliente-cadastrado`)
    - *Namespace:* `ParanaBanco.Messages`
- **Produz / Publica:** `PropostaGerada`
    - *Payload:* `ClienteId`, `PropostaId`, `Score`, `Status`
    - *Namespace:* `ParanaBanco.Messages`

## 🚀 Como Executar Localmente

### Pré-requisitos
- [.NET 10 SDK](https://dotnet.microsoft.com/)
- Ter o **RabbitMQ** rodando (conforme documentação do MS1)

### Passos para Execução

1. **Clone e inicie a Aplicação:**
   Navegue até a pasta raiz deste microsserviço e execute:
   ```bash
   dotnet run

### Fluxograma

![mermaid fluxograma.png](mermaid%20fluxograma.png)