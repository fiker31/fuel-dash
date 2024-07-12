using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FuelApplicationDashboard.Models
{
    public class changepinModel
    {
   
        [Required(ErrorMessage = "Current Password is required")]
        [StringLength(15, ErrorMessage = "Must be between 6 and 15 characters", MinimumLength = 6)]
        [DataType(DataType.Password)]
        public string currentpin { get; set; }

        [Required(ErrorMessage = "New password is required")]
        [StringLength(15, ErrorMessage = "Must be between 6 and 15 characters", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name ="New Password")]
        public string newpin { get; set; }



        [Required(ErrorMessage = "Confirm new password is required!")]
        [StringLength(15, ErrorMessage = "Must be between 6 and 15 characters", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Compare("newpin",ErrorMessage ="New password and confirm passsword dont match!")]
        public string newpinconfirm { get; set; }
    }
}