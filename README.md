# Many TestServers
This sample shows how to use two (or more) TestServers during an integration test in ASP NET Core 2.1

1. We basically create a `mocked` IHttpClientFactory (assuming your app is using it):

```cs
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
```

2. Then it's easy to replace the main service throught your test class:

```cs
public class IntegrationTest
{
    private readonly TestServer _factoryFrontend;

    public IntegrationTest()
    {
        _factoryFrontend = new TestServer(BuildFrontend());
    }

    private IWebHostBuilder BuildFrontend()
    {
        // Create TestServer for every dependency
        var _factoryAPI = new TestServer(new WebHostBuilder().UseStartup<API.Startup>());

        // Use the same name configured in your app and add its test server
        var mockedHttpClientFactory = new MockedHttpClientFactory().AddClient("APIClient", _factoryAPI);

        // Build the Startup using ConfigureTestServices
        return new WebHostBuilder()
            .UseStartup<Startup>().ConfigureTestServices(services =>
            {
                services.AddSingleton<IHttpClientFactory>(mockedHttpClientFactory);
            });
    }
    [...]
```
