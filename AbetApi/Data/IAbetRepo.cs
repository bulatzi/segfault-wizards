using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static AbetApi.Models.AbetModels;

namespace AbetApi.Data
{
    public interface IAbetRepo
    {
        string GetRole(string userId);
        List<Section> GetSectionsByUserId(string userId, int year, string term);
        Program_Outcomes GetCourseObjectives(string program);
        List<Course> GetCoursesByDepartment(string department);
        bool AddCourse(Course course);
        bool RemoveCourse(Course course);
        FacultyList GetFacultyList();
        bool AddFacultyMember(Info info, string facultyType);
        Form GetBlankForm(Section section);
        Form GetFormBySection(Section section);
        SqlReturn PostForm(Form form);
        List<Form> GetFormsByCourse(Course course);

    }
}
