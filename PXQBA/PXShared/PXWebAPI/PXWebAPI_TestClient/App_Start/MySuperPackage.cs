
[assembly: WebActivator.PreApplicationStartMethod(
	typeof(PXWebAPI_TestClient.App_Start.MySuperPackage), "PreStart")]

namespace PXWebAPI_TestClient.App_Start
{
	public static class MySuperPackage
	{
		public static void PreStart()
		{
			MVCControlsToolkit.Core.Extensions.Register();
		}
	}
}