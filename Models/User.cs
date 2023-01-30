using System.ComponentModel.DataAnnotations;

namespace LiteraturePlatformWebApi.Models
{
    public class User
    {
        public int UserId { get; set; }

        public string? Login { get; set; }

        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        public string Password { get; set; }

    }
}
