import cv2
import time
import random
import Neuron.Utils.Params as Params

from threading import Thread, RLock
from Core.EventManager import EventManager
from Core.Utils.EventType import EventType
from Neuron.Utils.JpgDecoder import JpgDecoder
from keras.preprocessing.image import img_to_array
from Socket.Messages.WideFieldPositionMessage import WideFieldPositionMessage

import Neuron.TankCapture as TankCapture


class MotionDetection(Thread):
    __isDisposed: bool = False
    BufferDictionary: dict = {}
    WorkDictionary: dict = {}
    Locker: RLock = RLock()

    def __init__(self):
        Thread.__init__(self)
        self.daemon = True

        EventManager.AddHandler(eventType=EventType.WideFieldImageGotten, action=self.OnMessageReceive)
        self.start()

    def run(self):
        while not self.__isDisposed:
            if len(self.BufferDictionary) == 0:
                time.sleep(Params.FPS)
                continue

            self.Locker.acquire()
            try:
                self.WorkDictionary.update(self.BufferDictionary)
                self.BufferDictionary.clear()
            finally:
                self.Locker.release()

            keys = list(self.WorkDictionary.keys())
            for packetId in keys:
                item = self.WorkDictionary.pop(packetId)
                # Detect
                prob, center = TankCapture.Execute(item['image'], packetId)

                if self.__isDisposed:
                    break
                responseObject = WideFieldPositionMessage(
                    packetId,
                    center[0],
                    center[1],
                    random.randint(100, 120),
                    random.randint(100, 120))

                item['client'].send(responseObject.Serialize())
                # print(f"[WideField] Sent object [{packetId}] | {responseObject.PositionX}:{responseObject.PositionY} | [{responseObject.SizeX}:{responseObject.SizeY}]")

        self.Locker.acquire()
        try:
            self.BufferDictionary.clear()
            self.WorkDictionary.clear()
        finally:
            self.Locker.release()

        print("Motion detection closed!")

    def OnMessageReceive(self, kwargs):
        packetId = kwargs['message'].PacketId
        image = JpgDecoder.Decode(kwargs['message'])
        # byteArray = img_to_array(JpgDecoder.Decode(kwargs['message']))

        self.Locker.acquire()
        try:
            print(f"Received packetId: {packetId}")
            self.BufferDictionary.update({packetId: {'client': kwargs['client'], 'image': image}})
            # self.BufferDictionary.update({packetId: {'client': kwargs['client'], 'data': byteArray}})
        finally:
            self.Locker.release()

    def Dispose(self):
        EventManager.RemoveHandler(eventType=EventType.WideFieldImageGotten, action=self.OnMessageReceive)
        self.__isDisposed = True
