using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TUFTManagement.DTO
{
    public class SaveSystemRoleDTO
    {
        public string mode { set; get; } = "";
        public string objID { set; get; } = "";
        public string parentID { set; get; } = "";
        public string objName { set; get; } = "";
        public int isActive { set; get; } = 0;
        public List<SaveSystemRole> listPosition { set; get; }
    }
    public class SaveSystemRole
    {
        public int positionID { set; get; } = 0;
        public int isActive { set; get; } = 0;
    }
}