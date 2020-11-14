from io import BytesIO
from PIL import Image
from Socket.Messages.ImageMessage import ImageMessage


class JpgDecoder:

    @staticmethod
    def copy(image):
        image.load()
        image.im = image.im.copy()
        image.pyaccess = None
        image.readonly = 0

    @staticmethod
    def save(image: Image):
        JpgDecoder.copy(image)

        image.encoderinfo = dict()
        image.encoderconfig = (-1, False, 0, False, 0, 0, 0, -1, None, bytes(), bytes())

    @staticmethod
    def Decode(message: ImageMessage):
        with BytesIO(message.JpgImageData) as stream:
            image = Image.open(stream)
            JpgDecoder.save(image)
            #imageName = f"server{message.PacketId}.jpg"
            # print(f"{imageName} size: {image.size}")
            #image.save(imageName)

        return image
