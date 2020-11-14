import time
import random
import Neuron.Utils.Params as Params

from threading import Thread, RLock
from Core.EventManager import EventManager
from Core.Utils.EventType import EventType
from Neuron.Utils.JpgDecoder import JpgDecoder
from keras.preprocessing.image import img_to_array
from Socket.Messages.WideFieldPositionMessage import WideFieldPositionMessage


# ToDo: write dispose and call
# EventManager.RemoveHandler(eventType=EventType.WideFieldImageGotten, action=self.OnMessageReceive)
class MotionDetection(Thread):
    IsDisposed: bool = False
    BufferDictionary: dict = {}
    WorkDictionary: dict = {}
    Locker: RLock = RLock()

    def __init__(self):
        Thread.__init__(self)
        self.daemon = True

        EventManager.AddHandler(eventType=EventType.WideFieldImageGotten, action=self.OnMessageReceive)
        self.start()

    def run(self):
        while not self.IsDisposed:
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
            for key in keys:
                item = self.WorkDictionary.pop(key)
                # ToDo: detect
                responseObject = WideFieldPositionMessage(key, random.randint(0, 640), random.randint(0, 480))

                print(f"[WideField] Bytes sent: {item['client'].send(responseObject.Serialize())}")
                print(f"[WideField] Sent coordinates {responseObject.PositionX}:{responseObject.PositionY}")

        print("Motion detection closed!")

    def OnMessageReceive(self, kwargs):
        packetId = kwargs['message'].PacketId
        byteArray = img_to_array(JpgDecoder.Decode(kwargs['message']))

        self.Locker.acquire()
        try:
            self.BufferDictionary.update({packetId: {'client': kwargs['client'], 'data': byteArray}})
        finally:
            self.Locker.release()
