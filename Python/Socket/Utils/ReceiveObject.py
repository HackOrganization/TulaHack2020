import Socket.Utils.ByteConverter as ByteConverter


# Сущность данных, получаемых от клиента
# Аккумулирует в себе пакеты, пока сообщение не придет полностью
class ReceiveObject:
    buffer = bytearray()
    CashedMessage = bytearray()

    __messageSize = -1

    def MessageSize(self):
        if len(self.CashedMessage) == 0:
            return self.__messageSize
        valueByteArray = bytearray()
        for i in range(4):
            valueByteArray.append(self.CashedMessage[i])
        self.__messageSize = ByteConverter.GetInteger(valueByteArray)
        return self.__messageSize

    def IsSameLength(self):
        return self.MessageSize() == len(self.CashedMessage)
