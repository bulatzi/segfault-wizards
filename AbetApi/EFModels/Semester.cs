using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.Json.Serialization;
using AbetApi.Data;

namespace AbetApi.EFModels
{
    public class Semester
    {
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public int SemesterId { get; set; }
        public int Year { get; set; }
        public string Term { get; set; }
        //string Session { get; set; } // Session is not required. All courses fall under the purview of spring/fall.
        [JsonIgnore]
        public ICollection<Course> Courses { get; set; }
        [JsonIgnore]
        public ICollection<Major> Majors { get; set; }

        // This constructor creates an empty semester.
        // You must add majors and courses afterwards
        public Semester(string Term, int year)
        {
            this.Term = Term;
            this.Year = year;
        }

        Semester()
        {
            this.Courses = new List<Course>();
            this.Majors = new List<Major>();
        }

        public async static Task AddSemester(Semester semester)
        {
            // Sets the user id to be 0, so entity framework will give it a primary key
            semester.SemesterId = 0;

            //Check the term is not null or empty.
            if (semester.Term == null || semester.Term == "")
            {
                throw new ArgumentException("The term cannot be empty.");
            }

            //Check the year is not before the establishment of the university.
            if (semester.Year < 1890)
            {
                throw new ArgumentException("The year cannot be empty, or less than the establishment date of UNT.");
            }

            //Format the term string to follow a standard.
            semester.Term = semester.Term[0].ToString().ToUpper() + semester.Term.Substring(1);

            //Opens a context with the database, makes changes, and saves the changes
            await using (var context = new ABETDBContext())
            {
                //Try to find the semester in the database.
                Semester tempSemester = context.Semesters.FirstOrDefault(p => p.Term == semester.Term && p.Year == semester.Year);

                //If the semester already exists in the database, then throw an exception.
                if(tempSemester != null)
                {
                    throw new ArgumentException("The semester you are trying to add already exists in the database.");
                }

                context.Semesters.Add(semester);
                context.SaveChanges();

                return;
            }
        } // AddSemester

        // This function searches for an item with the provided term and year. It returns null if the item isn't found.
        public async static Task<Semester> GetSemester(string term, int year)
        {
            //Check the term is not null or empty.
            if (term == null || term == "")
            {
                throw new ArgumentException("The term cannot be empty.");
            }

            //Check the year is not before the establishment of the university.
            if (year < 1890)
            {
                throw new ArgumentException("The year cannot be empty, or less than the establishment date of UNT.");
            }

            //Format the term string to follow a standard.
            term = term[0].ToString().ToUpper() + term.Substring(1);

            //Try to find the specified semester.
            await using (var context = new ABETDBContext())
            {
                Semester semester = context.Semesters.FirstOrDefault(p => p.Term == term && p.Year == year);

                //If the specified semester does not exist, then throw an exception.
                if(semester == null)
                {
                    throw new ArgumentException("The specified semester does not exist in the databaase.");
                }

                return semester;
            }
        }

        //COULD GIVE INDICATION OF NO SEMESTERS LISTED
        //This function returns all semesters in the Semester table from the database.
        public static async Task<List<Semester>> GetSemesters()
        {
            await using (var context = new ABETDBContext())
            {
                return context.Semesters.ToList();
            }
        } // GetSemesters

        // This function finds a semester by term and year, and updates the semester to the values of the provided semester object.
        // Note: This function may need to also include editing courses and majors in the future, but it's not here currently that data will need to have its own dedicated functions for editing.
        public static async Task EditSemester(string term, int year, Semester NewValue)
        {
            //Check that the term of the semester to be edited is not null or empty.
            if (term == null || term == "")
            {
                throw new ArgumentException("The term for which semester to edit cannot be empty.");
            }

            //Check that the year is not before the establishment of the university.
            if (year < 1890)
            {
                throw new ArgumentException("The year for which semester to edit cannot be empty, or less than the establishment date of UNT.");
            }

            //Check that the new term is not null or empty.
            if (NewValue.Term == null || NewValue.Term == "")
            {
                throw new ArgumentException("The new term cannot be empty.");
            }

            //Check that the new year is not before the establishment of the university.
            if (NewValue.Year < 1890)
            {
                throw new ArgumentException("The new year cannot be empty, or less than the establishment date of UNT.");
            }

            //Format the term strings to follow a standard.
            term = term[0].ToString().ToUpper() + term.Substring(1);
            NewValue.Term = NewValue.Term[0].ToString().ToUpper() + NewValue.Term.Substring(1);

            await using (var context = new ABETDBContext())
            {
                //Try to find the specified semester to be edited.
                Semester semester = context.Semesters.FirstOrDefault(p => p.Term == term && p.Year == year);

                //If the semester specified to edit does not exist, then throw an exception.
                if (semester == null)
                {
                    throw new ArgumentException("The specified semester does not exist in the database.");
                }

                //Try to find the new semester information in the database.
                Semester duplicateSemester = context.Semesters.FirstOrDefault(p => p.Term == NewValue.Term && p.Year == NewValue.Year);

                //If we do find the new information then that is a duplicate and we don't allow duplicates.
                if (duplicateSemester != null)
                {
                    throw new ArgumentException("That semester already exists in the database.");
                }

                //Copy new values of semester over to the semester that's being edited
                semester.Term = NewValue.Term;
                semester.Year = NewValue.Year;

                context.SaveChanges();
            }
        }

        // This function finds a semester and deletes it.
        // Anybody calling this function should make sure you want to call this function. Deletions are final.
        public async static Task DeleteSemester(string term, int year)
        {
            //Check that the term of the semester is not null or empty.
            if (term == null || term == "")
            {
                throw new ArgumentException("The term for which semester to delete cannot be empty.");
            }

            //Check that the year is not before the establishment of the university.
            if (year < 1890)
            {
                throw new ArgumentException("The year for which semester to delete cannot be empty, or less than the establishment date of UNT.");
            }

            //Format the term string to follow a standard.
            term = term[0].ToString().ToUpper() + term.Substring(1);

            await using (var context = new ABETDBContext())
            {
                //Try to find the specified semester.
                Semester semester = context.Semesters.FirstOrDefault(p => p.Term == term && p.Year == year);

                //If the specified semester does not exist, then throw an exception.
                if(semester == null)
                {
                    throw new ArgumentException("The semester specified to delete does not exist in the database.");
                }

                context.Remove(semester);
                context.SaveChanges();
            }
        } // DeleteSemester

        public static async Task<List<Course>> GetCourses(string term, int year)
        {
            //Check that the term of the semester is not null or empty..
            if (term == null || term == "")
            {
                throw new ArgumentException("The term for which semester's courses to display cannot be empty.");
            }

            //Check that the year is not before the establishment of the university.
            if (year < 1890)
            {
                throw new ArgumentException("The year for which semester's courses to display cannot be empty, or less than the establishment date of UNT.");
            }

            //Format the term string to follow a standard.
            term = term[0].ToString().ToUpper() + term.Substring(1);

            await using (var context = new ABETDBContext())
            {
                //Try to find the specified semester.
                Semester semester = context.Semesters.FirstOrDefault(p => p.Term == term && p.Year == year);

                //Throw an exception if the semester specified does not exist.
                if(semester == null)
                {
                    throw new ArgumentException("The semester specified does not exist in the database.");
                }

                context.Entry(semester).Collection(semester => semester.Courses).Load();

                return semester.Courses.ToList();
            }
        }
    }
    /*
        As of November 17th, 2021, the current semester catalogue is:
            Fall 2021
                8W1 Session
                8W2 Session
            Spring 2022
                3W1 Session  (Side note: This is the winter semester)
                8W1 Session
                8W2 Session
            Summer 2022
                3W1 Session
                8W1 Session
                8W2 Session
                5W1 Session
                10W Session
                5W2 Session
     */
}
