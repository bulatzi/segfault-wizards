using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.Json.Serialization;
using AbetApi.Data;

namespace AbetApi.EFModels
{
    public class MajorOutcome
    {
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public int MajorOutcomeId { get; set; }
        public string Name { get; set; } //e.g. 1, 2, 3, etc... The outcome designation.
        public string Description { get; set; }
        [JsonIgnore]
        public ICollection<Major> Majors { get; set; }
        [JsonIgnore]
        public ICollection<CourseOutcome> CourseOutcomes { get; set; }

        public MajorOutcome() {
            this.Majors = new List<Major>();
            this.CourseOutcomes = new List<CourseOutcome>();
        }
        public MajorOutcome(string Name, string description)
        {
            this.Name = Name;
            this.Description = description;
        }

        public static void AddMajorOutcome(string term, int year, string majorName, MajorOutcome majorOutcome)
        {
            majorOutcome.MajorOutcomeId = 0;
            using (var context = new ABETDBContext())
            {
                Major tempMajor = null;
                Semester semester = context.Semesters.FirstOrDefault(p => p.Term == term && p.Year == year);
                context.Entry(semester).Collection(semester => semester.Majors).Load();
                foreach (var major in semester.Majors)
                {
                    if(major.Name == majorName)
                    {
                        tempMajor = major;
                        break;
                    }
                }

                context.MajorOutcomes.Add(majorOutcome);
                tempMajor.MajorOutcomes.Add(majorOutcome);
                context.SaveChanges();
            }
        }

        public static async Task<MajorOutcome> GetMajorOutcome(string term, int year, string majorName, string outcomeName)
        {
            await using (var context = new ABETDBContext())
            {
                //FIXME - Add null checking
                Semester semester = context.Semesters.FirstOrDefault(p => p.Term == term && p.Year == year);
                context.Entry(semester).Collection(semester => semester.Majors).Load();
                foreach (var major in semester.Majors)
                {
                    if (major.Name == majorName)
                    {
                        context.Entry(major).Collection(major => major.MajorOutcomes).Load();
                        foreach (var majorOutcome in major.MajorOutcomes)
                        {
                            if (majorOutcome.Name == outcomeName)
                            {
                                return majorOutcome;
                            }
                        }
                        return null;
                    }
                }
                return null;
            }
        }

        public static void EditMajorOutcome(string term, int year, string majorName, string outcomeName, MajorOutcome NewValue)
        {
            using (var context = new ABETDBContext())
            {
                //FIXME - Add null checking
                Semester semester = context.Semesters.FirstOrDefault(p => p.Term == term && p.Year == year);
                context.Entry(semester).Collection(semester => semester.Majors).Load();
                foreach (var major in semester.Majors)
                {
                    if (major.Name == majorName)
                    {
                        context.Entry(major).Collection(major => major.MajorOutcomes).Load();
                        foreach (var majorOutcome in major.MajorOutcomes)
                        {
                            if (majorOutcome.Name == outcomeName)
                            {
                                majorOutcome.Name = NewValue.Name;
                                majorOutcome.Description = NewValue.Description;
                                context.SaveChanges();
                                return;
                            }
                        }
                    }
                }
            }
        }

        public static void DeleteMajorOutcome(string term, int year, string majorName, string outcomeName)
        {
            using (var context = new ABETDBContext())
            {
                //FIXME - Add null checking
                Semester semester = context.Semesters.FirstOrDefault(p => p.Term == term && p.Year == year);
                context.Entry(semester).Collection(semester => semester.Majors).Load();
                foreach (var major in semester.Majors)
                {
                    if (major.Name == majorName)
                    {
                        context.Entry(major).Collection(major => major.MajorOutcomes).Load();
                        foreach (var majorOutcome in major.MajorOutcomes)
                        {
                            if (majorOutcome.Name == outcomeName)
                            {
                                context.Remove(majorOutcome);
                                context.SaveChanges();
                                return;
                            }
                        }
                    }
                }
            }
        }
    }
}
