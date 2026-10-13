# install dotnet:

sudo apt install dotnet-sdk-10.0

# Build

Point to Rimworld root dir and Harmony DLL from downloaded mods

dotnet build -c Release -p:GameManagedDir=/mnt/data_medium/SteamLibrary/steamapps/common/RimWorld -p:HarmonyDll=/mnt/data_medium/SteamLibrary/steamapps/workshop/content/294100/2009463077/1.7/Assemblies/0Harmony.dll
