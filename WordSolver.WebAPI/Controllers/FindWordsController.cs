using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WordSolver.Core;

namespace WordSolver.WebAPI.Controllers
{
    [Route("/")]
    [ApiController]
    public class FindWordsController : ControllerBase
    {
        private const int DefaultMinLength = 3;
        private readonly IWordFinder _wf;

        public FindWordsController(IWordFinder wf)
        {
            _wf = wf;
        }

        [HttpGet("{letters:alpha:maxlength(7)}")]
        public List<string> Get(string letters)
        {
            return _wf.FindWords(letters.ToLower(), DefaultMinLength).ToList();
        }

        [HttpGet("{letters:alpha:maxlength(7)}/{minLength:range(1,7)}")]
        public ActionResult<List<string>> Get(string letters, int minLength)
        {
            return _wf.FindWords(letters.ToLower(), minLength).ToList();
        }
    }
}