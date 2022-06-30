using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace TUFTManagement.Models
{
    public class SearchMasterDataBodySetModel
    {
        public bool success { get; set; }
        public MsgModel msg { get; set; }
        public Pagination<SearchMasterDataBodySet> data { get; set; }
    }

    public class SearchMasterDataBodySet
    {
        public int id { set; get; } = 0;
        public float height { set; get; } = 0;
        public float weight { set; get; } = 0;
        public int chest { set; get; } = 0;
        public int waist { set; get; } = 0;
        public int hip { set; get; } = 0;
        public int isActive { set; get; } = 0;

        public void loadData(DataRow dr)
        {
            id = int.Parse(dr["id"].ToString());
            height = float.Parse(dr["height"].ToString());
            weight = float.Parse(dr["weight"].ToString());
            chest = int.Parse(dr["chest"].ToString());
            waist = int.Parse(dr["waist"].ToString());
            hip = int.Parse(dr["hip"].ToString());
            isActive = int.Parse(dr["status"].ToString().ToLower() == "true" ? "1" : "0");
        }
    }
}