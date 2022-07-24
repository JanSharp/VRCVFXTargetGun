
# VFX Target Gun

## Done

- when held show target locally
- when clicked, play effect at target location
  - if it is looped, star it, unless it was already looping, in which case disable (return to default state)
  - if it isn't looped, just play it. If all particle systems are already playing, create a new one, therefore allowing for multiple of the same effect to be active
- define effects using effect descriptors which all are a child of a specific GameObject
- interact on top of the gun to show a UI
  it's basically a grid of buttons that have been instantiated the first time you picked it up, where the currently active is indicated somehow. Currently looping ones are also marked somehow. Upon click of one of those buttons, change active effect to the clicked one and close the UI immediately
- indicate active state of both looped and non looped effects outside of the UI
- allow disabling of currently active looped effect without having to point at the ground
- disable target indicator if the current loop effect is active
- add a toggle in the UI to keep it open to allow for quick testing of the different effects, especially in VR where you have 2 hands. Yea I like this idea
- show name of selected effect outside of UI
- color legend for what they mean
- close button for the UI both to not confuse people and to make it easy to disable keep open and then close the UI, since those are right next to each other
- add stop button for currently active looped effects in the UI
- underline instead of bold
- deselect button
- reduce proximity for the UI toggle
- don't collide with the player (including the UI)
- make it smaller, like 2x smaller, and maybe angle it a bit so it's not exact 90 degrees. And maybe make it prettier
- adjust default layer mask to not collide with everything
- double tap prevention
- local visibility and grab-able toggle, global for all guns
- sync non looping effects when they are played
- sync looping effects when they are played
- is the first time a non loop effect gets used broken? In that it ends up getting teleported for the second one instead of making a new one?
  VRCInstantiate is just broken. There is no other explanation. All my references are correct, it works the second time you use it so that confirms it, but when modifying the position and rotation of the newly instantiated object right after it was created it behaves as if it was still referring to the old object. There is nothing I can do except blame VRChat and then hopefully find a workaround at some point
- sync controller state to allow giving another DM your gun
  - the synced state must not care about the UI, because the UI may not even exist yet. Well, the buttons.
    - when creating the buttons, adjust them to match the current state
  - sync selected effect
- sync looping effects for late joiners
- flip selected effect text to the other side when held in the left hand
- support non particle system effects. They really aren't effects anymore at that point, just objects that you can place in the world
- option for random rotation or fixed effect rotation (forward being aligned with the hit normal, up facing away from the gun as well as possible)
- disable the Hide UI toggle to prevent any of the proximity issues in VR
- disable ui box collider when it is hidden to remove the cursor while it's hidden
- preview selected effect at current target location

## Future Todo

- modes:
  - place mode
  - delete mode
  - preview mode
  - edit mode
- scaling
- stop absolutely everything button
- some sort of rotation control like locking rotation, locking rotation on some around some axis, snapping
- grid snapping
- positioning, most notably up and down to be able to place things on uneven ground without parts of it floating

## Version 2

The main feature is going to be multiple toggle effects (so loop and obj) which ultimately requires place and delete mode.

Preview mode is optional as described at the end.
Edit mode won't be in, at least not as the first thing, but it is mentioned below because the UI needs to have support for it in the future.

first, UI
- Effect button effect count top right
- Effect button "stop" becomes "stop/delete all"
- Insert row of buttons at the bottom of the body
- Place/Delete(/Edit) mode buttons in said button row, indicating active state using underlines just like effects
- Nuke button at the end of said button row to delete absolutely everything
- Cancel/Confirm popup UI for nuke button

maybe the button row should be below the legend because the legend is related to the buttons above it. Then we don't have a footer, but I can just put an ending line there, or just see how it looks. Keeping things separated is important, reduces perceived clutter.

mode colors: place blue, delete red, edit yellow.
well, actually they shouldn't have colors. I just love colorful stuff. But no colors are boring... but yea it doesn't need it. Although it would be nice if the color of the entire gun changed depending on the mode you're in. that sounds dope. and useful. and just for completeness sake add a small little TMPro text below the gun with the mode name in it...
or actually some text on the side of the gun, but that's covered by the hand. Below is probably fine and honestly not super necessary.
Alright, yay, I get to use colors. I've decided. Which doesn't mean anything because I might change my mind 5 seconds later.

a ball to toggle between delete and place mode probably positioned to the side and slightly below the gun to have it accessible but prevent accidental clicks. Proximity of ~0.15.

what if the preview mode was actually a checkbox... but it only affects place mode. preview in delete... actually would kind of work, just be hard to implement and probably unnecessary. but not off the table. actually wait, not hard at all, just disable the GameObject, regardless of if it's an object or a loop. yea preview does actually work for delete as well.

so what about preview in edit mode. I think edit mode is always going to use a preview. hm, edit mode might need a confirm button though, unless it updates in real time. Yea edit mode is a weird one. the thing is while preview in edit mode does make sense, like having 2 different versions of edit mode, one with a preview and one without, you might very well want to have preview enabled in place mode but disabled in edit mode, so having to hit the checkbox every time isn't great. although, that give me an idea. every button to switch to a mode can have its own preview toggle on it. Unfortunately preview really isn't intuitive, because it makes it non obvious what is local and what is synced. There could be a help button that explains it, but that's generally a bad solution. Preview being local could be indicated using transparency, that would actually work for all 3 modes. The only hard part is that transparency is anything but easy to switch to generically. I do like the idea though. But this is good, this makes preview a good optional feature to implement because it won't actually change the layout of the UI, only add to it. Regardless of how I'd end up implementing it. Well the help button might end up changing the layout a bit, but I'm sure I'd find a spot for that button.


completely unrelated, what about a laser to assist with aiming. useful when trying to aim at some thin collider.


pretty much a must for v2:

- [x] info button for keybindings
- [x] workaround the local player not existing during the upload process to prevent that annoying error
- [x] better styling to fit the theme, kind of
- [x] outline for selected effect
- [ ] figure out exact gun grips and fix the vertical position of the gun in desktop and perfectly fix the position in VR such that the gun is always exactly in your hand and you get instant feedback when moving it. So actually this doesn't work for desktop at all unless I find a way to get the player camera position, so if I do this at all it would only be for VR.

# Version 0.3.0

preview mode

# Version 0.4.0

<!-- cSpell:ignore Factorio -->

alright, so, a list for 0.4.0, the quality of life update:

- [x] only have a single effect descriptor for each effect, not per gun
- [x] only have one single local only gun
- [x] better help window with tabs
  - [x] a general info tab
  - [x] a key binds tab
  - [x] a changelog tab
    - [ ] update changelog and changelog tab with 0.4.0
  - [x] an about tab
  - [x] maybe make the window a bit bigger now that there is more text... maybe... probably
- [x] add text mesh pro font asset and material to not use the default one which might have been modified by someone else
- [x] fix "recalling"
- [x] clean up visibility toggle
- [x] clean up gun assignment
- [x] delete any effect (with a collider) in delete mode, not just selected effects (maybe there is something smart to be done about loop effects so we don't have to iterate all active ones)
- [x] Q picking like Factorio (desktop only for now)
- [x] highlight to know what you're about to Q pick (also desktop only for now)
- [ ] support colliders for non obj effects
- [ ] support mesh renderers for non obj effects to have a "preview" (?)
- [ ] add option for effect descriptors for what type of obj they are
  - [ ] dropdown with default "auto"
  - [ ] warn about potential oversights like only some particles being looped and some aren't
- [x] change delete everything description
- [x] change mode buttons to toggles with toggle groups
- [ ] change effect buttons to toggles with toggle groups
- [x] remove button prefab, put the original button in the grid already and disable it
- [x] disable pickup collider while the gun is held in the unity play mode because god it's annoying
- [ ] add local vs global effect management
  - [ ] 2 delete everything buttons - "delete mine" and "delete everyone's"
  - [ ] 2 stop/delete all buttons per effect, again one for "mine" and one for "every"
  - [ ] 2 counts per effect
  - [ ] 2 counts for total effects
    - [ ] include these counts in the confirmation UI

maybe:

- [ ] show inlay hints for key binds when the UI is open for desktop users

I decided against toggling between just modifying stuff placed by the local player or literally everything because it would require syncing a 32 bit integer for every effect just for that which I consider to be a big waste. It's possible but for now I'll just leave it out.

# Version 0.5.0

edit mode
