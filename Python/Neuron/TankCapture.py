import cv2
import os

import cv2
import numpy as np
from tensorflow.keras.models import load_model
from tensorflow.keras.preprocessing.image import img_to_array
from tensorflow.keras import backend as K

os.environ['TF_FORCE_GPU_ALLOW_GROWTH'] = 'true'
os.environ['TF_ENABLE_GPU_GARBAGE_COLLECTION'] = 'false'


def custom_loss(y_true, y_pred):
    presence, location = y_true[:, 0], y_true[:, 1:5]
    prob, pred_location = y_pred[:, 0], y_pred[:, 1:5]

    bce_loss = K.binary_crossentropy(presence, prob)

    mae_loss = presence * K.mean(K.abs(location - pred_location), axis=-1)

    alpha = 0.1
    loss = alpha * bce_loss + (1 - alpha) * mae_loss

    return loss


img_width, img_height = 640, 480
print("Loading models....")
base_model = load_model("1444unet_local_alpha.h5", custom_objects={'custom_loss': custom_loss})

print("Loading models completed!")


def Execute(frame):
    frame = img_to_array(frame)

    img = frame / 255
    img = cv2.cvtColor(img, cv2.COLOR_RGB2GRAY)
    img = np.expand_dims(img, axis=0)
    pred = base_model.predict(img)
    pred, x, y, w, h = pred[0]

    return int(pred * 100), (int(x), int(y)), (int(w), int(h))


# def FindCenter(probability, heatMap):  # heatMap: 20x15
#     if probability < 16:
#         return -1, -1
#
#     centerIndex = np.unravel_index(np.argmax(heatMap), heatMap.shape)  # x, y
#     return int(img_height / len(heatMap) * centerIndex[1]), int(
#         img_width / len(heatMap[centerIndex[0]]) * centerIndex[0])
#
#
# def FindSize(centerIndex):
#     if centerIndex == (-1, -1):
#         return 0, 0
#
#     # ToDo: find real size
#     return random.randint(100, 120), random.randint(100, 120)
