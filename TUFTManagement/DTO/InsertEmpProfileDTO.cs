using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TUFTManagement.DTO
{
    public class InsertEmpProfileDTO
    {
        public string empCode { set; get; } = "";
        public string userName { set; get; } = "";
        public string password { set; get; } = "";
        public string identityCard { set; get; } = "";
        public int titleID { set; get; } = 0;
        public string firstNameEN { set; get; } = "";
        public string lastNameEN { set; get; } = "";
        public string firstNameTH { set; get; } = "";
        public string lastNameTH { set; get; } = "";
        public string nickName { set; get; } = "";
        public string contact { set; get; } = "";
        public int positionID { set; get; } = 0;
        public string personalCode { set; get; } = "";
        public int personalNO { set; get; } = 0;
        public string dateOfBirth { set; get; } = "";
        public int productID { set; get; } = 0;
        public string joinDate { set; get; } = "";
        public string proPassDate { set; get; } = "";
        public decimal monthlySalary { set; get; } = 0;
        public decimal dailySalary { set; get; } = 0;
        public string tokenID { set; get; } = "";
        public int roleID { set; get; } = 0;
        public string condition { set; get; } = "";
    }
}