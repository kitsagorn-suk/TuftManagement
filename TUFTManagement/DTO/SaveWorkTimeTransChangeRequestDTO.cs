using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TUFTManagement.DTO
{
    public class SaveWorkTimeTransChangeRequestDTO
    {
        public int transChangeID { set; get; } = 0;
        public int statusApprove { set; get; } = 0;
        public string remark { set; get; } = "";
    }
}