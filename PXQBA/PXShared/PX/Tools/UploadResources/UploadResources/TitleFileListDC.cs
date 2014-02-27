using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using UploadResources.DataAccess;


namespace UploadResources.DataContainer
{
    public class TitleFileListDC : BaseDC
    {

        public int TitleID { get; set; }       
        public string SourceFilePath { get; set; }
        public string TargetFilePath { get; set; }
        public DataSet TitleFileListDataSet;


    }
}