using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TUFTManagement.DTO
{
    public class SaveSystemRoleAssignDTO
    {
        public int positionID { set; get; } = 0;
        public List<SaveSystemRoleTemp> listTemp { set; get; }
    }
    public class SaveSystemRoleTemp
    {
        public string objID { set; get; } = "";
        public string parentID { set; get; } = "";
        public string objName { set; get; } = "";
        public int isActive { set; get; } = 0;
    }
}