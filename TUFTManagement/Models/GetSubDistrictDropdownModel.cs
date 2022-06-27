using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace TUFTManagement.Models
{
    public class GetSubDistrictDropdownModel
    {
        public bool success { get; set; }
        public MsgModel msg { get; set; }
        public List<DropdownSubDistrict> data { get; set; }
    }

    public class DropdownSubDistrict
    {
        public int id { get; set; } = 0;
        public string value { get; set; } = "";
        public string zipCode { get; set; } = "";

        public void loadData(DataRow dr)
        {
            // name mapping table DB
            id = int.Parse(dr["id"].ToString());
            value = dr["name"].ToString();
            zipCode = dr["POSTCODE"].ToString();
        }

    }
}