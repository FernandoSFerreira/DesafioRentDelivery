# Desafio RentDelivery

## Descrição do Projeto

O projeto **Desafio RentDelivery** é uma API backend desenvolvida em .NET 8 com C# para gerenciar o aluguel de motos e entregadores. A aplicação permite registrar entregadores, motos, aluguéis, e controlar as entregas realizadas pelos entregadores com motos alugadas. O projeto foi desenvolvido seguindo as melhores práticas de arquitetura de software e padrões de design, como Domain-Driven Design (DDD), além de integrar logs e tratamento de erros utilizando Serilog e RabbitMQ para comunicação assíncrona.

## Requisitos Atendidos

- **Cadastro de Motos**: Permite a criação, leitura, atualização e exclusão de motos.
- **Cadastro de Entregadores**: Permite a criação, leitura, atualização e exclusão de entregadores.
- **Gestão de Aluguéis**: Registro de aluguéis de motos pelos entregadores, garantindo que apenas entregadores com aluguel ativo possam realizar entregas.
- **Gestão de Entregas**: Registro das entregas realizadas pelos entregadores.
- **Validações**: O entregador só pode realizar entregas se tiver um aluguel de moto ativo (data vigente).
- **Persistência de Dados**: Integração com PostgreSQL para dados transacionais e MongoDB para armazenamento de entregas e aluguéis.
- **Comunicação Assíncrona**: Uso de RabbitMQ para enviar mensagens assíncronas relacionadas às entregas.
- **Logs e Tratamento de Erros**: Implementação de logs estruturados e tratamento de erros com Serilog.

## Diferenciais Implementados

- **Arquitetura DDD (Domain-Driven Design)**: O projeto foi estruturado em camadas conforme os princípios de DDD, com separação clara entre Domain, Application, Infrastructure e API.
- **Docker e Docker Compose**: Configurações completas para containerização da aplicação e seus serviços dependentes (PostgreSQL, MongoDB, RabbitMQ).
- **Testes Automatizados**: Implementação de testes unitários e de integração utilizando xUnit e FluentAssertions, garantindo a confiabilidade das funcionalidades.
- **Clean Code**: Código desenvolvido seguindo os princípios de Clean Code, com nomes de variáveis e métodos claros, e tratamento de exceções consistente.
- **Documentação com Swagger**: A API foi documentada utilizando Swagger, facilitando a exploração dos endpoints.

## Como Executar a Aplicação

### Pré-requisitos

- Docker
- Docker Compose

### Passos para Executar

1. **Clone o repositório**:
   ```bash
   git clone https://github.com/seu-usuario/DesafioRentDelivery.git
   cd DesafioRentDelivery

2. **Configure as variáveis de ambiente (opcional)**:

- As variáveis de ambiente para as conexões com PostgreSQL, MongoDB e RabbitMQ estão configuradas no arquivo docker-compose.yml.
- Se necessário, você pode modificar as variáveis no arquivo ou criar um arquivo .env.

3. **Execute o Docker Compose**:
   ```bash
   docker-compose up -d

4. **Acesse a aplicação**:
- A API estará disponível em http://localhost:5000.
- A documentação do Swagger estará disponível em http://localhost:5000/swagger.

## Executando os Testes

1. **Teste Unitários**:

- Os testes unitários cobrem as lógicas de negócio do projeto. Para executá-los, utilize o comando:
   ```bash
   dotnet test tests/DesafioRentDelivery.UnitTests

2. **Testes de Integração**:
- Os testes de integração validam a comunicação com o banco de dados e a integração dos repositórios e serviços. Para executá-los, utilize o comando:
   ```bash
   dotnet test tests/DesafioRentDelivery.IntegrationTests

## Estrutura do Projeto
- **DesafioRentDelivery.API**: Camada de apresentação, expõe os endpoints da API.
- **DesafioRentDelivery.Application**: Camada de aplicação, responsável pelas regras de negócio.
- **DesafioRentDelivery.Domain**: Camada de domínio, contém as entidades e interfaces de repositório.
- **DesafioRentDelivery.Infrastructure**: Camada de infraestrutura, implementa os repositórios, integra com bancos de dados e serviços externos.
- **DesafioRentDelivery.UnitTests**: Projeto de testes unitários.
- **DesafioRentDelivery.IntegrationTests**: Projeto de testes de integração.

## Considerações Finais
Este projeto foi desenvolvido com foco em qualidade de código, arquitetura bem definida e testes automatizados. Ele serve como um exemplo robusto de como criar uma aplicação .NET escalável, testável e de fácil manutenção.

Se você tiver alguma dúvida ou sugestão, sinta-se à vontade para abrir uma issue ou pull request no repositório!

Autor: Fernando de Souza Ferreira
Licença: MIT
