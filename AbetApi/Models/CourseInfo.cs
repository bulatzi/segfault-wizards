namespace AbetApi.Models
{
    //This class is used to package specific data to be sent to the front end, for relevant course info
    public class CourseInfo
    {
        public string courseFriendlyName { get; set; }
        public string courseNumber { get; set; }
        public string coordinatorEUID { get; set; }

        public CourseInfo(string courseFriendlyName, string courseNumber, string coordinatorEUID)
        {
            this.courseFriendlyName = courseFriendlyName;
            this.courseNumber = courseNumber;
            this.coordinatorEUID = coordinatorEUID;
        }
    }
}
