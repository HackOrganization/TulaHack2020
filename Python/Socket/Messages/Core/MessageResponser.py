from Socket.Messages.Utils.MessageTypes import MessageTypes


# Выполняет действие в соотвествии с типом входящего сообщения и отправляет результат клиенту
def SendResponse(client, messageType: MessageTypes, message):
    if messageType == MessageTypes.CloseConnection:
        # ToDo: make action
        print("Close connection")
    elif messageType == MessageTypes.WideFieldImage or messageType == MessageTypes.TightFieldImage:
        # ToDo: make action
        print("Send coordinates")