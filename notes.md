
TODO: custom object pool
TODO: trash can

to test:
- item spawning
- item de-spawning

TODO: swimming script
TODO: script to change references to all kinds of things, most notably udon behaviours
TODO: door teleport setup editor tool

TODO: item sync: test unloaded players pickup up items
TODO: tool to mass replace object syncs with item sync



# Regex for adding, updating and removing debug messages in each method

dlt = debug log tracker, because I needed something simple to remember.

```

"parameters" (replace manually): MethodNameToBeExcluded, ClassName

(( *)(private|public)\s+(override\s+)?\w+\s+(?!MethodNameToBeExcluded)(\w+)\([^\)]*\)(\s|\n)*\{)(\n\s*Debug\.Log\(\$"<dlt>.*)?

adding and updating:

$1
$2    Debug.Log($"<dlt> ClassName {this.name}  $5");

removing:

$1

```

# Syncing

object associated - intended use
player associated - not at all (tags that I didn't check out yet)

state syncing - intended use
event syncing - intended use
parameterized event syncing - not intended use, but workable
incremental syncing - syncing hell

# Joystick KeyCodes

index controllers:

left a
JoystickButton3
Joystick1Button3

right a
JoystickButton1
Joystick2Button1

left b
JoystickButton2
Joystick1Button2

right b
JoystickButton0
Joystick2Button0

left squeeze
JoystickButton4
Joystick1Button4

right squeeze
JoystickButton5
Joystick2Button5

left trigger
JoystickButton14
Joystick1Button14

right trigger
JoystickButton15
Joystick2Button15

left joystick down
JoystickButton8
Joystick1Button8

right joystick down
JoystickButton9
Joystick2Button9
