using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TUFTManagement.Models
{
    public class AuthorizationModel
    {
        public int userID { get; set; } = 0;
        public int profileID { get; set; } = 0;
        public int companyID { get; set; } = 0;
        public int positionID { get; set; } = 0;
        //public string roleIDList { get; set; } = "";
        public string userName { get; set; } = "";
        public string password { get; set; } = "";
        public string shareCodeList { get; set; } = "";
        public string fromProject { get; set; } = "";
        public string signal { get; set; } = "";
        public double createDate { get; set; } = 0;
        public double expireDate { get; set; } = 0;
    }

    public class BasicResponse
    {
        public bool success { get; set; }
        public MsgModel msg { get; set; }
    }
}