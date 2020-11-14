from enum import IntEnum


class EventType(IntEnum):
    WideFieldImageGotten = 0  # {'message':message}
    TightFieldImageGotten = 1

    WideFieldPositionSend = 2
    TightFieldPositionSend = 3
