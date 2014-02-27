namespace Bfw.Agilix.DataContracts
{
    /// <summary>
    /// Parameters used to filter tasks.
    /// </summary>
    public class TaskSearch
    {
        /// <summary>
        /// Id of the task to find.
        /// </summary>
        public string TaskId { get; set; }

        /// <summary>
        /// Command string to find.
        /// </summary>
        public string Command { get; set; }
    }
}
