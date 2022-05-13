using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TUFTManagement.DTO
{
    public class SaveBodySetRequestDTO
    {
        public int id { set; get; } = 0;
        public float height { set; get; } = 0;
        public float weight { set; get; } = 0;
        public int chest { set; get; } = 0;
        public int waist { set; get; } = 0;
        public int hip { set; get; } = 0;

    }
}