using Macmillan.PXQBA.Business.Models;

namespace Macmillan.PXQBA.Business.Commands.Contracts
{
    public interface IParsedFileOperation
    {
        long AddParsedFile(string fileName, string questionsData, byte[] resourcesData);

        long SetParsedFileStatus(long id, ParsedFileStatus status);

        ParsedFile GetParsedFile(long id);
    }
}