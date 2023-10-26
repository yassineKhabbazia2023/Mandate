using KPMG.Constellation.Portal.Client;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace KPMG.Pulse.Back.Accounting.Mandate.AspNetCore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PortalTestController : ControllerBase
    {
        private readonly IPortalClient portalClient;

        public PortalTestController(IPortalClient portalClient)
        {

            this.portalClient = portalClient;
        }

        [HttpGet]
        public async Task<IActionResult> GetCollectionsAsync([FromQuery] string? query)
        {
            try
            {
                return this.Ok(this.portalClient.GetAccountsODataWithoutCache(query));
            }
            catch (Exception ex)
            {
                Console.Write($"{ex.Message}");
                throw;
            }
        }
    }
}
