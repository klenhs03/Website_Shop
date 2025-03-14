using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebsiteShop.DomainModels
{
    public class ChangePassword
    {
        public string OldPassword { get; set; } = "";

        public string NewPassword { get; set; } = "";

        public string ConfirmPassword { get; set; } = "";

        public string SuccessMessage { get; set; } = "";
    }
}
