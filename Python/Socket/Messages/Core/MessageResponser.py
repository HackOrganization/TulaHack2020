import socket as SocketLib

from Core.EventManager import EventManager
from Core.Utils.EventType import EventType
from Socket.Messages.Utils.MessageTypes import MessageTypes
from Socket.Messages.WideFieldPositionMessage import WideFieldPositionMessage


# Выполняет действие в соотвествии с типом входящего сообщения и отправляет результат клиенту
def SendResponse(client: SocketLib.socket, messageType: MessageTypes, message):
    if messageType == MessageTypes.CloseConnection:
        print("Close connection")
        return False
    elif messageType == MessageTypes.WideFieldImage:
        # Send message to MotionDetection controller
        EventManager.RaiseEvent(EventType.WideFieldImageGotten, kwargs={'client': client, 'message': message})


    elif messageType == MessageTypes.TightFieldImage:
        # ToDo: Save Id
        # ToDo: Send to neural
        print("[TightField] Send coordinates")

    return True
