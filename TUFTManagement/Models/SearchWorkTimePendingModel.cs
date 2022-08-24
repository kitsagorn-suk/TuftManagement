using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using TUFTManagement.DTO;

namespace TUFTManagement.Models
{
    public class SearchWorkTimePendingModel
    {
        public bool success { get; set; }
        public MsgModel msg { get; set; }
        public SearchWorkTimePending data { get; set; }
    }
        public class SearchWorkTimePending
    {
        public SearchWorktimePendingTotalDTO header { get; set; }
        public Pagination<SearchWorkTimePendingPage> body { get; set; }
    }

        public class SearchWorkTimePendingPage
    {
        public int transChangeWorkTimeID { set; get; } = 0;
        public int userID { set; get; } = 0;
        public string empCode { set; get; } = "";
        public string empName { set; get; } = "";
        public string departmentName { set; get; } = "";
        public string positionName { set; get; } = "";

        public int actionID { set; get; } = 0;
        public string actionName { set; get; } = "";

        public int workShiftOldID { set; get; } = 0;
        public string workShiftOldName { set; get; } = "";
        public int workShiftNewID { set; get; } = 0;
        public string workShiftNewName { set; get; } = "";

        public string remark { set; get; } = "";

        public void loadData(DataRow dr)
        {
            transChangeWorkTimeID = int.Parse(dr["user_id"].ToString());
            userID = int.Parse(dr["user_id"].ToString());
            empCode = dr["emp_code"].ToString();

            dr["emp_nick_name"].ToString();
            if (!string.IsNullOrEmpty(dr["emp_nick_name"].ToString()))
            {
                empName = dr["emp_name"].ToString() + "(" + dr["emp_nick_name"].ToString() + ")";
            }
            else
            {
                empName = dr["emp_name"].ToString();
            }

            positionName = dr["position_name"].ToString();
            departmentName = dr["department_name"].ToString();


            actionID = int.Parse(dr["action"].ToString());
            actionName = dr["action_name"].ToString();
            workShiftOldID = int.Parse(dr["work_shift_id_old"].ToString());
            workShiftOldName = dr["work_shift_name_old"].ToString();
            workShiftNewID = int.Parse(dr["work_shift_id_new"].ToString());
            workShiftNewName = dr["work_shift_name_new"].ToString();
            remark = dr["remark"].ToString();
        }
    }
}