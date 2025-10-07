using CarDealershipAPI.Models;

namespace CarDealershipAPI.Data
{
    public static class SeedData
    {
        public static void Initialize(AppDbContext db)
        {
            if (db.Users.Any()) return; // already seeded

            var admin = new User
            {
                Id = Guid.NewGuid(),
                Email = "admin@dealer.local",
                Name = "Admin User",
                Role = Role.Admin,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin123!") //demo password
            };
            db.Users.Add(admin);

            var vehicles = new List<Vehicle>
            {
            new Vehicle { Id = Guid.NewGuid(), Make = "Toyota", Model =
            "Corolla", Year = 2021, Price = 18000, Color = "White", Mileage = 25000,
            Available = true },
            new Vehicle { Id = Guid.NewGuid(), Make = "Honda", Model =
            "Civic", Year = 2020, Price = 19000, Color = "Black", Mileage = 30000,
            Available = true },
            new Vehicle { Id = Guid.NewGuid(), Make = "Ford", Model =
            "Mustang", Year = 2019, Price = 27000, Color = "Red", Mileage = 22000,
            Available = true },
            new Vehicle { Id = Guid.NewGuid(), Make = "Tesla", Model =
            "Model 3", Year = 2022, Price = 42000, Color = "Blue", Mileage = 8000,
            Available = true },
            new Vehicle { Id = Guid.NewGuid(), Make = "BMW", Model = "3Series", Year = 2018, Price = 25000, Color = "Silver", Mileage = 40000,
            Available = true },
            new Vehicle { Id = Guid.NewGuid(), Make = "Audi", Model = "A4",
            Year = 2017, Price = 23000, Color = "Gray", Mileage = 45000, Available = true },
            new Vehicle { Id = Guid.NewGuid(), Make = "Hyundai", Model =
            "Elantra", Year = 2021, Price = 16000, Color = "White", Mileage = 15000,
            Available = true },
            new Vehicle { Id = Guid.NewGuid(), Make = "Kia", Model =
            "Sportage", Year = 2020, Price = 20000, Color = "Green", Mileage = 28000,
            Available = true },
            new Vehicle { Id = Guid.NewGuid(), Make = "Nissan", Model =
            "Altima", Year = 2019, Price = 17000, Color = "Blue", Mileage = 32000,
            Available = true },
            new Vehicle { Id = Guid.NewGuid(), Make = "Mercedes", Model =
            "C-Class", Year = 2018, Price = 30000, Color = "Black", Mileage = 38000,
            Available = true }
            };
            
            db.Vehicles.AddRange(vehicles);
            db.SaveChanges();
        }
    }
}