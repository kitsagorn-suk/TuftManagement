using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TUFTManagement.Models
{
    public class ReturnMessageModel
    {
        public bool success { get; set; }
        public MsgModel msg { get; set; }
    }
}