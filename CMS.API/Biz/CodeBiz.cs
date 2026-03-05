using System.Collections.Generic;
using System.Data;

using CMS.API.Models;
using CMS.API.App_Code;
using Npgsql;
using NpgsqlTypes;

namespace CMS.API.Biz
{
    public class CodeBiz
    {
        /// <summary>
        /// 그룹코드 조회
        /// </summary>
        /// <param name="pCodeGroup">그룹코드</param>
        /// <param name="pCodeGroupNm">그룹코드명</param>
        /// <returns></returns>
        public static DataSet getGroupCodes(string pCodeGroup, string pCodeGroupNm)
        {
            NpgsqlParameter[] param = {
                                          new NpgsqlParameter("P_CODE_GROUP",    pCodeGroup),
                                          new NpgsqlParameter("P_CODE_GROUP_NM", pCodeGroupNm),
                                          new NpgsqlParameter("CUR",             NpgsqlDbType.Refcursor)
                                      };

            param[param.Length - 1].Direction = ParameterDirection.Output;

            return PostgresHelper.ExecuteDataSet(CommonProperties.ConnectionString, CommandType.StoredProcedure, "DID.PKG_CMS_CODE.PR_CODE_GROUP_LIST", param);
        }

        /// <summary>
        /// 코드 조회
        /// </summary>
        /// <param name="pCodeGroup">그룹 코드</param>
        /// <returns></returns>
        public static DataSet getCodes(string pCodeGroup)
        {
            NpgsqlParameter[] param = {
                                          new NpgsqlParameter("P_CODE_GROUP", pCodeGroup),
                                          new NpgsqlParameter("CUR",          NpgsqlDbType.Refcursor)
                                      };

            param[param.Length - 1].Direction = ParameterDirection.Output;

            return PostgresHelper.ExecuteDataSet(CommonProperties.ConnectionString, CommandType.StoredProcedure, "DID.PKG_CMS_CODE.PR_CODE_LIST", param);
        }

        /// <summary>
        /// 그룹코드 생성, 수정, 삭제 메서드
        /// </summary>
        /// <param name="TYPE">I : INSERT, U : UPDATE, D : DELETE</param>
        /// <param name="userId">사용자 아이디</param>
        /// <param name="codeModelList">대상 그룹코드 목록</param>
        /// <returns></returns>
        public static ResultModel codeGroupManage(string type, string userId, List<CodeGroupModel> codeGroupModelList)
        {
            CodeGroupModel model = (type.ToUpper() != "D" ? codeGroupModelList[0] : new CodeGroupModel());

            NpgsqlParameter[] param = {
                                          new NpgsqlParameter("P_TYPE",             type),
                                          new NpgsqlParameter("P_CODE_GROUP",       model.CODE_GROUP),
                                          new NpgsqlParameter("P_CODE_GROUP_NM",    model.CODE_GROUP_NM),
                                          new NpgsqlParameter("P_DISPLAY_SEQ",      model.DISPLAY_SEQ),
                                          new NpgsqlParameter("P_CODE_GROUP_DESC",  model.CODE_GROUP_DESC),
                                          new NpgsqlParameter("P_CODE_GROUP_ATTR1", model.CODE_GROUP_ATTR1),
                                          new NpgsqlParameter("P_CODE_GROUP_ATTR2", model.CODE_GROUP_ATTR2),
                                          new NpgsqlParameter("P_CODE_GROUP_ATTR3", model.CODE_GROUP_ATTR3),
                                          new NpgsqlParameter("P_REG_ID",           userId),
                                          new NpgsqlParameter("P_XML_REQ",          type == "D" ? JsonHelper.GetJsonToXmlString<CodeGroupModel>(codeGroupModelList) : null),
                                          new NpgsqlParameter("CUR", NpgsqlDbType.Refcursor)
                                      };
            param[param.Length - 1].Direction = ParameterDirection.Output;

            return Util.ConvertDataTable<ResultModel>(PostgresHelper.ExecuteDataSet(CommonProperties.ConnectionString, CommandType.StoredProcedure, "DID.PKG_CMS_CODE.PR_CODE_GROUP_MANAGE", param).Tables[0])[0];
        }

        /// <summary>
        /// 코드 생성, 수정, 삭제 메서드
        /// </summary>
        /// <param name="TYPE">I : INSERT, U : UPDATE, D : DELETE</param>
        /// <param name="userId">사용자 아이디</param>
        /// <param name="codeModelList">대상 코드 목록</param>
        /// <returns></returns>
        public static ResultModel codeManage(string type, string userId, List<CodeModel> codeModelList)
        {
            CodeModel model = (type.ToUpper() != "D" ? codeModelList[0] : new CodeModel());

            NpgsqlParameter[] param = {
                                          new NpgsqlParameter("P_TYPE",        type),
                                          new NpgsqlParameter("P_CODE_GROUP",  model.CODE_GROUP),
                                          new NpgsqlParameter("P_CODE",        model.CODE),
                                          new NpgsqlParameter("P_CODE_NAME",   model.CODE_NAME),
                                          new NpgsqlParameter("P_DISPLAY_SEQ", model.DISPLAY_SEQ),
                                          new NpgsqlParameter("P_CODE_DESC",   model.CODE_DESC),
                                          new NpgsqlParameter("P_CODE_ATTR1",  model.CODE_ATTR1),
                                          new NpgsqlParameter("P_CODE_ATTR2",  model.CODE_ATTR2),
                                          new NpgsqlParameter("P_CODE_ATTR3",  model.CODE_ATTR3),
                                          new NpgsqlParameter("P_REG_ID",      userId),
                                          new NpgsqlParameter("P_XML_REQ",     type == "D" ? JsonHelper.GetJsonToXmlString(codeModelList) : null),
                                          new NpgsqlParameter("CUR", NpgsqlDbType.Refcursor)
                                      };
            param[param.Length - 1].Direction = ParameterDirection.Output;

            return Util.ConvertDataTable<ResultModel>(PostgresHelper.ExecuteDataSet(CommonProperties.ConnectionString, CommandType.StoredProcedure, "DID.PKG_CMS_CODE.PR_CODE_MANAGE", param).Tables[0])[0];
        }
    }
}