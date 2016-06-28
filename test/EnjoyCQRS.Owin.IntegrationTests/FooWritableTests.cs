﻿using System;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using EnjoyCQRS.EventSource.Storage;
using EnjoyCQRS.Owin.IntegrationTests.Infrastructure;
using FluentAssertions;
using Microsoft.Owin.Testing;
using Xunit;

namespace EnjoyCQRS.Owin.IntegrationTests
{
    public class FooWritableTests : ServerTestBase
    {
        public const string CategoryName = "Integration";
        public const string CategoryValue = "Owin";

        [Trait(CategoryName, CategoryValue)]
        [Fact]
        public async Task Should_create_foo()
        {
            var eventStore = new InMemoryEventStore();

            var server = CreateTestServer(eventStore);

            var response = await server.CreateRequest("/command/foo").PostAsync();

            var result = await response.Content.ReadAsStringAsync();
            
            var aggregateId = ExtractAggregateIdFromResponseContent(result);

            InMemoryEventStore.Events.Count(e => e.AggregateId == aggregateId).Should().Be(1);
        }

        [Trait(CategoryName, CategoryValue)]
        [Fact]
        public async Task Should_do_something()
        {
            var eventStore = new InMemoryEventStore();

            var server = CreateTestServer(eventStore);

            var response = await server.CreateRequest("/command/foo").PostAsync();

            var result = await response.Content.ReadAsStringAsync();
            
            var aggregateId = ExtractAggregateIdFromResponseContent(result);

            response = await server.CreateRequest($"/command/foo/{aggregateId}/doSomething").PostAsync();

            response.StatusCode.Should().Be(HttpStatusCode.OK);

            aggregateId.Should().NotBeEmpty();
        }

        [Trait(CategoryName, CategoryValue)]
        [Fact]
        public async Task Should_emit_many_events()
        {
            var eventStore = new InMemoryEventStore();

            var server = CreateTestServer(eventStore);

            var response = await server.CreateRequest("/command/foo/flood/4").PostAsync();

            InMemoryEventStore.Events.Count.Should().Be(4);

            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }
        
        private Guid ExtractAggregateIdFromResponseContent(string content)
        {
            var match = Regex.Match(content, "{\"AggregateId\":\"(.*)\"}", RegexOptions.IgnoreCase | RegexOptions.Compiled);

            var aggregateId = match.Groups[1].Value;

            return Guid.Parse(aggregateId);
        }
    }
}
