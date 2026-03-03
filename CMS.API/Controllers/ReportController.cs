using CMS.API.Biz;
using CMS.API.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CMS.API.Controllers
{
    public class ReportController : BaseController
    {
        /// <summary>
        /// 센서 리포트
        /// </summary>
        /// <param name="TYPE"></param>
        /// <param name="sensorAreaModels"></param>
        /// <returns></returns>
        /// 
        public ActionResult reportcongestion()
        {
            ReportCodeModel model = new ReportCodeModel();

            DataTable dtUseYN = CodeBiz.getCodes("10099").Tables[0]; // USE_YN

            foreach (DataRow row in dtUseYN.Rows)
                ((List<SelectListItem>)model.TIME_LIST).Add(new SelectListItem { Text = row["CODE_NAME"].ToString(), Value = row["CODE"].ToString() });

            return View(model);
        }
        /// <summary>
        /// 센서 리포트
        /// </summary>
        /// <param name="TYPE"></param>
        /// <param name="congestionTermModel"></param>
        /// <returns></returns>
        /// 
        public ActionResult CongestionSection(CongestionTermModel congestionTermModel)
        {
            return Content(JsonConvert.SerializeObject(ReportBiz.CongestionSection(congestionTermModel), Formatting.Indented));
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