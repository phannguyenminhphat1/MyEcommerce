using System.ComponentModel.DataAnnotations;

namespace MyEcommerce.Admin.Users
{
    public class SetPasswordDto
    {
        [Required]
        public string? NewPassword { get; set; }

        [Required]
        public string? ConfirmNewPassword { get; set; }
    }
}