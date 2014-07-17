using System.ComponentModel;

namespace Macmillan.PXQBA.Business.QuestionParserModule.DataContracts
{
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
