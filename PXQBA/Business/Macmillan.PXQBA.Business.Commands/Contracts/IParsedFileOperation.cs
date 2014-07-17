using Macmillan.PXQBA.Business.Models;

namespace Macmillan.PXQBA.Business.Commands.Contracts
{
    public interface IParsedFileOperation
    {
        int AddParsedFile(string fileName, string questionsData);

        int SetParsedFileStatus(int id, ParsedFileStatus status);

        string GetParsedFile(int id);
    }
}