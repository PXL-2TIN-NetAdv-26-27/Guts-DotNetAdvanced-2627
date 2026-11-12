using BethanysPieShop.Models;
using BethanysPieShop.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace BethanysPieShop.Components;

public class ShoppingCartSummary: ViewComponent
{
    private readonly IShoppingCart _shoppingCart;

    public ShoppingCartSummary(IShoppingCart shoppingCart)
    {
        _shoppingCart = shoppingCart;
    }

    public IViewComponentResult Invoke()
    {
        var shoppingCartViewModel = new ShoppingCartViewModel(
            _shoppingCart.GetShoppingCartItems(),
            _shoppingCart.GetShoppingCartTotal());

        return View(shoppingCartViewModel);
    }
}