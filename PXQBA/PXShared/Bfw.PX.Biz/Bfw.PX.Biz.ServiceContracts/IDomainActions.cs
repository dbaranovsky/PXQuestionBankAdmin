// -----------------------------------------------------------------------
// <copyright file="IDomainActions.cs" company="Microsoft">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Bfw.PX.Biz.ServiceContracts
{
    using System.Xml.Linq;
    using Bfw.PX.Biz.DataContracts;

    /// <summary>
    /// Domain Actions
    /// </summary>
    public interface IDomainActions
    {
        Domain GetDomain(string domainName);

        Domain GetDomainById(string domainId);

        Domain CreateDomain(string parentId, string domainName, string domainUserSpace, string reference, XElement Data);

        bool CheckDomainExists(string domainId, string domainName);

        string GetGenericDomainId();

        Domain GetOrCreateDomain(string domainName, string parentDomainName, string onyxReferenceId, string userspacePrefix = "", string userspaceSuffix = "", bool copyResourcesForNewDomain = true);
    }
}
