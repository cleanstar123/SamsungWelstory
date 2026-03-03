using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CMS.API.Models
{
    public class ScheduleTemplateMapModel
    {
        public string SEQ          { get; set; }
        public string TEMPLATE_ID  { get; set; }
        public string TEMPLATE_URL { get; set; }
        public string PLAY_TIME    { get; set; }   // 재생시간(초)
    }
}