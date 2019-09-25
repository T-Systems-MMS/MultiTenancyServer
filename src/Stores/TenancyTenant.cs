﻿// Copyright (c) Kris Penner. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System;
using MultiTenancyServer.Models;

namespace MultiTenancyServer
{
    /// <summary>
    /// The default implementation of <see cref="TenancyTenant{TKey}"/> which uses a string as a primary key.
    /// </summary>
    public class TenancyTenant : TenancyTenant<string>
    {
        /// <summary>
        /// Initializes a new instance of <see cref="TenancyTenant"/>.
        /// </summary>
        /// <remarks>
        /// The Id property is initialized to form a new GUID string value.
        /// </remarks>
        public TenancyTenant() : base(Guid.NewGuid().ToString()) { }

        /// <summary>
        /// Initializes a new instance of <see cref="TenancyTenant"/>.
        /// </summary>
        /// <param name="canonicalName">The canonical name.</param>
        /// <remarks>
        /// The Id property is initialized to form a new GUID string value.
        /// </remarks>
        public TenancyTenant(string canonicalName) : base(Guid.NewGuid().ToString(), canonicalName)
        {
        }
    }

    /// <summary>
    /// Represents a tenant in the multi-tenancy system.
    /// </summary>
    /// <typeparam name="TKey">The type used for the primary key for the tenant.</typeparam>
    public class TenancyTenant<TKey> : ITenanted<TKey> where TKey : IEquatable<TKey>
    {
        /// <summary>
        /// Initializes a new instance of <see cref="TenancyTenant{TKey}"/>.
        /// </summary>
        /// <param name="id">Id of Tenanat</param>
        public TenancyTenant(TKey id)
        {
            this.Id = id;
        }

        /// <summary>
        /// Initializes a new instance of <see cref="TenancyTenant{TKey}"/>.
        /// </summary>
        /// <param name="id">Id of Tenanat</param>
        /// <param name="canonicalName">The canonical name.</param>
        public TenancyTenant(TKey id, string canonicalName) : this(id)
        {
            this.CanonicalName = canonicalName;
        }

        /// <summary>
        /// Gets or sets the canonical name for this tenant.
        /// </summary>
        public virtual string CanonicalName { get; set; }

        /// <summary>
        /// Gets or sets the normalized canonical name for this tenant.
        /// </summary>
        public virtual string NormalizedCanonicalName { get; set; }

        /// <summary>
        /// A random value that must change whenever a tenant is persisted to the store.
        /// </summary>
        public virtual string ConcurrencyStamp { get; set; } = Guid.NewGuid().ToString();

        /// <summary>
        /// Returns the canonical name for this tenant.
        /// </summary>
        public override string ToString()
            => CanonicalName;

        /// <inheritdoc cref="ITenanted{TKey}"/>
        public TKey Id { get; set; }
    }
}
