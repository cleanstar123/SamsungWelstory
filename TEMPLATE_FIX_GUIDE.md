# 템플릿 수정 화면 문제 해결 가이드

## 문제 요약
템플릿 수정 화면에서 저장된 템플릿 HTML이 아닌 원본 레이아웃 HTML이 표시되는 문제

## 원인 분석
1. `pr_template_manage` 프로시저에 `I` (Insert) 타입이 없음
2. 템플릿 생성 시 `FILE_PATH`와 `FILE_NM`이 NULL로 저장될 가능성
3. 템플릿 수정 시 `FILE_PATH`와 `FILE_NM`을 조회할 수 없음

## 해결 단계

### 1단계: 데이터베이스 프로시저 수정
```sql
-- fix_pr_template_manage.sql 실행
-- pr_template_manage 프로시저에 I (Insert) 타입 추가
```

**실행 방법:**
```bash
psql -h [host] -U [user] -d [database] -f fix_pr_template_manage.sql
```

### 2단계: 기존 템플릿 데이터 확인
```sql
-- verify_template_fix.sql 실행
-- FILE_PATH와 FILE_NM이 NULL인 템플릿 확인
```

### 3단계: NULL 데이터 수정 (필요시)
기존에 생성된 템플릿 중 FILE_PATH와 FILE_NM이 NULL인 경우 수동으로 업데이트:

```sql
-- 예시: template_id = 1인 템플릿의 파일 경로 업데이트
UPDATE publicdata.cms_template
SET 
    file_path = 'C:\inetpub\wwwroot\upload\template\[restaurant_code]\1',
    file_nm = 'T1.html',
    thumbnail_nm = 'https://[domain]/upload/template/[restaurant_code]/1/THUMBNAIL_T1.jpg',
    template_url = 'https://[domain]/upload/template/[restaurant_code]/1/T1.html'
WHERE restaurant_code = '[restaurant_code]'
  AND template_id = 1;
```

### 4단계: 새 템플릿 생성 테스트
1. CMS 웹 페이지에서 레이아웃 선택
2. 템플릿 생성 화면으로 이동
3. 컨텐츠 연결 후 템플릿 저장
4. 템플릿 목록에서 방금 생성한 템플릿 확인
5. 템플릿 수정 버튼 클릭
6. 저장된 템플릿 HTML이 제대로 표시되는지 확인

### 5단계: 문제 지속 시 추가 확인사항

#### A. 파일 시스템 확인
템플릿 HTML 파일이 실제로 저장되었는지 확인:
```
C:\inetpub\wwwroot\upload\template\[restaurant_code]\[template_id]\T[template_id].html
```

#### B. 로그 확인
`TemplateController.uploadFile` 메서드의 디버그 로그 확인:
- LAYOUT_ID 값
- TEMPLATE_NM 값
- FILE_PATH 값
- FILE_NM 값

#### C. ManageTemplate.cshtml 확인
라인 106-113에서 파일 경로가 올바른지 확인:
```csharp
@if (ViewBag.type == "U")
{
    { this.GetOutputWriter().Write(File.ReadAllText(Path.Combine(Model.FILE_PATH, Model.FILE_NM))); }
}
```

## 예상 결과
- 템플릿 생성 시 `cms_template` 테이블에 `FILE_PATH`와 `FILE_NM`이 정상적으로 저장됨
- 템플릿 수정 화면에서 저장된 템플릿 HTML이 표시됨
- 사용자가 추가한 이미지와 텍스트가 그대로 유지됨

## 추가 참고사항

### 템플릿 저장 흐름
1. 사용자가 템플릿 저장 버튼 클릭
2. JavaScript에서 `manageTemplate.saveTemplate()` 호출
3. `TemplateController.uploadFile` 메서드 실행
   - `ManageTemplateAll("I", ...)` → 템플릿 레코드 생성 (template_id 반환)
   - 파일 시스템에 HTML 파일 저장
   - `ManageTemplate("U", ...)` → FILE_PATH, FILE_NM 업데이트
4. 템플릿 목록 페이지로 리다이렉트

### 템플릿 수정 화면 로드 흐름
1. 사용자가 템플릿 목록에서 템플릿 클릭
2. `TemplateController.ManageTemplate(type="U", id=template_id)` 실행
3. `TemplateBiz.getTemplateList()` → `pr_template_list` 호출
4. `Model.FILE_PATH`와 `Model.FILE_NM` 확인
5. 파일 존재 여부 확인
6. `ManageTemplate.cshtml` 렌더링
7. 저장된 템플릿 HTML 표시
