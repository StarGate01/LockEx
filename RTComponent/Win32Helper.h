#pragma once

#include "Objbase.h"

typedef struct _CLIENT_ID 
{
	PVOID UniqueProcess;
	PVOID UniqueThread;
} CLIENT_ID;

typedef struct _MODULE_LIST_ENTRY 
{
	struct  _MODULE_LIST_ENTRY* Flink;
	struct  _MODULE_LIST_ENTRY* Blink;
	DWORD* baseAddress;
} MODULE_LIST_ENTRY;

typedef struct _PEB_LDR_DATA 
{
	ULONG Length;
	BOOLEAN Initialized;
	PVOID SsHandle;
	LIST_ENTRY InLoadOrderModuleList;
	LIST_ENTRY InMemoryOrderModuleList;
	MODULE_LIST_ENTRY* initModuleList;
} PEB_LDR_DATA;

typedef struct _PEB
{
	BYTE Reserved1[2];
	BYTE BeingDebugged;
	BYTE Reserved2[1];
	PVOID Reserved3[2];
	PEB_LDR_DATA* ldr;
} PEB;

typedef struct _TEB
{
	NT_TIB nt_tib;
	PVOID EnvironmentPointer;
	CLIENT_ID id;
	PVOID ActiveRpcHandle;
	PVOID ThreadLocalStoragePointer;
	PEB* currentPEB;
} TEB;

typedef HMODULE(*LoadLibraryEx)(LPCTSTR lpLibFileName, HANDLE hFile, DWORD dwFlags);

typedef HRSRC(*FindResourceEx)(_In_opt_ HMODULE hModule, _In_ LPCTSTR lpType, _In_ LPCTSTR lpName, _In_ WORD wLanguage);
typedef HGLOBAL(*LoadResource)(_In_opt_ HMODULE hModule, _In_ HRSRC hResInfo);
typedef DWORD(*SizeofResource)(_In_opt_ HMODULE hModule, _In_  HRSRC hResInfo);
typedef LPVOID (*LockResource)(_In_ HGLOBAL hResData);

/*typedef BOOL(*ENUMRESNAMEPROC)(_In_opt_ HMODULE hModule, _In_ LPCTSTR lpszType,	_In_ LPTSTR lpszName, _In_ LONG_PTR lParam);
typedef BOOL(*EnumResourceNamesEx)(_In_opt_ HMODULE hModule, _In_ LPCTSTR lpszType, _In_ ENUMRESNAMEPROC lpEnumFunc, _In_ LONG_PTR lParam, _In_ DWORD dwFlags, _In_ LANGID LangId);*/