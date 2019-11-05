###get_screenshot.so:
    
    `g++ -std=c++0x -shared -O3 -lX11 -fPIC -Wl,-soname,get_screenshot -o get_screenshot.so get_screenshot.cpp`


####(if opencv gets included):
    
    `g++ -std=c++0x -shared -O3 `\``pkg-config --cflags opencv4`\`` -lX11 `\``pkg-config --libs opencv4`\`` -fPIC -Wl,-soname,get_screenshot -o get_screenshot.so get_screenshot.cpp`
