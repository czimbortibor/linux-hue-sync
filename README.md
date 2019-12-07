# HueSync (working title)

Anything might change. In a gist: I need an app on my linux system to be able to use the [Philips Hue Entertainment](https://www2.meethue.com/en-us/entertainment). Official is non-existent, other solutions to my needs havent been found yet.
All in all: project for fun with an overcomplicated stack.

## Plan of attack
- [x] Http client in .netcore to communicate with the Hue Bridge:
- [ ] implement color syncing in the most basic way: grab a screenshot from the main display and determine the dominant color:
    - [ ] some fast, low-level, closest-to-graphics app to grab screenshots: partly done with Xlib, but results not satisfactory
    - [x] python implementation to grab screenshots as that seems easier and libraries are present
    - [x] calculate the dominant color on an image
    - [ ] convert result to the correct color space needed for the bulbs
    - [x] figure out calling python code from .netcore without external `Process` calls


## Getting Started
"works on my machine" for now.

The repo includes a custom built [pythonnet](https://github.com/pythonnet/pythonnet) dll, could not manage to get it working in the official way... `src/HueCli/localpackages/Python.Runtime.dll`

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
