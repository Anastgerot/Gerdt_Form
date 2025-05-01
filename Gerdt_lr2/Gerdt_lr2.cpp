#include "Session.h"
#include <iostream>
#include <boost/asio.hpp>
#include <thread>
#include <map>
#include <memory>
#include <mutex>
#include <atomic>

using namespace std;
using boost::asio::ip::tcp;

int maxID = MR_USER;
map<int, shared_ptr<Session>> sessions;


void launchClient(wstring path)
{
    wstring pathCopy = path;

    STARTUPINFO si = { sizeof(si) };
    PROCESS_INFORMATION pi;
    if (CreateProcess(NULL, &pathCopy[0],NULL, NULL, TRUE, CREATE_NEW_CONSOLE, NULL, NULL, &si, &pi))
    {
        CloseHandle(pi.hThread);
        CloseHandle(pi.hProcess);
    }
    else
    {
        wcerr << L"Не удалось запустить клиент: " << path << endl;
    }
}

void broadcastUpdate() {
    wstring response;
    for (auto& pair : sessions) {
        response += to_wstring(pair.second->id) + L"|";
    }
    response += L'\0'; 

    wcout << L"Обновление клиентов: " << response << endl;

    int dataSize = static_cast<int>(response.size() * sizeof(wchar_t));
    for (auto& pair : sessions) {
        Message updateMsg(MR_BROKER, pair.first, MT_UPDATE, response);
        pair.second->add(updateMsg);
    }
}

void processClient(tcp::socket s)
{
    try
    {
        Message m;
        int code = m.receive(s);
        cout << m.header.to << ": " << m.header.from << ": " << m.header.type << ": " << code << endl;
        switch (code)
        {
        case MT_INIT:
        {
            auto session = make_shared<Session>(++maxID, m.data);
            sessions[session->id] = session;
            Message::send(s, session->id, MR_BROKER, MT_INIT);
            broadcastUpdate();
            break;
        }
        case MT_EXIT:
        {
            sessions.erase(m.header.from);
            Message::send(s, m.header.from, MR_BROKER, MT_CONFIRM);
            broadcastUpdate();
            break;
        }
        case MT_GETDATA:
        {
            if (!sessions.empty()) {
                size_t sepPos = m.data.find(L'|');
                if (sepPos != wstring::npos) {
                    int id = stoi(m.data.substr(0, sepPos));
                    wstring text = m.data.substr(sepPos + 1);
         
                    if (id == 0) {
                        wcout << L"Сообщение всем клиентам: " << text << endl;
                        for (auto& pair : sessions) {
                            Message message(m.header.to, m.header.from, MT_DATA, text);
                            pair.second->add(message);
                        }
                    }
                    else if (id > 0 && id <= sessions.size()) {
                        auto iSession = sessions.find(m.header.from);
                        if (iSession != sessions.end())
                        {
                            iSession->second->send(s);
                        }

                    }
                }
                break;
            }
        }
        case MT_UPDATE: {
            wstring response;
            for (auto& s : sessions) {
                response += to_wstring(s.second->id) + L"|";
            }
            response += L'\0';

            int dataSize = static_cast<int>(response.size() * sizeof(wchar_t));
            sendData(s, &dataSize, sizeof(dataSize));
            sendData(s, response.c_str(), dataSize);

            break;
        }
        default:
        {
            auto iSessionFrom = sessions.find(m.header.from);
            if (iSessionFrom != sessions.end())
            {
                auto iSessionTo = sessions.find(m.header.to);
                if (iSessionTo != sessions.end())
                {
                    iSessionTo->second->add(m);
                }
                else if (m.header.to == MR_ALL)
                {
                    for (auto& pair : sessions)
                    {
                        if (pair.first != m.header.from)
                            pair.second->add(m);
                    }
                }
                Message::send(s, m.header.from, MR_BROKER, MT_CONFIRM);
            }
            break;
        }
      }
    }
    catch (std::exception& e)
    {
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

        launchClient(L"C:/Users/anast/OneDrive/Документы/GitHub/Gerdt_SystemProgram3/Debug/Gerdt_Form.exe");
        launchClient(L"C:/Users/anast/OneDrive/Документы/GitHub/Gerdt_SystemProgram3/Debug/Gerdt_Form.exe");


        while (true) {

            thread(processClient, a.accept()).detach();
        }
    }
    catch (const exception& e) {
        wcerr << L"Ошибка сервера: " << e.what() << endl;
    }

    return 0;
}
