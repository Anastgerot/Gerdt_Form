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

vector<Session*> sessions;
mutex mtx;
atomic<int> threadCounter(1); 

struct header {
    int id;
    int commandId;
    int size;
};

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

void processClient(tcp::socket s) {
    try {
        Message m;
        int code = m.receive(s);
        wcout << "TYPE " << m.header.messageType << endl;

        switch (code) {
        case MT_INIT: 
        {
            int count = stoi(m.data);
            for (int i = 1; i <= count; i++) {
                int newSessionID = threadCounter.fetch_add(1); 
                Session* cSession = new Session(newSessionID);
                sessions.push_back(cSession);
                thread t(MyThread, cSession);
                t.detach();
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

        launchClient(L"C:/Users/anast/OneDrive/Документы/GitHub/Gerdt_SystemProgram2/Debug/Gerdt_Form.exe");


        while (true) {

            thread(processClient, a.accept()).detach();
        }
    }
    catch (const exception& e) {
        wcerr << L"Ошибка сервера: " << e.what() << endl;
    }

    return 0;
}
