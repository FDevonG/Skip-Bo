public static class DeviceType
{
    public static bool IsDeviceAndroid() {
        bool android = false;
        #if UNITY_ANDROID
            android = true;
        #endif
        return android;
    }

    public static bool IsDeviceIos() {
        bool ios = false;
        #if UNITY_IOS
            ios = true;
        #endif
        return ios;
    }
}
