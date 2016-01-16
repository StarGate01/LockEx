#pragma once

extern "C"
{

	HMODULE WINAPI LoadLibraryExW(
		_In_       LPCTSTR lpFileName,
		_Reserved_ HANDLE  hFile,
		_In_       DWORD   dwFlags
		);

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

}