using System.Reflection;

namespace Runner;

/// <summary>
/// Entry point for running different configurations of the trading engine.
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

        string className = args[1]; // Example: "MomentumRun"
        var types = Assembly.GetExecutingAssembly().GetTypes();
        Type? runType = Assembly.GetExecutingAssembly().GetTypes()
            .FirstOrDefault(t => t.Name.Equals(className, StringComparison.OrdinalIgnoreCase));

        if (runType == null)
        {
            Console.WriteLine($"Error: Class '{className}' not found.");
            return;
        }

        MethodInfo? executeMethod = runType.GetMethod("Run", BindingFlags.Static | BindingFlags.Public);
        if (executeMethod == null)
        {
            Console.WriteLine($"Error: '{className}' does not have a Run() method.");
            return;
        }

        // Run the found method
        executeMethod.Invoke(null, null);
    }
}
