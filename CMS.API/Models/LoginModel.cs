using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CMS.API.Models
{
    public class LoginModel
    {
        [Required]
        public string USER_ID  { get; set; }
        [Required]
        public string USER_PW  { get; set; }
        public bool RememberMe { get; set; }
    }
}