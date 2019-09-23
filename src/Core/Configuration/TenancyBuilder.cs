// Copyright (c) Kris Penner. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using MultiTenancyServer.Models;
using MultiTenancyServer.Options;
using MultiTenancyServer.Services;
using MultiTenancyServer.Stores;

namespace MultiTenancyServer.Configuration.DependencyInjection
{
    /// <summary>
    /// Helper functions for configuring multi-tenancy services.
    /// </summary>
    /// <typeparam name="TTenant">The type representing a tenant.</typeparam>
    /// <typeparam name="TKey">The type of the primary key for a tenant.</typeparam>
    public class TenancyBuilder<TTenant, TKey>
        where TTenant : ITenanted<TKey>
        where TKey : IEquatable<TKey>
    {
        /// <summary>
        /// Creates a new instance of <see cref="TenancyBuilder{TTenant, TKey}"/>.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/> to attach to.</param>
        public TenancyBuilder(IServiceCollection services)
        {
            Services = services;
        }

        /// <summary>
        /// Gets the <see cref="IServiceCollection"/> services are attached to.
        /// </summary>
        /// <value>
        /// The <see cref="IServiceCollection"/> services are attached to.
        /// </value>
        public IServiceCollection Services { get; private set; }

        /// <summary>
        /// Adds an <see cref="ITenantValidator{TTenant, TKey}"/> for the <seealso cref="ITenanted{TKey}"/>.
        /// </summary>
        /// <typeparam name="TValidator">The tenant validator type.</typeparam>
        /// <returns>The current <see cref="TenancyBuilder{TTenant, TKey}"/> instance.</returns>
        public virtual TenancyBuilder<TTenant, TKey> AddTenantValidator<TValidator>()
            where TValidator : class, ITenantValidator<TTenant, TKey>
        {
            Services.AddScoped<ITenantValidator<TTenant, TKey>, TValidator>();
            return this;
        }

        /// <summary>
        /// Adds an <see cref="ITenantValidator{TTenant, TKey}"/> for the <seealso cref="ITenanted{TKey}"/>.
        /// </summary>
        /// <typeparam name="TValidator">The tenant validator type.</typeparam>
        /// <returns>The current <see cref="TenancyBuilder{TTenant, TKey}"/> instance.</returns>
        public virtual TenancyBuilder<TTenant, TKey> AddTenantValidator<TValidator>(Func<IServiceProvider, TValidator> validatorFactory)
            where TValidator : class, ITenantValidator<TTenant, TKey>
        {
            Services.AddScoped<ITenantValidator<TTenant, TKey>, TValidator>(validatorFactory);
            return this;
        }

        /// <summary>
        /// Adds an <see cref="TenancyErrorDescriber"/>.
        /// </summary>
        /// <typeparam name="TDescriber">The type of the error describer.</typeparam>
        /// <returns>The current <see cref="TenancyBuilder{TTenant, TKey}"/> instance.</returns>
        public virtual TenancyBuilder<TTenant, TKey> AddErrorDescriber<TDescriber>()
            where TDescriber : TenancyErrorDescriber
        {
            Services.AddScoped<TenancyErrorDescriber, TDescriber>();
            return this;
        }

        /// <summary>
        /// Adds an <see cref="TenancyErrorDescriber"/>.
        /// </summary>
        /// <typeparam name="TDescriber">The type of the error describer.</typeparam>
        /// <returns>The current <see cref="TenancyBuilder{TTenant, TKey}"/> instance.</returns>
        public virtual TenancyBuilder<TTenant, TKey> AddErrorDescriber<TDescriber>(Func<IServiceProvider, TDescriber> describerFactory)
            where TDescriber : TenancyErrorDescriber
        {
            Services.AddScoped<TenancyErrorDescriber, TDescriber>(describerFactory);
            return this;
        }

        /// <summary>
        /// Adds an <see cref="ITenantStore{TTenant, TKey}"/> for the <seealso cref="ITenanted{TKey}"/>.
        /// </summary>
        /// <typeparam name="TStore">The tenant store type.</typeparam>
        /// <returns>The current <see cref="TenancyBuilder{TTenant, TKey}"/> instance.</returns>
        public virtual TenancyBuilder<TTenant, TKey> AddTenantStore<TStore>()
            where TStore : class, ITenantStore<TTenant, TKey>
        {
            Services.AddScoped<ITenantStore<TTenant, TKey>, TStore>();
            return this;
        }

        /// <summary>
        /// Adds an <see cref="ITenantStore{TTenant, TKey}"/> for the <seealso cref="ITenanted{TKey}"/>.
        /// </summary>
        /// <typeparam name="TStore">The tenant store type.</typeparam>
        /// <returns>The current <see cref="TenancyBuilder{TTenant, TKey}"/> instance.</returns>
        public virtual TenancyBuilder<TTenant, TKey> AddTenantStore<TStore>(Func<IServiceProvider, TStore> storeFactory)
            where TStore : class, ITenantStore<TTenant, TKey>
        {
            Services.AddScoped<ITenantStore<TTenant, TKey>, TStore>(storeFactory);
            return this;
        }

        /// <summary>
        /// Adds a <see cref="TenantManager{TTenant, TKey}"/> for the <seealso cref="ITenanted{TKey}"/>.
        /// </summary>
        /// <typeparam name="TManager">The type of the tenant manager to add.</typeparam>
        /// <returns>The current <see cref="TenancyBuilder{TTenant, TKey}"/> instance.</returns>
        public virtual TenancyBuilder<TTenant, TKey> AddTenantManager<TManager>()
            where TManager : TenantManager<TTenant, TKey>
        {
            Services.AddScoped<TenantManager<TTenant, TKey>, TManager>();
            return this;
        }

        /// <summary>
        /// Adds a <see cref="TenantManager{TTenant, TKey}"/> for the <seealso cref="ITenanted{TKey}"/>.
        /// </summary>
        /// <typeparam name="TManager">The type of the tenant manager to add.</typeparam>
        /// <returns>The current <see cref="TenancyBuilder{TTenant, TKey}"/> instance.</returns>
        public virtual TenancyBuilder<TTenant, TKey> AddTenantManager<TManager>(Func<IServiceProvider, TManager> managerFactory)
            where TManager : TenantManager<TTenant, TKey>
        {
            Services.AddScoped<TenantManager<TTenant, TKey>, TManager>(managerFactory);
            return this;
        }

        /// <summary>
        /// Adds a <see cref="ITenancyProvider{TTenant, TKey}"/> for the <seealso cref="ITenanted{TKey}"/>.
        /// </summary>
        /// <typeparam name="TProvider">The type of the tenancy provider to add.</typeparam>
        /// <returns>The current <see cref="TenancyBuilder{TTenant, TKey}"/> instance.</returns>
        public virtual TenancyBuilder<TTenant, TKey> AddTenancyProvider<TProvider>()
            where TProvider : class, ITenancyProvider<TTenant, TKey>
        {
            Services.AddScoped<ITenancyProvider<TTenant, TKey>, TProvider>();
            return this;
        }

        /// <summary>
        /// Adds a <see cref="ITenancyProvider{TTenant, TKey}"/> for the <seealso cref="ITenanted{TKey}"/>.
        /// </summary>
        /// <typeparam name="TProvider">The type of the tenancy provider to add.</typeparam>
        /// <returns>The current <see cref="TenancyBuilder{TTenant, TKey}"/> instance.</returns>
        public virtual TenancyBuilder<TTenant, TKey> AddTenancyProvider<TProvider>(Func<IServiceProvider, TProvider> providerFactory)
            where TProvider : class, ITenancyProvider<TTenant, TKey>
        {
            Services.AddScoped<ITenancyProvider<TTenant, TKey>, TProvider>(providerFactory);
            return this;
        }

        /// <summary>
        /// Adds a <see cref="ITenancyContext{TTenant, TKey}"/> for the <seealso cref="ITenanted{TKey}"/>.
        /// </summary>
        /// <typeparam name="TContext">The type of the tenancy context to add.</typeparam>
        /// <returns>The current <see cref="TenancyBuilder{TTenant, TKey}"/> instance.</returns>
        public virtual TenancyBuilder<TTenant, TKey> AddTenancyContext<TContext>()
            where TContext : class, ITenancyContext<TTenant, TKey>
        {
            Services.AddScoped<ITenancyContext<TTenant, TKey>, TContext>();
            return this;
        }

        /// <summary>
        /// Adds a <see cref="ITenancyContext{TTenant, TKey}"/> for the <seealso cref="ITenanted{TKey}"/>.
        /// </summary>
        /// <typeparam name="TContext">The type of the tenancy context to add.</typeparam>
        /// <returns>The current <see cref="TenancyBuilder{TTenant, TKey}"/> instance.</returns>
        public virtual TenancyBuilder<TTenant, TKey> AddTenancyContext<TContext>(Func<IServiceProvider, TContext> contextFactory)
            where TContext : class, ITenancyContext<TTenant, TKey>
        {
            Services.AddScoped<ITenancyContext<TTenant, TKey>, TContext>(contextFactory);
            return this;
        }

        /// <summary>
        /// Register tenant specific options
        /// </summary>
        /// <typeparam name="TOptions">Type of options we are apply configuration to</typeparam>
        /// <param name="tenantConfig">Action to configure options for a tenant</param>
        /// <returns></returns>
        public virtual TenancyBuilder<TTenant, TKey> WithPerTenantOptions<TOptions>(Action<TOptions, TTenant> tenantConfig) where TOptions : class, new()
        {
            //Register the multi-tenant cache
            Services.AddSingleton<IOptionsMonitorCache<TOptions>>(a => ActivatorUtilities.CreateInstance<TenantOptionsCache<TOptions, TTenant, TKey>>(a));

            //Register the multi-tenant options factory
            Services.AddTransient<IOptionsFactory<TOptions>>(a => ActivatorUtilities.CreateInstance<TenantOptionsFactory<TOptions, TTenant, TKey>>(a, tenantConfig));

            //Register IOptionsSnapshot support
            Services.AddScoped<IOptionsSnapshot<TOptions>>(a => ActivatorUtilities.CreateInstance<TenantOptions<TOptions>>(a));

            //Register IOptions support
            Services.AddSingleton<IOptions<TOptions>>(a => ActivatorUtilities.CreateInstance<TenantOptions<TOptions>>(a));

            return this;
        }
    }
}
