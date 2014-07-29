using System.ComponentModel;

namespace Macmillan.PXQBA.Common.Models
{
    /// <summary>
    /// File type to import questions
    /// </summary>
    public enum QuestionFileType
    {
        [Description(".txt")]
        Respondus,
        [Description(".qti")]
        QTI,
        [Description(".qml")]
        QML
    }
}
