using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Inleveropdracht.Models;
using Inleveropdracht.Data;
using System.Linq;
using System;

namespace Inleveropdracht.Controllers
{
    public class OrdersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public OrdersController(ApplicationDbContext context)
        {
            _context = context;
        }
        public IActionResult Orders(int customerId, string selectedItems)
        {
            if (customerId <= 0)
            {
                return RedirectToAction("Login");
            }

            List<int> selectedItemIds = selectedItems.Split(',').Select(int.Parse).ToList();

            // Create a dictionary to store the counts of each ID
            Dictionary<int, int> idCounts = new Dictionary<int, int>();

            // Count the occurrences of each ID in the selected items
            foreach (int id in selectedItemIds)
            {
                if (idCounts.ContainsKey(id))
                {
                    idCounts[id]++;
                }
                else
                {
                    idCounts[id] = 1;
                }
            }

            // Query the product names based on the selected item IDs
            var unfinishedOrderIds = _context.Orders
                .Where(o => o.CustomerId == customerId && !o.IsFinished)
                .ToList();

            var selectedProducts = _context.Products
                .Where(p => selectedItemIds.Contains(p.ProductId))
                .ToList();


            decimal totalPrice = selectedProducts.Sum(p => p.Price * idCounts.GetValueOrDefault(p.ProductId, 0));

            var model = new OrderViewModel
            {
                CustomerId = customerId,
                Products = selectedProducts,
                TotalPrice = totalPrice,
                IdCounts = idCounts,
            };

            return View(model);
        }

        [HttpPost]
        public IActionResult BevestigBestelling(int klantId, string geselecteerdeItems)
        {
            List<int> selectedItemIds = geselecteerdeItems.Split(',').Select(int.Parse).ToList();

            // Create a dictionary to store the counts of each ID
            Dictionary<int, int> idCounts = new Dictionary<int, int>();

            // Count the occurrences of each ID in the selected items
            foreach (int id in selectedItemIds)
            {
                if (idCounts.ContainsKey(id))
                {
                    idCounts[id]++;
                }
                else
                {
                    idCounts[id] = 1;
                }
            }

            // Create a new Order instance
            var order = new Order
            {
                OrderDate = DateTime.Now,
                CustomerId = klantId
            };

            // Create OrderItems based on the selected product IDs and their counts
            foreach (var productId in idCounts.Keys)
            {
                var orderItem = new OrderItem
                {
                    ProductId = productId,
                    Quantity = idCounts[productId]
                };

                order.OrderItems.Add(orderItem);
            }

            // Add the order to the database
            _context.Orders.Add(order);
            _context.SaveChanges();

            var orderId = order.OrderId; // Get the OrderID after it's saved

            var result = new { success = true, OrderId = orderId };
            return Json(result);
        }




        public IActionResult PlaceOrderItem()
        {
            List<Order> orders = _context.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .ToList();

            return View(orders);
        }



        public IActionResult PlaceOrder()
        {
            var products = _context.Products.ToList();
            var orderItems = products.Select(product => new OrderItemViewModel
            {
                ProductId = product.ProductId,
                ProductDescription = product.ProductDescription,
                ProductImage = product.ProductImage,
                ProductName = product.ProductName,
                Price = product.Price,
                SalePercentage = product.SalePercentage,
            });

            // Return the view with the correct model type
            return View(orderItems.ToList()); 
        }

        public IActionResult PlaceOrderAdmin()
        {
            var products = _context.Products.ToList();
            var orderItems = products.Select(product => new OrderItemViewModel
            {
                ProductId = product.ProductId,
                ProductDescription = product.ProductDescription,
                ProductName = product.ProductName,
                Price = product.Price,
                SalePercentage = product.SalePercentage,
            });

            // Return the view with the correct model type
            return View(orderItems.ToList());
        }

        public IActionResult CreateProduct()
        {
            return View();
        }

        [HttpPost]
        public IActionResult CreateProduct(OrderItemViewModel orderItem)
        {
            if (ModelState.IsValid)
            {
                var product = new Product
                {
                    ProductName = orderItem.ProductName,
                    ProductDescription = orderItem.ProductDescription,
                    ProductImage = orderItem.ProductImage,
                    Price = orderItem.Price,
                };

                _context.Products.Add(product);
                _context.SaveChanges();

                return RedirectToAction("PlaceOrderAdmin");
            }


            return View(orderItem);
        }

        [HttpPost]
        public IActionResult DeleteProduct(int productId)
        {
            var product = _context.Products.Find(productId);

            if (product == null)
            {
                return RedirectToAction("PlaceOrder");
            }

            _context.Products.Remove(product);
            _context.SaveChanges();

            return RedirectToAction("PlaceOrder");
        }

        [HttpPost]
        public IActionResult CompleteOrder(int OrderId)
        {
            var order = _context.Orders.Find(OrderId);

            if (order == null)
            {
                return RedirectToAction("PlaceOrderItem");
            }

            order.IsFinished = true; // Mark the order as finished
            order.FinishedAt = DateTime.UtcNow; // Set the timestamp

            _context.SaveChanges();

            return RedirectToAction("PlaceOrderItem");
        }


        // Display the form for editing an existing product
        [HttpGet]
        public IActionResult EditProduct(int id)
        {
            // Find the product by its ID
            var product = _context.Products.FirstOrDefault(p => p.ProductId == id);

            if (product == null)
            {
                return NotFound(); // Product not found, return a 404 Not Found response
            }

            // Create a view model for editing the product
            var orderItem = new OrderItemViewModel
            {
                ProductName = product.ProductName,
                ProductDescription = product.ProductDescription,
                ProductImage = product.ProductImage,
                Price = product.Price,
                SalePercentage = product.SalePercentage
            };

            return View(orderItem);
        }

        // Handle the form submission for updating the product
        [HttpPost]
        public IActionResult EditProduct(int id, OrderItemViewModel orderItem)
        {
            if (ModelState.IsValid)
            {
                // Find the product by its ID
                var product = _context.Products.FirstOrDefault(p => p.ProductId == id);

                if (product == null)
                {
                    return NotFound(); // Product not found, return a 404 Not Found response
                }

                // Update the product properties with the new values from the form
                product.ProductName = orderItem.ProductName;
                product.ProductDescription = orderItem.ProductDescription;
                product.Price = orderItem.Price;
                product.ProductImage = orderItem.ProductImage;
                product.SalePercentage = orderItem.SalePercentage;

                _context.SaveChanges();

                return RedirectToAction("PlaceOrderAdmin");
            }

            return View(orderItem);
        }





    }

}
