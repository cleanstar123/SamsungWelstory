using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CMS.API.Models
{
    public class CongestionTermModel
    {   
        public string S_DATE { get; set; }
        public string E_DATE { get; set; }
        public string RESTAURANT_CODE { get; set; }
        public string INTERVAL { get; set; }
    }
    public class CongestionResultModel
    {
        public string COLLECT_TIME { get; set; }
        public string AREA_NM { get; set; }
        public string SERIAL_NO { get; set; }
        public string COUNT { get; set; }
    }
}