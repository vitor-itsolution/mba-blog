# Feedback do Instrutor

#### 28/10/24 - Revisão Inicial - Eduardo Pires

## Pontos Positivos:

- Boa separação de responsabilidades.
- Arquitetura enxuta de acordo com a complexidade do projeto
- Soube aplicar a validação de claims ou proprietario do post
- Mostrou entendimento do ecossistema de desenvolvimento em .NET

## Pontos Negativos:

- A entidade autor não existe, deveria atuar em conjunto com o user (Identity)
  -   Eu não tenho uma entidade autor.
- Existem maneiras mais elegantes de obter o usuário do identity e utilizar seus dados.
  -   Vou pesquisar.
- A camada "Data" poderia virar "Core" e receber serviços de aplicação para evitar a duplicação e repetição de código comum na API e Web.
  -   Usei a estrutura solicitada na documentação da atividade.
- A camada de API não está implementada
- A interface poderia ser melhor, não consigo clicar num post e visualizar detalhes
  -   Nao entendi. O post é exibido completao na como se fosse um card. O clique é somente para visualizar pos comentários
- Não consigo postar algo maior que 300 caracteres
  -   Eu limitei.

## Sugestões:

- Evoluir o projeto para as necessidades solicitadas no escopo sem aumentar a complexidade do projeto.
