﻿using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Diplomat;
using Diplomat.ConfigurationProvider.Extensions;
using Diplomat.Extensions;
using HappyTravel.Diplomat.Abstractions;
using HappyTravel.Diplomat.Consul.Api;
using HappyTravel.Diplomat.Consul.Api.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Linq;

namespace IntegrationConsole
{
    internal class Program
    {
        private static async Task Main(string[] _)
        {
            var builder = new HostBuilder()
                .ConfigureServices((hostContext, services) =>
                {
                    services.Configure<DiplomatOptions>(o =>
                    {
                        o.LocalSettingsPath = @"..\..\..\..\test-settings.json";
                        o.KeyPrefix = "tsutsujigasaki/production";
                    });
                    services.AddDiplomat();
                    services.AddConsulDiplomatProvider(ConfigFactory.FromEnvironment());
                }).UseConsoleLifetime();
 
            var host = builder.Build();

            var address = "";
            var path = "";
            var token = "";
            
            var config = new ConfigurationBuilder()
                .AddDiplomat(new List<Uri>{new(address)}, path, token)
                .Build();
            
            var debug = config.GetDebugView();
            ;
            
            host.Run();
        }
    }
}
