import Socket.Utils.Params as Params

from Socket.DeviceServer import SocketListener

from Core.EventManager import EventManager


def Execute():
    EventManager.Initialize()

    motionDetectionListener = SocketListener(Params.WIDE_FIELD_ADDRESS, Params.WIDE_FIELD_PORT, 1)

    while True:
        inputCommand = input()
        if inputCommand == "exit_quit":
            motionDetectionListener.Dispose()
            break

    motionDetectionListener.AcceptThread.join()


def TestFunction(kwargs):
    print("Without context:")
    for key in kwargs:
        print(f"{key} : {kwargs[key]}")


if __name__ == '__main__':
    Execute()
