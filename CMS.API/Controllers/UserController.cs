using System.Web.Mvc;
using CMS.API.App_Code;
using CMS.API.Biz;
using CMS.API.Models;
using Newtonsoft.Json;

namespace CMS.API.Controllers
{
    [Authorize]
    public class UserController : BaseController
    {
        /// <summary>
        /// 사용자 관리 화면
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// 사용자 목록
        /// </summary>
        /// <param name="userModel"></param>
        /// <returns></returns>
        [HttpPost]
        public string list(UserModel userModel)
        {
            ResultModel resultModel = userCheck();

            if (resultModel.ERROR_MSG != "")
            {
                return JsonConvert.SerializeObject(resultModel, Formatting.Indented);
            }
            else if ((UserBiz.getUserRole(User) == null ? "10002" : UserBiz.getUserRole(User)).Equals("10002"))
            {
                resultModel.ERROR_MSG = "접근 권한이 없습니다.";
                resultModel.ERR_CODE = "9998";
                return JsonConvert.SerializeObject(resultModel, Formatting.Indented);
            }
            else
                return JsonConvert.SerializeObject(UserBiz.userList(userModel), Formatting.Indented);
        }

        /// <summary>
        /// 레스토랑 목록
        /// </summary>
        /// <param name="restaurant"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetRestaurantList(string restaurant)
        {
            ResultModel resultModel = userCheck();

            if (resultModel.ERROR_MSG != "")
            {
                return Content(JsonConvert.SerializeObject(resultModel, Formatting.Indented));
            }
            else if ((UserBiz.getUserRole(User) == null ? "10002" : UserBiz.getUserRole(User)).Equals("10002"))
            {
                resultModel.ERROR_MSG = "접근 권한이 없습니다.";
                resultModel.ERR_CODE = "9998";
                return Content(JsonConvert.SerializeObject(resultModel, Formatting.Indented));
            }
            else
                return Content(JsonConvert.SerializeObject(UserBiz.getRestaurantList(restaurant), Formatting.Indented));
        }

        /// <summary>
        /// 회원 생성/수정/삭제
        /// </summary>
        /// <param name="userModel"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult ManageUser(string TYPE, UserModel userModel)
        {
            ResultModel resultModel = userCheck();

            //2019-10-22 XSS 공격에 대응하기 위해 넘어온 object에 대한 model 정보를 replace함
            CommonProperties.XSSCheck_UserModel(userModel);
            if (!TYPE.ToUpper().Equals("D") && (string.IsNullOrEmpty(userModel.USER_NM) || string.IsNullOrEmpty(userModel.USER_ID)))
            {
                resultModel.ERROR_MSG = "입력된 사용자 정보가 잘못되었습니다.";
                resultModel.ERR_CODE = "8001";
            }

            if (resultModel.ERROR_MSG != "")
                return Content(JsonConvert.SerializeObject(resultModel, Formatting.Indented));

            if (TYPE.ToUpper().Equals("I") || TYPE.ToUpper().Equals("U"))
            {
                // 2019-05-14 데이터 무결성 체크
                if (userModel.RESTAURANT_CODE == null)
                {
                    resultModel.ERROR_MSG = "레스토랑 코드를 확인하세요.";
                    resultModel.ERR_CODE = "9999";
                    return Content(JsonConvert.SerializeObject(resultModel, Formatting.Indented));
                }
                else if (userModel.USER_ID == null)
                {
                    resultModel.ERROR_MSG = "아이디를 확인하세요.";
                    resultModel.ERR_CODE = "9999";
                    return Content(JsonConvert.SerializeObject(resultModel, Formatting.Indented));
                }
                else if (userModel.USER_NM == null)
                {
                    resultModel.ERROR_MSG = "이름을 확인하세요.";
                    resultModel.ERR_CODE = "9999";
                    return Content(JsonConvert.SerializeObject(resultModel, Formatting.Indented));
                }
                // 2019-05-14 특수기호 입력 체크
                //else if (CommonProperties.speSymbol(userModel.USER_NM) || CommonProperties.speSymbol(userModel.USER_ID))
                //{
                //    resultModel.ERROR_MSG = "특수기호는 입력할 수 없습니다.";
                //    resultModel.ERR_CODE = "9999";
                //    return Content(JsonConvert.SerializeObject(resultModel, Formatting.Indented));
                //}
                else if (userModel.GROUP_CD.Length != 5 || userModel.USE_YN.Length != 1 || (string.IsNullOrEmpty(userModel.RESTAURANT_CODE) ? "" : userModel.RESTAURANT_CODE).Length > 15)
                {
                    resultModel.ERROR_MSG = "데이터 입력 오류가 발생하였습니다.";
                    resultModel.ERR_CODE = "9999";
                    return Content(JsonConvert.SerializeObject(resultModel, Formatting.Indented));
                }
            }

            // 사용자 생성일 경우 비밀번호 1111 초기화
            if (TYPE.ToUpper().Equals("I"))
            {
                userModel.USER_PW = Util.Encrypt("1111", CommonProperties.key);
            }
            else if (TYPE.ToUpper().Equals("U"))
            {
                if (!string.IsNullOrEmpty(userModel.USER_PW))
                {
                    userModel.USER_PW = Util.Encrypt(userModel.USER_PW, CommonProperties.key);
                }
            }

            return Content(JsonConvert.SerializeObject(UserBiz.manageUser(TYPE, UserBiz.getUserId(User), userModel)[0], Formatting.Indented));
        }

        /// <summary>
        /// 사용자 비밀번호 초기화 (1111)
        /// </summary>
        /// <param name="userModel"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult InitPass(UserModel userModel)
        {
            ResultModel resultModel = userCheck();

            if (resultModel.ERROR_MSG != "")
            {
                return Content(JsonConvert.SerializeObject(resultModel, Formatting.Indented));
            }
            else if ((UserBiz.getUserRole(User) == null ? "10002" : UserBiz.getUserRole(User)).Equals("10002"))
            {
                resultModel.ERROR_MSG = "접근 권한이 없습니다.";
                resultModel.ERR_CODE = "9998";
                return Content(JsonConvert.SerializeObject(resultModel, Formatting.Indented));
            }
            else
            {
                userModel.USER_PW = Util.Encrypt("1111", CommonProperties.key);
                return Content(JsonConvert.SerializeObject(UserBiz.manageUser("U", UserBiz.getUserId(User), userModel)[0], Formatting.Indented));
            }
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