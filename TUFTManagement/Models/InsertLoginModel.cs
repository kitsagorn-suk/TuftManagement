using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace TUFTManagement.Models
{
    public class InsertLoginModel
    {
        public bool success { get; set; }
        public MsgModel msg { get; set; }
        public InsertLogin data { get; set; }
    }

    public class InsertLogin
    {
        public int userID { get; set; } = 0;
        public void loadData(DataRow dr)
        {
            userID = int.Parse(dr["user_id"].ToString());
        }
    }
}