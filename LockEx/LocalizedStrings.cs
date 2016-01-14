using LockEx.Resources;

namespace LockEx
{

    public class LocalizedStrings
    {

        private static AppResources _localizedResources = new AppResources();

        public AppResources LocalizedResources { get { return _localizedResources; } }

    }

}