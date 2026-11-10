using PCL.Core.App;

namespace PCL;

/// <summary>业务层使用的路径常量。</summary>
public static class LauncherPaths
{
    /// <summary>Mesa Loader Windows 版本。</summary>
    public const string MesaLoaderWindowsVersion = "26.0.4";

    /// <summary>程序内嵌图片根路径，具体 URI 方案由 UI 层注册的资源访问实现决定。</summary>
    public static string ImageRoot => Basics.GetAppImageRootUri();

    /// <summary>程序可执行文件所在目录，以路径分隔符结尾。</summary>
    public static string ExecutableDirectory { get; } =
        Basics.ExecutableDirectory.EndsWith(Path.DirectorySeparatorChar)
            ? Basics.ExecutableDirectory
            : Basics.ExecutableDirectory + Path.DirectorySeparatorChar;

    /// <summary>临时目录，以路径分隔符结尾。</summary>
    public static string Temp => Paths.Temp + Path.DirectorySeparatorChar;

    /// <summary>
    ///     程序目录是否可写。包管理器通常会把程序安装到只读位置，
    ///     此时所有需要写入的内容都应改放到用户目录。
    /// </summary>
    private static bool ExecutableDirectoryWritable => _ExecutableDirectoryWritable.Value;

    // 注意：必须声明在所有使用它的静态字段之前，否则初始化顺序会导致空引用。
    private static readonly Lazy<bool> _ExecutableDirectoryWritable =
        new(_ProbeExecutableDirectoryWritable);

    /// <summary>在不留下残留文件的前提下探测程序目录是否可写。</summary>
    private static bool _ProbeExecutableDirectoryWritable()
    {
        var probe = Path.Combine(ExecutableDirectory, $".pcl-write-probe-{Environment.ProcessId}");
        try
        {
            using (File.Create(probe, 1, FileOptions.DeleteOnClose))
            {
            }

            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    /// <summary>
    ///     默认游戏目录，不以路径分隔符结尾。程序目录可写时沿用便携式布局
    ///     （程序目录下的 <c>.minecraft</c>）；若通过包管理器安装到只读位置，
    ///     则回退到已有的 <c>~/.minecraft</c>，否则使用用户数据目录。
    /// </summary>
    public static string DefaultMinecraftFolder { get; } = _ResolveDefaultMinecraftFolder();

    private static string _ResolveDefaultMinecraftFolder()
    {
        var preferred = Path.Combine(ExecutableDirectory, ".minecraft");
        if (Directory.Exists(preferred) || ExecutableDirectoryWritable) return preferred;

        var home = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        if (!string.IsNullOrEmpty(home))
        {
            var homeFolder = Path.Combine(home, ".minecraft");
            if (Directory.Exists(homeFolder)) return homeFolder;
        }

        return Path.Combine(Paths.Data, ".minecraft");
    }

    /// <summary>可用于临时存放文件的、不含特殊字符的目录，以路径分隔符结尾。</summary>
    public static string PureAsciiDirectory
    {
        get
        {
            // 程序目录可写时沿用便携布局；安装到只读位置（如 /opt）时改用用户数据目录
            if (ExecutableDirectory.All(c => c < 128) && ExecutableDirectoryWritable)
                return Path.Combine(ExecutableDirectory, "PCL") + Path.DirectorySeparatorChar;

            var data = Paths.Data;
            if (!string.IsNullOrEmpty(data) && data.All(c => c < 128))
                return Path.Combine(data, "PCL") + Path.DirectorySeparatorChar;

            var temp = Path.GetTempPath();
            if (temp.All(c => c < 128))
                return temp.EndsWith(Path.DirectorySeparatorChar)
                    ? temp
                    : temp + Path.DirectorySeparatorChar;

            return !PCL.Core.Platform.PlatformRuntime.Current.Capabilities.UsesWindowsPaths
                ? Path.Combine(temp, "PCL") + Path.DirectorySeparatorChar
                : Path.Combine(
                    Path.GetPathRoot(Environment.SystemDirectory)
                    ?? Path.DirectorySeparatorChar.ToString(),
                    "ProgramData",
                    "PCL");
        }
    }

    /// <summary>当前 Minecraft 文件夹路径，由 WPF 宿主同步设置。</summary>
    public static string? MinecraftFolder { get; set; }
}