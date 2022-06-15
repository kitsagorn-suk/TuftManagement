using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TUFTManagement.Models
{
    public class MsgModel
    {
        public MsgModel() { }
        public MsgModel(string text)
        {
            this.text = text;
        }
        public MsgModel(string text, int code)
        {
            this.text = text;
            this.code = code;
        }
        public int code { get; set; } = 0;
        public string text { get; set; } = "";
        public string topic { get; set; } = "";
    }
}