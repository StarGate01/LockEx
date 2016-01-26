#pragma once

using namespace Platform;
using namespace Platform::Collections;
using namespace Windows::Foundation::Collections;

namespace RTComponent
{

	namespace NotificationsSnapshot
	{

		public enum class BadgeValueType : int
		{
			None, Count, Glyph
		};

		public ref class DetailedTextInformation sealed 
		{
		public:
			property String^ DetailedText;
			property bool IsBoldText;
		};

		public ref class BadgeInformation sealed
		{
		public:
			property BadgeValueType Type;
			property String^ Value;
			property String^ IconUri;
		};

		public ref class Snapshot sealed
		{
		public:
			property int Size;
			property String^ DrivingModeIconUri;
			property String^ DoNotDisturbIconUri;
			property String^ AlarmIconUri;
			property int DetailedTextCount;
			property int BadgeCount;
			property IVector<DetailedTextInformation^>^ DetailedTexts;
			property IVector<BadgeInformation^>^ Badges;
		};

	}

    public ref class NativeAPI sealed
    {

	private:
		HMODULE UIXMobileAssets;

    public:
		NativeAPI();
		void InitUIXMAResources(int resWidth, int resHeight);
		void TurnScreenOn(bool state);
		int GetCurrentMode();
		NotificationsSnapshot::Snapshot^ GetNotificationsSnapshot();
		Array<uint8>^ GetUIXMAResource(String^ name);		

    };

}
