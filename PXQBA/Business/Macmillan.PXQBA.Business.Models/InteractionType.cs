using System.ComponentModel;

namespace Macmillan.PXQBA.Business.Models
{
    /// <summary>
    /// Types of Question interactions.
    /// </summary>
    public enum InteractionType
    {
        Bank = -1,
        Choice = 0,
        Match = 1,
        Answer = 2,
        Text = 3,
        Essay = 4,
        Composite = 5,
        Custom = 6,
        NotBank = 7
    }
}