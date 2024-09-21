using FoodStoreMVC.Models;

namespace FoodStoreMVC.Service.Interface
{
    public interface IOrderService
    {
        Task<List<Order>> GetAllOrdersAsync();   
        Task<Order> GetOrderByIdAsync(int id);   
        Task<bool> CreateOrderAsync(Order order); 
        Task<bool> UpdateOrderAsync(Order order); 
        Task<bool> DeleteOrderAsync(int id);
    }
}
