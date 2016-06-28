using EnjoyCQRS.EventSource.Storage;
using Microsoft.Owin.Testing;

namespace EnjoyCQRS.Owin.IntegrationTests.Infrastructure
{
    public abstract class ServerTestBase
    {
        protected TestServer CreateTestServer(IEventStore eventStore)
        {
            var startup = new Startup
            {
                EventStore = eventStore
            };

            var testServer = TestServer.Create(startup.Configuration);

            return testServer;
        }
    }
}