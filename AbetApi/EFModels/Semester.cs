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

          // NEEDS ADDITIONAL NULL CHECKING AND INPUT SCRUBBING
          public async static Task AddSemester(Semester semester)
          {
               // Sets the user id to be 0, so entity framework will give it a primary key
               semester.SemesterId = 0;

               //Opens a context with the database, makes changes, and saves the changes
               await using (var context = new ABETDBContext())
               {
                    context.Semesters.Add(semester);
                    context.SaveChanges();

                    return;
               }
          } // AddSemester

          // This function searches for an item with the provided term and year. It returns null if the item isn't found.
          public async static Task<Semester> GetSemester(string term, int year)
          {
               await using (var context = new ABETDBContext())
               {
                    Semester semester = context.Semesters.FirstOrDefault(p => p.Term == term && p.Year == year);
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
          public async static void EditSemester(string term, int year, Semester NewValue)
          {
               await using (var context = new ABETDBContext())
               {
                    Semester semester = context.Semesters.FirstOrDefault(p => p.Term == term && p.Year == year);
                    semester.Term = NewValue.Term;
                    semester.Year = NewValue.Year;

                    context.SaveChanges();
               }
          }

          // This function finds a semester and deletes it.
          // Anybody calling this function should make sure you want to call this function. Deletions are final.
          public async static Task DeleteSemester(string term, int year)
          {
               await using (var context = new ABETDBContext())
               {
                    Semester semester = context.Semesters.FirstOrDefault(p => p.Term == term && p.Year == year);
                    context.Remove(semester);
                    context.SaveChanges();

                    return;
               }
          } // DeleteSemester

          public static List<Course> GetCourses(string term, int year)
          {
               List<Course> list = new List<Course>();

               using (var context = new ABETDBContext())
               {
                    //FIXME - Add null check
                    Semester semester = context.Semesters.FirstOrDefault(p => p.Term == term && p.Year == year);
                    context.Entry(semester).Collection(semester => semester.Courses).Load();

                    foreach (var course in semester.Courses)
                    {
                         list.Add(course);
                    }
                    return list;
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
