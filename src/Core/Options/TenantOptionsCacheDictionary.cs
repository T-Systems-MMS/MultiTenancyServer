using System;
using System.Collections.Concurrent;
using Microsoft.Extensions.Options;

namespace MultiTenancyServer.Options
{
    /// <summary>
    /// Dictionary of tenant specific options caches
    /// </summary>
    /// <typeparam name="TOptions">The options.</typeparam>
    /// <typeparam name="TKey">The type of the primary key for the options.</typeparam>
    public class TenantOptionsCacheDictionary<TOptions, TKey>
        where TOptions : class
        where TKey : IEquatable<TKey>
    {
        /// <summary>
        /// Caches stored in memory
        /// </summary>
        private readonly ConcurrentDictionary<TKey, IOptionsMonitorCache<TOptions>> _tenantSpecificOptionCaches =
            new ConcurrentDictionary<TKey, IOptionsMonitorCache<TOptions>>();

        /// <summary>
        /// Get options for specific tenant (create if not exists)
        /// </summary>
        /// <param name="tenantId"></param>
        /// <returns>Options cache for the tenant.</returns>
        public IOptionsMonitorCache<TOptions> Get(TKey tenantId)
        {
            if (tenantId == null)
            {
                return new OptionsCache<TOptions>();
            }

            return _tenantSpecificOptionCaches.GetOrAdd(tenantId, new OptionsCache<TOptions>());
        }
    }
}