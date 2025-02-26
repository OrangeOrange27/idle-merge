using Package.Logger.Abstraction;
using Package.Logger.ZLogger;
using UnityEngine;

namespace Common.EntryPoint
{
    public static class LoggerInstaller
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSplashScreen)]
        private static void Install()
        {
            LogManager.SetImplementation(new ZLogManager());
        }
    }
}