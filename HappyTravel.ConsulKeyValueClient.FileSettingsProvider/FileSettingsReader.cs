using System;
using System.Collections.Generic;
using System.IO;
using HappyTravel.ConsulKeyValueClient.Abstractions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using JsonSerializer = Newtonsoft.Json.JsonSerializer;

namespace HappyTravel.ConsulKeyValueClient.FileSettingsProvider
{
    public class FileSettingsReader : ISettingsReader
    {
        public FileSettingsReader(ILoggerFactory loggerFactory, IOptions<DiplomatOptions> options)
        {
            _logger = loggerFactory.CreateLogger<FileSettingsReader>();
            _options = options.Value;
        }


        public Dictionary<string, object> Read()
        {
            var targetPath = _options.LocalSettingsPath;
            if (string.IsNullOrWhiteSpace(targetPath))
            {
                Console.WriteLine("You must specify the [path] to a settings file");
                return new Dictionary<string, object>();
            }

            var toolPath = Directory.GetCurrentDirectory();
            var filePath = Path.GetFullPath(Path.Combine(toolPath, targetPath));

            Console.WriteLine($"Getting settings from '{filePath}'...");

            using var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            fileStream.Seek(0, SeekOrigin.Begin);

            using var reader = new StreamReader(fileStream);
            using var jsonReader = new JsonTextReader(reader);

            var serializer = new JsonSerializer();
            return serializer.Deserialize<Dictionary<string, object>>(jsonReader)!;
        }
    
        
        private readonly ILogger _logger;
        private readonly DiplomatOptions _options;
    }
}
