using System.Collections.Generic;
using System.Linq;
using System.Data;
using System;

using CMS.API.Models;
using CMS.API.App_Code;
using Npgsql;
using NpgsqlTypes;
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
        public static string getUserId(IPrincipal User)
        {
            return User.Identity.GetUserId<string>();
        }

        /// <summary>
        /// 현재 로그인한 사용자의 이름을 리턴한다.
        /// </summary>
        public static string getUserNm(IPrincipal User)
        {
            return User.Identity.GetUserName();
        }

        /// <summary>
        /// 현재 로그인한 사용자의 권한을 리턴한다.
        /// </summary>
        public static string getUserRole(IPrincipal User)
        {
            List<string> roles = ((ClaimsIdentity)User.Identity).Claims
                .Where(c => c.Type == ClaimTypes.Role)
                .Select(c => c.Value).ToList();
            return roles[0];
        }

        /// <summary>
        /// 현재 로그인한 사용자의 레스토랑 코드를 리턴한다.
        /// </summary>
        public static string getRestaurantCode(IPrincipal User)
        {
            var claim = ((ClaimsIdentity)User.Identity).Claims.FirstOrDefault(c => c.Type == "urn:Custom:RESTAURANT_CODE");
            return claim == null ? string.Empty : claim.Value;
        }

        /// <summary>
        /// 현재 로그인한 사용자의 레스토랑 이름을 리턴한다.
        /// </summary>
        public static string getRestaurantName(IPrincipal User)
        {
            var claim = ((ClaimsIdentity)User.Identity).Claims.FirstOrDefault(c => c.Type == "urn:Custom:RESTAURANT_NM");
            return claim == null ? string.Empty : claim.Value;
        }

        /// <summary>
        /// 비밀번호를 바꿔야 하는지 체크
        /// </summary>
        public static string getChangePassYN(IPrincipal User)
        {
            var claim = ((ClaimsIdentity)User.Identity).Claims.FirstOrDefault(c => c.Type == "urn:Custom:CHANGE_PASS_YN");
            return claim == null ? string.Empty : claim.Value;
        }

        #endregion

        /// <summary>
        /// 사용자 검색
        /// </summary>
        public static DataSet userList(UserModel userModel)
        {
            string sql = "SELECT * FROM did.pr_user_list(@p_restaurant_code, @p_group_cd, @p_user_nm, @p_user_group_code, @p_user_group_nm)";

            NpgsqlParameter[] param = {
                new NpgsqlParameter("p_restaurant_code", NpgsqlDbType.Varchar) { Value = userModel.RESTAURANT_CODE ?? (object)DBNull.Value },
                new NpgsqlParameter("p_group_cd", NpgsqlDbType.Varchar) { Value = userModel.GROUP_CD ?? (object)DBNull.Value },
                new NpgsqlParameter("p_user_nm", NpgsqlDbType.Varchar) { Value = userModel.USER_NM ?? (object)DBNull.Value },
                new NpgsqlParameter("p_user_group_code", NpgsqlDbType.Varchar) { Value = userModel.USER_GROUP_CODE ?? (object)DBNull.Value },
                new NpgsqlParameter("p_user_group_nm", NpgsqlDbType.Varchar) { Value = userModel.USER_GROUP_NM ?? (object)DBNull.Value }
            };

            DataSet dsResult = PostgresHelper.ExecuteDataSet(CommonProperties.ConnectionString, CommandType.Text, sql, param);
            dsResult.Tables[0].TableName = "rows";
            return dsResult;
        }

        /// <summary>
        /// 레스토랑 검색
        /// </summary>
        public static DataSet getRestaurantList(string restaurant)
        {
            string sql = "SELECT * FROM did.pr_find_restaurant(@p_user_group_code)";

            int? userGroupCode = null;
            if (!string.IsNullOrEmpty(restaurant) && int.TryParse(restaurant, out int parsedValue))
                userGroupCode = parsedValue;

            NpgsqlParameter[] param = {
                new NpgsqlParameter("p_user_group_code", NpgsqlDbType.Integer) { Value = userGroupCode ?? (object)DBNull.Value }
            };

            DataSet dsResult = PostgresHelper.ExecuteDataSet(CommonProperties.ConnectionString, CommandType.Text, sql, param);
            dsResult.Tables[0].TableName = "rows";
            return dsResult;
        }

        /// <summary>
        /// 사용자 검색 (로그인시 사용)
        /// </summary>
        public static ResultModel user(string id, string password)
        {
            NpgsqlParameter[] param = {
                new NpgsqlParameter("P_USER_ID", id),
                new NpgsqlParameter("P_USER_PW", password)
            };

            string sql = "SELECT * FROM did.pr_user_login(@P_USER_ID, @P_USER_PW)";
            DataSet ds = PostgresHelper.ExecuteDataSet(CommonProperties.ConnectionString, CommandType.Text, sql, param);

            ResultModel result = new ResultModel();
            if (Util.IsNullDataset(ds))
            {
                ds.Tables[0].TableName = "rows";
                result.ERR_CODE = "0";
                result.DATA = ds.Tables[0];
            }
            return result;
        }

        /// <summary>
        /// 사용자 생성/수정/삭제
        /// </summary>
        public static List<ResultModel> manageUser(string TYPE, string userId, UserModel userModel)
        {
            string sql = @"SELECT * FROM did.pr_user_manage(
                @p_type, @p_user_id, @p_restaurant_code, @p_restaurant_nm, 
                @p_user_nm, @p_group_cd, @p_user_pw, @p_use_yn, @p_reg_id, 
                @p_user_group_code, @p_user_group_nm
            )";

            NpgsqlParameter[] param = {
                new NpgsqlParameter("p_type", NpgsqlDbType.Varchar) { Value = TYPE ?? (object)DBNull.Value },
                new NpgsqlParameter("p_user_id", NpgsqlDbType.Varchar) { Value = userModel.USER_ID ?? (object)DBNull.Value },
                new NpgsqlParameter("p_restaurant_code", NpgsqlDbType.Varchar) { Value = userModel.RESTAURANT_CODE ?? (object)DBNull.Value },
                new NpgsqlParameter("p_restaurant_nm", NpgsqlDbType.Varchar) { Value = userModel.RESTAURANT_NM ?? (object)DBNull.Value },
                new NpgsqlParameter("p_user_nm", NpgsqlDbType.Varchar) { Value = userModel.USER_NM ?? (object)DBNull.Value },
                new NpgsqlParameter("p_group_cd", NpgsqlDbType.Varchar) { Value = userModel.GROUP_CD ?? (object)DBNull.Value },
                new NpgsqlParameter("p_user_pw", NpgsqlDbType.Varchar) { Value = userModel.USER_PW ?? (object)DBNull.Value },
                new NpgsqlParameter("p_use_yn", NpgsqlDbType.Varchar) { Value = userModel.USE_YN ?? (object)DBNull.Value },
                new NpgsqlParameter("p_reg_id", NpgsqlDbType.Varchar) { Value = userId ?? (object)DBNull.Value },
                new NpgsqlParameter("p_user_group_code", NpgsqlDbType.Varchar) { Value = userModel.USER_GROUP_CODE ?? (object)DBNull.Value },
                new NpgsqlParameter("p_user_group_nm", NpgsqlDbType.Varchar) { Value = userModel.USER_GROUP_NM ?? (object)DBNull.Value }
            };

            return Util.ConvertDataTable<ResultModel>(PostgresHelper.ExecuteDataSet(CommonProperties.ConnectionString, CommandType.Text, sql, param).Tables[0]);
        }
    }
}
