using System;
using System.Linq;
using System.Reflection;
using Avalonia;

namespace UX;

internal class Program
{
    /// <summary>
    /// Initialization code. Don't use any Avalonia, third-party APIs or any
    /// SynchronizationContext-reliant code before AppMain is called: things aren't initialized
    /// yet and stuff might break.
    /// </summary>
    [STAThread]
    public static void Main(string[] args)
    {
        if (args.Length < 2 || args[0] != "--fileName")
        {
            Console.WriteLine("Usage: dotnet run -- --fileName <ClassName>");
            return;
        }

        string className = args[1];

        Type? runType = Assembly.GetExecutingAssembly().GetTypes()
            .FirstOrDefault(t => t.Name.Equals(className, StringComparison.OrdinalIgnoreCase));
        if (runType == null)
        {
            Console.WriteLine($"Error: Class '{className}' not found.");
            return;
        }

        object? instance = Activator.CreateInstance(runType);
        if (instance == null)
        {
            Console.WriteLine($"Error: '{className}' does not have a parameterless constructor.");
            return;
        }

        var method = runType.GetMethod("Run", BindingFlags.Instance | BindingFlags.Public);
        if (method == null)
        {
            Console.WriteLine($"Error: '{className}' does not have a Run() method.");
            return;
        }

        // Store the runnable instance for the app to use
        App.RunInstance = instance;
        App.RunMethod = method;

        BuildAvaloniaApp()
            .StartWithClassicDesktopLifetime(args);
    }

    /// <summary>
    /// Avalonia configuration, don't remove; also used by visual designer.
    /// </summary>
    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .WithInterFont()
            .LogToTrace();
}
