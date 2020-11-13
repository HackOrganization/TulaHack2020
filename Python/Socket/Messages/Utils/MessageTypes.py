from enum import IntEnum


class MessageTypes(IntEnum):
    Null = 0

    CloseConnection = 1 #Закрытие соединения

    WideFieldImage = 2 #Картинка с широкопольной камеры
    TightFieldImage = 3 #Картинка с узкопольной камеры

    WideFieldPosition = 4 #Координаты на широкопольной картинке
    TightFieldPosition = 5 #Координаты на узкополной картинке
