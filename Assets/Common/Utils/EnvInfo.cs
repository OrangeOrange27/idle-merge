namespace Common.Utils
{
        public static class EnvInfo
        {
                public static bool IsIOS => CheckIsIOS();
                public static bool IsAndroid => CheckIsAndroid();
                public static bool IsDev => CheckIsDev();
                public static bool IsEditor => CheckIsEditor();

                private static bool CheckIsIOS()
                {
#if UNITY_IOS
        return true;
#else
                        return false;
#endif
                }

                private static bool CheckIsAndroid()
                {
#if UNITY_ANDROID
        return true;
#else
                        return false;
#endif
                }

                private static bool CheckIsDev()
                {
#if CHEAT
        return true;
#else
                        return false;
#endif
                }


                private static bool CheckIsEditor()
                {
#if UNITY_EDITOR
                        return true;
#else
        return false;
#endif
                }
        }
}