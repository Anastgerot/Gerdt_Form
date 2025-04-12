#pragma once
#include "C:/Users/anast/OneDrive/Документы/GitHub/Gerdt_SystemProgram2/Gerdt_DLL/asio.h"

enum MessageTypes
{
	MT_CLOSE,
	MT_DATA,
	MT_INIT,
	MT_EXIT,
	MT_GETDATA
};


struct MessageHeader
{
	int messageType;
	int size;
};

struct Message
{
	MessageHeader header = { 0 };
	wstring data;
	Message() = default;
	Message(MessageTypes messageType, const wstring& data = L"")
		:data(data)
	{
		header = { messageType,  int(data.length() * sizeof(wchar_t)) };
	}

	void send(tcp::socket& s)
		{
			sendData(s, &header);
			if (header.size)
			{
				sendData(s, data.c_str(), header.size);
			}
		}

	int receive(tcp::socket& s)
	{
		receiveData(s, &header);
		if (header.size)
		{
			data.resize(header.size / sizeof(wchar_t));
			receiveData(s, &data[0], header.size);
		}
		return header.messageType;
	}
};

