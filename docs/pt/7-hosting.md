## Testes

Para facilitar o esfor�o para testar sua aplica��o, existe um pacote inicial para um *host* em uma *Console Application*.

Adicione um novo projeto de um *Console Application* e a partir do *Package Manager Console*, instale-o usando:

    Install-Package Takenet.MessagingHub.Client.ConsoleHost

*Observa��o*: este pacote tem como *target* o *framework* 4.6.1, ent�o altere o *target framework* do seu projeto.

## Produ��o

Em ambientes de produ��o uma solu��o mais robusta, como um *Windows Service*, deve ser utilizada.
