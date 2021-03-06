﻿using System;
using System.Net.Http.Headers;
using HappyTravel.ConsulKeyValueClient.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace HappyTravel.ConsulKeyValueClient.ConsulClient.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddConsulDiplomatProvider(this IServiceCollection services, Config config)
        {
            services.Configure<Config>(c =>
            {
                c.Address = config.Address;
                c.Scheme = config.Scheme;
                c.Token = config.Token;
            });

            var baseUrl = $"{config.Scheme}://{config.Address}";
            services.AddHttpClient(Client.HttpClientName, c =>
            {
                c.BaseAddress = new Uri(baseUrl);
                c.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                c.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", config.Token);
            });
            services.AddHttpClient(Client.HttpUploadClientName, c =>
            {
                c.BaseAddress = new Uri(baseUrl);
                c.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/x-www-form-urlencoded"));
                c.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", config.Token);
            });

            services.AddTransient<IKvClient, KvClient>();
            services.AddTransient<ISettingsProvider, ConsulSettingsProvider>();

            return services;
        }
    }
}
