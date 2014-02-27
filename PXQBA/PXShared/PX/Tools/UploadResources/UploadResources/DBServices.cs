using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using UploadResources.Exception;
using UploadResources.DataContainer;

namespace UploadResources.DataAccessLogic
{

    internal static class DBServices
    {


        public static void GetTitleToUpload(TitleDC dc)
        {
            try
            {
                dc.TitleDataSet = DataAccess.DataAccess.ExecuteDataSet(dc.ConnectionWrapper,
                                                                                     "dbo.GetTitleToUpload", null);
            }
            catch (System.Exception ex)
            {
                throw new DALException("DBServices.GetTitleToUpload", ex);
            }
        }


        public static void UpdateTitleUploadStatus(TitleDC dc)
        {
            try
            {
                var parameters = new List<SqlParameter>
                                     {
                                         new SqlParameter("@TitleID", dc.TitleID),                                         
                                         new SqlParameter("@ProcessingEndTime", dc.ProcessingEndTime),
                                         new SqlParameter("@TotalFiles", dc.FileCount),
                                         new SqlParameter("@TotalFileSize", dc.FileSizeString)
                                     };
                DataAccess.DataAccess.ExecuteNonQuery(dc.ConnectionWrapper, "dbo.UpdateTitleUploadStatus", parameters);
            }
            catch (System.Exception ex)
            {
                throw new DALException("DBServices.UpdateTitleUploadStatus", ex);
            }
        }


        public static void GetFileListByTitleID(TitleFileListDC dc)
        {
            try
            {
                var parameters = new List<SqlParameter>
                                     {
                                         new SqlParameter("@TitleID", dc.TitleID)
                                        
                                     };

                dc.TitleFileListDataSet = DataAccess.DataAccess.ExecuteDataSet(dc.ConnectionWrapper, "dbo.GetFileListByTitleID", parameters);
            }
            catch (System.Exception ex)
            {
                throw new DALException("DBServices.GetFileListByTitleID", ex);
            }
        }



        public static void LogUploadError(TitleDC dc)
        {
            try
            {

                foreach (UploadErrorsDC Error in dc.ProcessingErrors)
                {

                    var spParams = new SqlParameter[4];
                    spParams[0] = new SqlParameter("@TitleID", dc.TitleID);
                    spParams[1] = new SqlParameter("@SourcePath", Error.SourcePath);
                    spParams[2] = new SqlParameter("@TargetPath", Error.TargetPath);
                    spParams[3] = new SqlParameter("@Message", Error.Message);
                    
                    DataAccess.DataAccess.ExecuteNonQuery(dc.ConnectionWrapper, "LogUploadError",
                                                                             spParams);
                }
            }
            catch (System.Exception ex)
            {
                throw new DALException("LogUploadError", ex);
            }
        }

 
    }
}