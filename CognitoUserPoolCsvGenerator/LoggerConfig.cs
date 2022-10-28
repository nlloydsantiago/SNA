using Serilog;

namespace CognitoUserPoolCsvGenerator
{
    internal class LoggerConfig
    {
        public static ILogger Configure() 
            => new LoggerConfiguration()
                .MinimumLevel.Information()
                .WriteTo.Console()
                .CreateLogger();
    }
}
