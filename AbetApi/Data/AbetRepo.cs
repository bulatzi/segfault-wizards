using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data;
using static AbetApi.Models.AbetModels;

namespace AbetApi.Data
{
    public class AbetRepo : IAbetRepo
    {
        private string cs =
            @"Server=TRICO-SCHOOL\SQLEXPRESS;Database=abetdb;Trusted_Connection=True;User=abet_software;Password=RQmu>KBM(PC]$r9vM>s3=%nb(8F:PnR}";
        // on VM, server=TEBA-D\ABETDATABASE
        public AbetRepo()
        {

        }

        private SqlConnection GetConnection()
        {
            return new SqlConnection(cs);
        }

        public string GetRole(string userId)
        {
            string myString = "";
            //string euid = "DK2121";
            string query =
                @"select role_name from staff as s join roles as r on s.role = r.id where s.euid = @userId";

            SqlConnection conn = GetConnection();
            conn.Open();
            SqlCommand cmd = new SqlCommand(query, conn);
            cmd.Parameters.Add(new SqlParameter("@userId", SqlDbType.VarChar)).Value = userId;

            using SqlDataReader rd = cmd.ExecuteReader();
            while (rd.Read())
            {
                myString = rd.GetString(0);
            }
            conn.Close();

            return myString;

            //select role_name from abetdb.dbo.staff as s join abetdb.dbo.roles as r on s.role = r.id where s.euid = 'DK2121';
        }

        public IEnumerable<Section> GetSectionsByUserId(string userId, int year, string term)
        {
            List<Section> secList = new List<Section>();

            string query =
                @"select distinct st.first_name as 'i_firstname', st.last_name as 'i_lastname', st.euid as 'i_euid',
		sts.first_name as 'c_firstname', sts.last_name as 'c_lastname', sts.euid as 'c_euid',
		s.completed as 'sectionCompleted', s.num_of_students as 'NumberOfStudents', s.section_number as 'sectionNumber',
		c.display_name as 'displayName', c.course_number as 'courseNumber', c.completed as 'courseComplete', c.coordinator_comment as 'c_comment',
		c.department as 'department'
from sections s
inner join courses c on s.course_id = c.id
inner join staff st on s.instructor_id = st.euid
inner join staff sts on c.coordinator_id = sts.euid
where (s.instructor_id = @euid or c.coordinator_id = @euid) and c.semester = @term and c.year = @year";
            SqlConnection conn = GetConnection();
            conn.Open();
            SqlCommand cmd = new SqlCommand(query, conn);
            cmd.Parameters.Add(new SqlParameter("@euid", SqlDbType.VarChar, 50)).Value = userId;
            cmd.Parameters.Add(new SqlParameter("@term", SqlDbType.VarChar, 50)).Value = term;
            cmd.Parameters.Add(new SqlParameter("@year", SqlDbType.Int)).Value = year;
            cmd.Prepare();
            using SqlDataReader rd = cmd.ExecuteReader();
            while (rd.Read())
            {
                Section s = new Section
                {
                    Coordinator = new Coordinator(rd["c_firstname"].ToString(), rd["c_lastname"].ToString(),
                        rd["c_euid"].ToString()),
                    Instructor = new Instructor(rd["i_firstname"].ToString(), rd["i_lastname"].ToString(),
                        rd["i_euid"].ToString()),
                    CoordinatorComment = rd["c_comment"].ToString(),
                    CourseNumber = Convert.ToInt32(rd["courseNumber"]),
                    Department = rd["department"].ToString(),
                    DisplayName = rd["displayName"].ToString(),
                    IsCourseCompleted = Convert.ToBoolean(rd["courseComplete"]),
                    IsSectionCompleted = Convert.ToBoolean(rd["sectionCompleted"]),
                    //NumberOfStudents = Convert.ToInt32(rd["NumberOfStudents"]),
                    NumberOfStudents = rd["NumberOfStudents"] as int? ?? 0,
                    SectionNumber = Convert.ToInt32(rd["sectionNumber"]),
                    Year = year,
                    Semester = term
                };
                secList.Add(s);
            }
            conn.Close();
            return secList;
        }

        public Course_Objectives GetCourseObjectives(string program)
        {
            Student_Outcomes student_Outcomes;

            Course_Objectives co = new Course_Objectives();
            string query1 = @"select st.num as num, st.student_outcome from student_outcomes as st join programs as p 
on p.id = st.program_id where p.program = @program";
            SqlConnection conn = GetConnection();
            conn.Open();
            SqlCommand cmd = new SqlCommand(query1, conn);
            cmd.Parameters.Add(new SqlParameter("@program", SqlDbType.VarChar, 50)).Value = program;
            cmd.Prepare();
            using SqlDataReader rd = cmd.ExecuteReader();
            while (rd.Read())
            {
                student_Outcomes = new Student_Outcomes(Convert.ToInt32(rd["num"]), rd["student_outcome"].ToString()); 
            }
            query1 = @"select c.display_name, co.num, co.course_outcome from courses as c join course_outcomes as co on c.id = co.course_id";
            cmd = new SqlCommand(query1, conn);
            cmd.Prepare();
            while (rd.Read())
            {
                student_Outcomes = new Student_Outcomes(Convert.ToInt32(rd["num"]), rd["student_outcome"].ToString());
            }

            return co;

        }
    }
}
