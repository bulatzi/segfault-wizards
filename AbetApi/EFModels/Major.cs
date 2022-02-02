using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using AbetApi.Data;

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
        public ICollection<Semester> Semesters { get; set; } //

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

        // FIXME - Majors are required to have a semester that they're a part of.
        public async static void AddMajor(string term, int year, Major major)
        {
            //Sets the user ID to 0, to allow the database to auto increment the UserId value
            major.MajorId = 0;

            await using (var context = new ABETDBContext())
            {
                Semester semester = context.Semesters.FirstOrDefault(p => p.Term == term && p.Year == year);

                context.Majors.Add(major);
                semester.Majors.Add(major);
                context.SaveChanges();
            }
        }

        public static Major GetMajor(string term, int year, string name)
        {
            using (var context = new ABETDBContext())
            {
                Semester semester = context.Semesters.FirstOrDefault(p => p.Term == term && p.Year == year);
                context.Entry(semester).Collection(semester => semester.Majors).Load();

                foreach (var major in semester.Majors)
                {
                    if (major.Name == name)
                        return major;
                }
                return null;
            }
        }

       public static List<Major> GetAllMajors(string term, int year)
        {
            using (var context = new ABETDBContext())
            {
                Semester semester = context.Semesters.FirstOrDefault(p => p.Term == term && p.Year == year);

                context.Entry(semester).Collection(semester => semester.Majors).Load();

                return semester.Majors.ToList();
            }
        }

        public async static void EditMajor(string term, int year, string name, string NewValue)
        {
            await using (var context = new ABETDBContext())
            {
                Semester semester = context.Semesters.FirstOrDefault(p => p.Term == term && p.Year == year);
                context.Entry(semester).Collection(semester => semester.Majors).Load();

                foreach (var major in semester.Majors)
                {
                    if (major.Name == name)
                    {
                        major.Name = NewValue;
                        context.SaveChanges();
                        return;
                    }
                }
            }
        }

        public async static void DeleteMajor(string term, int year, string name)
        {
            await using (var context = new ABETDBContext())
            {
                Semester semester = context.Semesters.FirstOrDefault(p => p.Term == term && p.Year == year);
                context.Entry(semester).Collection(semester => semester.Majors).Load();

                foreach (var major in semester.Majors)
                {
                    if (major.Name == name)
                    {
                        context.Remove(major);
                        context.SaveChanges();
                        return;
                    }
                }
            }
        }
    }
}
