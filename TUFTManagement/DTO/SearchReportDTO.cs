using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TUFTManagement.DTO
{
    public class SearchReportDTO
    {
        public int userID { set; get; } = 0;
        public string paramSearch { set; get; } = "";
        public string[] departmentSearch { set; get; } = { "" };
        public string prepairDepartmentSearch { set; get; } = "";
        public string[] positionSearch { set; get; } = { "" };
        public string prepairPositionSearch { set; get; } = "";
        public string[] empTypeSearch { set; get; } = { "" };
        public string prepairEmpTypeSearch { set; get; } = "";
        public string[] empStatusSearch { set; get; } = { "" };
        public string prepairEmpStatusSearch { set; get; } = "";
        public int salaryFrom { set; get; } = 0;
        public int salaryTo { set; get; } = 0;
        public string dateFrom { set; get; } = "";
        public string dateTo { set; get; } = "";
        public string lang { set; get; } = "";
        public int perPage { set; get; } = 0;
        public int pageInt { set; get; } = 0;
        public int sortField { set; get; } = 0;
        public string sortType { set; get; } = "a";
    }
}