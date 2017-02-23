using System;
using System.Web.Http;
using Axh.Retro.CPU.Z80.Wiring;

namespace Axh.Retro.GameBoy.Web.Controllers.Api
{
    public class GameBoyController : ApiController
    {
        private readonly ICpuCoreContext _context;

        /// <summary>
        /// Initializes a new instance of the <see cref="GameBoyController"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        public GameBoyController(ICpuCoreContext context)
        {
            _context = context;
        }

        public IHttpActionResult GetAll() => Ok(_context.GetAllCoreIds());


        public IHttpActionResult Post()
        {
            var core = _context.GetNewCore();
            return Ok(core.CoreId);
        }

        public IHttpActionResult Delete(Guid id)
        {
            if (_context.StopCore(id))
            {
                return Ok();
            }

            return NotFound();
        }
    }
}
