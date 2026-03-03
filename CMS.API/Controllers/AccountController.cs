using CMS.API.Models;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;
using System.Data;
using CMS.API.Biz;
using System.Linq;
using CMS.API.App_Code;
using System;

namespace CMS.API.Controllers
{
    public class AccountController : Controller
    {
        public IAuthenticationManager Authentication => HttpContext.GetOwinContext().Authentication;

        /// <summary>
        /// 로그인 화면
        /// </summary>
        /// <returns></returns>
        public ActionResult Login()
        {
            LoginModel loginModel = new LoginModel();

            if (Request.Cookies["USER_ID"] != null && !string.IsNullOrEmpty(Request.Cookies["USER_ID"].Value))
            {
                loginModel.USER_ID    = Request.Cookies["USER_ID"].Value;
                loginModel.RememberMe = true;
            }

            return View(loginModel);
        }

        /// <summary>
        /// 로그인
        /// </summary>
        /// <param name="model"></param>
        /// <param name="ReturnUrl"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Login(LoginModel model, string ReturnUrl)
        {
            // TODO: Validation
            UserModel userModel = new UserModel();
            userModel.USER_ID = model.USER_ID;
            userModel.USER_PW = model.USER_PW;

            if (string.IsNullOrEmpty(model.USER_ID) || string.IsNullOrEmpty(model.USER_PW))
            {
                ViewBag.ERR_MSG = "아이디 또는 비밀번호를 입력하세요.";
                return View(model);
            }

            ResultModel result = UserBiz.user(userModel.USER_ID, Util.Encrypt(userModel.USER_PW, CommonProperties.key));

            if (result.ERR_CODE == "0")
            {
                if(model.RememberMe)
                {
                    HttpCookie cookie = new HttpCookie("USER_ID", model.USER_ID);
                    cookie.Expires = DateTime.Now.AddDays(7);                         // 7일간 쿠키저장
                    Response.Cookies.Add(cookie);
                }
                else
                {
                    HttpCookie cookie = new HttpCookie("USER_ID", model.USER_ID);
                    cookie.Expires = DateTime.Now.AddDays(-1);                        // 쿠키 삭제
                    Response.Cookies.Add(cookie);
                }

                // Sign User In
                SignIn(result.DATA, (model.USER_PW.Equals("1111") ? "Y" : "N"));

                //팝업을 보여줄 것이라면 true 변경
                bool isPopup = true;
                if (isPopup)
                {
                    if (Request.Cookies["POPUP_OPEN"] != null && !string.IsNullOrEmpty(Request.Cookies["POPUP_OPEN"].Value))
                        return RedirectToAction("Index", "schedule");
                    else
                        return RedirectToAction("Index", "schedule", new { POPUP = true });
                }
                else
                {
                    return RedirectToAction("Index", "schedule");
                }
            }
            else
                ViewBag.ERR_MSG = "로그인 정보가 없습니다.";


            return View(model);
        }

        /// <summary>
        /// 로그인 정보 생성
        /// </summary>
        /// <param name="userDt">사용자 정보</param>
        /// <param name="changePassYN">초기 비밀번호(1111)일 경우 Y 아니면 N</param>
        private void SignIn(DataTable userDt, string changePassYN)
        {
            var claims = new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier,    userDt.Rows[0]["USER_ID"].ToString()),
                new Claim(ClaimTypes.Name,              userDt.Rows[0]["USER_NM"].ToString()),
                new Claim("urn:Custom:RESTAURANT_CODE", userDt.Rows[0]["RESTAURANT_CODE"].ToString()),
                new Claim("urn:Custom:RESTAURANT_NM",   userDt.Rows[0]["RESTAURANT_NM"].ToString()),
                new Claim("urn:Custom:CHANGE_PASS_YN",  changePassYN),
                new Claim(ClaimTypes.Role,              userDt.Rows[0]["GROUP_CD"].ToString())
            };

            var identity = new ClaimsIdentity(claims, DefaultAuthenticationTypes.ApplicationCookie);
            Session["UserID"] = identity.GetUserId();

            Authentication.SignIn(identity);
        }

        /// <summary>
        /// 로그아웃
        /// </summary>
        /// <returns></returns>
        public ActionResult Logout()
        {
            Authentication.SignOut();
            Session.Abandon();
            return RedirectToAction("Login", "Account");
        }
    }
}