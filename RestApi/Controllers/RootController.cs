using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RestApi.Controllers
{
    [Route("/")]
    [ApiController]
    [ApiVersion("1.0")]
    public class RootController: ControllerBase
    {
        [HttpGet(Name = nameof(GetRoot))]
        [ProducesResponseType(200)]//optional - will help open api documentation,attribute specifies that this method could return 200 status 
        public IActionResult GetRoot()
        {
            var response = new
            {
                href = Url.Link(nameof(RoomsController.GetRooms), null),
                rooms=new
                {
                    href=Url.Link(nameof(RoomsController.GetRooms), null)
                },
                info=new
                {
                    href=Url.Link(nameof(InfoController.Getinfo), null)
                }
            };

            return Ok(response);
        }
    }
}
