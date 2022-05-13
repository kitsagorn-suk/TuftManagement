using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace TUFTManagement.Models
{
    public class CheckUserByTokenModel
    {
        public int userID { set; get; } = 0;
        public int roleID { set; get; } = 0;
        public string business_code { set; get; }

        public void loadData(DataRow dr)
        {
            userID = int.Parse(dr["id"].ToString());
            roleID = int.Parse(dr["role_id"].ToString());
            business_code = dr["business_code"].ToString();
        }
    }
}