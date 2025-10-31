**Dependencies:** Download .NET 8 Desktop Runtimes (**x64 required**) from https://dotnet.microsoft.com/en-us/download/dotnet/8.0
 
**Project goal:** providing a way to compute the best item combinations, from a provided list, that match user defined validations and weighted tiers for the item stats.

**Steps:**
1. Manually create a list (.txt file) with items from Path of Exile 2. I recommend using the browser extension "**PoE2 Trade Item Exporter**" on the ([official PoE2 trading website](https://www.pathofexile.com/trade2/search/poe2)).
2. Simply copy-paste each item's details (using the "**Export**" button provided by the extension), one after the other.
3. Adjust certain item names if you want to flag them for later.
   Adding "(MINE)" or "(POTENTIAL)" on the row with the item's name will flag some computed stats on the item, that you can later use in creating scoring tiers (e.g. if you rate an item that has "potential" very highly, you'd want to set the tier weight of the flag very high).
   		Example: Rapture Fend (MINE) 
    			 Glowering Crest Shield
4. Within the program, choose the file, open it, then parse it.
5. Create your validation requirements (e.g. the sum of all items' Chaos Resistance must be greater than 45).
6. Create weighted tiers for item stats. These are **independent** from the validations created before, and are used to score each item combination that passes the validations.
   Example:
   	- suppose you care about Maximum Amount of Mana  and  Cast Speed => e.g.: this will be "tier 1". Within this "tier 1", you value "Cast Speed" higher than "Max Amount of Mana" => set "Cast Speed"'s weight to 60% and "Max Amount of Mana"'s to 40%.
	- suppose you also care about Spell Damage% => e.g.: this will be "tier 2". Within this "tier 2", "Spell Damage%"'s weight will be 100% since it's the only one.
 	- suppose you care more about "tier 1" than "tier 2" => set "tier 1"'s weight to 65% and "tier 2"'s weight to 35%.
7. Choose how many item combinations you want to retrieve. Current memory storage hard limit is 10 million for untiered combinations. Tiered combinations can be set in the UI (default = first 100).
8. Compute and display the item combinations and score for each (based on the tiers you've set before).

**File samples - modify at will:**
- Sample PoE2 items file 1 - large number of possible combinations: [poe 2 items v2.txt](https://github.com/user-attachments/files/22846810/poe.2.items.v2.txt)
- Sample PoE2 items file 2 - small number of possible combinations: [poe 2 items.txt](https://github.com/user-attachments/files/23246634/poe.2.items.txt)
- Sample json config file to load in the program: [test config.json](https://github.com/user-attachments/files/23246635/test.config.json)

(**WIP:**): 
1. Parsing enchant mods (and other similar ones) when computing the item lists, for certain stats (i.e. also account for the "+18% chaos resistance" enchant for body armour and add it to any existing explicit mod).
2. Add all of the missing item stats in the game and the related stuff. Maybe add the Quality modifier (as strictly informative).
3. Implement proper management of hybrid (abyssal?) modifiers if they are different to the normal mods (e.g. "+X% to lightning and chaos resistance" => needs to be considered for each of the mentioned resistances).
4. Consider adding special item stats for pseudo mods (probably not needed since you can SUM() over certain stats, and seems basically the same thing, but analysis is required).
5. Differentiate item classes per character item slots (i.e. you can only have one weapon and one shield, or two weapons of the same class if dual wielding, etc).
    - Implement the restrictions from https://www.poe2wiki.net/wiki/Dual_wielding and/or give the user a choice of what to use in the main hand and off hand. Then compute combinations based on this.
    - Extend previous point to give users a rag doll with configurable item types for each, if they so choose, otherwise fall back to the wiki restrictions where applicable (and considering all items otherwise).
6. Implement a "Settings" form where users can change (and save in a config file) various parameters, such as: max number of combinations to store in memory.
7. Editing the initial item list within the program itself (add/remove items, adding custom flags, etc).
8. UI redesign to improve the general flow (if possible).
9. Better flow for saving and loading user configurations. 
10. Extensive testing.

(**WIP dependent on Path of Exile 2 Trade Item Exporter browser extension:**): 
1. Implement flags and tier/validation logic for different item stat types (rune, fractured, desecrated, etc).
2. Make use of the number of sockets (as an item stat).
