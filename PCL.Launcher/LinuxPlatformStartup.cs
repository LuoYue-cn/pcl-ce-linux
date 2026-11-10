using System.Text.Json;
using Avalonia;
using Avalonia.Rendering.Composition;
using Avalonia.X11;
using PCL.Core.App;
using PCL.UI.SystemIntegration;

namespace PCL;

internal sealed class LinuxPlatformStartup : IPlatformStartup
{
    public IPlatformShellIntegration Shell { get; } = new LinuxShellIntegration();

    /// <summary>渲染配置摘要，待日志服务就绪后输出。</summary>
    internal static string RenderConfigSummary { get; private set; } = "未配置";

    public void Initialize(string[] args)
    {
    }

    public AppBuilder Configure()
    {
        var builder = AppBuilder
            .Configure<App>()
            .UsePlatformDetect()
            .WithInterFont()
            .LogToTrace();

        builder = builder.With(CreateX11Options());

        if (!ShouldUseWayland())
            return builder;

        LinuxDesktopEntry.Ensure();
        builder = builder
            .With(
                new WaylandPlatformOptions
                {
                    AppId = LinuxDesktopEntry.AppId
                })
            .UseWaylandWithFallback()
            .With(
                new CompositionOptions
                {
                    UseRegionDirtyRectClipping = false
                });

        return builder;
    }

    /// <summary>
    ///     X11 渲染选项。
    ///     默认开启保留帧缓冲：启动游戏后启动器会隐藏主窗口，部分驱动/合成器在窗口
    ///     重新显示时不会保留后备缓冲，导致整窗空白（白屏）。
    ///     可用 <c>PCL_RENDER_MODE=software</c> 强制软件渲染，
    ///     <c>PCL_RENDER_ON_UI_THREAD=1</c> 改为在 UI 线程渲染。
    /// </summary>
    private static X11PlatformOptions CreateX11Options()
    {
        var options = new X11PlatformOptions
        {
            UseRetainedFramebuffer = true
        };

        if (Environment.GetEnvironmentVariable("PCL_RENDER_ON_UI_THREAD") == "1")
            options.ShouldRenderOnUIThread = true;

        var mode = Environment.GetEnvironmentVariable("PCL_RENDER_MODE");
        if (!string.IsNullOrWhiteSpace(mode))
        {
            var modes = new List<X11RenderingMode>();
            foreach (var part in mode.Split(',', StringSplitOptions.RemoveEmptyEntries))
                if (Enum.TryParse<X11RenderingMode>(part.Trim(), true, out var parsed))
                    modes.Add(parsed);
            if (modes.Count > 0)
                options.RenderingMode = modes;
        }

        // 此时日志服务尚未初始化，先记录摘要，待主窗口创建后再写入日志
        RenderConfigSummary =
            $"保留帧缓冲=true, 渲染模式={mode ?? "自动"}"
            + (options.ShouldRenderOnUIThread ? ", UI 线程渲染=true" : string.Empty);

        return options;
    }

    private static bool ShouldUseWayland()
    {
        if (Environment.GetEnvironmentVariable("PCL_FORCE_X11") is not null) return false;
        if (Environment.GetEnvironmentVariable("PCL_USE_WAYLAND") == "1") return true;
        return ReadProtocolFromConfig() == 2;
    }

    private static int ReadProtocolFromConfig()
    {
        try
        {
            var path = Path.Combine(Paths.SharedData, "config.v1.json");
            if (!File.Exists(path)) return 0;
            using var document = JsonDocument.Parse(File.ReadAllText(path));
            return document.RootElement.TryGetProperty("SystemDebugLinuxProtocol", out var value)
                && value.TryGetInt32(out var protocol)
                    ? Math.Clamp(protocol, 0, 2)
                    : 0;
        }
        catch
        {
            return 0;
        }
    }
}