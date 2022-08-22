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
        public int value { set; get; } = 0;
        public string nameEN { set; get; } = "";
        public string nameTH { set; get; } = "";
        public int order { set; get; } = 0;

        public void loadData(DataRow dr)
        {
            id = int.Parse(dr["id"].ToString());
            keyName = dr["key_name"].ToString();
            value = int.Parse(dr["value"].ToString());
            nameEN = dr["name_en"].ToString();
            nameTH = dr["name_th"].ToString();
            order = int.Parse(dr["order"].ToString());
        }
    }
}