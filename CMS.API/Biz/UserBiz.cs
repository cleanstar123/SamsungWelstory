using System.Collections.Generic;
using System.Linq;
using System.Data;

using CMS.API.Models;
using CMS.API.App_Code;
using Oracle.ManagedDataAccess.Client;
using System.Security.Claims;
using System.Security.Principal;
using Microsoft.AspNet.Identity;

namespace CMS.API.Biz
{
    public class UserBiz
    {
        #region 로그인 정보 프로퍼티

        /// <summary>
        /// 현재 로그인한 사용자의 아이디를 리턴한다.
        /// </summary>
        /// <param name="User"></param>
        /// <returns></returns>
        public static string getUserId(IPrincipal User)
        {
            return User.Identity.GetUserId<string>();
        }

        /// <summary>
        /// 현재 로그인한 사용자의 이름을 리턴한다.
        /// </summary>
        /// <param name="User"></param>
        /// <returns></returns>
        public static string getUserNm(IPrincipal User)
        {
            return User.Identity.GetUserName();
        }

        /// <summary>
        /// 현재 로그인한 사용자의 권한을 리턴한다.
        /// </summary>
        /// <param name="User"></param>
        /// <returns></returns>
        public static string getUserRole(IPrincipal User)
        {
            List<string> roles = ((ClaimsIdentity)User.Identity).Claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value).ToList();

            return roles[0];
        }

        /// <summary>
        /// 현재 로그인한 사용자의 레스토랑 코드를 리턴한다.
        /// </summary>
        /// <param name="User"></param>
        /// <returns></returns>
        public static string getRestaurantCode(IPrincipal User)
        {
            var claim = ((ClaimsIdentity)User.Identity).Claims.FirstOrDefault(c => c.Type == "urn:Custom:RESTAURANT_CODE");
            return claim == null ? string.Empty : claim.Value;
        }

        /// <summary>
        /// 현재 로그인한 사용자의 레스토랑 이름을 리턴한다.
        /// </summary>
        /// <param name="User"></param>
        /// <returns></returns>
        public static string getRestaurantName(IPrincipal User)
        {
            var claim = ((ClaimsIdentity)User.Identity).Claims.FirstOrDefault(c => c.Type == "urn:Custom:RESTAURANT_NM");
            return claim == null ? string.Empty : claim.Value;
        }


        /// <summary>
        /// 비밀번호를 바꿔야 하는지 체크
        /// </summary>
        /// <param name="User"></param>
        /// <returns></returns>
        public static string getChangePassYN(IPrincipal User)
        {
            var claim = ((ClaimsIdentity)User.Identity).Claims.FirstOrDefault(c => c.Type == "urn:Custom:CHANGE_PASS_YN");
            return claim == null ? string.Empty : claim.Value;
        }

        #endregion

        /// <summary>
        /// 사용자 검색
        /// </summary>
        /// <param name="userModel"></param>
        /// <returns></returns>
        public static DataSet userList(UserModel userModel)
        {
            OracleParameter[] param = {
                                          new OracleParameter("P_RESTAURANT_CODE", userModel.RESTAURANT_CODE),
                                          new OracleParameter("P_GROUP_CD",        userModel.GROUP_CD),
                                          new OracleParameter("P_USER_NM",         userModel.USER_NM),
                                          new OracleParameter("CUR", OracleDbType.RefCursor)
                                      };

            param[param.Length - 1].Direction = ParameterDirection.Output;

            DataSet dsResult = OracleHelper.ExecuteDataset(CommonProperties.ConnectionString, CommandType.StoredProcedure, "DID.PKG_CMS_USER.PR_USER_LIST", param);

            dsResult.Tables[0].TableName = "rows";

            return dsResult;
        }

        /// <summary>
        /// 레스토랑 검색
        /// </summary>
        /// <param name="restaurant">레스토랑 코드</param>
        /// <returns></returns>
        public static DataSet getRestaurantList(string restaurant)
        {
            OracleParameter[] param = {
                                          new OracleParameter("P_RESTAURANT_NM", restaurant),
                                          new OracleParameter("CUR", OracleDbType.RefCursor)
                                      };

            param[param.Length - 1].Direction = ParameterDirection.Output;

            DataSet dsResult = OracleHelper.ExecuteDataset(CommonProperties.ConnectionString, CommandType.StoredProcedure, "DID.PKG_CMS_USER.PR_FIND_RESTAURANT", param);

            dsResult.Tables[0].TableName = "rows";

            return dsResult;
        }

        /// <summary>
        /// 사용자 검색 (로그인시 사용)
        /// </summary>
        /// <param name="id">아이디</param>
        /// <param name="password">비밀번호</param>
        /// <returns></returns>
        public static ResultModel user(string id, string password)
        {
            OracleParameter[] param = {
                                          new OracleParameter("P_USER_ID", id),
                                          new OracleParameter("P_USER_PW", password),
                                          new OracleParameter("CUR", OracleDbType.RefCursor)
                                      };

            param[param.Length - 1].Direction = ParameterDirection.Output;

            DataSet ds = OracleHelper.ExecuteDataset(CommonProperties.ConnectionString, CommandType.StoredProcedure, "DID.PKG_CMS_USER.PR_USER_LOGIN", param);

            ResultModel result = new ResultModel();

            if (Util.IsNullDataset(ds))
            {
                ds.Tables[0].TableName = "rows";
                result.ERR_CODE        = "0";
                result.DATA            = ds.Tables[0];
            }

            return result;
        }

        /// <summary>
        /// 사용사 생성/수정/삭제
        /// </summary>
        /// <param name="TYPE">I(생성), U(수정), D(삭제)</param>
        /// <param name="userId"></param>
        /// <param name="userModel"></param>
        /// <returns></returns>
        public static List<ResultModel> manageUser(string TYPE, string userId, UserModel userModel)
        {
            OracleParameter[] param = {
                                          new OracleParameter("P_TYPE",            TYPE),
                                          new OracleParameter("P_USER_ID",         userModel.USER_ID),
                                          new OracleParameter("P_RESTAURANT_CODE", userModel.RESTAURANT_CODE),
                                          new OracleParameter("P_RESTAURANT_NM",   userModel.RESTAURANT_NM),
                                          new OracleParameter("P_USER_NM",         userModel.USER_NM),
                                          new OracleParameter("P_GROUP_CD",        userModel.GROUP_CD),
                                          new OracleParameter("P_USER_PW",         userModel.USER_PW),
                                          new OracleParameter("P_USE_YN",          userModel.USE_YN),
                                          new OracleParameter("P_REG_ID",          userId),
                                          new OracleParameter("CUR", OracleDbType.RefCursor)
                                      };

            param[param.Length - 1].Direction = ParameterDirection.Output;

            return Util.ConvertDataTable<ResultModel>(OracleHelper.ExecuteDataset(CommonProperties.ConnectionString, CommandType.StoredProcedure, "DID.PKG_CMS_USER.PR_USER_MANAGE", param).Tables[0]);
        }
    }
}