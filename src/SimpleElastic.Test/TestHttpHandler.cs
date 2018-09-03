using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleElastic.Test
{
    class TestHandler : DelegatingHandler
    {
        private HttpStatusCode _returnCode;
        private string _responseBody;

        public TestHandler(HttpStatusCode returnCode, string responseBody)
        {
            _returnCode = returnCode;
            _responseBody = responseBody;
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            return Task.FromResult(new HttpResponseMessage
            {
                StatusCode = _returnCode, Content = new StringContent(_responseBody)
            });
        }
    }
}
