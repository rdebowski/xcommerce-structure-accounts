using System.Linq;
using System.Threading.Tasks;
using XCommerce.StructuredAccounts.Components;
using XCommerce.StructuredAccounts.Helper;
using Sitecore.Commerce.Core;
using Sitecore.Commerce.EntityViews;
using Sitecore.Commerce.Plugin.Customers;
using Sitecore.Framework.Conditions;
using Sitecore.Framework.Pipelines;

namespace XCommerce.StructuredAccounts.Pipelines.Blocks
{
    [PipelineDisplayName("Customers.blocks.ConnectSubaccountToMainAccountBlock")]
    public class ConnectSubaccountToMainAccountBlock : PipelineBlock<Customer, Customer, CommercePipelineExecutionContext>
    {
        private static string addCustomerActionName = "AddCustomer";
        private readonly GetCustomerPipeline _getCustomerPipeline;
        private readonly PersistEntityPipeline _persistEntityPipeline;

        public ConnectSubaccountToMainAccountBlock(GetCustomerPipeline getCustomerPipeline, PersistEntityPipeline persistEntityPipeline)
        {
            _getCustomerPipeline = getCustomerPipeline;
            _persistEntityPipeline = persistEntityPipeline;
        }

        public override async Task<Customer> Run(Customer arg, CommercePipelineExecutionContext context)
        {
            Condition.Requires<Customer>(arg).IsNotNull<Customer>("The customer can not be null");
            Condition.Requires<string>(arg.UserName).IsNotNullOrEmpty("The customer user name can not be null");

            string mainAccountId = GetMainAccountId(context);
            if (string.IsNullOrEmpty(mainAccountId))
            {
                return arg;
            }

            var mainAccountCustomer = await _getCustomerPipeline.Run(new GetCustomerArgument(mainAccountId, string.Empty), context);
            StructuredAccountOwnerComponent structuredAccountOwnerComponent = mainAccountCustomer.GetComponent<StructuredAccountOwnerComponent>();
            structuredAccountOwnerComponent.Subaccounts.Add(new EntityReference(arg.Id, arg.FriendlyId));

            var resulttt = await _persistEntityPipeline.Run(new PersistEntityArgument(mainAccountCustomer), context);

            return arg;
        }

        private string GetMainAccountId(CommercePipelineExecutionContext context)
        {
            var entityView = context.CommerceContext.GetObjects<EntityView>().FirstOrDefault(x => x.Action == addCustomerActionName);
            ViewProperty mainAccountProperty = entityView.Properties.FirstOrDefault(x => x.Name == AccountContants.MainAccountId);

            return mainAccountProperty?.Value;
        }
    }
}