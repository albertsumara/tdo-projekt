using CarRentalApp.Areas.Identity.Data;
using CarRentalApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace CarRentalApp.Controllers
{
    public class ReservationController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly AppDbContext _context;
        public ReservationController(AppDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: ReservationController
        public ActionResult Index()
        {
            var reservations = _context.Reservations
            .Include(r => r.Car)  
            .Include(r => r.User) 
            .ToList();

            return View(reservations);
        }

        // GET: ReservationController/Details/5
        public ActionResult Details(int id)
        {
            var reservation = _context.Reservations
            .Include(r => r.Car)  
            .Include(r => r.User) 
            .FirstOrDefault(r => r.Id == id);

            if (reservation == null)
            {
                return NotFound();
            }

            return View(reservation);
        }

        [Authorize]
        public IActionResult Create(int? carId)
        {
            ViewBag.Cars = new SelectList(_context.Car.ToList(), "Id", "FullName");

            List<string> blockedDates = new List<string>();

            if (carId.HasValue)
            {
                var reservations = _context.Reservations
                    .Where(r => r.CarId == carId.Value)
                    .Select(r => new { r.StartDate, r.EndDate })
                    .ToList();

                foreach (var r in reservations)
                {
                    for (var date = r.StartDate.Date; date <= r.EndDate.Date; date = date.AddDays(1))
                    {
                        blockedDates.Add(date.ToString("yyyy-MM-dd"));
                    }
                }
            }

            ViewBag.BlockedDates = blockedDates;

            return View();
        }



        // POST: ReservationController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public IActionResult Create(Reservation model)
        {

            model.UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var existingReservations = _context.Reservations
                .Where(r => r.CarId == model.CarId)
                .ToList();

            foreach (var r in existingReservations)
            {
                if (model.StartDate.Date <= r.EndDate.Date && model.EndDate.Date >= r.StartDate.Date)
                {
                    ModelState.AddModelError("", "Wybrany zakres koliduje z istniejącą rezerwacją.");
                    break;
                }
            }

            if (!ModelState.IsValid)
            {
                ViewBag.Cars = new SelectList(_context.Car.ToList(), "Id", "FullName", model.CarId);

                List<string> blockedDates = new List<string>();
                foreach (var r in existingReservations)
                {
                    for (var date = r.StartDate.Date; date <= r.EndDate.Date; date = date.AddDays(1))
                        blockedDates.Add(date.ToString("yyyy-MM-dd"));
                }
                ViewBag.BlockedDates = blockedDates;

                return View(model);
            }

            _context.Reservations.Add(model);
            _context.SaveChanges();

            return RedirectToAction("Index", "Car");
        }

        [Authorize]
        public async Task<IActionResult> MyReservations()
        {
            // Pobieramy ID zalogowanego użytkownika
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var myReservations = await _context.Reservations
                .Include(r => r.Car)
                .Where(r => r.UserId == userId)
                .OrderByDescending(r => r.StartDate)
                .ToListAsync();

            return View(myReservations);
        }
        // GET: ReservationController/Edit/5
        [Authorize]
        public ActionResult Edit(int id)
        {
            var reservation = _context.Reservations.Find(id);

            if (reservation == null)
                return NotFound();

            ViewBag.Cars = new SelectList(
                _context.Car.ToList(),
                "Id",
                "FullName",
                reservation.CarId
            );

            return View(reservation);
        }

        // POST: ReservationController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<ActionResult> EditAsync(int id, Reservation reservation)
        {
            if (id != reservation.Id) return NotFound();

            var existingReservation = await _context.Reservations.AsNoTracking()
                .FirstOrDefaultAsync(r => r.Id == id);

            if (reservation.UserId == null && existingReservation != null)
            {
                reservation.UserId = existingReservation.UserId;
            }

            ModelState.Remove("UserId");
            ModelState.Remove("User");

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(reservation);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateException)
                {
                }
                return RedirectToAction(nameof(Index));
            }
            return View(reservation);
        }

        // GET: ReservationController/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reservation = await _context.Reservations
                .Include(r => r.Car)  
                .Include(r => r.User) 
                .FirstOrDefaultAsync(m => m.Id == id);

            if (reservation == null)
            {
                return NotFound();
            }

            return View(reservation);
        }

        // POST: ReservationController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public ActionResult Delete(int id, Reservation reservation)
        {
            try
            {
                _context.Reservations.Remove(reservation);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        [Authorize(Roles = "User")]
        public async Task<IActionResult> Cancel(int? id)
        {
            if (id == null)
                return NotFound();

            var reservation = await _context.Reservations
                .Include(r => r.Car)
                .Include(r => r.User)
                .FirstOrDefaultAsync(r => r.Id == id && r.User.UserName == User.Identity.Name);

            if (reservation == null)
                return NotFound();

            return View(reservation);
        }

        [HttpPost, ActionName("Cancel")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> CancelConfirmed(int id)
        {
            var reservation = await _context.Reservations
                .Include(r => r.User)
                .FirstOrDefaultAsync(r => r.Id == id && r.User.UserName == User.Identity.Name);

            if (reservation == null)
                return NotFound();

            try
            {
                _context.Reservations.Remove(reservation);
                await _context.SaveChangesAsync();
                return RedirectToAction("MyReservations");
            }
            catch
            {
                return View(reservation);
            }
        }


        [HttpGet]
        public IActionResult NotLogged()
        {
            return RedirectToPage("/Account/Login", new { area = "Identity", error = "Musisz się zalogować, aby zobaczyć swoje rezerwacje." });
        }


    }
}
