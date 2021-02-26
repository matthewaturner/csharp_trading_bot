using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ElectronSPA.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class BackTestController : ControllerBase
    {
        private readonly ILogger<BackTestController> logger;

        public BackTestController(ILogger<BackTestController> logger)
        {
            this.logger = logger;
        }

        [HttpGet]
        public IEnumerable<int> GetRandomListOfNumbers(int lengthOfArray)
        {
            var arr = new int[lengthOfArray];
            var rng = new Random();

            for (int i = 0; i < lengthOfArray; i++)
            {
                arr[i] = rng.Next(1000);
            }

            return arr;
        }
    }
}
