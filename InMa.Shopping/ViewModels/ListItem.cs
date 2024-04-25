namespace InMa.Shopping.ViewModels;

public sealed class ListItem(string product)
{
    public bool IsBought { get; set; }
    public string Product { get; set; } = product;

    public void Deconstruct(out string product, out bool isBought)
    {
        product = Product;
        isBought = IsBought;
    }

}