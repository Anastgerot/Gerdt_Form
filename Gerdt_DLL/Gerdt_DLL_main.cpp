#include "pch.h"
#include "framework.h"
#include <boost/asio.hpp>
#include <iostream> 
#include <string>
#include <mutex>
#include "asio.h"
#include "C:/Users/anast/OneDrive/Документы/GitHub/Gerdt_SystemProgram2/Gerdt_lr2/Message.h"

using namespace std;
using boost::asio::ip::tcp;

boost::asio::io_context io;
unique_ptr<tcp::socket> g_socket;
mutex g_socketMutex;



extern "C" {

    // Установка соединения с сервером
    _declspec(dllexport) bool initconnect() {
        try {

            if (!g_socket) {
                tcp::resolver resolver(io);
                tcp::resolver::results_type endpoints = resolver.resolve("127.0.0.1", "12345");
                g_socket = make_unique<tcp::socket>(io);
                boost::asio::connect(*g_socket, endpoints);
            }
            return true;
        }
        catch (const std::exception& e) {
            wcerr << L"Ошибка при подключении к серверу: " << e.what() << endl;
            return false;
        }
    }

    _declspec(dllexport) void __stdcall sendCommand(int commandId, const wchar_t* message)
    {
        try {
            boost::asio::io_context io;
            tcp::socket socket(io);
            tcp::resolver resolver(io);
            auto endpoints = resolver.resolve("127.0.0.1", "12345");
            boost::asio::connect(socket, endpoints);

            MessageHeader header;
            header.messageType = commandId;
            header.size = message ? static_cast<int>(wcslen(message) * sizeof(wchar_t)) : 0;

            sendData(socket, &header, sizeof(header));
            if (header.size > 0)
                sendData(socket, message, header.size);
        }
        catch (const std::exception& e) {
            std::wcerr << L"[DLL] Exception in sendCommand: " << e.what() << std::endl;
        }
    }

}
