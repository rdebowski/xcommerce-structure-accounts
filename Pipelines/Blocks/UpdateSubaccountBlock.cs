using System.Linq;
using System.Threading.Tasks;

using XCommerce.StructuredAccounts.Models;

using Sitecore.Commerce.Core;
using Sitecore.Commerce.EntityViews;
using Sitecore.Commerce.Plugin.Customers;
using Sitecore.Framework.Conditions;
using Sitecore.Framework.Pipelines;

namespace XCommerce.StructuredAccounts.Pipelines.Blocks
{
    public class UpdateSubaccountBlock : PipelineBlock<SubaccountModel, Customer, CommercePipelineExecutionContext>
    {
        private readonly IGetCustomerPipeline _getCustomer;
        public UpdateSubaccountBlock(IGetCustomerPipeline getCustomer)
        {
            _getCustomer = getCustomer;
        }

        public override async Task<Customer> Run(SubaccountModel arg, CommercePipelineExecutionContext context)
        {
            Condition.Requires<SubaccountModel>(arg).IsNotNull<SubaccountModel>("The customer can not be null");
            Condition.Requires<string>(arg.Id).IsNotNullOrEmpty("The customerId can not be null");
            var customer = await _getCustomer.Run(new GetCustomerArgument(arg.Id, string.Empty), context);

            var details = customer.GetComponent<CustomerDetailsComponent>().View.ChildViews.FirstOrDefault() as EntityView;
            details.SetPropertyValue(nameof(SubaccountModel.CanCreateOrders), arg.CanCreateOrders);
            details.SetPropertyValue(nameof(SubaccountModel.CanPurchase), arg.CanPurchase);

            return customer;
        }
    }
}
