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
        //SqlReturn GetCourseObjectives(string program);
        List<Course> GetCoursesByDepartment(string department);
        bool AddCourse(Course course);
        bool RemoveCourse(Course course);
        FacultyList GetFacultyList();
        bool AddFacultyMember(Info info, string facultyType);
        Form GetBlankForm(Section section);
        Form GetFormBySection(Section section);
        SqlReturn PostForm(Form form);
        List<Form> GetFormsByCourse(Course course);
        List<Section> GetSectionsByYearAndSemester(int year, string semester);
        List<Course> GetCoursesByYear(int year, string semester);
        bool PostSection(Section section);
        List<CourseMapping> GetCourseOutcomesByCourse(Course course);
        bool PostComment(Course course);
        SqlReturn PostAccessDbData(string filePath);
        SqlReturn PostStudentWorkInfo(StudentWork studentWork, Section section);
        StudentWork GetStudentWorkInfo(string fileId);
        bool AddProgram(string program);
        bool DeleteProgram(string program);
        List<UntPrograms> GetAllPrograms();
        //bool PostStudentSurvey(StudentSurvey studentSurvey);
        SqlReturn PostStudentSurvey(StudentSurvey studentSurvey);
        // catogorize by instructor, coordinator and admin functions

    }
}
