
# Changelog

## [0.4.4] - 2023-07-23

### Added

- Add notes about required initial state for particle systems and object effects ([`48592ea`](https://github.com/JanSharp/VRCVFXTargetGun/commit/48592ea58ea6a1a5ef4125323b2b234eaa709171))

## [0.4.3] - 2023-07-23

### Changed

- Disable UI in the gun prefab as that is the correct initial state ([`84aabe1`](https://github.com/JanSharp/VRCVFXTargetGun/commit/84aabe101284e182f283a94894e0033248300dae))

### Added

- Add notes about prefab variants and effect descriptors in readme ([`62ec53d`](https://github.com/JanSharp/VRCVFXTargetGun/commit/62ec53d7acc0a353eaaa9b8b09131702a9cfaf68))

### Removed

- Remove dev only test effects from the gun prefab ([`822fd45`](https://github.com/JanSharp/VRCVFXTargetGun/commit/822fd455f83613de938367c49704b48f691668c9))

## [0.4.2] - 2023-07-23

### Changed

- Exclude blender files from published packages ([`3836a06`](https://github.com/JanSharp/VRCVFXTargetGun/commit/3836a06a5e313969d412bfee92316f846ceaf4ed))

### Fixed

- Fix tag links in changelog ([`58171be`](https://github.com/JanSharp/VRCVFXTargetGun/commit/58171be6689d31d14d09fbc7e27c5c61de15204b))

## [0.4.1] - 2023-07-23

_First version of this package that is in the VCC listing._

### Changed

- **Breaking:** Separate VFX Target Gun into its own repo and make it VPM compatible ([`aa8eaa6`](https://github.com/JanSharp/VRCVFXTargetGun/commit/aa8eaa6fc0f21ab35cede4496ec90b7667da4c37), [`44d00c7`](https://github.com/JanSharp/VRCVFXTargetGun/commit/44d00c7d06cbb74101901930fe83c603d1ee85f3))
- **Breaking:** Update OnBuildUtil and other general editor scripting, use SerializedObjects ([`de04745`](https://github.com/JanSharp/VRCVFXTargetGun/commit/de04745880f0ea37345b5fd4e54de94fe7f05368), [`ee4ffb5`](https://github.com/JanSharp/VRCVFXTargetGun/commit/ee4ffb5ffe6218097cd01b94becc93bafb6ad2ca))
- Move EffectButton prefab into VFXTargetGun prefab ([`d6f876f`](https://github.com/JanSharp/VRCVFXTargetGun/commit/d6f876f07b190c0bdad212382e2a840f83bf1cc9))
- Hide edit mode button as it doesn't do anything ([`b421fe2`](https://github.com/JanSharp/VRCVFXTargetGun/commit/b421fe26df7e6aafebf78746ee750fe53fc0a94a))

## [0.4.0] - 2023-06-15

### Changed

- **Breaking:** Remove and change use of deprecated UdonSharp editor and Udon functions ([`6708b5f`](https://github.com/JanSharp/VRCVFXTargetGun/commit/6708b5f32894a2ecb6ac976922d025f0fe25eb40))
- Migrate to VRChat Creator Companion ([`9ae838c`](https://github.com/JanSharp/VRCVFXTargetGun/commit/9ae838cf1d6280c64c607559fb3ae9967b52bd99), [`78b73b6`](https://github.com/JanSharp/VRCVFXTargetGun/commit/78b73b6816612602b04daafeb4097351f087c01a))

### Fixed

- Fix late joiner syncing due to networking changes in VRChat ([`af4bd57`](https://github.com/JanSharp/VRCVFXTargetGun/commit/af4bd573062f8dfd5460a4b67e1d3dd2f4758aa1))

## [0.3.0] - 2022-07-10

### Added

- Previews for Place and Delete modes ([`c2ee809`](https://github.com/JanSharp/VRCVFXTargetGun/commit/c2ee809d2609abd304f621d5ef6859628aecf5ef), [`61af510`](https://github.com/JanSharp/VRCVFXTargetGun/commit/61af5109eafe71e73d2adacbc90ae33773a11781), [`fad9053`](https://github.com/JanSharp/VRCVFXTargetGun/commit/fad9053cf7fbe812fd58ea1a2e1f8ade192ad24f), [`97a446d`](https://github.com/JanSharp/VRCVFXTargetGun/commit/97a446d692eb3e7ca1e0bf46546b04b3e944b744), [`7039a4e`](https://github.com/JanSharp/VRCVFXTargetGun/commit/7039a4e51e2f46d8d002bea0988de052746bbc3c))

### Changed

- **Breaking:** Update VFXTargetGun prefab ([`a69b9b8`](https://github.com/JanSharp/VRCVFXTargetGun/commit/a69b9b88918993bb8035ec9ceda739025852a2e1))
- Move VFXTargetGun and ButtonEffect to `Prefabs` folder ([`f3b8d03`](https://github.com/JanSharp/VRCVFXTargetGun/commit/f3b8d03d71ae094e2e10aaa96ef0c91a7821029b))
- Put all cloned effects into a separate parent obj at runtime ([`c24b9c7`](https://github.com/JanSharp/VRCVFXTargetGun/commit/c24b9c7dcf9b07b0fcd25d0aaffab5ab02dfb93e))
- Organize syncing performance notes ([`fa56e61`](https://github.com/JanSharp/VRCVFXTargetGun/commit/fa56e61145e80dbd872405a4d71e3d7dd86caa2e))

## [0.2.3] - 2022-07-04

### Changed

- Work around particle systems being null somehow ([`4392c70`](https://github.com/JanSharp/VRCVFXTargetGun/commit/4392c7027d27daaa0e2ccab98e88e8b1e87df767))

## [0.2.2] - 2022-06-28

### Fixed

- Fix delete mode error when pointing at VRChat objects ([`8e93893`](https://github.com/JanSharp/VRCVFXTargetGun/commit/8e93893561ac3a27b4ac18958381c38be69a6a32))

## [0.2.1] - 2022-06-28

### Added

- **Breaking:** Add one EffectOrderSync instance per gun ([`f6a6f40`](https://github.com/JanSharp/VRCVFXTargetGun/commit/f6a6f40b071afbcbfdd2e9b0cdc2b42c2d806234))
- Add performance testing notes ([`422a3d5`](https://github.com/JanSharp/VRCVFXTargetGun/commit/422a3d5535a7637fce6f67da91f53ccdb5dc8632))

### Changed

- **Breaking:** Change full syncing to a single instance per gun instead of per effect ([`66ce401`](https://github.com/JanSharp/VRCVFXTargetGun/commit/66ce401587275d054a9b3cffc34a3af7fb50a3a0))
- **Breaking:** Use content size fitter instead of manual math ([`80a5ecc`](https://github.com/JanSharp/VRCVFXTargetGun/commit/80a5ecc20377a2d240e6b379fe1f391477d78f1e))
- Use 1 UInt64 array instead of 3 separate arrays to sync effect data making packages noticeably smaller ([`f4711bc`](https://github.com/JanSharp/VRCVFXTargetGun/commit/f4711bc345dbf417c464a3bcb8a2ef3735f51c41))
- Prevent inactive EffectDescriptors from syncing any data when someone joins ([`e3c7f4d`](https://github.com/JanSharp/VRCVFXTargetGun/commit/e3c7f4deab21e86fa83832eb4270f36af3ad4c7c))
- Improve EffectButton effect count text to support numbers >= 1000 ([`a0564d2`](https://github.com/JanSharp/VRCVFXTargetGun/commit/a0564d20fe81d0796e97ad44e0ebfeb076742021))

### Fixed

- Fix uninitialized effects syncing data ([`763f7bb`](https://github.com/JanSharp/VRCVFXTargetGun/commit/763f7bbc8f8970f7076cad9ba70d2a05aba81f2a))
- Fix delayed effects using the wrong time variable making them not delayed at all ([`feaa343`](https://github.com/JanSharp/VRCVFXTargetGun/commit/feaa3435fbf23df69aba8bd74ef133d03d7388e1))

## [0.2.0] - 2022-06-25

_The entire gun needs to be replaced except for the effects. Every effect requires an EffectDescriptorFullSync as its second child. Ensure the EffectButton is also updated._

### Changed

- **Breaking:** Update VFXTargetGun prefab for V2 ([`9846839`](https://github.com/JanSharp/VRCVFXTargetGun/commit/9846839aa2d28bbad7fee2cda60d2a60f3d680e2))
- Increase place indicator size and make it red ([`1aeaaf1`](https://github.com/JanSharp/VRCVFXTargetGun/commit/1aeaaf1a2b414521c49504e8283c99ad1fb9fe16))
- Change selected effect indication to an outline instead of underline in the UI ([`5676c67`](https://github.com/JanSharp/VRCVFXTargetGun/commit/5676c671d4c144507a8bb265e960479e6a7eef27))
- Lower double click prevention from `0.175`s to `0.075`s to allow intentional double clicks ([`584f600`](https://github.com/JanSharp/VRCVFXTargetGun/commit/584f6005581ec2398750bc630d639ec1fda7dbeb))
- Completely hide the UI toggle when the UI is open ([`07bba2d`](https://github.com/JanSharp/VRCVFXTargetGun/commit/07bba2d362459eb9109d58f7bd73269fbfe299b4))
- Change UI Toggle interaction proximity from `0.75` to `0.2` which is pretty close, same as place/delete mode toggle ([`24248c8`](https://github.com/JanSharp/VRCVFXTargetGun/commit/24248c8623e32a4c758aebd5944f176a9fc2f78d))
- Change gun item UseText depending on current mode and selected effect ([`85af96c`](https://github.com/JanSharp/VRCVFXTargetGun/commit/85af96caf4596bdce0dff628b56f4b7acd127b7e))
- Change all text to use TextMeshPro ([`7fdf499`](https://github.com/JanSharp/VRCVFXTargetGun/commit/7fdf4998cba27904be857a87231dc9827c27fcc9))
- Add EffectButton effect text padding and auto size for better fit and adjust stop/delete all button ([`e055ad2`](https://github.com/JanSharp/VRCVFXTargetGun/commit/e055ad202ca66ab6ac40c5c2e1c8bbcea225dbfd), [`4f3f243`](https://github.com/JanSharp/VRCVFXTargetGun/commit/4f3f243a0ff7669dbfc37167341856c7e8b5b758))

### Added

- **Breaking:** Add EffectDescriptorFullSync for syncing for late joiners per effect ([`c385c78`](https://github.com/JanSharp/VRCVFXTargetGun/commit/c385c783c26280f7e362de30dadb8d26913e3e9c), [`3ba1f17`](https://github.com/JanSharp/VRCVFXTargetGun/commit/3ba1f179b942a95f8d5ccc470bee2fde10d67f08), [`346f78a`](https://github.com/JanSharp/VRCVFXTargetGun/commit/346f78a5592719d69b537aa9f2008d8071f26d9c))
- Support creating multiple of the same loop or object effect at the same time ([`c379254`](https://github.com/JanSharp/VRCVFXTargetGun/commit/c37925495a4ca447f53f3e686cbbacd5a5104ef9), [`1d623ef`](https://github.com/JanSharp/VRCVFXTargetGun/commit/1d623efbeacbd6145f83225b431954ae7dfc9da7))
- Add delete mode along side place mode to delete loop or object effects ([`dcf943a`](https://github.com/JanSharp/VRCVFXTargetGun/commit/dcf943ad026de3c7a5a23032f444cfcc89305c70), [`0d99170`](https://github.com/JanSharp/VRCVFXTargetGun/commit/0d991703e1e28a0c1a2040d738c59f3dde195da2), [`b7b4717`](https://github.com/JanSharp/VRCVFXTargetGun/commit/b7b471759b1ba1668e2caeefd4f63c4f5616e753))
- Add VFXTargetGunRecallManager to teleport the most suitable gun to a player ([`b0ba760`](https://github.com/JanSharp/VRCVFXTargetGun/commit/b0ba760a1df8a652ac13fee48221d71e5334d3b5), [`96d9137`](https://github.com/JanSharp/VRCVFXTargetGun/commit/96d91372216d576861ef0d8c0685a9f5206375a4), [`798ff78`](https://github.com/JanSharp/VRCVFXTargetGun/commit/798ff78ad3aa06c4cf68b50eb0e7db8eadd6572a))
- Add buttons for changing modes in the UI ([`dcf943a`](https://github.com/JanSharp/VRCVFXTargetGun/commit/dcf943ad026de3c7a5a23032f444cfcc89305c70), [`28cc58b`](https://github.com/JanSharp/VRCVFXTargetGun/commit/28cc58bc6262eae52db7c83f69afb537d73ef0cb), [`db2d28e`](https://github.com/JanSharp/VRCVFXTargetGun/commit/db2d28ee9616e7649260739d9902546307c82480), [`6a4bd76`](https://github.com/JanSharp/VRCVFXTargetGun/commit/6a4bd76facde78a2051af1a76640ff5c079f2820))
- Add place/delete mode switch toggle ball outside of the UI ([`29269af`](https://github.com/JanSharp/VRCVFXTargetGun/commit/29269af78dcbd45a40239862834d6e58c803a3ff))
- Add delete everything button in the UI with confirmation popup ([`dcf943a`](https://github.com/JanSharp/VRCVFXTargetGun/commit/dcf943ad026de3c7a5a23032f444cfcc89305c70), [`574ab88`](https://github.com/JanSharp/VRCVFXTargetGun/commit/574ab88831f2e1bfa8d89b1c0648a1a4bc8b8dad))
- Add active effect count per effect ([`dcf943a`](https://github.com/JanSharp/VRCVFXTargetGun/commit/dcf943ad026de3c7a5a23032f444cfcc89305c70), [`c921dc5`](https://github.com/JanSharp/VRCVFXTargetGun/commit/c921dc5a53fa6865b38582ca67d7a5bd8c727dc6), [`a8b82f3`](https://github.com/JanSharp/VRCVFXTargetGun/commit/a8b82f3e1d0d82e24278fa77837f06a90f615585))
- Add laser to better gauge where the gun is pointing at all times ([`1aeaaf1`](https://github.com/JanSharp/VRCVFXTargetGun/commit/1aeaaf1a2b414521c49504e8283c99ad1fb9fe16))
- Add circle around place indicator for better top down visibility ([`1aeaaf1`](https://github.com/JanSharp/VRCVFXTargetGun/commit/1aeaaf1a2b414521c49504e8283c99ad1fb9fe16))
- Add forwards arrow to place indicator to reduce guessing game for rotations ([`ddc229b`](https://github.com/JanSharp/VRCVFXTargetGun/commit/ddc229b487f795f37ecf5f620c67c462df1e0be7))
- Add second laser pointing at the effect to be deleted ([`99d98cf`](https://github.com/JanSharp/VRCVFXTargetGun/commit/99d98cf14f10ba7795c4e227327e511588eb3adb))
- Add delete indicator which scales with the size of objects when possible ([`4fac828`](https://github.com/JanSharp/VRCVFXTargetGun/commit/4fac828136a5d62bcee8a72260c621e68d88b471), [`81d3366`](https://github.com/JanSharp/VRCVFXTargetGun/commit/81d3366208cc10d5760330ab1febb5d59e229b19))
- Implement pointing directly at the object to destroy ([`480180a`](https://github.com/JanSharp/VRCVFXTargetGun/commit/480180a9a38859b60e42958f789c88d90f784150))
- Add help window with general info and keybindings ([`843c094`](https://github.com/JanSharp/VRCVFXTargetGun/commit/843c0941b3f88e9a36a37b489a71a0f4521e33d4))
- Move UI to screen overlay for desktop users while the gun is held ([`bd0d599`](https://github.com/JanSharp/VRCVFXTargetGun/commit/bd0d59959f32f8499d50a2f1cbd62fe341999af1))
- Add `TAB` and `CTRL+TAB` keybinding to cycle selected effects ([`9a629cb`](https://github.com/JanSharp/VRCVFXTargetGun/commit/9a629cbc9b31ce8eb4dc08f0c9914cce4ab76499), [`e5f3d36`](https://github.com/JanSharp/VRCVFXTargetGun/commit/e5f3d36329fcfe83928e10a1fa8e8ba389adc82e), [`61e246d`](https://github.com/JanSharp/VRCVFXTargetGun/commit/61e246d455a0d32fb64d58ecb9bef42fe1d7c94b), [`f2eb8f8`](https://github.com/JanSharp/VRCVFXTargetGun/commit/f2eb8f8845f25beb30ed95c77a5e7760aeb72439))
- Add holding tab to quickly cycle selected effects ([`1a3b2ae`](https://github.com/JanSharp/VRCVFXTargetGun/commit/1a3b2aec665997d461ed07aa5020a9d9b08e1b00))
- Add `SHIFT+`, `ALT+`, `1, 2, ..., 9, 0` keybindings to select specific effects ([`41418fa`](https://github.com/JanSharp/VRCVFXTargetGun/commit/41418fa2c0b07f76235afdc32286e9c55cd9e27b), [`5286e17`](https://github.com/JanSharp/VRCVFXTargetGun/commit/5286e17e966b49316d7dfda39703c9f98679c235))
- Add `F` keybinding to toggle between place and delete mode ([`c568605`](https://github.com/JanSharp/VRCVFXTargetGun/commit/c568605f055e7ce17ebf13c7ba69355d90d71ce3), [`bf583ff`](https://github.com/JanSharp/VRCVFXTargetGun/commit/bf583ffb6f90f321379ddbf4f5d019e9c2e9f39d))
- Add `E` keybinding to toggle the UI ([`c568605`](https://github.com/JanSharp/VRCVFXTargetGun/commit/c568605f055e7ce17ebf13c7ba69355d90d71ce3))
- Add `Q` keybinding to deselect current effect ([`c568605`](https://github.com/JanSharp/VRCVFXTargetGun/commit/c568605f055e7ce17ebf13c7ba69355d90d71ce3))
- Add custom UI styles for sharp corners ([`1679231`](https://github.com/JanSharp/VRCVFXTargetGun/commit/16792319e0663d84edc7df2a341955d26c424f52))

### Fixed

- Fix turning off visibility dropping all currently held guns for everyone ([`d4ddb9b`](https://github.com/JanSharp/VRCVFXTargetGun/commit/d4ddb9b728dc05b7144f55dd837bb9ad7541e97a))
- Fix current selected effect text not toggling properly with visibility ([`07bba2d`](https://github.com/JanSharp/VRCVFXTargetGun/commit/07bba2d362459eb9109d58f7bd73269fbfe299b4))

## [0.1.0] - 2022-06-17

_Initial release._

### Added

- Add option to either randomly rotate effects or fix their rotation facing away from the gun ([`262f239`](https://github.com/JanSharp/VRCVFXTargetGun/commit/262f239719a46c9b0936145883dfdbd426489630), [`2ec9ed8`](https://github.com/JanSharp/VRCVFXTargetGun/commit/2ec9ed8a14f26b2dececd58f6de3a7e8b987b4f5))
- Support multiple particle systems per effect ([`31e5185`](https://github.com/JanSharp/VRCVFXTargetGun/commit/31e5185c6fd73364ec4df82b2069ca1c359ce308))
- Implement creating multiple of the same once effect at the same time ([`c6985d5`](https://github.com/JanSharp/VRCVFXTargetGun/commit/c6985d5baf69081f2ce45b1f00d938a95d31c88b))
- Add once, loop and object effect type ([`2335f23`](https://github.com/JanSharp/VRCVFXTargetGun/commit/2335f23863e09a94bcc4c5f61e5386041840d044), [`41673fc`](https://github.com/JanSharp/VRCVFXTargetGun/commit/41673fc256f7af4522f1ebf3589653c13be381d8), [`b3e0474`](https://github.com/JanSharp/VRCVFXTargetGun/commit/b3e047425ff476f1570e6704b99f03e73c51c302))
- Add selected effect text next to gun depending on holding hand ([`1338eac`](https://github.com/JanSharp/VRCVFXTargetGun/commit/1338eacca810bab159bc8acbdc58a52ca9bfee1e))
- Allow disabling loop effects without pointing at anything ([`03b7cc9`](https://github.com/JanSharp/VRCVFXTargetGun/commit/03b7cc90c28493259dae7b107c3574e5781aa5bc))
- Prevent no longer active once effects from playing for late joiner ([`4516a79`](https://github.com/JanSharp/VRCVFXTargetGun/commit/4516a7933d3037e048c1a46e4f50cfbc3a389e2a))
- Sync gun selected effect ([`773805c`](https://github.com/JanSharp/VRCVFXTargetGun/commit/773805cc1b5e4b1277d6225ba7779b28aa4635c8))
- Add target indicator where the gun is pointing ([`d023b07`](https://github.com/JanSharp/VRCVFXTargetGun/commit/d023b07ccddff6be964ae293fd3005d990a4d95f))
- Sync active effects ([`5ec7247`](https://github.com/JanSharp/VRCVFXTargetGun/commit/5ec7247187e5cb3a5e02f19b4f475a8e96c808b6), [`ec93f85`](https://github.com/JanSharp/VRCVFXTargetGun/commit/ec93f8502c18048dd670b7a7a497ac82bc643ea3), [`9a3b707`](https://github.com/JanSharp/VRCVFXTargetGun/commit/9a3b707f31ddc39de613175290abc80d727b757e))
- Add visibility toggle script with editor tool for quick gun duplicates updating ([`a7adf8f`](https://github.com/JanSharp/VRCVFXTargetGun/commit/a7adf8febd8ef5376b6d4db8db43414cb9a36701), [`2018722`](https://github.com/JanSharp/VRCVFXTargetGun/commit/2018722f1589eb562067bc2d26efcdde2d621770))
- Prevent accidental double clicks ([`50cdb20`](https://github.com/JanSharp/VRCVFXTargetGun/commit/50cdb208264be30f43ec8b8f8853484c9fead00b))
- Color UI toggle ball based on effect state ([`e7ac187`](https://github.com/JanSharp/VRCVFXTargetGun/commit/e7ac1878486069195769a506bd52db0487e57bf8))
- Add text for the selected effect name next to the gun ([`1b579ea`](https://github.com/JanSharp/VRCVFXTargetGun/commit/1b579ea6ef924f6291f700b9fb94caa431c65fc9))
- Add deselect effect button in UI ([`e11e8da`](https://github.com/JanSharp/VRCVFXTargetGun/commit/e11e8da572656e7190a785a15d80931fa2568f8c))
- Add stop button for loop and object effects in UI ([`ccfb0e8`](https://github.com/JanSharp/VRCVFXTargetGun/commit/ccfb0e8aae68a73e81da152dfffec3b1b5dc9ae2))
- Add close button in UI ([`5265aca`](https://github.com/JanSharp/VRCVFXTargetGun/commit/5265aca9b77d561d073c1dcacb6f3a5fbf1030c5))
- Add color legend for effect types and states in UI ([`b4951f7`](https://github.com/JanSharp/VRCVFXTargetGun/commit/b4951f7b8669bec0b9863be2e813c7e450a8a3ec))
- Add keep open toggle in UI ([`c003b3f`](https://github.com/JanSharp/VRCVFXTargetGun/commit/c003b3ff4effa88cb626418cdfd1e995979e02a5))
- Add effect state indication using color and underline in UI ([`cccc810`](https://github.com/JanSharp/VRCVFXTargetGun/commit/cccc810509a5e33da3fe6fe74918405f7a736e7d), [`87f06bb`](https://github.com/JanSharp/VRCVFXTargetGun/commit/87f06bbb25fc7988757d98593f52e4c6b830e1db), [`c2a401e`](https://github.com/JanSharp/VRCVFXTargetGun/commit/c2a401e819aa98c319cfe979375a1f61617f86a9))
- Add basic UI ([`b99f245`](https://github.com/JanSharp/VRCVFXTargetGun/commit/b99f2458b5de112ad58a25d7d378e146fd7112fb))
- Workaround for VRCInstantiate being weird ([`4e4840e`](https://github.com/JanSharp/VRCVFXTargetGun/commit/4e4840e50b88570484a528fd599ee78e6c26026c))

[0.4.4]: https://github.com/JanSharp/VRCVFXTargetGun/releases/tag/v0.4.4
[0.4.3]: https://github.com/JanSharp/VRCVFXTargetGun/releases/tag/v0.4.3
[0.4.2]: https://github.com/JanSharp/VRCVFXTargetGun/releases/tag/v0.4.2
[0.4.1]: https://github.com/JanSharp/VRCVFXTargetGun/releases/tag/v0.4.1
[0.4.0]: https://github.com/JanSharp/VRCVFXTargetGun/releases/tag/VFXTargetGun_v0.4.0
[0.3.0]: https://github.com/JanSharp/VRCVFXTargetGun/releases/tag/VFXTargetGun_v0.3.0
[0.2.3]: https://github.com/JanSharp/VRCVFXTargetGun/releases/tag/VFXTargetGun_v0.2.3
[0.2.2]: https://github.com/JanSharp/VRCVFXTargetGun/releases/tag/VFXTargetGun_v0.2.2
[0.2.1]: https://github.com/JanSharp/VRCVFXTargetGun/releases/tag/VFXTargetGun_v0.2.1
[0.2.0]: https://github.com/JanSharp/VRCVFXTargetGun/releases/tag/VFXTargetGun_v0.2.0
[0.1.0]: https://github.com/JanSharp/VRCVFXTargetGun/releases/tag/VFXTargetGun_v0.1.0
