using KPMG.Pulse.Back.Accounting.Mandate.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace KPMG.Pulse.Back.Accounting.Mandate.AspNetCore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PortalTestController : ControllerBase
    {
        private readonly IPortalManager portalClient;

        public PortalTestController(IPortalManager portalClient)
        {
            this.portalClient = portalClient;
        }

        [HttpGet]
        public async Task<IActionResult> GetCollectionsAsync([FromQuery] string? query, [FromQuery] int top)
        {
            try
            {
                query += "$filter=IBSCode eq '1000265308'&$select=Id,IBSCode,AccountName,Adresse,State,ZipCode,Country,AccountRegisterIdentification1,AccountDeliveryEmail,Role" +
                    "&$expand=Role($expand=Contact;$select=Contact,RoleFunctionName)";
                return this.Ok(await this.portalClient.GetAccountsODataWithoutCache(top, 0, query, true));
            }
            catch (Exception ex)
            {
                Console.Write($"{ex.Message}");
                throw;
            }
        }
    }
}
