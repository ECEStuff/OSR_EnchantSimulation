# OSR_EnchantSimulation
This is a crafting simulator for the OldSchoolRivals Ace Online server. The original crafting simulator was created by Discord user **thepolkameister** (Discord ID: 299393499927674880). The original creator had the project under the WTFPL license.

This simulator includes the following not in the original simulator:
- Base item cost (the cost of the item being modified).
- Crafting simulation of the factory found in the game. Original simulator simulated crafting only in the laboratory, another crafting area in the game.

How to use:
- Load the simulator. The simulator (EnchantSimulation.exe) can be found in OSR_EnchantSimulation/EnchantSimulation/bin/Release
  - The project can alternatively be loaded in Visual Studio as a Windows Forms Application. Load the .sln file.
- Choose your strategy. There are several preset ones that can't be edited and a custom one that can be edited to your preferences.
- Fill in the necessary costs of each crafting ingredient (enchant card, special enchant card, booster card, etc.). Units must be consistent.
- Each level represents the enchant level. For Legend items, level 1 refers to the 1st version (S-I for Legend Armor, "Dark" for Legend Weapons).
  - Target level is the level that you want the item to be at. This is at most level 7 for Legend Armor, level 15 for Legend Weapon, and level 12 (for simulation only) for enchanting weapons/armor.
  - Click a box in each column (only one type of protection card and/or one type of booster card may be used) to use that item in the corresponding enchant.
  - For enchanting only: if the special enchant card option is ticked, then a special enchant card will be used instead of a normal enchant card.
- If the crafting is for a Legend Armor or a Legend Weapon (this would be done in the factory instead of in the laboratory in the real game), click one of those options instead. Enchanting is the default equipment type option.
  - Legend probability ranges from 0 to 1. For example, for a 90% success rate, enter 0.9.
  - Booster cards and protection cards are not usable for Legend crafting.
- Click "Simulate!" when you are ready to view results. Note: high target levels (level 7 for Legend Armor or level 12+ for enchanting and Legend Weapons) take a long time of approximately 1 minute or more.

Crafting items:
- Booster cards multiplicatively increase the success rate by a certain percentage.
- If attempting to gain an enchant level would fail, a protection card will reset the item to a certain level. Basic resets to level 1; normal resets to level 5; hyper resets to level 10 (only if the attempted level was 11 or higher).
- Bible/scripture cost refers to the cost of converting an armor or weapon to a Level 1 Legend Item. Converting items to Legend quality requires a bible (for armor) and a scripture (for weapons). We'll refer to these upgrade items as books.
- The simulator keeps track of how many crafting ingredients and base items were used. If no protection cards are used, then the simulator increments the number of base items used by 1. Otherwise, each attempted enchant level will use up 1 of each of the designated items in addition to 1 normal enchant card (or special card if selected).
- The OldSchoolRivals has another crafting system for a special set of weapons. To simulate upgrading these weapons, select the "Legend Weapon" equipment type. Then, set the "Lv1 Legend Probability" to 1 (since their lowest version is level 1 by default) and set the "Bible/Scripture Cost" to 0.

TODO:
- Add multithreading in the simulation section of the code.
- Add unit conversion and specified units to each cost. Final cost would be in the the user's chosen units, and all item costs will be converted to the same unit if the costs are in different units.
- Test for robustness.
- Rename "Bible/Scripture cost" to "Book Cost."
- Add new equipment type "Special Weapons" for OldSchoolRivals special weapons.

