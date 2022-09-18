
# Changelog

## [1.0.1] - 2022-09-03

### Changed

- Change late joiner sync delay from 10s to 15s after someone joined (`fb1f91a`)
- Put debug log messages behind preprocessor `#if`s to get rid of log message spam in production builds (`20c8594`)

### Fixed

- Fix progress bar being full while waiting for late joiner sync to start after joining (`5d8b2c1`)
- Fix potential issues like soft locks with multiple people joining within 10 seconds (`ea5ad02`)

## [1.0.0] - 2022-08-19

### Added

- Add chalks drawing points and lines on chalkboards (`7d6032c`, `2e3dc8a`, `29f465f`, `4fb08c2`, `8d85b85`, `6e93bc3`, `e019b13`, `e89d194`, `cdfeb18`, `5ae8393`, `4b38b50`, `b13f02f`, `8a81f0c`, `af9804f`, `e89aade`)
- Implement sponges which are really just chalks with a bigger and square size (`ea62341`)
- Manage texture update rate for drawing, seeing someone draw and being really far away also depending on board texture size (`835d575`, `a07272a`, `f429e35`)
- Add syncing for both people in the map and late joiners (`fa73f27`, `81e2cfa`, `f9cab30`, `51c64ee`, `a2b549b`, `0ff2cd2`, `6664994`, `a5706c7`, `a396ba9`, `17e4cfa`)
- Add optional progress bar for late joiners (`94efb84`)
- Implement board clearing (`0fbd3b4`)
- Support using the same chalk on multiple boards (`a86c5ef`)
- Support all kinds of rotations of the chalkboard (`1f46ec9`)
- Make chalk pickup have fixed grip in desktop vs any grip in VR (`a4def30`)

<!-- Chalkboard_v1.0.0 -->

[1.0.0]: /dev/null
