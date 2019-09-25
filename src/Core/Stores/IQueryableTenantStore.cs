// Copyright (c) Kris Penner. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System;
using System.Linq;
using MultiTenancyServer.Models;

namespace MultiTenancyServer.Stores
{
    /// <summary>
    /// Provides an abstraction for querying tenants in a Tenant store.
    /// </summary>
    /// <typeparam name="TTenant">The type encapsulating a tenant.</typeparam>
    /// <typeparam name="TKey">The type of the primary key for a tenant.</typeparam>
    public interface IQueryableTenantStore<TTenant, TKey> : ITenantStore<TTenant, TKey>
        where TTenant : class, ITenanted<TKey>
        where TKey : IEquatable<TKey>
    {
        /// <summary>
        /// Returns an <see cref="IQueryable{T}"/> collection of tenants.
        /// </summary>
        /// <value>An <see cref="IQueryable{T}"/> collection of tenants.</value>
        IQueryable<TTenant> Tenants { get; }
    }
}
