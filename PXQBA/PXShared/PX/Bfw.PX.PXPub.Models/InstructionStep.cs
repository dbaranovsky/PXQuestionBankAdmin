using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.XPath;
using System.Xml;
using System.Xml.Linq;
using System.Text;

namespace Bfw.PX.PXPub.Models
{
    /// <summary>
    /// Represents a Quiz
    /// </summary>
    public class InstructionStep
    {
        public string Title { get; set; }
        public string Instructions { get; set; }
        public InstructionStepMedia Media { get; set; }
        public int Index { get; set; }

        public InstructionStep()
        {
            Media = new InstructionStepMedia();
        }
    }

    public class InstructionStepMedia
    {
        public string Url { get; set; }
        public string Height { get; set; }
        public string Width { get; set; }
    }

    public class Tutorial
    {
        public List<InstructionStep> Steps { get; set; }
        public string Title { get; set; }
        public string Id { get; set; }
        public Tutorial()
        {

        }
    }
}
