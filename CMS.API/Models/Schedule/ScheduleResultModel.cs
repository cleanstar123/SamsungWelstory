using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CMS.API.Models
{
    public class ScheduleResultModel
    {
        // 달력에 바인딩할 데이터 (플러그인에 맞게 값을 설정)
        public string identifier      { get; set; } // 스케줄 아이디
        public string start           { get; set; } // 스케줄 시작일자
        public string end             { get; set; } // 스케줄 종료일자
        public string title           { get; set; } // 스케줄명
        public string description     { get; set; } // 스케줄 설명
        
        public string tag             { get; set; } // 템플릿 명
        public string people          { get; set; } 
        public string desc            { get; set; } // 디스플레이 목록
        public string backgroundColor { get; set; } // 스케줄 배경색
        public string textColor       { get; set; } // 스케줄 글자색

        public string icon            { get; set; } // fa fa-exclamation-triangle  fa fa-moon-o
    }
}