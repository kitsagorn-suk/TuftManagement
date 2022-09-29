using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TUFTManagement.DTO
{
    public class SearchPayRollDTO
    {
        public string paramSearch { set; get; } = "";
        public string dateSearch { set; get; } = "";
        public string[] installmentSearch { set; get; } = { "" };
        public string prepairInstallmentSearch { set; get; } = "";
        public int perPage { set; get; } = 0;
        public int pageInt { set; get; } = 0;
        public int sortField { set; get; } = 0;
        public string sortType { set; get; } = "a";
    }
}