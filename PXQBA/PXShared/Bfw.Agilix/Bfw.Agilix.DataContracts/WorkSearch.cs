namespace Bfw.Agilix.DataContracts
{
    public class WorkSearch
    {
        //ID of entity (course or section) for which to get the list of work
        public string EntityId { get; set; }

        //Optional date that specifies the limit to look back in history from now for work items. 
        //If omitted, all work items are returned.
        public string Date { get; set; }

        //When true only work that needs grading. The default is false. 
        public bool OutStanding { get; set; }

        // When true, returns work for all enrollments, regardless of enrollment status.
        //When false, returns grades for only Active or Suspended enrollments. The default is false.
        public bool AllStatus { get; set; }
    }
}
