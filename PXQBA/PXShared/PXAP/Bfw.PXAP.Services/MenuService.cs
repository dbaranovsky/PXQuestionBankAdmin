using System.Collections.Generic;
using Bfw.PXAP.Models;
using Bfw.PXAP.ServiceContracts;


namespace Bfw.PXAP.Services
{
	public class MenuService : IMenuService
	{
		/// <summary>
		/// Gets the list of the external menu items
		/// </summary>
		/// <returns></returns>
		public List<ExternalMenuModel> GetExternalMenu(PXEnvironment env)
		{

			List<ExternalMenuModel> externalMenuModel = new List<ExternalMenuModel>();

			if (env != null)
			{
				externalMenuModel.Add(new ExternalMenuModel
				{
					MenuName = "DLAP",
					MenuURL = env.DlapServer
				});

				externalMenuModel.Add(new ExternalMenuModel
				{
					MenuName = "BrainHoney",
					MenuURL = env.BrainHoneyServer
				});

				externalMenuModel.Add(new ExternalMenuModel
				{
					MenuName = "BrainHoneyDocs",
					MenuURL = env.BrainHoneyDocs
				});

				externalMenuModel.Add(new ExternalMenuModel
				{
					MenuName = "PXDocs",
					MenuURL = env.PxDocs
				});

			}
			return externalMenuModel;

		}


		public List<MainMenuModel> GetSettingsMenu()
		{
			List<MainMenuModel> menuModels = new List<MainMenuModel>();

			menuModels.Add(new MainMenuModel
			{
				MenuName = "Environments",
				MenuAction = "Environments",
				MenuController = "Environment"
			});

			menuModels.Add(new MainMenuModel
			{
				MenuName = "Users",
				MenuAction = "Users",
				MenuController = "User"
			});

			return menuModels;
		}

		public List<MainMenuModel> GetMetadataMenu()
		{
			List<MainMenuModel> menuModels = new List<MainMenuModel>();

			menuModels.Add(new MainMenuModel
			{
				MenuName = "General",
				MenuAction = "Index",
				MenuController = "Metadata"
			});

			menuModels.Add(new MainMenuModel
			{
				MenuName = "Search",
				MenuAction = "Index",
				MenuController = "Metadata"
			});

			return menuModels;
		}

		public List<MainMenuModel> GetContentMenu()
		{
			List<MainMenuModel> menuModels = new List<MainMenuModel>();

			menuModels.Add(new MainMenuModel
			{
				MenuName = "Move Content",
				MenuAction = "Index",
				MenuController = "Content"
			});


			return menuModels;
		}
		public List<MainMenuModel> GetDLAPMenu()
		{
			List<MainMenuModel> menuModels = new List<MainMenuModel>();

			return menuModels;
		}

		public List<MainMenuModel> GetMainMenu()
		{
			List<MainMenuModel> mainMenuModel = new List<MainMenuModel>();

			mainMenuModel.Add(new MainMenuModel
			{
				MenuName = "Dashboard",
				MenuAction = "Index",
				MenuController = "Home"
			});

			mainMenuModel.Add(new MainMenuModel
			{
				MenuName = "Logs",
				MenuAction = "LogSearch",
				MenuController = "LogSearch"
			});

			//mainMenuModel.Add(new MainMenuModel
			//{
			//    MenuName = "Status",
			//    MenuAction = "",
			//    MenuController = ""
			//});

			mainMenuModel.Add(new MainMenuModel
			{
				MenuName = "Metadata",
				MenuAction = "Index",
				MenuController = "Metadata"
			});

			mainMenuModel.Add(new MainMenuModel
			{
				MenuName = "Settings",
				MenuAction = "Environments",
				MenuController = "Environment"
			});

			mainMenuModel.Add(new MainMenuModel
			{
				MenuName = "Content",
				MenuAction = "Index",
				MenuController = "Content"
			});

			mainMenuModel.Add(new MainMenuModel
			{
				MenuName = "Enrollment",
				MenuAction = "Index",
				MenuController = "Enrollment"
			});

			mainMenuModel.Add(new MainMenuModel
			{
				MenuName = "DlapCommand",
				MenuAction = "Index",
				MenuController = "DlapCommand"
			});

			mainMenuModel.Add(new MainMenuModel
			{
				MenuName = "PXWebUser Rights",
				MenuAction = "Index",
				MenuController = "PXWebUser"
			});

            mainMenuModel.Add(new MainMenuModel
            {
                MenuName = "AppFabric Cache",
                MenuAction = "Index",
                MenuController = "AppFabricCache"
            });

			return mainMenuModel;
		}
	}
}
