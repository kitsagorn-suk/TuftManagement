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
        public int keyID { set; get; } = 0;
        public string keyName { set; get; } = "";
        public int isActive { set; get; } = 0;
        public List<MasterDetail> masterList { set; get; }

        public void loadData(DataRow dr)
        {
            keyID = int.Parse(dr["id"].ToString());
            keyName = dr["key_name"].ToString();
            isActive = int.Parse(dr["is_active"].ToString().ToLower().Equals("true") ? "1" : "0");
        }
    }

    public class MasterDetail
    {
        public int masterID { set; get; } = 0;
        public string masterName { set; get; } = "";
        public int masterOrder { set; get; } = 0;
        public int isActive { set; get; } = 0;

        public void loadData(DataRow dr)
        {
            masterID = int.Parse(dr["id"].ToString());
            masterName = dr["name"].ToString();
            masterOrder = int.Parse(dr["order"].ToString());
            isActive = int.Parse(dr["is_active"].ToString().ToLower().Equals("true") ? "1" : "0");
        }
    }
}