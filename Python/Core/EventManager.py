from Core.Utils.EventType import EventType

# # # Example # #
# EventManager.Initialize()
# EventManager.AddHandler(EventType.WideFieldImageGet, TestFunction)
# EventManager.RaiseEvent(EventType.WideFieldImageGet, kwargs={'messageType': 0, 'message': 1, 'client': 2})
# EventManager.RemoveHandler(EventType.WideFieldImageGet, TestFunction)


class EventManagerEntity:
    HandlerMap: dict = {}

    def __init__(self):
        EventManager.Instance = self

    def AddHandler(self, eventType: EventType, action):
        if eventType in self.HandlerMap:
            self.HandlerMap.get(eventType).append(action)
        else:
            self.HandlerMap[eventType] = [action]

    def RemoveHandler(self, eventType: EventType, action):
        if eventType not in self.HandlerMap:
            return

        actionList = self.HandlerMap.get(eventType)
        if action not in actionList:
            return

        actionList.remove(action)
        if len(actionList) == 0:
            self.HandlerMap.pop(eventType)

    def RaiseEvent(self, eventType: EventType, kwargs):
        if eventType not in self.HandlerMap:
            return

        for action in self.HandlerMap.get(eventType):
            action(kwargs=kwargs)


class EventManager:
    Instance: EventManagerEntity

    @staticmethod
    def Initialize():
        EventManagerEntity()

    @staticmethod
    def AddHandler(eventType: EventType, action):
        EventManager.Instance.AddHandler(eventType=eventType, action=action)

    @staticmethod
    def RemoveHandler(eventType: EventType, action):
        EventManager.Instance.RemoveHandler(eventType=eventType, action=action)

    @staticmethod
    def RaiseEvent(eventType: EventType, kwargs):
        EventManager.Instance.RaiseEvent(eventType=eventType, kwargs=kwargs)

