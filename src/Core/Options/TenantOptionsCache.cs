using MultiTenancyServer.Services;

namespace MultiTenancyServer.Options
{
    using System;
    using Microsoft.Extensions.Options;
    using Models;

    /// <summary>
    /// Tenant aware options cache
    /// </summary>
    /// <typeparam name="TOptions">The options.</typeparam>
    /// <typeparam name="TTenant">The type representing a tenant.</typeparam>
    /// <typeparam name="TKey">The type of the primary key for a tenant.</typeparam>
    public class TenantOptionsCache<TOptions, TTenant, TKey> : IOptionsMonitorCache<TOptions>
        where TOptions : class
        where TTenant : class, ITenanted<TKey>
        where TKey : IEquatable<TKey>
    {
        private readonly ITenancyContextAccessor<TTenant, TKey> _tenantAccessor;
        private readonly TenantOptionsCacheDictionary<TOptions, TKey> _tenantSpecificOptionsCache = new TenantOptionsCacheDictionary<TOptions, TKey>();

        /// <summary>
        /// Initializes a new instance of the <see cref="TenantOptionsCache{TOptions,TTenant,TKey}"/> class.
        /// </summary>
        /// <param name="tenantAccessor">The accessor for the tenantcontext.</param>
        public TenantOptionsCache(ITenancyContextAccessor<TTenant, TKey> tenantAccessor)
        {
            _tenantAccessor = tenantAccessor;
        }

        /// <inheritdoc />
        public void Clear()
        {
            _tenantSpecificOptionsCache.Get(_tenantAccessor.GetTenantIdOrDefault()).Clear();
        }

        /// <inheritdoc />
        public TOptions GetOrAdd(string name, Func<TOptions> createOptions)
        {
            return _tenantSpecificOptionsCache.Get(_tenantAccessor.GetTenantIdOrDefault()).GetOrAdd(name, createOptions);
        }

        /// <inheritdoc />
        public bool TryAdd(string name, TOptions options)
        {
            return _tenantSpecificOptionsCache.Get(_tenantAccessor.GetTenantIdOrDefault()).TryAdd(name, options);
        }

        /// <inheritdoc />
        public bool TryRemove(string name)
        {
            return _tenantSpecificOptionsCache.Get(_tenantAccessor.GetTenantIdOrDefault()).TryRemove(name);
        }
    }
}
