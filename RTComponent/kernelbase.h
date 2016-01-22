#pragma once

//Libraries

HMODULE WINAPI LoadLibraryExW(
	_In_       LPCTSTR lpFileName,
	_Reserved_ HANDLE  hFile,
	_In_       DWORD   dwFlags
);

//Resources

HRSRC WINAPI FindResourceExW(
	_In_opt_ HMODULE hModule,
	_In_     LPCTSTR lpType,
	_In_     LPCTSTR lpName,
	_In_     WORD    wLanguage
);

HGLOBAL WINAPI LoadResource(
	_In_opt_ HMODULE hModule,
	_In_     HRSRC   hResInfo
	);

DWORD WINAPI SizeofResource(
	_In_opt_ HMODULE hModule,
	_In_     HRSRC   hResInfo
);

LPVOID WINAPI LockResource(
	_In_ HGLOBAL hResData
);

//Processes

/*typedef struct _STARTUPINFO {
	DWORD  cb;
	LPTSTR lpReserved;
	LPTSTR lpDesktop;
	LPTSTR lpTitle;
	DWORD  dwX;
	DWORD  dwY;
	DWORD  dwXSize;
	DWORD  dwYSize;
	DWORD  dwXCountChars;
	DWORD  dwYCountChars;
	DWORD  dwFillAttribute;
	DWORD  dwFlags;
	WORD   wShowWindow;
	WORD   cbReserved2;
	LPBYTE lpReserved2;
	HANDLE hStdInput;
	HANDLE hStdOutput;
	HANDLE hStdError;
} STARTUPINFO, *LPSTARTUPINFO;

typedef struct _PROCESS_INFORMATION {
	HANDLE hProcess;
	HANDLE hThread;
	DWORD  dwProcessId;
	DWORD  dwThreadId;
} PROCESS_INFORMATION, *LPPROCESS_INFORMATION;

BOOL WINAPI CreateProcessW(
	_In_opt_    LPCTSTR               lpApplicationName,
	_Inout_opt_ LPTSTR                lpCommandLine,
	_In_opt_    LPSECURITY_ATTRIBUTES lpProcessAttributes,
	_In_opt_    LPSECURITY_ATTRIBUTES lpThreadAttributes,
	_In_        BOOL                  bInheritHandles,
	_In_        DWORD                 dwCreationFlags,
	_In_opt_    LPVOID                lpEnvironment,
	_In_opt_    LPCTSTR               lpCurrentDirectory,
	_In_        LPSTARTUPINFO         lpStartupInfo,
	_Out_       LPPROCESS_INFORMATION lpProcessInformation
);*/