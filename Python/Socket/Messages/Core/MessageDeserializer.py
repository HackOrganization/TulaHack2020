from Socket.Messages.Utils.MessageTypes import MessageTypes
from Socket.Messages.CloseConnectionMessage import CloseConnectionMessage
from Socket.Messages.ImageMessage import ImageMessage
from Socket.Messages.Message import Message


# Возвращает типизированный класс сообщения
def Deserialize(messageType: MessageTypes, data: bytearray):
    if messageType == MessageTypes.CloseConnection:
        return CloseConnectionMessage.Deserialize()
    if messageType == MessageTypes.WideFieldImage or messageType == MessageTypes.TightFieldImage:
        return ImageMessage.Deserialize(data)

    return Message.Deserialize()