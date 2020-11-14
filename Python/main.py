import Socket.Utils.Params as Params

from Socket.DeviceServer import SocketListener

if __name__ == '__main__':
    print("Start working")
    motionDetection = SocketListener(Params.WIDE_FIELD_ADDRESS, Params.WIDE_FIELD_PORT, 1)
    print("Continue main...")

    while True:
        inputCommand = input()
        if inputCommand == "exit_quit":
            motionDetection.Dispose()
            break

    motionDetection.AcceptThread.join()
