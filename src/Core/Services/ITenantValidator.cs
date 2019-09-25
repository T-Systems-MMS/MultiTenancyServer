// Copyright (c) Kris Penner. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System;
using System.Threading.Tasks;
using MultiTenancyServer.Models;

namespace MultiTenancyServer.Services
{
    /// <summary>
    /// Provides an abstraction for tenant validation.
    /// </summary>
    /// <typeparam name="TTenant">The type encapsulating a tenant.</typeparam>
    /// <typeparam name="TKey">The type of the primary key for a tenant.</typeparam>
    public interface ITenantValidator<TTenant, TKey>
        where TTenant : class, ITenanted<TKey>
        where TKey : IEquatable<TKey>
    {
        /// <summary>
        /// Validates the specified <paramref name="tenant"/> as an asynchronous operation.
        /// </summary>
        /// <param name="manager">The <see cref="TenantManager{TTenant, TKey}"/> that can be used to retrieve tenant properties.</param>
        /// <param name="tenant">The tenant to validate.</param>
        /// <returns>The <see cref="Task"/> that represents the asynchronous operation, containing the <see cref="TenancyResult"/> of the validation operation.</returns>
        Task<TenancyResult> ValidateAsync(TenantManager<TTenant, TKey> manager, TTenant tenant);
    }
}
