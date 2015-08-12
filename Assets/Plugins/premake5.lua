location "build"
solution "cpp_plugin"
  configurations {"Debug", "x86", "x86_64"}
  language "C++"

  configuration "Debug"
    defines { "DEBUG" }
    flags { "Symbols"}
    targetsuffix "_d"

  configuration "x86 or x86_64"
    defines { "NDEBUG" }
    flags { "Optimize"}

  project "cpp_plugin"
    kind "SharedLib"
    files { "cpp_plugin/**.cpp", "cpp_plugin/**.h" }

    configuration "x86"
      targetdir "x86/"

    configuration "x86_64"
      targetdir "x86_64/"
