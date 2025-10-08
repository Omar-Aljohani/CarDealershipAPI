using System.Text.Json.Serialization;

namespace CarDealershipAPI.Models
{
    public enum Role { Customer, Admin }
    public class User
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        [JsonIgnore]
        public string PasswordHash { get; set; } = string.Empty;
        public Role Role { get; set; } = Role.Customer;
        // navigation
        [JsonIgnore]
        public List<Sale>? Sales { get; set; }
    }
}
