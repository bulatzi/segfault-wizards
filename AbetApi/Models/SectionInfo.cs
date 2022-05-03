namespace AbetApi.Models
{
    //This class is used to package specific data to be sent to the front end, for relevant course info
    public class SectionInfo
    {
        public string courseFriendlyName { get; set; }
        public string courseNumber { get; set; }
        public string sectionNumber { get; set; }
        public string instructorEUID { get; set; }
        public string coordinatorEUID { get; set; }

        public SectionInfo(string courseFriendlyName, string courseNumber, string sectionNumber, string instructorEUID, string coordinatorEUID)
        {
            this.courseFriendlyName = courseFriendlyName;
            this.courseNumber = courseNumber;
            this.sectionNumber = sectionNumber;
            this.instructorEUID = instructorEUID;
            this.coordinatorEUID = coordinatorEUID;
        }
    }
}
