using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TUFTManagement.DTO
{
    public class SaveSystemRoleAssignDTO
    {
        public int id { set; get; } = 0;
        public int positionID { set; get; } = 0;
        public int objectID { set; get; } = 0;
        public int isActive { set; get; } = 0;
    }
    
}