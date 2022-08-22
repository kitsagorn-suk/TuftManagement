using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace TUFTManagement.Models
{
    public class GetMasterKeyModel
    {
        public bool success { get; set; }
        public MsgModel msg { get; set; }
        public MasterKey data { get; set; }
    }

    public class MasterKey
    {
        public int keyID { set; get; } = 0;
        public string keyName { set; get; } = "";
        public int isActive { set; get; } = 0;

        public void loadData(DataRow dr)
        {
            keyID = int.Parse(dr["id"].ToString());
            keyName = dr["key_name"].ToString();
            isActive = int.Parse(dr["is_active"].ToString().ToLower().Equals("true") ? "1" : "0");
        }
    }
}