namespace Macmillan.PXQBA.Business.Models
{
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

        public string Text { get; set; }

        public string Value { get; set; }
    }
}
