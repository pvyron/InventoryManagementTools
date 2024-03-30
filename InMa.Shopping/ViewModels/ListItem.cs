namespace InMa.Shopping.ViewModels;

public sealed class ListItem(string product)
{
    public bool? Bought { get; set; }
    public string Product { get; set; } = product;

    public void Deconstruct(out string product, out bool bought)
    {
        product = Product;
        bought = Bought.GetValueOrDefault(false);
    }

}