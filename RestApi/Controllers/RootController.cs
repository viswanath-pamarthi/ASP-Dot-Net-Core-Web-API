using Microsoft.AspNetCore.Mvc;
using RestApi.Models;
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
            
            var response = new RootResponse
            {
                Self = Link.To(nameof(GetRoot),null), //Link.To(nameof(RoomsController.GetRooms), null), TODO: URL.Link(nameof(GetRoot), null)

                rooms= Link.ToCollection(nameof(RoomsController.GetAllRooms), null)
                ,
                info= Link.To(nameof(InfoController.Getinfo), null)
                
            };

            return Ok(response);
        }
      
    }
}
