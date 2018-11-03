using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace RafaelWare.Gulag
{
    public class DefaultProvider : IProvider
    {
        /// <summary>
        /// The container
        /// </summary>
        private readonly IoC container;

        /// <inheritdoc />
        public Type Type { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultProvider" /> class.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="container">The container.</param>
        /// <exception cref="System.ArgumentNullException">type</exception>
        public DefaultProvider(Type type, IoC container)
        {
            Type = type ?? throw new ArgumentNullException(nameof(type));
            this.container = container ?? throw new ArgumentNullException(nameof(container));
        }

        /// <inheritdoc />
        public object Create()
        {
            return InitializeType(Type);
        }

        /// <summary>
        /// Initializes the type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        private object InitializeType(Type type)
        {
            //use reflection to extract the constructor and invoke it
            var typeInfo = type.GetTypeInfo();
            var constructorInfo = typeInfo.DeclaredConstructors.OrderByDescending(ctor => ctor.GetParameters().Length).FirstOrDefault();

            //if no constructor found, then just create an instabce
            if (constructorInfo != null)
            {
                var constructorParameters = constructorInfo.GetParameters();

                if (constructorParameters.Length == 0) // default constructor
                    return constructorInfo.Invoke(new object[] { });
                else // not the default, needs parameters
                {
                    var parameters = new List<object>(constructorParameters.Length);
                    foreach (var parameter in constructorParameters)
                    {
                        var initializedParameter = container.Resolve(parameter.ParameterType);
                        parameters.Add(initializedParameter);
                    }

                    return constructorInfo.Invoke(parameters.ToArray());
                }
            }

            return Activator.CreateInstance(type);
        }
    }
}
