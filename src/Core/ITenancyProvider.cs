// Copyright (c) Kris Penner. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using System;
using System.Threading;
using System.Threading.Tasks;
using MultiTenancyServer.Models;

namespace MultiTenancyServer
{
    /// <summary>
    /// Provides an abstraction for accessing the current tenant of the scoped process.
    /// </summary>
    /// <typeparam name="TTenant">The type encapsulating a tenant.</typeparam>
    /// <typeparam name="TKey">The primary key for Tenant.</typeparam>
    public interface ITenancyProvider<TTenant, TKey>
        where TTenant : class, ITenanted<TKey>
        where TKey : IEquatable<TKey>
    {
        /// <summary>
        /// Asynchronously gets the current tenant of the scoped process.
        /// </summary>
        /// <returns>A task that when completed will result in the current tenant of the scoped process.</returns>
        Task<TTenant> GetCurrentTenantAsync(CancellationToken cancellationToken = default);
    }
}
