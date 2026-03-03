using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Mvc;

namespace CMS.API.Models
{
    public class SensorModel
    {
        public string SERIAL_NO { get; set; }
        public string RESTAURANT_CODE { get; set; }
        public string USE_YN { get; set; }
        public string REG_DTM { get; set; }
        public string MOD_DTM { get; set; }
        public string REG_ID { get; set; }
        public string MOD_ID { get; set; }

        public string USE_YN_SELECTED { get; set; }
        public IEnumerable<SelectListItem> USE_YN_LIST = new List<SelectListItem>();
    }
    public class SensorDataModel
    {
        public string SERIAL_NO { get; set; }
        public string RESTAURANT_CODE { get; set; }
        public string AREA_NM { get; set; }
        public string STATUS { get; set; }
        public string COUNT_IN { get; set; }
    }
    public class ReportCodeModel
    {
        public string TIME_SELECTED { get; set; }
        public IEnumerable<SelectListItem> TIME_LIST = new List<SelectListItem>();
    }
}