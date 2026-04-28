# MyFirstStS2Mod

这是一个基于 `RitsuLib` 的《杀戮尖塔2》Mod 工程骨架，放在 `mod` 目录下，方便你继续按照 `SlayTheSpire2ModdingTutorials` 扩展。

## 当前包含

- `RitsuLib` 初始化版 `Entry.cs`
- `MyFirstStS2Mod.json` 模组清单
- `MyFirstStS2Mod.csproj` 构建与 `RitsuLib` 依赖配置
- `project.godot` 基础 Godot 工程文件
- `Scripts` 下的内容分类目录
- `MyFirstStS2Mod/localization/zhs/` 本地化目录
- `MyFirstStS2Mod/images/` 资源目录

## 你需要先改的地方

1. 打开 `MyFirstStS2Mod.json`，把作者、描述、版本改成你自己的。
2. 现在可以先不填 `MyFirstStS2Mod.csproj` 里的 `Sts2Dir`。
3. 之后接入游戏环境时，再把 `Sts2Dir` 改成你的游戏目录。
4. 接入真实环境后，确认本机安装的 `RitsuLib` 版本与 NuGet 解析到的版本一致。

## 这套结构是干什么的

- 适合继续做卡牌、遗物、能力、药水、事件、角色等“内容型” mod。
- 通过 `RitsuLib` 自动注册内容，后面写 `[RegisterCard]`、`[RegisterCharacter]` 这类 attribute 就能接上教程。
- 比完全原生模板少写很多注册样板代码。

## 后续建议

- 想先做第一张卡：看 `SlayTheSpire2ModdingTutorials/RitsuLib/01 - 添加卡牌/README.md`。
- 想加遗物：看 `SlayTheSpire2ModdingTutorials/RitsuLib/03 - 添加新遗物/README.md`。
- 想做人：看 `SlayTheSpire2ModdingTutorials/RitsuLib/14 - 添加新人物/README.md`。

## 当前结构

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
