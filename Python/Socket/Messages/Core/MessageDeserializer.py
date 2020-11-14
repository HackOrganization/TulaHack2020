from Socket.Messages.CloseConnectionMessage import CloseConnectionMessage
from Socket.Messages.Utils.MessageTypes import MessageTypes
from Socket.Messages.ImageMessage import ImageMessage
from Socket.Messages.Message import Message


# Возвращает типизированный класс сообщения
def Deserialize(messageType: MessageTypes, data: bytearray):
    if messageType == MessageTypes.CloseConnection:
        return CloseConnectionMessage.Deserialize(data)
    if messageType == MessageTypes.WideFieldImage or messageType == MessageTypes.TightFieldImage:
        message = ImageMessage.Deserialize(data)
        # print(f"Received image length: {len(message.JpgImageData)}")
        return message

    return Message.Deserialize()
