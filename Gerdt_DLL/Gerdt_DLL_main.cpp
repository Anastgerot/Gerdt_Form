#include "pch.h"
#include "framework.h"
#include <boost/asio.hpp>
#include <iostream> 
#include <string>
#include <mutex>
#include "asio.h"
#include "C:/Users/anast/OneDrive/Документы/GitHub/Gerdt_SystemProgram3/Gerdt_lr2/Message.h"
#include <thread>

using namespace std;
using boost::asio::ip::tcp;

boost::asio::io_context io;
unique_ptr<tcp::socket> g_socket;
mutex g_socketMutex;
wchar_t* result = nullptr;

extern "C" {
    _declspec(dllexport) wchar_t* __stdcall getCountClients() {
        return result;
    }

    _declspec(dllexport) void __stdcall sendCommand(int commandId, const wchar_t* message)
    {
        boost::asio::io_context io;
        tcp::socket socket(io);
        tcp::resolver resolver(io);
        auto endpoints = resolver.resolve("127.0.0.1", "12345");

        boost::asio::connect(socket, endpoints);
        MessageHeader header;
        header.type = commandId;
        header.size = message ? static_cast<int>(wcslen(message) * sizeof(wchar_t)) : 0;


        sendData(socket, &header, sizeof(header));
        if (header.size > 0)
            sendData(socket, message, header.size);
            


        if (header.type == MT_UPDATE) {
            int dataSize;
            receiveData(socket, &dataSize);

            if (result) {
                delete[] result;
                result = nullptr;
            }

            result = new wchar_t[(dataSize / sizeof(wchar_t))]();
            receiveData(socket, result, dataSize);
        }
    }

}
