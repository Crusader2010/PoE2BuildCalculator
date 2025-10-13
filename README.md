**Project goal:** providing a way to compute the best item combinations, from a provided list, that match user defined validations and weighted tiers for the item stats.

**Steps:**
1. Manually create a list (.txt file) with items from Path of Exile 2. I recommend using the browser extension [**PoE2 Trade Item Exporter**](https://github.com/intGus/poe-trade2text) on the [official PoE2 trading website](https://www.pathofexile.com/trade2/search/poe2).
2. Simply copy-paste each item's details (using the "**Export**" button provided by the extension), one after the other.
3. Adjust certain item names if you want to flag them for later.
   Adding "(MINE)" or "(POTENTIAL)" on the row with the item's name will flag some computed stats on the item, that you can later use in creating scoring tiers (e.g. if you rate an item that has "potential" very highly, you'd want to set the tier weight of the flag very high).
   		Example: Rapture Fend (MINE) 
    			 Glowering Crest Shield
4. Within the program, choose the file, open it, then parse it.
5. Create your validation requirements (e.g. the sum of all items' Chaos Resistance must be greater than 45).
   - If you'd like to test your computer, don't create a validation function (this means every combination is viable) and press the "Benchmark" button.
   - The Benchmark will estimate how much time it takes for your computer to compute all possible (and valid) combinations.
   - The sample file provided results in more than 2 billion combinations. On my PC it takes around 3 minutes to compute them all.
7. Create weighted tiers for item stats.
   Example:
   	- suppose you care about Maximum Amount of Mana  and  Cast Speed => e.g.: this will be "tier 1". Within this "tier 1", you value "Cast Speed" higher than "Max Amount of Mana" => set "Cast Speed"'s weight to 60% and "Max Amount of Mana"'s to 40%.
	- suppose you also care about Spell Damage% => e.g.: this will be "tier 2". Within this "tier 2", "Spell Damage%"'s weight will be 100% since it's the only one.
 	- suppose you care more about "tier 1" than "tier 2" => set "tier 1"'s weight to 65% and "tier 2"'s weight to 35%.
8. Choose how many item combinations you want to retrieve (current memory storage hard limit is 10 million combinations but needs to be made configurable). (**WIP**)
9. Compute and display the item combinations and score for each (based on the tiers you've set before). (**WIP**)

**Sample PoE 2 items file:** [poe 2 items v2.txt](https://github.com/user-attachments/files/22846810/poe.2.items.v2.txt)

(**Other WIP**): 
1. Expanded validation logic, allowing you to set more types of validations. Since a combination is a list of items (having, if initially provided, 2 rings and 1 item of each other class), we can have any of:
   E.g.: **At least/most/exactly N/N% of items in a combination** satisfy the final evaluation expression (which makes that item combination valid).
   - Currently, we evaluate the subexpression of each group (based on the chosen stat operators), then successively left-apply the group-level operators (AND,OR,XOR), and end up with some binary arithmetics that result in a true/false validation.
   - The group evaluation simply sums the inner stats' expression over the list of items. E.g.: suppose group_1 is {Chaos Resistance - Cold Resistance between 40 and 90} => the final validator function will sum (Chaos Resistance - Cold Resistance) over the list of
     items in each combination and check if it's between the two values; which combinations pass this test are marked as valid.
     - Here, we can add more choices for the group evaluation: at least/most/exactly N/N% of items in any combination pass the group validation (e.g. have (Chaos Resistance - Cold Resistance) betweem 40 and 90). Currently, there is no matching logic for these; it also depends how you interpret the "at least 100% of items" choice - either by taking all items as a whole (the SUM), or by considering each item individually. We'll consider the latter. Thus we also need to keep the existing SUM(all items) as a choice. The new choices need to be added at group level.
     - It might also seem necessary to add a "NOT" toggle for groups. When combined with the previous choices, this negation can be either applied to the "between X and Y" final group value condition, and/or to the "at least/at most N/N% items". Since the latter will be chosen at group level (thus allowing "at most 0% of items <=> None" between X and Y <=> if 1+ items have that value between X and Y we fail the check), there is really no need to add the negation toggle (i.e. NOT(SUM() <=> all items as a whole have a value between X and Y) is the same as "at most 0/0% of items <=> None" have the value between X and Y ).
     - Likely we just need a good help dialog to inform users of these fancy conditions (Note: between implies inclusive):
	     - Any between X and Y <=> At least one between X and Y
	     - All(each) between X and Y <=> At least 100% between X and Y
       	 - None between X and Y <=> At most 0/0% between X and Y
	     - SUM(all) between X and Y => **default choice**
	     - Any NOT between X and Y <=> At least one <= X-1 OR At least one >= Y+1
	     - All(each) NOT between X and Y <=> Each item is either <= X-1 OR >= Y+1
         - None NOT between X and Y <=> At least 100% between X and Y
	     - SUM(all) NOT between X and Y <=> SUM(all) <= X-1 OR SUM(all) >= Y+1
	     - NOT Any NOT between X and Y <=> At least 100% between X and Y
	     - NOT All(each) NOT between X and Y <=> At least one between X and Y 
     
	- I'm also thinking of an UI redesign for this, to more easily apply group operators and "move" groups around: a left column with all created groups and group operators, and comboboxes to choose each group (such that you can easily swap around); the right side of the window would be kept for creating groups with distinct names, which can then be identified on the left side. This can further be expanded by allowing custom binary operations to be created and applied to the same group more than once (e.g. (((group1 AND !group2) OR group3) OR !group1) ).
2. Parsing enchant mods (and other similar ones) when computing the item lists, for certain stats (i.e. also account for the "+18% chaos resistance" enchant for body armour and add it to any existing explicit mod).
3. Improving the UX for some of the buttons on the validation groups.
4. Add all of the missing item stats in the game and the related stuff. Also add the Quality modifier, likely as strictly informative.
5. Implement proper management of hybrid (abyssal?) modifiers if they are different to the normal mods (e.g. "+X% to lightning and chaos resistance" => needs to be considered for each of the mentioned resistances).
6. Consider adding special item stats for pseudo mods (probably not needed since you can SUM() over certain stats, and seems basically the same thing, but analysis is required).
7. Implement flags and tier/validation logic for different item stat types (rune, fractured, desecrated, etc).
8. Differentiate item classes per character item slots (i.e. you can only have one weapon and one shield, or two weapons of the same class if dual wielding, etc).
    - Implement the restrictions from https://www.poe2wiki.net/wiki/Dual_wielding and/or give the user a choice of what to use in the main hand and off hand. Then compute combinations based on this.
    - Extend previous point to give users a rag doll with configurable item types for each, if they so choose, otherwise fall back to the wiki restrictions where applicable (and considering all items otherwise).
9. Implement a "Settings" form where users can change (and save in a config file) various data, likely as JSON, such as:
   - Max number of combinations to store in memory
   - Toggle to save configured tiers
   - Toggle to save rag doll configuration for item types
10. Editing the initial item list within the program itself (add/remove items, adding custom flags, etc).
11. UI redesign.
12. Extensive testing.
