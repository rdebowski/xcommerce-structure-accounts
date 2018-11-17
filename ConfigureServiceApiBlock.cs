// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConfigureServiceApiBlock.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2017
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace XCommerce.StructuredAccounts
{
    using System.Threading.Tasks;

    using XCommerce.StructuredAccounts.Models;

    using Microsoft.AspNetCore.OData.Builder;

    using Sitecore.Commerce.Core;
    using Sitecore.Framework.Conditions;
    using Sitecore.Framework.Pipelines;

    /// <summary>
    /// Defines a block which configures the OData model for the Carts plugin
    /// </summary>
    /// <seealso>
    ///     <cref>
    ///         Sitecore.Framework.Pipelines.PipelineBlock{Microsoft.AspNetCore.OData.Builder.ODataConventionModelBuilder,
    ///         Microsoft.AspNetCore.OData.Builder.ODataConventionModelBuilder,
    ///         Sitecore.Commerce.Core.CommercePipelineExecutionContext}
    ///     </cref>
    /// </seealso>
    [PipelineDisplayName("SubAccountsServiceApiBlock")]
    public class ConfigureServiceApiBlock : PipelineBlock<ODataConventionModelBuilder, ODataConventionModelBuilder, CommercePipelineExecutionContext>
    {
        /// <summary>
        /// The execute.
        /// </summary>
        /// <param name="modelBuilder">
        /// The argument.
        /// </param>
        /// <param name="context">
        /// The context.
        /// </param>
        /// <returns>
        /// The <see cref="ODataConventionModelBuilder"/>.
        /// </returns>
        public override Task<ODataConventionModelBuilder> Run(ODataConventionModelBuilder modelBuilder, CommercePipelineExecutionContext context)
        {
            Condition.Requires(modelBuilder).IsNotNull($"{this.Name}: The argument cannot be null.");

            var configurationCreateFeredatedOrder = modelBuilder.Function("GetSubaccounts");
            configurationCreateFeredatedOrder.Parameter<string>("customerId");
            configurationCreateFeredatedOrder.Returns<GetSubaccountsModel>();

            var configurationSubaccountUpdate = modelBuilder.Action("UpdateSubaccount");
            configurationSubaccountUpdate.Parameter<SubaccountModel>("subaccount");
            configurationSubaccountUpdate.Parameter<string>("mainaccountId");
            configurationSubaccountUpdate.Returns<SubaccountAcctionsResult>();

            var configurationSubaccountDelete = modelBuilder.Action("DeleteSubaccounts");
            configurationSubaccountDelete.CollectionParameter<string>("subaccountsId");
            configurationSubaccountDelete.Parameter<string>("mainaccountId");
            configurationSubaccountDelete.Returns<SubaccountAcctionsResult>();

            return Task.FromResult(modelBuilder);
        }
    }
}