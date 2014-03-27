using Macmillan.PXQBA.Business.Models;

namespace Macmillan.PXQBA.Business.Contracts
{
    public interface IModelProfileService
    {
        InteractionType CreateInteractionType(string questionType);
    }
}