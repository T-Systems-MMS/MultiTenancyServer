using System;
using System.Collections.Generic;
using Microsoft.Extensions.Options;
using MultiTenancyServer.Models;

namespace MultiTenancyServer.Options
{
    /// <summary>
    /// Create a new options instance with configuration applied
    /// </summary>
    /// <typeparam name="TOptions"></typeparam>
    /// <typeparam name="TTenant"></typeparam>
    /// <typeparam name="TKey"></typeparam>
    internal class TenantOptionsFactory<TOptions, TTenant, TKey> : IOptionsFactory<TOptions>
        where TOptions : class, new()
        where TTenant : ITenanted<TKey>
        where TKey : IEquatable<TKey>
    {

        private readonly IEnumerable<IConfigureOptions<TOptions>> setups;
        private readonly IEnumerable<IPostConfigureOptions<TOptions>> postConfigures;
        private readonly Action<TOptions, TTenant> tenantConfig;
        private readonly ITenancyContext<TTenant, TKey> tenantAccessor;

        public TenantOptionsFactory(
            IEnumerable<IConfigureOptions<TOptions>> setups,
            IEnumerable<IPostConfigureOptions<TOptions>> postConfigures, Action<TOptions, TTenant> tenantConfig,
            ITenancyContext<TTenant, TKey> tenantAccessor)
        {
            this.setups = setups;
            this.postConfigures = postConfigures;
            this.tenantAccessor = tenantAccessor;
            this.tenantConfig = tenantConfig;
        }

        /// <summary>
        /// Create a new options instance
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
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
            if (tenantAccessor.Tenant != null)
                tenantConfig(options, tenantAccessor.Tenant);

            //Apply post configuration
            foreach (var postConfig in postConfigures)
            {
                postConfig.PostConfigure(name, options);
            }

            return options;
        }
    }
}