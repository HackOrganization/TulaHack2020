import Socket.Utils.Params as Params

from Socket.DeviceServer import SocketListener
from Core.EventManager import EventManager
from Neuron.MotionDetection import MotionDetection


def Execute():
    EventManager.Initialize()

    motionDetectionListener = SocketListener(Params.WIDE_FIELD_ADDRESS, Params.WIDE_FIELD_PORT, 1)
    motionDetectionWorker = MotionDetection()

    while True:
        inputCommand = input()
        if inputCommand == "exit":
            motionDetectionListener.Dispose()
            motionDetectionWorker.Dispose()
            break

    if motionDetectionListener.AcceptThread.is_alive():
        motionDetectionListener.AcceptThread.join()
    if motionDetectionWorker.is_alive():
        motionDetectionWorker.join()


if __name__ == '__main__':
    Execute()
