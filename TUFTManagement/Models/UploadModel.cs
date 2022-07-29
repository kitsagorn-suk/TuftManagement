using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TUFTManagement.Models
{
    public class UploadModel
    {
        public bool success { get; set; }
        public MsgModel msg { get; set; }
        public _ServiceUploadData data { get; set; }
    }

    public class _ServiceUploadData
    {
        public string fileName { get; set; } = "";
        public string fileCode { get; set; } = "";
        public string fileSize { get; set; } = "";
    }
}