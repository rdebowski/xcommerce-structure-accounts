using System.Linq;

using Sitecore.Commerce.EntityViews;
using Sitecore.Commerce.Plugin.Customers;

namespace XCommerce.StructuredAccounts.Models
{
    public class SubaccountModel
    {
        public SubaccountModel()
        {

        }

        public SubaccountModel(Customer customer)
        {
            Id = customer.Id;
            FirstName = customer.FirstName;
            LastName = customer.LastName;
            Email = customer.Email;

            var customerDetailsComponent = customer.GetComponent<CustomerDetailsComponent>();
            var childViews = customerDetailsComponent.View.ChildViews.FirstOrDefault(x => x.Name == "Details") as EntityView;
            var canPurchase = childViews.Properties.FirstOrDefault(x => x.Name == "CanPurchase").Value;
            var canCreateOrders = childViews.Properties.FirstOrDefault(x => x.Name == "CanCreateOrders").Value;
            MainaccountId = childViews.Properties.FirstOrDefault(x => x.Name == "MainAccountId").Value;
            CanPurchase = bool.Parse(canPurchase);
            CanCreateOrders = bool.Parse(canCreateOrders);
            
            IsActive = customer.AccountStatus == "ActiveAccount";
        }
        public string MainaccountId { get; set; }
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public bool CanCreateOrders { get; set; }
        public bool CanPurchase { get; set; }
        public bool IsActive { get; set; }
    }
}
