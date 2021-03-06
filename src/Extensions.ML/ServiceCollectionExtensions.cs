﻿using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;

namespace Extensions.ML
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddPredictionEngine<TData, TPrediction>(this IServiceCollection services, string modelPath) where TData : class where TPrediction : class, new()
        {
            services.AddPredictionEngine<TData, TPrediction>(options => options.CreateModel = (mlContext) => {
                using (var fileStream = File.OpenRead(modelPath))
                {
                    return mlContext.Model.Load(fileStream);
                }
            });
            return services;
        }

        public static IServiceCollection AddPredictionEngine<TData, TPrediction>(this IServiceCollection services, Stream modelStream) where TData : class where TPrediction : class, new()
        {
            services.AddPredictionEngine<TData, TPrediction>(options => options.CreateModel = (mlContext) => {
                    return mlContext.Model.Load(modelStream);
            });
            return services;
        }

        public static IServiceCollection AddPredictionEngine<TData, TPrediction>(this IServiceCollection services, Action<PredictionEnginePoolOptions<TData, TPrediction>> configure) where TData : class where TPrediction : class, new()
        {
            services.Configure(configure);
            services.TryAddEnumerable(ServiceDescriptor.Singleton<IPostConfigureOptions<MLContextOptions>, PostMLContextOptionsConfiguration>());
            services.AddSingleton<PredictionEnginePool<TData, TPrediction>, PredictionEnginePool<TData, TPrediction>>();
            return services;
        }
    }
}
