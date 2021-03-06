# Возвращает булево значение из массива байтов по указанному смещению
def GetBool(bytearrayValue: bytearray, offset=0):
    return bool(GetInteger(bytearrayValue, offset, 1, False))


# Возвращает число указанной из массива байтов начиная с "offset" (параметрами "length" и "signed" регулируется тип
# данных)
def GetInteger(bytearrayValue: bytearray, offset=0, length=0, signed=True):
    if length == 0:
        length = 4
    integerBytearray = bytearray()
    for i in range(offset, offset + length):
        integerBytearray.append(bytearrayValue[i])
    return int.from_bytes(integerBytearray, byteorder='little', signed=signed)


# Возвращает массив байтов начиная с "offset" на указанную длину "length" (если "length" не укзаана, то возвращаются
# до конца)
def GetBytes(bytearrayValue: bytearray, offset=0, length=0):
    if length == 0:
        length = len(bytearrayValue) - offset
    returnArray = bytearray()
    for i in range(offset, offset + length):
        returnArray.append(bytearrayValue[i])
    return returnArray


# Возвращает массив байтов, эквивалентных Булевому значению
def GetBytesFromBool(value: bool):
    return value.to_bytes(1, byteorder='little')


# Возвращает массив байтов, эквивалентных целочисленному значению
def GetBytesFromInteger(value: int):
    return value.to_bytes(4, byteorder='little')


# Возвращает массив байтов, эквивалетных значению типа Ushort
def GetBytesFromUshort(value: int):
    return value.to_bytes(2, byteorder='little', signed=False)


# Возвращает массив байтов, эквивалетных значению типа Short
def GetBytesFromShort(value: int):
    return value.to_bytes(2, byteorder='little', signed=True)


# Возвращает массив байтов, эквивалентых значению типа byte
def GetBytesFromByteInteger(value: int):
    # ToDo: "signed" flag can be error
    return value.to_bytes(1, byteorder='little', signed=False)
