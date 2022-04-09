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
        public static Dictionary<string, int[]> ConvertToModelFullReport()
        {
            string term = "Spring";
            int year = 2022;
            string majorName;

            //List<Dictionary<string, int[]>> tempList = new List<Dictionary<string, int[]>>();

            //Dictionary<string, int[]> tempdictionary = new Dictionary<string, int[]>();

            //int[] tempArray = new int[] { 0,5,0,0,1,0,0,1,0,1};

            //tempdictionary.Add("1010", tempArray);
            //tempdictionary.Add("1030", tempArray);
            //tempdictionary.Add("2010", tempArray);
            //tempdictionary.Add("2030", tempArray);

            //return tempdictionary;

            //Tasks
            //Convert course outcomes to major outcomes to figure out what goes in which columns
            //aggregate numbers across all sections

            //Pull CourseOutcomesCompleted for all sections
            //Take the provided major, 

            //Make a function that takes course information and a major name, finds its course outcomes, maps it back to majors. if that major is contained, it steals the number of the major outcome and returns it

            //As I translate course outcomes to major outcomes, maybe I can just overwrite the number of the course outcome for simplicity
            //Then, iterate over all objects. Take the array of 10, initialize all with 0s. 
            //If the number is still 0 at the end, replace it with a "-"

            //What I have - a function that takes a course outcome and tells me which one it should be on. (one indexed)
            //Lets assume that I have already total'd the numbers, and I just need to move them around

            //make a dictionary that is the outcome name, and an int
            //total up each outcome across sections
            //When done, run the outcome name through the map translator
            //take the dictionary, and hard code put numbers in the array
            //Add array to temp dictionary

            ///////////////////////////////////////////////////////////////////////////////////////////
            //pull all outcomesCompleted in
            //Dictionary<Dictionary<Dictionary<string, int>>> courseNumber : section : major : outcomesCompleted
            //Iterate over all objects, and total up all course outcomes completed
            //With all of the data organized... create a dictionary for each major

            //Step 1: Sort all data in to an array of course outcomes completed. Go major : course : array
                    //Translate each courseOutcomeName in to a 0 indexed number to decide where the data goes
                    //When you set StudentOutcomesCompleted, verify that all outcome names can be converted to ints.
            //Step 2: By the same organization, create arrays of all 0's.
            //Step 3: Translate the first array in to the second array. One courseOutcome can map to multiple majorOutcomes
                    //Find the column's that the number needs to be added to, and add it to all appropriate columns
            //Step 4: Return a list of Dictionary<string, int[]>

            //pull list of courses
            //FIXME - make it add a field for all courses, not just the ones with StudentOutcomesCompleted objects
            //List<AbetApi.EFModels.Course> courses = AbetApi.EFModels.Course.GetCourses(term, year).Result;

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
            Dictionary<string, Dictionary<string, int[]>> calculatedData = new Dictionary<string, Dictionary<string, int[]>>();

            //Creates the first layer of dictionaries, which are each of the majors
            foreach (var major in majorsList)
            {
                aggregationData.Add(major.Name, new Dictionary<string, int[]>());
                calculatedData.Add(major.Name, new Dictionary<string, int[]>());
            }

            //Creates the second layer of dictionaries, which are all of the courses for each major
            foreach(var majorDictionary in aggregationData)
            {
                foreach(var course in courseList)
                {
                    majorDictionary.Value.Add(course.CourseNumber, new int[majorOutcomeColumns]);
                }
            }

            foreach (var majorDictionary in calculatedData)
            {
                foreach (var course in courseList)
                {
                    majorDictionary.Value.Add(course.CourseNumber, new int[majorOutcomeColumns]);
                }
            }

            //calculatedData.Value.Add(course.CourseNumber, new int[majorOutcomeColumns]);

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

            //System.Console.WriteLine("");

            //Get a list of majors, create a dictionary for each of them
            //Get a list of courses, create a dictionary for each of them, in each major dictionary
            //Scan through the courseoutcomescompleted, translate each in to its appropriate array

            ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            //Now the data for course outcomes is stored in the aggregationData object.
            //We need to map each column of course outcome data to its respective linked majorOutcomes. 

            //for each column
            //get the outcome name via... taking the index, adding one, and converting it to string
            //Send that outcome name in to the mapCoursetoMajorOutcome() function.
            //Take the list it returns, and convert each one in to integers again -1, to zero index.
            //For each item in that list, add the initial index to each column in the other object
                //If the number is zero, you can skip

            foreach(var majorDictionary in aggregationData)
            {
                string major = majorDictionary.Key;
                foreach(var courseDictionary in majorDictionary.Value)
                {
                    for(int i = 0; i < courseDictionary.Value.Length; i++)
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

                            for (int j = 0; j < intColumns.Count; j++)
                            {
                                calculatedData[major][courseDictionary.Key][intColumns[j]] += aggregationData[major][courseDictionary.Key][i];
                            }
                        }
                    }
                }
            }

            System.Console.WriteLine("");
            /*
             To test:
                Make a semester
                Make at least 3 classes
                make at least 3 sections
                create 2 course outcomes for each course
                create 2 major outcomes for each major
                links course and major outcomes
                
             */



            ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            ///This is just here to shut the function up
            Dictionary<string, int[]> tempdictionary = new Dictionary<string, int[]>();

            //int[] tempArray = new int[] { 0,5,0,0,1,0,0,1,0,1};

            //tempdictionary.Add("1010", tempArray);
            //tempdictionary.Add("1030", tempArray);
            //tempdictionary.Add("2010", tempArray);
            //tempdictionary.Add("2030", tempArray);

            return tempdictionary;
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
    }
}
