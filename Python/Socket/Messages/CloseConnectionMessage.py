import Socket.Messages.Utils.Params as Params
import Socket.Utils.ByteConverter as ByteConverter

from Socket.Messages.Message import Message
from Socket.Messages.Utils.MessageTypes import MessageTypes


# Класс сообщения о закрытии сокета
class CloseConnectionMessage(Message):
    SendGoodbyeMessage: bool

    def __init__(self, sendGoodbyeMessage):
        length = Params.MESSAGE_HEADER_LENGTH + 1 + 1
        self.MessageLength = length
        self.MessageType = MessageTypes.CloseConnection
        self.SendGoodbyeMessage = sendGoodbyeMessage

    def Serialize(self):
        returnArray = bytearray()
        returnArray.extend(ByteConverter.GetBytesFromInteger(self.MessageLength))
        returnArray.extend(ByteConverter.GetBytesFromByteInteger(self.MessageType))
        returnArray.extend(ByteConverter.GetBytesFromBool(self.SendGoodbyeMessage))
        return returnArray

    @staticmethod
    def Deserialize(data: bytearray):
        offset = Params.MESSAGE_HEADER_LENGTH + 1
        return CloseConnectionMessage(ByteConverter.GetBool(data, offset=offset))
