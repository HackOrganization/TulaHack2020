import time
import socket as SocketLib
import Socket.Utils.Params as Params
import Socket.Messages.Utils.Params as MessageParams
import Socket.Utils.ByteConverter as ByteConverter
import Socket.Messages.Core.MessageDeserializer as MessageDeserializer
import Socket.Messages.Core.MessageResponser as MessageResponser

from threading import Thread
from Socket.Utils.ReceiveObject import ReceiveObject
from Socket.Messages.Utils.MessageTypes import MessageTypes
from Socket.Messages.CloseConnectionMessage import CloseConnectionMessage


class SocketListener:
    Listener: SocketLib.socket
    ListenerEnabled: bool
    Clients: list
    AcceptThread: Thread

    def __init__(self, address: str, port: int, connections_count: int):
        self.ListenerEnabled = True
        self.Clients = []
        self.Listener = SocketLib.socket(SocketLib.AF_INET, SocketLib.SOCK_STREAM)
        self.Listener.setsockopt(SocketLib.SOL_SOCKET, SocketLib.SO_REUSEADDR, 1)
        self.Listener.bind((address, port))
        self.Listener.listen(connections_count)
        self.Listener.settimeout(Params.LISTENER_TIMEOUT)

        self.AcceptThread = ServerAcceptConnectionThread(self)
        self.AcceptThread.start()

    def Dispose(self):
        self.ListenerEnabled = False


class ServerAcceptConnectionThread(Thread):
    SocketListener: SocketListener
    ClientThreads: list

    def __init__(self, socketListener: SocketListener):
        Thread.__init__(self)
        self.daemon = True
        self.SocketListener = socketListener
        self.ClientThreads = []

    def run(self):
        print("Accepting new connections...")
        while self.SocketListener.ListenerEnabled:
            try:
                clientSocket, clientAddress = self.SocketListener.Listener.accept()
                newThread = ClientThread(self.SocketListener, clientAddress, clientSocket)

                self.SocketListener.Clients.append(clientSocket)
                self.ClientThreads.append(newThread)

                newThread.start()
            except SocketLib.timeout:
                continue

        for thread in self.ClientThreads:
            thread.join()

        print("Accepting connections closed!")
        self.Dispose()

    def Dispose(self):
        while len(self.SocketListener.Clients) > 0:
            time.sleep(Params.CLOSE_STEP_TIMEOUT)

        self.SocketListener.Listener.close()
        print("Listener closed!")


class ClientThread(Thread):
    SocketListener: SocketListener
    Client: SocketLib.socket

    def __init__(self, socketListener: SocketListener, clientAddress, clientSocket: SocketLib.socket):
        Thread.__init__(self)
        self.daemon = True
        self.SocketListener = socketListener
        self.Client = clientSocket
        self.Client.settimeout(Params.LISTENER_TIMEOUT)

        print(f"New connection established: {clientAddress}")

    def run(self):
        __receiveEnabled = True
        while __receiveEnabled and self.SocketListener.ListenerEnabled:
            try:
                # print("Awaiting message")
                receiveObject = ReceiveObject()
                while not receiveObject.IsSameLength():
                    receiveObject.buffer.extend(self.Client.recv(Params.BUFFER_SIZE))
                    receiveObject.CashedMessage.extend(receiveObject.buffer)
                    receiveObject.buffer.clear()

                # Определяем тип сообщения
                messageType = MessageTypes(
                    ByteConverter.GetInteger(
                        receiveObject.CashedMessage, MessageParams.MESSAGE_HEADER_LENGTH, 1))
                # Получаем объект сообщения
                message = MessageDeserializer.Deserialize(messageType, receiveObject.CashedMessage)
                # Очищаем кэш
                receiveObject.CashedMessage.clear()

                # Выполняем действие, связанное с этим сообщением
                # Если сообщение окончания работы сокета, то перкращаем чтение и закрываем сокет
                __receiveEnabled = MessageResponser.SendResponse(self.Client, messageType, message)
            except SocketLib.timeout:
                continue

        print(f"Calling close client {self.Client}")
        message = CloseConnectionMessage(not __receiveEnabled).Serialize()
        self.Client.send(message)
        self.Client.shutdown(SocketLib.SHUT_RDWR)
        self.Client.close()
        if self.Client in self.SocketListener.Clients:
            self.SocketListener.Clients.remove(self.Client)
        else:
            print(f"Socket [{self.Client}] not registered in list")
