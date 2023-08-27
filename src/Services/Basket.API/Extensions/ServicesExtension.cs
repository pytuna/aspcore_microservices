﻿using Basket.API.GrpcServices;
using Basket.API.Repositories;
using Basket.API.Repositories.Interfaces;
using Contracts.Common.Interfaces;
using EventBus.Messages.IntegrationEvents.Interfaces;
using Grpc.Core;
using Infrastructure.Common;
using Infrastructure.Extensions;
using Inventory_gRPC.Protos;
using MassTransit;
using Microsoft.Extensions.DependencyInjection.Extensions;
using RabbitMQ.Client;
using Shared.Configurations;

namespace Basket.API.Extensions
{
    public static class ServicesExtension
    {
        public static IServiceCollection AddConfigurationSettings(this IServiceCollection services, IConfiguration configuration)
        {
            var eventBusSettings = configuration.GetSection(nameof(EventBusSettings))
                .Get<EventBusSettings>();

            var grpcSettings = configuration.GetSection(nameof(GrpcSettings))
                .Get<GrpcSettings>();

            services.AddSingleton(grpcSettings);
            return services;
        }
        public static IServiceCollection ConfigureServices(this IServiceCollection services)
        {
            services.AddScoped<IBasketRepository, BasketRepository>();
            services.AddTransient<ISerializeService, SerializeService>();
            //services.AddTransient<IDistributedCacheService, DistributedCacheService>(); 
            return services;
        }
        
        public static void ConfigureRedis(this IServiceCollection services, IConfiguration configuration)
        {
            var redisSettings = services.GetOptions<CacheSettings>(nameof(CacheSettings));
            if (string.IsNullOrEmpty(redisSettings.ConnectionString))
            {
                throw new ArgumentNullException(nameof(redisSettings));
            }

            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = redisSettings.ConnectionString;
            });
        }

        public static IServiceCollection ConfigureGrpcServices (this IServiceCollection services)
        {
            var settings = services.GetOptions<GrpcSettings>(nameof(GrpcSettings));
            services.AddGrpcClient<StockProtoService.StockProtoServiceClient>(options =>
            {
                options.Address = new Uri(settings.StockUrl);
                // configue no ssl
                //options.ChannelOptionsActions.Add(channelOptions =>
                //{
                //    channelOptions.Credentials = ChannelCredentials.Insecure;
                //});
            });

            services.AddScoped<StockItemGrpcService>();
            return services;
        }

        public static void ConfigureMassTransit(this IServiceCollection services)
        {
            var settings = services.GetOptions<EventBusSettings>(nameof(EventBusSettings));
            if (string.IsNullOrEmpty(settings.HostAddress))
            {
                throw new ArgumentNullException(nameof(settings));
            }
            var mqConnection = new Uri(settings.HostAddress);
            services.TryAddSingleton(KebabCaseEndpointNameFormatter.Instance);
            services.AddMassTransit(config =>
            {
                config.UsingRabbitMq((ctx, cfg) =>
                {
                    cfg.Host(mqConnection);
                    //cfg.Publish<IBasketCheckoutEvent>(c =>
                    //{
                    //    c.Durable = true;
                    //    c.ExchangeType = ExchangeType.Fanout;
                    //});
                });

                config.AddRequestClient<IBasketCheckoutEvent>();

                // using rabbittmq and exchange header
                //config.UsingRabbitMq((ctx, cfg) =>
                //{
                //    cfg.Host(mqConnection);
                //    //cfg.ExchangeType = ExchangeType.Headers;
                //    cfg.ConfigureEndpoints(ctx, KebabCaseEndpointNameFormatter.Instance);

                //    //// add message
                //    //cfg.Message<IBasketCheckoutEvent>(x => x.SetEntityName("basket_checkout_queue"));
                //    cfg.Publish<IBasketCheckoutEvent>(x => x.ExchangeType = ExchangeType.Headers);

                //});
            });
        }
    }
}
