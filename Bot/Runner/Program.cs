using System.Reflection;

namespace Runner;

/// <summary>
/// Entry point for running different configurations of the trading engine.
/// Usage: dotnet run -- --fileName <ClassName/>
///  - ClassName is the name of the class to use as an entry point. It must have a static Run() method.
/// </summary>
class Program
{
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

        // Run(), with no params
        method.Invoke(instance, null);
    }
}
