**简体中文** | [English](README-EN.md) | [繁體中文](README-ZH_TW.md)

<div align="center">

<img src="PCL.Launcher/Assets/Images/icon.svg" alt="Logo" width="80" height="80">

# PCL CE Linux 移植版

基于 Avalonia 的 PCL CE Linux 构建，提供开箱即用的 Debian / Ubuntu 安装包。

[![Release](https://img.shields.io/github/v/release/LuoYue-cn/pcl-ce-linux?label=release&style=for-the-badge&logo=github)](https://github.com/LuoYue-cn/pcl-ce-linux/releases/latest)
[![Downloads](https://img.shields.io/github/downloads/LuoYue-cn/pcl-ce-linux/total?style=for-the-badge&label=downloads)](https://github.com/LuoYue-cn/pcl-ce-linux/releases)
[![Issues](https://img.shields.io/github/issues/LuoYue-cn/pcl-ce-linux?style=for-the-badge&label=issues&labelColor=444444&color=1F883D)](https://github.com/LuoYue-cn/pcl-ce-linux/issues)
[![License](https://img.shields.io/badge/license-Apache--2.0%20%2B%20custom-blue?style=for-the-badge)](LICENSE)

[下载最新版本](https://github.com/LuoYue-cn/pcl-ce-linux/releases/latest) |
[上游项目](https://github.com/PCL-Community/PCL-CE) |
[问题反馈](https://github.com/LuoYue-cn/pcl-ce-linux/issues/new/choose)

</div>

## 📖 这是什么

本仓库是 [PCL CE（Plain Craft Launcher 社区版）](https://github.com/PCL-Community/PCL-CE) 的 **Linux 移植版**。

原版 PCL CE 使用 WPF，只能在 Windows 上运行。本仓库基于社区的 Avalonia 迁移工作
（[PCL-CavE](https://github.com/PCL-Community/PCL-CavE)），把界面层替换为 Avalonia，
并补上了 Linux 下的系统集成、数据目录处理与打包发布。

> 本项目是**第三方修改版**，与龙腾猫跃及 PCL-Community 均无隶属关系。
> 遇到问题请提交到[本仓库](https://github.com/LuoYue-cn/pcl-ce-linux/issues)，不要向上游反馈。

## ✨ 功能

- 实例管理、版本安装、Mod / 整合包下载与安装、崩溃分析、皮肤管理等原版功能
- 微软登录、第三方验证、离线登录三种档案
- X11 与 Wayland 均可运行（默认按配置自动选择，也可用环境变量强制）
- 安装包自带 .NET 10 运行时，无需另外安装 .NET

## 💻 支持平台

| 系统 | 支持情况 | 说明 |
|---|---|---|
| Debian 12 / Ubuntu 22.04 及以上 | ✅ 支持 | 提供 `amd64` 安装包，X11 与 Wayland 均可 |
| 其他 Debian 系发行版（Mint、Deepin 等） | ⚠️ 理论可用 | 依赖包名一致时可用，未逐一测试 |
| arm64 | 🛠 需自行构建 | `ARCH=arm64 ./packaging/make-deb.sh` |
| Windows / macOS | ❌ 不适用 | 请使用上游 PCL CE 或原版 PCL |

## 🚀 安装

### 使用安装包（推荐）

从 [Releases](https://github.com/LuoYue-cn/pcl-ce-linux/releases/latest) 下载 `.deb` 后：

```bash
sudo apt install ./pcl-ce_<版本>_amd64.deb
pcl-ce
```

安装后可在应用菜单中找到「PCL 社区版」。

### 从源码构建

需要 [.NET 10 SDK](https://get.dot.net/10) 与 `dpkg-deb`：

```bash
./packaging/make-deb.sh                 # 产物输出到 dist/
VER=2.15.1-1 ./packaging/make-deb.sh    # 指定 Debian 版本
ARCH=arm64 ./packaging/make-deb.sh      # 构建 arm64
```

### 直接运行（不打包）

```bash
dotnet run --project PCL.Launcher -c Release
```

也可以先发布再运行：

```bash
dotnet publish PCL.Launcher/PCL.Launcher.csproj -c Release -r linux-x64 --self-contained true -o out
./out/PCL.Launcher
```

## 📂 数据目录

程序默认把数据写在可执行文件旁边；以安装包方式装在只读的 `/opt` 时，启动脚本会按 XDG 规范重定向：

| 内容 | 路径 | 覆盖变量 |
|---|---|---|
| 数据、日志、游戏实例 | `~/.local/share/PCLCE` | `PCL_PATH` / `PCL_PATH_LOCAL` |
| 配置 | `~/.config/PCLCE` | `PCL_PATH_SHARED` |
| 临时文件 | `$TMPDIR/PCLCE` | `PCL_PATH_TEMP` |

默认游戏目录：程序目录可写时为 `<程序目录>/.minecraft`；否则优先使用已存在的 `~/.minecraft`，
若不存在则使用 `~/.local/share/PCLCE/.minecraft`。

## 🔧 环境变量

| 变量 | 作用 |
|---|---|
| `PCL_OPAQUE_WINDOW=1` | 强制不透明窗口（窗口透明不可用、出现白边时使用） |
| `PCL_FORCE_X11=1` | 强制使用 X11 |
| `PCL_USE_WAYLAND=1` | 强制使用 Wayland |
| `PCL_DISABLE_DEBUG_HINT=1` | 隐藏 Debug 构建的提示 |

## ⚠️ 已知限制

- 仅提供 `amd64` 安装包，其他架构需自行构建
- 安装包未做 GPG 签名
- 应用内自动更新不适用于 `/opt` 安装，请通过新版安装包升级
- 本仓库基于尚未合入上游的 Avalonia 迁移分支，功能完整度请以实际测试为准

## 🔒 许可证与署名

- 原始作品：**Plain Craft Launcher**，作者 **龙腾猫跃**；社区维护版本：[PCL-Community/PCL-CE](https://github.com/PCL-Community/PCL-CE)
- Avalonia 迁移基础：[PCL-Community/PCL-CavE](https://github.com/PCL-Community/PCL-CavE)
- `PCL.Launcher/`、`PCL.Launcher.Core/` 使用 [自定义许可](PCL.Launcher.Core/LICENCE)（《PCL 分发有限许可》）
- 其余目录使用 [Apache License 2.0](LICENSE)

本仓库为第三方二次修改版本，保留上游许可文本与作者署名。

## 🤖 AI 使用说明

本仓库的 Linux 移植、打包与界面修复使用了生成式 AI（DeepSeek-V4.1-flash）辅助完成，
所有改动均已人工验证。
