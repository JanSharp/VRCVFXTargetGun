#! /usr/bin/env vrc_class_gen.lua

return {
  is_class_definition = true,
  usings = {
    "UdonSharp",
    "UnityEngine",
    "VRC.SDKBase",
    "VRC.Udon",
  },
  namespace = "JanSharp",
  class_name = "VFXEffectData",
  fields = {
    {type = "uint", name = "effectId"},
    {type = "int", name = "owningPlayerId"},
    {type = "object[]", name = "owningPlayerData"},
    {type = "uint", name = "createdTick"},
    {type = "EffectDescriptor", name = "descriptor"},
    {type = "Vector3", name = "position"},
    {type = "Quaternion", name = "rotation"},
    {type = "ulong", name = "uniqueId"}, -- Not part of the game state.
    {type = "int", name = "effectIndex", default = "-1"}, -- Not part of the game state.
  },
}
