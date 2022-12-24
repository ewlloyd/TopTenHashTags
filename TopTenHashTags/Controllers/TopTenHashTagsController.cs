using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace TopTenHashTags.Site.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TopTenHashTagsController : ControllerBase
    {
        private readonly ILogger<TopTenHashTagsController> _logger;
        private readonly ILeaderboard<string> _leaderboard;

        public TopTenHashTagsController(ILogger<TopTenHashTagsController> logger, ILeaderboard<string> leaderboard)
        {
            _logger = logger;
            _leaderboard = leaderboard;
        }

        [HttpGet(Name = "TopTenHashTags")]
        public IEnumerable<IResult<string>> Get() => _leaderboard.Results;
    }
}