# GB-BP-EDIT

Tool for editing blueprints for game Ground Branch. Originally made by ModernLancer. Forked by me and remade into CLI only.

This repository contains UAssetAPI.dll which is distributed under MIT license.

## Build
Requirements:
- dotnet 8
- UAssetAPI (included in repository)

Inside PowerShell proceed into project's directory and run: 

`dotnet publish -o .\`

This will build self-contained executable in the same directory as the code.

## Usage
`.\GB-BP-EDIT.exe A:\bsolute\path\to\blueprint.uasset+<command>`

For the command part open the blueprint of interest in UAssetGUI and assemble your command according to the image bellow (borrowed from original GUI tool's page on NexusMods).

![image](https://staticdelivery.nexusmods.com/mods/4269/images/318/318-1680467545-1621854329.png)

`-b <BackupLocation>` is optional flag that allows you to backup blueprint to desired location right before editing. This will backup 3 files with extensions `.uasset`, `.uexp`, `.uasset.uexp`. Will throw error on attempt to overwrite existing backups.

Example usage:

`.\GB-BP-EDIT.exe -b .\ .\Extracted\RailAttachment\OffsetRail\BP_OffsetRailAdapter.uasset++GBRail_GEN_VARIABLE+2+RelativeLocation+0+0+1337`

## Posibility for automation of mod installation

In theory, using [UnrealPak](https://github.com/allcoolthingsatoneplace/UnrealPakTool) the following can be done to completely spare your mod's enduser from manually editing blueprints.

```
# Unpack blueprints related to rail offset adapter
.\UnrealPak.exe.lnk ..\pakchunk100-[Canted_Aim]_P.pak -Extract ..\ -Filter=*BP_OffsetRailAdapter*

# Edit blueprint, no backup since tool produces backups in corrupted state
.\GB-BP-EDIT.exe .\RailAttachment\OffsetRail\BP_OffsetRailAdapter.uasset+GBRail_GEN_VARIABLE+2+RelativeLocation+0+0+0

# Packing it back up
.\UnrealPak.exe.lnk ..\pakchunk100-[Canted_Aim]_P.pak -Repak -Output=.\new.pak
```

In theory, because the final step results in actually common [error](https://github.com/allcoolthingsatoneplace/UnrealPakTool/issues/18). But who knows, maybe "It works on your machine"(tm).

## Reporting bugs

DM `niki.so` on Discord.
