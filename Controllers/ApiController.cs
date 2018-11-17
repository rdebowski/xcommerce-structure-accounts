using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http.OData;

using XCommerce.StructuredAccounts.Commands;
using XCommerce.StructuredAccounts.Models;

using Microsoft.AspNetCore.Mvc;

using Newtonsoft.Json.Linq;

using Sitecore.Commerce.Core;

namespace XCommerce.StructuredAccounts.Controllers
{

    public class ApiController : CommerceController
    {       
        public ApiController(IServiceProvider serviceProvider, CommerceEnvironment globalEnvironment) : base(serviceProvider, globalEnvironment)
        {           
           
        }

        [HttpGet]
        [Route("api/GetSubaccounts(customerId={customerId})")]
        public async Task<IActionResult> GetSubaccounts(string customerId)
        {
            var command = Command<GetSubaccountsCommand>();
            var result = await command.Process(this.CurrentContext, customerId);   
            
            return new ObjectResult(result);
        }

        [HttpPut]
        [Route("api/UpdateSubaccount()")]
        public async Task<IActionResult> UpdateSubaccount([FromBody]ODataActionParameters parameters)
        {
            var subaccount = (parameters["subaccount"] as JObject).ToObject<SubaccountModel>();
            string mainaccountId = parameters["mainaccountId"].ToString();
            var command = Command<UpdateSubaccountCommand>();
            var result = await command.Process(this.CurrentContext,(SubaccountModel)subaccount, mainaccountId);

            return new ObjectResult(result);
        }

        [HttpPut]
        [Route("api/DeleteSubaccounts()")]
        public async Task<IActionResult> DeleteSubaccounts([FromBody]ODataActionParameters parameters)
        {
            var subaccountsId = (parameters["subaccountsId"] as JArray).ToObject<List<string>>();
            string mainaccountId = parameters["mainaccountId"].ToString();
            var command = Command<DeleteSubaccountsCommand>();
            var result = await command.Process(this.CurrentContext, subaccountsId, mainaccountId);

            return new ObjectResult(result);
        }
    }
}