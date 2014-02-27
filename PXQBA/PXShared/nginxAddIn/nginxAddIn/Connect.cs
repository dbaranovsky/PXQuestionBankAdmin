using System;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;
using Extensibility;
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.CommandBars;
using System.Resources;
using System.Reflection;
using System.Globalization;
using Process = EnvDTE.Process;

namespace nginxAddIn
{
	/// <summary>The object for implementing an Add-in.</summary>
	/// <seealso class='IDTExtensibility2' />
	public class Connect : IDTExtensibility2, IDTCommandTarget
	{

		SolutionEvents solutionEvents;
		private DTE2 _applicationObject;
		private AddIn _addInInstance;
		BuildEvents buildEvents;

		/// <summary>Implements the constructor for the Add-in object. Place your initialization code within this method.</summary>
		public Connect()
		{
		}

		private void BuildEvents_OnBuildDone(EnvDTE.vsBuildScope scope, EnvDTE.vsBuildAction action)
		{
		    //if (!_applicationObject.Solution.FileName.Contains("PlatformX.sln")) return;

			var activeConfigurationName = _applicationObject.Solution.SolutionBuild.ActiveConfiguration.Name;
			var nginxFolder = "";
			var sourcePath="";
			var destPath = "";
			var sourceFile = "nginx.conf";
			var destFile = "nginx.conf";

			if (!_applicationObject.DTE.Globals.VariableExists["nginxSourcePath"] | !_applicationObject.DTE.Globals.VariableExists["nginxDestPath"])
			{
				var directoryInfo = System.IO.Directory.GetParent(_applicationObject.Solution.FullName).Parent;
				if (directoryInfo != null && directoryInfo.Exists)
				{
					if (directoryInfo.GetDirectories("nginx").Any())
					{
						nginxFolder = directoryInfo.GetDirectories("nginx")[0].FullName;
						sourcePath = nginxFolder + @"\conf\px_nginx_config\";
						destPath = nginxFolder + @"\conf\";

						_applicationObject.DTE.Globals["nginxFolder"] = nginxFolder;
						_applicationObject.DTE.Globals["nginxSourcePath"] = sourcePath;
						_applicationObject.DTE.Globals["nginxDestPath"] = destPath;					

					}
				}
			}
			else
			{
				nginxFolder = _applicationObject.DTE.Globals["nginxFolder"].ToString();
				sourcePath = _applicationObject.DTE.Globals["nginxSourcePath"].ToString();
				destPath = _applicationObject.DTE.Globals["nginxDestPath"].ToString();

			}

			if (!System.IO.Directory.Exists(sourcePath)) return;

			sourceFile = sourcePath + activeConfigurationName + "_" + sourceFile;
			destFile = destPath + destFile;
			try
			{
				var fi = new System.IO.FileInfo(sourceFile);
				fi.CopyTo(destFile,true);
			
				// Reload nginx:
				var proc = new System.Diagnostics.Process
					{
						StartInfo =
							new ProcessStartInfo(nginxFolder + @"\startnginx.bat")
								{
									Domain = nginxFolder,
									WorkingDirectory = nginxFolder,
									UseShellExecute = false
								},
						EnableRaisingEvents = true
					};
				proc.Exited += new EventHandler(Nginx_Restarted);

				proc.Start();
				proc.WaitForExit(1000);
				proc.Kill();
				if (proc.HasExited)
				{
					var ExitCode = proc.ExitCode;
				}

			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
			}
	
			if (activeConfigurationName == "Release") return;

		}

		// Handle Exited event and display process information. 
		private void Nginx_Restarted(object sender, System.EventArgs e)
		{
			var activeConfigurationName = _applicationObject.Solution.SolutionBuild.ActiveConfiguration.Name;

			MessageBox.Show("This is " + activeConfigurationName + " Configuration !");
		}

		/// <summary>Implements the OnConnection method of the IDTExtensibility2 interface. Receives notification that the Add-in is being loaded.</summary>
		/// <param term='application'>Root object of the host application.</param>
		/// <param term='connectMode'>Describes how the Add-in is being loaded.</param>
		/// <param term='addInInst'>Object representing this Add-in.</param>
		/// <seealso class='IDTExtensibility2' />
		public void OnConnection(object application, ext_ConnectMode connectMode, object addInInst, ref Array custom)
		{
			_applicationObject = (DTE2)application;
			_addInInstance = (AddIn)addInInst;

			solutionEvents = _applicationObject.Events.SolutionEvents;

			buildEvents = _applicationObject.Events.BuildEvents;

            buildEvents.OnBuildDone += BuildEvents_OnBuildDone;
			//buildEvents.OnBuildProjConfigDone += BuildEvents_OnBuildProjConfigDone;
					
			if(connectMode == ext_ConnectMode.ext_cm_UISetup)
			{
				object []contextGUIDS = new object[] { };
				Commands2 commands = (Commands2)_applicationObject.Commands;
				string toolsMenuName = "Tools";

				//Place the command on the tools menu.
				//Find the MenuBar command bar, which is the top-level command bar holding all the main menu items:
				Microsoft.VisualStudio.CommandBars.CommandBar menuBarCommandBar = ((Microsoft.VisualStudio.CommandBars.CommandBars)_applicationObject.CommandBars)["MenuBar"];

				//Find the Tools command bar on the MenuBar command bar:
				CommandBarControl toolsControl = menuBarCommandBar.Controls[toolsMenuName];
				CommandBarPopup toolsPopup = (CommandBarPopup)toolsControl;

				//This try/catch block can be duplicated if you wish to add multiple commands to be handled by your Add-in,
				//  just make sure you also update the QueryStatus/Exec method to include the new command names.
				try
				{
					//Add a command to the Commands collection:
					Command command = commands.AddNamedCommand2(_addInInstance, "nginxAddIn", "nginxAddIn", "Executes the command for nginxAddIn", true, 59, ref contextGUIDS, (int)vsCommandStatus.vsCommandStatusSupported+(int)vsCommandStatus.vsCommandStatusEnabled, (int)vsCommandStyle.vsCommandStylePictAndText, vsCommandControlType.vsCommandControlTypeButton);

					//Add a control for the command to the tools menu:
					if((command != null) && (toolsPopup != null))
					{
						command.AddControl(toolsPopup.CommandBar, 1);
					}
				}
				catch(System.ArgumentException)
				{
					//If we are here, then the exception is probably because a command with that name
					//  already exists. If so there is no need to recreate the command and we can 
                    //  safely ignore the exception.
				}
			}
		}

		/// <summary>Implements the OnDisconnection method of the IDTExtensibility2 interface. Receives notification that the Add-in is being unloaded.</summary>
		/// <param term='disconnectMode'>Describes how the Add-in is being unloaded.</param>
		/// <param term='custom'>Array of parameters that are host application specific.</param>
		/// <seealso class='IDTExtensibility2' />
		public void OnDisconnection(ext_DisconnectMode disconnectMode, ref Array custom)
		{
		}

		/// <summary>Implements the OnAddInsUpdate method of the IDTExtensibility2 interface. Receives notification when the collection of Add-ins has changed.</summary>
		/// <param term='custom'>Array of parameters that are host application specific.</param>
		/// <seealso class='IDTExtensibility2' />		
		public void OnAddInsUpdate(ref Array custom)
		{
		}

		/// <summary>Implements the OnStartupComplete method of the IDTExtensibility2 interface. Receives notification that the host application has completed loading.</summary>
		/// <param term='custom'>Array of parameters that are host application specific.</param>
		/// <seealso class='IDTExtensibility2' />
		public void OnStartupComplete(ref Array custom)
		{
		}

		/// <summary>Implements the OnBeginShutdown method of the IDTExtensibility2 interface. Receives notification that the host application is being unloaded.</summary>
		/// <param term='custom'>Array of parameters that are host application specific.</param>
		/// <seealso class='IDTExtensibility2' />
		public void OnBeginShutdown(ref Array custom)
		{
		}
		
		/// <summary>Implements the QueryStatus method of the IDTCommandTarget interface. This is called when the command's availability is updated</summary>
		/// <param term='commandName'>The name of the command to determine state for.</param>
		/// <param term='neededText'>Text that is needed for the command.</param>
		/// <param term='status'>The state of the command in the user interface.</param>
		/// <param term='commandText'>Text requested by the neededText parameter.</param>
		/// <seealso class='Exec' />
		public void QueryStatus(string commandName, vsCommandStatusTextWanted neededText, ref vsCommandStatus status, ref object commandText)
		{
			if(neededText == vsCommandStatusTextWanted.vsCommandStatusTextWantedNone)
			{
				if(commandName == "nginxAddIn.Connect.nginxAddIn")
				{
					status = (vsCommandStatus)vsCommandStatus.vsCommandStatusSupported|vsCommandStatus.vsCommandStatusEnabled;
					return;
				}
			}
		}

		/// <summary>Implements the Exec method of the IDTCommandTarget interface. This is called when the command is invoked.</summary>
		/// <param term='commandName'>The name of the command to execute.</param>
		/// <param term='executeOption'>Describes how the command should be run.</param>
		/// <param term='varIn'>Parameters passed from the caller to the command handler.</param>
		/// <param term='varOut'>Parameters passed from the command handler to the caller.</param>
		/// <param term='handled'>Informs the caller if the command was handled or not.</param>
		/// <seealso class='Exec' />
		public void Exec(string commandName, vsCommandExecOption executeOption, ref object varIn, ref object varOut, ref bool handled)
		{
			handled = false;
			if(executeOption == vsCommandExecOption.vsCommandExecOptionDoDefault)
			{
				if(commandName == "nginxAddIn.Connect.nginxAddIn")
				{
					handled = true;
					return;
				}
			}
		}

	}
}