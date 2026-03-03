using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CMS.API.Models
{
    public class WelmenuModel
    {
        public string AREA_ID         { get; set; }  // 평가 결과를 보여주는 컨텐츠 영역 아이디
        public string RESTAURANT_CODE { get; set; }
        public string MENU_DT         { get; set; }
        public string MEAL_TYPE       { get; set; }
        public string MEAL_TYPE_NM    { get; set; }
        public string PHOTO_CD        { get; set; }
        public string MENU_NM         { get; set; }
        public string MENU_NM_ENG     { get; set; }
        public string MENU_PRICE      { get; set; }
        public string MENU_KCAL       { get; set; }
        public string SIDE_DISH       { get; set; }
        public string HALL_NO         { get; set; }
        public string MENU_TYPE       { get; set; }
        public string COURSE_TYPE     { get; set; }
        public string MENU_CODE       { get; set; }
        public string EVAL_SCORE      { get; set; }
        public string SEQ             { get; set; }  // 메뉴 게시의 경우 메뉴코드가 키가 아님
    }
}