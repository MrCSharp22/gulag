using System;

namespace RafaelWare.Goolag
{
    public interface IProvider
    {
        /// <summary>
        /// Gets the target type of the service to resolve.
        /// </summary>
        /// <value>
        /// The type.
        /// </value>
        Type Type { get; }

        /// <summary>
        /// Creates the requested type.
        /// </summary>
        /// <returns></returns>
        object Create();
    }
}
