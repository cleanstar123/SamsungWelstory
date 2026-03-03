using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;

namespace CMS.API.Models
{
    public class ResultModel
    {
        public string ERR_CODE  { get; set; }
        public string ERROR_MSG { get; set; }
        public string ID        { get; set; }
        public DataTable DATA   { get; set; }
    }
}