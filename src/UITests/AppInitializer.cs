using System;
using System.IO;
using System.Linq;
using Xamarin.UITest;
using Xamarin.UITest.Queries;

namespace cakedemos.UITests
{
    public class AppInitializer
    {
        public static IApp StartApp(Platform platform)
        {
            //if (platform == Platform.Android)
            //{
            //    return ConfigureApp.Android.StartApp();
            //}

            return ConfigureApp.iOS
                               .AppBundle("../../../iOS/bin/iPhoneSimulator/Debug/cakedemos.iOS.app")
                               .DeviceIdentifier("4BD0FC1B-2A95-4975-8FC1-4ED60913D500")
                               .StartApp();
        }
    }
}
