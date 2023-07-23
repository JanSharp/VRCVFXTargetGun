
# VFX Target Gun

Infrastructure for vfx and object placable in world at runtime using a handheld gun.

# Installing

Head to my [VCC Listing](https://jansharp.github.io/vrc/vcclisting.xhtml) and follow the instructions there.

# Prefab Variants

For the best experience, use a prefab variant:
- Create a prefab variant of the VFXTargetGun prefab (right click => Create => Prefab Variant)
- Call it whatever you want
- Move it into some folder in your Assets
- Add children to the `Effects` object in the prefab
- Create multiple instances of your prefab variant and use those in your scene

Now when updating this package, all change to the main prefab will get applied to all of your instances in the scene without you having to do anything. So long as it's not breaking changes in regards to the effects parent setup.

# Effect Descriptor Structure

Every child of the `Effects` object in the gun prefab should take the following form:
```
Effects
┣╸MyEffect (with Effect Descriptor)
┃ ┣╸EffectObject (A single object containing all objects that ultimately define are the effect/object)
┃ ┗╸EffectClones (An empty object required by the Effect Descriptor)
┗╸...
```
The order of the children of the Effect Descriptor matters, and there must not be any more children than those 2.

If the `EffectObject` contains particle systems it is automatically considered a particle effect and if any of them are loop effects it is considered a loop particle effect. Otherwise it is an object effect.
