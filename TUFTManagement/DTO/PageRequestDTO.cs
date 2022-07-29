using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TUFTManagement.DTO
{
    public class PageRequestDTO
    {
        public string paramSearch { set; get; } = "";
        public int departmentSearch { set; get; } = 0;
        public int empTypeSearch { set; get; } = 0;
        public string statusSearch { set; get; } = "";
        public string lang { set; get; } = "";
        public int perPage { set; get; } = 0;
        public int pageInt { set; get; } = 0;
        public int sortField { set; get; } = 0;
        public string sortType { set; get; } = "a";
    }
}