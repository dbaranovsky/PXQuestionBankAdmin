using System.Collections.Generic;
using Macmillan.PXQBA.Business.Models;

namespace Macmillan.PXQBA.Business.Contracts
{
    public interface IQuestionMetadataService
    {
        IList<QuestionMetaField> GetAvailableFields();

        IList<QuestionMetaField> GetDataForFields(IEnumerable<string> fieldsNames);

    }

}
