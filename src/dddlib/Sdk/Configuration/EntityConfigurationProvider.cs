﻿// <copyright file="EntityConfigurationProvider.cs" company="dddlib contributors">
//  Copyright (c) dddlib contributors. All rights reserved.
// </copyright>

namespace dddlib.Sdk.Configuration
{
    using System;
    using System.Collections.Generic;

    internal class EntityConfigurationProvider
    {
        private readonly Dictionary<Type, EntityConfiguration> config = new Dictionary<Type, EntityConfiguration>();

        private readonly IBootstrapperProvider bootstrapperProvider;
        private readonly EntityAnalyzer typeAnalyzer;

        public EntityConfigurationProvider(IBootstrapperProvider bootstrapperProvider, EntityAnalyzer typeAnalyzer)
        {
            Guard.Against.Null(() => bootstrapperProvider);
            Guard.Against.Null(() => typeAnalyzer);

            this.bootstrapperProvider = bootstrapperProvider;
            this.typeAnalyzer = typeAnalyzer;
        }

        public EntityConfiguration GetConfiguration(Type type)
        {
            var runtimeTypeConfiguration = default(EntityConfiguration);
            if (!this.config.TryGetValue(type, out runtimeTypeConfiguration))
            {
                this.config.Add(type, runtimeTypeConfiguration = this.GetRuntimeTypeConfiguration(type));
            }

            return runtimeTypeConfiguration;
        }

        private EntityConfiguration GetRuntimeTypeConfiguration(Type type)
        {
            var bootstrapper = this.bootstrapperProvider.GetBootstrapper(type);

            // TODO (Cameron): This should be an injected configuration collection.
            var configuration = new BootstrapperConfiguration();

            bootstrapper.Invoke(configuration);

            var typeConfiguration = this.GetTypeConfiguration(type);
            var baseTypeConfiguration = type.BaseType == typeof(Entity) ? new EntityConfiguration() : this.GetConfiguration(type.BaseType);

            var config = EntityConfiguration.Merge(typeConfiguration, baseTypeConfiguration);
            config.RuntimeType = type;

            return config;
        }

        private EntityConfiguration GetTypeConfiguration(Type type)
        {
            var bootstrapper = this.bootstrapperProvider.GetBootstrapper(type);

            // TODO (Cameron): This should be an injected configuration collection.
            var configuration = new BootstrapperConfiguration();

            bootstrapper.Invoke(configuration);

            var bootstrapperConfiguration = configuration.GetEntityConfiguration(type);
            var typeAnalyzerConfiguration = this.typeAnalyzer.GetConfiguration(type);

            return EntityConfiguration.Combine(bootstrapperConfiguration, typeAnalyzerConfiguration);
        }
    }
}
