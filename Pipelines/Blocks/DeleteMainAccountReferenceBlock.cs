using System;
using System.Linq;
using System.Threading.Tasks;

using XCommerce.StructuredAccounts.Components;

using Sitecore.Commerce.Core;
using Sitecore.Commerce.EntityViews;
using Sitecore.Commerce.Plugin.Customers;
using Sitecore.Framework.Pipelines;

namespace XCommerce.StructuredAccounts.Pipelines.Blocks
{
    public class DeleteMainAccountReferenceBlock : PipelineBlock<Customer, Customer, CommercePipelineExecutionContext>
    {
        private readonly GetCustomerPipeline _getCustomerPipeline;
        private readonly PersistEntityPipeline _persistEntityPipeline;

        public DeleteMainAccountReferenceBlock(GetCustomerPipeline getCustomerPipeline, PersistEntityPipeline persistEntityPipeline)
        {
            _getCustomerPipeline = getCustomerPipeline;
            _persistEntityPipeline = persistEntityPipeline;
        }
        public override async Task<Customer> Run(Customer arg, CommercePipelineExecutionContext context)
        {
            var details = arg.GetComponent<CustomerDetailsComponent>().View.ChildViews.FirstOrDefault() as EntityView;

            if (details == null)
            {
                return arg;
            }

            string mainId = details.GetPropertyValue("MainAccountId").ToString();

            if (mainId == null) { return arg; }
            var mainAccount = await _getCustomerPipeline.Run(new GetCustomerArgument(mainId, string.Empty), context);
            var reference = mainAccount.HasComponent<StructuredAccountOwnerComponent>() 
                                    ? mainAccount.GetComponent<StructuredAccountOwnerComponent>()
                                      .Subaccounts.FirstOrDefault(x => x.EntityTarget.Equals(arg.Id, StringComparison.OrdinalIgnoreCase)) : null;
            if (reference != null)
            {
                var subaccounts = mainAccount.GetComponent<StructuredAccountOwnerComponent>();
                subaccounts.Subaccounts.Remove(reference);
                await _persistEntityPipeline.Run(new PersistEntityArgument(mainAccount), context);              
            }

            return arg;
        }
    }
}
