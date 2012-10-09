using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SAMS.Models
{
    public class ResetPasswordViewModel
    {
        public string Username { get; set; }
        public string NewPassword { get; set; }
    }
}