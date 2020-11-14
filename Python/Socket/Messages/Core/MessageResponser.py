import socket as SocketLib

from Socket.Messages.Utils.MessageTypes import MessageTypes
from Socket.Messages.WideFieldPositionMessage import WideFieldPositionMessage


# Выполняет действие в соотвествии с типом входящего сообщения и отправляет результат клиенту
def SendResponse(client: SocketLib.socket, messageType: MessageTypes, message):
    if messageType == MessageTypes.CloseConnection:
        print("Close connection")
        return False
    elif messageType == MessageTypes.WideFieldImage:
        # ToDo: save Id
        # ToDo: send to motionDetection
        responseObject = WideFieldPositionMessage(message.PacketId, message)
        print(f"[WideField] Bytes sent: {client.send(responseObject.Serialize())}")
        print(f"[WideField] Sent coordinates {responseObject.PositionX}:{responseObject.PositionY}")
    elif messageType == MessageTypes.TightFieldImage:
        # ToDo: Save Id
        # ToDo: Send to neural
        print("[TightField] Send coordinates")

    return True
