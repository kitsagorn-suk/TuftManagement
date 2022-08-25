using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TUFTManagement.DTO
{
    public class GetHistoryWorkShiftTimeDTO
    {
        public int empId { set; get; } = 0;
        public string date { set; get; }
        public int perPage { set; get; } = 0;
        public int pageInt { set; get; } = 0;
        public int sortField { set; get; } = 0;
        public string sortType { set; get; } = "";
    }
}