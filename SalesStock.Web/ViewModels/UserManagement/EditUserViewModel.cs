using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace SalesStock.Web.ViewModels.UserManagement
{
    public class EditUserViewModel
    {
        public string Id { get; set; } = string.Empty;

        [Required(ErrorMessage = "Username is required")]
        [Display(Name = "Username")]
        public string UserName { get; set; } = string.Empty;

        [Required(ErrorMessage = "E-mail is required")]
        [EmailAddress]
        [Display(Name = "E-mail")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Account status is required")]
        [Display(Name = "Account status")]
        public string Status { get; set; } = string.Empty;

        [Required(ErrorMessage = "Role is required")]
        [Display(Name = "User Role")]
        public string SelectedRole { get; set; } = string.Empty;
        public IEnumerable<SelectListItem> RoleList { get; set; } = new List<SelectListItem>();

        public bool IsEditingSelf { get; set; }
    }
}
