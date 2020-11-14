from Socket.Messages.Utils.MessageTypes import MessageTypes
import Socket.Utils.ByteConverter as ByteConverter
from Socket.Messages.Message import Message


# Класс сообщения картинки с одной из камер
class ImageMessage(Message):
    PacketId: int  # ushort : 2
    Width: int  # ushort : 2
    Height: int  # ushort : 2
    JpgImageData: bytearray

    def __init__(self, data: bytearray):
        self.MessageLength = ByteConverter.GetInteger(data)
        self.MessageType = MessageTypes(ByteConverter.GetInteger(data, 4, 1))
        self.PacketId = ByteConverter.GetInteger(data, 5, 2, False)
        self.Width = ByteConverter.GetInteger(data, 7, 2, False)
        self.Height = ByteConverter.GetInteger(data, 9, 2, False)
        # ToDo: Get image and convert it
        self.JpgImageData = ByteConverter.GetBytes(data, 11)

    def Serialize(self):
        returnArray = bytearray()
        length = 4 + 1 + 2 + (2 + 2 + len(self.JpgImageData))
        returnArray.extend(ByteConverter.GetBytesFromInteger(length))
        returnArray.extend(ByteConverter.GetBytesFromByteInteger(self.MessageType))
        returnArray.extend(ByteConverter.GetBytesFromUshort(self.PacketId))
        returnArray.extend(ByteConverter.GetBytesFromUshort(self.Width))
        returnArray.extend(ByteConverter.GetBytesFromUshort(self.Height))
        # ToDo: Get image and convert it
        returnArray.extend(self.JpgImageData)
        return returnArray

    @staticmethod
    def Deserialize(data: bytearray):
        return ImageMessage(data)