using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TUFTManagement.DTO
{
    public class MasterDataPositionDTO
    {
        public int masterID { set; get; } = 0;
        public string mode { set; get; } = "";
        public string nameEN { set; get; } = "";
        public string nameTH { set; get; } = "";
        public string isActive { set; get; } = "0";
        public int departmentID { set; get; } = 0;
        

    }
}