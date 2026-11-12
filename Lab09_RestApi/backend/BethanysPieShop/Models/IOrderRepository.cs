namespace BethanysPieShop.Models;

public interface IOrderRepository
{
    void CreateOrder(Order order);
    List<Order> GetAllOrders();
    Order? GetOrderById(int id);
    void UpdateOrder(Order order);
}