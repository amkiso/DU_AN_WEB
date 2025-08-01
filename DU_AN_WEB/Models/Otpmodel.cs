using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DU_AN_WEB.Models
{
	public class Otpmodel
	{public string Email { get; set; }
        public string Code { get; set; }
        public DateTime ExpirationTime { get; set; }
    }

}