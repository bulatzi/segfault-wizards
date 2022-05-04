using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using AbetApi.Data;
using System.Threading.Tasks;

using System;
using System.Diagnostics;

namespace AbetApi.EFModels
{
    public class Major
    {
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public int MajorId { get; set; }
        public string Name { get; set; } // e.g. CS, CE, etc...
        [JsonIgnore]
        public ICollection<MajorOutcome> MajorOutcomes { get; set; }

        [JsonIgnore]
        public ICollection<Semester> Semesters { get; set; }

        [JsonIgnore]
        public ICollection<Course> CoursesRequiredBy { get; set; }

        public Major()
        {
            this.Semesters = new List<Semester>();
            this.MajorOutcomes = new List<MajorOutcome>();
            this.CoursesRequiredBy = new List<Course>();
        }

        public Major(string Name)
        {
            this.Name = Name;
        }

        public static async Task AddMajor(string term, int year, string majorName)
        {
            //Check if the term is null or empty.
            if (term == null || term == "")
            {
                throw new ArgumentException("The term cannot be empty.");
            }

            //Check if the year is before the establishment date of the university.
            if (year < 1890)
            {
                throw new ArgumentException("The year cannot be empty, or less than the establishment date of UNT.");
            }

            //Check if the name of the major is null or empty.
            if (majorName == null || majorName == "")
            {
                throw new ArgumentException("The name of the major cannot be empty.");
            }

            //Format term and major name to follow a standard.
            term = term[0].ToString().ToUpper() + term[1..].ToLower();
            majorName = majorName.ToUpper();

            await using (var context = new ABETDBContext())
            {
                //Try to find the specified semester in the database.
                Semester semester = context.Semesters.FirstOrDefault(p => p.Term == term && p.Year == year);

                //If it does not exist, throw an exception.
                if (semester == null)
                {
                    throw new ArgumentException("The specified semester does not exist in the database.");
                }

                //Load the majors under the specified semester.
                context.Entry(semester).Collection(semester => semester.Majors).Load();

                //Loop through the majors and look for the specified major.
                foreach (Major major in semester.Majors)
                {
                    //If the major already exists, then that is a duplicate and we don't allow duplicates.
                    if (major.Name == majorName)
                    {
                        throw new ArgumentException("The specified major already exists in the database.");
                    }
                }

                //Add the major to the database.
                Major tempMajor = new Major(majorName);
                context.Majors.Add(tempMajor);
                semester.Majors.Add(tempMajor);

                context.SaveChanges();
            }
        } // AddMajor

        public static async Task<List<Major>> GetMajors(string term, int year)
        {
            //Check if the term is null or empty.
            if (term == null || term == "")
            {
                throw new ArgumentException("The term cannot be empty.");
            }

            //Check if the year is before the establishment date of the university.
            if (year < 1890)
            {
                throw new ArgumentException("The year cannot be empty, or less than the establishment date of UNT.");
            }

            await using (var context = new ABETDBContext())
            {
                //Try to find the specified semester in the database.
                Semester semester = context.Semesters.FirstOrDefault(p => p.Term == term && p.Year == year);

                //If it does not exist, throw an exception.
                if (semester == null)
                {
                    throw new ArgumentException("The specified semester does not exist in the database.");
                }

                //Load the majors under the specified semester.
                context.Entry(semester).Collection(semester => semester.Majors).Load();

                //Return the list of majors.
                return semester.Majors.ToList();
            }
        } // GetMajors

        public async static Task EditMajor(string term, int year, string oldMajorName, string newMajorName)
        {
            //Check if the term is null or empty.
            if (term == null || term == "")
            {
                throw new ArgumentException("The term cannot be empty.");
            }

            //Check if the year is before the establishment date of the university.
            if (year < 1890)
            {
                throw new ArgumentException("The year cannot be empty, or less than the establishment date of UNT.");
            }

            //Check if the old name of the major is null or empty.
            if (oldMajorName == null || oldMajorName == "")
            {
                throw new ArgumentException("The name of the major cannot be empty.");
            }

            //Check if the new name of the major is null or empty.
            if (newMajorName == null || newMajorName == "")
            {
                throw new ArgumentException("The name of the major cannot be empty.");
            }

            //Format term and major names to follow a standard.
            term = term[0].ToString().ToUpper() + term[1..].ToLower();
            oldMajorName = oldMajorName.ToUpper();
            newMajorName = newMajorName.ToUpper();

            await using (var context = new ABETDBContext())
            {
                Major tempMajor = null;

                //Try to find the specified semester in the database.
                Semester semester = context.Semesters.FirstOrDefault(p => p.Term == term && p.Year == year);

                //If it does not exist, throw an exception.
                if (semester == null)
                {
                    throw new ArgumentException("The specified semester does not exist in the database.");
                }

                //Load the majors under the specified semester.
                context.Entry(semester).Collection(semester => semester.Majors).Load();

                //Loop through the majors and look for the specified major.
                foreach (Major major in semester.Majors)
                {
                    //Check if the new major name already exists, if it does then that is a duplicate and we do not allow duplicates.
                    if (major.Name == newMajorName)
                    {
                        throw new ArgumentException("The new major name already exists in the database.");
                    }

                    //Find the major specified to edit.
                    if (major.Name == oldMajorName)
                    {
                        tempMajor = major;
                    }
                }

                //Check if old major is null.
                if (tempMajor == null)
                {
                    throw new ArgumentException("The specified major to edit does not exist in the database.");
                }

                tempMajor.Name = newMajorName;
                context.SaveChanges();
            }
        } // EditMajor

        public async static Task DeleteMajor(string term, int year, string majorName)
        {
            //Check if the term is null or empty.
            if (term == null || term == "")
            {
                throw new ArgumentException("The term cannot be empty.");
            }

            //Check if the year is before the establishment date of the university.
            if (year < 1890)
            {
                throw new ArgumentException("The year cannot be empty, or less than the establishment date of UNT.");
            }

            //Check if the old name of the major is null or empty.
            if (majorName == null || majorName == "")
            {
                throw new ArgumentException("The name of the major cannot be empty.");
            }

            //Format term and major name to follow a standard.
            term = term[0].ToString().ToUpper() + term[1..].ToLower();
            majorName = majorName.ToUpper();

            await using (var context = new ABETDBContext())
            {
                //Try to find the specified semester in the database.
                Semester semester = context.Semesters.FirstOrDefault(p => p.Term == term && p.Year == year);

                //If it does not exist, throw an exception.
                if (semester == null)
                {
                    throw new ArgumentException("The specified semester does not exist in the database.");
                }

                //Load the majors under the specified semester.
                context.Entry(semester).Collection(semester => semester.Majors).Load();

                //Loop through the majors and look for the specified major.
                foreach (Major major in semester.Majors)
                {
                    if (major.Name == majorName)
                    {
                        context.Remove(major);
                        context.SaveChanges();
                        return;
                    }
                }

                throw new ArgumentException("The major specified does not exist in the database.");
            }
        } // DeleteMajor

        //This function gets all of the courses required by a major
        public async static Task<List<Course>> GetCoursesByMajor(string term, int year, string majorName)
        {
            //This function follows join tables from: Semester -> Major -> MajorOutcome -> CourseOutcome -> Course
            //This function could be very inefficient with production amounts of data in it. It's searching through every course mapped to a course/major outcome, so it could try to add the same course to the hashset as many as 7+ times.

            //Check if the term is null or empty.
            if (term == null || term == "")
            {
                throw new ArgumentException("The term cannot be empty.");
            }

            //Check if the year is before the establishment date of the university.
            if (year < 1890)
            {
                throw new ArgumentException("The year cannot be empty, or less than the establishment date of UNT.");
            }

            //Check if the old name of the major is null or empty.
            if (majorName == null || majorName == "")
            {
                throw new ArgumentException("The name of the major cannot be empty.");
            }

            //Format term and major name to follow a standard.
            term = term[0].ToString().ToUpper() + term[1..].ToLower();
            majorName = majorName.ToUpper();

            await using (var context = new ABETDBContext())
            {
                Major tempMajor = null;

                //Try to find the specified semester in the database.
                Semester semester = context.Semesters.FirstOrDefault(p => p.Term == term && p.Year == year);

                //If it does not exist, throw an exception.
                if (semester == null)
                {
                    throw new ArgumentException("The specified semester does not exist in the database.");
                }

                //Load the majors under the specified semester.
                context.Entry(semester).Collection(semester => semester.Majors).Load();

                //Loop through the majors and look for the specified major.
                foreach (Major major in semester.Majors)
                {
                    if (major.Name == majorName)
                    {
                        tempMajor = major;
                        break;
                    }
                }

                //Check if major is null.
                if (tempMajor == null)
                {
                    throw new ArgumentException("The specified major does not exist in the database.");
                }

                //Loads all relevant major outcomes
                context.Entry(tempMajor).Collection(major => major.MajorOutcomes).Load();

                //Go through each list of course outcomes and add the course
                HashSet<Course> courses = new HashSet<Course>();
                foreach (MajorOutcome majorOutcome in tempMajor.MajorOutcomes)
                {
                    //Load course outcomes
                    context.Entry(majorOutcome).Collection(majorOutcome => majorOutcome.CourseOutcomes).Load();

                    foreach (CourseOutcome courseOutcome in majorOutcome.CourseOutcomes)
                    {
                        //Load the relevant course
                        context.Entry(courseOutcome).Collection(courseOutcome => courseOutcome.Courses).Load();

                        //Take that course, and add it to the hash set. This may add something to the list that's already there, but the hash set gets rid of duplicates.
                        Course tempCourse = courseOutcome.Courses.FirstOrDefault();
                        if (tempCourse != null)
                        {
                            courses.Add(tempCourse);
                        }
                    }
                }

                //Check to see if any courses were added to the list.
                if(courses.Count == 0)
                {
                    throw new ArgumentException("The major specified has no courses required.");
                }

                return courses.ToList();
            }
        } // GetCoursesByMajor

        public async static Task<List<MajorOutcome>> GetMajorOutcomesByMajor(string term, int year, string majorName)
        {
            //Check if the term is null or empty.
            if (term == null || term == "")
            {
                throw new ArgumentException("The term cannot be empty.");
            }

            //Check if the year is before the establishment date of the university.
            if (year < 1890)
            {
                throw new ArgumentException("The year cannot be empty, or less than the establishment date of UNT.");
            }

            //Check if the old name of the major is null or empty.
            if (majorName == null || majorName == "")
            {
                throw new ArgumentException("The name of the major cannot be empty.");
            }

            //Format term and major name to follow a standard.
            term = term[0].ToString().ToUpper() + term[1..].ToLower();
            majorName = majorName.ToUpper();

            await using (var context = new ABETDBContext())
            {
                //Try to find the specified semester in the database.
                Semester semester = context.Semesters.FirstOrDefault(p => p.Term == term && p.Year == year);

                //If it does not exist, throw an exception.
                if (semester == null)
                {
                    throw new ArgumentException("The specified semester does not exist in the database.");
                }

                //Load the majors under the specified semester.
                context.Entry(semester).Collection(semester => semester.Majors).Load();

                //Loop through the majors and look for the specified major.
                foreach (Major major in semester.Majors)
                {
                    //If we find the right major, then load the major outcomes under it and add them all to a list.
                    if (majorName == major.Name)
                    {
                        context.Entry(major).Collection(major => major.MajorOutcomes).Load();

                        //Check to see if the specified major has major outcomes.
                        if(major.MajorOutcomes.Count == 0)
                        {
                            throw new ArgumentException("The major specified has no major outcomes.");
                        }

                        return major.MajorOutcomes.ToList();
                    }
                }

                throw new ArgumentException("The major specified has no major outcomes.");
            }
        }//GetMajorOutcomesByMajor

    } // Major
}
