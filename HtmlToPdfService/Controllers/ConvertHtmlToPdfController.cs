using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace HtmlToPdfService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ConvertHtmlToPdfController : ControllerBase
    {
        private readonly ILogger<ConvertHtmlToPdfController> _logger;

        public ConvertHtmlToPdfController(ILogger<ConvertHtmlToPdfController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public ConvertHtmlToPdf Get(string url)
        {
            return new ConvertHtmlToPdf { UrlToPdf = url };
        }
    }
}
