using System.Text.Json.Serialization;

namespace user_managment.Model
{
    public class User
    {
        public int Id { get; set; }
        public required string Username { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public required string Email { get; set; }
        public Role Role { get; set; }

        [JsonIgnore]
        public string? PasswordHash { get; set; }
    }
}
