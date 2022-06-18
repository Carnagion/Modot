# Modot

**Modot** is a mod loader for applications made using Godot, inspired heavily by [RimWorld](https://rimworldgame.com)'s mod loading process.

Its API is aimed at allowing creators to easily modularise their Godot applications, create and deploy patches and DLCs, and let users expand the functionality of their applications.

# Features

- Load mods with C# assemblies, XML data, and resource packs at runtime
- Sort mods using load orders defined partially by each mod to prevent conflicts
- Optionally execute code from mod assemblies upon loading
- Load mods individually, bypassing load order restrictions

A more detailed explanation of all features along with instructions on usage is available on the [wiki](https://github.com/Carnagion/Modot/wiki).

# Installation

**Modot** is available as a [NuGet package](https://www.nuget.org/packages/Modot).  
Simply include the following lines in a Godot project's `.csproj` file (either by editing the file manually or letting an IDE install the package):
```xml
<ItemGroup>
    <PackageReference Include="Modot" Version="1.0.0"/>
</ItemGroup>
 ```

Due to [a bug](https://github.com/godotengine/godot/issues/42271) in Godot, the following lines will also need to be included in the `.csproj` file to properly compile along with NuGet packages:
```xml
<PropertyGroup>
    <CopyLocalLockFileAssemblies>true</CopyLocalLockFileAssemblies>
</PropertyGroup>
```

# Security

**Modot** includes the ability to execute code from C# assemblies (`.dll` files) at runtime.  
While this feature is immensely useful and opens up a plethora of possibilities for modding, it also comes with the risk of executing potentially malicious code.

This is unfortunately an issue that has no easy solution, as it is fairly difficult to accurately detect whether an assembly contains harmful code.

As such, it is important to note that **Modot does not bear the responsibility of checking for potentially malicious code in a mod's assembly**.

However, it does provide the option to ignore a mod's assemblies, preventing any code from being executed.  
Along with the ability to load mods individually, this can be used to ensure that only trusted mods can execute their code.

Another way to prevent executing malicious code is by restricting the source of mods to websites that thoroughly scan and verify uploaded user content, such as [Steam](https://store.steampowered.com). As mentioned earlier though, **it is not Modot's responsibility to implement such checks**.