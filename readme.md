# Avatar Base Setup
A small utility that sets up [VRCFT Templates](https://github.com/Adjerry91/VRCFaceTracking-Templates) and your own controllers in an intelligent-ish manner.

Layers not in use will have their weight set to zero, omitting them from being processed by Unity. If a user doesn't use face-tracking, then there is no need to keep those layers enabled.

**WIP implementation for Unity 2022.3+. Do not use productively!**

* Animation repathing is supported
* Works with or without ModularAvatar and VRCFury
* Non-destructive, automatically runs on VRChat upload
* Setup can be applied in advance. Your users won't have to install this package, if you include the output in your `.unitypackage`.
* Also works without VRCFT Templates present. (Face tracking won't be applied, everything else will)

## How To Use
* Add the `AVA/VRChat/AVA Base Setup` Component to the root of your avatar. (The same GameObject where the VRChatAvatarDescriptor is placed)
* Split up your animator controller into appropriate pieces.
* Place the partial controllers in the appropriate slots.
* Profit!

If the mesh with the face-tracking blendshapes is not named `Body`, add the `AVA/VRChat/Face Tracking Producer` component and specify the face mesh.

## FAQ
* Who is this for?\
	Avatar base creators.
* What does this mean for users?\
	Always up do date VRCFT Templates.
* Is this useful if I make avatar addons? (Clothing, and what not)\
	Nope.
* Does this replace ModularAvatar or VRCFury?\
	Nope, these operate on top of the setup created by this package.

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
