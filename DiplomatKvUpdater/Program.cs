using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using HappyTravel.Diplomat.Consul.Api;
using HappyTravel.Diplomat.Consul.Api.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;

namespace Diplomat.DiplomatKvUpdater
{
    internal class Program
    {
        private static async Task Main(string[] args)
        {
            if (args.Length == 0)
                return;

            if (string.Compare(args[0], "version", StringComparison.InvariantCultureIgnoreCase) == 0 ||
                string.Compare(args[0], "v", StringComparison.InvariantCultureIgnoreCase) == 0 || 
                string.Compare(args[0], "help", StringComparison.InvariantCultureIgnoreCase) == 0 ||
                string.Compare(args[0], "h", StringComparison.InvariantCultureIgnoreCase) == 0)
            {
                var versionString = Assembly.GetEntryAssembly()!
                    .GetCustomAttribute<AssemblyInformationalVersionAttribute>()
                    ?.InformationalVersion;

                Console.WriteLine("e,  environment <ENVIRONMENT_NAME>  The environment name to map to a Consul store section in a URL-friendly format.");
                Console.WriteLine("p,  path <PATH_TO_SETTINGS>         The path to the service settings file in .json format");
                Console.WriteLine("s,  service-name <SERVICE_NAME>     The service name to map to a Consul store section in a URL-friendly format");
                Console.WriteLine(Environment.NewLine);
                Console.WriteLine($"Diplomat KV Updater {versionString}");
                return;
            }

            await UpdateSettings(args);
        }


        private static string GetArgValue(string[] args, string[] keys)
        {
            for (var i = 0; i < args.Length; i++)
            {
                if (!keys.Any(key => key.Equals(args[i], StringComparison.InvariantCulture)))
                    continue;

                if (i + 1 >= args.Length)
                    continue;

                return args[i + 1];
            }

            return null;
        }


        private static string GetEnvironment(string[] args)
        {
            var environment = GetArgValue(args, new []{"environment", "e"});
            if (!string.IsNullOrWhiteSpace(environment))
                return environment.ToLowerInvariant();

            Console.WriteLine("You must specify the [environment] to a settings file");
            return null;

        }


        private static IKvClient GetKvClient()
        {
            var builder = new HostBuilder()
                .ConfigureServices((hostContext, services) => { services.AddConsulDiplomatProvider(ConfigFactory.FromEnvironment()); }).UseConsoleLifetime();

            var host = builder.Build();

            using var serviceScope = host.Services.CreateScope();
            var serviceProvider = serviceScope.ServiceProvider;

            return serviceProvider.GetRequiredService<IKvClient>();
        }


        private static string GetServiceName(string[] args)
        {
            var serviceName = GetArgValue(args, new []{"service-name", "s"});
            if (!string.IsNullOrWhiteSpace(serviceName))
                return serviceName.ToLowerInvariant();

            Console.WriteLine("You must specify the [service-name] to a settings file");
            return null;

        }


        private static Dictionary<string, object> ReadSettings(string[] args)
        {
            var targetPath = GetArgValue(args, new []{"path", "p"});
            if (string.IsNullOrWhiteSpace(targetPath))
            {
                Console.WriteLine("You must specify the [path] to a settings file");
                return null;
            }

            var toolPath = Directory.GetCurrentDirectory();
            var filePath = Path.GetFullPath(Path.Combine(toolPath, targetPath));

            Console.WriteLine($"Getting settings from '{filePath}'...");

            using var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            fileStream.Seek(0, SeekOrigin.Begin);

            using var reader = new StreamReader(fileStream);
            using var jsonReader = new JsonTextReader(reader);

            var serializer = new JsonSerializer();
            return serializer.Deserialize<Dictionary<string, object>>(jsonReader);
        }


        private static async Task UpdateSettings(string[] args)
        {
            var serviceName = GetServiceName(args);
            if (serviceName is null)
                return;

            var environment = GetEnvironment(args);
            if (environment is null)
                return;
            
            var settings = ReadSettings(args);
            if (settings is null)
                return;

            var kvClient = GetKvClient();
            foreach (var (key, value) in settings)
            {
                var combinedKey = $"{serviceName}/{environment}/{key}";
                Console.Write($"\rUpdating '{combinedKey}'...");

                await kvClient.Create(combinedKey, value);
            }

            Console.WriteLine(Environment.NewLine);
            Console.WriteLine("Done");
        }
    }
}
