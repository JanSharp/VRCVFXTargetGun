
<!-- cSpell:ignore ulongs -->

- [x] only apply changes to the texture if the player is near the board
- [x] late joiner syncing
  - [x] handle unexpected ownership transfer
  - [x] update chalk action structure to match chalkboard
  - [x] use 3 or 4 ulongs and sync less frequently. it's significantly more efficient in terms of total synced byte count
- [x] add progress bars just for fun
- [x] improve queue logic for chalks
- [x] scale previews based on board size and resolution
- [x] adjust update rates depending on resolution
- [x] use transparency for background
- [x] test planes
- [x] ~~looking at the boards from the side makes the lines disappear, this is most likely related to both them now being transparent and on planes in font of the board.~~ While this might be true when compared to the non transparent version before, there's nothing we can do about this, at least as far as I know. Plus I've tested it again and you really have to be looking at it from a very flat angle for the lines to get unreasonably thin. I'm calling this a non-issue.
- [x] ~~maybe test interpolation for less jagged drawing in VR. May depend on the user, their setup and general usage such as distance from the board, speed of writing.~~ Tested and the result is that it's a ton of complexity which is also incredibly difficult to get right to make it feel good. It's still not a bad idea but implementing it is very hard.
- [x] set grip depending on if the user is in VR
- [x] reduce allowed distance from the board in VR
- [x] move board up and down
- [x] you being the only single person in the world is going to break late joiner syncing for the first other person joining
  - [x] think about and test more edge cases involving being the only one in the instance during different stages of syncing
- [x] drawing a whole bunch with one chalk takes too long to catch up with incremental syncing causing you using another chalk too quickly being potentially overdrawn by the old chalk for all other clients, so in other words it's a de-sync. (fixed by registering fewer points per second)
- [x] ~~the indicator still disappears behind the plane that's drawn upon.~~ It might still happen sometimes but 99% of the time it seems to be working correctly at this point
- [x] remove debug log messages from production builds. So basically use preprocessor ifs
- [ ] maybe do something about the "can't go off the board" clamping in combination with the indicator. either move the indicator or add logic - mostly important for the sponge
- [x] the optional progressbar for syncing initializes with a value of 1 for the first 10 seconds (the time before it starts syncing). It should be 0
- [x] the 10 second late joiner sync delay can fail if multiple people join within 10 seconds


# Debugging

To get debug log messages (with the `<dlt>` prefix) add `ChalkboardDebug` to `Edit => Project Settings => Player => Other Settings => Scripting Define Symbols` (which is a `;` separated list for the record) and recompile scripts


# Notes about drawing curves

welcome to today's podcast where we figure out how to draw curves.

uh, enjoy...

so we have this DrawFromPrevTo function which is really just wrong. The name is wrong i mean. Let's fix that
There, fixed. Now, what next.

We have to change it to keep track of 3 points... no 2 points. 2 old points and the third one is going to be the newly drawn point. Additionally we need to keep track of a direction vector for the first point which is the oldest one. That point is the last drawn point on the board, like it actually has pixels on the board. And since we are currently holding the input button down we are drawing a line so that last point came from some direction. That direction is the one we need to keep track of. Then the second point is the second last point that was inputted. That point is going to indicate the next direction we have to move to, like we have to get close to this point. But we won't actually reach that point, we're going to cut short and start moving to the last point - the very new point - that just been inputted. We are also not going to reach that point, we're going to stop before we reach it, remember the last drawn point, save the direction vector and update the second point to the new point.

That will continue for as long as you're holding the button and drawing, but once you stop moving it needs to recognize that and finish drawing from the last drawn point to the last input point.

So in terms of states we have a couple:
- waiting for input: no prev state, nothing to draw, no input happened yet
- drawn point without direction and no previous input points: the last input point matches the last drawn point on the board exactly.
- drawn point without direction and one previous input point: the second last input point matches the last drawn point on the board, we have one new input point and we have no direction we last moved in
- drawn point with direction and one previous input point

if we are in one of the 2 last states we have to set a timeout for waiting for input. once that timeout is reached they will transition back to the second state

drawing only happens in the last 2 states... well I guess going from the first state to the second state does draw one single point. Outside of that all drawing is done in the last 2 states. The function doing so is the exact same, except that for the first state it has to figure out a start direction. Well instead of figuring out the start direction when we're drawing we could instead figure out the start direction when transitioning from the second state to the third state, that way the last two states become identical, so there's only one. the start direction would be exactly pointing from the last drawn point to the last input point.

there are more logistical problems: in what unit do we save points? currently they are saved as integers however we very most likely should use floats. Then we have have a fixed magnitude of the difference between 2 drawn points and only right before drawing on the board the float positions get converted to pixel coordinates. Nope, we can't do that. We have to be able to sync points and in order to keep those values as small as possible - in terms of bit count - using integers is much better. Using integers we can represent one point using 20 bits, 10 for the x axis, 10 for the y axis. Using floats is basically impossible... well no it's not but way too unreasonable. I'd have to implement a floating point number that's smaller than 4 bytes in udon. And even then I wouldn't be able to get it down to 10 bits, more like 16 or something. Using a fixed point number is more reasonable but still I'd have to use more bits, so it's not an option. I have to use integers. The only floating point numbers I can use is the direction vector, and I can use floats for the duration of drawing the line, once it's done drawing one line it's just going to store the integer point and the float direction

Ok I think I'm ready to do this. I still don't actually know how to draw a curve but I know how to setup the infrastructure around it.
I'm not going to think about late joiner syncing right now just yet, because I'll have to duplicate some logic for the chalk and for the chalkboard. Thinking about both at the same time requires a bit too much brain power
