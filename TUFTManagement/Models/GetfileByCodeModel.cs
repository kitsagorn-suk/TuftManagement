using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace TUFTManagement.Models
{
    public class GetfileByCodeModel
    {
        public bool success { get; set; }
        public MsgModel msg { get; set; }
        public List<_GetfileByCode> data { get; set; }
    }

    public class _GetfileByCode
    {
        public int id { set; get; } = 0;
        public string actionName { set; get; } = "";
        public string fileName { set; get; } = "";
        public string url { set; get; } = "";
        
        public void loadData(DataRow dr)
        {
            id = int.Parse(dr["id"].ToString());
            actionName = dr["action_name"].ToString();
            fileName = dr["name"].ToString();
            url = dr["url"].ToString();
        }
    }
}