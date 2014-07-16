using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Macmillan.PXQBA.Business.QuestionParserModule.Respondus
{
    internal static class ElementPattern
    {
        public const string QuestionBlock = @"((^)\d\.[ ][\s\S]*?(?=^\d\.[ ]))";//(^\d[.][ ][\s\S]*)";//(^\d+[.][ ].*[(].* point.*[)][\s\S])";
        public const string Title = @"^\d[.][ ].*";
        public const string Points = @"[(](.*)[ ]point[s]?[)]";
        public const string GeneralFeedback = @"General Feedback:.*";
        public const string Feedback = @"Feedback:.*";
        public const string CorrectAnswer = @"Correct Answer:.*";
        public const string CorrectAnswers = @"Correct Answer\(s\):.*";
        public const string Choice = @"^([*]?)([a-z|A-Z]*)[.|)][ ](.*)";
        public const string ChoiceId = @"^[*]?[a-z|A-Z]*[.|)][ ]";
        public const string MatchingChoice = @"^\[([a-z|A-Z]*)\]{1}[ ](\d*)[.][ ](.*)";
        public const string MatchingChoiceId = @"^[*]?[a-z|A-Z]*[.|)][ ]";
        public const string CorrectChoice = "*";
    }
}
