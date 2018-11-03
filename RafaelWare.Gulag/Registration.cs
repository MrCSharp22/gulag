using System;

namespace RafaelWare.Gulag
{
    public class Registration
    {
        /// <summary>
        /// Gets or sets the type of the service to register. This is the the type that users will request to resolve.
        /// </summary>
        /// <value>
        /// The type of the service.
        /// </value>
        public Type ServiceType { get; set; }

        /// <summary>
        /// Gets or sets the type of the target to initialize when the <see cref="ServiceType"/> is resolved.
        /// </summary>
        /// <value>
        /// The type of the target.
        /// </value>
        public Type TargetType { get; set; }

        /// <summary>
        /// Gets or sets the custom initialization action for the <see cref="TargetType"/>.
        /// </summary>
        /// <value>
        /// The initialization action.
        /// </value>
        public Func<object> BuilderFunc { get; set; }

        /// <summary>
        /// Gets or sets the life span of the service registered with this instance.
        /// </summary>
        /// <value>
        /// The life span.
        /// </value>
        public LifeSpan LifeSpan { get; set; }

        /// <summary>
        /// Gets or sets the cached instance.
        /// </summary>
        /// <value>
        /// The cached instance.
        /// </value>
        public object CachedInstance { get; set; }

        /// <summary>
        /// Gets or sets the scope object function.
        /// </summary>
        /// <value>
        /// The scope object function.
        /// </value>
        public Func<Registration, object> ScopeObjectProviderFunc { get; set; }

        /// <summary>
        /// Gets or sets the scope object.
        /// </summary>
        /// <value>
        /// The scope object.
        /// </value>
        public object ScopeObject { get; set; }

        /// <summary>
        /// Gets or sets the metadata.
        /// </summary>
        /// <value>
        /// The metadata.
        /// </value>
        public object Metadata { get; set; }

        /// <summary>
        /// Gets or sets the provider.
        /// </summary>
        /// <value>
        /// The provider.
        /// </value>
        public IProvider Provider { get; set; }
    }
}
