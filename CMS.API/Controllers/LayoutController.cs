using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using Microsoft.AspNet.Identity;
using System.IO;

using CMS.API.Biz;
using CMS.API.Models;
using CMS.API.App_Code;
using System.Diagnostics;

namespace CMS.API.Controllers
{
    [Authorize]
    public class LayoutController : BaseController
    {
        /// <summary>
        /// 레이아웃 관리 화면
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// 레이아웃 조회(페이징)
        /// </summary>
        /// <param name="layoutModel"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult getLayoutPageList(LayoutModel layoutModel)
        {
            ResultModel resultModel = userCheck();

            if (resultModel.ERROR_MSG != "")
                return Content(JsonConvert.SerializeObject(resultModel, Formatting.Indented));

            return Content(JsonConvert.SerializeObject(LayoutBiz.getLayoutPageList(layoutModel), Formatting.Indented));
        }

        /// <summary>
        /// 레이아웃 저장(업로드)
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult LayoutUpload()
        {
            ResultModel resultModel = userCheck();

            if (resultModel.ERROR_MSG != "")
                return Content(JsonConvert.SerializeObject(resultModel, Formatting.Indented));

            // 정합성 체크 : HTML 파일 : 1개, IMAGE 파일 : 0개 이상
            if (Request.Files.Count > 0)
            {
                HttpFileCollectionBase files = Request.Files;

                LayoutModel layoutModel                    = JsonConvert.DeserializeObject<LayoutModel>(Request.Form["layoutModel"]);                     // 레이아웃 기본 정보
                List<LayoutDetailModel> layoutDetailModels = JsonConvert.DeserializeObject<List<LayoutDetailModel>>(Request.Form["layoutDetailModels"]);  // 레이아웃 상세 정보 (컨텐츠 영역 정보)

                // 디버깅: 받은 데이터 로깅
                System.Diagnostics.Debug.WriteLine("=== LayoutUpload 받은 데이터 ===");
                System.Diagnostics.Debug.WriteLine("layoutModel: " + JsonConvert.SerializeObject(layoutModel));
                System.Diagnostics.Debug.WriteLine("layoutDetailModels count: " + (layoutDetailModels?.Count ?? 0));
                if (layoutDetailModels != null)
                {
                    foreach (var detail in layoutDetailModels)
                    {
                        System.Diagnostics.Debug.WriteLine($"  LAYOUT_SEQ: {detail.LAYOUT_SEQ}, CONTENT_TYPE: {detail.CONTENT_TYPE}");
                    }
                }

                string layoutFilename = string.Empty;                                     // 레이아웃(HTML) 파일명
                List<LayoutImageModel> layoutImageModels = new List<LayoutImageModel>();  // 레이아웃(HTML) 파일과 함께 사용할 고정 이미지

                try
                {
                    //2019-10-22 XSS 공격에 대응하기 위해 넘어온 object에 대한 model 정보를 replace함
                    CommonProperties.XSSCheck_LayoutModel(layoutModel);
                    CommonProperties.XSSCheck_LayoutDetailModel(layoutDetailModels);
                    CommonProperties.XSSCheck_LayoutImageModel(layoutImageModels);

                    // 디버깅: XSS 체크 후 데이터 확인
                    System.Diagnostics.Debug.WriteLine("=== XSS 체크 후 layoutDetailModels ===");
                    System.Diagnostics.Debug.WriteLine("Count: " + (layoutDetailModels?.Count ?? 0));
                    if (layoutDetailModels != null)
                    {
                        foreach (var detail in layoutDetailModels)
                        {
                            System.Diagnostics.Debug.WriteLine($"  LAYOUT_SEQ: {detail.LAYOUT_SEQ}, CONTENT_TYPE: {detail.CONTENT_TYPE}");
                        }
                    }

                    // 2019-05-14 특수기호 입력 체크
                    //if (CommonProperties.speSymbol(layoutModel.LAYOUT_NM) || CommonProperties.speSymbol(layoutModel.LAYOUT_DESC))
                    //    throw new Exception("특수기호는 입력할 수 없습니다.");

                    #region 레이아웃(HTML) 파일과 고정 이미지 파일 분리
                    for (int i = 0; i < files.Count; i++)
                    {
                        HttpPostedFileBase file = files[i];

                        string fileName      = Path.GetFileName(file.FileName);  // 확장자 포함 파일명 (IE의 경우 전체 경로일 수 있으므로 파일명 추출 메서드 사용)
                        string fileExtension = Path.GetExtension(fileName);      // .이 포함된 확장자 (ex) .html)


                        switch (fileExtension.ToUpper())
                        {
                            case ".HTML":
                                if (!string.IsNullOrEmpty(layoutFilename))
                                    throw new Exception("레이아웃(.html)파일은 1개만 업로드할 수 있습니다.");
                                else
                                    layoutFilename = fileName;
                                break;
                            case ".JPG":
                            case ".JPEG":
                            case ".GIF":
                            case ".PNG":
                                LayoutImageModel layoutImageModel = new LayoutImageModel();
                                layoutImageModel.FILE_NM = fileName;

                                layoutImageModels.Add(layoutImageModel);
                                break;
                            default:
                                throw new Exception("업로드할 수 없는 파일이 포함되어 있습니다.");
                        }
                    }
                    #endregion

                    layoutModel.FILE_NM = layoutFilename;  // 레이아웃(HTML) 파일명 할당

                    // 레이아웃 저장 후 아이디 리턴
                    resultModel = LayoutBiz.layoutManage("I", User.Identity.GetUserId<string>(), layoutModel, layoutDetailModels, layoutImageModels);

                    // 메세지가 정상이 아니면 오류 발생
                    if (!resultModel.ERR_CODE.Equals("0"))
                        throw new Exception(resultModel.ERROR_MSG);

                    // 정상적으로 저장되었다면 레이아웃 아이디 할당
                    layoutModel.LAYOUT_ID = resultModel.ID;

                    #region 레이아웃(HTML) 파일과 고정 이미지 저장

                    string saveHtmlPath  = Path.Combine(Server.MapPath("~/"), CommonProperties.LAYOUT_UPLOAD_PATH, resultModel.ID);            // HTML 파일 저장 경로
                    string saveImagePath = Path.Combine(Server.MapPath("~/"), CommonProperties.LAYOUT_UPLOAD_PATH, resultModel.ID, "images");  // 이미지 파일 저장 경로

                    // 파일경로가 존재하지 않을 경우
                    if (!Directory.Exists(saveHtmlPath))
                        Directory.CreateDirectory(saveHtmlPath);

                    if (!Directory.Exists(saveImagePath))
                        Directory.CreateDirectory(saveImagePath);

                    // 파일 저장
                    for (int i = 0; i < files.Count; i++)
                    {
                        HttpPostedFileBase file = files[i];
                        string originFileName = Path.GetFileName(file.FileName);
                        string fileExtension  = Path.GetExtension(originFileName).ToUpper();   // .이 포함된 확장자 (ex) .html)

                        if (fileExtension == ".HTML")
                        {
                            string fileName = Path.Combine(saveHtmlPath, originFileName);
                            file.SaveAs(fileName);
                        }
                        else
                        {
                            string fileName = Path.Combine(saveImagePath, originFileName);
                            file.SaveAs(fileName);
                        }
                    }

                    #endregion

                    #region 레이아웃 썸네일 이미지 생성 (조회시 사용)

                    string originImageFileName    = Path.ChangeExtension(layoutFilename, ".jpg");
                    string thumbnailImageFileName = "thumbnail_" + Path.ChangeExtension(layoutFilename, ".jpg");

                    string url          = string.Format("{0}/upload/layout/{1}/{2}", CommonProperties.HTTPS_DOMAIN_URL, resultModel.ID, layoutFilename);          // 레이아웃(HTML) URL
                    string urlThumbnail = string.Format("{0}/upload/layout/{1}/{2}", CommonProperties.HTTPS_DOMAIN_URL, resultModel.ID, thumbnailImageFileName);  // 레이아웃 썸네일(JPG) URL

                    ProcessStartInfo startInfo = new ProcessStartInfo(Server.MapPath("~/bin/ScreenShotUrl.exe"));
                    startInfo.WindowStyle = ProcessWindowStyle.Hidden;

                    startInfo.Arguments = string.Format("{0} {1} {2} {3} {4} {5}", url, string.IsNullOrEmpty(layoutModel.SCREEN_W) ? 1920 : int.Parse(layoutModel.SCREEN_W), string.IsNullOrEmpty(layoutModel.SCREEN_H) ? 1080 : int.Parse(layoutModel.SCREEN_H), CommonProperties.LAYOUT_THUMBNAIL_WIDTH, CommonProperties.LAYOUT_THUMBNAIL_HEIGHT, Path.Combine(saveHtmlPath, thumbnailImageFileName));
                    Process.Start(startInfo);


                    //CMSLib cmslib = new CMSLib();
                    //using (Bitmap bitmap = cmslib.GenerateWebSiteThumbnailImage(url, string.IsNullOrEmpty(layoutModel.SCREEN_W) ? 1920 : int.Parse(layoutModel.SCREEN_W), string.IsNullOrEmpty(layoutModel.SCREEN_H) ? 1080 : int.Parse(layoutModel.SCREEN_H), CommonProperties.LAYOUT_THUMBNAIL_WIDTH, CommonProperties.LAYOUT_THUMBNAIL_HEIGHT))
                    //{
                    //    bitmap.Save(Path.Combine(saveHtmlPath, thumbnailImageFileName), ImageFormat.Jpeg);
                    //}

                    //// 레이아웃 해상도에 맞게 원본 이미지 생성
                    //using (Bitmap bitmap = cMSLib.Capture(url, int.Parse(layoutModel.SCREEN_W), int.Parse(layoutModel.SCREEN_H)))
                    //{
                    //    bitmap.Save(Path.Combine(saveHtmlPath, Path.ChangeExtension(layoutFilename, ".jpg")));
                    //}

                    //// 생성한 원본 이미지 썸네일 생성 (조회하는 페이지의 썸네일 넓이 500px)
                    //Util.CreateThumbnail(Path.Combine(saveHtmlPath, Path.ChangeExtension(layoutFilename, ".jpg")), Path.Combine(saveHtmlPath, thumbnailImageFileName), 500);

                    #endregion

                    layoutModel.THUMBNAIL_NM = urlThumbnail;
                    resultModel = LayoutBiz.layoutManage("U", User.Identity.GetUserId<string>(), layoutModel);

                    if (!resultModel.ERR_CODE.Equals("0"))
                        throw new Exception(resultModel.ERROR_MSG);
                }
                catch (Exception ex)
                {
                    // 저장된 데이터 삭제
                    if(!string.IsNullOrEmpty(layoutModel.LAYOUT_ID))
                        DeleteLayout(layoutModel.LAYOUT_ID);

                    resultModel.ERR_CODE  = "9999";
                    resultModel.ERROR_MSG = ex.Message;
                }
            }
            else
            {
                resultModel.ERR_CODE  = "9999";
                resultModel.ERROR_MSG = "업로드할 파일이 없습니다.";    
            }

            return Content(JsonConvert.SerializeObject(resultModel, Formatting.Indented));
        }

        /// <summary>
        /// 레이아웃 삭제
        /// </summary>
        /// <param name="TYPE"></param>
        /// <param name="layoutModel"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult LayoutManage(string TYPE, LayoutModel layoutModel)
        {
            ResultModel resultModel = userCheck();

            if (resultModel.ERROR_MSG != "")
                return Content(JsonConvert.SerializeObject(resultModel, Formatting.Indented));

            return Content(JsonConvert.SerializeObject(DeleteLayout(layoutModel.LAYOUT_ID), Formatting.Indented));
        }

        /// <summary>
        /// 레이아웃 삭제
        /// </summary>
        /// <param name="layoutId">레이아웃 아이디</param>
        /// <returns>삭제 결과</returns>
        private ResultModel DeleteLayout(string layoutId)
        {
            if (string.IsNullOrEmpty(layoutId))
                throw new Exception("레이아웃 아이디가 없습니다.");

            LayoutModel layoutModel = new LayoutModel();

            layoutModel.RESTAURANT_CODE = UserBiz.getRestaurantCode(User);
            layoutModel.LAYOUT_ID       = layoutId;

            ResultModel resultModel = LayoutBiz.layoutManage("D", UserBiz.getUserId(User), layoutModel);

            if (resultModel.ERR_CODE.Equals("0"))
                Directory.Delete(Path.Combine(Server.MapPath("~/"), CommonProperties.LAYOUT_UPLOAD_PATH, layoutModel.LAYOUT_ID), true);

            return resultModel;
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