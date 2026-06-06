# WeaponSkin

A plugin for [ModSharp](https://github.com/Kxnrl/modsharp-public) that allows players to apply custom cosmetics in CS2, including weapon paint kits, stickers, music kits, agents, gloves, and custom 3D models.

## Features

- Weapon paint kits with wear, seed, StatTrak and name tags
- Stickers and keychains
- Knife skins
- Gloves
- Agents (CT & T)
- Music kits
- Medals/pins
- **Custom model skins** — load custom `.vmdl` weapon models via `sharp/configs/custom_models.json`

## Requirements

- [ModSharp](https://github.com/Kxnrl/modsharp-public) installed on your CS2 server
- MySQL, PostgreSQL, or SQLite database

## Installation

1. Download the latest release from the [releases](https://github.com/Noldez/WeaponSkin/releases) page.
2. Extract the contents of the zip file to the `{server_dir}/game/` directory (alongside the `sharp` folder).
3. Edit `sharp/configs/weaponskin.jsonc` to configure your database connection.

## Custom Models

To precache and use custom weapon models:

1. Place compiled model files (`.vmdl_c`) in `game/csgo/weapons/` or `game/csgo/models/weapons/`
2. Add the model path to `sharp/configs/custom_models.json`:

```json
[
  "weapons/yourauthor/yourmodel/yourmodel.vmdl"
]
```

3. Use the skin changer website or direct DB entry in `ws_custom_models` to assign the model to a player.

## Database Tables

| Table | Purpose |
|-------|---------|
| `ws_weapon_cosmetics` | Gun skins (paint, wear, seed, stattrak, nametag, stickers, keychain) |
| `ws_team_knives` | Knife selection per team |
| `ws_team_gloves` | Glove selection per team |
| `ws_team_agents` | Agent selection per team |
| `ws_team_medals` | Medal/pin selection |
| `ws_team_musickits` | Music kit selection |
| `ws_custom_models` | Custom model overrides |

## Request module

The SQL request module (`WeaponSkin.Request.Sql`) is required. It supports MySQL, PostgreSQL, and SQLite.
If you have a custom request implementation, it must implement the `IRequestManager` interface from `WeaponSkin.Shared`.
