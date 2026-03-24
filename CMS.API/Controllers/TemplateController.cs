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
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult ManageTemplate(string type, string id)
        {
            ResultModel resultModel = userCheck();

            if (string.IsNullOrEmpty(type) || string.IsNullOrEmpty(id))
            {
                return Redirect(Url.Action("index", "template"));
            }

            ViewBag.type = type;

            TemplateModel templateModel = new TemplateModel();

            templateModel.RESTAURANT_CODE = UserBiz.getRestaurantCode(User);

            if (type.ToUpper().Equals("I"))
            {
                templateModel.LAYOUT_ID = id;
            }
            else if (type.ToUpper().Equals("U"))
            {
                templateModel.TEMPLATE_ID = id;
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
            {
                string layoutFilePath = Server.MapPath(string.Format("/upload/layout/{0}/{1}", layoutModel.LAYOUT_ID, layoutModel.FILE_NM));
                file = new FileInfo(layoutFilePath);
            }
            else if (type.ToUpper().Equals("U"))
            {
                if (string.IsNullOrEmpty(templateModel.FILE_PATH) || string.IsNullOrEmpty(templateModel.FILE_NM))
                {
                    return Redirect(Url.Action("index", "template"));
                }

                string templateFilePath = Path.Combine(templateModel.FILE_PATH, templateModel.FILE_NM);
                file = new FileInfo(templateFilePath);
            }

            if (file.Exists)
            {
                return View(templateModel);
            }

            return Redirect(Url.Action("index", "template"));
        }

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
            {
                var ds = TemplateBiz.getTemplatePageList(templateModel);
                return Content(JsonConvert.SerializeObject(ds, Formatting.Indented));
            }
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

            CommonProperties.XSSCheck_TemplateModel(templateModel);
            CommonProperties.XSSCheck_TemplateMapModel(templateMapModels);
            if (templateModel != null && string.IsNullOrEmpty(templateModel.TEMPLATE_NM))
            {
                resultModel.ERROR_MSG = "템플릿 이름이 잘못되었습니다.";
                resultModel.ERR_CODE = "8001";
                return Content(JsonConvert.SerializeObject(resultModel, Formatting.Indented));
            }

            if (templateModel.RESTAURANT_CODE != UserBiz.getRestaurantCode(User))
            {
                resultModel.ERROR_MSG = "비정상 데이터 교환으로 인하여 로그아웃됩니다.";
                resultModel.ERR_CODE = "9998";
                return Content(JsonConvert.SerializeObject(resultModel, Formatting.Indented));
            }

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
                bool isContents = true;
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
                    throw new Exception(contentsFileName + " 컨텐츠 파일을 찾을 수 없어 사용할 수 없습니다.\n 다른 컨텐츠를 선택해주세요.");
                #endregion

                #region 02. 템플릿 아이디 설정
                if (TYPE.ToUpper().Equals("I"))
                {
                    resultModel = TemplateBiz.ManageTemplateAll("I", UserBiz.getUserId(User), templateModel, templateMapModels);
                    templateId = resultModel.ID;
                }
                else if (TYPE.ToUpper().Equals("U"))
                {
                    templateId = templateModel.TEMPLATE_ID;
                }
                #endregion

                #region 03. 템플릿 파일 저장
                saveRootPath = Path.Combine(Server.MapPath("~/"), CommonProperties.TEMPLATE_SAVE_PATH(UserBiz.getRestaurantCode(User), templateId));
                templateFileName = string.Format("T{0}.html", templateId);
                templateCaptureFileName = string.Format("T{0}.jpg", templateId);
                templateThumbnailFileName = string.Format("THUMBNAIL_T{0}.jpg", templateId);

                if (!Directory.Exists(saveRootPath))
                    Directory.CreateDirectory(saveRootPath);

                templateModel.TEMPLATE_HTML = templateModel.TEMPLATE_HTML.Replace("{TEMPLATE_PATH}", string.Format("{0}/upload/template/{1}/{2}/images", CommonProperties.HTTP_DOMAIN_URL, UserBiz.getRestaurantCode(User), templateId));
                System.IO.File.WriteAllText(Path.Combine(saveRootPath, templateFileName), templateModel.TEMPLATE_HTML, System.Text.Encoding.UTF8);
                #endregion

                #region 04. 레이아웃 고정 이미지 복사
                string targetDirName = Path.Combine(saveRootPath, "images");

                if (!string.IsNullOrEmpty(templateModel.LAYOUT_ID))
                {
                    string sourceDirName = Path.Combine(Server.MapPath("~/"), CommonProperties.LAYOUT_UPLOAD_PATH, templateModel.LAYOUT_ID, "images");

                    if (TYPE.ToUpper().Equals("I"))
                    {
                        DirectoryInfo dir = new DirectoryInfo(sourceDirName);

                        if (!Directory.Exists(targetDirName))
                            Directory.CreateDirectory(targetDirName);

                        if (dir.Exists)
                        {
                            FileInfo[] files = dir.GetFiles();
                            foreach (FileInfo file in files)
                            {
                                string uniqFileName = Util.GetUniqName(targetDirName, Path.GetFileNameWithoutExtension(file.Name), Path.GetExtension(file.Name));
                                string temppath = Path.Combine(targetDirName, file.Name);
                                file.CopyTo(temppath, true);
                            }
                        }
                    }
                }
                #endregion

                #region 05. 컨텐츠 파일 복사
                if (templateMapModels != null)
                {
                    for (int i = 0; i < templateMapModels.Count; i++)
                    {
                        templateMapModels[i].TEMPLATE_ID = templateId;

                        if (TYPE.ToUpper().Equals("I") || TYPE.ToUpper().Equals("U"))
                        {
                            if (!string.IsNullOrEmpty(templateMapModels[i].CONTENT_FILE_PATH) && !string.IsNullOrEmpty(templateMapModels[i].CONTENT_FILE_NM))
                            {
                                if (TYPE.ToUpper().Equals("U") && !string.IsNullOrEmpty(templateMapModels[i].FILE_NM) && System.IO.File.Exists(Path.Combine(targetDirName, templateMapModels[i].FILE_NM)))
                                    System.IO.File.Delete(Path.Combine(targetDirName, templateMapModels[i].FILE_NM));

                                string uniqFileName = Util.GetUniqName(targetDirName, Path.GetFileNameWithoutExtension(templateMapModels[i].CONTENT_FILE_NM), Path.GetExtension(templateMapModels[i].CONTENT_FILE_NM));
                                System.IO.File.Copy(Path.Combine(templateMapModels[i].CONTENT_FILE_PATH, templateMapModels[i].CONTENT_FILE_NM), Path.Combine(targetDirName, uniqFileName), true);
                                templateMapModels[i].FILE_NM = Path.Combine(targetDirName, uniqFileName);
                            }
                        }
                    }
                }
                #endregion

                #region 06. 썸네일 이미지 생성
                // 썸네일 생성용 임시 HTML: 외부 IP를 127.0.0.1로 치환하여 ScreenShotUrl.exe가 내부에서 접근 가능하게 함
                string savedHtml = System.IO.File.ReadAllText(Path.Combine(saveRootPath, templateFileName), System.Text.Encoding.UTF8);
                string internalHtml = savedHtml.Replace(CommonProperties.HTTP_DOMAIN_URL, CommonProperties.INTERNAL_DOMAIN_URL)
                                               .Replace(CommonProperties.HTTPS_DOMAIN_URL, CommonProperties.INTERNAL_DOMAIN_URL);
                string tempFileName = string.Format("TEMP_T{0}.html", templateId);
                string tempFilePath = Path.Combine(saveRootPath, tempFileName);
                System.IO.File.WriteAllText(tempFilePath, internalHtml, System.Text.Encoding.UTF8);

                string url = string.Format("{0}/upload/template/{1}/{2}/{3}?type=S", CommonProperties.INTERNAL_DOMAIN_URL, UserBiz.getRestaurantCode(User), templateId, tempFileName);
                string thumbnailOutputPath = Path.Combine(saveRootPath, templateThumbnailFileName);
                string screenshotExePath = Server.MapPath("~/bin/ScreenShotUrl.exe");
                string screenshotArgs = string.Format("{0} {1} {2} {3} {4} {5}",
                    url,
                    string.IsNullOrEmpty(templateModel.SCREEN_W) ? 1920 : int.Parse(templateModel.SCREEN_W),
                    string.IsNullOrEmpty(templateModel.SCREEN_H) ? 1080 : int.Parse(templateModel.SCREEN_H),
                    CommonProperties.LAYOUT_THUMBNAIL_WIDTH,
                    CommonProperties.LAYOUT_THUMBNAIL_HEIGHT,
                    thumbnailOutputPath);

                ProcessStartInfo startInfo = new ProcessStartInfo(screenshotExePath);
                startInfo.WindowStyle = ProcessWindowStyle.Hidden;
                startInfo.WorkingDirectory = Server.MapPath("~/bin/");
                startInfo.UseShellExecute = false;
                startInfo.CreateNoWindow = true;
                startInfo.Arguments = screenshotArgs;

                Process process = Process.Start(startInfo);
                if (process != null)
                {
                    bool exited = process.WaitForExit(30000);
                    process.Close();
                    if (!exited)
                        throw new Exception("썸네일 생성 시간이 초과되었습니다. (30초)");
                }

                // 임시 HTML 삭제
                if (System.IO.File.Exists(tempFilePath))
                    System.IO.File.Delete(tempFilePath);

                string thumbnailPath = Path.Combine(saveRootPath, templateThumbnailFileName);
                if (!System.IO.File.Exists(thumbnailPath))
                    throw new Exception("썸네일 이미지 생성에 실패했습니다. 템플릿을 저장할 수 없습니다.");
                #endregion

                #region 07. DB 업데이트
                templateModel.TEMPLATE_ID = templateId;
                templateModel.FILE_PATH = saveRootPath;
                templateModel.FILE_NM = templateFileName;
                templateModel.FILE_EXT = Path.GetExtension(templateFileName).TrimStart('.');

                FileInfo fileInfo = new FileInfo(Path.Combine(saveRootPath, templateFileName));
                templateModel.FILE_SIZE = fileInfo.Length.ToString();

                templateModel.THUMBNAIL_NM = string.Format("{0}/upload/template/{1}/{2}/{3}", CommonProperties.HTTPS_DOMAIN_URL, UserBiz.getRestaurantCode(User), templateId, templateThumbnailFileName);
                templateModel.TEMPLATE_URL = string.Format("{0}/upload/template/{1}/{2}/{3}", CommonProperties.HTTPS_DOMAIN_URL, UserBiz.getRestaurantCode(User), templateId, templateFileName);

                resultModel = TemplateBiz.ManageTemplate("U", UserBiz.getUserId(User), templateModel);
                resultModel = TemplateBiz.ManageTemplateMap("U", UserBiz.getRestaurantCode(User), templateMapModels, UserBiz.getUserId(User));
                #endregion
            }
            catch (Exception ex)
            {
                if (!string.IsNullOrEmpty(templateId) && TYPE.ToUpper().Equals("I"))
                {
                    templateModel.TEMPLATE_ID = templateId;
                    TemplateBiz.ManageTemplate("D", UserBiz.getUserId(User), templateModel);

                    DirectoryInfo dir = new DirectoryInfo(saveRootPath);
                    if (dir.Exists)
                        dir.Delete(true);
                }

                resultModel.ERR_CODE = "9999";
                resultModel.ERROR_MSG = ex.Message;
            }

            return Content(JsonConvert.SerializeObject(resultModel, Formatting.Indented));
        }


        [HttpPost]
        public ActionResult SaveTemplate(string TYPE, TemplateModel templateModel, List<TemplateMapModel> templateMapModels)
        {
            ResultModel resultModel = userCheck();

            CommonProperties.XSSCheck_TemplateModel(templateModel);
            CommonProperties.XSSCheck_TemplateMapModel(templateMapModels);

            if (resultModel.ERROR_MSG != "")
                return Content(JsonConvert.SerializeObject(resultModel, Formatting.Indented));

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

                #region 01. 컨텐츠 검증
                bool isContents = true;
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
                    throw new Exception(contentsFileName + " 컨텐츠 파일을 찾을 수 없어 사용할 수 없습니다.\n 다른 컨텐츠를 선택해주세요.");
                #endregion

                #region 02. 템플릿 아이디 설정
                if (TYPE.ToUpper().Equals("I"))
                {
                    resultModel = TemplateBiz.ManageTemplateAll("I", UserBiz.getUserId(User), templateModel, templateMapModels);
                    templateId = resultModel.ID;
                }
                else if (TYPE.ToUpper().Equals("U"))
                {
                    templateId = templateModel.TEMPLATE_ID;
                }
                #endregion

                #region 03. 템플릿 파일 저장
                saveRootPath = Path.Combine(Server.MapPath("~/"), CommonProperties.TEMPLATE_SAVE_PATH(UserBiz.getRestaurantCode(User), templateId));
                templateFileName = string.Format("T{0}.html", templateId);
                templateCaptureFileName = string.Format("T{0}.jpg", templateId);
                templateThumbnailFileName = string.Format("THUMBNAIL_T{0}.jpg", templateId);

                if (!Directory.Exists(saveRootPath))
                    Directory.CreateDirectory(saveRootPath);

                templateModel.TEMPLATE_HTML = templateModel.TEMPLATE_HTML.Replace("{TEMPLATE_PATH}", string.Format("{0}/upload/template/{1}/{2}/images", CommonProperties.HTTP_DOMAIN_URL, UserBiz.getRestaurantCode(User), templateId));
                System.IO.File.WriteAllText(Path.Combine(saveRootPath, templateFileName), templateModel.TEMPLATE_HTML, System.Text.Encoding.UTF8);
                #endregion

                #region 04. 레이아웃 고정 이미지 복사
                string targetDirName = Path.Combine(saveRootPath, "images");

                if (!string.IsNullOrEmpty(templateModel.LAYOUT_ID))
                {
                    string sourceDirName = Path.Combine(Server.MapPath("~/"), CommonProperties.LAYOUT_UPLOAD_PATH, templateModel.LAYOUT_ID, "images");

                    if (TYPE.ToUpper().Equals("I"))
                    {
                        DirectoryInfo dir = new DirectoryInfo(sourceDirName);

                        if (!Directory.Exists(targetDirName))
                            Directory.CreateDirectory(targetDirName);

                        if (dir.Exists)
                        {
                            FileInfo[] files = dir.GetFiles();
                            foreach (FileInfo file in files)
                            {
                                string uniqFileName = Util.GetUniqName(targetDirName, Path.GetFileNameWithoutExtension(file.Name), Path.GetExtension(file.Name));
                                string temppath = Path.Combine(targetDirName, file.Name);
                                file.CopyTo(temppath, true);
                            }
                        }
                    }
                }
                #endregion

                #region 05. 컨텐츠 파일 복사
                if (templateMapModels != null)
                {
                    for (int i = 0; i < templateMapModels.Count; i++)
                    {
                        templateMapModels[i].TEMPLATE_ID = templateId;

                        if (TYPE.ToUpper().Equals("I") || TYPE.ToUpper().Equals("U"))
                        {
                            if (!string.IsNullOrEmpty(templateMapModels[i].CONTENT_FILE_PATH) && !string.IsNullOrEmpty(templateMapModels[i].CONTENT_FILE_NM))
                            {
                                if (TYPE.ToUpper().Equals("U") && !string.IsNullOrEmpty(templateMapModels[i].FILE_NM) && System.IO.File.Exists(Path.Combine(targetDirName, templateMapModels[i].FILE_NM)))
                                    System.IO.File.Delete(Path.Combine(targetDirName, templateMapModels[i].FILE_NM));

                                string uniqFileName = Util.GetUniqName(targetDirName, Path.GetFileNameWithoutExtension(templateMapModels[i].CONTENT_FILE_NM), Path.GetExtension(templateMapModels[i].CONTENT_FILE_NM));
                                System.IO.File.Copy(Path.Combine(templateMapModels[i].CONTENT_FILE_PATH, templateMapModels[i].CONTENT_FILE_NM), Path.Combine(targetDirName, uniqFileName), true);
                                templateMapModels[i].FILE_NM = Path.Combine(targetDirName, uniqFileName);
                            }
                        }
                    }
                }
                #endregion

                #region 06. 썸네일 이미지 생성
                // 썸네일 생성용 임시 HTML: 외부 IP를 127.0.0.1로 치환하여 ScreenShotUrl.exe가 내부에서 접근 가능하게 함
                string savedHtml = System.IO.File.ReadAllText(Path.Combine(saveRootPath, templateFileName), System.Text.Encoding.UTF8);
                string internalHtml = savedHtml.Replace(CommonProperties.HTTP_DOMAIN_URL, CommonProperties.INTERNAL_DOMAIN_URL)
                                               .Replace(CommonProperties.HTTPS_DOMAIN_URL, CommonProperties.INTERNAL_DOMAIN_URL);
                string tempFileName = string.Format("TEMP_T{0}.html", templateId);
                string tempFilePath = Path.Combine(saveRootPath, tempFileName);
                System.IO.File.WriteAllText(tempFilePath, internalHtml, System.Text.Encoding.UTF8);

                string url = string.Format("{0}/upload/template/{1}/{2}/{3}?type=S", CommonProperties.INTERNAL_DOMAIN_URL, UserBiz.getRestaurantCode(User), templateId, tempFileName);
                string thumbnailOutputPath = Path.Combine(saveRootPath, templateThumbnailFileName);
                string screenshotExePath = Server.MapPath("~/bin/ScreenShotUrl.exe");
                string screenshotArgs = string.Format("{0} {1} {2} {3} {4} {5}",
                    url,
                    string.IsNullOrEmpty(templateModel.SCREEN_W) ? 1920 : int.Parse(templateModel.SCREEN_W),
                    string.IsNullOrEmpty(templateModel.SCREEN_H) ? 1080 : int.Parse(templateModel.SCREEN_H),
                    CommonProperties.LAYOUT_THUMBNAIL_WIDTH,
                    CommonProperties.LAYOUT_THUMBNAIL_HEIGHT,
                    thumbnailOutputPath);

                ProcessStartInfo startInfo = new ProcessStartInfo(screenshotExePath);
                startInfo.WindowStyle = ProcessWindowStyle.Hidden;
                startInfo.WorkingDirectory = Server.MapPath("~/bin/");
                startInfo.UseShellExecute = false;
                startInfo.CreateNoWindow = true;
                startInfo.Arguments = screenshotArgs;

                Process process = Process.Start(startInfo);
                if (process != null)
                {
                    bool exited = process.WaitForExit(30000);
                    process.Close();
                    if (!exited)
                        throw new Exception("썸네일 생성 시간이 초과되었습니다. (30초)");
                }

                // 임시 HTML 삭제
                if (System.IO.File.Exists(tempFilePath))
                    System.IO.File.Delete(tempFilePath);

                string thumbnailPath = Path.Combine(saveRootPath, templateThumbnailFileName);
                if (!System.IO.File.Exists(thumbnailPath))
                    throw new Exception("썸네일 이미지 생성에 실패했습니다. 템플릿을 저장할 수 없습니다.");
                #endregion

                #region 07. DB 업데이트
                templateModel.TEMPLATE_ID = templateId;
                templateModel.FILE_PATH = saveRootPath;
                templateModel.FILE_NM = templateFileName;
                templateModel.FILE_EXT = Path.GetExtension(templateFileName).TrimStart('.');

                FileInfo fileInfo = new FileInfo(Path.Combine(saveRootPath, templateFileName));
                templateModel.FILE_SIZE = fileInfo.Length.ToString();

                templateModel.THUMBNAIL_NM = string.Format("{0}/upload/template/{1}/{2}/{3}", CommonProperties.HTTPS_DOMAIN_URL, UserBiz.getRestaurantCode(User), templateId, templateThumbnailFileName);
                templateModel.TEMPLATE_URL = string.Format("{0}/upload/template/{1}/{2}/{3}", CommonProperties.HTTPS_DOMAIN_URL, UserBiz.getRestaurantCode(User), templateId, templateFileName);

                resultModel = TemplateBiz.ManageTemplate("U", UserBiz.getUserId(User), templateModel);
                resultModel = TemplateBiz.ManageTemplateMap("U", UserBiz.getRestaurantCode(User), templateMapModels, UserBiz.getUserId(User));
                #endregion
            }
            catch (Exception ex)
            {
                if (!string.IsNullOrEmpty(templateId) && TYPE.ToUpper().Equals("I"))
                {
                    templateModel.TEMPLATE_ID = templateId;
                    TemplateBiz.ManageTemplate("D", UserBiz.getUserId(User), templateModel);

                    DirectoryInfo dir = new DirectoryInfo(saveRootPath);
                    if (dir.Exists)
                        dir.Delete(true);
                }

                resultModel.ERR_CODE = "9999";
                resultModel.ERROR_MSG = ex.Message;
            }

            return Content(JsonConvert.SerializeObject(resultModel, Formatting.Indented));
        }

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
