#include "Session.h"
#include <iostream>
#include <boost/asio.hpp>
#include <thread>
#include <map>
#include <memory>
#include <mutex>

using namespace std;
using boost::asio::ip::tcp;

vector<Session*> sessions;
mutex mtx; 

struct header {
    int id;
    int commandId;
    int size;
};


void MyThread(Session* ses) {
    {
    lock_guard<mutex> lock(mtx);
    wcout << L"Поток " << ses->sessionID << L" работает" << endl;
    }
    while (true) {

        Message m;
        if (ses->getMessages(m)) {
            if (m.header.messageType == MT_CLOSE) {
                lock_guard<mutex> lock(mtx);
                wcout << L"Поток " << ses->sessionID << L" закрыт" << endl;

                auto it = std::find(sessions.begin(), sessions.end(), ses);
                if (it != sessions.end()) {
                    sessions.erase(it);
                }

                delete ses;
                break;
            }

            else if (m.header.messageType == MT_DATA) {
                wstring filename = to_wstring(ses->sessionID) + L".txt";
                HANDLE hFile = CreateFileW(filename.c_str(), GENERIC_WRITE, FILE_SHARE_WRITE | FILE_SHARE_READ, nullptr, CREATE_ALWAYS, FILE_ATTRIBUTE_NORMAL, nullptr);
                if (hFile != INVALID_HANDLE_VALUE) {
                    WCHAR bom = 0xFEFF;
                    DWORD bytesWritten;
                    WriteFile(hFile, &bom, sizeof(WCHAR), &bytesWritten, NULL);

                    // Запись данных
                    WriteFile(hFile, m.data.c_str(), static_cast<DWORD>(m.data.length() * sizeof(wchar_t)), &bytesWritten, NULL);
                    CloseHandle(hFile);
                }
            }
        }
        this_thread::sleep_for(chrono::milliseconds(300));
    }
}

// Функция для обработки клиентов
void processClient(tcp::socket s) {
    try {
        Message m;
        int code = m.receive(s);
        cout << "TYPE " << m.header.messageType << endl;
        wcout << "data " << m.data << endl;

        switch (code) {
        case MT_INIT: {

            try {
                int count = stoi(m.data);
                for (int i = 1; i <= count; i++) {
                    Session* cSession = new Session(i);
                    sessions.push_back(cSession);
                    thread t(MyThread, cSession);
                    t.detach();
                }
            }
            catch (const std::exception& e) {
                wcout << "Error creating session: " << e.what() << endl;
            }
            break;
        }

        case MT_EXIT: {
            {
                lock_guard<mutex> lock(mtx);
                Session* cSession = sessions.back();
                Message m(MT_CLOSE);
                cSession->addMessage(m);
            }
            break;
        }

        //case MT_GETDATA: {
        //    {
 /*               if (!sessions.empty()) {
                    header h;
                    wstring m = getMessage(h);

                    if (h.id == -1) {
                        wcout << L"Главный поток: " << m << endl;
                        SetEvent(events[0]);
                    }
                    else if (h.id == 0) {
                        wcout << L"Сообщение всем потокам: " << m << endl;
                        for (auto& c : sessions) {
                            Message message(MT_DATA, m);
                            c->addMessage(message);
                        }
                        SetEvent(events[0]);
                    }
                    else {
                        if (h.id > 0 && h.id <= sessions.size()) {
                            Session* cSession = sessions[h.id - 1];
                            Message message(MT_DATA, m);
                            cSession->addMessage(message);
                        }
                        SetEvent(events[0]);
                    }
                }
                break;*/
        //    }
        //    break;
        //}

        //default: {
        //    // Обработка других типов сообщений
        //    {
        //        lock_guard<mutex> lock(mtx);
        //        auto iSessionFrom = sessions.find(m.header.from);
        //        if (iSessionFrom != sessions.end()) {
        //            auto iSessionTo = sessions.find(m.header.to);
        //            if (iSessionTo != sessions.end()) {
        //                iSessionTo->second->add(m);  // Добавление сообщения в сессию получателя
        //            }
        //            else if (m.header.to == MR_ALL) {
        //                // Отправка сообщения всем сессиям, кроме отправителя
        //                for (auto it = sessions.begin(); it != sessions.end(); ++it) {
        //                    if (it->first != m.header.from) {
        //                        it->second->add(m);
        //                    }
        //                }
        //            }
        //            Message::send(s, m.header.from, MR_BROKER, MT_CONFIRM);  // Подтверждение получения сообщения
        //        }
        //    }
        //    break;
        //}
        }
    }
    catch (std::exception& e) {
        std::wcerr << "Exception: " << e.what() << endl;
    }
}


int main() {
    try {
        locale::global(locale("rus_rus.866"));
        wcin.imbue(locale());
        wcout.imbue(locale());

        int port = 12345;
        boost::asio::io_context io;
        tcp::acceptor a(io, tcp::endpoint(tcp::v4(), port));


        wcout << L"Сервер запущен..." << endl;

        while (true) {

            thread(processClient, a.accept()).detach();
        }
    }
    catch (const exception& e) {
        wcerr << L"Ошибка сервера: " << e.what() << endl;
    }

    return 0;
}