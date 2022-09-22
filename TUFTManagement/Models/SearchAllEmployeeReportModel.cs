using System.Data;

namespace TUFTManagement.Models
{
    public class SearchAllEmployeeReportModel
    {
        public bool success { get; set; }
        public MsgModel msg { get; set; }
        public SearchAllEmployeeReport data { get; set; }
    }
    public class SearchAllEmployeeReport
    {
        public EmployeeReportHeader header { get; set; }
        public Pagination<SearchAllEmployeeReportBody> body { get; set; }
    }

    public class EmployeeReportHeader
    {
        public int workTimeAll { set; get; } = 0;
        public int workPR { set; get; } = 0;
        public int workFullTime { set; get; } = 0;
        public int workPartTime { set; get; } = 0;
        public int workContact { set; get; } = 0;

        public void loadData(DataRow dr)
        {
            workTimeAll = int.Parse(dr["emp_all"].ToString());
            workPR = int.Parse(dr["emp_pr"].ToString());
            workFullTime = int.Parse(dr["emp_full_time"].ToString());
            workPartTime = int.Parse(dr["emp_part_time"].ToString());
            workContact = int.Parse(dr["emp_contact"].ToString());
        }
    }

    public class SearchAllEmployeeReportBody
    {
        public string empCode { set; get; } = "";
        public string empName { set; get; } = "";
        public string departmentName { set; get; } = "";
        public string positionName { set; get; } = "";
        public string empType { set; get; } = "";
        public string empPhoneNum { set; get; } = "";
        public string empStatus { set; get; } = "";

        public void loadData(DataRow dr)
        {
            empCode = dr["emp_code"].ToString();
            empName = dr["name"].ToString();
            departmentName = dr["department"].ToString();
            positionName = dr["position"].ToString();
            empType = dr["emp_type"].ToString();
            empPhoneNum = dr["phone_number"].ToString();
            empStatus = dr["emp_status"].ToString();
        }
    }
}