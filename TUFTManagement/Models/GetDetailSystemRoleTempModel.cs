using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace TUFTManagement.Models
{
    public class GetDetailSystemRoleTempModel
    {
        public bool success { get; set; }
        public MsgModel msg { get; set; }
        public Temp data { get; set; }
    }

    public class Temp
    {
        public int parentID { set; get; } = 0;
        public string objectName { set; get; } = "";
        public int objectID { set; get; } = 0;
        public int isActive { set; get; } = 0;

        public void loadData(DataRow dr)
        {
            parentID = int.Parse(dr["parent_id"].ToString());
            objectName = dr["object_name"].ToString();
            objectID = int.Parse(dr["object_id"].ToString());
            isActive = int.Parse(dr["is_active"].ToString());
        }
    }
}