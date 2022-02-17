using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TUFTManagement.DTO
{
    public class LoginRequestDTO
    {
        public int companyID { set; get; } = 0;
        public string user_name { set; get; } = "";
        public string password { set; get; } = "";
        public string deviceType { set; get; } = "";
        public string deviceID { set; get; } = "";
        public string deviceToken { set; get; } = "";
    }
}