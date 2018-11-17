namespace XCommerce.StructuredAccounts
{
    using System.Reflection;

    using XCommerce.StructuredAccounts.Helper;
    using XCommerce.StructuredAccounts.Pipelines;
    using XCommerce.StructuredAccounts.Pipelines.Blocks;

    using Microsoft.Extensions.DependencyInjection;

    using Sitecore.Commerce.Core;
    using Sitecore.Commerce.EntityViews;
    using Sitecore.Commerce.Plugin.Customers;
    using Sitecore.Framework.Configuration;
    using Sitecore.Framework.Pipelines.Definitions.Extensions;

    public class ConfigureSitecore : IConfigureSitecore
    {
        public void ConfigureServices(IServiceCollection services)
        {
            var assembly = Assembly.GetExecutingAssembly();
            services.RegisterAllPipelineBlocks(assembly);
            services.AddSingleton<IRegistrationHelper, RegistrationHelper>();
            services.Sitecore().Pipelines(config => config
            .AddPipeline<IGetSubaccountsPipeline, GetSubaccountsPipeline>(configure =>
            {
                configure.Add<GetSubaccountsBlock>();
            })
            .AddPipeline<IUpdateSubaccountPipeline, UpdateSubaccountPipeline>(conf =>
            {
                conf.Add<UpdateSubaccountBlock>();
                conf.Add<PersistCustomerBlock>();
            })
            .ConfigurePipeline<IGetEntityViewPipeline>(configure =>
            {
                configure.Replace<GetCustomerDetailsViewBlock, GetCustomerDetailsViewBlockCustom>();
            })
            .ConfigurePipeline<ICreateCustomerPipeline>(configure =>
            {
                configure.Add<ConnectSubaccountToMainAccountBlock>().After<CreateCustomerBlock>();
            })
            .ConfigurePipeline<IDeleteCustomerPipeline>(conf => conf.Add<DeleteMainAccountReferenceBlock>().After<GetCustomerBlock>())
            .ConfigurePipeline<IConfigureServiceApiPipeline>(configure => configure.Add<XCommerce.StructuredAccounts.ConfigureServiceApiBlock>()));
            

            services.RegisterAllCommands(assembly);

        }
    }
}