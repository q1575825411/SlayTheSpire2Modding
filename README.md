# MyFirstStS2Mod

`MyFirstStS2Mod` 是一个基于 `RitsuLib` 的《杀戮尖塔2》模组工程模板，用于独立维护模组代码、资源和本地化内容。

当前工程处于独立开发模式，未绑定本机游戏安装目录。接入游戏环境后，再补充本地路径与运行依赖。

## 当前设计进度

按 `docs/design` 中当前设计文档统计，项目目前共设计 `53` 张卡牌。

- 基础牌：`4`
- 附加牌：`4`
- 锦囊牌一期：`5`
- 锦囊牌二期：`5`
- 灼烧流派：`10`
- 装备武器：`11`
- 装备防具与坐骑：`9`
- 装备宝物：`5`

统计口径：

- 不包含 `+` 版强化卡
- 不包含 `醉酒`、`灼烧`、`寒冷` 等状态定义
- 以 `docs/design` 当前文档内容为准

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

## 接入游戏环境并编译

### 1. 准备本机环境

- 安装 `.NET 9 SDK`
- 安装 `Godot 4.5.1 .NET` 版本
- 准备本机《杀戮尖塔2》安装目录

Windows 常见目录示例：

```text
C:\Program Files (x86)\Steam\steamapps\common\Slay the Spire 2
```

工程默认假定游戏数据目录为：

```text
<Sts2Dir>\data_sts2_windows_x86_64
```

该目录下需要能找到：

- `sts2.dll`
- `0Harmony.dll`

### 2. 填写游戏目录

修改 [MyFirstStS2Mod.csproj](/C:/workspace/python/game/mod/MyFirstStS2Mod/MyFirstStS2Mod.csproj)：

```xml
<Sts2Dir>C:\Program Files (x86)\Steam\steamapps\common\Slay the Spire 2</Sts2Dir>
```

填写后，以下引用会自动指向游戏目录：

- `$(Sts2DataDir)\sts2.dll`
- `$(Sts2DataDir)\0Harmony.dll`

同时，构建完成后的 `Copy Mod` 目标会把 `dll` 和 `json` 自动复制到：

```text
<Sts2Dir>\mods\MyFirstStS2Mod
```

### 3. 还原依赖并构建 DLL

在项目目录执行：

```powershell
dotnet restore
dotnet build .\MyFirstStS2Mod.csproj -c Debug
```

如果只需要发布版构建：

```powershell
dotnet build .\MyFirstStS2Mod.csproj -c Release
```

构建产物通常位于：

```text
bin\Debug\net9.0\
bin\Release\net9.0\
```

### 4. 导出 PCK

本项目的资源和脚本还需要通过 Godot 导出 `pck`。

建议做法：

1. 用 `Godot 4.5.1 .NET` 打开 [project.godot](/C:/workspace/python/game/mod/MyFirstStS2Mod/project.godot)
2. 在 Godot 中确认导出预设可用
3. 导出 Windows 对应的 `pck`

常见目标文件通常是：

```text
MyFirstStS2Mod.pck
```

如果当前还没有建立导出预设，需要先在 Godot 里手动创建一次。

### 5. 检查模组清单

确认 [MyFirstStS2Mod.json](/C:/workspace/python/game/mod/MyFirstStS2Mod/MyFirstStS2Mod.json) 中这些字段正确：

- `id`
- `name`
- `author`
- `version`
- `has_pck`
- `has_dll`
- `dependencies`

其中：

- `id` 应与资源根目录一致
- 使用 `RitsuLib` 时应保留 `dependencies: ["STS2-RitsuLib"]`

### 6. 放入游戏 mods 目录

最终需要进入游戏 `mods` 目录的文件通常包括：

- `MyFirstStS2Mod.json`
- `MyFirstStS2Mod.dll`
- `MyFirstStS2Mod.pck`

建议目录结构：

```text
<Sts2Dir>\mods\MyFirstStS2Mod\
├── MyFirstStS2Mod.json
├── MyFirstStS2Mod.dll
└── MyFirstStS2Mod.pck
```

如果 `Copy Mod` 已生效，`dll` 和 `json` 会自动复制；`pck` 仍需要你在导出后放到同一目录。

### 7. 首次联调建议

- 先用 `Debug` 构建
- 先只验证模组是否被识别和成功加载
- 再验证卡牌、本地化、图片、装备效果
- 如果构建成功但游戏不识别，优先检查 `json`、`id`、`dll`、`pck` 文件名是否一致
