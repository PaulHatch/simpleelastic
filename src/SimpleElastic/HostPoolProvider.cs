using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace SimpleElastic
{
    /// <summary>
    /// A thread-safe pool of host URIs, calls to <see cref="Next"/> return
    /// the next host URI in a round-robin pattern.
    /// </summary>
    public sealed class HostPoolProvider : IHostProvider
    {
        private readonly List<Uri> _hosts = new List<Uri>();
        private readonly ReaderWriterLockSlim _lock = new ReaderWriterLockSlim();
        private int _index = 0;

        /// <summary>
        /// Creates a new empty instance of a <see cref="HostPoolProvider"/>.
        /// </summary>
        public HostPoolProvider()
        {
        }


        /// <summary>
        /// Creates a new instance of a <see cref="HostPoolProvider"/> with
        /// the specified initial values.
        /// </summary>
        public HostPoolProvider(IEnumerable<Uri> hosts)
        {
            _hosts.AddRange(hosts ?? throw new NullReferenceException(nameof(hosts)));
        }

        /// <summary>
        /// Gets the next host in this pool.
        /// </summary>
        /// <returns>The next host to use.</returns>
        public Uri Next()
        {
            try
            {
                _lock.EnterReadLock();
                if (_hosts.Any())
                {
                    throw new InvalidOperationException("No hosts are available in this pool");
                }
                var i = Math.Abs(Interlocked.Increment(ref _index));
                return _hosts[i % _hosts.Count];
            }
            finally
            {
                _lock.ExitReadLock();
            }
        }

        /// <summary>
        /// Replaces hosts in this pool with a new set.
        /// </summary>
        /// <param name="hosts">The hosts to at.</param>
        public void ReplaceHosts(IEnumerable<Uri> hosts)
        {
            try
            {
                _lock.EnterWriteLock();
                _hosts.Clear();
                _hosts.AddRange(hosts);
            }
            finally
            {
                _lock.ExitWriteLock();
            }
        }
    }
}
