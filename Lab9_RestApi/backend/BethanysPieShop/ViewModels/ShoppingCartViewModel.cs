using BethanysPieShop.Models;

namespace BethanysPieShop.ViewModels;

public class ShoppingCartViewModel
{
    public IEnumerable<ShoppingCartItem> ShoppingCartItems { get; }
    public decimal ShoppingCartTotal { get; }

    public ShoppingCartViewModel(IEnumerable<ShoppingCartItem> shoppingCartItems, decimal shoppingCartTotal)
    {
        ShoppingCartItems = shoppingCartItems;
        ShoppingCartTotal = shoppingCartTotal;
    }
}