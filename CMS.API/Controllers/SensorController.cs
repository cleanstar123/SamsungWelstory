using CMS.API.App_Code;
using CMS.API.Biz;
using CMS.API.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CMS.API.Controllers
{
    [Authorize]
    public class SensorController : BaseController
    {
        /// <summary>
        /// 센서 관리 화면
        /// </summary>
        /// <returns></returns>
        public ActionResult sensormanagement()
        {
            SensorModel model = new SensorModel();

            DataTable dtUseYN = CodeBiz.getCodes("10004").Tables[0]; // USE_YN

            foreach (DataRow row in dtUseYN.Rows)
                ((List<SelectListItem>)model.USE_YN_LIST).Add(new SelectListItem { Text = row["CODE_NAME"].ToString(), Value = row["CODE"].ToString() });

            return View(model);
        }
        /// <summary>
        /// 센서 조회
        /// </summary>
        /// <param name="areaModel"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult SensorList(AreaModel areaModel)
        {
            ResultModel resultModel = userCheck();

            if (resultModel.ERROR_MSG != "")
                return Content(JsonConvert.SerializeObject(resultModel, Formatting.Indented));
            
            if (areaModel.RESTAURANT_CODE != UserBiz.getRestaurantCode(User))
            {
                resultModel.ERROR_MSG = "비정상 데이터 교환으로 인하여 로그아웃됩니다. 센서리스트 조회" + areaModel.RESTAURANT_CODE  + " /  "+ UserBiz.getRestaurantCode(User);
                resultModel.ERR_CODE = "9998";
                return Content(JsonConvert.SerializeObject(resultModel, Formatting.Indented));
            }
            else
                return Content(JsonConvert.SerializeObject(SensorBiz.getSensorList(areaModel), Formatting.Indented));
        }
        /// <summary>
        /// 센서 저장/수정/삭제
        /// </summary>
        /// <param name="TYPE"></param>
        /// <param name="sensorModels"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult SensorManage(string TYPE, List<SensorModel> sensorModels)
        {
            ResultModel resultModel = userCheck();

            if (!TYPE.ToUpper().Equals("D") && sensorModels != null)
            {
                foreach (SensorModel sensorModel in sensorModels)
                {
                    if (string.IsNullOrEmpty(sensorModel.SERIAL_NO))
                    {
                        resultModel.ERR_CODE = "8001";
                        resultModel.ERROR_MSG = "시리얼번호가 잘못되었습니다.";
                        return Content(JsonConvert.SerializeObject(resultModel, Formatting.Indented));
                    }
                }
            }

            if (resultModel.ERROR_MSG != "")
                return Content(JsonConvert.SerializeObject(resultModel, Formatting.Indented));


            // 2019-05-14 디스플레이 조회 데이터 변조 체크
            if (sensorModels[0].RESTAURANT_CODE != UserBiz.getRestaurantCode(User))
            {
                resultModel.ERROR_MSG = "비정상 데이터 교환으로 인하여 로그아웃됩니다.";
                resultModel.ERR_CODE = "9998";
                return Content(JsonConvert.SerializeObject(resultModel, Formatting.Indented));
            }
            
            return Content(JsonConvert.SerializeObject(SensorBiz.sensorManage(TYPE, UserBiz.getRestaurantCode(User), sensorModels, UserBiz.getUserId(User)), Formatting.Indented));
        }
        /// <summary>
        /// 영역 조회
        /// </summary>
        /// <param name="areaModel"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult AreaList(AreaModel areaModel)
        {
            ResultModel resultModel = userCheck();

            if (resultModel.ERROR_MSG != "")
                return Content(JsonConvert.SerializeObject(resultModel, Formatting.Indented));
            
            if (areaModel.RESTAURANT_CODE != UserBiz.getRestaurantCode(User))
            {
                resultModel.ERROR_MSG = "비정상 데이터 교환으로 인하여 로그아웃됩니다.";
                resultModel.ERR_CODE = "9998";
                return Content(JsonConvert.SerializeObject(resultModel, Formatting.Indented));
            }
            else
            return Content(JsonConvert.SerializeObject(SensorBiz.getAreaList(areaModel), Formatting.Indented));
        }
        /// <summary>
        /// 매핑 조회
        /// </summary>
        /// <param name="sensorAreaModel"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult SensorAreaList(SensorAreaModel sensorAreaModel)
        {
            ResultModel resultModel = userCheck();

            if (resultModel.ERROR_MSG != "")
                return Content(JsonConvert.SerializeObject(resultModel, Formatting.Indented));

            if (sensorAreaModel.RESTAURANT_CODE != UserBiz.getRestaurantCode(User))
            {
                resultModel.ERROR_MSG = "비정상 데이터 교환으로 인하여 로그아웃됩니다.";
                resultModel.ERR_CODE = "9998";
                return Content(JsonConvert.SerializeObject(resultModel, Formatting.Indented));
            }
            else
                return Content(JsonConvert.SerializeObject(SensorBiz.getSensorAreaList(sensorAreaModel), Formatting.Indented));
        }
        /// <summary>
        /// 영역 저장/수정/삭제
        /// </summary>
        /// <param name="TYPE"></param>
        /// <param name="areaModels"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult AreaManage(string TYPE, List<AreaModel> areaModels)
        {
            ResultModel resultModel = userCheck();

            if (!TYPE.ToUpper().Equals("D") && areaModels != null)
            {
                if (string.IsNullOrEmpty(areaModels[0].AREA_NM))
                {
                        resultModel.ERR_CODE = "8001";
                        resultModel.ERROR_MSG = "영역이름이 잘못되었습니다.";
                        return Content(JsonConvert.SerializeObject(resultModel, Formatting.Indented));
                }
            }

            if (resultModel.ERROR_MSG != "")
                return Content(JsonConvert.SerializeObject(resultModel, Formatting.Indented));


            // 2019-05-14 디스플레이 조회 데이터 변조 체크
            if (areaModels[0].RESTAURANT_CODE != UserBiz.getRestaurantCode(User))
            {
                resultModel.ERROR_MSG = "비정상 데이터 교환으로 인하여 로그아웃됩니다.";
                resultModel.ERR_CODE = "9998";
                return Content(JsonConvert.SerializeObject(resultModel, Formatting.Indented));
            }
            return Content(JsonConvert.SerializeObject(SensorBiz.areaManage(TYPE, UserBiz.getRestaurantCode(User), areaModels, UserBiz.getUserId(User)), Formatting.Indented));
        }
        /// <summary>
        /// 센서-영역 매핑 저장/삭제
        /// </summary>
        /// <param name="TYPE"></param>
        /// <param name="sensorAreaModels"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult SensorAreaManage(string TYPE, List<SensorAreaModel> sensorAreaModels)
        {
            ResultModel resultModel = userCheck();

            if (!TYPE.ToUpper().Equals("D") && sensorAreaModels != null)
            {
                if (string.IsNullOrEmpty(sensorAreaModels[0].AREA_CD))
                {
                    resultModel.ERR_CODE = "8001";
                    resultModel.ERROR_MSG = "영역이름이 잘못되었습니다.";
                    return Content(JsonConvert.SerializeObject(resultModel, Formatting.Indented));
                }
            }

            if (resultModel.ERROR_MSG != "")
                return Content(JsonConvert.SerializeObject(resultModel, Formatting.Indented));


            // 2019-05-14 디스플레이 조회 데이터 변조 체크
            if (sensorAreaModels[0].RESTAURANT_CODE != UserBiz.getRestaurantCode(User))
            {
                resultModel.ERROR_MSG = "비정상 데이터 교환으로 인하여 로그아웃됩니다.";
                resultModel.ERR_CODE = "9998";
                return Content(JsonConvert.SerializeObject(resultModel, Formatting.Indented));
            }
            return Content(JsonConvert.SerializeObject(SensorBiz.sensorAreaManage(TYPE, UserBiz.getRestaurantCode(User), sensorAreaModels, UserBiz.getUserId(User)), Formatting.Indented));
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