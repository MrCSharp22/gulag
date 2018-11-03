using System;

namespace RafaelWare.Gulag
{
    public abstract class Provider<T> : IProvider
    {
        /// <summary>
        /// The container
        /// </summary>
        protected readonly IoC Container;

        /// <inheritdoc />
        public virtual Type Type { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Provider{T}" /> class.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="container">The container.</param>
        /// <exception cref="System.ArgumentNullException">
        /// type
        /// or
        /// container
        /// </exception>
        protected Provider(Type type, IoC container)
        {
            Type = type ?? throw new ArgumentNullException(nameof(type));
            this.Container = container ?? throw new ArgumentNullException(nameof(container));
        }

        /// <inheritdoc />
        public object Create()
        {
            return CreateInstance();
        }

        /// <summary>
        /// Creates the instance.
        /// </summary>
        /// <returns></returns>
        protected abstract T CreateInstance();
    }
}
