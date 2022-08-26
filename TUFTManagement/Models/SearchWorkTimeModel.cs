using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using TUFTManagement.DTO;

namespace TUFTManagement.Models
{
    public class SearchWorkTimeModel
    {
        public bool success { get; set; }
        public MsgModel msg { get; set; }
        public SearchWorkTimeAll data { get; set; }
    }

    public class SearchWorkTimeAll
    {
        public List<SearchWorkShiftTimeAllTotalDTO> header { get; set; }
        public Pagination<SearchWorkTime> body { get; set; }
    }

    public class SearchWorkTime
    {
        public int userID { set; get; } = 0;
        public int empProfileID { set; get; } = 0;
        public string empCode { set; get; } = "";
        public string empName { set; get; } = "";
        public string departmentName { set; get; } = "";
        public string positionName { set; get; } = "";
        public string workShiftCode { set; get; } = "";
        public string workTime { set; get; } = "";
        public string leave { set; get; } = "";
        public string isChange { set; get; } = "";

        public void loadData(DataRow dr)
        {
            userID = int.Parse(dr["user_id"].ToString());
            empProfileID = int.Parse(dr["id"].ToString());
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
            workShiftCode = dr["ws_code"].ToString();
            workTime = dr["worktime"].ToString();
            leave = dr["leave"].ToString();
            isChange = dr["is_change"].ToString();
        }
    }
}