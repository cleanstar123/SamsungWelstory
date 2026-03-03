using System.Web.Mvc;
using CMS.API.Biz;

namespace CMS.API.Controllers
{
    public class BaseController : Controller
    {
        protected override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            base.OnActionExecuted(filterContext);

            ViewBag.USER_ID        = UserBiz.getUserId(User);
            ViewBag.USER_NM        = UserBiz.getUserNm(User);
            ViewBag.RESTAURANT_NM  = UserBiz.getRestaurantName(User);
            ViewBag.RESTAURANT_CD  = UserBiz.getRestaurantCode(User);
            ViewBag.USER_ROLE      = UserBiz.getUserRole(User);
            ViewBag.CHANGE_PASS_YN = UserBiz.getChangePassYN(User);

            if (string.IsNullOrEmpty(ViewBag.USER_ID) || string.IsNullOrEmpty(ViewBag.USER_NM) || string.IsNullOrEmpty(ViewBag.RESTAURANT_NM) || string.IsNullOrEmpty(ViewBag.USER_ROLE) || string.IsNullOrEmpty(ViewBag.RESTAURANT_CD) || string.IsNullOrEmpty(ViewBag.CHANGE_PASS_YN))
            {
                filterContext.Result = new RedirectResult(Url.Action("Logout", "Account"));
                return;
            }
        }
    }
}