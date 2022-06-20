using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using TUFTManagement.Models;

namespace TUFTManagement.Core
{
    public class DecodeString
    {
        public string Connection(string shareCode)
        {
            string connectionString = "";

            try
            {
                string[] data = shareCode.Split('.');
                if (data.Length < 1 || string.IsNullOrEmpty(shareCode))
                {
                    connectionString = WebConfigurationManager.AppSettings["connectionStrings"];
                }
                else
                {
                    connectionString = Utility.Base64ForUrlDecode(data[0]);
                }
                
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            return connectionString;
        }
    }
}