import os
import random

import numpy as np
import tensorflow as tf
from tensorflow.keras.layers import Input
from tensorflow.keras.models import Model
from tensorflow.keras.models import load_model
from tensorflow.keras.preprocessing.image import img_to_array

os.environ['TF_FORCE_GPU_ALLOW_GROWTH'] = 'true'

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
    img = frame / 255
    img = np.expand_dims(img, axis=0)
    tank_probability, heatmap = make_gradcam_heatmap(img)

    return tank_probability, heatmap


def Execute(frame):
    frame = img_to_array(frame)

    probability, heatMap = detect_tank(frame)

    probability = round(probability * 100)
    centerIndex = FindCenter(probability, heatMap)

    return probability, centerIndex, FindSize(centerIndex)


def FindCenter(probability, heatMap):  # heatMap: 20x15
    if probability < 16:
        return -1, -1

    centerIndex = np.unravel_index(np.argmax(heatMap), heatMap.shape)  # x, y
    return int(img_height / len(heatMap) * centerIndex[1]), int(
        img_width / len(heatMap[centerIndex[0]]) * centerIndex[0])


def FindSize(centerIndex):
    if centerIndex == (-1, -1):
        return 0, 0

    # ToDo: find real size
    return random.randint(100, 120), random.randint(100, 120)
