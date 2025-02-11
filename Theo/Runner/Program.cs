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

        // Run()
        executeMethod.Invoke(null, null);
    }
}
