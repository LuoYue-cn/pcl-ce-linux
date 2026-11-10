# Linux 打包

本目录用于把 Avalonia 版 PCL CE 打包成 Debian 安装包。

## 构建

```bash
# 依赖：.NET 10 SDK、dpkg-deb
./packaging/make-deb.sh

# 指定 Debian 版本 / 架构
VER=2.15.1-1 ./packaging/make-deb.sh
ARCH=arm64 ./packaging/make-deb.sh
```

产物输出到 `dist/`：

```
dist/pcl-ce_<version>_<arch>.deb
```

## 安装

```bash
sudo apt install ./dist/pcl-ce_2.15.1-1_amd64.deb
pcl-ce
```

## 布局

```
/opt/pcl-ce/                                  自包含 .NET 运行时与程序
/usr/bin/pcl-ce                               启动脚本
/usr/share/applications/pcl-community-edition.desktop
/usr/share/icons/hicolor/256x256/apps/pcl-community-edition.png
```

## 数据目录

程序默认把数据写在可执行文件旁边。通过包管理器安装到只读的 `/opt` 时，
启动脚本按 XDG 规范设置：

| 环境变量 | 位置 |
|---|---|
| `PCL_PATH` / `PCL_PATH_LOCAL` | `$XDG_DATA_HOME/PCLCE`（默认 `~/.local/share/PCLCE`） |
| `PCL_PATH_SHARED` | `$XDG_CONFIG_HOME/PCLCE`（默认 `~/.config/PCLCE`） |

默认游戏目录在程序目录不可写时回退为 `~/.minecraft`（若已存在），
否则为 `$XDG_DATA_HOME/PCLCE/.minecraft`。

## 兼容性开关

| 环境变量 | 作用 |
|---|---|
| `PCL_OPAQUE_WINDOW=1` | 强制不透明窗口，用于窗口透明不可用、出现白边的环境 |
| `PCL_FORCE_X11=1` | 强制使用 X11 而非 Wayland |
| `PCL_USE_WAYLAND=1` | 强制使用 Wayland |
