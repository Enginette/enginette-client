# enginette-client
 A client for the [Enginette](https://github.com/DDev247/enginette) project
 
# How does this work?
 This program works by adding some registry keys (it deletes them when you uninstall) for Windows to recognize the protocol `enginette-client://` as a shortcut for launching the program.
 
# Install instructions
 - Step 1: 
  Download setup from [Releases](https://github.com/DDev247/enginette-client/releases/latest)
 - Step 2:
  Run the setup.
 - Step 3:
  Enjoy. To test it go to ~~Enginette~~ (coming soon!) and launch an engine.

# FAQ
### Q: The program doesn't start the simulator
 + A: Either you didn't enter the simulator exe file or file is not found on computer.
### Q: I think I entered the correct exe file but it still doesn't start
 + A: This means the file cannot be found.

### Q: How to enter a new simulator exe file?
 + A: Go to `%localappdata%\enginette-client` and delete `settings.json`. Now you can launch the simulator as normal and it will prompt you to insert the exe file location.

### Q: Program doesn't do anything when opening
 + A: The program is **not** meant to be launched normally.  
  
