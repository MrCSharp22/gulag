using System;

namespace RafaelWare.Gulag
{
    public class InvalidRegistrationException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidRegistrationException"/> class.
        /// </summary>
        public InvalidRegistrationException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidRegistrationException"/> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public InvalidRegistrationException(string message) : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidRegistrationException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="inner">The inner.</param>
        public InvalidRegistrationException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}
