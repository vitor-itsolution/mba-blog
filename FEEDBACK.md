# Feedback do Instrutor

#### 28/10/24 - Revisão Inicial - Eduardo Pires

## Pontos Positivos:

- Boa separação de responsabilidades.
- Arquitetura enxuta de acordo com a complexidade do projeto
- Soube aplicar a validação de claims ou proprietario do post
- Mostrou entendimento do ecossistema de desenvolvimento em .NET

## Pontos Negativos:

- A entidade autor não existe, deveria atuar em conjunto com o user (Identity)
- Existem maneiras mais elegantes de obter o usuário do identity e utilizar seus dados.
- A camada "Data" poderia virar "Core" e receber serviços de aplicação para evitar a duplicação e repetição de código comum na API e Web.
- A camada de API não está implementada
- A interface poderia ser melhor, não consigo clicar num post e visualizar detalhes
- Não consigo postar algo maior que 300 caracteres

## Sugestões:

- Evoluir o projeto para as necessidades solicitadas no escopo sem aumentar a complexidade do projeto.
