// Copyright (c) Kris Penner. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KodeAid;
using MultiTenancyServer.Models;

namespace MultiTenancyServer.Services
{
    /// <summary>
    /// Provides validation services for tenant classes.
    /// </summary>
    /// <typeparam name="TTenant">The type encapsulating a tenant.</typeparam>
    /// <typeparam name="TKey">The type of the primary key for a tenant.</typeparam>
    public class TenantValidator<TTenant, TKey> : ITenantValidator<TTenant, TKey>
        where TTenant : ITenanted<TKey>
        where TKey : IEquatable<TKey>
    {
        /// <summary>
        /// Creates a new instance of <see cref="TenantValidator{TTenant, TKey}"/>/
        /// </summary>
        /// <param name="errors">The <see cref="TenancyErrorDescriber"/> used to provider error messages.</param>
        public TenantValidator(TenancyErrorDescriber errors = null)
        {
            Describer = errors ?? new TenancyErrorDescriber();
        }

        /// <summary>
        /// Gets the <see cref="TenancyErrorDescriber"/> used to provider error messages for the current <see cref="TenantValidator{TTenant, TKey}"/>.
        /// </summary>
        /// <value>The <see cref="TenancyErrorDescriber"/> used to provider error messages for the current <see cref="TenantValidator{TTenant, TKey}"/>.</value>
        public TenancyErrorDescriber Describer { get; private set; }

        /// <summary>
        /// Validates the specified <paramref name="tenant"/> as an asynchronous operation.
        /// </summary>
        /// <param name="manager">The <see cref="TenantManager{TTenant, TKey}"/> that can be used to retrieve tenant properties.</param>
        /// <param name="tenant">The tenant to validate.</param>
        /// <returns>The <see cref="Task"/> that represents the asynchronous operation, containing the <see cref="TenancyResult"/> of the validation operation.</returns>
        public virtual async Task<TenancyResult> ValidateAsync(TenantManager<TTenant, TKey> manager, TTenant tenant)
        {
            ArgCheck.NotNull(nameof(manager), manager);
            ArgCheck.NotNull(nameof(tenant), tenant);
            var errors = new List<TenancyError>();
            await ValidateCanonicalName(manager, tenant, errors);
            return errors.Count > 0 ? TenancyResult.Failed(errors.ToArray()) : TenancyResult.Success;
        }

        private async Task ValidateCanonicalName(TenantManager<TTenant, TKey> manager, TTenant tenant, ICollection<TenancyError> errors)
        {
            var canonicalName = await manager.GetCanonicalNameAsync(tenant);
            if (string.IsNullOrWhiteSpace(canonicalName))
            {
                errors.Add(Describer.InvalidCanonicalName(canonicalName));
            }
            else if (!string.IsNullOrEmpty(manager.Options.Tenant.AllowedCanonicalNameCharacters) &&
                canonicalName.Any(c => !manager.Options.Tenant.AllowedCanonicalNameCharacters.Contains(c)))
            {
                errors.Add(Describer.InvalidCanonicalName(canonicalName));
            }
            else
            {
                var owner = await manager.FindByCanonicalNameAsync(canonicalName);
                if (owner != null &&
                    !string.Equals(await manager.GetTenantIdAsync(owner), await manager.GetTenantIdAsync(tenant)))
                {
                    errors.Add(Describer.DuplicateCanonicalName(canonicalName));
                }
            }
        }
    }
}
