using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CMS.API.Models
{
    public class ScheduleModel
    {
        public string RESTAURANT_CODE  { get; set; }
        public string SCHEDULE_ID      { get; set; }
        public string SCHEDULE_TYPE    { get; set; }
        public string SCHEDULE_NM      { get; set; }
        public string SCHEDULE_DESC    { get; set; }
        public string START_DATE       { get; set; }
        public string END_DATE         { get; set; }
        public string START_TIME       { get; set; }
        public string END_TIME         { get; set; }
        public string PLAY_TIME        { get; set; }
        public string MON_YN           { get; set; }
        public string TUE_YN           { get; set; }
        public string WED_YN           { get; set; }
        public string THU_YN           { get; set; }
        public string FRI_YN           { get; set; }
        public string SAT_YN           { get; set; }
        public string SUN_YN           { get; set; }
        public string DISPLAY_ID       { get; set; }

        public string ROLLING_YN       { get; set; }
        public string URL              { get; set; }  // 롤링 N일 경우 템플릿 URL, 롤링 Y일 경우 랩퍼 템플릿 URL
    }
}