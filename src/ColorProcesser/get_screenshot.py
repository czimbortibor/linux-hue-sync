import os
import ctypes
import tkinter
from PIL import Image
import numpy as np
import scipy
import scipy.misc
import scipy.cluster
import binascii
import pyscreenshot as ImageGrab


def get_screen_size() -> (int, int):
    root = tkinter.Tk()
    width = root.winfo_screenwidth()
    height = root.winfo_screenheight()

    return (width, height)

def get_c_lib() -> ctypes.CDLL:
    lib_name = 'get_screenshot.so'
    absolute_lib_path = os.path.dirname(os.path.abspath(__file__)) + os.path.sep + lib_name

    screenshot_lib = ctypes.CDLL(absolute_lib_path)
    return screenshot_lib

def get_screen_shot_c_lib() -> Image:
    screenshot_lib = get_c_lib()

    width, height = get_screen_size()
    buffer_length = width * height * 3

    func = screenshot_lib.getScreenShot
    data_buffer = (ctypes.c_ubyte * buffer_length)()
    func.argtypes = [ctypes.c_ubyte * buffer_length, ctypes.c_uint, ctypes.c_uint]

    func(data_buffer, width, height)

    screenshot = Image.frombuffer('RGB', (width, height), data_buffer, 'raw', 'RGB', 0, 1)
    del data_buffer

    return screenshot

def get_screenshot_imgagegrab() -> Image:
    screenshot = ImageGrab.grab()

    return screenshot

def get_dominant_color(image: Image) -> str:
    resized_image = image.resize((150, 150))
    pixel_array = np.asarray(resized_image)
    pixel_array_shape = pixel_array.shape
    reshaped_pixel_array = pixel_array.reshape(scipy.product(pixel_array_shape[:2]), pixel_array_shape[2]).astype(float)

    centroids, _ = scipy.cluster.vq.kmeans(reshaped_pixel_array, 5, iter=20, thresh=1e-5)
    
    codes, _ = scipy.cluster.vq.vq(reshaped_pixel_array, centroids)
    color_counts, _ = scipy.histogram(codes, len(centroids)) 

    highest_index = scipy.argmax(color_counts)
    peak = centroids[highest_index]
    hex_color = "#" + binascii.hexlify(bytearray(int(c) for c in peak)).decode('ascii')

    return hex_color

if __name__ == "__main__":
    # screenshot = get_screen_shot_c_lib()
    screenshot = get_screenshot_imgagegrab()
    # screenshot.save("out" + str(i) + ".jpg")
    hex_color = get_dominant_color(screenshot)