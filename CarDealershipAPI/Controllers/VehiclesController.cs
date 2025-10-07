using CarDealershipAPI.Data;
using CarDealershipAPI.DTOs;
using CarDealershipAPI.Models;
using CarDealershipAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Model;

namespace CarDealershipAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class VehiclesController : ControllerBase
    {
        private readonly AppDbContext _db;
        private readonly VehicleService _svc;
        private readonly OtpService _otp;
        public VehiclesController(AppDbContext db, VehicleService svc, OtpService otp)
        {
            _db = db;
            _svc = svc;
            _otp = otp;
        }

        [HttpGet]
        public async Task<IActionResult> Browse([FromQuery] string? make, [FromQuery] string? model,
            [FromQuery] decimal? minPrice, [FromQuery] decimal? maxPrice)
        {
            var q = _svc.Query().Where(v => v.Available);
            if (!string.IsNullOrWhiteSpace(make)) q = q.Where(v => v.Make.ToLower().Contains(make.ToLower()));
            if (!string.IsNullOrWhiteSpace(model)) q = q.Where(v => v.Model.ToLower().Contains(model.ToLower()));
            if (minPrice.HasValue) q = q.Where(v => v.Price >= minPrice.Value);
            if (maxPrice.HasValue) q = q.Where(v => v.Price <= maxPrice.Value);

            var list = await q.ToListAsync();
            return Ok(list);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Details(Guid id)
        {
            var v = await _svc.GetByIdAsync(id);
            if(v == null) return NotFound();
            return Ok(v);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] VehicleCreateDto dto)
        {
            var v = new Vehicle{
                Id = Guid.NewGuid(),
                Make = dto.Make,
                Model = dto.Model,
                Year = dto.Year,
                Price = dto.Price,
                Color = dto.Color,
                Mileage = dto.Mileage,
                Description = dto.Description,
                Available = true
            };

            await _svc.AddAsync(v);
            return CreatedAtAction(nameof(Details), new { id = v.Id }, v);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] VehicleUpdateDto dto)
        {
            // OTP protection required: client must send X-OTP-Token header after validating OTP
            var currentUserId = Guid.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value);
            var otpToken = Request.Headers["X-OTP-Token"].FirstOrDefault();
            var ok = await _otp.ValidateOtpTokenForActionAsync(currentUserId, "UpdateVehicle", otpToken);
            if (!ok) return Forbid();

            var v = await _svc.GetByIdAsync(id);
            if (v == null) return NotFound();

            v.Make = dto.Make;
            v.Model = dto.Model;
            v.Year = dto.Year;
            v.Price = dto.Price;
            v.Color = dto.Color;
            v.Mileage = dto.Mileage;
            v.Available = dto.Available;
            v.Description = dto.Description;

            await _svc.UpdateAsync(v);
            return Ok(v);
        }

    }
}
