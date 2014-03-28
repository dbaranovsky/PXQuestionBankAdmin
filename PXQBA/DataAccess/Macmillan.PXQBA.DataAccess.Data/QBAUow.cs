using System;
using System.Data.Entity;
using System.Data.Entity.Validation;
using Bfw.Common.Logging;

namespace Macmillan.PXQBA.DataAccess.Data
{
    public class QBAUow : IQBAUow, IDisposable
    {
        private readonly QBADummyModelContainer dbContext;

        private readonly ILogger logger;

        /// <summary>
        /// Creates eticket unity of work with the provided repository provider
        /// </summary>
        /// <param name="context">Db context </param>
        public QBAUow(QBADummyModelContainer context, ILogger logger)
        {
            dbContext = context;
            this.logger = logger;
        }

        public QBADummyModelContainer DbContext
        {
            get
            {
                return dbContext;
            }
        }

        /// <summary>
        /// Commits info from dbContext to database
        /// </summary>
        public void Commit()
        {
            try
            {
                dbContext.SaveChanges();
            }
            catch (DbEntityValidationException exc)
            {
                foreach (var error in exc.EntityValidationErrors)
                {
                    logger.Log(string.Format("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                        error.Entry.Entity.GetType().Name, error.Entry.State), LogSeverity.Error);
                    foreach (var ve in error.ValidationErrors)
                    {
                        logger.Log(string.Format("- Property: \"{0}\", Error: \"{1}\"",
                            ve.PropertyName, ve.ErrorMessage), LogSeverity.Error);
                    }
                }
                throw;
            }
        }

        #region IDisposable Members

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion

        /// <summary>
        /// Dispose if the appropriate parameter value
        /// </summary>
        /// <param name="disposing">If disposing</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (dbContext != null)
                {
                    dbContext.Dispose();
                }
            }
        }
    }
}