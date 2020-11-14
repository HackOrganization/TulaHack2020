import Socket.Messages.Utils.Params as Params
import Socket.Utils.ByteConverter as ByteConverter

from Socket.Messages.Message import Message
from Socket.Messages.Utils.MessageTypes import MessageTypes


# Класс сообщения координат объекта на картинке
class WideFieldPositionMessage(Message):
    PositionX: int  # short : 2
    PositionY: int  # short : 2
    SizeX: int  # ushort : 2
    SizeY: int  # ushort : 2

    def __init__(self, position: tuple, size: tuple):
        length = Params.MESSAGE_HEADER_LENGTH + 1 + (2 + 2) + (2 + 2)
        self.MessageLength = length
        self.MessageType = MessageTypes.WideFieldPosition
        self.PositionX = position[0]
        self.PositionY = position[1]
        self.SizeX = size[0]
        self.SizeY = size[1]

    def Serialize(self):
        returnArray = bytearray()
        returnArray.extend(ByteConverter.GetBytesFromInteger(self.MessageLength))
        returnArray.extend(ByteConverter.GetBytesFromByteInteger(self.MessageType))
        returnArray.extend(ByteConverter.GetBytesFromShort(self.PositionX))
        returnArray.extend(ByteConverter.GetBytesFromShort(self.PositionY))
        returnArray.extend(ByteConverter.GetBytesFromUshort(self.SizeX))
        returnArray.extend(ByteConverter.GetBytesFromUshort(self.SizeY))
        return returnArray
