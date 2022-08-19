using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TUFTManagement.DTO
{
    public class RequestDTO
    {
        public int id { set; get; } = 0;
        public int userID { set; get; } = 0;
        public string fileCode { set; get; } = "";

    }
}