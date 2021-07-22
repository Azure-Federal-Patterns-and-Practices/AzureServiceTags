using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AzureServiceTags.WebApp.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace AzureServiceTags.WebApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ServiceTagController : ControllerBase
    {
        private readonly ILogger<ServiceTagController> logger;
        private readonly ServiceTagProvider serviceTagProvider;

        public ServiceTagController(ILogger<ServiceTagController> logger, ServiceTagProvider serviceTagProvider)
        {
            this.logger = logger;
            this.serviceTagProvider = serviceTagProvider;
        }

        [Route("{serviceTagId}/" + nameof(AddressPrefixes))]
        [HttpPost]
        [HttpGet]
        public async Task<IActionResult> AddressPrefixes(string serviceTagId, string cloudId)
        {
            var addressPrefixes = new List<string>();
            var serviceTagListFiles = await this.serviceTagProvider.GetAllServiceTagListFilesAsync();
            foreach (var serviceTagList in serviceTagListFiles.Select(s => s.ServiceTagList))
            {
                if (cloudId == null || string.Equals(cloudId, serviceTagList.Cloud, StringComparison.OrdinalIgnoreCase))
                {
                    addressPrefixes.AddRange(serviceTagList.Values
                        .Where(s => string.Equals(serviceTagId, s.Id, StringComparison.OrdinalIgnoreCase))
                        .SelectMany(s => s.Properties.AddressPrefixes)
                        .Distinct());
                }
            }
            return Ok(addressPrefixes);
        }
    }
}