using System.Collections.Generic;

namespace AbetApi.Models
{
    public class StudentOutcomesCompleted
    {

        public static List<Dictionary<string, string>> ConvertToModelStudentOutcomesCompleted()
        {
            List<Dictionary<string, string>> tempList = new List<Dictionary<string, string>>();

            Dictionary<string, string> tempDictionary = new Dictionary<string, string>();

            tempDictionary.Add("outcomeName", "1");
            tempDictionary.Add("outcomeDescription", "Describe how a computer's CPU, Main Memory, Secondary Storage and I/O work together to execute a computer program.");
            tempDictionary.Add("IT", "20");
            tempDictionary.Add("CS", "30");
            tempDictionary.Add("CE", "40");
            tempDictionary.Add("CYS", "10");

            tempList.Add(tempDictionary);

            tempDictionary = new Dictionary<string, string>();

            tempDictionary.Add("outcomeName", "2");
            tempDictionary.Add("outcomeDescription", "Make use of a computer system's hardware, editor(s), operating system, system software and network to build computer software and submit that software for grading.");
            tempDictionary.Add("IT", "20");
            tempDictionary.Add("CS", "30");
            tempDictionary.Add("CE", "40");
            tempDictionary.Add("CYS", "10");

            tempList.Add(tempDictionary);

            return tempList;
        }
    }
}
