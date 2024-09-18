# Suni

A simple C# bot for Discord

## Hello!

**Suni** is super flexible, designed to bring customizable tools to your server. Whether you want to add interactive features or make administration easier, Suni has a ton of commands to make your server more fun and engaging.

---

## What It Can Do

Suni comes packed with features, including customizable commands and useful tools that allow you to interact with your server in new ways.

- `&calc <exp>`: Gives a visual representation of the solution or expression you provide.
- `&npt command <cmd>` : Run an npt code added to your server (can be used by any member)

---

## What's New

I'm currently working on a feature called `npt`, which will allow you to run script code as a parallel bot within Suni itself.  You will be able to trigger these scripts with a short command, and even members of your server will be able to use them according to your restrictions.

usage example:
```markdown
--definitions--
**onlycase(@get<has_permission_admin>)** -> **kit** // Cancels code execution if user does not have admin
--end--
**npt::** **banAsync("25d")** -> **messageFirstWord** // enables banning by id and username
```

---

Suni is still in development, but I will have the beta version ready for release soon. See you soon!