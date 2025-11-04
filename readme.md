# Avatar Base Setup
A small utility that sets up [VRCFT Templates](https://github.com/Adjerry91/VRCFaceTracking-Templates) and your own controllers in an intelligent-ish manner.

**WIP implementation for Unity 2022.3+. Do not use productively!**

Layers not in use will have their weight set to zero, omitting them from being processed by Unity. If a user doesn't use face-tracking, then there is no need to keep those layers enabled.

* Works with or without ModularAvatar and VRCFury
* Animation repathing is supported
* Non-destructive, automatically runs on VRChat upload
* Also works without VRCFT Templates present, to account for any eventuality. (Face tracking won't be applied, everything else will)

## How To Use
* Add the `AVA/VRChat/AVA Base Setup` Component to the root of your avatar. (The same GameObject where the VRChatAvatarDescriptor is placed)
* Split up your animator controller into appropriate pieces.
* Place the partial controllers in the appropriate slots.
* Profit

<!--
## Installation
* VRChat Creator Companion: https://squirrelbite.github.io/vpm/
* Unity Package Manager: `Window` -> `Package Manager` -> `+` -> `Add package from git URL...`
Add the following URL: `https://github.com/emperorofmars/ava_base_setup.git#upm`
-->

## License
All source-code in this repository, except when noted in individual files and/or directories, is licensed under either:

* MIT License (LICENSE-MIT or <http://opensource.org/licenses/MIT>)
* Apache License, Version 2.0 (LICENSE-APACHE2 or <http://www.apache.org/licenses/LICENSE-2.0>)
