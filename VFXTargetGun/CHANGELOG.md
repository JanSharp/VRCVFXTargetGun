
# Changelog

## [0.4.0] - 2023-06-15

### Changed

- **Breaking:** Remove and change use of deprecated UdonSharp editor and Udon functions (`6708b5f`)
- Migrate to VRChat Creator Companion (`9ae838c`, `78b73b6`)

### Fixed

- Fix late joiner syncing due to networking changes in VRChat (`af4bd57`)

## [0.3.0] - 2022-07-10

### Added

- Previews for Place and Delete modes (`c2ee809`, `61af510`, `fad9053`, `97a446d`, `7039a4e`)

### Changed

- **Breaking:** Update VFXTargetGun prefab (`a69b9b8`)
- Move VFXTargetGun and ButtonEffect to `Prefabs` folder (`f3b8d03`)
- Put all cloned effects into a separate parent obj at runtime (`c24b9c7`)
- Organize syncing performance notes (`fa56e61`)

## [0.2.3] - 2022-07-04

### Changed

- Work around particle systems being null somehow (`4392c70`)

## [0.2.2] - 2022-06-28

### Fixed

- Fix delete mode error when pointing at VRChat objects (`8e93893`)

## [0.2.1] - 2022-06-28

### Added

- **Breaking:** Add one EffectOrderSync instance per gun (`f6a6f40`)
- Add performance testing notes (`422a3d5`)

### Changed

- **Breaking:** Change full syncing to a single instance per gun instead of per effect (`66ce401`)
- **Breaking:** Use content size fitter instead of manual math (`80a5ecc`)
- Use 1 UInt64 array instead of 3 separate arrays to sync effect data making packages noticeably smaller (`f4711bc`)
- Prevent inactive EffectDescriptors from syncing any data when someone joins (`e3c7f4d`)
- Improve EffectButton effect count text to support numbers >= 1000 (`a0564d2`)

### Fixed

- Fix uninitialized effects syncing data (`763f7bb`)
- Fix delayed effects using the wrong time variable making them not delayed at all (`feaa343`)

## [0.2.0] - 2022-06-25

_The entire gun needs to be replaced except for the effects. Every effect requires an EffectDescriptorFullSync as its second child. Ensure the EffectButton is also updated._

### Changed

- **Breaking:** Update VFXTargetGun prefab for V2 (`9846839`)
- Increase place indicator size and make it red (`1aeaaf1`)
- Change selected effect indication to an outline instead of underline in the UI (`5676c67`)
- Lower double click prevention from `0.175`s to `0.075`s to allow intentional double clicks (`584f600`)
- Completely hide the UI toggle when the UI is open (`07bba2d`)
- Change UI Toggle interaction proximity from `0.75` to `0.2` which is pretty close, same as place/delete mode toggle (`24248c8`)
- Change gun item UseText depending on current mode and selected effect (`85af96c`)
- Change all text to use TextMeshPro (`7fdf499`)
- Add EffectButton effect text padding and auto size for better fit and adjust stop/delete all button (`e055ad2`, `4f3f243`)

### Added

- **Breaking:** Add EffectDescriptorFullSync for syncing for late joiners per effect (`c385c78`, `3ba1f17`, `346f78a`)
- Support creating multiple of the same loop or object effect at the same time (`c379254`, `1d623ef`)
- Add delete mode along side place mode to delete loop or object effects (`dcf943a`, `0d99170`, `b7b4717`)
- Add VFXTargetGunRecallManager to teleport the most suitable gun to a player (`b0ba760`, `96d9137`, `798ff78`)
- Add buttons for changing modes in the UI (`dcf943a`, `28cc58b`, `db2d28e`, `6a4bd76`)
- Add place/delete mode switch toggle ball outside of the UI (`29269af`)
- Add delete everything button in the UI with confirmation popup (`dcf943a`, `574ab88`)
- Add active effect count per effect (`dcf943a`, `c921dc5`, `a8b82f3`)
- Add laser to better gauge where the gun is pointing at all times (`1aeaaf1`)
- Add circle around place indicator for better top down visibility (`1aeaaf1`)
- Add forwards arrow to place indicator to reduce guessing game for rotations (`ddc229b`)
- Add second laser pointing at the effect to be deleted (`99d98cf`)
- Add delete indicator which scales with the size of objects when possible (`4fac828`, `81d3366`)
- Implement pointing directly at the object to destroy (`480180a`)
- Add help window with general info and keybindings (`843c094`)
- Move UI to screen overlay for desktop users while the gun is held (`bd0d599`)
- Add `TAB` and `CTRL+TAB` keybinding to cycle selected effects (`9a629cb`, `e5f3d36`, `61e246d`, `f2eb8f8`)
- Add holding tab to quickly cycle selected effects (`1a3b2ae`)
- Add `SHIFT+`, `ALT+`, `1, 2, ..., 9, 0` keybindings to select specific effects (`41418fa`, `5286e17`)
- Add `F` keybinding to toggle between place and delete mode (`c568605`, `bf583ff`)
- Add `E` keybinding to toggle the UI (`c568605`)
- Add `Q` keybinding to deselect current effect (`c568605`)
- Add custom UI styles for sharp corners (`1679231`)

### Fixed

- Fix turning off visibility dropping all currently held guns for everyone (`d4ddb9b`)
- Fix current selected effect text not toggling properly with visibility (`07bba2d`)

## [0.1.0] - 2022-06-17

_Initial release._

### Added

- Add option to either randomly rotate effects or fix their rotation facing away from the gun (`262f239`, `2ec9ed8`)
- Support multiple particle systems per effect (`31e5185`)
- Implement creating multiple of the same once effect at the same time (`c6985d5`)
- Add once, loop and object effect type (`2335f23`, `41673fc`, `b3e0474`)
- Add selected effect text next to gun depending on holding hand (`1338eac`)
- Allow disabling loop effects without pointing at anything (`03b7cc9`)
- Prevent no longer active once effects from playing for late joiner (`4516a79`)
- Sync gun selected effect (`773805c`)
- Add target indicator where the gun is pointing (`d023b07`)
- Sync active effects (`5ec7247`, `ec93f85`, `9a3b707`)
- Add visibility toggle script with editor tool for quick gun duplicates updating (`a7adf8f`, `2018722`)
- Prevent accidental double clicks (`50cdb20`)
- Color UI toggle ball based on effect state (`e7ac187`)
- Add text for the selected effect name next to the gun (`1b579ea`)
- Add deselect effect button in UI (`e11e8da`)
- Add stop button for loop and object effects in UI (`ccfb0e8`)
- Add close button in UI (`5265aca`)
- Add color legend for effect types and states in UI (`b4951f7`)
- Add keep open toggle in UI (`c003b3f`)
- Add effect state indication using color and underline in UI (`cccc810`, `87f06bb`, `c2a401e`)
- Add basic UI (`b99f245`)
- Workaround for VRCInstantiate being weird (`4e4840e`)

<!-- VFXTargetGun_v0.4.0 -->
<!-- VFXTargetGun_v0.3.0 -->
<!-- VFXTargetGun_v0.2.3 -->
<!-- VFXTargetGun_v0.2.2 -->
<!-- VFXTargetGun_v0.2.1 -->
<!-- VFXTargetGun_v0.2.0 -->
<!-- VFXTargetGun_v0.1.0 -->

[0.4.0]: /dev/null
[0.3.0]: /dev/null
[0.2.3]: /dev/null
[0.2.2]: /dev/null
[0.2.1]: /dev/null
[0.2.0]: /dev/null
[0.1.0]: /dev/null
