using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace TUFTManagement.Models
{
    public class GetAccessRoleModel
    {
        public bool success { get; set; }
        public MsgModel msg { get; set; }
        public GetAccessRole data { get; set; }

        public class GetAccessRole
        {
            public List<AccessRole> accessList { get; set; }
        }

            
    }
    
}