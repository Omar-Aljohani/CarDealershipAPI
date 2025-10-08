using CarDealershipAPI.Data;
using CarDealershipAPI.DTOs;
using CarDealershipAPI.Models;
using CarDealershipAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Reflection.PortableExecutable;

namespace CarDealershipAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SalesController : ControllerBase
    {
        private readonly AppDbContext _db;
        private readonly OtpService _otp;

        public SalesController(AppDbContext db, OtpService otp)
        {
            _db = db;
            _otp = otp;
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Details(Guid id)
        {
            var v = await _db.Sales.FindAsync(id);
            if (v == null) return NotFound();
            return Ok(v);
        }

        [Authorize(Roles = "Customer")]
        [HttpPost("request-purchase")]
        public async Task<IActionResult> PurchaseRequest([FromBody] PurchaseRequestDto dto)
        {
            var currentUserId = Guid.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value);
            var otpToken = Request.Headers["X-OTP-Token"].FirstOrDefault();
            var ok = await _otp.ValidateOtpTokenForActionAsync(currentUserId,
            OTPAction.PurchaseRequest, otpToken);
            if (!ok) return Forbid();

            var v = await _db.Vehicles.FindAsync(dto.VehicleId);
            if (v == null || !v.Available) return BadRequest(new {message ="Vehicle unavailable"});


            var sale = new Sale
            {
                Id = Guid.NewGuid(),
                requesterId = currentUserId,
                VehicleId = v.Id,
                UserId = currentUserId,
                Price = v.Price,
                PurchasedAt = DateTime.UtcNow
            };

            v.Available = false;
            _db.Sales.Add(sale);
            _db.Vehicles.Update(v);
            await _db.SaveChangesAsync();

            return Ok( new { message = "Purchase request processed (demo).", saleId = sale});
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("process-sale")]
        public async Task<IActionResult> ProcessSale([FromBody] ProcessSaleDto dto)
        {
            var sale = await _db.Sales.FindAsync(dto.SaleID);
            if (sale == null) return NotFound(new { message = "Sale not found" });
            if (sale.ProcessedAt != null) return BadRequest(new { message = "Sale already processed" });

            sale.ProcessedAt = DateTime.UtcNow;
            await _db.SaveChangesAsync();

            return Ok(sale);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("add-sale")]
        public async Task<IActionResult> AddSale([FromBody] AddSaleDto dto)
        {
            var currentUserId = Guid.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value);
            var vehicle = await _db.Vehicles.FindAsync(dto.VehicleId);
            var customer = await _db.Users.FindAsync(dto.CustomerId);
            if (vehicle == null || customer == null) return BadRequest(new { message = "Invalid vehicle or customer" });
            if (!vehicle.Available) return BadRequest(new { message = "Vehicle is unavailable" });

            var sale = new Sale
            {
                Id = Guid.NewGuid(),
                requesterId = currentUserId,
                VehicleId = vehicle.Id,
                UserId = customer.Id,
                Price = dto.Price,
                PurchasedAt = DateTime.UtcNow
            };

            vehicle.Available = false;
            _db.Sales.Add(sale);
            _db.Vehicles.Update(vehicle);
            await _db.SaveChangesAsync();

            return CreatedAtAction(nameof(Details), new {id = sale.Id}, sale.Id);
        }

        [Authorize]
        [HttpGet("history")]
        public async Task<IActionResult> History()
        {
            var userId = Guid.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value);
            var role = User.FindFirst(System.Security.Claims.ClaimTypes.Role)!.Value;
            if (role == "Admin")
            {
                var all = await _db.Sales.Include(s => s.Vehicle).Include(s => s.User).ToListAsync();
                return Ok(all);
            }
            else
            {
                var usersSales = await _db.Sales.Where(s => s.UserId == userId).Include(s => s.Vehicle).ToListAsync();
                return Ok(usersSales);
            }
        }
    }
}
