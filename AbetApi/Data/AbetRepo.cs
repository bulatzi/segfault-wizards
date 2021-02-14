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
        int year = 2021;
        string semester = "spring";

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
                        MappedStudentOutcomes = (rd["student_outcome_mapping"].ToString()).Select(c => int.Parse(c.ToString())).ToArray()
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

        public List<Course> GetCoursesByDepartment(string department)
        {
            List<Course> coursesList = new List<Course>();
            Course course;
            string query = @"select c.course_number, st.first_name, st.last_name, st.euid, c.course_number, c.display_name, c.coordinator_comment, 
c.completed as IsCourseCompleted, c.department from courses as c 
left join staff as st on c.coordinator_id = st.euid
where c.department = @department and c.year = @year and c.semester = @semester";

            SqlConnection conn = GetConnection();
            conn.Open();
            SqlCommand cmd = new SqlCommand(query, conn);
            cmd.Parameters.Add(new SqlParameter("@department", SqlDbType.VarChar, 50)).Value = department;
            cmd.Parameters.Add(new SqlParameter("@year", SqlDbType.Int)).Value = year;
            cmd.Parameters.Add(new SqlParameter("@semester", SqlDbType.VarChar, 50)).Value = semester;
            using (SqlDataReader rd = cmd.ExecuteReader())
            {
                while (rd.Read())
                {
                    course = new Course
                    {
                        Coordinator = new Coordinator(rd["first_name"].ToString(), rd["last_name"].ToString(), rd["euid"].ToString()),
                        CourseNumber = Convert.ToInt32(rd["course_number"]),
                        DisplayName = rd["display_name"].ToString(),
                        CoordinatorComment = rd["coordinator_comment"].ToString(),
                        IsCourseCompleted = Convert.ToBoolean(rd["IsCourseCompleted"]),
                        Department = department,
                        Year = year,
                        Semester = semester
                    };
                    coursesList.Add(course);
                }
            }

            return coursesList;
        }

        public bool AddCourse(Course course)
        {
            string insertQuery = @"insert into courses (year, semester, department, course_number, coordinator_id, display_name)
values (@years,@semester, @department, @course_number, @coordinator_id, @display_name)";

            SqlConnection conn = GetConnection();
            conn.Open();

            SqlCommand cmd = new SqlCommand(insertQuery, conn);
            cmd.Parameters.Add(new SqlParameter("@years", SqlDbType.Int)).Value = course.Year;
            cmd.Parameters.Add(new SqlParameter("@semester", SqlDbType.VarChar, 50)).Value = course.Semester;
            cmd.Parameters.Add(new SqlParameter("@department", SqlDbType.VarChar, 50)).Value = course.Department;
            cmd.Parameters.Add(new SqlParameter("@course_number", SqlDbType.VarChar, 50)).Value = course.CourseNumber;
            cmd.Parameters.Add(new SqlParameter("@coordinator_id", SqlDbType.VarChar, 15)).Value = course.Coordinator.Id;
            cmd.Parameters.Add(new SqlParameter("@display_name", SqlDbType.VarChar, 50)).Value = course.DisplayName;
            cmd.Prepare();

            return cmd.ExecuteNonQuery() > 0;
        }

        public bool RemoveCourse(Course course)
        {
            string removeQuery = @"delete from courses where year = @year and semester = @semester and 
department = @department and course_number = @course_number";

            SqlConnection conn = GetConnection();
            conn.Open();

            SqlCommand cmd = new SqlCommand(removeQuery, conn);
            cmd.Parameters.Add(new SqlParameter("@year", SqlDbType.Int)).Value = course.Year;
            cmd.Parameters.Add(new SqlParameter("@semester", SqlDbType.VarChar, 50)).Value = course.Semester;
            cmd.Parameters.Add(new SqlParameter("@department", SqlDbType.VarChar, 50)).Value = course.Department;
            cmd.Parameters.Add(new SqlParameter("@course_number", SqlDbType.VarChar, 50)).Value = course.CourseNumber;
            cmd.Prepare();

            return cmd.ExecuteNonQuery() > 0;
        }

        public FacultyList GetFacultyList()
        {
            FacultyList facultyList = new FacultyList();
            Instructor instructor;
            Info info = new Info();

            string selectQuery = @"select st.euid as id, st.first_name, st.last_name, f.faculty_type 
from staff as st join faculty_types as f 
on st.faculty_type_id = f.id;";

            SqlConnection conn = GetConnection();
            conn.Open();

            SqlCommand cmd = new SqlCommand(selectQuery, conn);
            using (SqlDataReader rd = cmd.ExecuteReader())
            {
                while (rd.Read())
                {
                    instructor = new Instructor
                    {
                        FirstName = rd["first_name"].ToString(),
                        LastName = rd["last_name"].ToString(),
                        Id = rd["id"].ToString()
                    };
                    if (rd["faculty_type"].ToString() == "Full-time")
                        facultyList.Normal.Add(instructor);
                    /*else if (rd["faculty_type"].ToString() == "Adjuncts")
                    {
                        // add to facultyList.adjuncts(info);
                    }
                    else
                    {
                        // add to facultyList.fellow(info);
                    }
                    */

                    //}

                }
            }
            return facultyList;
        }

        public bool AddFacultyMember(Info info, string role)
        {
            //string selectQuery = @"select id from abetdb.dbo.faculty_types where faculty_type = 'Full-Time'";
            string insertQuery = @"insert into abetdb.dbo.staff (euid, first_name, last_name, role, faculty_type_id) 
values (@euid, @first_name, @last_name, @role, @faculty_type_id)";

            SqlConnection conn = GetConnection();
            conn.Open();

            SqlCommand cmd = new SqlCommand(insertQuery, conn);
            cmd.Parameters.Add(new SqlParameter("@euid", SqlDbType.VarChar, 20)).Value = "BB2020";
            cmd.Parameters.Add(new SqlParameter("@first_name", SqlDbType.VarChar, 50)).Value = "Barrett";
            cmd.Parameters.Add(new SqlParameter("@last_name", SqlDbType.VarChar, 50)).Value = "Bryant";
            cmd.Parameters.Add(new SqlParameter("@role", SqlDbType.Int)).Value = 2;
            cmd.Parameters.Add(new SqlParameter("@faculty_type_id", SqlDbType.Int)).Value = 1;
            cmd.Prepare();

            cmd.ExecuteNonQuery();

            return true;
        }
    }
}
