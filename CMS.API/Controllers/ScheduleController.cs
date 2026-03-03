using System;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Data;
using System.IO;
using Newtonsoft.Json;

using CMS.API.Biz;
using CMS.API.Models;
using CMS.API.App_Code;

namespace CMS.API.Controllers
{
    public class ScheduleController : BaseController
    {
        /// <summary>
        /// 스케줄 관리 화면
        /// </summary>
        /// <returns></returns>
        [Authorize]
        public ActionResult Index()
        {
            DisplayModel displayModel = new DisplayModel();
            displayModel.RESTAURANT_CODE = UserBiz.getRestaurantCode(User);

            DataSet ds = DisplayBiz.getDisplays(displayModel);

            ((List<SelectListItem>)displayModel.DISPLAY_LIST).Add(new SelectListItem { Text = "전체", Value = "" });

            foreach (DataRow row in ds.Tables[0].Rows)
            {
                ((List<SelectListItem>)displayModel.DISPLAY_LIST).Add(new SelectListItem { Text = row["DISPLAY_NM"].ToString(), Value = row["DISPLAY_ID"].ToString() });
            }

            return View(displayModel);
        }

        /// <summary>
        /// 스케줄 등록/수정/삭제 팝업 화면
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize]
        public ActionResult ManageSchedule(string id)
        {
            return View();
        }

        /// <summary>
        /// 스케줄 조회 (단일)
        /// </summary>
        /// <param name="scheduleModel"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        public string GetSchedule(ScheduleModel scheduleModel)
        {
            ResultModel resultModel = userCheck();

            if (resultModel.ERROR_MSG != "")
                return JsonConvert.SerializeObject(resultModel, Formatting.Indented);

            scheduleModel.RESTAURANT_CODE = UserBiz.getRestaurantCode(User);
            return JsonConvert.SerializeObject(ScheduleBiz.getSchedule(scheduleModel), Formatting.Indented);
        }

        /// <summary>
        /// 스케줄 조회 (다중)
        /// </summary>
        /// <param name="scheduleModel"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        public string GetSchedules(ScheduleModel scheduleModel)
        {
            ResultModel resultModel = userCheck();

            if (resultModel.ERROR_MSG != "")
                return JsonConvert.SerializeObject(resultModel, Formatting.Indented);

            scheduleModel.RESTAURANT_CODE = UserBiz.getRestaurantCode(User);
            return JsonConvert.SerializeObject(ScheduleBiz.getSchedules(scheduleModel), Formatting.Indented);
        }

        /// <summary>
        /// 스케줄 저장/수정/삭제
        /// </summary>
        /// <param name="TYPE"></param>
        /// <param name="scheduleModel"></param>
        /// <param name="displayModels"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        public string ManageSchedule(string TYPE, ScheduleModel scheduleModel, List<ScheduleTemplateMapModel> scheduleTemplateMapModels, List<DisplayModel> displayModels)
        {
            ResultModel resultModel = userCheck();

            //2019-10-22 XSS 공격에 대응하기 위해 넘어온 object에 대한 model 정보를 replace함
            CommonProperties.XSSCheck_ScheduleModel(scheduleModel);
            CommonProperties.XSSCheck_ScheduleTemplateMapModel(scheduleTemplateMapModels);
            CommonProperties.XSSCheck_DisplayModel(displayModels);
            if (!TYPE.ToUpper().Equals("D") && string.IsNullOrEmpty(scheduleModel.SCHEDULE_NM))
            {
                resultModel.ERR_CODE = "8001";
                resultModel.ERROR_MSG = "스케줄 이름이 잘못되었습니다.";
                return JsonConvert.SerializeObject(resultModel, Formatting.Indented);
            }

            if (resultModel.ERROR_MSG != "")
                return JsonConvert.SerializeObject(resultModel, Formatting.Indented);

            try
            {
                if (TYPE.ToUpper().Equals("I") || TYPE.ToUpper().Equals("U"))
                {
                    // 2019-05-14 특수기호 입력 체크
                    //if (CommonProperties.speSymbol(scheduleModel.SCHEDULE_NM) || CommonProperties.speSymbol(scheduleModel.SCHEDULE_DESC))
                    //{
                    //    resultModel.ERROR_MSG = "특수기호는 입력할 수 없습니다.";
                    //    resultModel.ERR_CODE = "9999";
                    //    return JsonConvert.SerializeObject(resultModel, Formatting.Indented);
                    //}

                    if (scheduleTemplateMapModels.Count == 0)
                    {
                        scheduleModel.ROLLING_YN = "N";
                    }
                    else if (scheduleTemplateMapModels.Count == 1)
                    {
                        scheduleModel.URL = scheduleTemplateMapModels[0].TEMPLATE_URL;
                        scheduleModel.ROLLING_YN = "N";
                    }
                    else
                    {
                        scheduleModel.ROLLING_YN = "Y";
                    }
                }

                List<ScheduleModel> scheduleModels = new List<ScheduleModel>();
                scheduleModels.Add(scheduleModel);

                resultModel = ScheduleBiz.ManageSchedule(TYPE, UserBiz.getUserId(User), UserBiz.getRestaurantCode(User), scheduleModels, scheduleTemplateMapModels, displayModels);

                // 롤링 스케줄 생성시 경우 랩퍼 템플릿 생성
                if ((TYPE.ToUpper().Equals("I") || TYPE.ToUpper().Equals("U")) && scheduleModel.ROLLING_YN.Equals("Y"))
                {
                    if (resultModel.ERR_CODE.Equals("0") && !string.IsNullOrEmpty(resultModel.ID))
                    {
                        string wrapperHtml = System.IO.File.ReadAllText(Path.Combine(Server.MapPath("~/"), CommonProperties.TEMPLATE_WRAPPER_SAVE_PATH, "wrapper.html"), System.Text.Encoding.UTF8).Replace("{RESTAURANT_CODE}", UserBiz.getRestaurantCode(User)).Replace("{SCHEDULE_ID}", resultModel.ID);

                        // 저장 경로가 존재하지 않을 경우 생성
                        if (!Directory.Exists(Path.Combine(Server.MapPath("~/"), CommonProperties.TEMPLATE_WRAPPER_SAVE_PATH, UserBiz.getRestaurantCode(User))))
                        {
                            System.IO.Directory.CreateDirectory(Path.Combine(Server.MapPath("~/"), CommonProperties.TEMPLATE_WRAPPER_SAVE_PATH, UserBiz.getRestaurantCode(User)));
                        }

                        System.IO.File.WriteAllText(Path.Combine(Server.MapPath("~/"), CommonProperties.TEMPLATE_WRAPPER_SAVE_PATH, UserBiz.getRestaurantCode(User), string.Format("S{0}.html", resultModel.ID)), wrapperHtml, System.Text.Encoding.UTF8);

                        scheduleModels[0].SCHEDULE_ID = resultModel.ID;
                        scheduleModels[0].URL = string.Format("{0}/{1}/{2}/{3}", CommonProperties.HTTPS_DOMAIN_URL, CommonProperties.TEMPLATE_WRAPPER_SAVE_PATH, UserBiz.getRestaurantCode(User), string.Format("S{0}.html", resultModel.ID)); // Path.Combine(CommonProperties.HTTPS_DOMAIN_URL, CommonProperties.TEMPLATE_WRAPPER_SAVE_PATH, UserBiz.getRestaurantCode(User), string.Format("S{0}.html", resultModel.ID));

                        resultModel = ScheduleBiz.ManageSchedule("U", UserBiz.getUserId(User), UserBiz.getRestaurantCode(User), scheduleModels, scheduleTemplateMapModels, displayModels);
                    }
                    else
                    {
                        throw new Exception(resultModel.ERROR_MSG);
                    }
                }
                else if (TYPE.ToUpper().Equals("D"))
                {
                    string deleteFile = Path.Combine(Server.MapPath("~/"), CommonProperties.TEMPLATE_WRAPPER_SAVE_PATH, UserBiz.getRestaurantCode(User), string.Format("S{0}.html", scheduleModel.SCHEDULE_ID));

                    // 삭제할 파일이 존재할 경우 삭제
                    if (System.IO.File.Exists(deleteFile))
                        System.IO.File.Delete(deleteFile);
                }
            }
            catch (Exception ex)
            {

                resultModel.ERR_CODE = "9999";
                resultModel.ERROR_MSG = ex.Message;
            }

            return JsonConvert.SerializeObject(resultModel, Formatting.Indented);
        }

        /// <summary>
        /// 스케줄 중복 체크
        /// </summary>
        /// <param name="TYPE"></param>
        /// <param name="scheduleModel"></param>
        /// <param name="displayModels"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        public string CheckScheduleOverlap(string TYPE, ScheduleModel scheduleModel, List<DisplayModel> displayModels)
        {
            List<ScheduleModel> scheduleModels = new List<ScheduleModel>();
            scheduleModels.Add(scheduleModel);

            return JsonConvert.SerializeObject(ScheduleBiz.CheckScheduleOverlap(TYPE, UserBiz.getUserId(User), UserBiz.getRestaurantCode(User), scheduleModels, displayModels), Formatting.Indented);
        }

        public ResultModel userCheck()
        {
            ResultModel resultModel = new ResultModel();

            if (Session["UserID"] == null)
            {
                resultModel.ERROR_MSG = "세션이 종료되어 로그인 화면으로 이동합니다.";
                resultModel.ERR_CODE = "9998";
            }
            else if (UserBiz.getUserId(User) != Session["UserID"].ToString())
            {
                resultModel.ERROR_MSG = "비정상 데이터 교환으로 인하여 로그아웃됩니다.";
                resultModel.ERR_CODE = "9998";
            }
            else
            {
                resultModel.ERROR_MSG = "";
                resultModel.ERR_CODE = "0";
            }

            return resultModel;
        }
    }
}