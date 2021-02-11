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
        IEnumerable<Section> GetSectionsByUserId(string userId, int year, string term);
        Course_Objectives GetCourseObjectives(string program);

    }
}
