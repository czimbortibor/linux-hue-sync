#include <iostream>
#include <X11/Xlib.h>
#include <X11/Xutil.h>


extern "C" void getScreenShot(unsigned char* dataBuffer, unsigned int width, unsigned int height)
{
  Display* display = XOpenDisplay(nullptr);
  Window rootWindow = DefaultRootWindow(display);

  unsigned int x = 0;
  unsigned int y = 0;

  XImage* image = XGetImage(display, rootWindow, x, y, width, height, AllPlanes, ZPixmap);

  unsigned long red_mask   = image->red_mask;
  unsigned long green_mask = image->green_mask;
  unsigned long blue_mask  = image->blue_mask;
  unsigned int index = 0;
  for (unsigned int i = 0; i < height; i++) {
    for (unsigned int j = 0; j < width; j++) {
      unsigned long pixel = XGetPixel(image, j, i);
      unsigned char blue  = (pixel & blue_mask);
      unsigned char green = (pixel & green_mask) >> 8;
      unsigned char red   = (pixel & red_mask) >> 16;

      dataBuffer[index + 2] = blue;
      dataBuffer[index + 1] = green;
      dataBuffer[index + 0] = red;

      index += 3;
    }
  }

  XDestroyImage(image);
}