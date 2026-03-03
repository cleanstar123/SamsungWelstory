using CMS.API.Biz;
using CMS.API.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;


namespace CMS.API.Controllers
{
    public class WelmenuController : Controller
    {

        /// <summary>
        /// 게시 메뉴 조회
        /// </summary>
        /// <param name="welmenuModel"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        public string list(WelmenuModel welmenuModel)
        {
            welmenuModel.RESTAURANT_CODE = UserBiz.getRestaurantCode(User);
            return JsonConvert.SerializeObject(WelmenuBiz.welmenuList(welmenuModel), Formatting.Indented);
        }

        /// <summary>
        /// 평가 결과
        /// </summary>
        /// <param name="welmenuModel"></param>
        /// <returns></returns>
        [HttpPost]
        public string getSatisfactionlist(List<WelmenuModel> welmenuModels)
        {
            return JsonConvert.SerializeObject(WelmenuBiz.getSatisfactionlist(welmenuModels), Formatting.Indented);
        }

        /// <summary>
        /// 평가 결과2
        /// </summary>
        /// <param name="welmenuModel"></param>
        /// <returns></returns>
        [HttpPost]
        public string getSatisfactionlist2(List<WelmenuModel> welmenuModels)
        {
            return JsonConvert.SerializeObject(WelmenuBiz.getSatisfactionlist2(welmenuModels), Formatting.Indented);
        }

        /// <summary>
        /// 메뉴 평가 하기
        /// </summary>
        /// <param name="welmenuModel"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult setSatisfaction(WelmenuModel welmenuModel)
        {
            return Content(JsonConvert.SerializeObject(WelmenuBiz.setSatisfaction(welmenuModel), Formatting.Indented));
        }

        [HttpPost]
        public ActionResult getDate(string dateFormat)
        {
            return Content(JsonConvert.SerializeObject(DateTime.Now.ToString(string.IsNullOrEmpty(dateFormat) ? "yyyy년 MM월 dd일" : dateFormat)));
        }

        [HttpPost]
        public ActionResult GetScheduleTemplateMapList(string RESTAURANT_CODE, string SCHEDULE_ID)
        {
            return Content(JsonConvert.SerializeObject(ScheduleBiz.getScheduleTemplateMapList(RESTAURANT_CODE, SCHEDULE_ID), Formatting.Indented));
        }
    }
}