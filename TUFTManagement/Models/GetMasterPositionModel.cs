using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace TUFTManagement.Models
{
    public class GetMasterPositionModel
    {
        public bool success { get; set; }
        public MsgModel msg { get; set; }
        public MasterPosition data { get; set; }
    }

    public class MasterPosition
    {
        public int positionID { set; get; } = 0;
        public string posNameEN { set; get; } = "";
        public string posNameTH { set; get; } = "";
        public int isActive { set; get; } = 0;
        public string deptNameEN { set; get; } = "";
        public string deptNameTH { set; get; } = "";

        public void loadData(DataRow dr)
        {
            positionID = int.Parse(dr["id"].ToString());
            posNameEN = dr["position_name_en"].ToString();
            posNameTH = dr["position_name_th"].ToString();
            deptNameEN = dr["department_name_en"].ToString(); ;
            deptNameTH = dr["department_name_en"].ToString(); ;
            isActive = int.Parse(dr["is_active"].ToString().ToLower().Equals("true") ? "1" : "0");
        }
    }
}