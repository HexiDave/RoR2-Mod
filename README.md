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

# Changes from the base game

> __Warning:__ these changes reduce the difficulty of the game and are intended for my friends and I to get the most out of the game. In future versions, these will be configurable.

* Added Artifact selection - _warning_: they're not complete, so use with caution
* Added a 5% chance for chests to drop a duplicate item
* Added the Bandit class
* Added the ability to sprint in any direction
* Added a passive ability to Artificer to buff "1-3% per item" <sup>1</sup>
* Added awarding 1 Lunar Coin on defeating the stage boss fight

* Removed the cooldown on the Commando's roll
* Removed most friendly-fire effects on Lunar items - nobody picks them up, otherwise.
* Removed fall damage

* Reduced the damage/duration of Fire and Helfire on players by 50%/20% and 70%/50% respectively. These will be adjustable in the next version.
* Doubled the base damage on Commando's pistol attack
* Changed the Artificer's wall to be a sprint/freeze attack <sup>1</sup>
* Changed the Artificer's main attack to be continuous
* Unlocked all items
* Increased max player count to 16
* Enabled the rule to allow money to pass between rounds
* Changed the rule to give 100 base money
* Changed Shrine of Order to allow up to 10 purchases
* Increased the one-hit protection to 40% (from 90%) of max health (i.e. no more than 40% of your HP taken in one attack)
* Reduced Merc dash time to 3s and allowed continued sprinting after dashing  

# Credits

__TODO__
1. Need to find where these changes were from - bear with me