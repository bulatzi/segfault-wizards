using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AbetApi.Authentication
{
    public static class RoleTypes
    {
        //Hierarchy of role levels for authorization
        public const string Admin = "Admin"; //All admins are coordinators and instructors
        public const string Coordinator = "Coordinator, Admin"; //All coordinators are instructors
        public const string Instructor = "Instructor, Coordinator, Admin";
        public const string Student = "Student";
    }
}
