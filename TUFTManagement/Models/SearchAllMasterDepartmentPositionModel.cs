using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace TUFTManagement.Models
{
    public class SearchAllMasterDepartmentPositionModel
    {
        public bool success { get; set; }
        public MsgModel msg { get; set; }
        public Pagination<SearchAllMasterDepartmentPosition> data { get; set; }
    }
    
    public class SearchAllMasterDepartmentPosition
    {
        public int departmentID { set; get; } = 0;
        public string departmentName { set; get; } = "";
        public int positionID { set; get; } = 0;
        public string positionName { set; get; } = "";
        public int isActive { set; get; } = 0;


        public void LoadData(DataRow dr)
        {
            departmentID = int.Parse(dr["department_id"].ToString());
            departmentName = dr["department_name"].ToString();
            positionID = int.Parse(dr["position_id"].ToString());
            positionName = dr["position_name"].ToString();
            isActive = int.Parse(dr["is_active"].ToString().ToLower() == "true" ? "1" : "0");

        }
    }
    
}