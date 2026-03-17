using System;
using System.Linq;
using System.Web.Http;
using CMS.API.App_Code;
using Npgsql;
using NpgsqlTypes;
using System.Data;

namespace CMS.API.Controllers
{
    /// <summary>
    /// DID Client용 API 컨트롤러
    /// </summary>
    [RoutePrefix("api/v1")]
    public class ClientApiController : ApiController
    {
        /// <summary>
        /// 클라이언트 스케줄 조회
        /// POST /api/v1/schedule/SelectSchedule
        /// </summary>
        [HttpPost]
        [Route("schedule/SelectSchedule")]
        public IHttpActionResult SelectSchedule([FromBody] ScheduleRequest request)
        {
            try
            {
                // 헤더에서 인증 정보 추출
                var accessToken = Request.Headers.Contains("accessToken") 
                    ? Request.Headers.GetValues("accessToken").FirstOrDefault() 
                    : null;
                var accessDispId = Request.Headers.Contains("accessDispId") 
                    ? Request.Headers.GetValues("accessDispId").FirstOrDefault() 
                    : null;

                if (string.IsNullOrEmpty(accessToken) || string.IsNullOrEmpty(accessDispId))
                {
                    return Ok(new
                    {
                        header = new { returnCode = "9999", returnMessage = "인증 정보가 없습니다." },
                        body = new { actionType = "00", scheduleId = "0", templateId = "" }
                    });
                }

                // 현재 시간에 맞는 스케줄 조회 (프로시저 사용)
                string sql = "SELECT * FROM did.pr_schedule_client_select(@p_restaurant_code, @p_display_id)";

                NpgsqlParameter[] parameters = {
                    new NpgsqlParameter("p_restaurant_code", NpgsqlDbType.Varchar) { Value = accessToken },
                    new NpgsqlParameter("p_display_id", NpgsqlDbType.Integer) { Value = int.Parse(accessDispId) }
                };

                DataSet ds = PostgresHelper.ExecuteDataSet(
                    CommonProperties.ConnectionString, 
                    CommandType.Text, 
                    sql, 
                    parameters
                );

                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    var row = ds.Tables[0].Rows[0];
                    string scheduleId = row["SCHEDULE_ID"].ToString();
                    string templateUrl = row["TEMPLATE_URL"].ToString();
                    string modDtm = row["MOD_DTM"].ToString();

                    // 이전 스케줄과 비교
                    string actionType = "11"; // 신규 또는 업데이트
                    if (request != null && request.scheduleId == scheduleId)
                    {
                        actionType = "00"; // 변경 없음
                    }

                    return Ok(new
                    {
                        header = new { returnCode = "0000", returnMessage = "성공" },
                        body = new
                        {
                            actionType = actionType,
                            scheduleId = scheduleId,
                            templateId = templateUrl,
                            modDtm = modDtm
                        }
                    });
                }
                else
                {
                    // 스케줄 없음 - 화면 OFF
                    return Ok(new
                    {
                        header = new { returnCode = "0000", returnMessage = "스케줄 없음" },
                        body = new
                        {
                            actionType = "99", // 화면 OFF
                            scheduleId = "0",
                            templateId = ""
                        }
                    });
                }
            }
            catch (Exception ex)
            {
                return Ok(new
                {
                    header = new { returnCode = "9999", returnMessage = ex.Message },
                    body = new { actionType = "00", scheduleId = "0", templateId = "" }
                });
            }
        }

        /// <summary>
        /// 클라이언트 스케줄 업데이트 확인
        /// POST /api/v1/schedule/UpdateSchedule
        /// </summary>
        [HttpPost]
        [Route("schedule/UpdateSchedule")]
        public IHttpActionResult UpdateSchedule([FromBody] UpdateScheduleRequest request)
        {
            try
            {
                // 단순히 성공 응답 반환 (로깅 목적)
                return Ok(new
                {
                    header = new { returnCode = "0000", returnMessage = "성공" }
                });
            }
            catch (Exception ex)
            {
                return Ok(new
                {
                    header = new { returnCode = "9999", returnMessage = ex.Message }
                });
            }
        }
    }

    /// <summary>
    /// 스케줄 조회 요청 모델
    /// </summary>
    public class ScheduleRequest
    {
        public string scheduleId { get; set; }
    }

    /// <summary>
    /// 스케줄 업데이트 요청 모델
    /// </summary>
    public class UpdateScheduleRequest
    {
        public string BeforeScheduleId { get; set; }
        public string scheduleId { get; set; }
    }
}
