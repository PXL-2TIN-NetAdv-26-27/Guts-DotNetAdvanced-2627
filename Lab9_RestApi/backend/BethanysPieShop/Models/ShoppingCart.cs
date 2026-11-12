using Microsoft.EntityFrameworkCore;
namespace BethanysPieShop.Models;
public class ShoppingCart : IShoppingCart
{
    private readonly BethanysPieShopDbContext _context;
    private readonly string? _cartId;

    private ShoppingCart(BethanysPieShopDbContext context, string cartId)
    {
        _cartId = cartId;
        _context = context;
    }

    public static ShoppingCart GetCart(ISession? session, BethanysPieShopDbContext context)
    {
        string? cartId = session?.GetString("CartId"); // If no cartId in session, create a new one
        if (cartId is null)
        {
            cartId = Guid.NewGuid().ToString();
            session?.SetString("CartId", cartId); // Save cartId in session
        }

        return new ShoppingCart(context, cartId);
    }

    public void AddToCart(Pie pie)
    {
        var shoppingCartItem =
            _context.ShoppingCartItems.SingleOrDefault(
                s => s.Pie.PieId == pie.PieId && s.ShoppingCartId == _cartId);

        if (shoppingCartItem is null)
        {
            shoppingCartItem = new ShoppingCartItem
            {
                ShoppingCartId = _cartId,
                Pie = pie,
                Amount = 1
            };

            _context.ShoppingCartItems.Add(shoppingCartItem);
        }
        else
        {
            shoppingCartItem.Amount++;
        }
        _context.SaveChanges();
    }

    public int RemoveFromCart(Pie pie)
    {
        var shoppingCartItem =
            _context.ShoppingCartItems.SingleOrDefault(
                s => s.Pie.PieId == pie.PieId && s.ShoppingCartId == _cartId);

        var localAmount = 0;

        if (shoppingCartItem != null)
        {
            if (shoppingCartItem.Amount > 1)
            {
                shoppingCartItem.Amount--;
                localAmount = shoppingCartItem.Amount;
            }
            else
            {
                _context.ShoppingCartItems.Remove(shoppingCartItem);
            }
        }

        _context.SaveChanges();

        return localAmount;
    }

    public List<ShoppingCartItem> GetShoppingCartItems()
    {
        return _context.ShoppingCartItems.Where(c => c.ShoppingCartId == _cartId)
            .Include(s => s.Pie)
            .ToList();
    }

    public void ClearCart()
    {
        var cartItems = _context
            .ShoppingCartItems
            .Where(cart => cart.ShoppingCartId == _cartId);

        _context.ShoppingCartItems.RemoveRange(cartItems);

        _context.SaveChanges();
    }

    public decimal GetShoppingCartTotal()
    {
        decimal total = _context.ShoppingCartItems.Where(c => c.ShoppingCartId == _cartId)
            .Select(c => c.Pie.Price * c.Amount).Sum(); 
        return total;
    }
}