using System.Collections.Generic;

namespace AbetApi.Models
{
    public class FullReport
    {
        public static Dictionary<string, int[]> ConvertToModelFullReport()
        {
            //List<Dictionary<string, int[]>> tempList = new List<Dictionary<string, int[]>>();

            Dictionary<string, int[]> tempdictionary = new Dictionary<string, int[]>();

            int[] tempArray = new int[] { 0,5,0,0,1,0,0,1,0,1};

            tempdictionary.Add("1010", tempArray);
            tempdictionary.Add("1030", tempArray);
            tempdictionary.Add("2010", tempArray);
            tempdictionary.Add("2030", tempArray);

            return tempdictionary;
        }
    }
}
