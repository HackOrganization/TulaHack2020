from Socket.Messages.Message import Message
import Socket.Utils.ByteConverter as ByteConverter
from Socket.Messages.Utils.MessageTypes import MessageTypes


# Класс сообщения о закрытии сокета
class CloseConnectionMessage(Message):
    def __init__(self):
        self.MessageLength = 5
        self.MessageType = MessageTypes.CloseConnection

    def Serialize(self):
        returnArray = bytearray()
        length = 4 + 1
        returnArray.extend(ByteConverter.GetBytesFromInteger(length))
        returnArray.extend(ByteConverter.GetBytesFromByteInteger(self.MessageType))
        return returnArray

    @staticmethod
    def Deserialize():
        return CloseConnectionMessage()