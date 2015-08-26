location "build"
solution "cpp_plugin"
  configurations {"Debug", "Release"}
  platforms { "x32", "x64" }
  language "C++"

  configuration "Debug"
    defines { "DEBUG" }
    flags { "Symbols"}
    targetsuffix "_d"

  configuration "Release"
    defines { "NDEBUG" }
    flags { "Optimize"}

  project "cpp_plugin"
    kind "SharedLib"
    files { "cpp_plugin/**.cpp", "cpp_plugin/**.h" }

    configuration "x32"
      targetdir "x86/"

    configuration "x64"
      targetdir "x86_64/"
