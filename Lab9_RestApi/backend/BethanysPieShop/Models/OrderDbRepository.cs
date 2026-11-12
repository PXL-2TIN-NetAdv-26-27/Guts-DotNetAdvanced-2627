namespace BethanysPieShop.Models;

public class OrderDbRepository : IOrderRepository
{
    private readonly BethanysPieShopDbContext _context;
    private readonly IShoppingCart _shoppingCart;

    public OrderDbRepository(BethanysPieShopDbContext context, IShoppingCart shoppingCart)
    {
        _context = context;
        _shoppingCart = shoppingCart;
    }

    public void CreateOrder(Order order)
    {
        order.OrderPlaced = DateTime.Now;

        List<ShoppingCartItem>? shoppingCartItems = _shoppingCart.GetShoppingCartItems();
        order.OrderTotal = _shoppingCart.GetShoppingCartTotal();

        order.OrderDetails = new List<OrderDetail>();

        foreach (ShoppingCartItem? shoppingCartItem in shoppingCartItems)
        {
            var orderDetail = new OrderDetail
            {
                Amount = shoppingCartItem.Amount,
                PieId = shoppingCartItem.Pie.PieId,
                Price = shoppingCartItem.Pie.Price
            };

            order.OrderDetails.Add(orderDetail);
        }

        _context.Orders.Add(order);

        _context.SaveChanges();
    }

    public List<Order> GetAllOrders()
    {
        throw new NotImplementedException();
    }

    public Order? GetOrderById(int orderId)
    {
        throw new NotImplementedException();
    }

    public void UpdateOrder(Order order)
    {
        throw new NotImplementedException();
    }
}