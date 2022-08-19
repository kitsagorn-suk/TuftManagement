using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace TUFTManagement.Models
{
    public class GetSystemMasterModel
    {
        public bool success { get; set; }
        public MsgModel msg { get; set; }
        public SystemMaster data { get; set; }
    }

    public class SystemMaster
    {
        public int masterID { set; get; } = 0;
        public string keyName { set; get; } = "";
        public int value { set; get; } = 0;
        public string nameEN { set; get; } = "";
        public string nameTH { set; get; } = "";
        public int order { set; get; } = 0;
        public int isActive { set; get; } = 0;

        public void loadData(DataRow dr)
        {
            masterID = int.Parse(dr["id"].ToString());
            keyName = dr["key_name"].ToString();
            value = int.Parse(dr["value"].ToString());
            nameEN = dr["name_en"].ToString(); ;
            nameTH = dr["name_th"].ToString(); ;
            order = int.Parse(dr["order"].ToString());
            isActive = int.Parse(dr["is_active"].ToString().ToLower().Equals("true") ? "1" : "0");
        }
    }
}