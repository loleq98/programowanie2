
using Microsoft.AspNetCore.Mvc;
using WEBAPI;

namespace WEBAPI.Controllers
{
    [Route("api/[controller]")]
    [Produces("application/json")]
    [ApiController]
    public class CalculateController : ControllerBase
    {   
        [HttpGet]
        public IActionResult CalculateX(string formula,double? x)
        {         

          if(formula == null || x == null )
          {
            var bad2= new {
              status = "error",
              result = "wrong input!"};
               return BadRequest(bad2);
          }

          RPN rpn = new RPN(formula,(double)x);
        
          if(rpn.CheckInput())
          {
              var good = new {
                  status = "ok",
                  result = rpn.CalculateValueOfGivenX()              
                };
              return Ok(good);
          }
          var bad= new {
              status = "error",
              result = rpn.Message()
          };
          return BadRequest(bad);
        }

        [HttpGet]
        [Route("xy")]
        public IActionResult CalculateInterval(string formula,double? from,double? to,int? n)
        {         
          if(formula == null || from == null || to==null || n==null)
          {
            var bad2= new {
              status = "error",
              result = "wrong input"};
               return BadRequest(bad2);
          }

          RPN rpn = new RPN(formula,(double)from,(double)to,(int)n);
          
          if(rpn.CheckInput())
          {
              var good = new {
                  status = "ok",
                  result = rpn.CalculateValueOfGivenInterval()              
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