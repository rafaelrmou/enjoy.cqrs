using System;
using EnjoyCQRS.Commands;

namespace EnjoyCQRS.IntegrationTests.Shared.StubApplication.Commands.BarAggregate
{
    [UICommandName("bar-speak")]
    public class SpeakCommand : Command
    {
        public string Text { get; }

        public SpeakCommand(Guid aggregateId, string text) : base(aggregateId)
        {
            Text = text;
        }
    }
}