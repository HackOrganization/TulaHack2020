from Socket.Messages.Utils.MessageTypes import MessageTypes
import Socket.Utils.ByteConverter as ByteConverter
import Socket.Messages.Utils.Params as Params


# Базовый класс сообщения
class Message:
    MessageLength: int  # int 4
    MessageType: MessageTypes  # byte 1

    def __init__(self):
        self.MessageLength = Params.MESSAGE_HEADER_LENGTH + 1
        self.MessageType = MessageTypes.Null

    def Serialize(self):
        returnArray = bytearray()
        length = Params.MESSAGE_HEADER_LENGTH + 1
        returnArray.extend(ByteConverter.GetBytesFromInteger(length))
        returnArray.extend(ByteConverter.GetBytesFromByteInteger(self.MessageType))
        return returnArray

    @staticmethod
    def Deserialize():
        return Message()
