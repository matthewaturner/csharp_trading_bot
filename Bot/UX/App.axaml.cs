using System.Reflection;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;

namespace UX;

public partial class App : Application
{
    public static object? RunInstance { get; set; }
    public static MethodInfo? RunMethod { get; set; }

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            // Execute the Run method which will show the appropriate window
            // The window will set itself as MainWindow when Show() is called
            RunMethod?.Invoke(RunInstance, null);
        }

        base.OnFrameworkInitializationCompleted();
    }
}
