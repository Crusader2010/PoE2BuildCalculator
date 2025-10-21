﻿**Project goal:** providing a way to compute the best item combinations, from a provided list, that match user defined validations and weighted tiers for the item stats.

**Steps:**
1. Manually create a list (.txt file) with items from Path of Exile 2. I recommend using the browser extension "**PoE2 Trade Item Exporter**" on the ([official PoE2 trading website](https://www.pathofexile.com/trade2/search/poe2)).
2. Simply copy-paste each item's details (using the "**Export**" button provided by the extension), one after the other.
3. Adjust certain item names if you want to flag them for later.
   Adding "(MINE)" or "(POTENTIAL)" on the row with the item's name will flag some computed stats on the item, that you can later use in creating scoring tiers (e.g. if you rate an item that has "potential" very highly, you'd want to set the tier weight of the flag very high).
   		Example: Rapture Fend (MINE) 
    			 Glowering Crest Shield
4. Within the program, choose the file, open it, then parse it.
5. Create your validation requirements (e.g. the sum of all items' Chaos Resistance must be greater than 45).
6. Create weighted tiers for item stats.
   Example:
   	- suppose you care about Maximum Amount of Mana  and  Cast Speed => e.g.: this will be "tier 1". Within this "tier 1", you value "Cast Speed" higher than "Max Amount of Mana" => set "Cast Speed"'s weight to 60% and "Max Amount of Mana"'s to 40%.
	- suppose you also care about Spell Damage% => e.g.: this will be "tier 2". Within this "tier 2", "Spell Damage%"'s weight will be 100% since it's the only one.
 	- suppose you care more about "tier 1" than "tier 2" => set "tier 1"'s weight to 65% and "tier 2"'s weight to 35%.
7. Choose how many item combinations you want to retrieve (current memory storage hard limit is 10 million combinations but needs to be made configurable). (**WIP**)
8. Compute and display the item combinations and score for each (based on the tiers you've set before). (**WIP**)

**Sample PoE 2 items file:** [poe 2 items v2.txt](https://github.com/user-attachments/files/22846810/poe.2.items.v2.txt)

(**Other WIP**): 
1. Even better validation logic, allowing to set more types of validations, other than SUM() between X and Y. Since a combination is a list of items (if initially provided, 2 rings and 1 item of each other class), we can have any of:
   E.g.: **At least/most/exactly N/N% of items in at least/most/exactly K/K% of combinations** satisfy the final evaluation expression         

2. Differentiate item classes per character item slots (i.e. you can only have one weapon and one shield, or two weapons of the same class if dual wielding, etc).
    - Implement the restrictions from https://www.poe2wiki.net/wiki/Dual_wielding and/or give the user a choice of what to use in the main hand and off hand. Then compute combinations based on this.
    - Extend previous point to give users a rag doll with configurable item types for each, if they so choose, otherwise fall back to the wiki restrictions where applicable (and considering all items otherwise).
3. Implement a "Settings" form where users can change (and save in a config file) various parameters, such as:
   - Max number of combinations to store in memory
   - Toggle to save configured tiers
   - Toggle to save rag doll configuration for item types
4. Editing the initial item list within the program itself (add/remove items, adding custom flags, etc).
5. UI redesign.
6. Extensive testing.
