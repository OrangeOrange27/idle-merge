using System;
using UnityEngine;
using VContainer;
using ZLogger;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace Common.GlobalServiceLocator
{
    public class GlobalServices
    {
        private static GlobalServices _instance;
        private static IObjectResolver _objectResolver;
        
        private static readonly ILogger Logger = Package.Logger.Abstraction.LogManager.GetLogger<GlobalServices>();
        public static void Initialize(IObjectResolver objectResolver)
        {
            if (_instance != null)
                return;
            _objectResolver = objectResolver;
            _instance = new GlobalServices();
        }

        public static void Reset()
        {
            _instance = null;
        }

        public static T Get<T>()
        {
            if (_instance == null && Application.isEditor && !Application.isPlaying)
            {
                Logger.ZLogWarning("GlobalService wasn't initialized but was run out of playmode. Return default({0})", typeof(T).Name);
                return default;
            }
            
            ThrowIfNotInit();
            return _objectResolver.Resolve<T>();
        }
        
        public static bool TryGet<T>(out T result)
        {
            if (_instance == null)
            {
                result = default;
                return false;
            }
            result = _objectResolver.Resolve<T>();
            return true;
        }
        
        private static void ThrowIfNotInit()
        {
            if (_instance == null)
            {
                throw new Exception("You cannot use GlobalServices before calling Initialize");
            }
        }
    }
}