
using System;
using System.Data;
using System.Reflection;
using UploadResources.Extensions;

namespace UploadResources.DataAccess
{
    /// <summary>
    /// Base class for all Data Containers.  Class provides all basic properties for data containers.
    /// </summary>
    public class BaseDC
    {

        private string _errorID;


        /// <summary>
        /// comma seperated list of error ids generated during execution of business process
        /// </summary>
        public string ErrorID
        {
            get { return _errorID; }
            set
            {
                if (_errorID.IsNotNullOrEmpty())
                    _errorID += "," + value;
                else
                    _errorID = value;
            }
        }

        /// <summary>
        /// A property to return DataTable that contains all properties
        /// </summary>
        public DataTable PropertiesDataTable
        {
            get
            {
                var propertiesDataTable = new DataSet();
                propertiesDataTable.Tables.Add(new DataTable());

                var fieldInfo = this.GetType().GetProperties();

                //Create a column for each property in data container object
                foreach (var info in fieldInfo)
                {
                    if (info.Name == "PropertiesDataTable" || info.Name == "ConnectionWrapper")
                        continue;
                    var dataColumn = new DataColumn(info.Name);
                    dataColumn.DefaultValue = info.GetValue(this, null);
                    if (dataColumn.DefaultValue.ToString().IsNullOrEmpty())
                        dataColumn.DefaultValue = "";
                    propertiesDataTable.Tables[0].Columns.Add(dataColumn);
                }
                //Add a new row with default value
                propertiesDataTable.Tables[0].Rows.Add(propertiesDataTable.Tables[0].NewRow());
                return propertiesDataTable.Tables[0];
            }
        }

        /// <summary>
        /// Connection token to connect to database and handle transaction
        /// </summary>
        public ConnectionTokenWrapperBase ConnectionWrapper { get; set; }

    }
}