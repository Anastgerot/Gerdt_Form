#include "pch.h"
#include "framework.h"
#include <boost/asio.hpp>
#include <iostream> 
#include <string>
#include <mutex>
#include "asio.h"

using namespace std;
using boost::asio::ip::tcp;

boost::asio::io_context io;
unique_ptr<tcp::socket> g_socket;
mutex g_socketMutex;


struct MessageHeader {
    int type;
    int size;
};

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

    _declspec(dllexport) bool sendMessage(int type, const wchar_t* data) {
        try {
            if (!g_socket || !g_socket->is_open()) return false;

            if (!data) {
                wcerr << L"Ошибка: data == nullptr" << endl;
                return false;
            }

            wstring messageData(data);
            MessageHeader header{ type, static_cast<int>(messageData.length() * sizeof(wchar_t)) };

            if (header.size == 0) {
                wcerr << L"Ошибка: пустое сообщение" << endl;
                return false;
            }

            // Логирование данных
            wcout << L"Отправка данных: " << messageData << endl;
            sendData(*g_socket, &header);
            sendData(*g_socket, messageData.c_str(), header.size);

            return true;
        }
        catch (const std::exception& e) {
            wcerr << L"Ошибка в sendMessage: " << e.what() << endl;
            return false;
        }
    }

    //_declspec(dllexport) bool sendMessage(int type, const wchar_t* data) {

    //    if (!g_socket || !g_socket->is_open()) return false;

    //    wstring messageData(data);
    //    MessageHeader header{ type, static_cast<int>(messageData.length() * sizeof(wchar_t)) };

    //    sendData(*g_socket, &header);
    //    sendData(*g_socket, messageData.c_str(), header.size);

    //    return true;

    //}


}
