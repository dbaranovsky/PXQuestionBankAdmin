namespace Bfw.Agilix.DataContracts
{
    /// <summary>
    /// Attachment contract
    /// </summary>
    /// 
    public class Attachment
    {
        /// <summary>
        /// Path to the attachment
        /// </summary>
        public string Href { get; set; }

        /// <summary>
        /// Name of the attachment
        /// </summary>
        public string Name { get; set; }
    }
}