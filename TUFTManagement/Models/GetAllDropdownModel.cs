using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace TUFTManagement.Models
{
    public class GetAllDropdownModel
    {
        public bool success { get; set; }
        public MsgModel msg { get; set; }
        public List<_DropdownAllData> data { get; set; }
    }
    public class _DropdownAllData
    {
        public int id { get; set; } = 0;
        public string value { get; set; } = "";

        public void loadData(DataRow dr)
        {
            // name mapping table DB
            id = int.Parse(dr["id"].ToString());
            value = dr["name"].ToString();
        }
    }
}