﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bfw.PX.Biz.DataContracts
{
[Serializable]
    /// <summary>
    /// Class holding settings for the tinyMCE editor 
    /// </summary>
    public class tinyMCE
    {
        /// <summary>
        /// Name of the editor's configuration
        /// </summary>
        public string EditorOptions { get; set; }
    }
}
