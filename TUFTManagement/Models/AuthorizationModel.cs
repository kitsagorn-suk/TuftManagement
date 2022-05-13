using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TUFTManagement.Models
{
    public class AuthorizationModel
    {
        public int user_id { get; set; } = 0;
        public int profile_id { get; set; } = 0;
        public int company_id { get; set; } = 0;
        public int role_id { get; set; } = 0;
        public string username { get; set; } = "";
        public string password { get; set; } = "";
        public string business_code { get; set; } = "";
        public string type_login { get; set; } = "";
        public string signal { get; set; } = "";
        public double create_date { get; set; } = 0;
        public double expire_date { get; set; } = 0;
    }

    public class BasicResponse
    {
        public bool success { get; set; }
        public MsgModel msg { get; set; }
    }
}