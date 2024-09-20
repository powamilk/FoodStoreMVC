using FoodStoreMVC.Models;
using FoodStoreMVC.Service.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace FoodStoreMVC.Controllers
{
    public class OrderController : Controller
    {
        private readonly AppDbContext _context;

        public OrderController(AppDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            var orders = await _context.Orders
                                       .Include(o => o.OrderItems)
                                       .ThenInclude(oi => oi.Product)
                                       .ToListAsync();
            return View(orders);
        }
        public IActionResult Create()
        {
            ViewBag.Products = new SelectList(_context.Products, "Id", "Name");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Order order, List<OrderItem> orderItems)
        {
            if (ModelState.IsValid)
            {
                decimal totalAmount = 0;

                foreach (var item in orderItems)
                {
                    var product = await _context.Products.FindAsync(item.ProductId);
                    if (product != null)
                    {
                        item.Product = product;
                        item.Order = order;
                        totalAmount += item.Quantity * product.Price;
                    }
                }

                order.TotalAmount = totalAmount;
                order.OrderItems = orderItems;

                _context.Orders.Add(order);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Products = new SelectList(_context.Products, "Id", "Name");
            return View(order);
        }
        public async Task<IActionResult> Edit(int id)
        {
            var order = await _context.Orders
                                      .Include(o => o.OrderItems)
                                      .FirstOrDefaultAsync(o => o.Id == id);
            if (order == null)
            {
                return NotFound();
            }

            ViewBag.Products = new SelectList(_context.Products, "Id", "Name");
            return View(order);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, Order order, List<OrderItem> orderItems)
        {
            if (ModelState.IsValid)
            {
                var existingOrder = await _context.Orders
                    .Include(o => o.OrderItems)
                    .FirstOrDefaultAsync(o => o.Id == id);

                if (existingOrder == null) return NotFound();

                _context.OrderItems.RemoveRange(existingOrder.OrderItems);

                decimal totalAmount = 0;

                foreach (var item in orderItems)
                {
                    var product = await _context.Products.FindAsync(item.ProductId);
                    if (product != null)
                    {
                        item.Product = product;
                        item.Order = existingOrder;
                        totalAmount += item.Quantity * product.Price;
                    }
                }

                existingOrder.OrderItems = orderItems;
                existingOrder.TotalAmount = totalAmount;

                _context.Orders.Update(existingOrder);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            ViewBag.Products = new SelectList(_context.Products, "Id", "Name");
            return View(order);
        }
        public async Task<IActionResult> Delete(int id)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order == null)
            {
                return NotFound();
            }

            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
