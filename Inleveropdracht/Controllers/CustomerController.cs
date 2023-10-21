using Microsoft.AspNetCore.Mvc;
using Inleveropdracht.Data;
using Inleveropdracht.Models;
using Microsoft.AspNetCore.Identity;
using System.Diagnostics;
using Microsoft.EntityFrameworkCore;

namespace Inleveropdracht.Controllers
{
    public class CustomerController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;

        public CustomerController(ApplicationDbContext context, UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public IActionResult Customer()
        {
            var customers = _context.Customers.ToList();
            return View(customers);
        }

        public IActionResult Registratie()
        {
            return View();
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(Customer loginModel)
        {
            // Authenticate the user (match email and password with the database).
            var customer = _context.Customers.FirstOrDefault(c => c.Email == loginModel.Email && c.CustomerPassword == loginModel.CustomerPassword);

            if (customer != null)
            {
                bool isAdmin = customer.IsAdminClass;
                // Create a cookie with the CustomerId.
                var options = new CookieOptions
                {
                    Expires = DateTime.Now.AddHours(1), // Set the expiration time for the cookie
                    IsEssential = true, // This is necessary for the cookie to be sent on every request
                };

                Response.Cookies.Append("CustomerId", customer.CustomerId.ToString(), options);

                ViewBag.IsAdmin = isAdmin;

                return RedirectToAction("Customer");
            }
            else
            {
                // Handle login failure.
                ModelState.AddModelError(string.Empty, "Invalid email or password.");
                return View("Login", loginModel);
            }
        }

        [HttpGet] // Change this to the appropriate HTTP verb (e.g., [HttpGet])
        public IActionResult CheckIsAdmin()
        {
            if (Request.Cookies.TryGetValue("CustomerId", out var customerIdCookie))
            {
                if (int.TryParse(customerIdCookie, out int customerId))
                {
                    // Use the customerId to query the database and get the IsAdminClass value
                    var isAdminClass = _context.Customers
                        .Where(c => c.CustomerId == customerId)
                        .Select(c => c.IsAdminClass)
                        .FirstOrDefault();

                    return Json(new { isAdmin = isAdminClass });
                }
            }

            // If the customerId is not found in the cookie or there is an error, return an appropriate response
            return Json(new { isAdmin = false });
        }
        public IActionResult Profiel()
        {
            // Check if the "CustomerId" cookie exists
            if (Request.Cookies.TryGetValue("CustomerId", out var customerIdCookie))
            {
                if (int.TryParse(customerIdCookie, out int customerId))
                {
                    // Use the customerId to query the database for the user's information
                    var user = _context.Customers.FirstOrDefault(c => c.CustomerId == customerId);

                    if (user != null)
                    {
                        // Fetch the customer's orders, including finished ones
                        user.Orders = _context.Orders
                            .Include(o => o.OrderItems)
                            .ThenInclude(oi => oi.Product)
                            .Where(o => o.CustomerId == customerId)
                            .ToList();

                        // Calculate points based on the number of orders and items
                        int totalPoints = user.Orders.Count * 10; // You can adjust the points calculation as needed

                        user.Points = totalPoints;

                        // Pass the user's information, including their points, to the view
                        return View(user);
                    }
                }
            }

            // Handle the case when the user is not authenticated or their information is not found
            // For example, you can redirect them to the login page or display an error message.
            return RedirectToAction("Login");
        }




        public IActionResult CustomerView()
        {
            if (Request.Cookies.TryGetValue("CustomerId", out var customerId))
            {
                // Convert the customerId from the cookie to an integer (assuming it's an integer).
                if (int.TryParse(customerId, out int customerIdInt))
                {
                    // Retrieve customer data based on customerIdInt.
                    var customer = _context.Customers.FirstOrDefault(c => c.CustomerId == customerIdInt);

                    if (customer != null)
                    {
                        return View("Customer", customer);
                    }
                }
            }

            // Handle cases where the user is not logged in.
            return View("NotLoggedIn");
        }

        public IActionResult Logout()
        {
            Response.Cookies.Delete("CustomerId");
            return RedirectToAction("Login");
        }

        [HttpPost]
        public IActionResult UpdateAdminStatus(int customerId, bool isAdmin)
        {
            // Find the customer by ID in your data source
            var customer = _context.Customers.FirstOrDefault(c => c.CustomerId == customerId);

            if (customer != null)
            {
                // Update the IsAdminClass property
                customer.IsAdminClass = isAdmin;
                _context.SaveChanges();

                return Json(new { success = true });
            }

            // If the customer is not found, return an error
            return Json(new { success = false, error = "Customer not found." });
        }






    [HttpPost]
        [Route("Customer/CreateCustomer")] // Specify a unique route for this action
        public IActionResult CreateCustomer(Customer customer)
        {
            if (!ModelState.IsValid)
            {
                return View(customer);
            }

            _context.Customers.Add(customer);
            _context.SaveChanges();

            return RedirectToAction("Customer");
        }

        [HttpPost]
        public IActionResult Registreren(Customer customer)
        {
            if (!ModelState.IsValid)
            {
                return View(customer);
            }

            _context.Customers.Add(customer);
            _context.SaveChanges();

            return RedirectToAction("Login");
        }


        [HttpGet]
        public IActionResult EditCustomer(int id)
        {
            var customer = _context.Customers.FirstOrDefault(c => c.CustomerId == id);

            if (customer == null)
            {
                return NotFound();
            }

            var customerViewModel = new OrderViewModel
            {
                CustomerName = customer.CustomerName,
                Email = customer.Email,
                PhoneNumber = customer.PhoneNumber,
                Address = customer.Address
            };

            // Return the _EditCustomer partial view
            return PartialView("_EditCustomer", customerViewModel);
        }



        [HttpPost]
        public IActionResult EditCustomer(int id, OrderViewModel customerViewModel)
        {
            if (ModelState.IsValid)
            {
                // Find the customer by ID
                var customer = _context.Customers.FirstOrDefault(c => c.CustomerId == id);

                if (customer == null)
                {
                    return NotFound(); // Customer not found, return a 404 Not Found response
                }

                // Update the customer properties with the new values from the form
                customer.CustomerName = customerViewModel.CustomerName;
                customer.Email = customerViewModel.Email;
                customer.PhoneNumber = customerViewModel.PhoneNumber;
                customer.Address = customerViewModel.Address;

                _context.SaveChanges();

                // Redirect to a page where you want to go after successfully saving changes
                return RedirectToAction("Customer");
            }

            // If the model state is not valid, return the view with validation errors
            return View(customerViewModel);
        }



        public IActionResult DeleteCustomer(int CustomerId)
        {
            // Log the value of CustomerId to ensure it's correct
            Console.WriteLine("CustomerId: " + CustomerId);

            var customer = _context.Customers.FirstOrDefault(c => c.CustomerId == CustomerId);

            // Log the value of customer to check if it's null
            if (customer == null)
            {
                Console.WriteLine("Customer is null");
            }

            _context.Customers.Remove(customer);
            _context.SaveChanges();

            return RedirectToAction("Customer");
        }

        public IActionResult Registration()
        {
            return View();
        }
    }
}
