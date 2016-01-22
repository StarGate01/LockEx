#pragma once

#include "ShellLockScreenAPI.h"

HRESULT Shell_TurnScreenOn(
	_In_ BOOL bOn
);

HRESULT WINAPI Shell_InitReminders(
	_Out_ INT* pRPCHandle
);

HRESULT WINAPI Shell_DeInitReminders(
	_In_ INT* pRPCHandle
);

HRESULT Shell_FindFirstReminder(
	_Inout_ INT* p0,
	_Inout_ INT* sp1,
	_Inout_ INT* p2,
	_Inout_ INT* p3
);

HRESULT Shell_FindNextReminder(
	VOID
);

HRESULT Shell_FindReminderClose(
	VOID
);

HRESULT Shell_GetReminderData(
	VOID
);

HRESULT Shell_ReleaseReminderData(
	VOID
);