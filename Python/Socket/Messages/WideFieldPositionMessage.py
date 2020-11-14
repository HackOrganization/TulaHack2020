import Socket.Messages.Utils.Params as Params
import Socket.Utils.ByteConverter as ByteConverter

from Socket.Messages.Message import Message
from Socket.Messages.Utils.MessageTypes import MessageTypes


# Класс сообщения координат объекта на картинке
class WideFieldPositionMessage(Message):
    PacketId: int  # ushort 2
    PositionX: int  # short : 2
    PositionY: int  # short : 2
    SizeX: int  # ushort : 2
    SizeY: int  # ushort : 2

    def __init__(self, packetId: int, positionX: int, positionY: int, sizeX: int, sizeY: int):
        length = Params.MESSAGE_HEADER_LENGTH + 1 + 2 + (2 + 2) + (2 + 2)
        self.MessageLength = length
        self.MessageType = MessageTypes.WideFieldPosition
        self.PacketId = packetId
        self.PositionX = positionX
        self.PositionY = positionY
        self.SizeX = sizeX
        self.SizeY = sizeY

    def Serialize(self):
        returnArray = bytearray()
        returnArray.extend(ByteConverter.GetBytesFromInteger(self.MessageLength))
        returnArray.extend(ByteConverter.GetBytesFromByteInteger(self.MessageType))
        returnArray.extend(ByteConverter.GetBytesFromUshort(self.PacketId))
        returnArray.extend(ByteConverter.GetBytesFromShort(self.PositionX))
        returnArray.extend(ByteConverter.GetBytesFromShort(self.PositionY))
        returnArray.extend(ByteConverter.GetBytesFromUshort(self.SizeX))
        returnArray.extend(ByteConverter.GetBytesFromUshort(self.SizeY))
        return returnArray
