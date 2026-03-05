using System;
using System.Data;
using Npgsql;
using NpgsqlTypes;

namespace DebugConsole
{
    class Program
    {
        static string connectionString = "Host=210.219.229.46;Port=6543;Database=postgres;Username=dbuser;Password=dbuser1234!@#$;SearchPath=publicdata;";

        static void Main(string[] args)
        {
            Console.WriteLine("=== Npgsql Parameter Debug Console ===\n");

            // Test 1: CommandType.Text (직접 SQL 호출)
            Console.WriteLine("Test 1: CommandType.Text");
            TestWithCommandTypeText();

            Console.WriteLine("\n" + new string('=', 50) + "\n");

            // Test 2: CommandType.StoredProcedure (기본 방식)
            Console.WriteLine("Test 2: CommandType.StoredProcedure (기본)");
            TestWithStoredProcedure_Basic();

            Console.WriteLine("\n" + new string('=', 50) + "\n");

            // Test 3: CommandType.StoredProcedure (NpgsqlDbType 명시)
            Console.WriteLine("Test 3: CommandType.StoredProcedure (NpgsqlDbType 명시)");
            TestWithStoredProcedure_ExplicitType();

            Console.WriteLine("\n" + new string('=', 50) + "\n");

            // Test 4: CommandType.StoredProcedure (소문자 파라미터)
            Console.WriteLine("Test 4: CommandType.StoredProcedure (소문자 파라미터)");
            TestWithStoredProcedure_Lowercase();

            Console.WriteLine("\n" + new string('=', 50) + "\n");

            // Test 5: DisplayBiz.cs와 동일한 패턴 (변수 사용)
            Console.WriteLine("Test 5: CommandType.StoredProcedure (DisplayBiz 패턴)");
            TestWithStoredProcedure_DisplayBizPattern();

            Console.WriteLine("\n" + new string('=', 50) + "\n");

            // Test 6: 파라미터 정보 출력
            Console.WriteLine("Test 6: 함수 파라미터 정보 조회");
            GetFunctionParameters();

            Console.WriteLine("\n완료. 아무 키나 누르세요...");
            Console.ReadKey();
        }

        static void TestWithCommandTypeText()
        {
            try
            {
                using (var conn = new NpgsqlConnection(connectionString))
                {
                    conn.Open();
                    using (var cmd = new NpgsqlCommand())
                    {
                        cmd.Connection = conn;
                        cmd.CommandType = CommandType.Text;
                        cmd.CommandText = "SELECT * FROM publicdata.pr_display_list(@p1, @p2, @p3)";
                        cmd.Parameters.AddWithValue("@p1", "RST001");
                        cmd.Parameters.AddWithValue("@p2", 0);
                        cmd.Parameters.AddWithValue("@p3", "");

                        using (var reader = cmd.ExecuteReader())
                        {
                            int count = 0;
                            while (reader.Read())
                            {
                                count++;
                                Console.WriteLine($"  Row {count}: display_id={reader["display_id"]}, display_nm={reader["display_nm"]}");
                            }
                            Console.WriteLine($"  총 {count}개 행 반환");
                        }
                    }
                }
                Console.WriteLine("  [SUCCESS]");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"  [ERROR] {ex.GetType().Name}: {ex.Message}");
            }
        }

        static void TestWithStoredProcedure_Basic()
        {
            try
            {
                using (var conn = new NpgsqlConnection(connectionString))
                {
                    conn.Open();
                    using (var cmd = new NpgsqlCommand("publicdata.pr_display_list", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        // 기본 방식 - int 0을 object로 캐스팅해야 enum이 아닌 값으로 인식됨
                        cmd.Parameters.Add(new NpgsqlParameter("P_RESTAURANT_CODE", "RST001"));
                        cmd.Parameters.Add(new NpgsqlParameter("P_DISPLAY_ID", (object)0));
                        cmd.Parameters.Add(new NpgsqlParameter("P_DISPLAY_NM", ""));

                        Console.WriteLine("  파라미터 상태:");
                        foreach (NpgsqlParameter p in cmd.Parameters)
                        {
                            Console.WriteLine($"    {p.ParameterName}: Value={p.Value}, NpgsqlDbType={p.NpgsqlDbType}, IsNullable={p.IsNullable}");
                        }

                        using (var reader = cmd.ExecuteReader())
                        {
                            int count = 0;
                            while (reader.Read()) count++;
                            Console.WriteLine($"  총 {count}개 행 반환");
                        }
                    }
                }
                Console.WriteLine("  [SUCCESS]");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"  [ERROR] {ex.GetType().Name}: {ex.Message}");
            }
        }

        static void TestWithStoredProcedure_ExplicitType()
        {
            try
            {
                using (var conn = new NpgsqlConnection(connectionString))
                {
                    conn.Open();
                    using (var cmd = new NpgsqlCommand("publicdata.pr_display_list", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        // NpgsqlDbType 명시적 지정
                        var p1 = new NpgsqlParameter();
                        p1.ParameterName = "P_RESTAURANT_CODE";
                        p1.NpgsqlDbType = NpgsqlDbType.Varchar;
                        p1.Value = "RST001";
                        cmd.Parameters.Add(p1);

                        var p2 = new NpgsqlParameter();
                        p2.ParameterName = "P_DISPLAY_ID";
                        p2.NpgsqlDbType = NpgsqlDbType.Integer;
                        p2.Value = 0;
                        cmd.Parameters.Add(p2);

                        var p3 = new NpgsqlParameter();
                        p3.ParameterName = "P_DISPLAY_NM";
                        p3.NpgsqlDbType = NpgsqlDbType.Varchar;
                        p3.Value = "";
                        cmd.Parameters.Add(p3);

                        Console.WriteLine("  파라미터 상태:");
                        foreach (NpgsqlParameter p in cmd.Parameters)
                        {
                            Console.WriteLine($"    {p.ParameterName}: Value={p.Value}, NpgsqlDbType={p.NpgsqlDbType}, IsNullable={p.IsNullable}");
                        }

                        using (var reader = cmd.ExecuteReader())
                        {
                            int count = 0;
                            while (reader.Read()) count++;
                            Console.WriteLine($"  총 {count}개 행 반환");
                        }
                    }
                }
                Console.WriteLine("  [SUCCESS]");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"  [ERROR] {ex.GetType().Name}: {ex.Message}");
            }
        }

        static void TestWithStoredProcedure_Lowercase()
        {
            try
            {
                using (var conn = new NpgsqlConnection(connectionString))
                {
                    conn.Open();
                    using (var cmd = new NpgsqlCommand("publicdata.pr_display_list", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        // 소문자 파라미터명
                        var p1 = new NpgsqlParameter();
                        p1.ParameterName = "p_restaurant_code";
                        p1.NpgsqlDbType = NpgsqlDbType.Varchar;
                        p1.Value = "RST001";
                        cmd.Parameters.Add(p1);

                        var p2 = new NpgsqlParameter();
                        p2.ParameterName = "p_display_id";
                        p2.NpgsqlDbType = NpgsqlDbType.Integer;
                        p2.Value = 0;
                        cmd.Parameters.Add(p2);

                        var p3 = new NpgsqlParameter();
                        p3.ParameterName = "p_display_nm";
                        p3.NpgsqlDbType = NpgsqlDbType.Varchar;
                        p3.Value = "";
                        cmd.Parameters.Add(p3);

                        Console.WriteLine("  파라미터 상태:");
                        foreach (NpgsqlParameter p in cmd.Parameters)
                        {
                            Console.WriteLine($"    {p.ParameterName}: Value={p.Value}, NpgsqlDbType={p.NpgsqlDbType}, IsNullable={p.IsNullable}");
                        }

                        using (var reader = cmd.ExecuteReader())
                        {
                            int count = 0;
                            while (reader.Read()) count++;
                            Console.WriteLine($"  총 {count}개 행 반환");
                        }
                    }
                }
                Console.WriteLine("  [SUCCESS]");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"  [ERROR] {ex.GetType().Name}: {ex.Message}");
            }
        }

        static void TestWithStoredProcedure_DisplayBizPattern()
        {
            try
            {
                using (var conn = new NpgsqlConnection(connectionString))
                {
                    conn.Open();
                    using (var cmd = new NpgsqlCommand("publicdata.pr_display_list", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        // DisplayBiz.cs와 동일한 패턴
                        string restaurantCode = "RST001";
                        int displayId = 0;
                        string displayNm = null; // null 값 테스트

                        cmd.Parameters.Add(new NpgsqlParameter("P_RESTAURANT_CODE", restaurantCode));
                        cmd.Parameters.Add(new NpgsqlParameter("P_DISPLAY_ID", displayId));
                        cmd.Parameters.Add(new NpgsqlParameter("P_DISPLAY_NM", displayNm));

                        Console.WriteLine("  파라미터 상태:");
                        foreach (NpgsqlParameter p in cmd.Parameters)
                        {
                            Console.WriteLine($"    {p.ParameterName}: Value={p.Value ?? "NULL"}, NpgsqlDbType={p.NpgsqlDbType}, IsNullable={p.IsNullable}");
                        }

                        using (var reader = cmd.ExecuteReader())
                        {
                            int count = 0;
                            while (reader.Read()) count++;
                            Console.WriteLine($"  총 {count}개 행 반환");
                        }
                    }
                }
                Console.WriteLine("  [SUCCESS]");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"  [ERROR] {ex.GetType().Name}: {ex.Message}");
            }
        }

        static void GetFunctionParameters()
        {
            try
            {
                using (var conn = new NpgsqlConnection(connectionString))
                {
                    conn.Open();
                    string sql = @"
                        SELECT
                            p.proname AS function_name,
                            pg_get_function_arguments(p.oid) AS arguments,
                            pg_get_function_result(p.oid) AS return_type
                        FROM pg_proc p
                        JOIN pg_namespace n ON p.pronamespace = n.oid
                        WHERE n.nspname = 'publicdata'
                        AND p.proname = 'pr_display_list'";

                    using (var cmd = new NpgsqlCommand(sql, conn))
                    {
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Console.WriteLine($"  함수명: {reader["function_name"]}");
                                Console.WriteLine($"  인자: {reader["arguments"]}");
                                Console.WriteLine($"  반환: {reader["return_type"]}");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"  [ERROR] {ex.GetType().Name}: {ex.Message}");
            }
        }
    }
}
