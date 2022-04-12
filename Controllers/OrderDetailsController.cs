using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DI_probni.Data;
using DI_probni.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;

namespace DI_probni.Controllers
{
    public class OrderDetailsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;
        public const string OrderSessionKey = "OrderId";
        public OrderDetailsController(ApplicationDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        [Authorize]
        // GET: OrderDetails
        public async Task<IActionResult> Index()
        {
            var orderId = GetOrdertId();
            if (orderId == null)
            {
                return RedirectToAction("Index", "Products");
            } 
            var currentUser = _userManager.GetUserId(User);
           
            var applicationDbContext = _context.OrderDetails
                .Include(p => p.Product)
                .Include(o => o.Order)
                .Where(x => (x.OrderId == orderId) &&
                            (x.Order.Final == false) &&
                            (x.Order.UserId == currentUser)); //&& (x.Order.OrderedOn==DateTime.Now.Date)

            return View(await applicationDbContext.ToListAsync());
        }
        //{
        //    var applicationDbContext = _context.OrderDetails
        //        .Include(o => o.Product);
        //    return View(await applicationDbContext.ToListAsync());
        //}
        public async Task<IActionResult> Calculate(int orderId)
        {
            var currentUser = _userManager.GetUserId(User);
            var dbOrderList = _context.OrderDetails
               .Include(p => p.Product)
               .Include(o => o.Order)
               .Where(x => (x.OrderId == orderId) &&
                           (x.Order.Final == false) &&
                           (x.Order.UserId == currentUser));
            decimal sum = 0;
            foreach (var item in dbOrderList)
            {
                sum += (item.Product.Price * item.Quantity);
            }
            //започва актуализиране на таблицата Orders /total=....; final=true
            Order order = await _context.Orders.FindAsync(orderId);
            if (order==null)
            {
                return NotFound();
            }
            order.Final = true;
            order.Total = sum;

            _context.Orders.Update(order);
            await _context.SaveChangesAsync();
            //изтрива ОРДЕРид от сесията
            HttpContext.Session.Remove("OrderSessionKey");

            return Content("SUM = " + sum.ToString());
            //return View(await applicationDbContext.ToListAsync());
        }
        // GET: OrderDetails/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var orderDetails = await _context.OrderDetails
                .Include(o => o.Product)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (orderDetails == null)
            {
                return NotFound();
            }

            return View(orderDetails);
        }

        //метод за взимане на информацията от сесията за потребителя
        public int? GetOrdertId()
        {
            return HttpContext.Session.GetInt32("OrderSessionKey");
        }
        []
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductVM product)
        {
            if (!ModelState.IsValid) //при грешка в модела
            {
                ViewData["ProductId"] = new SelectList(_context.Products, "Id", "Id");
                return View();
            }

            if (GetOrdertId() == null) //Ако потребителят няма поръчка досега от влизането си!!
            {
                Order order = new Order()
                {
                    UserId = _userManager.GetUserId(User),
                    OrderedOn = DateTime.Now.Date
                };
                _context.Orders.Add(order);
                await _context.SaveChangesAsync();
                HttpContext.Session.SetInt32("OrderSessionKey", order.Id);// Създавам запис в СЕСИЯТА за потребителя с Номер поръчка
            }

            //Ако потребителят ВЕЧЕ е направил поръчка!!
            int shoppingCardId = (int)GetOrdertId();
            var orderItem = await _context.OrderDetails
                .SingleOrDefaultAsync(x => (x.ProductId == product.Id && x.OrderId == shoppingCardId));
            if (orderItem == null) //Ако поръчва друг/нов продукт се записва в OrderDetails
            {
                orderItem = new OrderDetails()
                {
                    ProductId = product.Id,
                    Quantity = product.Quantity,
                    OrderId = (int)GetOrdertId()
                };
                _context.OrderDetails.Add(orderItem);
            }
            else //ако избира поръчан вече продукт се увеличава количеството му
            {
                orderItem.Quantity = orderItem.Quantity + product.Quantity;
                _context.OrderDetails.Update(orderItem);
            }
            await _context.SaveChangesAsync();
            //return Content("OK");
            return RedirectToAction("Index", "Products"); //??? къде да се върнем?
        }

        // POST: OrderDetails/Create>>>>>>> СТАРАТА версия от ГИТ-а
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Create([Bind("Id,OrderId,ProductId,Quantity")] ProductOrderVM orderproduct)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        ViewData["ProductId"] = new SelectList(_context.Products, "Id", "Id", orderproduct.ProductId);
        //        return View(orderproduct);
        //    }
        //    Order order = new Order()
        //    {
        //        UserId = _userManager.GetUserId(User),
        //        OrderedOn = DateTime.Now
        //    };
        //    _context.Orders.Add(order);
        //    await _context.SaveChangesAsync();

        //    OrderDetails modelToDb = new OrderDetails()
        //    {
        //        ProductId = orderproduct.ProductId,
        //        Quantity = orderproduct.Quantity,
        //        OrderId = order.Id
        //    };
        //    _context.OrderDetails.Add(modelToDb);
        //    await _context.SaveChangesAsync();

        //    return Content("OK");
        //    // return RedirectToAction(nameof(Index));

        //}

        // GET: OrderDetails/Edit/5
        //public async Task<IActionResult> Edit(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var orderDetails = await _context.OrderDetails.FindAsync(id);
        //    if (orderDetails == null)
        //    {
        //        return NotFound();
        //    }
        //    ViewData["ProductId"] = new SelectList(_context.Products, "Id", "Id", orderDetails.ProductId);
        //    return View(orderDetails);
        //}

        //// POST: OrderDetails/Edit/5
        //// To protect from overposting attacks, enable the specific properties you want to bind to.
        //// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Edit(int id, [Bind("Id,OrderId,ProductId,Quantity")] OrderDetails orderDetails)
        //{
        //    if (id != orderDetails.Id)
        //    {
        //        return NotFound();
        //    }

        //    if (ModelState.IsValid)
        //    {
        //        try
        //        {
        //            _context.Update(orderDetails);
        //            await _context.SaveChangesAsync();
        //        }
        //        catch (DbUpdateConcurrencyException)
        //        {
        //            if (!OrderDetailsExists(orderDetails.Id))
        //            {
        //                return NotFound();
        //            }
        //            else
        //            {
        //                throw;
        //            }
        //        }
        //        return RedirectToAction(nameof(Index));
        //    }
        //    ViewData["ProductId"] = new SelectList(_context.Products, "Id", "Id", orderDetails.ProductId);
        //    return View(orderDetails);
        //}

        // GET: OrderDetails/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var orderDetails = await _context.OrderDetails
                .Include(o => o.Product)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (orderDetails == null)
            {
                return NotFound();
            }

            return View(orderDetails);
        }

        // POST: OrderDetails/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var orderDetails = await _context.OrderDetails.FindAsync(id);
            _context.OrderDetails.Remove(orderDetails);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool OrderDetailsExists(int id)
        {
            return _context.OrderDetails.Any(e => e.Id == id);
        }
    }
}
