using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TUFTManagement.DTO
{
    public class HeadersDTO
    {
        public string authHeader { set; get; } = "";
        public string lang { set; get; } = "";
        public string fromProject { set; get; } = "";
        public string shareCode { set; get; } = "";    }
}