from imutils.video import VideoStream
import numpy as np
import imutils
import time
import cv2
import os
import time  # !!!!!!!!!
import os
import numpy as np
from tensorflow.keras.models import Model
from tensorflow.keras.layers import GlobalAveragePooling2D, Input, Dense
from tensorflow.keras.preprocessing.image import array_to_img, img_to_array
from tensorflow.keras import regularizers
import IPython
import matplotlib.pyplot as plt

import matplotlib.cm as cm
import tensorflow as tf
from tensorflow.keras.models import load_model
from tensorflow.keras.applications.xception import Xception
from tensorflow.keras.models import Model
from tensorflow.keras.applications.xception import preprocess_input
from tensorflow.keras.applications.xception import decode_predictions
from tensorflow.keras.preprocessing.image import img_to_array
from tensorflow.keras.models import load_model

img_width, img_height = 640, 480
print("Loading models....")
head_model = load_model("head_model.h5")
base_model = load_model("base_model.h5")

inputs = Input(shape=(img_height, img_width, 3))
model = Model(inputs, head_model(base_model(inputs)))
print("Loading models completed!")


def make_gradcam_heatmap(img_array):
    last_conv_layer = base_model.layers[-1]

    # Then, we compute the gradient of the top predicted class for our input image
    # with respect to the activations of the last conv layer
    with tf.GradientTape() as tape:
        # Compute activations of the last conv layer and make the tape watch it
        # print(img_array.shape)
        last_conv_layer_output = base_model(img_array)
        tape.watch(last_conv_layer_output)
        # Compute class predictions
        preds = head_model(last_conv_layer_output)
        top_pred_index = tf.argmax(preds[0])
        top_class_channel = preds[:, top_pred_index]
        prob = float(top_class_channel[0])

    # This is the gradient of the top predicted class with regard to
    # the output feature map of the last conv layer
    grads = tape.gradient(top_class_channel, last_conv_layer_output)

    # This is a vector where each entry is the mean intensity of the gradient
    # over a specific feature map channel
    pooled_grads = tf.reduce_mean(grads, axis=(0, 1, 2))

    # We multiply each channel in the feature map array
    # by "how important this channel is" with regard to the top predicted class
    last_conv_layer_output = last_conv_layer_output.numpy()[0]
    pooled_grads = pooled_grads.numpy()
    for i in range(pooled_grads.shape[-1]):
        last_conv_layer_output[:, :, i] *= pooled_grads[i]

    # The channel-wise mean of the resulting feature map
    # is our heatmap of class activation
    heatmap = np.mean(last_conv_layer_output, axis=-1)

    # For visualization purpose, we will also normalize the heatmap between 0 & 1

    heatmap = np.maximum(heatmap, 0)
    if np.max(heatmap) != 0:
        heatmap /= np.max(heatmap)

    return prob, heatmap


def detect_tank(frame):
    # img = cv2.cvtColor(frame, cv2.COLOR_BGR2RGB)

    img = frame / 255
    img = np.expand_dims(img, axis=0)
    # img = preprocess_input(img)
    tank_probability, heatmap = make_gradcam_heatmap(img)

    return tank_probability, heatmap  # !!!!!


def Execute(frame, packetId):
    frame = img_to_array(frame)

    probability, heatMap = detect_tank(frame)
    # heatMap: 20x15

    probability = round(probability * 100, 1)
    centerIndex = FindCenter(probability, heatMap)

    print(F"Packet: {packetId}. Probability: {probability}. Center: {centerIndex}")
    # cv2.imshow("Heatmap", np.kron(heatMap, np.ones((30, 30))))

    return probability, centerIndex


def FindCenter(probability, heatMap):
    if probability < 16:
        return -1, -1

    centerIndex = np.unravel_index(np.argmax(heatMap), heatMap.shape) # x, y
    return int(img_height / len(heatMap) * centerIndex[1]), int(img_width / len(heatMap[centerIndex[0]]) * centerIndex[0])
