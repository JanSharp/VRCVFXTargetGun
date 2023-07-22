
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

## Todo

- preview selected effect at current target location

## Maybe

- toggle the UI off when someone else picks up the gun. still possible to be toggled on again afterwards while the other person is holding it
- positioning, most notably up and down to be able to place thins on uneven ground without parts of it floating

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

after that:

<!-- cSpell:ignore Factorio -->

- refactor effects for combined pooling and performance
- Q picking like Factorio (requires effect refactor)
- preview
- edit mode



# Version 0.2.4

preview mode
unify effects into one pool instead of per gun (test network racing somehow)
toggle between just modifying stuff placed by this gun or literally everything
deleting any effect in delete mode, not just the selected one. Nice point and click (maybe there is something smart to be done about loop effects so we don't have to iterate all active ones)

# Version 0.3.0

edit mode
