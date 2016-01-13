﻿#include "pch.h"
#include "Objbase.h"
#include "Win32Helper.h"
#include "NativeAPI.h"
#include <windows.h>
#include <sstream>

using namespace std;
using namespace Platform;

using namespace RTComponent;
using namespace RTComponent::NotificationsSnapshot;

HMODULE KernelBase32, ShellChromeAPI, UIXMobileAssets;
LoadLibraryEx LoadLibraryExW;
Shell_TurnScreenOn Shell_TurnScreenOnW;
Shell_LockScreen_GetNotificationsSnapshot Shell_LockScreen_GetNotificationsSnapshotW;
FindResourceEx FindResourceExW;
LoadResource LoadResourceW;
SizeofResource SizeofResourceW;
LockResource LockResourceW;

NativeAPI::NativeAPI()
{
	TEB* teb = NtCurrentTeb();
	KernelBase32 = (HMODULE)teb->currentPEB->ldr->initModuleList->Flink->baseAddress;
	LoadLibraryExW = (LoadLibraryEx)GetProcAddress(KernelBase32, "LoadLibraryExW");
	ShellChromeAPI = LoadLibraryExW(L"ShellChromeAPI.dll", NULL, 0x00001000);
	Shell_TurnScreenOnW = (Shell_TurnScreenOn)GetProcAddress(ShellChromeAPI, "Shell_TurnScreenOn");
	Shell_LockScreen_GetNotificationsSnapshotW = (Shell_LockScreen_GetNotificationsSnapshot)GetProcAddress(ShellChromeAPI, "Shell_LockScreen_GetNotificationsSnapshot");
}

void NativeAPI::InitUIXMAResources(int resWidth, int resHeight)
{
	FindResourceExW = (FindResourceEx)GetProcAddress(KernelBase32, "FindResourceExW");
	LoadResourceW = (LoadResource)GetProcAddress(KernelBase32, "LoadResource");
	SizeofResourceW = (SizeofResource)GetProcAddress(KernelBase32, "SizeofResource");
	LockResourceW = (LockResource)GetProcAddress(KernelBase32, "LockResource");
	wstringstream sstm;
	sstm << L"UIXMobileAssets" << resWidth << L"x" << resHeight << L".dll";
	UIXMobileAssets = LoadLibraryExW(sstm.str().c_str(), NULL, 0x00001000);
	if (UIXMobileAssets == NULL) throw ref new InvalidArgumentException("The corresponding UIXMobileAssets{ScreenResolution}.dll was not found");
}

void NativeAPI::TurnScreenOn(Boolean state)
{
	Shell_TurnScreenOnW(state);
}

Snapshot^ NativeAPI::GetNotificationsSnapshot()
{
	DEVICE_LOCK_SCREEN_SNAPSHOT* pSnapshot = new DEVICE_LOCK_SCREEN_SNAPSHOT();
	pSnapshot->size = sizeof(DEVICE_LOCK_SCREEN_SNAPSHOT);
	HRESULT shellRes = Shell_LockScreen_GetNotificationsSnapshotW(pSnapshot);
	if (shellRes == S_OK)
	{
		Snapshot^ mSnap = ref new NotificationsSnapshot::Snapshot();
		mSnap->Size = pSnapshot->size;
		mSnap->DrivingModeIconUri = ref new String(pSnapshot->drivingModeIconUri);
		mSnap->DoNotDisturbIconUri = ref new String(pSnapshot->doNotDisturbModeIconUri);
		mSnap->AlarmIconUri = ref new String(pSnapshot->alarmIconUri);
		mSnap->DetailedTextCount = pSnapshot->detailedTextCount;
		mSnap->DetailedTexts = ref new Vector<DetailedTextInformation^>();
		for (unsigned int i = 0; i < pSnapshot->detailedTextCount; i++)
		{
			DetailedTextInformation^ dtInfo = ref new DetailedTextInformation();
			dtInfo->DetailedText = ref new String(pSnapshot->detailedTexts[i].detailedText);
			dtInfo->IsBoldText = pSnapshot->detailedTexts[i].isBoldText;
			mSnap->DetailedTexts->Append(dtInfo);
		}
		mSnap->BadgeCount = pSnapshot->badgeCount;
		mSnap->Badges = ref new Vector<BadgeInformation^>();
		for (unsigned int i = 0; i < pSnapshot->badgeCount; i++)
		{
			BadgeInformation^ bInfo = ref new BadgeInformation();
			bInfo->Type = static_cast<BadgeValueType>(pSnapshot->badges[i].badgeValueType);
			bInfo->Value = ref new String(pSnapshot->badges[i].badgeValue);
			bInfo->IconUri = ref new String(pSnapshot->badges[i].badgeIconUri);
			mSnap->Badges->Append(bInfo);
		}
		return mSnap;
	}
	else if (shellRes == E_ACCESSDENIED) throw ref new AccessDeniedException("Missing permission for Shell_LockScreen_GetNotificationsSnapshot");
	else throw ref new FailureException("Shell_LockScreen_GetNotificationsSnapshot failed");
}

Array<uint8>^ NativeAPI::GetUIXMAResource(String^ name)
{
	if (UIXMobileAssets != NULL)
	{
		if (name->IsEmpty() || name->Length() == 0) throw ref new InvalidArgumentException("Resource Name must not be empty");
		HRSRC resHandle = FindResourceExW(UIXMobileAssets, RT_RCDATA, name->Data(), MAKELANGID(LANG_ENGLISH, SUBLANG_ENGLISH_US));
		if (resHandle != NULL)
		{
			HGLOBAL resLHandle = LoadResourceW(UIXMobileAssets, resHandle);
			if (resLHandle == NULL) throw ref new FailureException("LoadResourceW failed");
			DWORD resSize = SizeofResourceW(UIXMobileAssets, resHandle);
			if (resSize == NULL) throw ref new FailureException("SizeofResourceW failed");
			LPVOID resData = LockResourceW(resLHandle);
			if (resData == NULL) throw ref new FailureException("LockResourceW failed");
			UINT8 *bytes = new UINT8[resSize];
			memcpy(bytes, resData, resSize);
			return ref new Array<uint8>(bytes, resSize);
		}
		else
		{
			DWORD err = GetLastError();
			if (err == ERROR_RESOURCE_NAME_NOT_FOUND) throw ref new OutOfBoundsException("The requested resource was not found in UIXMobileAssets");
			throw ref new FailureException("FindResourceExW failed");
		}
	}
	else throw ref new ClassNotRegisteredException("UIXMobileAssets is not loaded; use InitUIXMAResources to load");
}