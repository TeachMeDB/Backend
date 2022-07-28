using Microsoft.AspNetCore.Mvc;
using youAreWhatYouEat.Authorization;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace youAreWhatYouEat.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthtestController : ControllerBase
    {
        // GET api/<AuthtestController>/
        [HttpGet]
        public async Task<MyTokenReply> Get(string token)
        {
            MyTokenReply ret = await MyToken.checkToken(token);
            return ret;
        }
    }
}
