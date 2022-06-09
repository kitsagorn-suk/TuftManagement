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
        public string roleIDList { set; get; } = "";
        public string shareCodeList { set; get; } = "";

        public void loadData(DataRow dr)
        {
            userID = int.Parse(dr["id"].ToString());
            roleIDList = dr["role_id_list"].ToString();
            shareCodeList = dr["share_code_list"].ToString();
        }
    }
}