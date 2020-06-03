using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace WEBAPI.Controllers
{
    [Route("api/[controller]")]
    [Produces("application/json")]
    [ApiController]
    public class TokensController : ControllerBase
    {   
        [HttpGet]
        public IActionResult GetTokens(string formula)
        {                    
          RPN rpn = new RPN(formula);
          
          if(rpn.CheckInput())
          {
              var good = new {
                  status = "ok",
                  infix = rpn.GetInfixTokenList(),
                  rpn = rpn.GetRPNTokenList()
                };
              return Ok(good);
          }
          var bad= new {
              status = "error",
              result = rpn.Message()
          };
          return BadRequest(bad);
        }
    }
}
