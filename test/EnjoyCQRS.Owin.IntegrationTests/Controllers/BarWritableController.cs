using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;
using EnjoyCQRS.EventSource;
using EnjoyCQRS.IntegrationTests.Shared;
using EnjoyCQRS.MessageBus;

namespace EnjoyCQRS.Owin.IntegrationTests.Controllers
{
    [RoutePrefix("command/bar")]
    public class BarWritableController : ApiController
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICommandDispatcher _dispatcher;
        private readonly CommandDiscovery _commandDiscovery;

        public BarWritableController(IUnitOfWork unitOfWork, ICommandDispatcher dispatcher)
        {
            _commandDiscovery = new CommandDiscovery(typeof(FooAssembler).Assembly.GetTypes());
            _unitOfWork = unitOfWork;
            _dispatcher = dispatcher;
        }
        
        [Route("commit")]
        public async Task<IHttpActionResult> Commit(IEnumerable<CommandDescriptor> commands)
        {
            var realCommands = _commandDiscovery.Discovery(commands);
            
            foreach (var realCommand in realCommands)
            {
                await _dispatcher.DispatchAsync(realCommand);
            }

            await _unitOfWork.CommitAsync();

            return Ok();
        }
    }
}
