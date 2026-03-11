using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;

using CMS.API.Models;
using CMS.API.App_Code;
using Npgsql;
using NpgsqlTypes;


namespace CMS.API.Biz
{
    public class WelmenuBiz
    {

        public static List<WelmenuModel> welmenuList(WelmenuModel welmenuModel)
        {
            string sql = @"SELECT * FROM publicdata.pr_welmenu_list(@p_restaurant_code, @p_menu_dt, @p_menu_meal_type)";

            NpgsqlParameter[] param = {
                new NpgsqlParameter("@p_restaurant_code", welmenuModel.RESTAURANT_CODE ?? (object)DBNull.Value),
                new NpgsqlParameter("@p_menu_dt", welmenuModel.MENU_DT ?? (object)DBNull.Value),
                new NpgsqlParameter("@p_menu_meal_type", welmenuModel.MEAL_TYPE ?? (object)DBNull.Value)
            };

            try
            {
                return Util.ConvertDataTable<WelmenuModel>(
                    PostgresHelper.ExecuteDataSet(
                        CommonProperties.ConnectionString, 
                        CommandType.Text, 
                        sql, 
                        param
                    ).Tables[0]
                );
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"welmenuList 오류: {ex.Message}");
                return new List<WelmenuModel>();
            }
        }

        public static DataSet getSatisfactionlist(List<WelmenuModel> welmenuModels)
        {
            NpgsqlParameter[] param = {
                new NpgsqlParameter("p_xml_req", NpgsqlDbType.Text) { Value = JsonHelper.GetJsonToXmlString<WelmenuModel>(welmenuModels) ?? (object)DBNull.Value }
            };

            string sql = "SELECT * FROM publicdata.pr_welmenu_eval_result(@p_xml_req)";
            return PostgresHelper.ExecuteDataSet(CommonProperties.ConnectionString, CommandType.Text, sql, param);
        }

        public static DataSet getSatisfactionlist2(List<WelmenuModel> welmenuModels)
        {
            NpgsqlParameter[] param = {
                new NpgsqlParameter("p_xml_req", NpgsqlDbType.Text) { Value = JsonHelper.GetJsonToXmlString<WelmenuModel>(welmenuModels) ?? (object)DBNull.Value }
            };

            string sql = "SELECT * FROM publicdata.pr_welmenu_eval_result_view(@p_xml_req)";
            return PostgresHelper.ExecuteDataSet(CommonProperties.ConnectionString, CommandType.Text, sql, param);
        }

        public static ResultModel setSatisfaction(WelmenuModel welmenuModel)
        {
            NpgsqlParameter[] param = {
                new NpgsqlParameter("p_restaurant_code", NpgsqlDbType.Varchar) { Value = welmenuModel.RESTAURANT_CODE ?? (object)DBNull.Value },
                new NpgsqlParameter("p_hall_no", NpgsqlDbType.Varchar) { Value = welmenuModel.HALL_NO ?? (object)DBNull.Value },
                new NpgsqlParameter("p_menu_dt", NpgsqlDbType.Varchar) { Value = welmenuModel.MENU_DT ?? (object)DBNull.Value },
                new NpgsqlParameter("p_meal_type", NpgsqlDbType.Varchar) { Value = welmenuModel.MEAL_TYPE ?? (object)DBNull.Value },
                new NpgsqlParameter("p_course_type", NpgsqlDbType.Varchar) { Value = welmenuModel.COURSE_TYPE ?? (object)DBNull.Value },
                new NpgsqlParameter("p_menu_code", NpgsqlDbType.Varchar) { Value = welmenuModel.MENU_CODE ?? (object)DBNull.Value },
                new NpgsqlParameter("p_eval_score", NpgsqlDbType.Integer) { Value = string.IsNullOrEmpty(welmenuModel.EVAL_SCORE) ? (object)DBNull.Value : int.Parse(welmenuModel.EVAL_SCORE) },
                new NpgsqlParameter("p_reg_id", NpgsqlDbType.Varchar) { Value = "DID" }
            };

            string sql = "SELECT * FROM publicdata.pr_welmenu_eval_in_kiosk(@p_restaurant_code, @p_hall_no, @p_menu_dt, @p_meal_type, @p_course_type, @p_menu_code, @p_eval_score, @p_reg_id)";
            DataSet ds = PostgresHelper.ExecuteDataSet(CommonProperties.ConnectionString, CommandType.Text, sql, param);

            ResultModel result = new ResultModel();

            if (Util.IsNullDataset(ds))
            {
                result.ERR_CODE = ds.Tables[0].Rows[0]["ERR_CODE"].ToString();
                result.ERROR_MSG = ds.Tables[0].Rows[0]["ERROR_MSG"].ToString();
            }

            return result;
        }

    }
}