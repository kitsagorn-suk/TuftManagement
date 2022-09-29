using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TUFTManagement.DTO
{
    public class MasterDataDTO
    {
        public int masterID { set; get; } = 0;
        public string mode { set; get; } = "";
        public string nameEN { set; get; } = "";
        public string nameTH { set; get; } = "";
        public int deptID { set; get; } = 0;
        public string isActive { set; get; } = "";
        public int IsCancel { set; get; } = 0;
        public string keyName { set; get; } = "";
    }
}