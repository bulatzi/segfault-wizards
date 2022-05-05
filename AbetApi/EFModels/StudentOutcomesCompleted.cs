using System;
using AbetApi.Data;
using System.Text.Json.Serialization;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

//! The EFModels namespace
/*! 
 * This namespace falls under the AbetAPI namespace, and is for EFModels.
 * The EFModels are generally called from the Controllers namespace, to 
 * provide the controllers functionality, ultimately giving endpoints/functionality
 * for the UI elements
 */
namespace AbetApi.EFModels
{

    //! The StudentOutcomesCompleted Class
    /*! 
     * This class gets called by the StudentOutcomesCompletedController class
     * and provides functions to get and return data.
     * The object is a container for storing multiple majors and numbers of students who completed various outcomes.
     */
    public class StudentOutcomesCompleted
    {
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        //! The StudentOutcomesCompletedID setter/getter function
        /*!
         *  This is a single line dual function for setting and getting
         */
        public int StudentOutcomesCompletedId { get; set; }
        //! The Term setter/getter function
        /*!
         *  This is a single line dual function for setting and getting
         */
        public string Term { get; set; }
        //! The Year setter/getter function
        /*!
         *  This is a single line dual function for setting and getting
         */
        public int Year { get; set; }
        //! The ClassDepartment setter/getter function
        /*!
         *  This is a single line dual function for setting and getting
         */
        public string ClassDepartment { get; set; }
        //! The CourseNumber setter/getter function
        /*!
         *  This is a single line dual function for setting and getting
         */
        public string CourseNumber { get; set; }
        //! The CourseOutcomeName setter/getter function
        /*!
         *  This is a single line dual function for setting and getting
         */
        public string CourseOutcomeName { get; set; }
        //! The SectionName setter/getter function
        /*!
         *  This is a single line dual function for setting and getting
         */
        public string SectionName { get; set; }
        //! The MajorName setter/getter function
        /*!
         *  This is a single line dual function for setting and getting
         */
        public string MajorName { get; set; }
        //! The StudentsCompleted setter/getter function
        /*!
         *  This is a single line dual function for setting and getting
         */
        public int StudentsCompleted { get; set; }
        //! Parameterized Constructor
        /*!
         * This is a constructor to create a StudentOutcomesCompleted object from the given parameters
         * \param instructorEUID A string of the desired instructors enterprise user identification (EUID).
         * \param sectionCompleted A bool of whether a section is completed or not
         * \param sectionNumber A string of the section number of the section of the course for the survey
         * \param numberOfStudents An int of the number of students for a section
         */
        public StudentOutcomesCompleted(string Term, int Year, string ClassDepartment, string CourseNumber, string SectionName, string CourseOutcomeName,  string MajorName, int StudentsCompleted)
        {
            this.Term = Term;
            this.Year = Year;
            this.ClassDepartment = ClassDepartment;
            this.CourseNumber = CourseNumber;
            this.CourseOutcomeName = CourseOutcomeName;
            this.SectionName = SectionName;
            this.MajorName = MajorName;
            this.StudentsCompleted = StudentsCompleted;
        }

        //! The SetStudentOutcomesCompleted function
        /*! 
         * This function sets the students completed for a given major/course outcome.
         * If the the given major/course outcome doesn't exits, it creates it.
         * It is an async Task to pass exceptions to the Controllers.StudentOutcomesCompletedController in Controllers
         * \param term The Term (Fall/Spring) for the given semester
         * \param year The year for the given semester
         * \param department Major department, such as CSCE or MEEN
         * \param courseNumber Course identifier, such as 3600 for Systems Programming
         * \param sectionName
         * \param CourseOutcomeName
         * \param MajorName
         * \param StudentsCompleted
         */
        public async static Task SetStudentOutcomesCompleted(string Term, int Year, string ClassDepartment, string CourseNumber, string SectionName, string CourseOutcomeName, string MajorName, int StudentsCompleted)
        {
            await using(var context = new ABETDBContext())
            {
                //Search existing items. If one of that major name already exists, overwrite it and exit early

                List<StudentOutcomesCompleted> doesExist = context.StudentOutcomesCompleted.Where(p => p.Term == Term && p.Year == Year && p.ClassDepartment == ClassDepartment && p.CourseNumber == CourseNumber && p.SectionName == SectionName && p.CourseOutcomeName == CourseOutcomeName && p.MajorName == MajorName).ToList();
                if(doesExist.Any())
                {
                    foreach(var tempStudentOutcomesCompleted in doesExist)
                    {
                        if (tempStudentOutcomesCompleted.MajorName == MajorName)
                        {
                            tempStudentOutcomesCompleted.StudentsCompleted = StudentsCompleted;
                            context.SaveChanges();
                            return;
                        }
                    }
                }

                //If the object doesn't exist, add it and return
                StudentOutcomesCompleted studentOutcomesCompleted = new StudentOutcomesCompleted(Term, Year, ClassDepartment, CourseNumber, SectionName, CourseOutcomeName, MajorName, StudentsCompleted);
                studentOutcomesCompleted.StudentOutcomesCompletedId = 0;

                context.StudentOutcomesCompleted.Add(studentOutcomesCompleted);
                context.SaveChanges();

                return;
            }

        } // SetStudentOutcomesCompleted

        //! The GetStudentOutcomesCompleted function
        /*! 
         * This function finds a StudentOutcomesCompleted object that matches the given parameters and sets the list equal to it, then returns it
         * It is an async Task<List<StudentOutcomesCompleted>> to pass exceptions and a List of studentOutcomesComplted objects to the Controllers.StudentOutcomesCompletedController in Controllers
         * \param term The Term (Fall/Spring) for the given semester
         * \param year The year for the given semester
         * \param ClassDepartment Department for the class
         * \param CourseNumber Course identifier, such as 3600 for Systems Programming
         * \param SectionName Course section, such as 001 or 002
         */
        public async static Task<List<StudentOutcomesCompleted>> GetStudentOutcomesCompleted(string Term, int Year, string ClassDepartment, string CourseNumber, string SectionName)
        {
            //each object is a standalone object
            //write it as a linq statement, matching on all the parameters provided in the function call

            await using (var context = new ABETDBContext())
            {
                List<StudentOutcomesCompleted> studentOutcomesCompleted = context.StudentOutcomesCompleted.Where<StudentOutcomesCompleted>(p => p.Term == Term && p.Year == Year && p.ClassDepartment == ClassDepartment && p.CourseNumber == CourseNumber && p.SectionName == SectionName).ToList();

                return studentOutcomesCompleted;
            }
        } // GetStudentOutcomesCompleted

        //! The GetSemesterStudentOutcomesCompleted function
        /*! 
         * This function finds a StudentOutcomesCompleted object that matches the given parameters and sets the list equal to it, then returns it
         * It then sends that list to ConvertToModelStudentOutcomesCompleted to attach a number of students that have completed
         * an associated course/major outcome.
         * It is an async Task<List<StudentOutcomesCompleted>> to pass exceptions and a List of studentOutcomesComplted objects to the Controllers.StudentOutcomesCompletedController in Controllers
         * \param term The Term (Fall/Spring) for the given semester
         * \param year The year for the given semester
         */
        public async static Task<List<StudentOutcomesCompleted>> GetSemesterStudentOutcomesCompleted(string Term, int Year)
        {
            await using (var context = new ABETDBContext())
            {
                List<StudentOutcomesCompleted> studentOutcomesCompleted = context.StudentOutcomesCompleted.Where<StudentOutcomesCompleted>(p => p.Term == Term && p.Year == Year).ToList();

                return studentOutcomesCompleted;
            }
        } // GetSemesterStudentOutcomesCompleted
    }
}
