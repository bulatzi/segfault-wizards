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

        public MajorOutcome()
        {
            this.Majors = new List<Major>();
            this.CourseOutcomes = new List<CourseOutcome>();
        }
        public MajorOutcome(string Name, string description)
        {
            this.Name = Name;
            this.Description = description;
        }

        public static async Task AddMajorOutcome(string term, int year, string majorName, MajorOutcome majorOutcome)
        {
            // Sets the major outcome id to be 0, so entity framework will give it a primary key
            majorOutcome.MajorOutcomeId = 0;

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

            //Check if the major name is null or empty.
            if (majorName == null || majorName == "")
            {
                throw new ArgumentException("The major name cannot be empty.");
            }

            //Check if the major outcome designation is null or empty.
            if(majorOutcome.Name == null || majorOutcome.Name == "")
            {
                throw new ArgumentException("The major outcome identifier cannot be empty.");
            }

            //Check if the major outcome description is null or empty.
            if(majorOutcome.Description == null || majorOutcome.Description == "")
            {
                throw new ArgumentException("The major outcome description cannot be empty.");
            }

            //Format term and major name to follow a standard.
            term = term[0].ToString().ToUpper() + term.Substring(1);
            majorName = majorName[0].ToString().ToUpper() + majorName.Substring(1);

            await using (var context = new ABETDBContext())
            {
                Major tempMajor = null;

                //Try to find the specified semester in the database.
                Semester semester = context.Semesters.FirstOrDefault(p => p.Term == term && p.Year == year);

                //If it does not exist, throw an exception.
                if(semester == null)
                {
                    throw new ArgumentException("The specified semester does not exist in the database.");
                }

                //Load the majors under the specified semester.
                context.Entry(semester).Collection(semester => semester.Majors).Load();

                //Loop through the majors and look for the specified major.
                foreach (var major in semester.Majors)
                {
                    if (major.Name == majorName)
                    {
                        tempMajor = major;
                    }
                }

                //Check if major is null.
                if(tempMajor == null)
                {
                    throw new ArgumentException("The specified major does not exist in the database.");
                }

                //Load the major outcomes under the major specified.
                context.Entry(tempMajor).Collection(major => major.MajorOutcomes).Load();

                //Try to find the new major outcome in the database.
                foreach(MajorOutcome duplicateMajorOutcome in tempMajor.MajorOutcomes)
                {
                    //If we do find the major outcome already in the database, that is a duplicate and we do not allow duplicates.
                    if (duplicateMajorOutcome.Name == majorOutcome.Name)
                    {
                        throw new ArgumentException("That major outcome already exists in the database.");
                    }
                }

                //Add the major outcome to the major outcome table, and the major join table, then save changes.
                context.MajorOutcomes.Add(majorOutcome);
                tempMajor.MajorOutcomes.Add(majorOutcome);

                context.SaveChanges();
            }
        } // AddMajorOutcome

        public static async Task<MajorOutcome> GetMajorOutcome(string term, int year, string majorName, string outcomeName)
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

            //Check if the major name is null or empty.
            if (majorName == null || majorName == "")
            {
                throw new ArgumentException("The major name cannot be empty.");
            }

            //Check if the major outcome designation is null or empty.
            if (outcomeName == null || outcomeName == "")
            {
                throw new ArgumentException("The major outcome identifier cannot be empty.");
            }

            //Format term and major name to follow a standard.
            term = term[0].ToString().ToUpper() + term.Substring(1);
            majorName = majorName[0].ToString().ToUpper() + majorName.Substring(1);

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
                foreach (var major in semester.Majors)
                {
                    if (major.Name == majorName)
                    {
                        //Load the major outcomes under the specified major.
                        context.Entry(major).Collection(major => major.MajorOutcomes).Load();

                        //Loop through the major outcomes and look for the specified major outcome.
                        foreach (var majorOutcome in major.MajorOutcomes)
                        {
                            if (majorOutcome.Name == outcomeName)
                            {
                                return majorOutcome;
                            }
                        }
                        throw new ArgumentException("The specified major outcome does not exist in the database.");
                    }
                }
                throw new ArgumentException("The specified major does not exist in the database.");
            }
        } // GetMajorOutcome

        public static async Task EditMajorOutcome(string term, int year, string majorName, string outcomeName, MajorOutcome NewValue)
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

            //Check if the major name is null or empty.
            if (majorName == null || majorName == "")
            {
                throw new ArgumentException("The major name cannot be empty.");
            }

            //Check if the major outcome designation is null or empty.
            if (outcomeName == null || outcomeName == "")
            {
                throw new ArgumentException("The major outcome identifier cannot be empty.");
            }

            //Check if the new major outcome designation is null or empty.
            if (NewValue.Name == null || NewValue.Name == "")
            {
                throw new ArgumentException("The new major outcome identifier cannot be empty.");
            }

            //Check if the new major outcome description is null or empty.
            if (NewValue.Description == null || NewValue.Description == "")
            {
                throw new ArgumentException("The new major outcome description cannot be empty.");
            }

            //Format term and major name to follow a standard.
            term = term[0].ToString().ToUpper() + term.Substring(1);
            majorName = majorName[0].ToString().ToUpper() + majorName.Substring(1);

            await using (var context = new ABETDBContext())
            {
                Major tempMajor = null;
                MajorOutcome tempMajorOutcome = null;

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
                foreach (var major in semester.Majors)
                {
                    if (major.Name == majorName)
                    {
                        tempMajor = major;
                        break;
                    }
                }

                //Check if major is null.
                if(tempMajor == null)
                {
                    throw new ArgumentException("The specified major does not exist in the database.");
                }

                //Load the major outcomes under the specified major.
                context.Entry(tempMajor).Collection(major => major.MajorOutcomes).Load();

                //Loop through the major outcomes and look for the specified major outcome.
                foreach(MajorOutcome majorOutcome in tempMajor.MajorOutcomes)
                {
                    //If we are trying to change a major outcome's identifier to an already existing major outcome identifier, that is a duplicate and we do not allow duplicates.                    
                    if (outcomeName != NewValue.Name && majorOutcome.Name == NewValue.Name)
                    {
                        throw new ArgumentException("The new major outcome identifier already exists in the database.");
                    }

                    //Find the major outcome specified to edit.
                    if (majorOutcome.Name == outcomeName)
                    {
                        tempMajorOutcome = majorOutcome;
                    }
                }

                //Check if major outcome is null.
                if(tempMajorOutcome == null)
                {
                    throw new ArgumentException("The specified major outcome to edit does not exist in the database.");
                }

                tempMajorOutcome.Name = NewValue.Name;
                tempMajorOutcome.Description = NewValue.Description;

                context.SaveChanges();
            }
        } // EditMajorOutcome

        // WILL GIVE A 200 SUCCESS IN MAJORNAME AND OUTCOMENAME, EVEN IF IT DOESN"T EXIST OR THE VALUE IS NULL
        public static async Task DeleteMajorOutcome(string term, int year, string majorName, string outcomeName)
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

            //Check if the major name is null or empty.
            if (majorName == null || majorName == "")
            {
                throw new ArgumentException("The major name cannot be empty.");
            }

            //Check if the major outcome designation is null or empty.
            if (outcomeName == null || outcomeName == "")
            {
                throw new ArgumentException("The major outcome identifier cannot be empty.");
            }

            //Format term and major name to follow a standard.
            term = term[0].ToString().ToUpper() + term.Substring(1);
            majorName = majorName[0].ToString().ToUpper() + majorName.Substring(1);

            await using (var context = new ABETDBContext())
            {
                Major tempMajor = null;
                MajorOutcome tempMajorOutcome = null;

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
                foreach (var major in semester.Majors)
                {
                    if (major.Name == majorName)
                    {
                        tempMajor = major;
                    }
                }

                //Check if major is null.
                if (tempMajor == null)
                {
                    throw new ArgumentException("The specified major does not exist in the database.");
                }

                //Load the major outcomes under the major specified.
                context.Entry(tempMajor).Collection(major => major.MajorOutcomes).Load();

                //Try to find the new major outcome in the database.
                foreach (MajorOutcome majorOutcome in tempMajor.MajorOutcomes)
                {
                    //If we do find the major outcome, then remove it.
                    if (majorOutcome.Name == outcomeName)
                    {
                        tempMajorOutcome = majorOutcome;
                        break;
                    }
                }

                //Check if major outcome is null.
                if(tempMajorOutcome == null)
                {
                    throw new ArgumentException("The specified major outcome does not exist in the database.");
                }

                context.Remove(tempMajorOutcome);
                context.SaveChanges();
            }
        } // DeleteMajorOutcome
    } // MajorOutcome
}
