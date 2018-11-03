namespace RafaelWare.Goolag
{
    public enum LifeSpan
    {
        /// <summary>
        /// A new instance of the service will be initialized every time it is resolved.
        /// </summary>
        Transient,

        /// <summary>
        /// An instane of the service will be initialized for the first resolve, then the instance is cached for next resolves.
        /// </summary>
        Singleton,

        /// <summary>
        /// The life span of this service is controlled by the user.
        /// </summary>
        Custom,
    }
}
