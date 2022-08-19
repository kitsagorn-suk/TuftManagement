using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TUFTManagement.DTO
{
    public class SystemMasterDTO
    {
        public string mode { set; get; } = "";
        public int masterID { set; get; } = 0;
        public int keyID { set; get; } = 0;
        public int value { set; get; } = 0;
        public string nameEN { set; get; } = "";
        public string nameTH { set; get; } = "";
        public int order { set; get; } = 0;
        public string IsActive { set; get; } = "";
    }
}