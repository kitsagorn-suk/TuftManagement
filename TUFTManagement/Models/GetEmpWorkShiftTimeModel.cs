using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace TUFTManagement.Models
{
    public class GetEmpWorkShiftTimeModel
    {
        public bool success { get; set; }
        public MsgModel msg { get; set; }
        public EmpWorkShiftTimeDetail data { get; set; }
    }

    public class EmpWorkShiftTimeDetail
    {
        
        public EmpWorkShiftTimeHeader dataHeader { get; set; }
        public Pagination<EmpWorkShiftTimeSearch> dataSearch { get; set; }
    }

        public class EmpWorkShiftTimeSearch
    {
        public int id { set; get; } = 0;
        public string createDate { set; get; } = "";
        public string action { set; get; } = "";
        public string actionCode { set; get; } = "";
        public string tradeName { set; get; } = "";

        public void loadData(DataRow dr)
        {
            id = int.Parse(dr["id"].ToString());
            createDate = dr["create_date"].ToString();
            action = dr["action"].ToString();
            actionCode = dr["action_code"].ToString();
            tradeName = dr["trade_name"].ToString();
        }
    }

    public class EmpWorkShiftTimeHeader
    {
        public string empCode { set; get; } = "";
        public string empName { set; get; } = "";
        public string empNickName { set; get; } = "";
        public string deptName { set; get; } = "";
        public string shiftCode { set; get; } = "";
        

        public void loadData(DataRow dr)
        {
            empCode = dr["emp_code"].ToString();
            empName = dr["emp_name"].ToString();
            empNickName = dr["emp_nickname"].ToString();
            deptName = dr["department_name"].ToString();
            //shiftCode = dr["name_th"].ToString();
           
        }
    }
}