using System;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Data;
using Newtonsoft.Json;
using System.IO;

using CMS.API.Biz;
using CMS.API.Models;
using CMS.API.App_Code;
using System.Diagnostics;

namespace CMS.API.Controllers
{
    [Authorize]
    public class TemplateController : BaseController
    {
        /// <summary>
        /// 템플릿 관리 화면
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// 템플릿 생성/수정 화면
        /// </summary>
        /// <param name="type">I(생성), U(수정)</param>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult ManageTemplate(string type, string id)
        {
            ResultModel resultModel = userCheck();

            // 레이아웃, 템플릿 정보가 없으면 템플릿 화면으로 이동
            if (string.IsNullOrEmpty(type) || string.IsNullOrEmpty(id))
            {
                return Redirect(Url.Action("index", "template"));
            }

            ViewBag.type = type;

            TemplateModel templateModel = new TemplateModel();

            templateModel.RESTAURANT_CODE = UserBiz.getRestaurantCode(User);
            templateModel.TEMPLATE_ID = id;
            templateModel.LAYOUT_ID = id;

            // 템플릿 수정일 경우 템플릿 정보 추가로 가져옴
            if (type.ToUpper().Equals("U"))
            {
                templateModel = TemplateBiz.getTemplateList(templateModel);
                templateModel.templateMapModels = TemplateBiz.getTemplateMapList(templateModel);
            }

            LayoutModel layoutModel = new LayoutModel();
            layoutModel.RESTAURANT_CODE = templateModel.RESTAURANT_CODE;
            layoutModel.LAYOUT_ID = templateModel.LAYOUT_ID;

            DataSet ds = LayoutBiz.layoutDetail(layoutModel);

            layoutModel.LAYOUT_TYPE = ds.Tables[0].Rows[0]["LAYOUT_TYPE"].ToString();
            layoutModel.LAYOUT_NM = ds.Tables[0].Rows[0]["LAYOUT_NM"].ToString();
            layoutModel.LAYOUT_DESC = ds.Tables[0].Rows[0]["LAYOUT_DESC"].ToString();
            layoutModel.FILE_NM = ds.Tables[0].Rows[0]["FILE_NM"].ToString();
            layoutModel.CONTENT_CNT = ds.Tables[0].Rows[0]["CONTENT_CNT"].ToString();
            layoutModel.SCREEN_W = ds.Tables[0].Rows[0]["SCREEN_W"].ToString();
            layoutModel.SCREEN_H = ds.Tables[0].Rows[0]["SCREEN_H"].ToString();
            layoutModel.SCREEN_W_H = ds.Tables[0].Rows[0]["SCREEN_W_H"].ToString();
            layoutModel.LAYOUT_HV_TYPE = ds.Tables[0].Rows[0]["LAYOUT_HV_TYPE"].ToString();
            layoutModel.LAYOUT_H_BY_V = ds.Tables[0].Rows[0]["LAYOUT_H_BY_V"].ToString();
            layoutModel.EVAL_USE_YN = ds.Tables[0].Rows[0]["EVAL_USE_YN"].ToString();

            templateModel.layoutModel = layoutModel;

            foreach (DataRow row in ds.Tables[1].Rows)
            {
                LayoutDetailModel layoutDetailModel = new LayoutDetailModel();

                layoutDetailModel.CONTENT_TYPE = row["CONTENT_TYPE"].ToString();
                layoutDetailModel.LAYOUT_SEQ = row["LAYOUT_SEQ"].ToString();
                layoutDetailModel.CONTENT_TYPE_NM = row["CONTENT_TYPE_NM"].ToString();

                templateModel.layoutDetailModels.Add(layoutDetailModel);
            }

            FileInfo file = null;

            if (type.ToUpper().Equals("I"))
                file = new FileInfo(Server.MapPath(string.Format("/upload/layout/{0}/{1}", layoutModel.LAYOUT_ID, layoutModel.FILE_NM)));
            else if (type.ToUpper().Equals("U"))
            {
                // 수정시 템플릿 정보가 없을시 오류
                if (string.IsNullOrEmpty(templateModel.FILE_PATH) || string.IsNullOrEmpty(templateModel.FILE_NM))
                {
                    return Redirect(Url.Action("index", "template"));
                }

                file = new FileInfo((Path.Combine(templateModel.FILE_PATH, templateModel.FILE_NM)));
            }

            if (file.Exists)
                return View(templateModel);

            return Redirect(Url.Action("index", "template"));
        }

        /// <summary>
        /// 템플릿 조회(페이징)
        /// </summary>
        /// <param name="templateModel"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetTemplatePageList(TemplateModel templateModel)
        {
            ResultModel resultModel = userCheck();

            if (resultModel.ERROR_MSG != "")
                return Content(JsonConvert.SerializeObject(resultModel, Formatting.Indented));
            else if (templateModel.RESTAURANT_CODE != UserBiz.getRestaurantCode(User))
            {
                resultModel.ERROR_MSG = "비정상 데이터 교환으로 인하여 로그아웃됩니다.";
                resultModel.ERR_CODE = "9998";
                return Content(JsonConvert.SerializeObject(resultModel, Formatting.Indented));
            }
            else
                return Content(JsonConvert.SerializeObject(TemplateBiz.getTemplatePageList(templateModel), Formatting.Indented));
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult uploadFile()
        {
            ResultModel resultModel = userCheck();

            if (resultModel.ERROR_MSG != "")
                return Content(JsonConvert.SerializeObject(resultModel, Formatting.Indented));

            string TYPE = Request.Form["TYPE"].ToString();

            TemplateModel templateModel = JsonConvert.DeserializeObject<TemplateModel>(Request.Form["templateModel"]);
            List<TemplateMapModel> templateMapModels = JsonConvert.DeserializeObject<List<TemplateMapModel>>(Request.Form["templateMapModels"]);

            //2019-10-22 XSS 공격에 대응하기 위해 넘어온 object에 대한 model 정보를 replace함
            CommonProperties.XSSCheck_TemplateModel(templateModel);
            CommonProperties.XSSCheck_TemplateMapModel(templateMapModels);
            if (templateModel != null && string.IsNullOrEmpty(templateModel.TEMPLATE_NM))
            {
                resultModel.ERROR_MSG = "템플릿 이름이 잘못되었습니다.";
                resultModel.ERR_CODE = "8001";
                return Content(JsonConvert.SerializeObject(resultModel, Formatting.Indented));
            }

            // 2019-05-14 컨텐츠 조회 데이터 변조 체크
            if (templateModel.RESTAURANT_CODE != UserBiz.getRestaurantCode(User))
            {
                resultModel.ERROR_MSG = "비정상 데이터 교환으로 인하여 로그아웃됩니다.";
                resultModel.ERR_CODE = "9998";
                return Content(JsonConvert.SerializeObject(resultModel, Formatting.Indented));
            }

            // 저장할 템플릿 루트 경로
            string saveRootPath = string.Empty;
            string templateFileName = string.Empty;
            string templateCaptureFileName = string.Empty;
            string templateThumbnailFileName = string.Empty;
            string templateId = string.Empty;

            try
            {
                // 2019-05-14 특수기호 입력 체크
                //string str = @"[~!@\#$%^&*\()\=+|\\/?""<>']";
                //System.Text.RegularExpressions.Regex rex = new System.Text.RegularExpressions.Regex(str);

                //if (rex.IsMatch(templateModel.TEMPLATE_NM) || rex.IsMatch(templateModel.TEMPLATE_DESC))
                //    throw new Exception("특수기호는 입력할 수 없습니다.");

                #region 00. 템플릿 정보가 없을 경우 강제 오류

                if (string.IsNullOrEmpty(templateModel.TEMPLATE_HTML))
                    throw new Exception("템플릿 정보(HTML)가 없습니다.");

                #endregion

                #region 01. 템플릿과 연결된 컨텐츠 검증

                bool isContents = true;                    // 템플릿과 연결된 컨텐츠 파일이 모두 존재하면 true, 하나라도 없으면 false
                string contentsFileName = string.Empty;

                if (templateMapModels != null)
                {
                    if (TYPE.ToUpper().Equals("I"))
                    {
                        for (int i = 0; i < templateMapModels.Count; i++)
                        {

                            if (!string.IsNullOrEmpty(templateMapModels[i].CONTENT_FILE_PATH) && !string.IsNullOrEmpty(templateMapModels[i].CONTENT_FILE_NM))
                            {
                                if (!System.IO.File.Exists(Path.Combine(templateMapModels[i].CONTENT_FILE_PATH, templateMapModels[i].CONTENT_FILE_NM)))
                                {
                                    isContents = false;
                                    contentsFileName = templateMapModels[i].CONNECTION_NM;
                                    break;
                                }
                            }
                        }
                    }
                }

                if (isContents == false)
                {
                    throw new Exception(contentsFileName + " 컨텐츠 파일을 찾을 수 없어 사용할 수 없습니다.\n 다른 컨텐츠를 선택해주세요.");
                }

                #endregion

                #region 02. 템플릿 아이디 설정

                if (TYPE.ToUpper().Equals("I"))
                {
                    // DB 저장 후 생성된 아이디를 리턴 받음
                    resultModel = TemplateBiz.ManageTemplateAll("I", UserBiz.getUserId(User), templateModel, templateMapModels);
                    templateId = resultModel.ID;
                }
                else if (TYPE.ToUpper().Equals("U"))
                {
                    templateId = templateModel.TEMPLATE_ID;
                }

                #endregion

                #region 03. 템플릿 파일 저장 (동일한 이름의 파일이 있을경우(수정일경우) 파일을 덮어씀)

                // 루트 경로 셋팅
                saveRootPath = Path.Combine(Server.MapPath("~/"), CommonProperties.TEMPLATE_SAVE_PATH(UserBiz.getRestaurantCode(User), templateId));
                templateFileName = string.Format("T{0}.html", templateId);
                templateCaptureFileName = string.Format("T{0}.jpg", templateId);
                templateThumbnailFileName = string.Format("THUMBNAIL_T{0}.jpg", templateId);

                if (!Directory.Exists(saveRootPath))
                    Directory.CreateDirectory(saveRootPath);

                // 템플릿 URL 변경
                templateModel.TEMPLATE_HTML = templateModel.TEMPLATE_HTML.Replace("{TEMPLATE_PATH}", string.Format("{0}/upload/template/{1}/{2}/images", CommonProperties.HTTP_DOMAIN_URL, UserBiz.getRestaurantCode(User), templateId));

                // 템플릿(HTML) 파일 저장
                System.IO.File.WriteAllText(Path.Combine(saveRootPath, templateFileName), templateModel.TEMPLATE_HTML, System.Text.Encoding.UTF8);

                #endregion

                #region 04. 템플릿을 생성할때 사용한 레이아웃의 고정 이미지를 복사해옴

                string sourceDirName = Path.Combine(Server.MapPath("~/"), CommonProperties.LAYOUT_UPLOAD_PATH, templateModel.LAYOUT_ID, "images");  // 복사할 디렉토리
                string targetDirName = Path.Combine(saveRootPath, "images");                                                                        // 저장할 디렉토리

                // 생성일 경우만 고정 이미지를 복사해옴 (수정일 경우는 이미 복사한 고정 이미지가 있음)
                if (TYPE.ToUpper().Equals("I"))
                {
                    DirectoryInfo dir = new DirectoryInfo(sourceDirName);

                    // 저장할 디렉토리가 없을 경우 생성
                    if (!Directory.Exists(targetDirName))
                        Directory.CreateDirectory(targetDirName);

                    // 레이아웃의 고정이미지가 있을 경우 실행
                    if (dir.Exists)
                    {
                        DirectoryInfo[] dirs = dir.GetDirectories();

                        FileInfo[] files = dir.GetFiles();

                        foreach (FileInfo file in files)
                        {
                            string uniqFileName = Util.GetUniqName(targetDirName, Path.GetFileNameWithoutExtension(file.Name), Path.GetExtension(file.Name));

                            string temppath = Path.Combine(targetDirName, file.Name);
                            file.CopyTo(temppath, true);
                        }
                    }
                }

                #endregion

                #region 05. 템플릿과 연결된 컨텐츠를 복사해옴

                // 5. 템플릿(HTML)과 연결된 컨텐츠 파일 복사
                if (templateMapModels != null)
                {
                    for (int i = 0; i < templateMapModels.Count; i++)
                    {
                        templateMapModels[i].TEMPLATE_ID = templateId;

                        // 템플릿 생성일 경우 
                        // 템플릿 수정일 경우
                        if (TYPE.ToUpper().Equals("I") || TYPE.ToUpper().Equals("U"))
                        {
                            // 연결한 컨텐츠의 파일 경로가 있을 경우 실행
                            if (!string.IsNullOrEmpty(templateMapModels[i].CONTENT_FILE_PATH) && !string.IsNullOrEmpty(templateMapModels[i].CONTENT_FILE_NM))
                            {
                                // 템플릿 수정
                                // 기존 컨텐츠 수정
                                // 기존 컨텐츠 파일명
                                // 기존 컨텐츠 파일 존재
                                // 기존 파일 삭제
                                if (TYPE.ToUpper().Equals("U") && !string.IsNullOrEmpty(templateMapModels[i].FILE_NM) && System.IO.File.Exists(Path.Combine(targetDirName, templateMapModels[i].FILE_NM)))
                                {
                                    System.IO.File.Delete(Path.Combine(targetDirName, templateMapModels[i].FILE_NM));
                                }

                                string uniqFileName = Util.GetUniqName(targetDirName, Path.GetFileNameWithoutExtension(templateMapModels[i].CONTENT_FILE_NM), Path.GetExtension(templateMapModels[i].CONTENT_FILE_NM));

                                System.IO.File.Copy(Path.Combine(templateMapModels[i].CONTENT_FILE_PATH, templateMapModels[i].CONTENT_FILE_NM), Path.Combine(targetDirName, uniqFileName), true);
                                templateMapModels[i].FILE_NM = Path.Combine(targetDirName, uniqFileName);
                            }
                        }
                    }
                }

                #endregion

                #region 06. 저장된 템플릿 스크린샷 및 썸네일 이미지 생성

                string url = string.Format("{0}/upload/template/{1}/{2}/{3}?type=S", CommonProperties.HTTPS_DOMAIN_URL, UserBiz.getRestaurantCode(User), templateId, templateFileName);

                ProcessStartInfo startInfo = new ProcessStartInfo(Server.MapPath("~/bin/ScreenShotUrl.exe"));
                startInfo.WindowStyle = ProcessWindowStyle.Hidden;

                startInfo.Arguments = string.Format("{0} {1} {2} {3} {4} {5}", url, string.IsNullOrEmpty(templateModel.SCREEN_W) ? 1920 : int.Parse(templateModel.SCREEN_W), string.IsNullOrEmpty(templateModel.SCREEN_H) ? 1080 : int.Parse(templateModel.SCREEN_H), CommonProperties.LAYOUT_THUMBNAIL_WIDTH, CommonProperties.LAYOUT_THUMBNAIL_HEIGHT, Path.Combine(saveRootPath, templateThumbnailFileName));
                Process.Start(startInfo);


                //CMSLib cmslib = new CMSLib();
                //using (Bitmap bitmap = cmslib.GenerateWebSiteThumbnailImage(url, string.IsNullOrEmpty(templateModel.SCREEN_W) ? 1920 : int.Parse(templateModel.SCREEN_W), string.IsNullOrEmpty(templateModel.SCREEN_H) ? 1080 : int.Parse(templateModel.SCREEN_H), CommonProperties.LAYOUT_THUMBNAIL_WIDTH, CommonProperties.LAYOUT_THUMBNAIL_HEIGHT))
                //{
                //    bitmap.Save(Path.Combine(saveRootPath, templateThumbnailFileName), ImageFormat.Jpeg);
                //}

                //using (Bitmap bitmap = cMSLib.Capture(url, 1920, 1080))
                //{
                //    bitmap.Save(Path.Combine(saveRootPath, templateCaptureFileName), ImageFormat.Jpeg);
                //}

                //Util.CreateThumbnail(Path.Combine(saveRootPath, templateCaptureFileName), Path.Combine(saveRootPath, templateThumbnailFileName), CommonProperties.LAYOUT_THUMBNAIL_WIDTH);

                #endregion

                #region 07. 저장한 썸네일 이미지 경로 및 URL DB 업데이트

                // 7. 파일 저장 정보 DB UPDATE
                templateModel.TEMPLATE_ID = templateId;
                templateModel.FILE_PATH = saveRootPath;
                templateModel.FILE_NM = templateFileName;
                templateModel.THUMBNAIL_NM = string.Format("{0}/upload/template/{1}/{2}/{3}", CommonProperties.HTTPS_DOMAIN_URL, UserBiz.getRestaurantCode(User), templateId, templateThumbnailFileName); // Request.Headers["host"] + "/upload/template/" + UserBiz.getRestaurantCode(User) + "/" + templateId + "/" + templateThumbnailFileName;
                templateModel.TEMPLATE_URL = string.Format("{0}/upload/template/{1}/{2}/{3}", CommonProperties.HTTPS_DOMAIN_URL, UserBiz.getRestaurantCode(User), templateId, templateFileName);          // Request.Headers["host"] + "/upload/template/" + UserBiz.getRestaurantCode(User) + "/" + templateId + "/" + templateFileName;

                resultModel = TemplateBiz.ManageTemplate("U", UserBiz.getUserId(User), templateModel);
                resultModel = TemplateBiz.ManageTemplateMap("U", UserBiz.getRestaurantCode(User), templateMapModels, UserBiz.getUserId(User));

                #endregion
            }
            catch (Exception ex)
            {
                if (!string.IsNullOrEmpty(templateId) && TYPE.ToUpper().Equals("I"))
                {
                    // 1, DB 삭제
                    templateModel.TEMPLATE_ID = templateId;
                    TemplateBiz.ManageTemplate("D", UserBiz.getUserId(User), templateModel);

                    // 2. FILE 삭제
                    DirectoryInfo dir = new DirectoryInfo(saveRootPath);
                    if (dir.Exists)
                        dir.Delete(true);
                }

                resultModel.ERR_CODE = "9999";
                resultModel.ERROR_MSG = ex.Message;
            }

            return Content(JsonConvert.SerializeObject(resultModel, Formatting.Indented));
        }

        /// <summary>
        /// 템플릿 저장/수정
        /// </summary>
        /// <param name="TYPE"></param>
        /// <param name="templateModel"></param>
        /// <param name="templateMapModels"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult SaveTemplate(string TYPE, TemplateModel templateModel, List<TemplateMapModel> templateMapModels)
        {
            ResultModel resultModel = userCheck();

            //2019-10-22 XSS 공격에 대응하기 위해 넘어온 object에 대한 model 정보를 replace함
            CommonProperties.XSSCheck_TemplateModel(templateModel);
            CommonProperties.XSSCheck_TemplateMapModel(templateMapModels);

            if (resultModel.ERROR_MSG != "")
                return Content(JsonConvert.SerializeObject(resultModel, Formatting.Indented));

            // 저장할 템플릿 루트 경로
            string saveRootPath = string.Empty;
            string templateFileName = string.Empty;
            string templateCaptureFileName = string.Empty;
            string templateThumbnailFileName = string.Empty;
            string templateId = string.Empty;

            try
            {
                #region 00. 템플릿 정보가 없을 경우 강제 오류

                if (string.IsNullOrEmpty(templateModel.TEMPLATE_HTML))
                    throw new Exception("템플릿 정보(HTML)가 없습니다.");

                #endregion

                #region 01. 템플릿과 연결된 컨텐츠 검증

                bool isContents = true;                    // 템플릿과 연결된 컨텐츠 파일이 모두 존재하면 true, 하나라도 없으면 false
                string contentsFileName = string.Empty;

                if (templateMapModels != null)
                {
                    if (TYPE.ToUpper().Equals("I"))
                    {
                        for (int i = 0; i < templateMapModels.Count; i++)
                        {

                            if (!string.IsNullOrEmpty(templateMapModels[i].CONTENT_FILE_PATH) && !string.IsNullOrEmpty(templateMapModels[i].CONTENT_FILE_NM))
                            {
                                if (!System.IO.File.Exists(Path.Combine(templateMapModels[i].CONTENT_FILE_PATH, templateMapModels[i].CONTENT_FILE_NM)))
                                {
                                    isContents = false;
                                    contentsFileName = templateMapModels[i].CONNECTION_NM;
                                    break;
                                }
                            }
                        }
                    }
                }

                if (isContents == false)
                {
                    throw new Exception(contentsFileName + " 컨텐츠 파일을 찾을 수 없어 사용할 수 없습니다.\n 다른 컨텐츠를 선택해주세요.");
                }

                #endregion

                #region 02. 템플릿 아이디 설정

                if (TYPE.ToUpper().Equals("I"))
                {
                    // DB 저장 후 생성된 아이디를 리턴 받음
                    resultModel = TemplateBiz.ManageTemplateAll("I", UserBiz.getUserId(User), templateModel, templateMapModels);
                    templateId = resultModel.ID;
                }
                else if (TYPE.ToUpper().Equals("U"))
                {
                    templateId = templateModel.TEMPLATE_ID;
                }

                #endregion

                #region 03. 템플릿 파일 저장 (동일한 이름의 파일이 있을경우(수정일경우) 파일을 덮어씀)

                // 루트 경로 셋팅
                saveRootPath = Path.Combine(Server.MapPath("~/"), CommonProperties.TEMPLATE_SAVE_PATH(UserBiz.getRestaurantCode(User), templateId));
                templateFileName = string.Format("T{0}.html", templateId);
                templateCaptureFileName = string.Format("T{0}.jpg", templateId);
                templateThumbnailFileName = string.Format("THUMBNAIL_T{0}.jpg", templateId);

                if (!Directory.Exists(saveRootPath))
                    Directory.CreateDirectory(saveRootPath);

                // 템플릿 URL 변경
                templateModel.TEMPLATE_HTML = templateModel.TEMPLATE_HTML.Replace("{TEMPLATE_PATH}", string.Format("{0}/upload/template/{1}/{2}/images", CommonProperties.HTTP_DOMAIN_URL, UserBiz.getRestaurantCode(User), templateId));

                // 템플릿(HTML) 파일 저장
                System.IO.File.WriteAllText(Path.Combine(saveRootPath, templateFileName), templateModel.TEMPLATE_HTML, System.Text.Encoding.UTF8);

                #endregion

                #region 04. 템플릿을 생성할때 사용한 레이아웃의 고정 이미지를 복사해옴

                string sourceDirName = Path.Combine(Server.MapPath("~/"), CommonProperties.LAYOUT_UPLOAD_PATH, templateModel.LAYOUT_ID, "images");  // 복사할 디렉토리
                string targetDirName = Path.Combine(saveRootPath, "images");                                                                        // 저장할 디렉토리

                // 생성일 경우만 고정 이미지를 복사해옴 (수정일 경우는 이미 복사한 고정 이미지가 있음)
                if (TYPE.ToUpper().Equals("I"))
                {
                    DirectoryInfo dir = new DirectoryInfo(sourceDirName);

                    // 저장할 디렉토리가 없을 경우 생성
                    if (!Directory.Exists(targetDirName))
                        Directory.CreateDirectory(targetDirName);

                    // 레이아웃의 고정이미지가 있을 경우 실행
                    if (dir.Exists)
                    {
                        DirectoryInfo[] dirs = dir.GetDirectories();

                        FileInfo[] files = dir.GetFiles();

                        foreach (FileInfo file in files)
                        {
                            string uniqFileName = Util.GetUniqName(targetDirName, Path.GetFileNameWithoutExtension(file.Name), Path.GetExtension(file.Name));

                            string temppath = Path.Combine(targetDirName, file.Name);
                            file.CopyTo(temppath, true);
                        }
                    }
                }

                #endregion

                #region 05. 템플릿과 연결된 컨텐츠를 복사해옴

                // 5. 템플릿(HTML)과 연결된 컨텐츠 파일 복사
                if (templateMapModels != null)
                {
                    for (int i = 0; i < templateMapModels.Count; i++)
                    {
                        templateMapModels[i].TEMPLATE_ID = templateId;

                        // 템플릿 생성일 경우 
                        // 템플릿 수정일 경우
                        if (TYPE.ToUpper().Equals("I") || TYPE.ToUpper().Equals("U"))
                        {
                            // 연결한 컨텐츠의 파일 경로가 있을 경우 실행
                            if (!string.IsNullOrEmpty(templateMapModels[i].CONTENT_FILE_PATH) && !string.IsNullOrEmpty(templateMapModels[i].CONTENT_FILE_NM))
                            {
                                // 템플릿 수정
                                // 기존 컨텐츠 수정
                                // 기존 컨텐츠 파일명
                                // 기존 컨텐츠 파일 존재
                                // 기존 파일 삭제
                                if (TYPE.ToUpper().Equals("U") && !string.IsNullOrEmpty(templateMapModels[i].FILE_NM) && System.IO.File.Exists(Path.Combine(targetDirName, templateMapModels[i].FILE_NM)))
                                {
                                    System.IO.File.Delete(Path.Combine(targetDirName, templateMapModels[i].FILE_NM));
                                }

                                string uniqFileName = Util.GetUniqName(targetDirName, Path.GetFileNameWithoutExtension(templateMapModels[i].CONTENT_FILE_NM), Path.GetExtension(templateMapModels[i].CONTENT_FILE_NM));

                                System.IO.File.Copy(Path.Combine(templateMapModels[i].CONTENT_FILE_PATH, templateMapModels[i].CONTENT_FILE_NM), Path.Combine(targetDirName, uniqFileName), true);
                                templateMapModels[i].FILE_NM = Path.Combine(targetDirName, uniqFileName);
                            }
                        }
                    }
                }

                #endregion

                #region 06. 저장된 템플릿 스크린샷 및 썸네일 이미지 생성

                string url = string.Format("{0}/upload/template/{1}/{2}/{3}?type=S", CommonProperties.HTTPS_DOMAIN_URL, UserBiz.getRestaurantCode(User), templateId, templateFileName);

                ProcessStartInfo startInfo = new ProcessStartInfo(Server.MapPath("~/bin/ScreenShotUrl.exe"));
                startInfo.WindowStyle = ProcessWindowStyle.Hidden;

                startInfo.Arguments = string.Format("{0} {1} {2} {3} {4} {5}", url, string.IsNullOrEmpty(templateModel.SCREEN_W) ? 1920 : int.Parse(templateModel.SCREEN_W), string.IsNullOrEmpty(templateModel.SCREEN_H) ? 1080 : int.Parse(templateModel.SCREEN_H), CommonProperties.LAYOUT_THUMBNAIL_WIDTH, CommonProperties.LAYOUT_THUMBNAIL_HEIGHT, Path.Combine(saveRootPath, templateThumbnailFileName));
                Process.Start(startInfo);


                //CMSLib cmslib = new CMSLib();
                //using (Bitmap bitmap = cmslib.GenerateWebSiteThumbnailImage(url, string.IsNullOrEmpty(templateModel.SCREEN_W) ? 1920 : int.Parse(templateModel.SCREEN_W), string.IsNullOrEmpty(templateModel.SCREEN_H) ? 1080 : int.Parse(templateModel.SCREEN_H), CommonProperties.LAYOUT_THUMBNAIL_WIDTH, CommonProperties.LAYOUT_THUMBNAIL_HEIGHT))
                //{
                //    bitmap.Save(Path.Combine(saveRootPath, templateThumbnailFileName), ImageFormat.Jpeg);
                //}

                //using (Bitmap bitmap = cMSLib.Capture(url, 1920, 1080))
                //{
                //    bitmap.Save(Path.Combine(saveRootPath, templateCaptureFileName), ImageFormat.Jpeg);
                //}

                //Util.CreateThumbnail(Path.Combine(saveRootPath, templateCaptureFileName), Path.Combine(saveRootPath, templateThumbnailFileName), CommonProperties.LAYOUT_THUMBNAIL_WIDTH);

                #endregion

                #region 07. 저장한 썸네일 이미지 경로 및 URL DB 업데이트

                // 7. 파일 저장 정보 DB UPDATE
                templateModel.TEMPLATE_ID = templateId;
                templateModel.FILE_PATH = saveRootPath;
                templateModel.FILE_NM = templateFileName;
                templateModel.THUMBNAIL_NM = string.Format("{0}/upload/template/{1}/{2}/{3}", CommonProperties.HTTPS_DOMAIN_URL, UserBiz.getRestaurantCode(User), templateId, templateThumbnailFileName); // Request.Headers["host"] + "/upload/template/" + UserBiz.getRestaurantCode(User) + "/" + templateId + "/" + templateThumbnailFileName;
                templateModel.TEMPLATE_URL = string.Format("{0}/upload/template/{1}/{2}/{3}", CommonProperties.HTTPS_DOMAIN_URL, UserBiz.getRestaurantCode(User), templateId, templateFileName);          // Request.Headers["host"] + "/upload/template/" + UserBiz.getRestaurantCode(User) + "/" + templateId + "/" + templateFileName;

                resultModel = TemplateBiz.ManageTemplate("U", UserBiz.getUserId(User), templateModel);
                resultModel = TemplateBiz.ManageTemplateMap("U", UserBiz.getRestaurantCode(User), templateMapModels, UserBiz.getUserId(User));

                #endregion
            }
            catch (Exception ex)
            {
                if (!string.IsNullOrEmpty(templateId) && TYPE.ToUpper().Equals("I"))
                {
                    // 1, DB 삭제
                    templateModel.TEMPLATE_ID = templateId;
                    TemplateBiz.ManageTemplate("D", UserBiz.getUserId(User), templateModel);

                    // 2. FILE 삭제
                    DirectoryInfo dir = new DirectoryInfo(saveRootPath);
                    if (dir.Exists)
                        dir.Delete(true);
                }

                resultModel.ERR_CODE = "9999";
                resultModel.ERROR_MSG = ex.Message;
            }

            return Content(JsonConvert.SerializeObject(resultModel, Formatting.Indented));
        }

        /// <summary>
        /// 템플릿 삭제
        /// </summary>
        /// <param name="templateModel"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult DeleteTemplate(TemplateModel templateModel)
        {
            ResultModel resultModel = userCheck();

            if (resultModel.ERROR_MSG != "")
                return Content(JsonConvert.SerializeObject(resultModel, Formatting.Indented));

            try
            {
                resultModel = TemplateBiz.ManageTemplateAll("D", UserBiz.getUserId(User), templateModel, null);

                if (resultModel.ERR_CODE.Equals("0") && Directory.Exists(Path.Combine(Server.MapPath("~/"), CommonProperties.TEMPLATE_SAVE_PATH(UserBiz.getRestaurantCode(User), templateModel.TEMPLATE_ID))))
                    Directory.Delete(Path.Combine(Server.MapPath("~/"), CommonProperties.TEMPLATE_SAVE_PATH(UserBiz.getRestaurantCode(User), templateModel.TEMPLATE_ID)), true);
            }
            catch (Exception ex)
            {
                resultModel.ERR_CODE = "9999";
                resultModel.ERROR_MSG = ex.Message;
            }

            return Content(JsonConvert.SerializeObject(TemplateBiz.ManageTemplateAll("D", UserBiz.getUserId(User), templateModel, null), Formatting.Indented));
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