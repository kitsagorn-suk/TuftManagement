using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace TUFTManagement.Models
{
    public class GetMasterDataModel
    {
        public bool success { get; set; }
        public MsgModel msg { get; set; }
        public MasterData data { get; set; }
    }

    public class MasterData
    {
        public int masterID { set; get; } = 0;
        public string nameEN { set; get; } = "";
        public string nameTH { set; get; } = "";
        public int isActive { set; get; } = 0;

        public void loadData(DataRow dr)
        {
            masterID = int.Parse(dr["id"].ToString());
            nameEN = dr["name_en"].ToString();
            nameTH = dr["name_th"].ToString();
            isActive = int.Parse(dr["is_active"].ToString().ToLower().Equals("true") ? "1" : "0");
        }
    }
}