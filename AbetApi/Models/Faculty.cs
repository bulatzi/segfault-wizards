using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AbetApi.Models
{
    public class Faculty
    {
        public List<AbetApi.EFModels.User> Admins { get; set; }
        public List<AbetApi.EFModels.User> Instructors { get; set; }
        public List<AbetApi.EFModels.User> Coordinators { get; set; }
    }
}
