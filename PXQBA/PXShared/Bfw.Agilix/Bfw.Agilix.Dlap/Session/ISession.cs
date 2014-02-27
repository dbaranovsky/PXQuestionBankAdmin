using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bfw.Agilix.Dlap.Session
{
    /// <summary>
    /// Implementors provide the ability to communicate with DLAP over a number of commands.
    /// Manages connection to DLAP as well as enforces the DLAP command pattern.
    /// </summary>
    public interface ISession
    {
        #region Properties

        /// <summary>
        /// True if the user currently browsing the site is an annonymous user
        /// </summary>
        bool IsAnnonymous { get; set; }

        /// <summary>
        /// Provides access to logging facilities
        /// </summary>
        Bfw.Common.Logging.ILogger Logger { get; set; }

        /// <summary>
        /// Id of the user the session belongs to
        /// </summary>
        string UserId { get; set; }

        /// <summary>
        /// If true, then asynchronous commands will be run asynchronously.
        /// </summary>
        bool AllowAsync { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// Executes the DlapCommand
        /// </summary>
        /// <param name="command">Command to execute</param>
        void Execute(DlapCommand command);

        /// <summary>
        /// Executes the command with increased permissions
        /// </summary>
        /// <param name="command">Command to execute using elevated privileges</param>
        void ExecuteAsAdmin(DlapCommand command);

        /// <summary>
        /// Executes the command with increased permissions
        /// </summary>
        /// <param name="environment">Environment to send command to</param>
        /// <param name="command">Command to execute</param>
        void ExecuteAsAdmin(String environment, DlapCommand command);

        /// <summary>
        /// Sends a request to Dlap and returns the response
        /// </summary>
        /// <typeparam name="TRequest">Type of Request to send</typeparam>
        /// <typeparam name="TResponse">Type of Response expected</typeparam>
        /// <param name="tRequest">Request instance to send</param>
        /// <returns>Response returned from Dlap</returns>
        TResponse Send<TRequest, TResponse>(TRequest tRequest) where TRequest : IDlapRequestTransformer, new() where TResponse : IDlapResponseParser, new();

        /// <summary>
        /// Direct passthrough to DlapConnection.Send
        /// </summary>
        /// <param name="request">Request to send to DLAP</param>
        /// <returns>Response returned by DLAP</returns>
        DlapResponse Send(DlapRequest request, bool asAdmin = false, string environment = "");

        #endregion
    }
}
