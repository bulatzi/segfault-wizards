using System.Collections.Generic;
using System.Threading.Tasks;
using AbetApi.EFModels;
using AbetApi.Data;
using System.Linq;
using System;

namespace AbetApi.Models
{
    public class FullReport
    {
        public static Dictionary<string, Dictionary<string, string[]>> GenerateFullReport(string term, int year)
        {
            //Step 1: Sort all data in to an array of course outcomes completed. Go major : course : array
            //Translate each courseOutcomeName in to a 0 indexed number to decide where the data goes
            //When you set StudentOutcomesCompleted, verify that all outcome names can be converted to ints.
            //Step 2: By the same organization, create arrays of all 0's.
            //Step 3: Translate the first array in to the second array. One courseOutcome can map to multiple majorOutcomes
            //Find the column's that the number needs to be added to, and add it to all appropriate columns
            //Step 4: Return a list of Dictionary<string, int[]>

            ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            //This block of code creates a blank container to read data in to for data aggregation

            //Pulls a list of all StudentOutcomesCompleted. (includes all sections for every class)
            List<AbetApi.EFModels.StudentOutcomesCompleted> studentOutcomesCompletedList = AbetApi.EFModels.StudentOutcomesCompleted.GetSemesterStudentOutcomesCompleted(term, year).Result;

            //Pulls a list of all majors
            List<AbetApi.EFModels.Major> majorsList = AbetApi.EFModels.Major.GetMajors(term, year).Result;

            //Pulls a list of all courses in the semester
            List<AbetApi.EFModels.Course> courseList = AbetApi.EFModels.Course.GetCourses(term, year).Result;

            //This is an arbitrary array size that represents the number of columns displayed in the "Full report". After discussions with the sponsor, this number is a hard coded 10. (the current max number of major outcomes is 7)
            const int majorOutcomeColumns = 10;

            //Creates the container for data aggregation
            Dictionary<string, Dictionary<string, int[]>> aggregationData = new Dictionary<string, Dictionary<string, int[]>>();
            Dictionary<string, Dictionary<string, float[]>> calculatedData = new Dictionary<string, Dictionary<string, float[]>>();
            Dictionary<string, Dictionary<string, string[]>> calculatedStringData = new Dictionary<string, Dictionary<string, string[]>>();

            //This is for storing compensation multiplier for student count in a course
            //This is necessary because... it's possible to map multiple course outcomes to the same major outcome. When that happens, if the number of students is 30, now you have to calculate that value as if total students was 60.
            //If you don't account for this, the value can exceed 100%, which is obviously wrong
            Dictionary<string, Dictionary<string, int[]>> studentCountCompensation = new Dictionary<string, Dictionary<string, int[]>>();

            //Creates the first layer of dictionaries, which are each of the majors
            foreach (var major in majorsList)
            {
                aggregationData.Add(major.Name, new Dictionary<string, int[]>());
                calculatedData.Add(major.Name, new Dictionary<string, float[]>());
                calculatedStringData.Add(major.Name, new Dictionary<string, string[]>());
                studentCountCompensation.Add(major.Name, new Dictionary<string, int[]>());
            }

            //Creates the second layer of dictionaries, which are all of the courses for each major
            //populate the dictionaries used to calculate data
            foreach (var majorDictionary in aggregationData)
            {
                foreach (var course in courseList)
                {
                    majorDictionary.Value.Add(course.CourseNumber, new int[majorOutcomeColumns]);
                }
            }

            //populate the dictionaries used to store the summed data
            foreach (var majorDictionary in calculatedData)
            {
                foreach (var course in courseList)
                {
                    majorDictionary.Value.Add(course.CourseNumber, new float[majorOutcomeColumns]);

                    //Fill each array with -1
                    //-1 represents that the array hasn't been mapped to anything, so that value can be replaced by a - in the report
                    for (int i = 0; i < majorOutcomeColumns; i++)
                    {
                        majorDictionary.Value[course.CourseNumber][i] = -1;
                    }
                }
            }

            //populate the dictionaries used to store the output strings of the function
            foreach (var majorDictionary in calculatedStringData)
            {
                foreach (var course in courseList)
                {
                    majorDictionary.Value.Add(course.CourseNumber, new string[majorOutcomeColumns]);
                }
            }

            //populate the dictionaries used to compensate for total student count
            foreach(var majorDictionary in studentCountCompensation)
            {
                foreach(var course in courseList)
                {
                    majorDictionary.Value.Add(course.CourseNumber, new int[majorOutcomeColumns]);

                    //populate that int array with all 0's
                    for(int i = 0; i < majorDictionary.Value.Count; i++)
                    {
                        majorDictionary.Value[course.CourseNumber][i] = 0;
                    }
                }
            }

            ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            //This block of code will iterate through the list of student outcomes completed and sum them, organized by major->course->courseOutcome

            //sort all of the studentOutcomesCompleted in to appropriate arrays
            foreach (var studentOutcomesCompleted in studentOutcomesCompletedList)
            {
                //Find the zero indexed column each number should go in to
                int column;
                Int32.TryParse(studentOutcomesCompleted.CourseOutcomeName, out column);
                column--;

                //Store that data in the appropriate column. (e.g. outcome 1 should be stored in index 0 of the array)
                aggregationData[studentOutcomesCompleted.MajorName][studentOutcomesCompleted.CourseNumber][column] += studentOutcomesCompleted.StudentsCompleted;
            }

            ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            //Now the data for course outcomes is stored in the aggregationData object.
            //We need to map each column of course outcome data to its respective linked majorOutcomes. 

            //for each column
            //get the outcome name via... taking the index, adding one, and converting it to string
            //Send that outcome name in to the mapCoursetoMajorOutcome() function.
            //Take the list it returns, and convert each one in to integers again -1, to zero index.
            //For each item in that list, add the initial index to each column in the other object
            //If the number is zero, you can skip

            foreach (var majorDictionary in aggregationData)
            {
                string major = majorDictionary.Key;
                foreach (var courseDictionary in majorDictionary.Value)
                {
                    for (int i = 0; i < courseDictionary.Value.Length; i++)
                    {
                        //Take the index
                        //+1 it
                        //convert it to string
                        //pass it in that map function and return a list of strings
                        //convert those strings back in to 0 indexed ints
                        //take the data at the index and add it to all indexes in that other list (but in the new calculatedData data structure)

                        string tempString = (i + 1).ToString();
                        var columnsToAdd = MapCourseOutcomeToMajorOutcome(term, year, courseDictionary.Key, tempString, major).Result;

                        if (columnsToAdd != null)
                        {
                            //This next section takes one-indexed string names of columns and converts them to zero-indexed ints
                            List<int> intColumns = new List<int>();
                            foreach (var columnString in columnsToAdd)
                            {
                                //convert to int
                                int tempInt;
                                Int32.TryParse(columnString, out tempInt);
                                tempInt--;
                                intColumns.Add(tempInt);
                            }

                            //This section increments the values of the student count compensation arrays
                            
                            foreach(var column in intColumns)
                            {
                                studentCountCompensation[major][courseDictionary.Key][column]++;
                            }

                            for (int j = 0; j < intColumns.Count; j++)
                            {
                                //This line re-baselines the number to 0 if appropriate, or it adds the number to the calculated column
                                if (calculatedData[major][courseDictionary.Key][intColumns[j]] == -1)
                                    calculatedData[major][courseDictionary.Key][intColumns[j]]++;
                                calculatedData[major][courseDictionary.Key][intColumns[j]] += aggregationData[major][courseDictionary.Key][i];
                            }
                        }
                    }
                }
            }

            //Iterate through all calculated data arrays, and convert existing values in to percentages.
            ////////////////////////////////////////////////////////////////////////////////////////////////////////////// 

            foreach (var majorDictionary in aggregationData)
            {
                string major = majorDictionary.Key;
                foreach (var courseDictionary in majorDictionary.Value)
                {
                    for (int i = 0; i < courseDictionary.Value.Length; i++)
                    {
                        //I need to replace columns to add with... just scan through everything. Anything that isn't -1, do the calculation on it
                        //This section doesn't need the columns to add stuff because we've already done the adding. Now, we only need to convert totaled major outcomes in to percentages

                        if (calculatedData[major][courseDictionary.Key][i] != -1)
                        {
                            //Get the total number of students in the course
                            int studentCount = GetTotalStudents(term, year, courseDictionary.Key, major);

                            //If applicable, convert each column in to a percentage
                            if (studentCount == 0)
                            {
                                calculatedData[major][courseDictionary.Key][i] = 0;
                            }
                            else
                            {
                                //This line replaces each count with its percentage equivalent, corrected for student count
                                calculatedData[major][courseDictionary.Key][i] = (calculatedData[major][courseDictionary.Key][i] / (studentCount * studentCountCompensation[major][courseDictionary.Key][i])) * 100;
                            }
                        }
                    }
                }
            }

            ///////////////////////////////////////////////////////////////////////////////////////////////////////////////


            //if -1, it means there are no linked course outcomes associated with that major outcome, so it replaces it with "--"
            foreach (var major in calculatedData)
            {
                foreach (var course in major.Value)
                {
                    for (int i = 0; i < course.Value.Length; i++)
                    {
                        if (calculatedData[major.Key][course.Key][i] == -1)
                            calculatedStringData[major.Key][course.Key][i] = "--";
                        else
                        calculatedStringData[major.Key][course.Key][i] = calculatedData[major.Key][course.Key][i].ToString("0.0");
                    }
                }
            }

            return calculatedStringData;
        }

        //This function takes a course outcome name, and returns a list of associated major outcomes.
        private static async Task<List<string>> MapCourseOutcomeToMajorOutcome(string term, int year/*, string courseDepartment*/, string courseNumber, string courseOutcomeName, string majorName)
        {
            //Find the course
            //Find the course outcomes
            //Find the specific course outcome
            //load major outcomes
            //find all linked major outcomes
            //Stash the major outcome name aside somewhere, so you don't have to backtrack again
            //iterate through those major outcomes and verify that the major is for the expected majorName
            //If yes, replace the courseOutcomeName with the majorOutcomeName

            await using (var context = new ABETDBContext())
            {
                //FIXME - Add null checking for this entire function. Lots of places to not find things.
                //Finds the semester
                Semester semester = context.Semesters.FirstOrDefault(p => p.Term == term && p.Year == year);

                //Finds the course
                context.Entry(semester).Collection(semester => semester.Courses).Load();
                foreach (var course in semester.Courses)
                {
                    //if it finds the course, Find all existing course outcomes
                    if (/*course.Department == courseDepartment && */course.CourseNumber == courseNumber)
                    {
                        //Loads existing course outcomes
                        context.Entry(course).Collection(course => course.CourseOutcomes).Load();

                        //Find the relevant course outcome
                        foreach (var courseOutcome in course.CourseOutcomes)
                        {
                            //If found, map it back to relevant major outcomes
                            if (courseOutcome.Name == courseOutcomeName)
                            {
                                //Load appropriate major outcomes
                                context.Entry(courseOutcome).Collection(courseOutcome => courseOutcome.MajorOutcomes).Load();

                                //Load all majors
                                foreach (var majorOutcome in courseOutcome.MajorOutcomes)
                                {
                                    context.Entry(majorOutcome).Collection(majorOutcome => majorOutcome.Majors).Load();
                                }

                                List<string> relevantMajorOutcomes = new List<string>();

                                //Scan through major outcomes, and validate that the major it belongs to is the one we're looking for
                                foreach (var majorOutcome in courseOutcome.MajorOutcomes)
                                {
                                    //FIXME - make this account for not finding a major
                                    if (majorOutcome.Majors.First().Name == majorName)
                                    {
                                        relevantMajorOutcomes.Add(majorOutcome.Name);
                                    }
                                }

                                return relevantMajorOutcomes;
                            }
                        }
                    }
                }
                return null;
            }
        }

        //This function returns the total number of students for the provided major for all sections of the selected course
        //It returns 0 if the major does not have a grade entered for that section
        private static int GetTotalStudents(string term, int year, string courseNumber, string majorName)
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

            //Check if the course number is null or empty.
            if (courseNumber == null || courseNumber == "")
            {
                throw new ArgumentException("The course number cannot be empty.");
            }

            using (var context = new ABETDBContext())
            {
                Course tempCourse = null;

                //Find the semester/course the section will belong to
                Semester semester = context.Semesters.FirstOrDefault(p => p.Term == term && p.Year == year);

                //Check if the semester is null.
                if (semester == null)
                {
                    throw new ArgumentException("The specified semester does not exist in the database.");
                }

                //Finds the relevant course
                context.Entry(semester).Collection(semester => semester.Courses).Load();
                foreach (var course in semester.Courses)
                {
                    if (course.CourseNumber == courseNumber)
                    {
                        tempCourse = course;
                        break;
                    }
                }

                //Check if course is null.
                if (tempCourse == null)
                {
                    throw new ArgumentException("The specified course does not exist in the database.");
                }

                //Load the sections under the course specified.
                context.Entry(tempCourse).Collection(tempCourse => tempCourse.Sections).Load();

                int totalStudents = 0;

                //For each section
                foreach (var section in tempCourse.Sections)
                {

                    //Loads existing grades for that section
                    context.Entry(section).Collection(section => section.Grades).Load();

                    //scan through the grades, and find the one for the appropriate major
                    foreach (var grade in section.Grades)
                    {
                        if (grade.Major == majorName)
                        {
                            totalStudents += grade.TotalStudents;
                        }
                    }
                }
                return totalStudents;
            }
            return 0;
        }
    }
}
