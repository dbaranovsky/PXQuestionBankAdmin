using System.Collections.Generic;

namespace Macmillan.PXQBA.Business.Commands.Contracts
{
    public interface IKeywordOperation
    {
        IEnumerable<string> GetKeywordList(string courseId, string fieldName);

        void AddKeywords(string courseId, string fieldName, IEnumerable<string> keywords);
    }
}