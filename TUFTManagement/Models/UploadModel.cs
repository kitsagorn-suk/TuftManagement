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
        public string fileCode { get; set; } = "";
        public List<_fileDetails> fileDetails { get; set; }
    }

    public class _fileDetails
    {
        public int id { get; set; } = 0;
        public string fileName { get; set; } = "";
        public string fileUrl { get; set; } = "";
        public string fileSize { get; set; } = "";
    }

}