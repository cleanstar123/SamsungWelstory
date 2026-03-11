using System.Collections.Generic;
using System.Web.Mvc;
using Newtonsoft.Json;
using CMS.API.Biz;
using CMS.API.Models;
using CMS.API.App_Code;

namespace CMS.API.Controllers
{
    [Authorize]
    public class CodeController : BaseController
    {
        /// <summary>
        /// 그룹코드, 코드 관리 페이지
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// 그룹 코드 조회
        /// </summary>
        /// <param name="codeGroup">그룹 코드</param>
        /// <param name="codeGroupNm">그룹 코드 이름</param>
        /// <returns>그룹 코드 리스트</returns>
        [HttpPost]
        public ActionResult GroupCodes(string codeGroup, string codeGroupNm)
        {
            ResultModel resultModel = userCheck();

            if (resultModel.ERROR_MSG != "")
                return Content(JsonConvert.SerializeObject(resultModel, Formatting.Indented));

            return Content(JsonConvert.SerializeObject(CodeBiz.getGroupCodes(codeGroup, codeGroupNm), Formatting.Indented));
        }

        /// <summary>
        /// 코드 조회
        /// </summary>
        /// <param name="codeGroup">그룹 코드</param>
        /// <returns>코드 리스트</returns>
        [HttpPost]
        public ActionResult Codes(string codeGroup)
        {
            ResultModel resultModel = userCheck();

            if (resultModel.ERROR_MSG != "")
                return Content(JsonConvert.SerializeObject(resultModel, Formatting.Indented));

            return Content(JsonConvert.SerializeObject(CodeBiz.getCodes(codeGroup), Formatting.Indented));
        }

        /// <summary>
        /// 그룹 코드 저장/수정/삭제
        /// </summary>
        /// <param name="TYPE">I : 저장, U : 수정, D : 삭제</param>
        /// <param name="codeGroupModel">그룹코드 모델</param>
        /// <returns>실행 결과</returns>
        [HttpPost]
        public ActionResult CodeGroupManage(string TYPE, List<CodeGroupModel> codeGroupModel)
        {
            ResultModel resultModel = userCheck();

            // null 체크 및 빈 리스트 초기화
            if (codeGroupModel == null)
            {
                codeGroupModel = new List<CodeGroupModel>();
            }

            //2019-10-22 XSS 공격에 대응하기 위해 넘어온 object에 대한 model 정보를 replace함
            CommonProperties.XSSCheck_CodeGroupModel(codeGroupModel);
            if (!TYPE.ToUpper().Equals("D") && codeGroupModel != null)
            {
                foreach (CodeGroupModel model in codeGroupModel)
                {
                    if (string.IsNullOrEmpty(model.CODE_GROUP_NM))
                    {
                        resultModel.ERR_CODE = "8001";
                        resultModel.ERROR_MSG = "코드 그룹 이름이 잘못되었습니다.";
                        return Content(JsonConvert.SerializeObject(resultModel, Formatting.Indented));
                    }
                }
            }

            if (resultModel.ERROR_MSG != "")
                return Content(JsonConvert.SerializeObject(resultModel, Formatting.Indented));

            if (TYPE != "D")
            {
                // 2019-05-14 특수기호 입력 체크
                //if (CommonProperties.speSymbol(codeGroupModel[0].CODE_GROUP_NM) || CommonProperties.speSymbol(codeGroupModel[0].CODE_GROUP_DESC))
                //{
                //    resultModel.ERROR_MSG = "특수기호는 입력할 수 없습니다.";
                //    resultModel.ERR_CODE = "9999";
                //    return Content(JsonConvert.SerializeObject(resultModel, Formatting.Indented));
                //}
            }            

            return Content(JsonConvert.SerializeObject(CodeBiz.codeGroupManage(TYPE, UserBiz.getUserId(User), codeGroupModel), Formatting.Indented));
        }

        /// <summary>
        /// 코드 저장/수정/삭제
        /// </summary>
        /// <param name="TYPE">I : 저장, U : 수정, D : 삭제</param>
        /// <param name="codeModel">그룹코드 모델</param>
        /// <returns>실행 결과</returns>
        [HttpPost]
        public ActionResult CodeManage(string TYPE, List<CodeModel> codeModel)
        {
            ResultModel resultModel = userCheck();

            // null 체크 및 빈 리스트 초기화
            if (codeModel == null)
            {
                codeModel = new List<CodeModel>();
            }

            //2019-10-22 XSS 공격에 대응하기 위해 넘어온 object에 대한 model 정보를 replace함
            CommonProperties.XSSCheck_CodeModel(codeModel);
            if (!TYPE.ToUpper().Equals("D") && codeModel != null)
            {
                foreach (CodeModel model in codeModel)
                {
                    if (string.IsNullOrEmpty(model.CODE_NAME))
                    {
                        resultModel.ERR_CODE = "8001";
                        resultModel.ERROR_MSG = "코드 이름이 잘못되었습니다.";
                        return Content(JsonConvert.SerializeObject(resultModel, Formatting.Indented));
                    }
                }
            }

            if (resultModel.ERROR_MSG != "")
                return Content(JsonConvert.SerializeObject(resultModel, Formatting.Indented));

            if (TYPE != "D")
            {
                // 2019-05-14 특수기호 입력 체크
                //if (CommonProperties.speSymbol(codeModel[0].CODE) || CommonProperties.speSymbol(codeModel[0].CODE_NAME) || CommonProperties.speSymbol(codeModel[0].CODE_DESC))
                //{
                //    resultModel.ERROR_MSG = "특수기호는 입력할 수 없습니다.";
                //    resultModel.ERR_CODE = "9999";
                //    return Content(JsonConvert.SerializeObject(resultModel, Formatting.Indented));
                //}
            }                

            return Content(JsonConvert.SerializeObject(CodeBiz.codeManage(TYPE, UserBiz.getUserId(User), codeModel), Formatting.Indented));
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