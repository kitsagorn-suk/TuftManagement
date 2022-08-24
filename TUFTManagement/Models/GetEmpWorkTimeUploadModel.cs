using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TUFTManagement.Models
{
    public class GetEmpWorkTimeUploadModel
    {
        public bool success { get; set; }
        public MsgModel msg { get; set; }
        public EmpWorkTimeUpload data { get; set; }
    }

    public class EmpWorkTimeUpload
    {
        public List<EmployeeUpload> employeeUpload { get; set; }
    }

    public class EmployeeUpload
    {
        public int userID { set; get; } = 0;
        public string empCode { set; get; } = "";
        public string empFullName { set; get; } = "";
        public string departmentPositionName { set; get; } = "";
        public int isFix { set; get; } = 0;
        public List<WorkShiftUpload> workShiftUpload { get; set; }
    }

    public class WorkShiftUpload
    {
        public int empWorkShiftID { set; get; } = 0;
        public string wsCode { set; get; } = "";
        public string timeStart { set; get; } = "";
        public string timeEnd { set; get; } = "";
        public string workDate { set; get; } = "";
    }
}