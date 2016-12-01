using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Axh.Retro.GameBoy.Web.Controllers
{
    public class GameBoyController : ApiController
    {
        public IHttpActionResult GetAll()
        {
            return Ok(new [] { 1, 2 });
        }
    }
}
