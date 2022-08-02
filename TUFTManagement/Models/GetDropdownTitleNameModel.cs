using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace TUFTManagement.Models
{
    public class GetDropdownTitleNameModel
    {
        public bool success { get; set; }
        public MsgModel msg { get; set; }
        public List<DropdownTitleName> data { get; set; }
    }

    public class DropdownTitleName
    {
        public int id { get; set; } = 0;
        public string value { get; set; } = "";
        public string valueTh { get; set; } = "";
        public string valueEn { get; set; } = "";

        public void loadData(DataRow dr)
        {
            // name mapping table DB
            id = int.Parse(dr["id"].ToString());
            value = dr["name"].ToString();

            if (dr["name_th"].ToString() != null || dr["name_th"].ToString() != "")
            {
                valueTh = dr["name_th"].ToString();
            }
            if (dr["name_en"].ToString() != null || dr["name_en"].ToString() != "")
            {
                valueEn = dr["name_en"].ToString();
            }


        }

    }
}