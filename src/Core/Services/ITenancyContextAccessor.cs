using System;
using MultiTenancyServer.Models;

namespace MultiTenancyServer.Services
{
    /// <summary>
    /// Accessor for the current tenancycontext.
    /// </summary>
    /// <typeparam name="TTenant">The type encapsulating a tenant.</typeparam>
    /// <typeparam name="TKey">The type of the primary key for a tenant.</typeparam>
    public interface ITenancyContextAccessor<TTenant, TKey>
        where TTenant : class, ITenanted<TKey>
        where TKey : IEquatable<TKey>
    {
        /// <summary>
        /// Gets the current TenancyContext.
        /// </summary>
        ITenancyContext<TTenant, TKey> TenancyContext { get; }

        /// <summary>
        /// Gets the tenantid or the default value of TKey.
        /// </summary>
        /// <returns>Gets the tenantid or the default value of TKey.</returns>
        TKey GetTenantIdOrDefault();
    }
}
