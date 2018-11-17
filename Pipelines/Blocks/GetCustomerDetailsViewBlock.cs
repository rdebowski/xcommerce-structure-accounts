using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using XCommerce.StructuredAccounts.Helper;
using Sitecore.Commerce.Core;
using Sitecore.Commerce.EntityViews;
using Sitecore.Commerce.Plugin.Shops;
using Sitecore.Framework.Pipelines;

namespace Sitecore.Commerce.Plugin.Customers
{
    [PipelineDisplayName("Customers.blocks.getcustomerdetailsview")]
    public class GetCustomerDetailsViewBlockCustom : PipelineBlock<EntityView, EntityView, CommercePipelineExecutionContext>
    {
        private readonly IGetLocalizedCustomerStatusPipeline _getLocalizedCustomerStatusPipeline;
        private readonly IRegistrationHelper _registrationHelper;

        public GetCustomerDetailsViewBlockCustom(IGetLocalizedCustomerStatusPipeline getLocalizedCustomerStatusPipeline, IRegistrationHelper registrationHelper)
          : base((string)null)
        {
            this._getLocalizedCustomerStatusPipeline = getLocalizedCustomerStatusPipeline;
            _registrationHelper = registrationHelper;
        }

        public override async Task<EntityView> Run(EntityView entityView, CommercePipelineExecutionContext context)
        {
            GetCustomerDetailsViewBlockCustom detailsViewBlock = this;

            EntityViewArgument request = context.CommerceContext.GetObject<EntityViewArgument>();
            if (string.IsNullOrEmpty(request?.ViewName) || !request.ViewName.Equals(context.GetPolicy<KnownCustomerViewsPolicy>().Details, StringComparison.OrdinalIgnoreCase) && !request.ViewName.Equals(context.GetPolicy<KnownCustomerViewsPolicy>().Master, StringComparison.OrdinalIgnoreCase))
                return entityView;
            if (request.ForAction.Equals(context.GetPolicy<KnownCustomerActionsPolicy>().AddCustomer, StringComparison.OrdinalIgnoreCase) && request.ViewName.Equals(context.GetPolicy<KnownCustomerViewsPolicy>().Details, StringComparison.OrdinalIgnoreCase))
            {
                await detailsViewBlock.PopulateDetails(entityView, (Customer)null, true, false, context);
                return entityView;
            }
            bool isEditAction = request.ForAction.Equals(context.GetPolicy<KnownCustomerActionsPolicy>().EditCustomer, StringComparison.OrdinalIgnoreCase);
            if (!(request.Entity is Customer) || !isEditAction && !string.IsNullOrEmpty(request.ForAction))
                return entityView;
            Customer entity = (Customer)request.Entity;
            EntityView view;
            if (request.ViewName.Equals(context.GetPolicy<KnownCustomerViewsPolicy>().Master, StringComparison.OrdinalIgnoreCase))
            {
                ViewProperty viewProperty1 = entityView.Properties.FirstOrDefault<ViewProperty>((Func<ViewProperty, bool>)(p => p.Name.Equals("Name", StringComparison.OrdinalIgnoreCase)));
                if (viewProperty1 != null)
                {
                    viewProperty1.RawValue = entity == null || string.IsNullOrEmpty(entity.FirstName) && string.IsNullOrEmpty(entity.LastName) ? (object)string.Empty : (string.IsNullOrEmpty(entity.FirstName) || string.IsNullOrEmpty(entity.LastName) ? (!string.IsNullOrEmpty(entity.FirstName) ? (object)entity.FirstName : (object)entity.LastName) : (object)string.Format("{0} {1}", (object)entity.FirstName, (object)entity.LastName));
                    ViewProperty viewProperty2 = entityView.Properties.FirstOrDefault<ViewProperty>((Func<ViewProperty, bool>)(p => p.Name.Equals("DisplayName", StringComparison.OrdinalIgnoreCase)));
                    if (viewProperty2 != null)
                    {
                        viewProperty2.RawValue = viewProperty1.RawValue;
                    }
                    else
                    {
                        List<ViewProperty> properties = entityView.Properties;
                        ViewProperty viewProperty3 = new ViewProperty();
                        viewProperty3.Name = "DisplayName";
                        viewProperty3.RawValue = viewProperty1.RawValue;
                        properties.Add(viewProperty3);
                    }
                }
                EntityView entityView1 = new EntityView();
                entityView1.EntityId = entityView.EntityId;
                entityView1.Name = context.GetPolicy<KnownCustomerViewsPolicy>().Details;
                view = entityView1;
                entityView.ChildViews.Add((Model)view);
            }
            else
                view = entityView;
            await detailsViewBlock.PopulateDetails(view, entity, false, isEditAction, context);
            //return null;
            return entityView;
        }

        protected virtual async Task PopulateDetails(EntityView view, Customer customer, bool isAddAction, bool isEditAction, CommercePipelineExecutionContext context)
        {
            if (view == null)
                return;
            ValidationPolicy validationPolicy = ValidationPolicy.GetValidationPolicy(context.CommerceContext, typeof(Customer));
            ValidationPolicy detailsValidationPolicy = ValidationPolicy.GetValidationPolicy(context.CommerceContext, typeof(CustomerDetailsComponent));
            CustomerPropertiesPolicy propertiesPolicy = context.GetPolicy<CustomerPropertiesPolicy>();
            EntityView details = (EntityView)null;
            if (customer != null && customer.HasComponent<CustomerDetailsComponent>())
                details = customer.GetComponent<CustomerDetailsComponent>().View.ChildViews.FirstOrDefault<Model>((Func<Model, bool>)(v => v.Name.Equals("Details", StringComparison.OrdinalIgnoreCase))) as EntityView;
            List<string> languages = new List<string>();
            Shop shop = context.CommerceContext.GetObjects<Shop>().FirstOrDefault<Shop>();
            if (shop != null && shop.Languages.Any<string>())
                languages = shop.Languages;
            foreach (string detailsProperty in propertiesPolicy?.DetailsProperties)
            {
                string propertyName = detailsProperty;
                if (!isAddAction || !propertyName.Equals(propertiesPolicy?.AccountNumber, StringComparison.OrdinalIgnoreCase))
                {
                    ValidationAttributes validationAttributes = validationPolicy.Models.FirstOrDefault<Model>((Func<Model, bool>)(m => m.Name.Equals(propertyName, StringComparison.OrdinalIgnoreCase))) as ValidationAttributes;
                    if (propertyName.Equals(propertiesPolicy?.AccountStatus, StringComparison.OrdinalIgnoreCase))
                    {
                        KnownCustomersStatusesPolicy statusesPolicy = context.GetPolicy<KnownCustomersStatusesPolicy>();
                        List<Selection> statuses = new List<Selection>();
                        string currentStatus = customer?.AccountStatus ?? string.Empty;
                        if (isAddAction | isEditAction)
                        {
                            PropertyInfo[] propertyInfoArray = typeof(KnownCustomersStatusesPolicy).GetProperties();
                            for (int index = 0; index < propertyInfoArray.Length; ++index)
                            {
                                PropertyInfo propertyInfo = propertyInfoArray[index];
                                if (!propertyInfo.Name.Equals("PolicyId", StringComparison.OrdinalIgnoreCase) && !propertyInfo.Name.Equals("Models", StringComparison.OrdinalIgnoreCase))
                                {
                                    string status = propertyInfo.GetValue((object)statusesPolicy, (object[])null) as string;
                                    if (!string.IsNullOrEmpty(status))
                                    {
                                        LocalizedTerm localizedTerm = await this._getLocalizedCustomerStatusPipeline.Run(new LocalizedCustomerStatusArgument(status, (object[])null), context);
                                        List<Selection> selectionList = statuses;
                                        Selection selection = new Selection();
                                        selection.DisplayName = localizedTerm?.Value;
                                        selection.Name = status;
                                        selectionList.Add(selection);
                                        status = (string)null;
                                    }
                                }
                            }
                            propertyInfoArray = (PropertyInfo[])null;
                        }
                        else if (!string.IsNullOrEmpty(currentStatus))
                        {
                            LocalizedTerm localizedTerm = await this._getLocalizedCustomerStatusPipeline.Run(new LocalizedCustomerStatusArgument(currentStatus, (object[])null), context);
                            if (!string.IsNullOrEmpty(localizedTerm?.Value))
                                currentStatus = localizedTerm?.Value;
                        }
                        List<ViewProperty> properties = view.Properties;
                        ViewProperty viewProperty = new ViewProperty();
                        viewProperty.Name = propertiesPolicy?.AccountStatus;
                        viewProperty.RawValue = (object)currentStatus;
                        viewProperty.IsReadOnly = !isAddAction && !isEditAction;
                        ValidationAttributes validationAttributes1 = validationAttributes;
                        viewProperty.IsRequired = validationAttributes1 != null && validationAttributes1.MinLength > 0;
                        viewProperty.Policies = (IList<Policy>)new List<Policy>()
            {
              (Policy) new AvailableSelectionsPolicy()
              {
                List = statuses
              }
            };
                        properties.Add(viewProperty);
                    }
                    else if (propertyName.Equals(propertiesPolicy?.LoginName, StringComparison.OrdinalIgnoreCase))
                    {
                        List<ViewProperty> properties = view.Properties;
                        ViewProperty viewProperty = new ViewProperty();
                        viewProperty.Name = propertiesPolicy?.LoginName;
                        viewProperty.RawValue = (object)(customer?.LoginName ?? string.Empty);
                        viewProperty.IsReadOnly = !isAddAction;
                        viewProperty.IsRequired = true;
                        List<Policy> policyList;
                        if (isAddAction)
                        {
                            ValidationAttributes validationAttributes1 = validationAttributes;
                            if ((validationAttributes1 != null ? (validationAttributes1.MaxLength > 0 ? 1 : 0) : 0) != 0)
                            {
                                policyList = new List<Policy>()
                                {
                                  (Policy) new MaxLengthPolicy()
                                  {
                                    MaxLengthAllow = validationAttributes.MaxLength
                                  }
                                };
                                goto label_28;
                            }
                        }
                        policyList = new List<Policy>();
                        label_28:
                        viewProperty.Policies = (IList<Policy>)policyList;
                        properties.Add(viewProperty);
                    }
                    else if (propertyName.Equals(propertiesPolicy?.Domain, StringComparison.OrdinalIgnoreCase))
                    {
                        List<ViewProperty> properties = view.Properties;
                        ViewProperty viewProperty = new ViewProperty();
                        viewProperty.Name = propertiesPolicy?.Domain;
                        viewProperty.RawValue = (object)(customer?.Domain ?? string.Empty);
                        viewProperty.IsReadOnly = !isAddAction;
                        viewProperty.IsRequired = true;
                        List<Policy> policyList;
                        if (!isAddAction)
                        {
                            policyList = new List<Policy>();
                        }
                        else
                        {
                            policyList = new List<Policy>();
                            AvailableSelectionsPolicy selectionsPolicy = new AvailableSelectionsPolicy();
                            List<Selection> selectionList;
                            if (propertiesPolicy?.Domains == null || !propertiesPolicy.Domains.Any<string>() || !(isAddAction | isEditAction))
                            {
                                selectionList = new List<Selection>();
                            }
                            else
                            {
                                CustomerPropertiesPolicy propertiesPolicy1 = propertiesPolicy;
                                selectionList = propertiesPolicy1 != null ? propertiesPolicy1.Domains.Select<string, Selection>((Func<string, Selection>)(s =>
                                {
                                    return new Selection()
                                    {
                                        DisplayName = s,
                                        Name = s
                                    };
                                })).ToList<Selection>() : (List<Selection>)null;
                            }
                            selectionsPolicy.List = selectionList;
                            policyList.Add((Policy)selectionsPolicy);
                        }
                        viewProperty.Policies = (IList<Policy>)policyList;
                        properties.Add(viewProperty);
                    }
                    else if (propertyName.Equals(propertiesPolicy?.UserName, StringComparison.OrdinalIgnoreCase))
                    {
                        if (!isAddAction)
                        {
                            List<ViewProperty> properties = view.Properties;
                            ViewProperty viewProperty = new ViewProperty();
                            viewProperty.Name = propertiesPolicy?.UserName;
                            viewProperty.RawValue = (object)(customer?.UserName ?? string.Empty);
                            viewProperty.IsReadOnly = !isAddAction;
                            viewProperty.IsRequired = true;
                            List<Policy> policyList;
                            if (isAddAction)
                            {
                                ValidationAttributes validationAttributes1 = validationAttributes;
                                if ((validationAttributes1 != null ? (validationAttributes1.MaxLength > 0 ? 1 : 0) : 0) != 0)
                                {
                                    policyList = new List<Policy>()
                                    {
                                      (Policy) new MaxLengthPolicy()
                                      {
                                        MaxLengthAllow = validationAttributes.MaxLength
                                      }
                                    };
                                    goto label_43;
                                }
                            }
                            policyList = new List<Policy>();
                            label_43:
                            viewProperty.Policies = (IList<Policy>)policyList;
                            properties.Add(viewProperty);
                        }
                    }
                    else if (propertyName.Equals(propertiesPolicy?.Language, StringComparison.OrdinalIgnoreCase))
                    {
                        object obj = details?.GetPropertyValue(propertiesPolicy?.Language) ?? (((languages == null ? 0 : (languages.Any<string>() ? 1 : 0)) & (isAddAction ? 1 : 0)) != 0 ? (object)languages.FirstOrDefault<string>() : (object)string.Empty);
                        List<ViewProperty> properties = view.Properties;
                        ViewProperty viewProperty = new ViewProperty();
                        viewProperty.Name = propertiesPolicy?.Language;
                        viewProperty.RawValue = obj ?? (object)string.Empty;
                        viewProperty.IsReadOnly = !isAddAction && !isEditAction;
                        ValidationAttributes validationAttributes1 = validationAttributes;
                        viewProperty.IsRequired = validationAttributes1 != null && validationAttributes1.MinLength > 0;
                        viewProperty.Policies = (IList<Policy>)new List<Policy>()
                        {
                          (Policy) new AvailableSelectionsPolicy()
                          {
                            List = (languages == null || !languages.Any<string>() || !(isAddAction | isEditAction) ? new List<Selection>() : languages.Select<string, Selection>((Func<string, Selection>) (s =>
                            {
                              return new Selection()
                              {
                                DisplayName = s,
                                Name = s
                              };
                            })).ToList<Selection>())
                          }
                        };
                        properties.Add(viewProperty);
                    }
                    else if (propertyName.Equals("IsCompany", StringComparison.OrdinalIgnoreCase))
                    {
                        ViewProperty viewProperty = _registrationHelper.SetCustomViewProperty(details, propertyName, !isAddAction, false.GetType(), true);
                        if (viewProperty != null)
                        {
                            view.Properties.Add(viewProperty);
                        }
                    }
                    else if (propertyName.Equals("ConsentRegulation", StringComparison.OrdinalIgnoreCase))
                    {
                        ViewProperty viewProperty = _registrationHelper.SetCustomViewProperty(details, propertyName, false, false.GetType(), true);
                        if (viewProperty != null)
                        {
                            view.Properties.Add(viewProperty);
                        }
                    }
                    else if (propertyName.Equals("ConsentProcessingContactData", StringComparison.OrdinalIgnoreCase))
                    {
                        ViewProperty viewProperty = _registrationHelper.SetCustomViewProperty(details, propertyName, false, false.GetType(), true);
                        if (viewProperty != null)
                        {
                            view.Properties.Add(viewProperty);
                        }
                    }
                    else if (propertyName.Equals("CanPurchase", StringComparison.OrdinalIgnoreCase))
                    {
                        ViewProperty viewProperty = _registrationHelper.SetCustomViewProperty(details, propertyName, false, false.GetType(), true);
                        if (viewProperty != null)
                        {
                            view.Properties.Add(viewProperty);
                        }
                    }
                    else if (propertyName.Equals("CanCreateOrders", StringComparison.OrdinalIgnoreCase))
                    {
                        ViewProperty viewProperty = _registrationHelper.SetCustomViewProperty(details, propertyName, false, false.GetType(), true);
                        if (viewProperty != null)
                        {
                            view.Properties.Add(viewProperty);
                        }
                    }
                    else if (propertyName.Equals("CanSeeDiscountPrices", StringComparison.OrdinalIgnoreCase))
                    {
                        ViewProperty viewProperty = _registrationHelper.SetCustomViewProperty(details, propertyName, false, false.GetType(), true);
                        if (viewProperty != null)
                        {
                            view.Properties.Add(viewProperty);
                        }
                    }
                    else
                    {
                        PropertyInfo property = typeof(Customer).GetProperty(propertyName);
                        if (property != (PropertyInfo)null)
                        {
                            List<ViewProperty> properties = view.Properties;
                            ViewProperty viewProperty = new ViewProperty();
                            viewProperty.Name = propertyName;
                            viewProperty.RawValue = customer == null ? (object)string.Empty : property.GetValue((object)customer, (object[])null);
                            viewProperty.IsReadOnly = !isAddAction && !isEditAction || propertyName.Equals(propertiesPolicy?.AccountNumber, StringComparison.OrdinalIgnoreCase);
                            ValidationAttributes validationAttributes1 = validationAttributes;
                            viewProperty.IsRequired = validationAttributes1 != null && validationAttributes1.MinLength > 0;
                            List<Policy> policyList;
                            if (isAddAction | isEditAction)
                            {
                                ValidationAttributes validationAttributes2 = validationAttributes;
                                if ((validationAttributes2 != null ? (validationAttributes2.MaxLength > 0 ? 1 : 0) : 0) != 0)
                                {
                                    policyList = new List<Policy>()
                  {
                    (Policy) new MaxLengthPolicy()
                    {
                      MaxLengthAllow = validationAttributes.MaxLength
                    }
                  };
                                    goto label_51;
                                }
                            }
                            policyList = new List<Policy>();
                            label_51:
                            viewProperty.Policies = (IList<Policy>)policyList;
                            properties.Add(viewProperty);
                        }
                        else
                        {
                            object propertyValue = details?.GetPropertyValue(propertyName);
                            ValidationAttributes validationAttributes1 = detailsValidationPolicy.Models.FirstOrDefault<Model>((Func<Model, bool>)(m => m.Name.Equals(propertyName, StringComparison.OrdinalIgnoreCase))) as ValidationAttributes;
                            List<ViewProperty> properties = view.Properties;
                            ViewProperty viewProperty = new ViewProperty();
                            viewProperty.Name = propertyName;
                            viewProperty.RawValue = propertyValue ?? (object)string.Empty;
                            viewProperty.IsReadOnly = !isAddAction && !isEditAction;
                            viewProperty.IsRequired = validationAttributes1 != null && validationAttributes1.MinLength > 0;
                            List<Policy> policyList;
                            if (!(isAddAction | isEditAction) || validationAttributes1 == null || validationAttributes1.MaxLength <= 0)
                                policyList = new List<Policy>();
                            else
                                policyList = new List<Policy>()
                {
                  (Policy) new MaxLengthPolicy()
                  {
                    MaxLengthAllow = validationAttributes1.MaxLength
                  }
                };
                            viewProperty.Policies = (IList<Policy>)policyList;
                            properties.Add(viewProperty);
                            validationAttributes = (ValidationAttributes)null;
                        }
                    }
                }
            }
            List<string> stringList1 = new List<string>();
            if (customer?.Tags != null && customer.Tags.Any<Tag>())
            {
                List<string> stringList2 = stringList1;
                Customer customer1 = customer;
                IEnumerable<string> collection = (customer1 != null ? customer1.Tags.Where<Tag>((Func<Tag, bool>)(t => !t.Excluded)) : (IEnumerable<Tag>)null).Select<Tag, string>((Func<Tag, string>)(tag => tag.Name));
                stringList2.AddRange(collection);
            }
            if (isAddAction)
                return;
            List<ViewProperty> properties1 = view.Properties;
            ViewProperty viewProperty1 = new ViewProperty();
            viewProperty1.Name = "IncludedTags";
            viewProperty1.RawValue = (object)stringList1.ToArray();
            viewProperty1.IsReadOnly = !isAddAction && !isEditAction;
            viewProperty1.IsRequired = false;
            viewProperty1.Policies = (IList<Policy>)new List<Policy>();
            viewProperty1.UiType = isEditAction ? "Tags" : "List";
            viewProperty1.OriginalType = "List";
            properties1.Add(viewProperty1);
        }
    }
}