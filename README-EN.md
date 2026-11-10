[简体中文](README.md) | **English** | [繁體中文](README-ZH_TW.md)

<div align="center">

<img src="PCL.Launcher/Assets/Images/icon.svg" alt="Logo" width="80" height="80">

# PCL CE Linux

An Avalonia-based Linux build of PCL CE, shipped as a ready-to-install Debian package.

[![Release](https://img.shields.io/github/v/release/LuoYue-cn/pcl-ce-linux?label=release&style=for-the-badge&logo=github)](https://github.com/LuoYue-cn/pcl-ce-linux/releases/latest)
[![Downloads](https://img.shields.io/github/downloads/LuoYue-cn/pcl-ce-linux/total?style=for-the-badge&label=downloads)](https://github.com/LuoYue-cn/pcl-ce-linux/releases)
[![Issues](https://img.shields.io/github/issues/LuoYue-cn/pcl-ce-linux?style=for-the-badge&label=issues&labelColor=444444&color=1F883D)](https://github.com/LuoYue-cn/pcl-ce-linux/issues)

[Download](https://github.com/LuoYue-cn/pcl-ce-linux/releases/latest) |
[Upstream](https://github.com/PCL-Community/PCL-CE) |
[Report an issue](https://github.com/LuoYue-cn/pcl-ce-linux/issues/new/choose)

</div>

## 📖 What is this

This repository is a **Linux port** of [PCL CE (Plain Craft Launcher Community Edition)](https://github.com/PCL-Community/PCL-CE).

Upstream PCL CE is built on WPF and only runs on Windows. This port builds on the community
Avalonia migration ([PCL-CavE](https://github.com/PCL-Community/PCL-CavE)), replaces the UI layer
with Avalonia, and adds Linux system integration, data-directory handling and packaging.

> This is an **unofficial, third-party build**, not affiliated with 龙腾猫跃 or PCL-Community.
> Please report issues [here](https://github.com/LuoYue-cn/pcl-ce-linux/issues), not upstream.

## ✨ Features

- Instance management, game installation, mod / modpack download and install, crash analysis, skins
- Microsoft, third-party (authlib) and offline profiles
- Runs on X11 and Wayland (auto-detected, can be forced with environment variables)
- The package is self-contained (.NET 10 runtime bundled)

## 💻 Supported platforms

| System | Status | Notes |
|---|---|---|
| Debian 12 / Ubuntu 22.04+ | ✅ Supported | `amd64` package, X11 and Wayland |
| Other Debian-based distros | ⚠️ Expected to work | Not individually tested |
| arm64 | 🛠 Build it yourself | `ARCH=arm64 ./packaging/make-deb.sh` |
| Windows / macOS | ❌ Not applicable | Use upstream PCL CE or the original PCL |

## 🚀 Install

### From the package (recommended)

Download the `.deb` from [Releases](https://github.com/LuoYue-cn/pcl-ce-linux/releases/latest):

```bash
sudo apt install ./pcl-ce_<version>_amd64.deb
pcl-ce
```

### Build from source

Requires [.NET 10 SDK](https://get.dot.net/10) and `dpkg-deb`:

```bash
./packaging/make-deb.sh                 # output in dist/
VER=2.15.1-1 ./packaging/make-deb.sh    # custom Debian version
ARCH=arm64 ./packaging/make-deb.sh      # arm64
```

### Run without packaging

```bash
dotnet run --project PCL.Launcher -c Release
```

## 📂 Data directories

The launcher writes data next to its executable by default. When installed read-only under
`/opt`, the launcher script redirects everything following the XDG spec:

| Content | Path | Override |
|---|---|---|
| Data, logs, instances | `~/.local/share/PCLCE` | `PCL_PATH` / `PCL_PATH_LOCAL` |
| Configuration | `~/.config/PCLCE` | `PCL_PATH_SHARED` |
| Temporary files | `$TMPDIR/PCLCE` | `PCL_PATH_TEMP` |

Default game folder: `<install dir>/.minecraft` when writable; otherwise an existing `~/.minecraft`,
or `~/.local/share/PCLCE/.minecraft`.

## 🔧 Environment variables

| Variable | Effect |
|---|---|
| `PCL_OPAQUE_WINDOW=1` | Force an opaque window (use when transparency is unavailable) |
| `PCL_FORCE_X11=1` | Force X11 |
| `PCL_USE_WAYLAND=1` | Force Wayland |
| `PCL_DISABLE_DEBUG_HINT=1` | Hide the Debug-build hint |

## ⚠️ Known limitations

- `amd64` packages only; other architectures must be built locally
- The package is not GPG-signed
- In-app auto-update does not apply to `/opt` installs; upgrade via a new package
- Based on an Avalonia migration branch that is not merged upstream yet

## 🔒 License & credits

- Original work: **Plain Craft Launcher** by **龙腾猫跃**; community edition: [PCL-Community/PCL-CE](https://github.com/PCL-Community/PCL-CE)
- Avalonia migration base: [PCL-Community/PCL-CavE](https://github.com/PCL-Community/PCL-CavE)
- `PCL.Launcher/` and `PCL.Launcher.Core/` use a [custom license](PCL.Launcher.Core/LICENCE)
- Everything else is licensed under [Apache License 2.0](LICENSE)

## 🤖 AI usage

Generative AI (DeepSeek-V4.1-flash) was used to assist with the Linux port, packaging and UI fixes.
All changes were reviewed and verified manually.
