// -----------------------------------------------------------------------
//     Copyright (c) 2025 Matthew Turner.
//     Licensed under the MIT-NC License (Non-Commercial).
// -----------------------------------------------------------------------

using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;

namespace FormRunner;

internal static class Program
{
    // Import the necessary Windows API functions
    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern bool AllocConsole();

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern IntPtr GetStdHandle(int nStdHandle);

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern bool GetConsoleMode(IntPtr hConsoleHandle, out uint lpMode);

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern bool SetConsoleMode(IntPtr hConsoleHandle, uint dwMode);

    private const int STD_OUTPUT_HANDLE = -11;
    private const uint ENABLE_VIRTUAL_TERMINAL_PROCESSING = 0x0004;

    static void EnableAnsiColors()
    {
        IntPtr consoleHandle = GetStdHandle(STD_OUTPUT_HANDLE);
        if (GetConsoleMode(consoleHandle, out uint mode))
        {
            SetConsoleMode(consoleHandle, mode | ENABLE_VIRTUAL_TERMINAL_PROCESSING);
        }
    }

    /// <summary>
    /// The main entry point for the application.
    /// </summary>
    [STAThread]
    [SupportedOSPlatform("windows")]
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
        EnableAnsiColors();

        // To customize application configuration such as set high DPI settings or default font,
        // see https://aka.ms/applicationconfiguration.
        ApplicationConfiguration.Initialize();

        // Run(), with no params
        Form form = (Form)method.Invoke(instance, null)!;

        Application.Run(form);
    }
}