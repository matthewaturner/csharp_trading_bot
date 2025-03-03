using System.Reflection;
using System.Runtime.InteropServices;

namespace FormRunner;

internal static class Program
{

    // Import the AllocConsole function from kernel32.dll
    [DllImport("kernel32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    static extern bool AllocConsole();

    /// <summary>
    ///  The main entry point for the application.
    /// </summary>
    [STAThread]
    static void Main(string[] args)
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

        // Always run a console app
        AllocConsole();

        // To customize application configuration such as set high DPI settings or default font,
        // see https://aka.ms/applicationconfiguration.
        ApplicationConfiguration.Initialize();

        // Run(), with no params
        Form form = (Form)method.Invoke(instance, null)!;

        Application.Run(form);

    }
}