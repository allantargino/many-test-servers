using Microsoft.AspNetCore.TestHost;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace Frontend.Tests
{
    public class MockedHttpClientFactory : IHttpClientFactory
    {
        private readonly IDictionary<string, TestServer> testServers;

        public MockedHttpClientFactory()
        {
            testServers = new Dictionary<string, TestServer>();
        }
        
        public MockedHttpClientFactory AddClient(string name, TestServer testServer)
        {
            if (string.IsNullOrWhiteSpace(name)) throw new ArgumentNullException( nameof(name));
            if (testServer == null) throw new ArgumentNullException(nameof(testServer));

            testServers.Add(name, testServer);

            return this;
        }

        public HttpClient CreateClient(string name)
        {
            if (string.IsNullOrWhiteSpace(name)) throw new ArgumentNullException( nameof(name));

            return testServers[name].CreateClient();
        }
    }
}
