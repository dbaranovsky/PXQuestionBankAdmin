using System.Collections.Generic;
using Macmillan.PXQBA.Business.Models.Web;

namespace Macmillan.PXQBA.Business.Contracts
{
    public interface IQuestionMetadataService
    {
        IList<QuestionFieldDescriptor> GetAvailableFields();

        IList<QuestionFieldDescriptor> GetDataForFields(IEnumerable<string> fieldsNames);

    }

}
