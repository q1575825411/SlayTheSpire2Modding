# MyFirstStS2Mod

`MyFirstStS2Mod` 是一个基于 `RitsuLib` 的《杀戮尖塔2》模组工程模板，用于独立维护模组代码、资源和本地化内容。

当前工程处于独立开发模式，未绑定本机游戏安装目录。接入游戏环境后，再补充本地路径与运行依赖。

## 目录结构

```text
MyFirstStS2Mod
├── Scripts
│   ├── Cards
│   ├── Characters
│   ├── Patches
│   ├── Relics
│   └── Entry.cs
├── MyFirstStS2Mod
│   ├── images
│   │   ├── cards
│   │   ├── characters
│   │   ├── relics
│   │   └── ui
│   └── localization
│       └── zhs
│           ├── ancients.json
│           ├── card_keywords.json
│           ├── cards.json
│           ├── characters.json
│           ├── encounters.json
│           ├── enchantments.json
│           ├── events.json
│           ├── monsters.json
│           ├── orbs.json
│           ├── potions.json
│           ├── powers.json
│           ├── relics.json
│           └── static_hover_tips.json
├── MyFirstStS2Mod.csproj
├── MyFirstStS2Mod.json
└── project.godot
```

## 工程说明

- `Scripts/Entry.cs` 为模组入口，使用 `RitsuLib` 初始化并注册当前程序集内容。
- `Scripts/Cards`、`Scripts/Relics`、`Scripts/Characters` 用于放置主要内容类型。
- `Scripts/Patches` 用于维护 Harmony patch 或其他底层修改。
- `MyFirstStS2Mod/localization/zhs` 用于维护简体中文本地化文本。
- `MyFirstStS2Mod/images` 用于维护卡图、遗物图、角色图和界面资源。

## 依赖

- `Godot.NET.Sdk 4.5.1`
- `.NET 9`
- `STS2.RitsuLib`
- `sts2.dll`
- `0Harmony.dll`

## 开发约定

- `MyFirstStS2Mod.json` 为模组清单，`id` 应与模组资源根目录保持一致。
- `MyFirstStS2Mod.csproj` 中的 `Sts2Dir` 当前留空，接入游戏环境后再填写。
- `Copy Mod` 构建目标仅在 `Sts2Dir` 非空时执行。
- 新增内容优先按 `SlayTheSpire2ModdingTutorials` 的 `RitsuLib` 章节组织。

## 接入游戏环境

1. 在 `MyFirstStS2Mod.csproj` 中填写 `Sts2Dir`。
2. 确认本机可解析 `sts2.dll`、`0Harmony.dll` 与 `STS2.RitsuLib`。
3. 构建 `dll`。
4. 导出 `pck`。
5. 将 `json`、`dll`、`pck` 放入游戏 `mods` 目录。
