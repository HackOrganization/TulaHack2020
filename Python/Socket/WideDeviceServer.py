import socket as SocketLib
import Socket.Utils.Params as Params
import Socket.Utils.ByteConverter as ByteConverter
import Socket.Messages.Core.MessageDeserializer as MessageDeserializer
import Socket.Messages.Core.MessageResponser as MessageResponser

from Socket.Utils.ReceiveObject import ReceiveObject
from Socket.Messages.Utils.MessageTypes import MessageTypes


# Создает серверный сокет и обрабатывает входящие подключения и получение сообщений
def CreateListener(address, port, connections_count):
    listener = SocketLib.socket()
    listener.bind((Params.WIDE_FIELD_ADDRESS, Params.WIDE_FIELD_PORT))
    listener.listen(1)

    client, address = listener.accept()
    print(f"Connected: {address}")

    __receiveEnabled = True
    while __receiveEnabled:

        print("Awaiting message")
        receiveObject = ReceiveObject()
        while not receiveObject.IsSameLength():
            receiveObject.buffer.extend(client.recv(Params.BUFFER_SIZE))
            receiveObject.CashedMessage.extend(receiveObject.buffer)
            receiveObject.buffer.clear()

        messageType = MessageTypes(ByteConverter.GetInteger(receiveObject.CashedMessage, 4, 1))
        message = MessageDeserializer.Deserialize(messageType, receiveObject.CashedMessage)
        receiveObject.CashedMessage.clear()

        MessageResponser.SendResponse(client, messageType, message)
