using System;
using System.Collections.Generic;
using Microsoft.Extensions.Options;
using MultiTenancyServer.Models;
using MultiTenancyServer.Services;

namespace MultiTenancyServer.Options
{
    /// <summary>
    /// Create a new options instance with configuration applied
    /// </summary>
    /// <typeparam name="TOptions">The Options.</typeparam>
    /// <typeparam name="TTenant">The type representing a tenant.</typeparam>
    /// <typeparam name="TKey">The type of the primary key for a tenant.</typeparam>
    public class TenantOptionsFactory<TOptions, TTenant, TKey> : IOptionsFactory<TOptions>
        where TOptions : class, new()
        where TTenant : class, ITenanted<TKey>
        where TKey : IEquatable<TKey>
    {

        private readonly IEnumerable<IConfigureOptions<TOptions>> setups;
        private readonly IEnumerable<IPostConfigureOptions<TOptions>> postConfigures;
        private readonly Action<TOptions, TTenant> tenantConfig;
        private readonly ITenancyContextAccessor<TTenant, TKey> tenantAccessor;

        /// <summary>
        /// Initializes a new instance of the <see cref="TenantOptionsFactory{TOptions,TTenant,TKey}"/> class.
        /// </summary>
        /// <param name="setups">IEnumerable of <see cref="IConfigureOptions{TOptions}"/></param>
        /// <param name="postConfigures">IEnumerable of <see cref="IPostConfigureOptions{TOptions}"/></param>
        /// <param name="tenantConfig">The action <see cref="Action{TOptions, TTenant}"/></param>
        /// <param name="tenantAccessor">The tenancycontext accessor.</param>
        public TenantOptionsFactory(
            IEnumerable<IConfigureOptions<TOptions>> setups,
            IEnumerable<IPostConfigureOptions<TOptions>> postConfigures, 
            Action<TOptions, TTenant> tenantConfig,
            ITenancyContextAccessor<TTenant, TKey> tenantAccessor)
        {
            this.setups = setups;
            this.postConfigures = postConfigures;
            this.tenantAccessor = tenantAccessor;
            this.tenantConfig = tenantConfig;
        }

        /// <summary>
        /// Create a new options instance
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>The options created.</returns>
        public TOptions Create(string name)
        {
            var options = new TOptions();

            //Apply options setup configuration
            foreach (var setup in setups)
            {
                if (setup is IConfigureNamedOptions<TOptions> namedSetup)
                {
                    namedSetup.Configure(name, options);
                }
                else
                {
                    setup.Configure(options);
                }
            }

            //Apply tenant specifc configuration (to both named and non-named options)
            if (tenantAccessor.TenancyContext?.Tenant != null)
                tenantConfig(options, tenantAccessor.TenancyContext.Tenant);

            //Apply post configuration
            foreach (var postConfig in postConfigures)
            {
                postConfig.PostConfigure(name, options);
            }

            return options;
        }
    }
}