using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Web;
using System.Web.Mvc;
using CMS.API.App_Code;
using CMS.API.Biz;
using CMS.API.Models;
using Newtonsoft.Json;

namespace CMS.API.Controllers
{
    [Authorize]
    public class ContentsController : BaseController
    {
        /// <summary>
        /// 컨텐츠 관리 화면
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// 컨텐츠 조회 (페이징)
        /// </summary>
        /// <param name="contentsModel"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult getContentsPageList(ContentsModel contentsModel)
        {
            ResultModel resultModel = userCheck();

            if (resultModel.ERROR_MSG != "")
                return Content(JsonConvert.SerializeObject(resultModel, Formatting.Indented));

            // 2019-05-14 컨텐츠 조회 데이터 변조 체크
            if (contentsModel.RESTAURANT_CODE != UserBiz.getRestaurantCode(User))
            {
                resultModel.ERROR_MSG = "비정상 데이터 교환으로 인하여 로그아웃됩니다.";
                resultModel.ERR_CODE = "9998";
                return Content(JsonConvert.SerializeObject(resultModel, Formatting.Indented));
            }

            DataSet ds = ContentsBiz.getContentsPageList(contentsModel);

            foreach (DataRow row in ds.Tables[0].Rows)
                row["TEXT_HTML"] = WebUtility.HtmlDecode(row["TEXT_HTML"].ToString());

            return Content(JsonConvert.SerializeObject(ds, Formatting.Indented));
        }

        /// <summary>
        /// 컨텐츠 저장(업로드)/수정 (헤드라인, 뱃지, 이미지, 동영상)
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult uploadFile()
        {
            ResultModel resultModel = userCheck();

            if (resultModel.ERROR_MSG != "")
                return Content(JsonConvert.SerializeObject(resultModel, Formatting.Indented));

            List<ContentsModel> contentsModels = new List<ContentsModel>();   // 생성된 컨텐츠 파일 정보를 저장 (파일 저장 이후 DB 저장 목적, 저장중 실패 하거나 DB 저장 실패시 롤백 목적)

            try
            {
                ContentsModel contentsModel = JsonConvert.DeserializeObject<ContentsModel>(Request.Form["contentsModel"]);

                //2019-10-22 XSS 공격에 대응하기 위해 넘어온 object에 대한 model 정보를 replace함
                CommonProperties.XSSCheck_ContentsModel(contentsModel);
                if (string.IsNullOrEmpty(contentsModel.CONTENT_NM))
                {
                    resultModel.ERR_CODE = "8001";
                    resultModel.ERROR_MSG = "컨텐츠 이름이 잘못되었습니다.";
                    return Content(JsonConvert.SerializeObject(resultModel, Formatting.Indented));
                }

                if (contentsModel.RESTAURANT_CODE != UserBiz.getRestaurantCode(User))
                {
                    resultModel.ERROR_MSG = "비정상 데이터 교환으로 인하여 로그아웃됩니다.";
                    resultModel.ERR_CODE = "9998";
                    return Content(JsonConvert.SerializeObject(resultModel, Formatting.Indented));
                }
                else
                {
                    // 2019-05-14 특수기호 입력 체크
                    //if (CommonProperties.speSymbol(contentsModel.CONTENT_NM))
                    //    throw new Exception("특수기호는 입력할 수 없습니다.");

                    string saveContentsPath = Path.Combine(Server.MapPath("~/"), CommonProperties.CONTENTS_UPLOAD_PATH, UserBiz.getRestaurantCode(User), contentsModel.CONTENT_TYPE, DateTime.Now.ToString("yyyyMMdd"));
                    string saveContentsThumbnailPath = Path.Combine(Server.MapPath("~/"), CommonProperties.CONTENTS_UPLOAD_PATH, UserBiz.getRestaurantCode(User), contentsModel.CONTENT_TYPE, DateTime.Now.ToString("yyyyMMdd"), "thumbnail");

                    if (Request.Form["TYPE"].ToString().Equals("U") && contentsModel.IS_FILE_UPDATE)
                    {
                        saveContentsPath = contentsModel.FILE_PATH;
                        saveContentsThumbnailPath = contentsModel.THUMBNAIL_PATH;

                        if (System.IO.File.Exists(Path.Combine(saveContentsPath, contentsModel.FILE_NM)))
                            System.IO.File.Delete(Path.Combine(saveContentsPath, contentsModel.FILE_NM));

                        if (System.IO.File.Exists(Path.Combine(saveContentsThumbnailPath, contentsModel.THUMBNAIL_NM)))
                            System.IO.File.Delete(Path.Combine(saveContentsThumbnailPath, contentsModel.THUMBNAIL_NM));
                    }

                    // 이미지, 헤드라인, 뱃지, 동영상
                    if (Request.Files.Count > 0)
                    {
                        foreach (string fileNm in Request.Files)
                        {
                            HttpPostedFileBase file = Request.Files[fileNm];

                            string fileFullName = Path.GetFileName(file.FileName);                                                                                    // 확장자 포함 파일명 (IE의 경우 전체 경로일 수 있음)
                            string fileExtension = Path.GetExtension(fileFullName);                                                                                    // 확장자(. 포함)
                            string fileName = Path.GetFileNameWithoutExtension(fileFullName);                                                                     // 확장자 제외 파일명 .Replace(fileExtension, string.Empty)
                            string fileUniqName = Util.GetUniqName(saveContentsPath, fileName, fileExtension);                                                        // 저장하려는 경로에 같은 이름의 파일이 있을 수 있으므로 유일한 파일명을 만듬
                            string fileThumbnailUniqName = Util.GetUniqName(saveContentsThumbnailPath, fileName, contentsModel.CONTENT_TYPE != "M" ? fileExtension : ".jpg");  // 썸네일 이미지 파일명
                            string convertFileThumbnailUniqName = string.Empty;

                            // 저장하려는 경로가 없을 경우 경로 생성
                            if (!Directory.Exists(saveContentsPath))
                                Directory.CreateDirectory(saveContentsPath);

                            if (!Directory.Exists(saveContentsThumbnailPath))
                                Directory.CreateDirectory(saveContentsThumbnailPath);

                            // 파일 저장
                            file.SaveAs(Path.Combine(saveContentsPath, fileUniqName));

                            // 웹에서 사용하지 못하는 동영상이 업로드 되었을 경우 MP4 파일로 변환
                            if (contentsModel.CONTENT_TYPE == "M")
                            {
                                ProcessStartInfo startInfo = new ProcessStartInfo(Server.MapPath("~/bin/ScreenShotMovie.exe"));
                                startInfo.WindowStyle = ProcessWindowStyle.Hidden;
                                startInfo.Arguments = string.Format("-i {0} -ss {1} -vframes 1 {2} {3} {4} {5}", Path.Combine(saveContentsPath, fileUniqName), "00:00:05", saveContentsThumbnailPath, fileThumbnailUniqName, CommonProperties.CONTETNS_THUMBNAIL_WIDTH, CommonProperties.CONTETNS_THUMBNAIL_HEIGHT);
                                Process.Start(startInfo);

                                // 웹에서 스트리밍할 수 없는 동영상의 경우 MP4 파일로 변환
                                if (fileExtension.ToUpper() != ".MP4" && fileExtension != ".WEBM" && fileExtension != ".OGG")
                                {
                                    // MP4 변환시 사용할 파일명
                                    string convertFileUniqName = Util.GetUniqName(saveContentsPath, fileName, ".mp4");

                                    startInfo = new ProcessStartInfo(Server.MapPath("~/bin/ConvertMP4.exe"));
                                    startInfo.WindowStyle = ProcessWindowStyle.Hidden;
                                    startInfo.Arguments = string.Format("-i {0} {1}", Path.Combine(saveContentsPath, fileUniqName), Path.Combine(saveContentsPath, convertFileUniqName));
                                    Process.Start(startInfo);

                                    fileUniqName = convertFileUniqName;
                                }
                            }

                            string YYYYMMDD = saveContentsPath.Substring(saveContentsPath.LastIndexOf("\\") + 1);

                            ContentsModel tempModel = new ContentsModel();
                            tempModel.CONTENT_ID = contentsModel.CONTENT_ID;
                            tempModel.CONTENT_NM = contentsModel.CONTENT_NM;
                            tempModel.CONTENT_TYPE = contentsModel.CONTENT_TYPE;
                            tempModel.TEXT_HTML = contentsModel.TEXT_HTML;
                            tempModel.FILE_PATH = saveContentsPath;
                            tempModel.THUMBNAIL_PATH = saveContentsThumbnailPath;
                            tempModel.FILE_NM = fileUniqName;
                            tempModel.THUMBNAIL_NM = fileThumbnailUniqName;
                            tempModel.FILE_EXT = fileExtension;
                            tempModel.FILE_SIZE = file.ContentLength.ToString();
                            tempModel.FILE_URL = string.Format("{0}/{1}/{2}/{3}/{4}/{5}", CommonProperties.HTTPS_DOMAIN_URL, CommonProperties.CONTENTS_UPLOAD_PATH, UserBiz.getRestaurantCode(User), contentsModel.CONTENT_TYPE, YYYYMMDD, fileUniqName);
                            tempModel.THUMBNAIL_URL = string.Format("{0}/{1}/{2}/{3}/{4}/{5}/{6}", CommonProperties.HTTPS_DOMAIN_URL, CommonProperties.CONTENTS_UPLOAD_PATH, UserBiz.getRestaurantCode(User), contentsModel.CONTENT_TYPE, YYYYMMDD, "thumbnail", fileThumbnailUniqName);
                            tempModel.TEMPLATE_ORG_URL = string.Format("/image/{0}", fileUniqName);                                                                                                                                                               // 필요 없을듯
                            tempModel.VIDEO_DURATION = contentsModel.VIDEO_DURATION;

                            contentsModels.Add(tempModel);

                            switch (contentsModel.CONTENT_TYPE)
                            {
                                case "I":
                                case "H":
                                case "B":
                                    System.IO.Stream stream = file.InputStream;
                                    System.Drawing.Image image = System.Drawing.Image.FromStream(stream);

                                    // 썸네일 이미지 생성 (이미지, 헤드라인, 뱃지)
                                    Util.CreateThumbnail(Path.Combine(saveContentsPath, fileUniqName), Path.Combine(saveContentsThumbnailPath, fileThumbnailUniqName), CommonProperties.CONTETNS_THUMBNAIL_WIDTH, CommonProperties.CONTETNS_THUMBNAIL_HEIGHT);
                                    break;
                            }
                        }
                    }
                    // 수정일 경우 파일이 없을 수 있음
                    else
                    {
                        ContentsModel tempModel = new ContentsModel();
                        tempModel.CONTENT_ID = contentsModel.CONTENT_ID;
                        tempModel.CONTENT_NM = contentsModel.CONTENT_NM;
                        tempModel.CONTENT_TYPE = contentsModel.CONTENT_TYPE;

                        contentsModels.Add(tempModel);
                    }

                    resultModel = ContentsBiz.manageContents(Request.Form["TYPE"].ToString(), UserBiz.getUserId(User), UserBiz.getRestaurantCode(User), contentsModels);
                }
            }
            catch (Exception ex)
            {
                resultModel.ERR_CODE = "9999";
                //resultModel.ERROR_MSG = ex.Message;
                resultModel.ERROR_MSG = "컨텐츠 업로드 중 오류가 발생하였습니다.\n데이터를 확인 후 다시 등록해주세요.";
            }

            return Content(JsonConvert.SerializeObject(resultModel, Formatting.Indented));
        }

        /// <summary>
        /// 컨텐츠 텍스트 저장/수정
        /// </summary>
        /// <param name="TYPE"></param>
        /// <param name="contentsModel"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult uploadText(string TYPE, ContentsModel contentsModel)
        {
            ResultModel resultModel = userCheck();

            //2019-10-22 XSS 공격에 대응하기 위해 넘어온 object에 대한 model 정보를 replace함
            CommonProperties.XSSCheck_ContentsModel(contentsModel);
            if (string.IsNullOrEmpty(contentsModel.CONTENT_NM))
            {
                resultModel.ERR_CODE = "8001";
                resultModel.ERROR_MSG = "컨텐츠 이름이 잘못되었습니다.";
            }

            if (resultModel.ERROR_MSG != "")
                return Content(JsonConvert.SerializeObject(resultModel, Formatting.Indented));

            if (contentsModel.RESTAURANT_CODE != UserBiz.getRestaurantCode(User))
            {
                resultModel.ERROR_MSG = "비정상 데이터 교환으로 인하여 로그아웃됩니다.";
                resultModel.ERR_CODE = "9998";
                return Content(JsonConvert.SerializeObject(resultModel, Formatting.Indented));
            }
            else
            {
                // 2019-05-14 특수기호 입력 체크
                //if (CommonProperties.speSymbol(contentsModel.CONTENT_NM) || CommonProperties.speSymbol(contentsModel.TEXT_HTML))
                //{
                //    resultModel.ERROR_MSG = "특수기호는 입력할 수 없습니다.";
                //    resultModel.ERR_CODE = "9999";
                //    return Content(JsonConvert.SerializeObject(resultModel, Formatting.Indented));
                //}

                List<ContentsModel> contentsModels = new List<ContentsModel>();

                contentsModels.Add(contentsModel);
                return Content(JsonConvert.SerializeObject(ContentsBiz.manageContents(TYPE, UserBiz.getUserId(User), UserBiz.getRestaurantCode(User), contentsModels), Formatting.Indented));
            }
        }

        /// <summary>
        /// 컨텐츠 URL 저장/수정
        /// </summary>
        /// <param name="TYPE"></param>
        /// <param name="contentsModel"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult uploadURL(string TYPE, ContentsModel contentsModel)
        {
            ResultModel resultModel = userCheck();

            //2019-10-22 XSS 공격에 대응하기 위해 넘어온 object에 대한 model 정보를 replace함
            CommonProperties.XSSCheck_ContentsModel(contentsModel);
            if (string.IsNullOrEmpty(contentsModel.CONTENT_NM))
            {
                resultModel.ERR_CODE = "8001";
                resultModel.ERROR_MSG = "컨텐츠 이름이 잘못되었습니다.";
            }

            if (resultModel.ERROR_MSG != "")
                return Content(JsonConvert.SerializeObject(resultModel, Formatting.Indented));

            if (contentsModel.RESTAURANT_CODE != UserBiz.getRestaurantCode(User))
            {
                resultModel.ERROR_MSG = "비정상 데이터 교환으로 인하여 로그아웃됩니다.";
                resultModel.ERR_CODE = "9998";
                return Content(JsonConvert.SerializeObject(resultModel, Formatting.Indented));
            }
            else
            {
                // 컨텐츠 저장 경로 기준
                // 루트경로/upload/contetns/레스토랑코드/컨텐츠타입/년월일/파일명
                string saveContentsPath = string.Empty;
                string saveContentsThumbnailPath = string.Empty;

                try
                {
                    // 2019-05-14 특수기호 입력 체크
                    //if (CommonProperties.speSymbol(contentsModel.CONTENT_NM) || CommonProperties.speSymbol(contentsModel.FILE_URL))
                    //{
                    //    resultModel.ERROR_MSG = "특수기호는 입력할 수 없습니다.";
                    //    resultModel.ERR_CODE = "9999";
                    //    return Content(JsonConvert.SerializeObject(resultModel, Formatting.Indented));
                    //}

                    if (TYPE.Equals("I"))
                    {
                        saveContentsPath = Path.Combine(Server.MapPath("~/"), CommonProperties.CONTENTS_UPLOAD_PATH, UserBiz.getRestaurantCode(User), contentsModel.CONTENT_TYPE, DateTime.Now.ToString("yyyyMMdd"));
                        saveContentsThumbnailPath = Path.Combine(Server.MapPath("~/"), CommonProperties.CONTENTS_UPLOAD_PATH, UserBiz.getRestaurantCode(User), contentsModel.CONTENT_TYPE, DateTime.Now.ToString("yyyyMMdd"), "thumbnail");

                    } // 업데이트일 경우 기존 원본 이미지와 썸네일 이미지를 삭제
                    else if (TYPE.Equals("U"))
                    {
                        saveContentsPath = contentsModel.FILE_PATH;
                        saveContentsThumbnailPath = contentsModel.THUMBNAIL_PATH;

                        if (System.IO.File.Exists(Path.Combine(saveContentsPath, contentsModel.FILE_NM)))
                            System.IO.File.Delete(Path.Combine(saveContentsPath, contentsModel.FILE_NM));

                        if (System.IO.File.Exists(Path.Combine(saveContentsThumbnailPath, contentsModel.THUMBNAIL_NM)))
                            System.IO.File.Delete(Path.Combine(saveContentsThumbnailPath, contentsModel.THUMBNAIL_NM));
                    }

                    string fileUniqName = Util.GetUniqName(saveContentsPath, "URL_IMAGE", ".jpg");
                    string fileThumbnailUniqName = Util.GetUniqName(saveContentsThumbnailPath, "URL_IMAGE_THUBNAIL", ".jpg");

                    // contentsModel.FILE_PATH          = saveContentsPath;          // 원본 이미지 경로
                    // contentsModel.FILE_NM            = fileUniqName;              // 원본 이미지 이름
                    contentsModel.THUMBNAIL_PATH = saveContentsThumbnailPath;    // 썸네일 이미지 경로
                    contentsModel.THUMBNAIL_NM = fileThumbnailUniqName;        // 썸네일 이미지 이름
                    contentsModel.THUMBNAIL_URL = string.Format("{0}/{1}/{2}/{3}/{4}/{5}/{6}", CommonProperties.HTTP_DOMAIN_URL, CommonProperties.CONTENTS_UPLOAD_PATH, UserBiz.getRestaurantCode(User), contentsModel.CONTENT_TYPE, DateTime.Now.ToString("yyyyMMdd"), "thumbnail", fileThumbnailUniqName);
                    contentsModel.TEMPLATE_ORG_URL = contentsModel.FILE_URL;

                    // 2026-03-18 contentsPath 주석처리
                    //if (!Directory.Exists(saveContentsPath))
                    //    Directory.CreateDirectory(saveContentsPath);

                    if (!Directory.Exists(saveContentsThumbnailPath))
                        Directory.CreateDirectory(saveContentsThumbnailPath);


                    ProcessStartInfo startInfo = new ProcessStartInfo(Server.MapPath("~/bin/ScreenShotUrl.exe"));
                    startInfo.WindowStyle = ProcessWindowStyle.Hidden;

                    startInfo.Arguments = string.Format("{0} {1} {2} {3} {4} {5}", contentsModel.FILE_URL, 1920, 1080, CommonProperties.CONTETNS_THUMBNAIL_WIDTH, CommonProperties.CONTETNS_THUMBNAIL_HEIGHT, Path.Combine(saveContentsThumbnailPath, fileThumbnailUniqName));
                    Process.Start(startInfo);

                    //CMSLib cMSLib = new CMSLib();

                    //using (Bitmap bitmap = cMSLib.Capture(contentsModel.FILE_URL, 1920, 1080))
                    //{
                    //    bitmap.Save(Path.Combine(contentsModel.FILE_PATH, contentsModel.FILE_NM), ImageFormat.Jpeg);
                    //}

                    //Util.CreateThumbnail(Path.Combine(saveContentsPath, fileUniqName), Path.Combine(saveContentsThumbnailPath, fileThumbnailUniqName), CommonProperties.CONTETNS_THUMBNAIL_WIDTH);

                    List<ContentsModel> contentsModels = new List<ContentsModel>();
                    contentsModels.Add(contentsModel);

                    resultModel = ContentsBiz.manageContents(TYPE, UserBiz.getUserId(User), UserBiz.getRestaurantCode(User), contentsModels);
                }
                catch (Exception ex)
                {
                    resultModel.ERR_CODE = "9999";
                    resultModel.ERROR_MSG = ex.Message;
                }

                return Content(JsonConvert.SerializeObject(resultModel, Formatting.Indented));
            }
        }

        /// <summary>
        /// 컨텐츠 삭제
        /// </summary>
        /// <param name="contentsModels"></param>
        /// <returns></returns>
        [HttpPost]
        public string manageDelete(List<ContentsModel> contentsModels)
        {
            ResultModel resultModel = userCheck();

            if (resultModel.ERROR_MSG != "")
                return JsonConvert.SerializeObject(resultModel, Formatting.Indented);

            try
            {
                resultModel = ContentsBiz.manageDelete(UserBiz.getRestaurantCode(User), UserBiz.getUserId(User), contentsModels);

                if (resultModel.ERR_CODE.Equals("0"))
                {
                    for (int i = 0; i < contentsModels.Count; i++)
                    {
                        // 2026.03.18 컨텐츠 조건(URL) 조건 추가
                        if (contentsModels[i].CONTENT_TYPE == "U")
                        {
                            if (System.IO.File.Exists(Path.Combine(contentsModels[i].THUMBNAIL_PATH, contentsModels[i].THUMBNAIL_NM)))
                                System.IO.File.Delete(Path.Combine(contentsModels[i].THUMBNAIL_PATH, contentsModels[i].THUMBNAIL_NM));
                        }

                        else if (contentsModels[i].CONTENT_TYPE != "T")
                        {
                            if (System.IO.File.Exists(Path.Combine(contentsModels[i].FILE_PATH, contentsModels[i].FILE_NM)))
                                System.IO.File.Delete(Path.Combine(contentsModels[i].FILE_PATH, contentsModels[i].FILE_NM));

                            if (System.IO.File.Exists(Path.Combine(contentsModels[i].THUMBNAIL_PATH, contentsModels[i].THUMBNAIL_NM)))
                                System.IO.File.Delete(Path.Combine(contentsModels[i].THUMBNAIL_PATH, contentsModels[i].THUMBNAIL_NM));
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                resultModel.ERR_CODE = "9999";
                resultModel.ERROR_MSG = ex.Message;
            }

            return JsonConvert.SerializeObject(resultModel, Formatting.Indented);
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