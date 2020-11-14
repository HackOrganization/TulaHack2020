import Socket.Messages.Utils.Params as Params
import Socket.Utils.ByteConverter as ByteConverter

from Socket.Messages.Message import Message
from Socket.Messages.Utils.MessageTypes import MessageTypes


# Класс сообщения картинки с одной из камер
class ImageMessage(Message):
    Width: int  # ushort : 2
    Height: int  # ushort : 2
    JpgImageData: bytes  # : len()

    def __init__(self, data: bytearray):
        self.MessageLength = len(data)
        self.MessageType = MessageTypes(ByteConverter.GetInteger(data, 4, 1))
        self.Width = ByteConverter.GetInteger(data, 5, 2, False)
        self.Height = ByteConverter.GetInteger(data, 7, 2, False)
        self.JpgImageData = bytes(data[9:])  # ByteConverter.GetBytes(data, 9)

    def Serialize(self):
        returnArray = bytearray()
        length = Params.MESSAGE_HEADER_LENGTH + 1 + 2 + (2 + 2 + len(self.JpgImageData))
        returnArray.extend(ByteConverter.GetBytesFromInteger(length))
        returnArray.extend(ByteConverter.GetBytesFromByteInteger(self.MessageType))
        returnArray.extend(ByteConverter.GetBytesFromUshort(self.Width))
        returnArray.extend(ByteConverter.GetBytesFromUshort(self.Height))
        returnArray.extend(self.JpgImageData)
        return returnArray

    @staticmethod
    def Deserialize(data: bytearray):
        return ImageMessage(data)
