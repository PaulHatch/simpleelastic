using System;

namespace SimpleElastic
{
    /// <summary>
    /// Defines a connection provider which supplies host endpoints
    /// to the Elasticsearch client.
    /// </summary>
    public interface IHostProvider
    {
        /// <summary>
        /// Gets the host for the next request.
        /// </summary>
        /// <returns>The next host to use.</returns>
        Uri Next();
    }
}