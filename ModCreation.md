#So you want to create a mod

###Lets take PhaseToEndlessStim as an example

The basic json looks like this

	{
		"Files":[
			{
				"SQLiteFiles":[],
				"ComparisonString":"global function OnWeaponPrimaryAttack_titanability_phase_dash",
				"ReplacedCodeFile":"mp_titanability_phase_dash.nut",
				"Directory":"mp_titanability_phase_dash",
				"AddressOffset":0
			}
		],
		"Description":"This turns phase dash into unlimited super stim that can be toggled",
		"Name":"Example.PhaseToEndlessStim",
		"Directory":"Example.PhaseToEndlessStim"
	}
	
	
Lets start from the beginning

##### Files array
This contains a list of all files, how to find the location to replace, and the relative directory of the folder

##### Description
A simple description of the mod. shows up as a tooltip in the program

##### Name
The name of the mod

##### Directory

The name of the directory.the folder in the mods folder should have the same name as the json files



### The files array type

##### SQLiteFiles
These are pointer found in cheat engine using pointerscans ability to export to sqlite. these are much faster than searching through memory. [Tutorial to pointerscanning](https://www.youtube.com/watch?v=MiCoP2MrDOU).

##### ComparisonString
This is the string we compare so we are sure that we have found the right address. This can be the start of a file but doesnt have to. Use the addressoffset if you cannot use the start of the file

##### AddressOffset
This is an optional parameter. This applies an offset of characters to the address found.

This is so that if you cannot find the start of the file reliably, you can use this to go back to the start of the file.

##### ReplacedCodeFile
This is the file that will overwrite the old one. MUST NOT BE LONGER THAN THE ORIGINAL VERSION OR WE RISK REPlACING NECCESARY MEMORY.

Current file types that can be replaced are set,gnut and nut files

##### Directory
The directory the file is located in. this is relative to the main mod directory