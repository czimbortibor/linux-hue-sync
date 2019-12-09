# HueSync (working title)

Anything might change. In a gist: I need an app on my linux system to be able to use the [Philips Hue Entertainment](https://www2.meethue.com/en-us/entertainment). Official is non-existent, other solutions to my needs havent been found yet.
All in all: project for fun with an overcomplicated stack.

## Plan of attack
- [x] Http client in .netcore to communicate with the Hue Bridge:
    - [x] make it async (adjust something in the python usage)
- [x] get a screenshot (main display for now):
    - [x] python implementation to grab screenshots as that seems easier and libraries are present
    - [ ] some fast, low-level, closest-to-graphics app to grab screenshots: partly done with Xlib,         but results not satisfactory
- [x] implement color syncing in the most basic way: grab a screenshot from the main display and determine the dominant color:
    - [x] calculate the dominant color on an image
    - [x] convert result to the correct color space needed for the bulbs
        - [ ] double check
    - [x] figure out calling python code from .netcore without external `Process` calls
- [ ] implement color syncing in a more advanced way: map the display with the entertainment area/light bulbs


## Getting Started
"works on my machine" for now.

The repo includes a custom built [pythonnet](https://github.com/pythonnet/pythonnet) dll, could not manage to get it working in the official way... `src/localpackages/Python.Runtime.dll`

### Prerequisites
- .net core 3.1
- python libs:
    - tkinter
    - PIL
    - numpy
    - scipy
    - [pyscreenshot](https://pypi.org/project/pyscreenshot/)
    - [colormath](https://python-colormath.readthedocs.io/en/latest/)



## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details
