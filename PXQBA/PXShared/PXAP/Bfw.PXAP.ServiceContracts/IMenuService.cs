using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using Bfw.PXAP.Models;


namespace Bfw.PXAP.ServiceContracts
{
    [ServiceContract]
    public interface IMenuService
    {
        [OperationContract]
        List<ExternalMenuModel> GetExternalMenu(PXEnvironment env);

        [OperationContract]
        List<MainMenuModel> GetMainMenu();

        [OperationContract]
        List<MainMenuModel> GetSettingsMenu();

        [OperationContract]
        List<MainMenuModel> GetMetadataMenu();

        [OperationContract]
        List<MainMenuModel> GetContentMenu();

        [OperationContract]
        List<MainMenuModel> GetDLAPMenu();
    }
}
