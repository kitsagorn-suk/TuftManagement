using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace TUFTManagement.Models
{
    public class SearchSystemMasterModel
    {
        public bool success { get; set; }
        public MsgModel msg { get; set; }
        public Pagination<SearchSystemMaster> data { get; set; }
    }

    public class SearchSystemMaster
    {
        public int id { set; get; } = 0;
        public string keyName { set; get; } = "";
        public int isActive { set; get; } = 0;
        public List<AllMasterInKey> allMasterInKey { set; get; }

        public void loadData(DataRow dr)
        {
            id = int.Parse(dr["id"].ToString());
            keyName = dr["key_name"].ToString();
            isActive = int.Parse(dr["is_active"].ToString().ToLower() == "true" ? "1" : "0");
        }
    }

    public class AllMasterInKey
    {
        public int id { set; get; } = 0;
        public string name { set; get; } = "";
        public int order { set; get; } = 0;
        
        public void loadData(DataRow dr)
        {
            id = int.Parse(dr["id"].ToString());
            name = dr["name"].ToString();
            order = int.Parse(dr["order"].ToString());
        }
    }
}