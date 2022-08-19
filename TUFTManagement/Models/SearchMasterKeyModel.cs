using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace TUFTManagement.Models
{
    public class SearchMasterKeyModel
    {
        public bool success { get; set; }
        public MsgModel msg { get; set; }
        public Pagination<SearchMasterKey> data { get; set; }
    }

    public class SearchMasterKey
    {
        public int id { set; get; } = 0;
        public string keyName { set; get; } = "";
        

        public void loadData(DataRow dr)
        {
            id = int.Parse(dr["id"].ToString());
            keyName = dr["key_name"].ToString();
            
        }
    }
}