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
            @"Server=TEBA-D\ABETDATABASE;Database=abetdb;Trusted_Connection=True";
        // on VM, server=TEBA-D\ABETDATABASE
        // on mine, server=TRICO-SCHOOL\SQLEXPRESS
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

        public Program_Outcomes GetCourseObjectives(string program)
        {
            Program_Outcomes program_outcomes ;
            Course_Objectives course_objectives = null;
            Course_Outcome course_outcome;
            Student_Outcome student_outcome;
            List<Student_Outcome> student_Outcomes = new List<Student_Outcome>();
            List<Course_Outcome> course_Outcomes = new List<Course_Outcome>();
            List<Course_Objectives> courseObjectives = new List<Course_Objectives>();
            string displayName = null;

            string query1 = @"select co.num as 'order', co.course_outcome as outcome, cob.student_outcome_mapping, c.display_name from 
course_outcomes as co join course_objective as cob on co.id = cob.course_outcome_id 
join courses as c on c.id = co.course_id where c.department = 'csce'; ";
            SqlConnection conn = GetConnection();
            conn.Open();
            SqlCommand cmd = new SqlCommand(query1, conn);
            cmd.Prepare();
            using (SqlDataReader rd = cmd.ExecuteReader())
            {
                while (rd.Read())
                {
                    course_outcome = new Course_Outcome
                    {
                        Order = Convert.ToInt32(rd["order"]),
                        Outcome = rd["outcome"].ToString(),
                        Mapped = rd["student_outcome_mapping"].ToString()
                    };
                    if (displayName == null) displayName = rd["display_name"].ToString();
                    else if (displayName != rd["display_name"].ToString())
                    {
                        course_objectives = new Course_Objectives(displayName, course_Outcomes);
                        courseObjectives.Add(course_objectives);
                        displayName = rd["display_name"].ToString();
                        course_Outcomes = new List<Course_Outcome>();

                    }
                    course_Outcomes.Add(course_outcome);
                }
            }
            
            course_objectives = new Course_Objectives(displayName, course_Outcomes);
            courseObjectives.Add(course_objectives);

            query1 = @"select st.num, st.student_outcome, p.program from student_outcomes as 
st join programs as p on p.id = st.program_id where p.program = @program";
            cmd = new SqlCommand(query1, conn);
            cmd.Parameters.Add(new SqlParameter("@program", SqlDbType.VarChar, 50)).Value = program;
            cmd.Prepare();
            using (SqlDataReader rd = cmd.ExecuteReader())
            {
                while (rd.Read())
                {
                    student_outcome = new Student_Outcome
                    {
                        Order = Convert.ToInt32(rd["num"]),
                        Outcome = rd["student_outcome"].ToString()
                    };
                    student_Outcomes.Add(student_outcome);
                }
            }

            program_outcomes = new Program_Outcomes(program, courseObjectives, student_Outcomes);

            return program_outcomes;

        }
    }
}
