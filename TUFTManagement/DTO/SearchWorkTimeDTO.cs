using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TUFTManagement.DTO
{
    public class SearchWorkTimeDTO
    {
        public string dateSearch { set; get; } = "";
        public string[] departmentSearch { set; get; } = { "" };
        public string prepairDepartmentSearch { set; get; } = "";
        public string[] positionSearch { set; get; } = { "" };
        public string prepairPositionSearch { set; get; } = "";
        public string[] workShiftSearch { set; get; } = { "" };
        public string prepairWorkShiftSearch { set; get; } = "";
        public int shiftSearch { set; get; } = 0;
        public string lang { set; get; } = "";
        public string paramSearch { set; get; } = "";
        public int perPage { set; get; } = 0;
        public int pageInt { set; get; } = 0;
        public int sortField { set; get; } = 0;
        public string sortType { set; get; } = "";
    }
}