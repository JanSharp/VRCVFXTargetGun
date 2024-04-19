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
  class_name = "VFXPlayerData",
  fields = {
    {type = "uint", name = "playerId"},
    {type = "string", name = "displayName"},
    {type = "uint", name = "ownedEffectCount"},
    {type = "uint", name = "cloneCount"},
  },
}
