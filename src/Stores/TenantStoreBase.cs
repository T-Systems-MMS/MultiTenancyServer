﻿// Copyright (c) Kris Penner. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using KodeAid;
using Microsoft.Extensions.Logging;

namespace MultiTenancyServer.Stores
{
    /// <summary>
    /// Represents a new instance of a persistence store for the specified tenant type.
    /// </summary>
    /// <typeparam name="TTenant">The type representing a tenant.</typeparam>
    /// <typeparam name="TKey">The type of the primary key for a tenant.</typeparam>
    public abstract class TenantStoreBase<TTenant, TKey> :
        IQueryableTenantStore<TTenant, TKey>
        where TTenant : TenancyTenant<TKey>
        where TKey : IEquatable<TKey>
    {
        /// <summary>
        /// Creates a new instance.
        /// </summary>
        /// <param name="describer">The <see cref="TenancyErrorDescriber"/> used to describe store errors.</param>
        /// <param name="logger">The logger.</param>
        protected TenantStoreBase(TenancyErrorDescriber describer, ILogger logger)
        {
            ArgCheck.NotNull(nameof(describer), describer);
            ArgCheck.NotNull(nameof(logger), logger);
            ErrorDescriber = describer;
            Logger = logger;
        }

        private bool _disposed;

        /// <summary>
        /// A navigation property for the tenants the store contains.
        /// </summary>
        public abstract IQueryable<TTenant> Tenants
        {
            get;
        }

        /// <summary>
        /// Gets or sets the <see cref="TenancyErrorDescriber"/> for any error that occurred with the current operation.
        /// </summary>
        public TenancyErrorDescriber ErrorDescriber { get; set; }

        /// <summary>
        /// The <see cref="ILogger"/> used to log messages from the store.
        /// </summary>
        /// <value>
        /// The <see cref="ILogger"/> used to log messages from the store.
        /// </value>
        public ILogger Logger { get; set; }

        /// <summary>
        /// Gets the tenant identifier for the specified <paramref name="tenant"/>.
        /// </summary>
        /// <param name="tenant">The tenant whose identifier should be retrieved.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>The <see cref="Task"/> that represents the asynchronous operation, containing the identifier for the specified <paramref name="tenant"/>.</returns>
        public virtual Task<string> GetTenantIdAsync(TTenant tenant, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (tenant == null)
            {
                throw new ArgumentNullException(nameof(tenant));
            }
            return Task.FromResult(ConvertIdToString(tenant.TenantId));
        }

        /// <summary>
        /// Gets the canonical name for the specified <paramref name="tenant"/>.
        /// </summary>
        /// <param name="tenant">The tenant whose canonical name should be retrieved.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>The <see cref="Task"/> that represents the asynchronous operation, containing the canonical name for the specified <paramref name="tenant"/>.</returns>
        public virtual Task<string> GetCanonicalNameAsync(TTenant tenant, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (tenant == null)
            {
                throw new ArgumentNullException(nameof(tenant));
            }
            return Task.FromResult(tenant.CanonicalName);
        }

        /// <summary>
        /// Sets the given <paramref name="canonicalName" /> for the specified <paramref name="tenant"/>.
        /// </summary>
        /// <param name="tenant">The tenant whose canonical name should be set.</param>
        /// <param name="canonicalName">The tenant canonical name to set.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>The <see cref="Task"/> that represents the asynchronous operation.</returns>
        public virtual Task SetCanonicalNameAsync(TTenant tenant, string canonicalName, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (tenant == null)
            {
                throw new ArgumentNullException(nameof(tenant));
            }
            tenant.CanonicalName = canonicalName;
            return Task.CompletedTask;
        }

        /// <summary>
        /// Gets the normalized canonical name for the specified <paramref name="tenant"/>.
        /// </summary>
        /// <param name="tenant">The tenant whose normalized canonical name should be retrieved.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>The <see cref="Task"/> that represents the asynchronous operation, containing the normalized canonical name for the specified <paramref name="tenant"/>.</returns>
        public virtual Task<string> GetNormalizedCanonicalNameAsync(TTenant tenant, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (tenant == null)
            {
                throw new ArgumentNullException(nameof(tenant));
            }
            return Task.FromResult(tenant.NormalizedCanonicalName);
        }

        /// <summary>
        /// Sets the given normalized canonical name for the specified <paramref name="tenant"/>.
        /// </summary>
        /// <param name="tenant">The tenant whose canonical name should be set.</param>
        /// <param name="normalizedCanonicalName">The normalized canonical name to set.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>The <see cref="Task"/> that represents the asynchronous operation.</returns>
        public virtual Task SetNormalizedCanonicalNameAsync(TTenant tenant, string normalizedCanonicalName, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (tenant == null)
            {
                throw new ArgumentNullException(nameof(tenant));
            }
            tenant.NormalizedCanonicalName = normalizedCanonicalName;
            return Task.CompletedTask;
        }

        /// <summary>
        /// Creates the specified <paramref name="tenant"/> in the tenant store.
        /// </summary>
        /// <param name="tenant">The tenant to create.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>The <see cref="Task"/> that represents the asynchronous operation, containing the <see cref="TenancyResult"/> of the creation operation.</returns>
        public abstract Task<TenancyResult> CreateAsync(TTenant tenant, CancellationToken cancellationToken = default);

        /// <summary>
        /// Updates the specified <paramref name="tenant"/> in the tenant store.
        /// </summary>
        /// <param name="tenant">The tenant to update.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>The <see cref="Task"/> that represents the asynchronous operation, containing the <see cref="TenancyResult"/> of the update operation.</returns>
        public abstract Task<TenancyResult> UpdateAsync(TTenant tenant, CancellationToken cancellationToken = default);

        /// <summary>
        /// Deletes the specified <paramref name="tenant"/> from the tenant store.
        /// </summary>
        /// <param name="tenant">The tenant to delete.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>The <see cref="Task"/> that represents the asynchronous operation, containing the <see cref="TenancyResult"/> of the update operation.</returns>
        public abstract Task<TenancyResult> DeleteAsync(TTenant tenant, CancellationToken cancellationToken = default);

        /// <summary>
        /// Finds and returns a tenant, if any, who has the specified <paramref name="tenantId"/>.
        /// </summary>
        /// <param name="tenantId">The tenant ID to search for.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>
        /// The <see cref="Task"/> that represents the asynchronous operation, containing the tenant matching the specified <paramref name="tenantId"/> if it exists.
        /// </returns>
        public abstract Task<TTenant> FindByIdAsync(string tenantId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Finds and returns a tenant, if any, who has the specified normalized canonical name.
        /// </summary>
        /// <param name="normalizedCanonicalName">The normalized canonical name to search for.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>
        /// The <see cref="Task"/> that represents the asynchronous operation, containing the tenant matching the specified <paramref name="normalizedCanonicalName"/> if it exists.
        /// </returns>
        public abstract Task<TTenant> FindByCanonicalNameAsync(string normalizedCanonicalName, CancellationToken cancellationToken = default);

        /// <summary>
        /// Converts the provided <paramref name="id"/> to a strongly typed key object.
        /// </summary>
        /// <param name="id">The id to convert.</param>
        /// <returns>An instance of <typeparamref name="TKey"/> representing the provided <paramref name="id"/>.</returns>
        public virtual TKey ConvertIdFromString(string id)
        {
            if (id == null)
            {
                return default;
            }
            return (TKey)TypeDescriptor.GetConverter(typeof(TKey)).ConvertFromInvariantString(id);
        }

        /// <summary>
        /// Converts the provided <paramref name="id"/> to its string representation.
        /// </summary>
        /// <param name="id">The id to convert.</param>
        /// <returns>An <see cref="string"/> representation of the provided <paramref name="id"/>.</returns>
        public virtual string ConvertIdToString(TKey id)
        {
            if (Equals(id, default(TKey)))
            {
                return null;
            }
            return id.ToString();
        }

        /// <summary>
        /// Return a tenant with the matching tenantId if it exists.
        /// </summary>
        /// <param name="tenantId">The tenant's id.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>The tenant if it exists.</returns>
        protected abstract Task<TTenant> FindTenantAsync(TKey tenantId, CancellationToken cancellationToken);

        /// <summary>
        /// Throws if this class has been disposed.
        /// </summary>
        protected void ThrowIfDisposed()
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(GetType().Name);
            }
        }

        /// <summary>
        /// Dispose the store
        /// </summary>
        public void Dispose()
        {
            _disposed = true;
        }
    }
}
