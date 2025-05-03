#include "pch.h"
#include "framework.h"
#include <boost/asio.hpp>
#include <iostream> 
#include <string>
#include <mutex>
#include "asio.h"
#include "C:/Users/anast/OneDrive/Документы/GitHub/Gerdt_SystemProgram3/Gerdt_lr2/Message.h"
#include "C:/Users/anast/OneDrive/Документы/GitHub/Gerdt_SystemProgram3/Gerdt_lr2/Message.cpp"
#include <thread>

using namespace std;
using boost::asio::ip::tcp;

boost::asio::io_context io;
unique_ptr<tcp::socket> g_socket;
mutex g_socketMutex;
wchar_t* result = nullptr;


extern "C" {


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
    }

    _declspec(dllexport) wchar_t* __stdcall UpdateState(int type) {
        // Очистка старого результата
        if (result) {
            delete[] result;
            result = nullptr;
        }

        // Создаем сокет и подключаемся
        boost::asio::io_context io;
        tcp::socket socket(io);
        tcp::resolver resolver(io);
        auto endpoints = resolver.resolve("127.0.0.1", "12345");
        boost::asio::connect(socket, endpoints);

        // Определяем команду на основе типа
        MessageHeader header;
        header.type = 0;
        header.size = 0;
        header.type = (type == 0) ? MT_UPDATE : MT_UPDATE_MESSAGES;


        sendData(socket, &header, sizeof(header));

        int dataSize;
        receiveData(socket, &dataSize);

        if (result) {
            delete[] result;
            result = nullptr;
        }

        result = new wchar_t[(dataSize / sizeof(wchar_t))]();
        receiveData(socket, result, dataSize);

        return result;
    }
}