from Socket.Messages.Utils.MessageTypes import MessageTypes
import Socket.Utils.ByteConverter as ByteConverter
from Socket.Messages.Message import Message
from Socket.Messages.ImageMessage import ImageMessage


class WideFieldPositionMessage(Message):
    PacketId: int  # ushort 2
    PositionX: int  # ushort : 2
    PositionY: int  # ushort : 2

    def __init__(self, packetId: int, positionX: int, positionY: int):
        self.Initialize(packetId, positionX, positionY)

    def __init__(self, packetId: int, message: ImageMessage):
        self.Initialize(packetId, range(0, message.Width, message.Height))

    def Initialize(self, packetId: int, positionX: int, positionY: int):
        length = 4 + 1 + 2 + 2
        self.MessageLength = length
        self.MessageType = MessageTypes.WideFieldPosition
        self.PacketId = packetId
        self.PositionX = positionX
        self.PositionY = positionY

    def Serialize(self):
        returnArray = bytearray()
        returnArray.extend(ByteConverter.GetBytesFromInteger(self.MessageLength))
        returnArray.extend(ByteConverter.GetBytesFromByteInteger(self.MessageType))
        returnArray.extend(ByteConverter.GetBytesFromUshort(self.PacketId))
        returnArray.extend(ByteConverter.GetBytesFromUshort(self.PositionX))
        returnArray.extend(ByteConverter.GetBytesFromUshort(self.PositionY))
        return returnArray
