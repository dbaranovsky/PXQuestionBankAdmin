using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Macmillan.PXQBA.DataAccess.Data
{
    public interface IQBAUow
    {
        QBADummyModelContainer DbContext { get; }
         /// <summary>
        /// Commits info from DbContext to database
        /// </summary>
        void Commit(); 
    }
}
