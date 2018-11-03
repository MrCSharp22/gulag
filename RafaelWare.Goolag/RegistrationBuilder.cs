using System;

namespace RafaelWare.Goolag
{
    public class RegistrationBuilder
    {
        /// <summary>
        /// The container
        /// </summary>
        private readonly IoC container;

        /// <summary>
        /// The registration information
        /// </summary>
        private readonly Registration registration;

        /// <summary>
        /// Initializes a new instance of the <see cref="RegistrationBuilder"/> class.
        /// </summary>
        /// <param name="container">The container.</param>
        /// <exception cref="ArgumentNullException">container</exception>
        public RegistrationBuilder(IoC container)
        {
            this.container = container
                ?? throw new ArgumentNullException(nameof(container));

            registration = new Registration();
        }

        /// <summary>
        /// Starts registering the specified service type. 
        /// If you got this <see cref="RegistrationBuilder"/> instance from the <see cref="IoC"/> instance then no need to call this.
        /// </summary>
        /// <param name="serviceType">Type of the service.</param>
        /// <returns></returns>
        public RegistrationBuilder For(Type serviceType)
        {
            if (registration.ServiceType != null)
                throw new InvalidOperationException(
                    $"A service type of: {registration.ServiceType} is already in registration with this instance of the builder");

            registration.ServiceType = serviceType;
            return this;
        }

        /// <summary>
        /// Starts registering the specified service type. 
        /// If you got this <see cref="RegistrationBuilder"/> instance from the <see cref="IoC"/> instance then no need to call this.
        /// </summary>
        /// <typeparam name="TService">The type of the service.</typeparam>
        /// <returns></returns>
        public RegistrationBuilder For<TService>()
        {
            return For(typeof(TService));
        }

        /// <summary>
        /// Specifies the life span of the service.
        /// </summary>
        /// <param name="lifeSpan">The life span.</param>
        /// <returns></returns>
        public RegistrationBuilder WithLifeSpanOf(LifeSpan lifeSpan = LifeSpan.Singleton)
        {
            registration.LifeSpan = lifeSpan;
            return this;
        }

        /// <summary>
        /// Registers this service with a life span of <see cref="LifeSpan.Singleton"/>
        /// </summary>
        /// <returns></returns>
        public RegistrationBuilder AsSingleton() => WithLifeSpanOf();

        /// <summary>
        /// Registers this service with a life span of <see cref="LifeSpan.Transient"/>
        /// </summary>
        /// <returns></returns>
        public RegistrationBuilder AsTransient() => WithLifeSpanOf(LifeSpan.Transient);

        /// <summary>
        /// Specifies the object that defines the life span of this service.
        /// </summary>
        /// <param name="scopeObjectProviderFunc">The scope function.</param>
        /// <returns></returns>
        public RegistrationBuilder InScope(Func<Registration, object> scopeObjectProviderFunc)
        {
            if (registration.ScopeObjectProviderFunc != null)
                throw new InvalidOperationException($"A scope object function is already registered with this instance");

            registration.ScopeObjectProviderFunc = scopeObjectProviderFunc;
            return WithLifeSpanOf(LifeSpan.Custom);
        }

        /// <summary>
        /// Sets the metadata to the specified object.
        /// </summary>
        /// <param name="metadata">The metadata.</param>
        /// <returns></returns>
        /// <exception cref="System.InvalidOperationException">Current registration already has a metadata object associated with it</exception>
        public RegistrationBuilder SetMetadataTo(object metadata)
        {
            if (registration.Metadata != null)
                throw new InvalidOperationException("Current registration already has a metadata object associated with it");

            registration.Metadata = metadata;
            return this;
        }

        /// <summary>
        /// Specifies the target type to initialize when this service is resolved.
        /// </summary>
        /// <param name="targetType">Type of the target.</param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public void Use(Type targetType)
        {
            if (registration.TargetType != null)
                throw new InvalidOperationException($"A target type of: {registration.TargetType} is already specified in this instance of the builder.");

            registration.TargetType = targetType;

            //check to see if there is a provider, if not, use the default one 
            if (registration.Provider == null)
                registration.Provider = new DefaultProvider(targetType, container);

            container.Register(registration);
        }

        /// <summary>
        /// Specifies the target type to initialize when this service is resolved.
        /// </summary>
        /// <typeparam name="TTarget">The type of the target.</typeparam>
        /// <returns></returns>
        public void Use<TTarget>()
        {
            Use(typeof(TTarget));
        }

        /// <summary>
        /// Builds the service with the specified builder function.
        /// </summary>
        /// <param name="builderFunc">The builder function.</param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public RegistrationBuilder Use(Func<object> builderFunc)
        {
            if (registration.BuilderFunc != null)
                throw new InvalidOperationException($"A builder function was already specified for this instance");

            registration.BuilderFunc = builderFunc;
            return this;
        }

        /// <summary>
        /// Uses the specified instance.
        /// </summary>
        /// <param name="instance">The instance.</param>
        /// <exception cref="InvalidOperationException">
        /// </exception>
        public void Use(object instance)
        {
            if (registration.CachedInstance != null)
                throw new InvalidOperationException($"A cached instance already exists in this instance.");

            if (registration.BuilderFunc == null &&
                (registration.LifeSpan == LifeSpan.Transient || registration.LifeSpan == LifeSpan.Custom))
                throw new InvalidOperationException($"No builder function was registered to create this type later");

            registration.CachedInstance = instance;
            Use(instance?.GetType());
        }

        /// <summary>
        /// Uses the provider.
        /// </summary>
        /// <param name="provider">The provider.</param>
        /// <returns></returns>
        public void UseProvider(IProvider provider)
        {
            if (registration.Provider != null)
                throw new InvalidOperationException("This instance already has a provider registered.");

            registration.Provider = provider;
            Use(provider.Type);
        }
    }
}
