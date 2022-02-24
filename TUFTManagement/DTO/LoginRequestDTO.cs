using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TUFTManagement.DTO
{
    public class LoginRequestDTO
    {
        public string username { set; get; } = "";
        public string password { set; get; } = "";
    }
}