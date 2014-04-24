using System.Collections.Generic;
using Macmillan.PXQBA.Business.Models;

namespace Macmillan.PXQBA.Business.Contracts
{
    public interface IQuestionMetadataService
    {
        IList<QuestionMetaField> GetAvailableFields(Course course);

        IList<QuestionMetaField> GetDataForFields(Course course, IEnumerable<string> fieldsNames);

        string GetQuestionCardLayout(Course course);
    }

}
