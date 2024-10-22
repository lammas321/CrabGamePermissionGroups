# CrabGamePermissionGroups
A BepInEx mod for Crab Game that adds permission groups.

Depends on [PersistentData](https://github.com/lammas321/CrabGamePersistentData)

## What are permission groups?
Permission groups can be assigned to players to give them elevated permissions.

Currently, the only mod that utilizes these permissions is ChatCommands, with which you can choose who can use certain commands.

## How can this be configured?
Upon running the game for the first time, it will create a config file at "BepInEx/config/lammas123.PermissionGroups.cfg", as well as two permission group files, "host.txt" and "member.txt", in your "BepInEx/config/lammas123.PermissionGroups/" directory.

In the config file, you can set the default group id ("member" by default) and the host's group id ("host" by default). These link to the permission group files saved in that directory I mentioned previously.

The permission group files follow this format:
- Everything before the ".txt" in the file's name is the group's id. It is recommended to keep this short, lowercase, and have no spaces or special characters.
- The first line of the file will be the permission group's display name, or how it'll show up in game, I'll get back to this later.
- Every line after that is a different permission that permission group has. For example, to give members permission to use !help, you'd write "command.help" on a new line.

You are able to add as many permission groups as you want and give them any permissions you see fit.

## How do I assign a permission group to someone?
You can modify the permission group id saved in the player's persistent client data file, or with ChatCommands, you can write "!setclientdata <player> PermissionGroup <permission group id>" to change it in game.

## So what about the display names?
Using the [BetterChat](https://github.com/lammas321/CrabGameBetterChat) mod, you can add the player's permission group (or permission group id if you want) into their name when they send chat messages.
When adding permission groups to the formatting, use PERMISSION_GROUP to use the permission group's display name, and PERMISSION_GROUP_ID to use it's id.
