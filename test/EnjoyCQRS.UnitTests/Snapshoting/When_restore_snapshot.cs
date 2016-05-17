using System;
using EnjoyCQRS.EventSource.Snapshots;
using EnjoyCQRS.UnitTests.Domain;
using EnjoyCQRS.UnitTests.Domain.AggregateWithEntities;
using EnjoyCQRS.UnitTests.Domain.Snapshots;
using FluentAssertions;
using Xunit;

namespace EnjoyCQRS.UnitTests.Snapshoting
{
    public class When_restore_snapshot
    {
        private StubAggregateSnapshot _snapshot;
        private ComplexStubAggregate _stubAggregate;

        public When_restore_snapshot()
        {
            _snapshot = new StubAggregateSnapshot
            {
                AggregateId = Guid.NewGuid(),
                Name = "Coringa",
                Version = 1
            };

            _stubAggregate = new ComplexStubAggregate();
            ((ISnapshotAggregate<StubAggregateSnapshot>)_stubAggregate).Restore(_snapshot);
        }

        [Fact]
        public void Should_set_aggregate_properties()
        {
            _stubAggregate.Name.Should().Be(_snapshot.Name);

            _stubAggregate.Version.Should().Be(_snapshot.Version);
        }
    }
}