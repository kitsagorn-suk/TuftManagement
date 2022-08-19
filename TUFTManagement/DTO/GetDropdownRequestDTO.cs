using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TUFTManagement.DTO
{
    public class GetDropdownRequestDTO
    {
        public string moduleName { set; get; } = "";
        public int provinceID { set; get; } = 0;
        public int districtID { set; get; } = 0;
        public int departmentID { set; get; } = 0;
    }
}