// TemplateController.cs의 uploadFile 메서드와 SaveTemplate 메서드에서
// Region 03 부분을 아래 코드로 교체하세요

#region 03. 템플릿 파일 저장 (동일한 이름의 파일이 있을경우(수정일경우) 파일을 덮어씀)

// 루트 경로 셋팅
saveRootPath = Path.Combine(Server.MapPath("~/"), CommonProperties.TEMPLATE_SAVE_PATH(UserBiz.getRestaurantCode(User), templateId));
templateFileName = string.Format("T{0}.html", templateId);
templateCaptureFileName = string.Format("T{0}.jpg", templateId);
templateThumbnailFileName = string.Format("THUMBNAIL_T{0}.jpg", templateId);

if (!Directory.Exists(saveRootPath))
    Directory.CreateDirectory(saveRootPath);

// 이미지 디렉토리 생성
string imagesDirPath = Path.Combine(saveRootPath, "images");
if (!Directory.Exists(imagesDirPath))
    Directory.CreateDirectory(imagesDirPath);

// Base64 이미지를 파일로 저장하고 HTML에서 경로로 변경
System.Text.RegularExpressions.Regex base64Regex = new System.Text.RegularExpressions.Regex(@"<img\s+[^>]*src=""data:image/([^;]+);base64,([^""]+)""[^>]*>", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
int imageCounter = 0;

templateModel.TEMPLATE_HTML = base64Regex.Replace(templateModel.TEMPLATE_HTML, match =>
{
    try
    {
        string imageFormat = match.Groups[1].Value; // png, jpg, jpeg 등
        string base64Data = match.Groups[2].Value;
        
        // Base64 디코딩
        byte[] imageBytes = Convert.FromBase64String(base64Data);
        
        // 파일명 생성 (중복 방지를 위해 카운터 사용)
        string imageFileName = string.Format("menu_image_{0}.{1}", imageCounter++, imageFormat);
        string imageFilePath = Path.Combine(imagesDirPath, imageFileName);
        
        // 파일 저장
        System.IO.File.WriteAllBytes(imageFilePath, imageBytes);
        
        System.Diagnostics.Debug.WriteLine($"[Base64 Image Saved] {imageFileName}");
        
        // HTML에서 Base64를 파일 경로로 변경
        string imagePath = string.Format("{0}/upload/template/{1}/{2}/images/{3}", 
            CommonProperties.HTTP_DOMAIN_URL, 
            UserBiz.getRestaurantCode(User), 
            templateId, 
            imageFileName);
        
        return match.Value.Replace(match.Groups[0].Value, 
            match.Value.Replace(string.Format("data:image/{0};base64,{1}", imageFormat, base64Data), imagePath));
    }
    catch (Exception ex)
    {
        System.Diagnostics.Debug.WriteLine($"[Base64 Image Save Error] {ex.Message}");
        return match.Value; // 오류 시 원본 유지
    }
});

// 템플릿 URL 변경
templateModel.TEMPLATE_HTML = templateModel.TEMPLATE_HTML.Replace("{TEMPLATE_PATH}", string.Format("{0}/upload/template/{1}/{2}/images", CommonProperties.HTTP_DOMAIN_URL, UserBiz.getRestaurantCode(User), templateId));

// 템플릿(HTML) 파일 저장
System.IO.File.WriteAllText(Path.Combine(saveRootPath, templateFileName), templateModel.TEMPLATE_HTML, System.Text.Encoding.UTF8);

#endregion
