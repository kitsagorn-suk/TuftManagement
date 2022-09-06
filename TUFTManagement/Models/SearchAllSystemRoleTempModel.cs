using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace TUFTManagement.Models
{
    public class SearchAllSystemRoleTempModel
    {
        public bool success { get; set; }
        public MsgModel msg { get; set; }
        public Pagination<SearchAllSystemRoleTemp> data { get; set; }
    }

    public class SearchAllSystemRoleTemp
    {
        public int objectID { set; get; } = 0;
        public string objectName { set; get; } = "";
        public int parentID { set; get; } = 0;
        public int isActive { set; get; } = 0;

        public void loadData(DataRow dr)
        {
            objectID = int.Parse(dr["object_id"].ToString());
            parentID = int.Parse(dr["parent_id"].ToString());
            objectName = dr["object_name"].ToString();
            isActive = int.Parse(dr["is_active"].ToString());
        }
    }
}