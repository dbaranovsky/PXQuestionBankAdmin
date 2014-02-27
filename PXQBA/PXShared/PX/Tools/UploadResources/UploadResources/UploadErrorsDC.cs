using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using UploadResources.DataAccess;

namespace UploadResources.DataContainer
{
    public class UploadErrorsDC : BaseDC
    {
        public string TargetPath { get; set; }
        public string SourcePath { get; set; }
        public string Message { get; set; }
    }
}