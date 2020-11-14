from io import BytesIO
from PIL import Image
from Socket.Messages.ImageMessage import ImageMessage


class JpgDecoder:

    @staticmethod
    def Decode(message: ImageMessage):
        with BytesIO(message.JpgImageData) as stream:
            image = Image.open(stream)
            # imageName = f"server{message.PacketId}.jpg"
            # print(f"{imageName} size: {image.size}")
            # image.save(imageName)
        return image
