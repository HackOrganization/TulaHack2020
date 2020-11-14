from io import BytesIO
from PIL import Image
from Socket.Messages.ImageMessage import ImageMessage


class JpgDecoder:

    @staticmethod
    def Decode(message: ImageMessage):
        with BytesIO(message.JpgImageData) as stream:
            image = Image.open(stream)
            image.save(f"server{message.PacketId}.jpg")
        return image
