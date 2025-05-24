#pragma once
#include "Message.h"
#include "asio.h"

class Session
{
private:
	queue<Message> messages = {};
public:
	int id;
	wstring name;

	mutex mx;
	Session(int id, wstring name)
		:id(id), name(name)
	{
	}

	queue<Message> getMessages() {
		return this->messages;
	}

	void add(Message& m)
	{
		lock_guard<mutex> lg(mx);
		messages.push(m);
	}

	void send(tcp::socket& s)
	{
		lock_guard<mutex> lg(mx);
		if (messages.empty())
		{
			Message::send(s, id, MR_BROKER, MT_NODATA);
		}
		else
		{
			messages.front().send(s);
			messages.pop();
		}
	}
};
