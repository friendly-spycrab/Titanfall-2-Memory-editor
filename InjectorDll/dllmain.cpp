#include<Windows.h>
#include <vector>
#include "MinHook.h"

#if defined _M_X64
#pragma comment(lib, "MinHook.x64.lib")
#elif defined _M_IX86
#pragma comment(lib, "MinHook.x86.lib")
#endif

using namespace std;
extern "C" DWORD GetAddress();//Get the address from asm
extern "C" void ResetRax();
extern "C" void SetNewAddress(DWORD64 Address);


BOOL APIENTRY DllMain(HMODULE hModule, DWORD  ul_reason_for_call, LPVOID lpReserved)
{
	switch (ul_reason_for_call)
	{
		case DLL_PROCESS_ATTACH:
		{
			break;
		}

		case DLL_PROCESS_DETACH:
			break;

		case DLL_THREAD_ATTACH:
			break;

		case DLL_THREAD_DETACH:
			break;
	}

	return TRUE;
}

byte Test[] = { 0x67,0x6c,0x6f,0x62,0x61 ,0x6c,0x20,0x66,0x75,0x6e,0x63,0x74,0x69,0x6f,0x6e,0x20,0x47,0x65,0x74,0x50,0x69,0x6c,0x6f,0x74,0x4c,0x6f,0x61,0x64,0x6f,0x75,0x74,0x46,0x6f,0x72,0x43,0x75,0x72,0x72,0x65,0x6e,0x74,0x4d,0x61,0x70,0x53,0x50,0xa };
DWORD Address = 0;
vector<FILE> Files;


extern "C" void CheckIfShouldRedirect()
{
	Address = GetAddress();
	ResetRax();
	byte *Location = reinterpret_cast<byte*>(Address + 10);

	MessageBox(NULL, L"I have done something", L"T", MB_OK);

}

bool CheckIsSameValue(byte *Location, byte BArray[])
{
	byte *Pointer = Location;
	bool IsSame = true;
	for (size_t i = 0; i < (sizeof(BArray) / sizeof(*BArray)); i++)
	{
		Pointer += i;
		if (BArray[i] != *Pointer)
		{
			IsSame = false;
			break;
		}
	}

	return IsSame;
}

void Hook()
{
	//MH_Initialize();


}

extern "C" __declspec(dllexport) int Initialize(int *Address)
{
	return 0;
}

extern "C" __declspec(dllexport) void TestFunctionCall(byte Param)
{
	MessageBox(NULL, L"Function called and changed", L"Dll says:", MB_OK);
}

//extern "C" __declspec(dllexport) void AddCompare(byte *data)
//{
//
//
//}


/*
	Contains the pointer to the new data. a comparison string.
*/
struct File
{
	byte *Pointer;
	byte Comparison[];
};
