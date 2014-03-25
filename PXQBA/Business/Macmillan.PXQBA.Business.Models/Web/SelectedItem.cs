using System.Collections.Generic;

namespace Macmillan.PXQBA.Business.Models.Web
{
    /// <summary>
    /// Represents selected list item
    /// </summary>
    public class SelectedItem
    {
        public SelectedItem()
        {
            Text = string.Empty;
            Id = string.Empty;
        }

        public SelectedItem(string text, string id, params string[] additionalInfo)
        {
            Text = text;
            Id = id;

            AdditionalInfo.AddRange(additionalInfo);
        }

        /// <summary>
        /// Gets or sets text of the selected item
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// Gets or sets id of the selected item
        /// </summary>
        public string Id { get; set; }

        private List<string> additionalInfo = EmptyList;
        /// <summary>
        /// Gets or sets the additional info.
        /// </summary>
        public List<string> AdditionalInfo
        {
            get
            {
                return additionalInfo ?? EmptyList;
            }
            set
            {
                additionalInfo = value;
            }
        }

        /// <summary>
        /// Gets the additional info by index.
        /// </summary>
        /// <param name="index">The index. Default value = 0</param>
        /// <returns>Additional info</returns>
        public string GetAdditionalInfoByIndex(int index = 0)
        {
            if (index >= 0 && AdditionalInfo.Count > index)
            {
                return AdditionalInfo[index];
            }
            return string.Empty;
        }

        public static List<string> EmptyList
        {
            get
            {
                return new List<string>();
            }
        }
    }
}