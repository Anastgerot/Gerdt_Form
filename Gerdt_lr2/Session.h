#pragma once
#include "C:/Users/anast/OneDrive/Документы/GitHub/Gerdt_SystemProgram2/Gerdt_DLL/asio.h"
#include "Message.h"

class Session
{
private:
	queue<Message> messages = {};
public:

	int sessionID;
	Session(int sessionID):sessionID(sessionID)
	{
	}

	void addMessage(Message& m)
	{
		messages.push(m);
	}

	bool getMessages(Message& m)
	{
		bool res = false;
		if (!messages.empty())
		{
			res = true;
			m = messages.front();
			messages.pop();
		}
		return res;
	}

	void addMessage(MessageTypes messageType, const wstring& data = L"")
	{
		Message m(messageType, data);
		addMessage(m);
	}
};
