using System;

[assembly: WebActivator.PreApplicationStartMethod(
    typeof($rootnamespace$.App_Start.MySuperPackage), "PreStart")]

namespace $rootnamespace$.App_Start {
    public static class MySuperPackage {
        public static void PreStart() {
            MVCControlsToolkit.Core.Extensions.Register();
        }
    }
}