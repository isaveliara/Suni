# Suni

Um bot simples feito em C# para o Discord

## Olá!

A **Suni** é super flexível, feita para trazer ferramentas personalizáveis para o seu servidor. Se você quer adicionar suas funcionalidades interativas ou facilitar a administração, a Suni tem um monte de comandos para deixar seu servidor mais divertido e engajado.

---

## O Que Ela Pode Fazer

A Suni vem cheio de recursos, incluindo comandos personalizáveis e ferramentas úteis que permitem interagir com o seu servidor de novas maneiras.

- `&calc <exp>`: Dá uma representação visual da solução ou expressão que você fornecer.
- `&npt command <cmd>` : Roda um código npt adicionado em seu servidor, como por exemplo, &npt command request-ticket; Cria um canal de ticket para o ativador.

---

## Novidades

Atualmente, estou trabalhando em uma funcionalidade chamada `npt`, que vai permitir executar códigos de script como um bot paralelo dentro da própia Suni. Você vai poder acionar esses scripts com um comando curto, e até mesmo os membros do seu servidor poderão usá-los conforme suas restrições.

exemplo de uso:
```ansi
\033[38;5;246m--definitions--\033[0m
\033[38;5;198monlycase\033[0m(\033[38;5;83m@get<has_permission_admin>\033[0m) \033[38;5;198m->\033[0m \033[38;5;116mkit\033[0m \033[38;5;244m// Cancela a execução do código se o usuário não possuir admin\033[0m
\033[38;5;246m--end--\033[0m
\033[38;5;198mnpt::\033[0m \033[38;5;116mbanAsync\033[0m(\033[38;5;118m"25d"\033[0m) \033[38;5;198m->\033[0m \033[38;5;116mmessageFirstWord\033[0m \033[38;5;244m// capacita banimento por id e nome de usuário\033[0m
```

---

A Suni ainda está em desenvolvimento, mas logo terei a versão beta pronta para lançamento. Até mais!