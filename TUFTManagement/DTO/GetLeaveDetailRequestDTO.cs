using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TUFTManagement.DTO
{
    public class GetLeaveDetailRequestDTO
    {
        public int leaveID { set; get; } = 0;
        public string pLang { set; get; } = "";
    }
}