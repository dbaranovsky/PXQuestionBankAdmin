using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Macmillan.PXQBA.Business.Models
{
    public enum NotificationType
    {
        [Description(@"Editing a question without first creating a draft means that your edits will be immediately available in the question. 
                        You will be able to restore an earlier version of the question within the History tab.")]
        EditInPlaceQuestionInProgress = 0,

        [Description(@"You can safely make edits to this draft without affecting the question that is currently available to instructors. 
                        Once you are satisfied with your edits, you’ll be able to publish the draft back into the original question.")]
        NewDraftForAvailableToInstructors = 1,

        [Description(@"You are about to publish a draft. The edits reflected in this draft will replace all content in the original question. 
                        Click Proceed to continue with the publish process.")]
        PublishChangesMadeWithinDraft = 2,


    }
}
