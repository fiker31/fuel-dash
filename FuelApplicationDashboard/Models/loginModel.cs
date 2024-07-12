using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FuelApplicationDashboard.Models
{
    public class loginModel
    {
        [Required]
        public string Shortcode { get; set; }

        [Required]
        public string Password  { get; set; }
    }
}