using System;
using System.Collections.Generic;
using System.Linq;

namespace RafaelWare.Goolag
{
    public sealed class IoC
    {
        /// <summary>
        /// Gets the registration requests made to this container.
        /// </summary>
        /// <value>
        /// The registration requests.
        /// </value>
        private readonly List<Registration> registrations;

        #region Singleton

        /// <summary>
        /// The initialized instance
        /// </summary>
        private static readonly IoC InitializedInstance = new IoC();

        /// <summary>
        /// Gets the current.
        /// </summary>
        /// <value>
        /// The current.
        /// </value>
        public static IoC Current => InitializedInstance;

        /// <summary>
        /// Prevents a default instance of the <see cref="IoC"/> class from being created.
        /// </summary>
        private IoC()
        {
            registrations = new List<Registration>();
        }

        /// <summary>
        /// Initializes the <see cref="IoC"/> class.
        /// </summary>
        static IoC()
        {
        }

        #endregion

        /// <summary>
        /// Registers the specified service type.
        /// </summary>
        /// <param name="serviceType">Type of the service.</param>
        /// <param name="targetType">Type of the target.</param>
        /// <param name="builderFunc">The builder function.</param>
        /// <param name="lifeSpan">The life span.</param>
        /// <param name="instance">The instance.</param>
        /// <param name="scopeObjectProviderFunc">The scope function.</param>
        /// <param name="scopeObject">The scope object.</param>
        /// <param name="metadata">The metadata.</param>
        /// <param name="provider">The provider.</param>
        public void Register(Type serviceType, Type targetType, Func<object> builderFunc = null,
            LifeSpan lifeSpan = LifeSpan.Singleton,
            object instance = null, Func<Registration, object> scopeObjectProviderFunc = null,
            object scopeObject = null, object metadata = null, IProvider provider = null)
        {
            Register(new Registration
            {
                ServiceType = serviceType,
                TargetType = targetType,
                BuilderFunc = builderFunc,
                LifeSpan = lifeSpan,
                CachedInstance = instance,
                ScopeObjectProviderFunc = scopeObjectProviderFunc,
                ScopeObject = scopeObject,
                Metadata = metadata,
                Provider = provider,
            });
        }

        /// <summary>
        /// Registers the specified initialization function.
        /// </summary>
        /// <typeparam name="TService">The type of the service.</typeparam>
        /// <typeparam name="TTarget">The type of the target.</typeparam>
        /// <param name="builderFunc">The builder function.</param>
        /// <param name="lifeSpan">The life span.</param>
        /// <param name="instance">The instance.</param>
        /// <param name="scopeObjectProviderFunc">The scope function.</param>
        /// <param name="scopeObject">The scope object.</param>
        /// <param name="metadata">The metadata.</param>
        /// <param name="provider">The provider.</param>
        public void Register<TService, TTarget>(Func<object> builderFunc = null,
            LifeSpan lifeSpan = LifeSpan.Singleton,
            object instance = null, Func<Registration, object> scopeObjectProviderFunc = null,
            object scopeObject = null, object metadata = null, IProvider provider = null)
        {
            Register(typeof(TService),
                typeof(TTarget),
                builderFunc,
                lifeSpan,
                instance,
                scopeObjectProviderFunc,
                scopeObject,
                metadata,
                provider);
        }

        /// <summary>
        /// Registers the specified registration request.
        /// </summary>
        /// <param name="registrationRequest">The registration request.</param>
        /// <exception cref="ArgumentNullException">registrationRequest</exception>
        /// <exception cref="InvalidOperationException"></exception>
        public void Register(Registration registrationRequest)
        {
            if (registrationRequest == null)
                throw new ArgumentNullException(nameof(registrationRequest));

            if (IsRegistered(registrationRequest))
                throw new InvalidOperationException($"Service: {registrationRequest.ServiceType} is already registered with this container");

            registrations.Add(registrationRequest);
        }

        /// <summary>
        /// Starts registration process for the specified service type.
        /// </summary>
        /// <param name="serviceType">Type of the service.</param>
        /// <returns></returns>
        public RegistrationBuilder Register(Type serviceType)
        {
            return new RegistrationBuilder(this)
                .For(serviceType);
        }

        /// <summary>
        /// Starts registration process for the specified service type.
        /// </summary>
        /// <typeparam name="TService">The type of the service.</typeparam>
        /// <returns></returns>
        public RegistrationBuilder Register<TService>()
        {
            return Register(typeof(TService));
        }

        /// <summary>
        /// Resolves the specified service type.
        /// </summary>
        /// <param name="serviceType">Type of the service.</param>
        /// <returns></returns>
        /// <exception cref="System.Exception">Error in registration object data</exception>
        public object Resolve(Type serviceType)
        {
            if (IsRegistered(serviceType, out var registration))
            {
                /* 
                 * transient life span ignores any cached instances
                 * and uses either the custom provider, builder func, or the default provider to create it everytime
                 */
                if (registration.LifeSpan == LifeSpan.Transient)
                {
                    return Resolve(registration);
                }

                /*
                 * singleton life span means the service instance cache will live as long as the current 
                 * instance of the app is alive.
                 */
                if (registration.LifeSpan == LifeSpan.Singleton)
                {
                    //check if we have a cached instance of this
                    if (registration.CachedInstance != null)
                        return registration.CachedInstance;

                    UpdateCachedInstane(registration, Resolve(registration));
                    return registration.CachedInstance;
                }

                /*
                 * custom life span means that the cached instance's life of the service is controlled
                 * by the scope object function. If it returns a new scope object, then we destroy old instance
                 * and build a new one and store it.
                 */
                if (registration.LifeSpan == LifeSpan.Custom)
                {
                    //check if scope is still valid
                    var possibleNewScope = registration.ScopeObjectProviderFunc(registration);

                    if (possibleNewScope?.Equals(registration.ScopeObject) == true)
                    {
                        //check if we have a cached instance of this
                        if (registration.CachedInstance != null)
                            return registration.CachedInstance;

                        UpdateCachedInstane(registration, Resolve(registration));
                        return registration.CachedInstance;
                    }

                    //if not valid, update to new one and build new instance
                    registration.ScopeObject = possibleNewScope;

                    UpdateCachedInstane(registration, Resolve(registration));
                    return registration.CachedInstance;
                }
            }

            return new DefaultProvider(serviceType, this).Create();
        }

        /// <summary>
        /// Resolves the specified service type.
        /// </summary>
        /// <typeparam name="TService">The type of the service.</typeparam>
        /// <returns></returns>
        public TService Resolve<TService>()
        {
            return (TService)Resolve(typeof(TService));
        }

        /// <summary>
        /// Resolves the specified registration.
        /// </summary>
        /// <param name="registration">The registration.</param>
        /// <returns></returns>
        /// <exception cref="RafaelWare.Goolag.InvalidRegistrationException"></exception>
        public object Resolve(Registration registration)
        {
            if (registration.BuilderFunc != null)
                return registration.BuilderFunc();

            if (registration.Provider != null)
                return registration.Provider.Create();

            //reaching here means the registration info was fucked
            throw new InvalidRegistrationException();
        }

        /// <summary>
        /// Determines whether the specified registration request is registered.
        /// </summary>
        /// <param name="registrationRequest">The registration request.</param>
        /// <returns>
        ///   <c>true</c> if the specified registration request is registered; otherwise, <c>false</c>.
        /// </returns>
        /// <exception cref="ArgumentNullException">registrationRequest</exception>
        public bool IsRegistered(Registration registrationRequest)
        {
            if (registrationRequest == null)
                throw new ArgumentNullException(nameof(registrationRequest));

            return registrations.Any(rr => rr.ServiceType == registrationRequest.ServiceType);
        }

        /// <summary>
        /// Determines whether the specified service type is registered.
        /// </summary>
        /// <param name="serviceType">Type of the service.</param>
        /// <param name="registration">The registration.</param>
        /// <returns>
        ///   <c>true</c> if the specified service type is registered; otherwise, <c>false</c>.
        /// </returns>
        /// <exception cref="ArgumentNullException">serviceType</exception>
        public bool IsRegistered(Type serviceType, out Registration registration)
        {
            if (serviceType == null)
                throw new ArgumentNullException(nameof(serviceType));

            registration = registrations.FirstOrDefault(r => r.ServiceType == serviceType);

            return registration != null;
        }

        /// <summary>
        /// Determines whether this instance is registered.
        /// </summary>
        /// <typeparam name="TService">The type of the service.</typeparam>
        /// <returns>
        ///   <c>true</c> if this instance is registered; otherwise, <c>false</c>.
        /// </returns>
        public bool IsRegistered<TService>()
        {
            return IsRegistered(typeof(TService), out _);
        }

        /// <summary>
        /// Updates the cached instane and disposes of the old one if it implements <see cref="IDisposable"/>.
        /// </summary>
        /// <param name="registration">The registration.</param>
        /// <param name="newCachedInstance">The new cached instance.</param>
        private void UpdateCachedInstane(Registration registration, object newCachedInstance)
        {
            if (registration.CachedInstance is IDisposable disposableCachedInstance)
                disposableCachedInstance.Dispose();

            registration.CachedInstance = newCachedInstance;
        }
    }
}
