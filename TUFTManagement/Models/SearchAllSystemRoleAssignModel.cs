using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace TUFTManagement.Models
{
    public class SearchAllSystemRoleAssignModel
    {
        public bool success { get; set; }
        public MsgModel msg { get; set; }
        public Pagination<SearchAllSystemRoleAssign> data { get; set; }
    }

    public class SearchAllSystemRoleAssign
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