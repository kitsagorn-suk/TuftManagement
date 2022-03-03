using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace TUFTManagement.Models
{
    public class SearchMasterDataModel
    {
        public bool success { get; set; }
        public MsgModel msg { get; set; }
        public Pagination<SearchMasterData> data { get; set; }
    }

    public class SearchMasterData
    {
        public int masterID { set; get; } = 0;
        public string masterNameEN { set; get; } = "";
        public string masterNameTH { set; get; } = "";
        public int isActive { set; get; }

        public void loadData(DataRow dr)
        {
            masterID = int.Parse(dr["id"].ToString());
            masterNameEN = dr["name_en"].ToString();
            masterNameTH = dr["name_th"].ToString();
            isActive = int.Parse(dr["is_active"].ToString().ToLower() == "true" ? "1" : "0");
        }
    }
}