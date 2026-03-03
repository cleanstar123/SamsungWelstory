using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using Newtonsoft.Json;
using Microsoft.AspNet.Identity;

using CMS.API.Biz;
using CMS.API.Models;
using CMS.API.App_Code;

namespace CMS.API.Controllers
{
    [Authorize]
    public class DisplayController : BaseController
    {
        #region 디스플레이 관리

        /// <summary>
        /// 디스플레이 관리 화면
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            DisplayModel model = new DisplayModel();

            DataTable dtOS    = CodeBiz.getCodes("10005").Tables[0]; // OS
            DataTable dtUseYN = CodeBiz.getCodes("10004").Tables[0]; // USE_YN

            foreach (DataRow row in dtOS.Rows)
                ((List<SelectListItem>)model.DISPLAY_OS_LIST).Add(new SelectListItem { Text = row["CODE_NAME"].ToString(), Value = row["CODE"].ToString() });
            
            foreach (DataRow row in dtUseYN.Rows)
                ((List<SelectListItem>)model.USE_YN_LIST).Add(new SelectListItem { Text = row["CODE_NAME"].ToString(), Value = row["CODE"].ToString() });
            
            return View(model);
        }

        /// <summary>
        /// 디스플레이 조회
        /// </summary>
        /// <param name="displayModel"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Displays(DisplayModel displayModel)
        {
            ResultModel resultModel = userCheck();

            if (resultModel.ERROR_MSG != "")
                return Content(JsonConvert.SerializeObject(resultModel, Formatting.Indented));

            // 2019-05-14 디스플레이 조회 데이터 변조 체크
            if (displayModel.RESTAURANT_CODE != UserBiz.getRestaurantCode(User))
            {
                resultModel.ERROR_MSG = "비정상 데이터 교환으로 인하여 로그아웃됩니다.";
                resultModel.ERR_CODE = "9998";
                return Content(JsonConvert.SerializeObject(resultModel, Formatting.Indented));
            }
            else
                return Content(JsonConvert.SerializeObject(DisplayBiz.getDisplays(displayModel), Formatting.Indented));
        }

        /// <summary>
        /// 디스플레이 저장/수정/삭제
        /// </summary>
        /// <param name="TYPE"></param>
        /// <param name="displayModels"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult DisplayManage(string TYPE, List<DisplayModel> displayModels)
        {
            ResultModel resultModel = userCheck();

            //2019-10-22 XSS 공격에 대응하기 위해 넘어온 object에 대한 model 정보를 replace함
            CommonProperties.XSSCheck_DisplayModel(displayModels);

            if (!TYPE.ToUpper().Equals("D") && displayModels != null)
            {
                foreach (DisplayModel displayModel in displayModels)
                {
                    if (string.IsNullOrEmpty(displayModel.DISPLAY_NM))
                    {
                        resultModel.ERR_CODE = "8001";
                        resultModel.ERROR_MSG = "디스플레이 이름이 잘못되었습니다.";
                        return Content(JsonConvert.SerializeObject(resultModel, Formatting.Indented));
                    }
                }
            }

            if (resultModel.ERROR_MSG != "")
                return Content(JsonConvert.SerializeObject(resultModel, Formatting.Indented));

            //if (TYPE != "D")
            //{
            //    // 2019-05-14 특수기호 입력 체크
            //    if (CommonProperties.speSymbol(displayModels[0].DISPLAY_NM) || CommonProperties.speSymbol(displayModels[0].DISPLAY_DESC) || CommonProperties.speSymbol(displayModels[0].SCREEN_H) || CommonProperties.speSymbol(displayModels[0].SCREEN_W) || CommonProperties.speSymbol(displayModels[0].DISPLAY_IP) || CommonProperties.speSymbol(displayModels[0].DISPLAY_MAC))
            //    {
            //        resultModel.ERROR_MSG = "특수기호는 입력할 수 없습니다.";
            //        resultModel.ERR_CODE = "9999";
            //        return Content(JsonConvert.SerializeObject(resultModel, Formatting.Indented));
            //    }
            //}

            // 2019-05-14 디스플레이 조회 데이터 변조 체크
            if (displayModels[0].RESTAURANT_CODE != UserBiz.getRestaurantCode(User))
            {
                resultModel.ERROR_MSG = "비정상 데이터 교환으로 인하여 로그아웃됩니다.";
                resultModel.ERR_CODE = "9998";
                return Content(JsonConvert.SerializeObject(resultModel, Formatting.Indented));
            }

            return Content(JsonConvert.SerializeObject(DisplayBiz.displayManage(TYPE, UserBiz.getRestaurantCode(User), displayModels, UserBiz.getUserId(User)), Formatting.Indented));
        }

        /// <summary>
        /// 디스플레이 재시작
        /// </summary>
        /// <param name="displayModel"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult DisplayRestart(DisplayModel displayModel)
        {
            ResultModel resultModel = userCheck();

            //2019-10-22 XSS 공격에 대응하기 위해 넘어온 object에 대한 model 정보를 replace함
            CommonProperties.XSSCheck_DisplayModel(displayModel);

            if (resultModel.ERROR_MSG != "")
                return Content(JsonConvert.SerializeObject(resultModel, Formatting.Indented));

            return Content(JsonConvert.SerializeObject(DisplayBiz.displayRestart(UserBiz.getUserId(User), displayModel), Formatting.Indented));
        }

        #endregion

        #region 디스플레이 그룹 관리

        /// <summary>
        /// 디스플레이 그룹 관리 화면
        /// </summary>
        /// <returns></returns>
        public ActionResult DisplayGroup()
        {
            return View();
        }

        /// <summary>
        /// 디스플레이 그룹 조회
        /// </summary>
        /// <param name="displayGroupModel"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult DisplayGroups(DisplayGroupModel displayGroupModel)
        {
            ResultModel resultModel = userCheck();

            if (resultModel.ERROR_MSG != "")
                return Content(JsonConvert.SerializeObject(resultModel, Formatting.Indented));

            if (displayGroupModel.RESTAURANT_CODE != UserBiz.getRestaurantCode(User))
            {
                resultModel.ERROR_MSG = "비정상 데이터 교환으로 인하여 로그아웃됩니다.";
                resultModel.ERR_CODE = "9998";
                return Content(JsonConvert.SerializeObject(resultModel, Formatting.Indented));
            }
            else
                return Content(JsonConvert.SerializeObject(DisplayBiz.getDisplayGroups(displayGroupModel), Formatting.Indented));
        }

        /// <summary>
        /// 디스플레이 그룹 저장/수정/삭제
        /// </summary>
        /// <param name="TYPE"></param>
        /// <param name="displayGroupModels"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult DisplayGroupManage(string TYPE, List<DisplayGroupModel> displayGroupModels)
        {
            ResultModel resultModel = userCheck();

            //2019-10-22 XSS 공격에 대응하기 위해 넘어온 object에 대한 model 정보를 replace함
            CommonProperties.XSSCheck_DisplayGroupModel(displayGroupModels);
            if (!TYPE.ToUpper().Equals("D") && displayGroupModels != null)
            {
                foreach (DisplayGroupModel displayGroupModel in displayGroupModels)
                {
                    if (string.IsNullOrEmpty(displayGroupModel.DISPLAY_GROUP_NM))
                    {
                        resultModel.ERR_CODE = "8001";
                        resultModel.ERROR_MSG = "디스플레이 그룹 이름이 잘못되었습니다.";
                        return Content(JsonConvert.SerializeObject(resultModel, Formatting.Indented));
                    }
                }
            }

            if (resultModel.ERROR_MSG != "")
                return Content(JsonConvert.SerializeObject(resultModel, Formatting.Indented));

            //if (TYPE != "D")
            //{
            //    // 2019-05-14 특수기호 입력 체크
            //    if (CommonProperties.speSymbol(displayGroupModels[0].DISPLAY_GROUP_NM) || CommonProperties.speSymbol(displayGroupModels[0].DISPLAY_GROUP_DESC))
            //    {
            //        resultModel.ERROR_MSG = "특수기호는 입력할 수 없습니다.";
            //        resultModel.ERR_CODE = "9999";
            //        return Content(JsonConvert.SerializeObject(resultModel, Formatting.Indented));
            //    }
            //}

            if (displayGroupModels[0].RESTAURANT_CODE != UserBiz.getRestaurantCode(User))
            {
                resultModel.ERROR_MSG = "비정상 데이터 교환으로 인하여 로그아웃됩니다.";
                resultModel.ERR_CODE = "9998";
                return Content(JsonConvert.SerializeObject(resultModel, Formatting.Indented));
            }
            else
                return Content(JsonConvert.SerializeObject(DisplayBiz.displayGroupManage(TYPE, UserBiz.getRestaurantCode(User), displayGroupModels, UserBiz.getUserId(User)), Formatting.Indented));
        }

        /// <summary>
        /// 디스플레이 그룹에 속한 디스플레이 목록 및 속하지 않은 디스플레이 목록 조회
        /// </summary>
        /// <param name="displayGroupModel"></param>
        /// <returns></returns>
        [HttpPost]
        public string DisplayMaps(DisplayGroupModel displayGroupModel)
        {
            ResultModel resultModel = userCheck();

            if (resultModel.ERROR_MSG != "")
                return JsonConvert.SerializeObject(resultModel, Formatting.Indented);

            displayGroupModel.RESTAURANT_CODE = UserBiz.getRestaurantCode(User);
            return JsonConvert.SerializeObject(DisplayBiz.getDisplayMaps(displayGroupModel), Formatting.Indented);
        }

        /// <summary>
        /// 디스플레이 그룹에 속한 디스플레이 저장/수정/삭제
        /// </summary>
        /// <param name="displayGroupModel"></param>
        /// <param name="displayMapModels"></param>
        /// <returns></returns>
        [HttpPost]
        public string DisplayMapManage(DisplayGroupModel displayGroupModel, List<DisplayMapModel> displayMapModels)
        {
            ResultModel resultModel = userCheck();

            //2019-10-22 XSS 공격에 대응하기 위해 넘어온 object에 대한 model 정보를 replace함
            CommonProperties.XSSCheck_DisplayGroupModel(displayGroupModel);

            if (resultModel.ERROR_MSG != "")
                return JsonConvert.SerializeObject(resultModel, Formatting.Indented);

            return JsonConvert.SerializeObject(DisplayBiz.displayMapManage(displayGroupModel, displayMapModels, User.Identity.GetUserId<string>()), Formatting.Indented);
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

        #endregion
    }
}