# Setup from source

1. Run the __AssemblySetup__ project to initialize the paths needed for the patch project
   * You may need to Unload/Reload the __Assembly-CSharp.HexiDave.mm__ project for it to take effect
2. Run the __Patcher__ project to actually apply the patch to the RoR2 assembly

# Patching

There are a few options for patching:

1. __Run *Patcher.exe* directly__: this will attempt to use a local patch file, and if missing download the latest pack from GitHub.
2. __Drop a Zip file onto *Patcher.exe*__: this will unzip the patch and apply it.
3. __Drop a patch file onto *Patcher.exe*__: this will apply the patch directly.

# Uninstalling the patch

__TODO__
