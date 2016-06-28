using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Reflection;
using System.Threading.Tasks;
using EnjoyCQRS.Commands;
using EnjoyCQRS.EventSource.Storage;
using EnjoyCQRS.IntegrationTests.Shared;
using EnjoyCQRS.Owin.IntegrationTests.Infrastructure;
using FluentAssertions;
using Newtonsoft.Json;
using Xunit;

namespace EnjoyCQRS.Owin.IntegrationTests
{
    public class UICommandAttributeTests : ServerTestBase
    {
        [Fact]
        public async Task Should_dispatch_commands_based_on_attribute()
        {
            var aggregateId = Guid.NewGuid();

            var commands = new[]
            {
                new CommandDescriptor("create-bar", JsonConvert.SerializeObject(new { aggregateId = aggregateId })),
                new CommandDescriptor("bar-speak", JsonConvert.SerializeObject(new { aggregateId = aggregateId, text = "hello world" }))
            };

            var eventStore = new InMemoryEventStore();

            var server = CreateTestServer(eventStore);

            var response = await server.CreateRequest("/command/bar/commit").And(message =>
            {
                message.Content = new ObjectContent(typeof(IEnumerable<CommandDescriptor>), commands, new JsonMediaTypeFormatter());
            }).PostAsync();

            InMemoryEventStore.Events.Count.Should().Be(2);

            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }
    }

    public class CommandDescriptor
    {
        public CommandDescriptor(string name, string data)
        {
            Name = name;
            Data = data;
        }

        public string Name { get; }
        public string Data { get; }
    }

    public class CommandDiscovery
    {
        private readonly Type[] _commands;

        public CommandDiscovery(Type[] commands)
        {
            _commands = commands;
        }

        public IEnumerable<ICommand> Discovery(IEnumerable<CommandDescriptor> commandDescriptors)
        {
            var commands = from type in _commands
                where type.GetCustomAttribute<UICommandNameAttribute>() != null
                select new
                {
                    @Type = type,
                    Attr = type.GetCustomAttribute<UICommandNameAttribute>()
                }
                into a
                from cd in commandDescriptors
                where cd.Name == a.Attr.Name
                select (ICommand) JsonConvert.DeserializeObject(cd.Data, a.Type);

            return commands;
        }
    }
}