namespace Macmillan.PXQBA.Business.Models
{
    /// <summary>
    /// Model for available item in dropdown
    /// </summary>
    public class AvailableChoiceItem
    {
        public AvailableChoiceItem()
        {
        }

        public AvailableChoiceItem(string value, string text)
        {
            Value = value;
            Text = text;
        }

        public AvailableChoiceItem(string value)
        {
            Text = value;
            Value = value;
        }

        /// <summary>
        /// Item display text
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// Item value
        /// </summary>
        public string Value { get; set; }
    }
}
