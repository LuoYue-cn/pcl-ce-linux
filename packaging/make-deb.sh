#!/bin/bash
# 构建 PCL CE (Linux / Avalonia) 的 Debian 安装包。
#
#   ./packaging/make-deb.sh              # 版本号取 csproj，修订号 -1
#   VER=2.15.1-3 ./packaging/make-deb.sh # 指定完整 Debian 版本
#   ARCH=arm64 ./packaging/make-deb.sh   # 需要对应 RuntimeIdentifier
#
# 产物默认输出到 <仓库>/dist/。
set -euo pipefail
umask 022

SRC="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
OUT="${OUT:-$SRC/dist}"
APP=pcl-ce
APPID=pcl-community-edition
ARCH="${ARCH:-amd64}"
UPSTREAM_VERSION="$(sed -n 's:.*<Version>\(.*\)</Version>.*:\1:p' \
    "$SRC/PCL.Launcher/PCL.Launcher.csproj" | head -1)"
VER="${VER:-${UPSTREAM_VERSION:-2.15.1}-1}"
RID="linux-$ARCH"

PKG="$OUT/pkg"

rm -rf "$PKG"
mkdir -p "$PKG/opt/pcl-ce" \
         "$PKG/usr/bin" \
         "$PKG/usr/share/applications" \
         "$PKG/usr/share/icons/hicolor/256x256/apps" \
         "$PKG/usr/share/doc/$APP" \
         "$PKG/DEBIAN"

echo "[1/6] 发布自包含二进制…"
dotnet publish "$SRC/PCL.Launcher/PCL.Launcher.csproj" \
    -c Release -r "$RID" --self-contained true \
    -p:DebugType=none -p:GenerateDocumentationFile=false \
    -o "$PKG/opt/pcl-ce" > "$OUT/publish.log" 2>&1
find "$PKG/opt/pcl-ce" -name '*.pdb' -delete
find "$PKG/opt/pcl-ce" -type d -exec chmod 0755 {} +
find "$PKG/opt/pcl-ce" -type f -exec chmod 0644 {} +
find "$PKG/opt/pcl-ce" -maxdepth 1 -type f -executable -exec chmod 0755 {} +
chmod 0755 "$PKG/opt/pcl-ce/PCL.Launcher"

echo "[2/6] 生成启动脚本…"
cat > "$PKG/usr/bin/$APP" <<'LAUNCHER'
#!/bin/sh
# PCL Community Edition 启动脚本
#
# 程序默认把数据、配置与日志写在可执行文件旁边。通过包管理器安装时该目录
# 位于只读的 /opt，因此这里按 XDG 规范把各个目录重定向到用户目录。
set -e

: "${XDG_DATA_HOME:=$HOME/.local/share}"
: "${XDG_CONFIG_HOME:=$HOME/.config}"

DATA_DIR="$XDG_DATA_HOME/PCLCE"
CONFIG_DIR="$XDG_CONFIG_HOME/PCLCE"
mkdir -p "$DATA_DIR" "$CONFIG_DIR"

# 数据目录（游戏实例、日志、下载缓存等）
export PCL_PATH="$DATA_DIR"
# 本地缓存目录
export PCL_PATH_LOCAL="$DATA_DIR"
# 配置与同步数据目录
export PCL_PATH_SHARED="$CONFIG_DIR"

exec /opt/pcl-ce/PCL.Launcher "$@"
LAUNCHER
chmod 0755 "$PKG/usr/bin/$APP"

echo "[3/6] 生成 .desktop…"
cat > "$PKG/usr/share/applications/$APPID.desktop" <<'DESKTOP'
[Desktop Entry]
Type=Application
Version=1.0
Name=PCL Community Edition
Name[zh_CN]=PCL 社区版
Name[zh_TW]=PCL 社群版
GenericName=Minecraft Launcher
GenericName[zh_CN]=Minecraft 启动器
Comment=Launch and manage Minecraft
Comment[zh_CN]=启动与管理 Minecraft
Exec=pcl-ce %U
Icon=pcl-community-edition
Terminal=false
Categories=Game;
Keywords=minecraft;launcher;pcl;
StartupNotify=true
StartupWMClass=pcl-community-edition
DESKTOP

echo "[4/6] 安装图标与版权文件…"
install -m 0644 "$SRC/PCL.Launcher/Assets/Images/icon.png" \
    "$PKG/usr/share/icons/hicolor/256x256/apps/$APPID.png"

{
    echo "Format: https://www.debian.org/doc/packaging-manuals/copyright-format/1.0/"
    echo "Upstream-Name: PCL-CE"
    echo "Source: https://github.com/PCL-Community/PCL-CE"
    echo
    echo "Files: *"
    echo "Copyright: 2016-2026 龙腾猫跃 (Plain Craft Launcher)"
    echo "           2024-2026 PCL Community"
    echo "License: Apache-2.0"
    echo " 本包中的 PCL.Core、PCL.Core.UI 等组件以 Apache License 2.0 授权，"
    echo " 完整文本见 /opt/pcl-ce/LICENSE。"
    echo
    echo "Files: opt/pcl-ce/LICENCE.PCL.Launcher.Core"
    echo "Copyright: 成都瓜皮龙科技有限公司"
    echo "License: PCL-Distribution-Limited"
    echo " 《PCL 分发有限许可》：允许在满足署名、命名、开源与保留本协议等条件"
    echo " 的前提下修改与再分发；完整文本见 /opt/pcl-ce/LICENCE.PCL.Launcher.Core。"
} > "$PKG/usr/share/doc/$APP/copyright"
chmod 0644 "$PKG/usr/share/doc/$APP/copyright" \
           "$PKG/usr/share/applications/$APPID.desktop"

echo "[5/6] 生成 control…"
INSTALLED_SIZE=$(du -sk --exclude=DEBIAN "$PKG" | cut -f1)

cat > "$PKG/DEBIAN/control" <<CONTROL
Package: $APP
Version: $VER
Section: games
Priority: optional
Architecture: $ARCH
Installed-Size: $INSTALLED_SIZE
Maintainer: PCL-CE Linux Packagers <noreply@users.noreply.github.com>
Homepage: https://github.com/PCL-Community/PCL-CE
Depends: libc6, libgcc-s1, libstdc++6,
 libfontconfig1, libfreetype6, libexpat1, libpng16-16, zlib1g, libbz2-1.0, libbrotli1,
 libx11-6, libxext6, libxrandr2, libxi6, libxcursor1, libxfixes3, libice6, libsm6,
 libgl1, libegl1, libdrm2, libgbm1,
 libicu76 | libicu74 | libicu72 | libicu70,
 libssl3t64 | libssl3
Recommends: libwayland-client0, libwayland-cursor0, libwayland-egl1, libxkbcommon0,
 libgtk-3-0t64 | libgtk-3-0,
 fonts-noto-cjk | fonts-wqy-zenhei
Description: PCL Community Edition - Minecraft launcher (Linux build)
 PCL CE is a community fork of Plain Craft Launcher (PCL), featuring
 instance management, modpack installation, mod download, crash analysis
 and multiplayer support.
 .
 This package contains the Avalonia UI port of PCL CE, built as a
 self-contained .NET 10 application for Linux (X11 and Wayland).
CONTROL

# 不生成 postinst/postrm：桌面与图标缓存由 desktop-file-utils / hicolor-icon-theme
# 的 dpkg trigger 负责。自建脚本会在 root 环境下调用 GLib 工具，可能因为
# 连不上用户 D-Bus 会话而输出 org.freedesktop.DBus.Error.NoReply。

echo "[6/6] 打包…"
mkdir -p "$OUT"
dpkg-deb --root-owner-group -Zxz --build "$PKG" "$OUT/${APP}_${VER}_${ARCH}.deb"
ls -lh "$OUT/${APP}_${VER}_${ARCH}.deb"
