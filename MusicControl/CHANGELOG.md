
# Changelog

## [0.2.0] - 2022-06-12

### Changed

- Rename MusicToggle to MusicArea (`6307354`)
- Improve input validation and error handling (`a0be987`)
- Migrate to VRChat Creator Companion (`9ae838c`, `78b73b6`)

### Fixed

- Fix respawning while in a music trigger zone breaking the MusicManager (`a0be987`)
- Fix using the same MusicDescriptor as default and for an area always having lowest priority (`b9975e6`)
- Fix MusicToggle not checking colliding player being local (`b8d5dad`)

## [0.1.1] - 2022-09-03

### Fixed

- Fix MusicToggle not checking colliding player being local (`b8d5dad`)

## [0.1.0] - 2022-08-25

### Added

- Add concept of default music which plays by default, like at spawn and outside of any trigger (`3c33f1d`, `52c8c19`, `fcf3d5a`)
- Add concept of a music stack, used by trigger zones which change music (`3c33f1d`, `eb6ebd3`, `ec5a4c7`, `4211fe2`, `b4707fb`, `9f72b56`)
- Add script to change default music (`c5f49d9`)

<!-- MusicControl_v0.2.0 -->
<!-- MusicControl_v0.1.1 -->
<!-- MusicControl_v0.1.0 -->

[0.2.0]: /dev/null
[0.1.1]: /dev/null
[0.1.0]: /dev/null
