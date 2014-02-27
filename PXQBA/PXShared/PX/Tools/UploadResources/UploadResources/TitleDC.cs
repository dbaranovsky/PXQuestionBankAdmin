using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using UploadResources.DataAccess;


namespace UploadResources.DataContainer
{
    public class TitleDC : BaseDC
    {
        public TitleDC()
        {
            ProcessingErrors = new List<UploadErrorsDC>();
        }
        public int TitleID { get; set; }
        public int? IsFileList { get; set; }
        public string TargetPath { get; set; }
        public string SourcePath { get; set; }        
        public string Environment { get; set; }
        public string TargetEntityId { get; set; }
        public DateTime ProcessingStartTime { get; set; }
        public DateTime ProcessingEndTime { get; set; }
        public int FileCount { get; set; }
        public string FileSizeString { get; set; }
        public List<UploadErrorsDC> ProcessingErrors;
        public DataSet TitleDataSet;


    }
}