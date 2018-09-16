using System;
using System.Collections.Generic;
using System.Text;

namespace SimpleElastic
{
    /// <summary>
    /// Connection provider which contains a single host.
    /// </summary>
    public class SingleHostProvider : IHostProvider
    {
        private Uri _host;

        /// <summary>
        /// Initializes a new instance of the <see cref="SingleHostProvider"/> class.
        /// </summary>
        /// <param name="host">The host.</param>
        public SingleHostProvider(Uri host)
        {
            _host = host;
        }

        /// <summary>
        /// Gets the host for the next request.
        /// </summary>
        /// <returns>
        /// The next host to use.
        /// </returns>
        public Uri Next() => _host;
    }
}
