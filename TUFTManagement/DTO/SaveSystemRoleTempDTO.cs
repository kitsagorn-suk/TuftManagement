using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TUFTManagement.DTO
{
    public class SaveSystemRoleTempDTO
    {
        public int objectID { set; get; } = 0;
        public int parentID { set; get; } = 0;
        public string objectName { set; get; } = "";
        public int isActive { set; get; } = 0;
    }
}