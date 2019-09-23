namespace MultiTenancyServer.Options
{
    using System;
    using Microsoft.Extensions.Options;
    using Models;

    /// <summary>
    /// Tenant aware options cache
    /// </summary>
    /// <typeparam name="TOptions"></typeparam>
    /// <typeparam name="TTenant"></typeparam>
    /// <typeparam name="TKey"></typeparam>
    public class TenantOptionsCache<TOptions, TTenant, TKey> : IOptionsMonitorCache<TOptions>
        where TOptions : class
        where TTenant : ITenanted<TKey>
        where TKey : IEquatable<TKey>
    {

        private readonly ITenancyContext<TTenant, TKey> _tenantAccessor;

        private readonly TenantOptionsCacheDictionary<TOptions, TKey> _tenantSpecificOptionsCache =
            new TenantOptionsCacheDictionary<TOptions, TKey>();

        public TenantOptionsCache(ITenancyContext<TTenant, TKey> tenantAccessor)
        {
            _tenantAccessor = tenantAccessor;
        }

        public void Clear()
        {
            _tenantSpecificOptionsCache.Get(_tenantAccessor.Tenant.TenantId).Clear();
        }

        public TOptions GetOrAdd(string name, Func<TOptions> createOptions)
        {
            return _tenantSpecificOptionsCache.Get(_tenantAccessor.Tenant.TenantId)
                .GetOrAdd(name, createOptions);
        }

        public bool TryAdd(string name, TOptions options)
        {
            return _tenantSpecificOptionsCache.Get(_tenantAccessor.Tenant.TenantId)
                .TryAdd(name, options);
        }

        public bool TryRemove(string name)
        {
            return _tenantSpecificOptionsCache.Get(_tenantAccessor.Tenant.TenantId)
                .TryRemove(name);
        }
    }
}
