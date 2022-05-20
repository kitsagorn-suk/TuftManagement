using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace TUFTManagement.Models
{
    public class GetEmpWorkTimeModel
    {
        public bool success { get; set; }
        public MsgModel msg { get; set; }
        public GetEmpWorkTime data { get; set; }
    }

    public class GetEmpWorkTime
    {
        public int empWorkTimeID { set; get; } = 0;
        public int empID { set; get; } = 0;
        public int empWorkShiftID { set; get; } = 0;
        public string workDate { set; get; } = "";
        public string workIn { set; get; } = "";
        public string workOut { set; get; } = "";
        public string floorIn { set; get; } = "";
        public string floorOut { set; get; } = "";
        public int isChange { set; get; } = 0;
        public int isFix { set; get; } = 0;
        public int status { set; get; } = 0;

        public int oldWorkShiftID { set; get; } = 0;
        public int newWorkShiftID { set; get; } = 0;
        public int statusApprove { set; get; } = 0;
        public int approveBy { set; get; } = 0;
        public string approveDate { set; get; } = "";
        public string remark { set; get; } = "";

        public void loadData(DataRow dr)
        {
            empWorkTimeID = int.Parse(dr["worktime_id"].ToString());
            empID = int.Parse(dr["user_id"].ToString());
            empWorkShiftID = int.Parse(dr["work_shift_id"].ToString());
            workDate = dr["work_date"].ToString();
            workIn = dr["work_in"].ToString();
            workOut = dr["work_out"].ToString();
            floorIn = dr["floor_in"].ToString();
            floorOut = dr["floor_out"].ToString();
            isChange = int.Parse(dr["is_change"].ToString().ToLower().Equals("true") ? "1" : "0");
            isFix = int.Parse(dr["is_fix"].ToString().ToLower().Equals("true") ? "1" : "0");
            status = int.Parse(dr["status"].ToString().ToLower().Equals("true") ? "1" : "0");

            oldWorkShiftID = int.Parse(dr["work_shift_id_old"].ToString());
            newWorkShiftID = int.Parse(dr["work_shift_id_new"].ToString());
            statusApprove = int.Parse(dr["status_approve"].ToString());
            approveBy = int.Parse(dr["approve_by"].ToString());
            approveDate = dr["approve_date"].ToString();
            remark = dr["remark"].ToString();
        }
    }
}