using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TUFTManagement.DTO
{
    public class SearchWorkTimePendingDTO
    {
        public string paramSearch { set; get; } = "";

        public int[] departmentList { set; get; }
        public int[] positionList { set; get; }
        public int[] workShiftIDList { set; get; }

        public string prepairDepartmentList { set; get; }
        public string prepairPositionList { set; get; }
        public string prepairWorkShiftIDList { set; get; }
        public string dateSearch { set; get; } = "";
        public string lang { set; get; } = "";
        public int perPage { set; get; } = 0;
        public int pageInt { set; get; } = 0;
        public int sortField { set; get; } = 0;
        public string sortType { set; get; } = "";
    }
}