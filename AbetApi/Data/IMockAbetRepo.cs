using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static AbetApi.Models.AbetModels;

namespace AbetApi.Data
{
    public interface IMockAbetRepo
    {
        IEnumerable<Section> GetSectionsByUserId(string userId, int year, string semester);
        IEnumerable<Section> GetSectionsByYearAndSemester(int year, string semester);
        Form GetFormBySection(Section section);
        IEnumerable<Form> GetFormsByCourse(Course course);
        IEnumerable<Form> GetFormsByYearAndSemester(int year, string semester);
        Form GetBlankForm();
        bool PostForm(Form form);
        bool PostComment(Course course);
        bool PostSection(Section section);
        string GetRole(string userId);
        FacultyList GetFacultyList();
        bool AddFacultyMember(Info info, string role);
    }
}
