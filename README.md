# Monologue

Monologue is a Unity3D script which aims to emulate text boxes with gradually revealed text and a beeping sound effect meant to resemble speech as seen in many older JRPGs and visual novels (e.g. Phoenix Wright Ace Attorney).

## Screencapture

![Screencapture](https://github.com/ekx/Monologue/blob/master/Screenshots/MonologueGif.gif)

## License

* [MIT](https://github.com/ekx/Monologue/blob/master/LICENSE)

## Instructions

* Add the "Monologue" script to a game object containing a "Text" and "AudioSource" component.  
* Configure the settings to your liking and call the "AnimateText" function to get started.  
* The "AdvanceText" function skips the animation and immediately displays the whole text.  
* Once the animation has finished (or "AdvanceText" was called) "OnTextOutputFinished" triggers.  

For a basic setup see the included "Example.unity" scene.

## Contributing

Please see the [issues section](https://github.com/ekx/Monologue/issues) to
report any bugs or feature requests and to see the list of known issues.

[Pull requests](https://github.com/ekx/Monologue/pulls) are always welcome.
