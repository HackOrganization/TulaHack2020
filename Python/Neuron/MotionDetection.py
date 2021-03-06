import time
from threading import Thread, RLock

import Neuron.TankCapture as TankCapture
import Neuron.Utils.Params as Params
from Core.EventManager import EventManager
from Core.Utils.EventType import EventType
from Neuron.Utils.JpgDecoder import JpgDecoder
from Socket.Messages.WideFieldPositionMessage import WideFieldPositionMessage


class MotionDetection(Thread):
    __isDisposed: bool = False
    Buffer: list = []
    Locker: RLock = RLock()

    def __init__(self):
        Thread.__init__(self)
        self.daemon = True

        EventManager.AddHandler(eventType=EventType.WideFieldImageGotten, action=self.OnMessageReceive)
        self.start()

    def run(self):
        while not self.__isDisposed:
            workData = {}
            fixTime = time.time()

            self.Locker.acquire()
            try:
                length = len(self.Buffer)
                if length == 0:
                    time.sleep(Params.FPS)
                    continue

                workData = self.Buffer.pop(length-1)
                self.Buffer.clear()
            finally:
                self.Locker.release()

            prob, center, size = TankCapture.Execute(workData['image'])

            if self.__isDisposed:
                break

            responseObject = WideFieldPositionMessage(prob, center, size)
            if not workData['client'].fileno() == -1:
                workData['client'].send(responseObject.Serialize())

        self.Locker.acquire()
        try:
            self.Buffer.clear()
        finally:
            self.Locker.release()

        print("Motion detection closed!")

    def OnMessageReceive(self, kwargs):
        image = JpgDecoder.Decode(kwargs['message'].JpgImageData)

        self.Locker.acquire()
        try:
            self.Buffer.append({'client': kwargs['client'], 'image': image})
        finally:
            self.Locker.release()

    def Dispose(self):
        EventManager.RemoveHandler(eventType=EventType.WideFieldImageGotten, action=self.OnMessageReceive)
        self.__isDisposed = True
