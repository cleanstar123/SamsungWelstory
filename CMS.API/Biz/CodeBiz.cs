using System.Collections.Generic;
using System.Data;

using CMS.API.Models;
using CMS.API.App_Code;
using Oracle.ManagedDataAccess.Client;

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
            OracleParameter[] param = {
                                          new OracleParameter("P_CODE_GROUP",    pCodeGroup),
                                          new OracleParameter("P_CODE_GROUP_NM", pCodeGroupNm),
                                          new OracleParameter("CUR",             OracleDbType.RefCursor)
                                      };

            param[param.Length - 1].Direction = ParameterDirection.Output;

            return OracleHelper.ExecuteDataset(CommonProperties.ConnectionString, CommandType.StoredProcedure, "DID.PKG_CMS_CODE.PR_CODE_GROUP_LIST", param);
        }

        /// <summary>
        /// 코드 조회
        /// </summary>
        /// <param name="pCodeGroup">그룹 코드</param>
        /// <returns></returns>
        public static DataSet getCodes(string pCodeGroup)
        {
            OracleParameter[] param = {
                                          new OracleParameter("P_CODE_GROUP", pCodeGroup),
                                          new OracleParameter("CUR",          OracleDbType.RefCursor)
                                      };

            param[param.Length - 1].Direction = ParameterDirection.Output;

            return OracleHelper.ExecuteDataset(CommonProperties.ConnectionString, CommandType.StoredProcedure, "DID.PKG_CMS_CODE.PR_CODE_LIST", param);
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

            OracleParameter[] param = {
                                          new OracleParameter("P_TYPE",             type),
                                          new OracleParameter("P_CODE_GROUP",       model.CODE_GROUP),
                                          new OracleParameter("P_CODE_GROUP_NM",    model.CODE_GROUP_NM),
                                          new OracleParameter("P_DISPLAY_SEQ",      model.DISPLAY_SEQ),
                                          new OracleParameter("P_CODE_GROUP_DESC",  model.CODE_GROUP_DESC),
                                          new OracleParameter("P_CODE_GROUP_ATTR1", model.CODE_GROUP_ATTR1),
                                          new OracleParameter("P_CODE_GROUP_ATTR2", model.CODE_GROUP_ATTR2),
                                          new OracleParameter("P_CODE_GROUP_ATTR3", model.CODE_GROUP_ATTR3),
                                          new OracleParameter("P_REG_ID",           userId),
                                          new OracleParameter("P_XML_REQ",          type == "D" ? JsonHelper.GetJsonToXmlString<CodeGroupModel>(codeGroupModelList) : null),
                                          new OracleParameter("CUR", OracleDbType.RefCursor)
                                      };
            param[param.Length - 1].Direction = ParameterDirection.Output;

            return Util.ConvertDataTable<ResultModel>(OracleHelper.ExecuteDataset(CommonProperties.ConnectionString, CommandType.StoredProcedure, "DID.PKG_CMS_CODE.PR_CODE_GROUP_MANAGE", param).Tables[0])[0];
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

            OracleParameter[] param = {
                                          new OracleParameter("P_TYPE",        type),
                                          new OracleParameter("P_CODE_GROUP",  model.CODE_GROUP),
                                          new OracleParameter("P_CODE",        model.CODE),
                                          new OracleParameter("P_CODE_NAME",   model.CODE_NAME),
                                          new OracleParameter("P_DISPLAY_SEQ", model.DISPLAY_SEQ),
                                          new OracleParameter("P_CODE_DESC",   model.CODE_DESC),
                                          new OracleParameter("P_CODE_ATTR1",  model.CODE_ATTR1),
                                          new OracleParameter("P_CODE_ATTR2",  model.CODE_ATTR2),
                                          new OracleParameter("P_CODE_ATTR3",  model.CODE_ATTR3),
                                          new OracleParameter("P_REG_ID",      userId),
                                          new OracleParameter("P_XML_REQ",     type == "D" ? JsonHelper.GetJsonToXmlString(codeModelList) : null),
                                          new OracleParameter("CUR", OracleDbType.RefCursor)
                                      };
            param[param.Length - 1].Direction = ParameterDirection.Output;

            return Util.ConvertDataTable<ResultModel>(OracleHelper.ExecuteDataset(CommonProperties.ConnectionString, CommandType.StoredProcedure, "DID.PKG_CMS_CODE.PR_CODE_MANAGE", param).Tables[0])[0];
        }
    }
}