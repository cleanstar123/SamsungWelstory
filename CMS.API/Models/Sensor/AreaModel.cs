using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CMS.API.Models
{
    public class AreaModel
    {
        public string RESTAURANT_CODE { get; set; }
        public string AREA_NM { get; set; }
        public string AREA_CD { get; set; }
        public string NORMAL { get; set; }
        public string CROWDED { get; set; }
        public string UNCROWDED { get; set; }
        public string REG_ID { get; set; }
        public string REG_DTM { get; set; }
        public string MOD_ID { get; set; }
        public string MOD_DTM { get; set; }
    }
}