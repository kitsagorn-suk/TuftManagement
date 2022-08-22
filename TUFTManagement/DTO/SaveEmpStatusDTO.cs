using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TUFTManagement.DTO
{
    public class SaveEmpStatusDTO
    {
        public int userID { set; get; } = 0;
        public int employmentStatusID { set; get; } = 0;
        public string imageEmploymentCode { set; get; } = "";
    }
}