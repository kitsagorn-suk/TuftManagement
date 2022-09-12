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
        public int positionID { set; get; } = 0;
        public string shareCodeList { set; get; } = "";

        public void loadData(DataRow dr)
        {
            userID = int.Parse(dr["id"].ToString());
            positionID = int.Parse(dr["position_id"].ToString());
            shareCodeList = dr["share_code_list"].ToString();
        }
    }
}