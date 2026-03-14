# Avatar Base Setup
A small utility that sets up [VRCFT Templates](https://github.com/Adjerry91/VRCFaceTracking-Templates) and your own controllers in an intelligent-ish manner.

🌰 **[Report Issues](https://codeberg.org/emperorofmars/ava_base_setup/issues)**

Layers not in use will have their weight set to zero, omitting them from being processed by Unity. If a user doesn't use face-tracking, then there is no need to keep those layers enabled.

* Animation repathing is supported
* Works with or without ModularAvatar and VRCFury
* Non-destructive, automatically runs on VRChat upload
* Setup can be applied in advance. Your users won't have to install this package, if you include the output in your `.unitypackage`.
* Also works without VRCFT Templates present. (Face tracking won't be applied, everything else will)

## How To Use
* Add the `AVA/VRChat/Avatar Setup` Component to the root, or a new GameObject parented to the root of your avatar.
* Add the `AVA/VRChat/Behaviours/Face Tracking` Component to the same or child GameObject.
* Split up your animator controllers, menus and parameters into appropriate pieces.
* For each partial controller, create a `AVA/VRChat/Behaviours/Animator Controller` Component and reference the relevant bits there.
* Profit!

If the mesh with the face-tracking blendshapes is not named `Body`, add the `AVA/VRChat/Face Tracking Producer` component and specify the face mesh.

## FAQ
* Who is this for?\
	Avatar base creators.
* What does this mean for users?\
	Always up do date face-tracking setup & better performance.
* Is this useful if I make avatar addons? (Clothing and such)\
	Nope, use ModularAvatar or VRCFury.
* Does this replace or compete with ModularAvatar or VRCFury?\
	Nope, these operate on top of the base setup created by this package.

## Installation
* VRChat Creator Companion: https://vpm.squirrelbite.com/
* Unity Package Manager: `Window` → `Package Manager` → `+` → `Add package from git URL...`
Add the following URL: `https://codeberg.org/emperorofmars/ava_base_setup.git#upm`

Please open issues for any bugs or misbehavior you notice. Feel free to open issues for feature requests.

## Contributing
Human made contributions via pull-requests are welcome.

### Guidelines
* Any form of LLM contribution is prohibited, this also includes issues and PRs.
* Please open an issue first for larger changes.

### Development
* Clone the repository into the `Packages` directory of a Unity 2022.3+ project set up with the VRChat SDK for Avatars.
* Preferably setup Unity to use VSCode with the [recommended extensions](./.vscode/extensions.json).
* Use Unity to open the C# project.

## License
All source-code in this repository, except when noted in individual files and/or directories, is licensed under either:

* MIT License (LICENSE-MIT or <http://opensource.org/licenses/MIT>)
* Apache License, Version 2.0 (LICENSE-APACHE2 or <http://www.apache.org/licenses/LICENSE-2.0>)
