using InMa.ShoppingList.Models;

namespace InMa.ShoppingList.Components.Pages;

public partial class Lists
{
    private IList<ShoppingListItem> Items { get; set; }
    
    private string NewProductName { get; set; }
    private bool AddingProduct { get; set; } = false;

    public Lists()
    {
        Items = new List<ShoppingListItem>()
        {
            new ShoppingListItem("Bread"),
            new ShoppingListItem("CocaCola")
        };
    }

    async Task AddNewProduct()
    {
        try
        {
            AddingProduct = true;
            
            if (string.IsNullOrWhiteSpace(NewProductName))
            {
                return;
            }

            if (Items.Any(i => i.Product.Equals(NewProductName, StringComparison.OrdinalIgnoreCase)))
            {
                return;
            }

            Items.Add(new ShoppingListItem(NewProductName));
        }
        finally
        {
            AddingProduct = false;
        }
    }
}