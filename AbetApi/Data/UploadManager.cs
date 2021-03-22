using AbetApi.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using static AbetApi.Models.AbetModels;

namespace AbetApi.Data
{
    public class UploadManager : IUploadManager
    {
        private string cs = @"Server=TEBA-D\ABETDATABASE;Database=abetdb11;Trusted_Connection=True";
        //@"Server=TRICO-SCHOOL\SQLEXPRESS;Database=abetdb;Trusted_Connection=True";    <-- Yafet Server
        //@"Server=DESKTOP-5BU0BPP;Database=abetdb;Trusted_Connection=True";            <-- Rafael Server
        //@"Server=LAPTOP-838TO9CN\SQLEXPRESS;Database=abetdb;Trusted_Connection=True"; <-- Emmanuelli's local DB
        //@"Server=TEBA-D\ABETDATABASE;Database=abetdb;Trusted_Connection=True          <-- Server for RemoteDesktop

        private SqlConnection GetConnection()
        {
            return new SqlConnection(cs);
        }

        public string FilePath { get; set; } = null;
        public string ErrorMessage { get; set; }

        //stores a received file in the Uploads folder
        public void StoreFile(IFormFile file, List<string> acceptableTypes)
        {
            try
            {
                //check if file type is acceptable
                if (!acceptableTypes.Contains(Path.GetExtension(file.FileName)))
                {
                    ErrorMessage = "Error: Unexpected file type.";
                    return;
                }

                if (file.Length > 0)
                {
                    string folderPath = Path.Combine(Directory.GetCurrentDirectory(), "Uploads");

                    //create Uploads folder if it does not exist
                    if (!Directory.Exists(folderPath))
                        Directory.CreateDirectory(folderPath);

                    FilePath = Path.Combine(folderPath, file.FileName);

                    using (FileStream stream = new FileStream(FilePath, FileMode.Create))
                    {
                        file.CopyTo(stream);
                        stream.Flush();
                    }
                }
                else
                {
                    ErrorMessage = "Error: File contains no data.";
                }
            }
            catch
            {
                ErrorMessage = "Internal Server Error: Please try again later.";
            }
        }

        public SqlReturn InsertAccess2SQLserver()
        {
            // unless specified, current year and semester will be calculated using current date
            int current_year = DateTime.Now.Year;
            string current_department = "csce";
            string current_semester;
            int month = DateTime.Now.Month;
            if (month > 1 && month < 5) current_semester = "spring";
            else if (month > 8 && month < 1) current_semester = "fall";
            else current_semester = "summer";
            
            //variable declaration
            int ce, cs, it, status;
            int i, j, result;
            string outcome;
            string student_outcome;
            string[] p = { "CE - ", "CS - ", "IT - " };   // add "CYBR - " later
            string[] programs = { "ce", "cs", "it" };     // add "cybr" later
            SqlReturn sqlReturn = new SqlReturn();
            sqlReturn.code = 1;

            // access database connection
            string dsn = @"Provider=Microsoft.ACE.OLEDB.12.0; Data Source = " + FilePath;
            string query = @"SELECT [Course Number] as course_number, [Name of Course] as course_name, Coordinator, 
            [Group CE] as group_ce, [Group CS] as group_cs, [Group IT] as group_it, 
            Outcome1, Outcome2, Outcome3, Outcome4, Outcome5, Outcome6, Outcome7, Outcome8, Outcome9,
            [CE - StudentOutcomes1], [CE - StudentOutcomes2], [CE - StudentOutcomes3], [CE - StudentOutcomes4], [CE - StudentOutcomes5], [CE - StudentOutcomes6], 
            [CE - StudentOutcomes7], [CE - StudentOutcomes8], [CE - StudentOutcomes9], [CE - StudentOutcomes10], [CE - StudentOutcomes11],  [CE - StudentOutcomes12], 
            [CE - StudentOutcomes13], [CE - StudentOutcomes14],
            [CS - StudentOutcomes1], [CS - StudentOutcomes2], [CS - StudentOutcomes3], [CS - StudentOutcomes4], [CS - StudentOutcomes5], [CS - StudentOutcomes6], 
            [CS - StudentOutcomes7], [CS - StudentOutcomes8], [CS - StudentOutcomes9], [CS - StudentOutcomes10], [CS - StudentOutcomes11],  [CS - StudentOutcomes12], 
            [CS - StudentOutcomes13], [CS - StudentOutcomes14],
            [IT - StudentOutcomes1], [IT - StudentOutcomes2], [IT - StudentOutcomes3], [IT - StudentOutcomes4], [IT - StudentOutcomes5], [IT - StudentOutcomes6], 
            [IT - StudentOutcomes7], [IT - StudentOutcomes8], [IT - StudentOutcomes9], [IT - StudentOutcomes10], [IT - StudentOutcomes11],  [IT - StudentOutcomes12], 
            [IT - StudentOutcomes13], [IT - StudentOutcomes14]
            FROM Sheet1";
            OleDbConnection conn = new OleDbConnection(dsn);
            OleDbCommand cmd = new OleDbCommand(query, conn);

            // sql server connection
            string query1 = @"insert into courses (year, semester, department, course_number, coordinator_name, 
display_name, group_ce, group_cs, group_it, status)
values 
(@year, @semester, @department, @course_number, @coordinator_name, @display_name, @group_ce, @group_cs, @group_it, @status); 
SELECT SCOPE_IDENTITY()";
            string query2 = @"insert into course_outcomes (num, course_outcome, course_id)
values (@num, @course_outcome, @course_id)";
            string query3 = @"insert into course_objectives (course_id, student_course_mapping, program, student_outcome_order) 
VALUES (@course_id, @mapping, @program, @order)";
            SqlConnection conn1 = GetConnection();
            SqlCommand cmd1;

            try
            {
                conn.Open();
                conn1.Open();
                using (OleDbDataReader rd = cmd.ExecuteReader())
                {
                    while (rd.Read())
                    {
                        status = 1;
                        ce = rd["group_ce"].ToString().Length == 0 ? 0 : 1;
                        cs = rd["group_cs"].ToString().Length == 0 ? 0 : 1;
                        it = rd["group_it"].ToString().Length == 0 ? 0 : 1;
                        if (rd["course_name"].ToString().Contains("*")) status = 0;

                        cmd1 = new SqlCommand(query1, conn1);
                        cmd1.Parameters.Add(new SqlParameter("@year", SqlDbType.Int)).Value = current_year;
                        cmd1.Parameters.Add(new SqlParameter("@semester", SqlDbType.VarChar, 11)).Value = current_semester;
                        cmd1.Parameters.Add(new SqlParameter("@department", SqlDbType.VarChar, 11)).Value = current_department;
                        cmd1.Parameters.Add(new SqlParameter("@course_number", SqlDbType.VarChar, 11)).Value = rd["course_number"].ToString();
                        cmd1.Parameters.Add(new SqlParameter("@coordinator_name", SqlDbType.VarChar, 50)).Value = rd["Coordinator"].ToString();
                        cmd1.Parameters.Add(new SqlParameter("@display_name", SqlDbType.VarChar, 100)).Value = rd["course_name"].ToString();
                        cmd1.Parameters.Add(new SqlParameter("@group_ce", SqlDbType.TinyInt)).Value = ce;
                        cmd1.Parameters.Add(new SqlParameter("@group_cs", SqlDbType.TinyInt)).Value = cs;
                        cmd1.Parameters.Add(new SqlParameter("@group_it", SqlDbType.TinyInt)).Value = it;
                        cmd1.Parameters.Add(new SqlParameter("@status", SqlDbType.TinyInt)).Value = status;
                        result = Convert.ToInt32(cmd1.ExecuteScalar());
                        Console.WriteLine(result);

                        for (i = 1; i <= 9; i++)
                        {
                            outcome = String.Concat("Outcome", i);
                            if (rd[outcome].ToString().Length != 0)
                            {
                                cmd1 = new SqlCommand(query2, conn1);
                                cmd1.Parameters.Add(new SqlParameter("@num", SqlDbType.Int)).Value = i;
                                cmd1.Parameters.Add(new SqlParameter("@course_outcome", SqlDbType.VarChar, -1)).Value = rd[outcome].ToString();
                                cmd1.Parameters.Add(new SqlParameter("@course_id", SqlDbType.Int)).Value = result;
                                cmd1.ExecuteScalar();
                            }
                        }

                        for (i = 0; i < 3; i++)
                        {
                            for (j = 1; j <= 14; j++)
                            {
                                student_outcome = p[i] + "StudentOutcomes" + j;
                                cmd1 = new SqlCommand(query3, conn1);
                                cmd1.Parameters.Add(new SqlParameter("@course_id", SqlDbType.Int)).Value = result;
                                cmd1.Parameters.Add(new SqlParameter("@mapping", SqlDbType.VarChar, 50)).Value = rd[student_outcome].ToString();
                                cmd1.Parameters.Add(new SqlParameter("@program", SqlDbType.VarChar, 10)).Value = programs[i];
                                cmd1.Parameters.Add(new SqlParameter("@order", SqlDbType.Int)).Value = j;
                                cmd1.ExecuteScalar();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                sqlReturn.message = ex.Message;
                sqlReturn.code = -1;
                conn.Close();
                conn1.Close();
                return sqlReturn;
            }


            return sqlReturn;
            throw new NotImplementedException();
        }
    }
}
