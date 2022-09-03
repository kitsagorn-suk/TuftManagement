using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace TUFTManagement.Models
{
    public class GetDetailSystemRoleAssignModel
    {
        public bool success { get; set; }
        public MsgModel msg { get; set; }
        public Assign data { get; set; }
    }

    public class Assign
    {
        public int id { set; get; } = 0;
        public int positionID { set; get; } = 0;
        public string positionName { set; get; } = "";
        public int objectID { set; get; } = 0;
        public int isActive { set; get; } = 0;

        public void loadData(DataRow dr)
        {
            id = int.Parse(dr["id"].ToString());
            positionID = int.Parse(dr["position_id"].ToString());
            positionName = dr["name_en"].ToString();
            objectID = int.Parse(dr["object_id"].ToString()); 
            isActive = int.Parse(dr["is_active"].ToString());
        }
    }
}