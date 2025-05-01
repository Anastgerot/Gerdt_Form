#pragma once
#include "C:/Users/anast/OneDrive/Документы/GitHub/Gerdt_SystemProgram3/Gerdt_DLL/asio.h"

enum MessageTypes
{
	MT_INIT,
	MT_EXIT,
	MT_GETDATA,
	MT_DATA,
	MT_UPDATE,
	MT_NODATA,
	MT_CONFIRM

};

enum MessageRecipients
{
	MR_BROKER = 10,
	MR_ALL = 50,
	MR_USER = 0
};


struct MessageHeader
{
	int to;
	int from;
	int type;
	int size;

};

struct Message
{
	MessageHeader header = { 0 };
	wstring data;
	static int clientID;

	Message() {}
	Message(int to, int from, int type = MT_DATA, const wstring& data = L"")
	{
		this->data = data;
		header = { to, from, type, int(data.length() * sizeof(wchar_t)) };
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
		return header.type;
	}
	static void send(tcp::socket& s, int to, int from, int type = MT_DATA, const wstring& data = L"");
	static Message send(int to, int type = MT_DATA, const wstring& data = L"");

};

