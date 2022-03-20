#include <stdio.h>
#include <windows.h>

#pragma comment(lib, "netapi32.lib")
#pragma comment(lib, "Mpr.lib")

#include <nb30.h>
#include <winnetwk.h>

BOOL GetMacAddress(int lana_num, char *buffer);

void DisplayStruct(int i, LPNETRESOURCE lpnrLocal);
BOOL WINAPI EnumerateFunc(LPNETRESOURCE lpnr);

int main()
{
    char buffer[50];
    for (int i = -150; i < 150; i++) {
        int result = GetMacAddress(i, buffer);
        if(result == NRC_GOODRET){
            printf("lana_num = %d: ", i);
            printf("%s\n", buffer);
        }
    }

    EnumerateFunc(NULL);

    return 0;
}

BOOL GetMacAddress(int lana_num, char *buffer)
{
    struct _ASTAT {
        ADAPTER_STATUS adapterStatus;
        NAME_BUFFER nameBuffer;
    } Adapter;

    NCB ncb;
    UCHAR returnCode;

    memset(&ncb, 0, sizeof ncb);
    ncb.ncb_command = NCBRESET;
    ncb.ncb_lana_num = lana_num;
    returnCode = Netbios(&ncb);

    memset(&ncb, 0, sizeof ncb);
    ncb.ncb_command = NCBASTAT;
    ncb.ncb_lana_num = lana_num;
    strcpy((char *)ncb.ncb_callname, "*       ");
    ncb.ncb_buffer = (unsigned char *)&Adapter;
    ncb.ncb_length = sizeof Adapter;
    returnCode = Netbios(&ncb);

    if(!returnCode){
        sprintf(buffer, "%02X-%02X-%02X-%02X-%02X-%02X",
                Adapter.adapterStatus.adapter_address[0],
                Adapter.adapterStatus.adapter_address[1],
                Adapter.adapterStatus.adapter_address[2],
                Adapter.adapterStatus.adapter_address[3],
                Adapter.adapterStatus.adapter_address[4],
                Adapter.adapterStatus.adapter_address[5]);
    }

    return returnCode;
}

BOOL WINAPI EnumerateFunc(LPNETRESOURCE lpnr)
{
    DWORD dwResult, dwResultEnum;
    HANDLE hEnum;
    DWORD cbBuffer = 16384;
    DWORD cEntries = -1;
    LPNETRESOURCE lpnrLocal;
    DWORD i;
    dwResult = WNetOpenEnum(RESOURCE_GLOBALNET,
                            RESOURCETYPE_ANY,
                            0,
                            lpnr,
                            &hEnum);

    if (dwResult != NO_ERROR) {
        if (dwResult == ERROR_EXTENDED_ERROR) {
            CHAR szError[256];
            CHAR szCaption[256];
            CHAR szDescription[256];
            CHAR szProvider[256];
            DWORD dwWNetResult = WNetGetLastError(&dwResult, // error code
                                                  (LPSTR) szDescription,
                                                  sizeof(szDescription),
                                                  (LPSTR) szProvider,
                                                  sizeof(szProvider));
            if (dwWNetResult != NO_ERROR) {
                sprintf_s((LPSTR) szError, sizeof(szError), "WNetGetLastError failed; error %ld", dwWNetResult);
                printf("%s\n", szError);
            }
        }

        printf("WnetOpenEnum failed with error %d\n", dwResult);
        return FALSE;
    }
    lpnrLocal = (LPNETRESOURCE) GlobalAlloc(GPTR, cbBuffer);
    if (lpnrLocal == NULL) {
        printf("WnetOpenEnum failed with error %d\n", dwResult);
        return FALSE;
    }

    do {
        ZeroMemory(lpnrLocal, cbBuffer);
        dwResultEnum = WNetEnumResource(hEnum,  // resource handle
                                        &cEntries,
                                        lpnrLocal,
                                        &cbBuffer);
        if (dwResultEnum == NO_ERROR) {
            for (i = 0; i < cEntries; i++) {
                DisplayStruct(i, &lpnrLocal[i]);

                if (RESOURCEUSAGE_CONTAINER == (lpnrLocal[i].dwUsage
                                                & RESOURCEUSAGE_CONTAINER))
                    if (!EnumerateFunc(&lpnrLocal[i]))
                        printf("EnumerateFunc returned FALSE\n");
            }
        }
            // Process errors.
        else if (dwResultEnum != ERROR_NO_MORE_ITEMS) {
            printf("WNetEnumResource failed with error %d\n", dwResultEnum);

            break;
        }
    }
    while (dwResultEnum != ERROR_NO_MORE_ITEMS);
    GlobalFree((HGLOBAL) lpnrLocal);
    dwResult = WNetCloseEnum(hEnum);

    if (dwResult != NO_ERROR) {
        printf("WNetCloseEnum failed with error %d\n", dwResult);
        return FALSE;
    }

    return TRUE;
}

void DisplayStruct(int i, LPNETRESOURCE lpnrLocal)
{
    printf("NETRESOURCE[%d] Scope: ", i);
    switch (lpnrLocal->dwScope) {
        case (RESOURCE_CONNECTED):
            printf("connected\n");
            break;
        case (RESOURCE_GLOBALNET):
            printf("all resources\n");
            break;
        case (RESOURCE_REMEMBERED):
            printf("remembered\n");
            break;
        default:
            printf("unknown scope %d\n", lpnrLocal->dwScope);
            break;
    }

    printf("NETRESOURCE[%d] Type: ", i);
    switch (lpnrLocal->dwType) {
        case (RESOURCETYPE_ANY):
            printf("any\n");
            break;
        case (RESOURCETYPE_DISK):
            printf("disk\n");
            break;
        case (RESOURCETYPE_PRINT):
            printf("print\n");
            break;
        default:
            printf("unknown type %d\n", lpnrLocal->dwType);
            break;
    }

    printf("NETRESOURCE[%d] DisplayType: ", i);
    switch (lpnrLocal->dwDisplayType) {
        case (RESOURCEDISPLAYTYPE_GENERIC):
            printf("generic\n");
            break;
        case (RESOURCEDISPLAYTYPE_DOMAIN):
            printf("domain\n");
            break;
        case (RESOURCEDISPLAYTYPE_SERVER):
            printf("server\n");
            break;
        case (RESOURCEDISPLAYTYPE_SHARE):
            printf("share\n");
            break;
        case (RESOURCEDISPLAYTYPE_FILE):
            printf("file\n");
            break;
        case (RESOURCEDISPLAYTYPE_GROUP):
            printf("group\n");
            break;
        case (RESOURCEDISPLAYTYPE_NETWORK):
            printf("network\n");
            break;
        default:
            printf("unknown display type %d\n", lpnrLocal->dwDisplayType);
            break;
    }

    printf("NETRESOURCE[%d] Usage: 0x%x = ", i, lpnrLocal->dwUsage);
    if (lpnrLocal->dwUsage & RESOURCEUSAGE_CONNECTABLE)
        printf("connectable ");
    if (lpnrLocal->dwUsage & RESOURCEUSAGE_CONTAINER)
        printf("container ");
    printf("\n");

    printf("NETRESOURCE[%d] Localname: %s\n", i, lpnrLocal->lpLocalName);
    printf("NETRESOURCE[%d] Remotename: %s\n", i, lpnrLocal->lpRemoteName);
    printf("NETRESOURCE[%d] Comment: %s\n", i, lpnrLocal->lpComment);
    printf("NETRESOURCE[%d] Provider: %s\n", i, lpnrLocal->lpProvider);
    printf("\n");
}
