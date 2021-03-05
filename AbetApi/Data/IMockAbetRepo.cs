using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static AbetApi.Models.AbetModels;

namespace AbetApi.Data
{
    public interface IMockAbetRepo
    {
        List<Section> GetSectionsByUserId(string userId, int year, string semester);
        List<Section> GetSectionsByYearAndSemester(int year, string semester);
        Form GetFormBySection(Section section);
        List<Form> GetFormsByCourse(Course course);
        List<Form> GetFormsByYearAndSemester(int year, string semester);
        Form GetBlankForm();
        bool PostForm(Form form);
        bool PostComment(Course course);
        bool AddSection(Section section);
        string GetRole(string userId);
        FacultyList GetFacultyList();
        bool AddFacultyMember(Info info, string facultyType);
        Program_Outcomes GetCourseObjectives(string program);
        List<Course> GetCoursesByDepartment(string department);
        bool AddCourse(Course course);
        bool RemoveCourse(Course course);
        bool PostCourseOutcomes(List<Course_Outcome> courseOutcomesList);
        List<Course_Outcome> GetCourseOutcomesByCourse(Course course);
        List<Course> GetCoursesByYear(int year, string semester);
    }
}
