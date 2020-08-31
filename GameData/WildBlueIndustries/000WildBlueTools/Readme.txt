Wild Blue Tools

A KSP mod that provides common functionality for mods by Wild Blue Industries.

---INSTALLATION---

Copy the contents of the mod's GameData directory into your GameData folder.

1.81.3
- Raptium balance changes.

1.81.2
- Fix WBIOmniConverter yield resources being produced when required conditions weren't met.
- Fix WBIOmniConverter yield resources being produced immediately after previously unmet requirements were met.
- Fix WBIOmniConverter yield results not showing all the cycle results after player leaves the vessel for a length of time and returns.
- Fix background converters not respecting locked resources.
- Background processors no longer consume ElectricCharge for simplicity; it's either that or bog the game down with finding and running power generators.

1.81.1
- Updated OmniConverter part info.

1.80
- WBIOmniStorage now supports its own set of MAX_RESOURCE_MULTIPLIER nodes. If defined within the part module, and a global MAX_RESOURCE_MULTIPLIER has the same resource, then the part module's MAX_RESOURCE_MULTIPLIER definition will be used instead.

1.79
- WBIOmniStorage now supports its own set of OMNIRESOURCECOMBO nodes. If a combo is defined within the part module and a combo with the same resources is defined as a global OMNIRESOURCECOMBO, then the part's OMNIRESOURCECOMBO will be used.

1.78
- Bug fixes

1.77
OmniConverter Templates
- Templates can now specify a minimumCrew as a requirement to run. The part must have a crew equal to or greater than the specified minimum in order to run. If the template also specifies an ExperienceEffect then each minimum crew member needs the ExperienceEffect.
- Templates can now list requiresCommNet as a requirement to run.
- Templates can now list one of: requiresSplashed, requiresSubmerged, or requiresOrbiting as requirements to run.
- Templates can now produce Science as a YIELD_RESOURCE if in career or science sandbox mode.

New OmniConverter templates
- Lab Time - produces LabTime. It requires that the part have a minimum crew of 2.
- Science! - produces Science as a yield resource. It requires that the part has a minimum crew of 2 and requires a CommNet connection back to the home world.

1.76.1
- More bug fixes

1.75
- Bug fixes

1.74
- Updated to KSP 1.8

1.73.5
- Fix drill NREs

1.73.4
- Fix for drills breaking.
- New OmniConverter templates for when Air and Stress are enabled from Snacks.

1.73.3
- Updated Snacks support.

1.73.2
- Fixed crew requirements for experiment labs.

1.73.1
- Removed part restrictions on several experiments.

1.73.0
- Updated to KSP 1.7

Jade's awesome updates:
- Nearly all decal files contained in WBT and no longer hosted in the other WBI mods. All 80+ total decals have been redrawn from scratch.
- Support for TAC LS provided in the decals. Templates not included.
- Resource templates updated descriptions to give them personality and make their roles clear.
- Omniconverters: added MonoPropellant power (MPU) option.
- Omniconverters: added Nitronite options.
- Omniconverters: re-balanced to appear in appropriate tech nodes (basic science, science tech row, fusion power...)
- Omniconverters: re-balanced for better power production rates and more opportunities to acquire Slag for base-building, and for power producers to not require kerbal skill.
- Make OmniStorage available in Lite Blue play mode. Recently some players have been trying to get OmniStorage available without Classic Stock around it.

1.72.1
- Fixed editor issue with OmniStorage.

1.72.0
- Fixed overchange of Equipment costs for OmniConverter templates.
- Fixed incorrect display of Equipment costs in the Operations Manager.
- Fixed duplicate OmniConverter and OmniStorage loadouts that happen when you revert a flight back to the editor and create a new part.
- Added ARP icons- thanks JadeOfMaar!

1.71.0
- Fixed duplicate Extraplanetary Launchpad OmniConverter templates.
- Fixed ability to reconfigure deflated parts.
- Misc. Bug fixes
- Moved Pathfinder's GoldStrike to Wild Blue Tools.
- You can now prospect at any time, but some resources won't be found unless you travel a certain minimum distance (hint: 3km is typical).
- Sample GOLDSTRIKE config node:
GOLDSTRIKE
{
	resourceName = Aurum

	//Restricts where the resource can be found. Separate multiple types by semicolon. Options: Planetary;Oceanic;Atmospheric;Exospheric. Planetary is default.
	resourceTypes = Planetary

	//List of planet names, separated by semicolon, where the resource can be found. Default: Any, which means any planet can have the resource.
	planetsFound = Any

	//List of biome names, separated by semicolon, where the resource can be found. Default: Any, which means any biome can have the resource.
	biomesFound = Any

	//The minimum distance in kilometers between prospect attempts required in order to include the resource among the possible prospected resources.
	//Default is 3 kilometers.
	//Ex: Aurum, Gemstones, and PreciousMetals all require you to travel at least 3 kilometers before they become available. Assume that Blutonium could be found every 5km.
	//If you only travel 2km before making a prospect attempt, then none of the resources will be found. If you travel 6km, then Aurum, Gemstones, PreciousMetals, and Blutonium
	//will all be potential candidates for prospecting. You still need to make a successful prospecting check to obtain a node.
	minProspectDistance = 3

	//Minimum required altitude in order for the resource to be a prospecting candidate. Can be negative for below sea level. Default is a double.MinValue.
	//minAltitude = 0

	//Maxiumum required altitude in order for the resource to be a prospecting candidate. Can be negative for below sea level. Default is double.MaxValue.
	//maxAltitude = -1000

	//If near an anomaly, some resources are more likely to occur than others. This parameter indicates how likely the resource will be found near an anomaly.
	//anomalyChance = 65

	//Minimum units in the lode
	minAmount = 200

	//Maximum units in the lode
	maxAmount = 4000

	//Roll well enough, and multiply the lode size by this multiplier
	motherlodeMultiplier = 1.5
}

1.70.0
- Bug fixes

1.7
- Recompiled for KSP 1.6
- Bug fixes.

1.61.1
- Experiments can now require solar orbit to run properly.
- Removed duplicate Play Mode patches.

1.61
- Fixed issues with resetting and rerunning science experiments that still claimed to be completed.
- Fixed issue with science experiments requiring twice as many resources to complete when checkPartResources is set to true.
- The Breakthrough Research experiment will (at long last!) unlock unresearched tech nodes in the event of a breakthrough.
- WBIOmniConverter will stop processing if the vessel's ElectricCharge falls below minimumVesselPercentEC. Default is 5.
- Consolidated the functionality of WBIEnhancedExperiment with WBIModuleScienceExperiment.
- Removed WBIEnhancedExperiment.
- Added new ElectricCharge + Rock = Konkrete OmniConverter to Classic Stock. This is similar to the real-world LavaHive regolith melter.
- Omni converters will automatically shut down if the vessel's Electric Charge falls below 5%.
- Fixed some issues related to the Operations Manager not showing up properly.
- Fixed emitters on the Buckboards producing smoke when they shouldn't.
- Fixed issue where assembling parts with resource costs didn't actually spend the required resources.
- Fixed issue with heavy parts causing physics and collider problems during assembly.

1.60
Last release for KSP 1.4.5!

WBIOmniStorage
- You can now specify a maxAmountMultiplier on individual resources within an OMNIRESOURCECOMBO. Just add the field to a RESOURCE node. It defaults to 1.0. This field will take the final amount of units that a part can hold for the given combo resource and multiply it by maxAmountMultiplier.
- You now have the ability to decouple resources within a resource combo and adjust their amounts individually.

WBIOmniConverter
- Converters can now produce one or more resources that are specified by a YIELD_RESOURCE node after a set amount of time has passed. Check out the Greenhouse converter template in Templates/ClassicStock/OmniConverters.cfg for an example.
- Converters have the option to require a die roll to determine if the resource yield succeeds, fails, critically succeeds, or critically fails. This only applies to resources defined by YIELD_RESOURCE nodes. To enable, set minimumSuccess, criticalSuccess, and criticalFailure to values greater than 0 on an OMNICONVERTER template.
- Converters can now support background processing (They run even when the vessel is unloaded). USE SPARINGLY! Too much background processing will slow the game down. For performance reasons, converters are run in the background once every six in-game hours. To enable, set enableBackgroundProcessing = true in an OMNICONVERTER template node.
- Converters now support effects. You can specify a startEffect, stopEffect, and runningEffect.

PROTIP: If you don't specify any templateNodes, then you can still set up an OmniConverter to work like a regular ModuleResourceConverter, but with the extra benefits of playing Effects, timed resource production with optional success checks, and background processing (yes, you can set it at the part module's config node if you don't use omni converter templates).

Classic Stock
- Added new Greenhouse OmniCoverter template. It's only available if Snacks is installed. It will produce Snacks after 180 hours. There's a chance that the yield will be higher than normal, lower than normal, or fail completely. It also runs in the background.
- The Haber Process, Composter, Snack Grinder, and Organic Chips Omni Converters will now convert their resources even when the vessel is unloaded and out of physics range.

Bug fixes
- Fixed issue where switching Play Modes would cause some files to not be renamed and cause all kinds of fun for players...

1.59
WBIOmniStorage
- Added new MAX_AMOUNT_MODIFIER node that can be used to increase the storage capacity of a particular resource. See the new OmniStorageModifiers.cfg file located in Templates/Common for details.

Classic Stock
- Fixed issue with OmniConverters incorrectly displaying "missing" status after changing the recipie.
- OmniConverters will now properly prepare their conversion recipie after you change what they convert.
- Fixed OPAL Processor not being able to produce Water when the part lacks the Water resource.

Bug Fixes & Enhancements
- The Ops Manager will again update its button tabs when you change configurations.

1.58

Omni Converters & Storage
- Added search functions to OmniStorage and OmniConverter GUI.
- Play Mode now lists which mods support a particular mode.

Classic Stock
- Added new Classic Stock omni converters: Propellium Distiller, Oxium Distiller, Snack Grinder (requres Snacks), Soil Dehydrator (requires Snacks).

1.57.5
- Fixed an issue where drills would generate an NRE upon startup when BARIS isn't installed.

1.57.4
- Fixed situation where the geology lab could generate a NullReferenceException while performing a biome analysis and the local biome hasn't been unlocked.
- Fixed an issue where drills would generate an NRE upon startup when BARIS isn't installed.
- Fixed a situation where drills don't realize that the biome has been unlocked.

1.57.3
- Fixes Play Mode failing to rename certain files. NOTE: You might need to reset your current play mode. Simply open the WBT app from the Space Center, choose another mode, press OK, and again open the app, selecting your original play mode. Then be sure to restart KSP.

1.57.2
- Fixes issue in WBIModuleResourceHarvester where, if you don't specify any harvest types with harvestType, the harvester will default back to its singular HarvesterType field.
- Fix for OmniStorage resources on the restricted list not being added during flight.

1.57
- On a first-time install of Wild Blue Tools, if you have Community Resource Pack installed then the play mode will default to CRP.
- Fixed resource collection issues with the WBIModuleResourceHarvester.
- Ops Manager won't show converter controls in the VAB/SPH.
- Some OmniConverter templates are now available outside of Classic Stock play mode.

1.56.0
- ARP icons updates courtesy of JadeOfMaar
- Updated OmniCoverters and OmniStorage courtesy of JadeOfMaar
- Duplicate convertible storage items fixed courtesy of JadeOfMaar
- Vessel can now handle multiple diving computers on the same vessel.
- New WBIModuleResourceHarvester can harvest all the resources in the biome, and can support multiple harvest types.

1.55.12
- Bug fixes

1.55.11
- Bug fixes

1.55.9
- Un-broke the template managers.

1.55.7
- Recompiled for KSP 1.4.4

1.55.6
- Fixes issue where resources were consumed when trying to reconfigure a disassembled module.

1.55.5
- Fixes to OmniStorage.

1.55.4
- Bug fixes

1.55.3
- Bug fixes

1.55.2
- Bug fixes
- Classic Stock templates update - thanks JadeOfMaar! :)

1.55.1
- Bug fixes

1.55
- Improved resource summary in the geology lab.
- Fixed NRE issues with the WBIProspector.
- Classic Stock play mode now provides a new storage template: Omni Storage. This template lets you add and configure any number of resources up to the container's maximum storage volume. The part module that handles the capability is the new WBIOmniStorage.
- Added the new WBIOmniConverter. It lets you configure individual converters from a set of converter templates. There are a number of pre-defined templates for Classic Stock play mode.
- Adjusted Classic Stock resource densities to reflect the 5-liter standard used by most stock resources.
- Adjusted Classic Stock storage capacities to reflect the 5-liter standard used by most stock resources. These changes will affect new parts and when you reconfigure an existing part.
- Classic Stock is now the default Play Mode for new installs of WBI mods. Existing games are unchanged.

1.50
- Recompiled for KSP 1.4.1
- Gave ElectroPlasma a small amount of density.
- Fixed an edge case where WBIProspector would generate an NRE.
- WBIModuleScienceExperiment can now check for proximity to an anomaly as a requirement.
- New experiment result: WBIUnlockTechResult - You can use this experiment result to flag one or more parts as experimental. Just like with a part test contract, an experimental part can be used even if its tech node is unlocked.
- Added new Breakthrough Research contract. It makes use of the WBIUnlockTechResult module described above.
- New WBIModuleAsteroidResource allows you to guarantee that a resource will be available if the asteroid is a magic boulder.
- New WBIToolTipManager can read PART_TIP config nodes and provide tips to players. It's an alternate way to teach players how to use specific parts and handy for those who don't read the KSPedia, part descriptions, or wiki pages.
- Many updates for Classic Stock play mode - thanks JadeOfMarr! :)

Refinery

The Refinery is an new app available at the space center and in flight. It lets you produce and/or store resources in limited amounts. Such resources could be rare and not readily available for purchase. In fact, if you try to launch a craft with Refinery resources, those resources will be cleared before pre-launch. Once you launch the craft, you can purchase the Refinery resource, and when you recover the craft, its Refinery resources will be stored in the Refinery up to its maximum capacity.

Refinery nodes specify resources that are produced and/or stored in the Refinery.
You can specify one or more Refinery nodes with the same resource. Each node will become a production tier and appear in the order that you list them in the config file. Below is an example.

//New tiers appear in the same order as these nodes appear in the config file.
REFINERY
{
	resourceName = Graviolium

	//Optional tech node required to unlock the tier.
	techRequired = wbiSaucerTechnologies

	//How many units per day of the resource to produce
	//Set to 0 if you want to just store the resource and not produce it.
	unitsPerDay = 10

	//Multiplied by the resource's unit cost to determine the cost to produce a unit of the resource.
	unitCostMultiplier = 1.75

	//Maximum number of units that can be stored at the Refinery.
	maxAmount = 50000

	//The cost to unlock the production tier
	unlockCost = 250000
}

1.40
- Streamlined the WBIModuleResourceConverter
- WBIProspector now supports one or more harvest types.
- WBIProspector can now prospect resources from the atmosphere, exosphere, ocean, and planet.

1.31.2
- Bug Fixes

1.39.1
- Cruise Control fix for single-mode engines. They still need ModuleEnginesFX though.
- Fix for deprecated parts.
- ModuleManager update.

1.39

New Props
- Disco Ball: With spinning lights!
- Dance Floor: Animated dance floor!
- Jukebox: Actually plays music! Music files must be in the .ogg format and go in the 000WildBlueTools/Music folder.

ARP Icons
Added new Alternate Resource Panel icons courtesy of JadeOfMaar. These look great! :)

Bug Fixes And Enhancements
- Fixed missing resource requirements when templates require multiple resources.
- Inflatable parts can now be inflated only once.
- Added new WBICruiseControl part module. WBICruiseControl must be placed after your engine part module and multimode switch modules. It enables you to conduct engine burns during timewarp. NOTE: Heat mechanics don't work during cruise control, this is a known limitation. To use cruise control, be sure to set the desired throttle in the context menu, and set the fuel reserve percentage. Finally turn on Cruise Control. Then engage timewarp. Note that KSP doesn't allow you to adjust the controls in the context menu during timewarp, that's a game limitation.


1.38
- Bug fixes.

1.37
- Fixed missing resource icons.

1.36
- Far Future Technologies support: NuclearSaltWater used in place of Explodium.
- The Docking port helper now supports docking ports making use of WBILight. Hence, ports light up and you can set the color on the light.

1.35
- When dumping resources, any resources that are locked won't be dumped.
- Added resource distribution to the Buckboard 6000.
- WBISelfDestruct now supports explosions when staged.

1.31
- The S.A.F.E.R. fuel supply will last up to 10.86 years of continuous output.
- Fix for parts not remembering what template they're using.
- Blutonium and NuclearFuel are now Flow Mode all vessel. Reactors that use them are still restricted to resources in the part itself.
- Added NuclearWaste resource for Classic Stock.
- The S.A.F.E.R. now produces NuclearWaste as part of its outputs. With Pathfinder installed, it can be reprocessed into NuclearFuel.

1.28
- KIS storage volumes now properly calculated.
- Code cleanup
- Play Mode selection moved from Pathfinder to Wild Blue Tools.
- Added new Classic Stock play mode.
- CRP is now a separate download.

1.26
- BARIS is now an optional download as originally intended- just took awhile for me to figure out how to make that work. DO NOT DELETE the 000ABARISBridgeDoNotDelete FOLDER! That plugin is the bridge between this mod and BARIS.
- Recalibrated templates for Cryo Engines and added cryo tank cooling to LiquidHydrogen storage.

1.25
- BARIS update. 

1.23
- NRE fixes.

1.20
- Recompiled for KSP 1.3.
- Possible click-through fix.

1.19
- Minor bug fixes

1.18
- Experiments can now check the entire vessel via their "checkVesselResources" flag, to see if there are enough resources to complete the experiment.

1.17
- Added a Tweakscale patch for the plasma screens. Thanks for the patch, Violet_Wyvern!
- S.A.F.E.R. : The Safe Affordable Fission Engine for Rovers generates ElectricCharge for your spacecraft needs. It is based upon the real-world SAFE - 400 reactor created by NASA.

1.15
- Restricted the number of contracts that are offered and/or active.
- Fixed a situation where experiments weren't registering as completed.
- Contracts won't be offered until you've orbited the target world and have unlocked the proper tech tree.

1.14
- New WBIEnhancedExperiment PartModule. It brings the enhanced experiment requirements of WBIModuleScienceExperiments to science parts that don't need to be loaded into an experiment lab.
- Science system now generates research contracts.
- Bug fixes

1.12.0
- Bug Fixes

1.11.0
- Bug Fixes

1.10.0
New Props
- Holoscreen: This prop works the same way as the internal plasma screen, but you can toggle the screen on and off.

Bug Fixes & Enhancements
- WBIGeoLab now integrates into the Operations Manager.
- You can properly configure a part to be a battery by using the ConverterSkill.
- Fixed an issue with WBIHeatRadiator not showing up in the Operations Manager.
- Fixed an issue with IVAs spawning in the editor when inflating parts.
- You can now select the default image for the Plasma Screen in addition to screens in the Screenshots folder.
- Moved the kPad and plasma screens to the Utility tab.
- The experiment lab now accounts for the science multiplier difficulty setting when generating bonus science.

1.9.0
- Added WBINameTag, WBIGroundStabilizer, and WBIGeoLab.
- Added the Buckboard 6000

1.8.10
- KSP 1.2.2 update.

1.8.9
- Greenhouse fixes.

1.8.8
- Bug fixes & enhancements.

1.8.7
- Disabled angle snap.

1.8.6
- If the target docking port supports angle snap that you can turn on/off (all WBI docking ports do), and it's turned off, then it will be turned on if the active port's angle snap is turned on.

1.8.5

WBIDockingNodeHelper
- Added ability to enable/disable angle snap, and the ability to set the snap angle.

Other
- Cleaned up some logging issues related to missing part modules and textures when supported mods aren't installed.

1.8.4
- Updated to KSP 1.2.1
- Minor bug fixes with WBILight

1.8.3
- Fixed some welding issues.
- Greenhouses won't harvest crops if you run out of resources.

1.8.1
- You can now weld ports during eva.

1.8.0
- Added WBIConvertibleMPL. Use this when you want science labs with stock Mobile Processing Lab functionality to be able to switch to a different configuration.

1.7.3
- Fixed an issue where the greenhouse would freeze the game on load.

1.7.0
Updated to KSP 1.2. Expect additional patches as KSP is fixed and mods are updated.

1.6.5
- Growth time is no longer reduced based upon experienced Scientists. Yield is still affected by experience though.
- Greenhouses now show where they're at in the growth cycle and show up in the Ops Manager.

1.6.0
- Experiments can now be created in the field by some labs. To that end, experiments have the option to specify what resources they need and how much. If not specified, then a default value will be used that's equal to the experiment mass times 10 in the default resource, or a minimum amount of the default resource, whichever is greater.
- Labs have the ability to restrict the experiments they create based upon a list of tags. Hence experiments may list a set of tags as well. If an experiment has no tags that match the tags required by the lab then it won't show up in the list of experiments that it can create.
- Experiments can now require asteroids with a minimum mass.
NOTE: Basic and DeepFreeze experiments are now located in WildBlueTools; there is no effect to MOLE users.

1.5.0
- Bug fixes and new ice cream flavors.

1.4.2
- Minor fixes to the science system.

1.4.1
- Part mass is now correctly calculated.

1.4.0
- Added animation button prop to control external animations from the IVA.
- The cabin lights button prop can now control external light animations.
- Fixed an issue where resources required by experiments wouldn't be accumulated.

1.3.13
- Added template for Uraninite.

1.3.12
- You can now change the configration on tanks with symmetrical parts. In the SPH/VAB it will happen automatically when you select a new configuration. After launch, you'll have the option to change symmetrical tanks.

1.3.11
- Added WBISelfDestruct and WBIOmniDecouple.
- If fuel tanks are arrayed symmetrically, you'll no longer be able to reconfigure them. It's either that or let the game explode (ie nothing I can do about it except prevent players from changing symmetrical tanks).

1.3.10
- Fixed an issue where the greenhouse wasn't properly calculating the crop growth time.
- Fixed an NRE with lights
- Improved rendering performance for the Operations Manager.

1.3.9
- Fixed an issue with the CryoFuels MM support to avoid duplicate templates.

1.3.8
- Fixed an issue with crew transfers not working after changing a part's crew capacity during a template switch.

1.3.7
- Fixed an issue with WBILight throwing NREs in the VAB/SPH.

1.3.6
- Minor bug fixes

1.3.5
- Fixed an issue where lab experimetns would be completed before even being started.

1.3.4
- Fixed an issue where the "Bonus Science" tab would break the operations manager in the VAB/SPH.

1.3.3 Science Overhaul

- Experiment Manifest and transfer screens now list the part they're associated with.
- Fixed an issue where experiment info wasn't showing up in the VAB/SPH.
- The Load Experiment window now appears slightly offset from the Manifest window to make it easier to distinguish that you're now loading experiments into the part.
- The Transfer Experiment button now makes it more clear that it is a transfer experiment button.
- In the VAB/SPH, the Experiment Manifest will show a new "Load Experiment" button.
- You can now run/pause individual experiments.
- Changed how experiments check for and consume resources; they now go vessel-wide.
- The Experiment Lab will no longer stop if it, say, runs out of resources or the part doesn't have enough crew.
- Improved rendering performance of the experiment windows.
- Fixed an issue where experiments wouldn't show up after reloading a craft.

1.3.2
- New props

1.3.1
- Added Local Operations Manager window to enable controlling all PartModules on all vessels within physics range that implement IOpsView. This is used by mods such as Pathfinder.
- Added Slag and Konkrete resources, templates, and icons.
- Added WBIProspector and WBIAsteroidProcessor. These PartModules can convert non-ElectricCharge resources into all the resources available in a biome/asteroid, and can produce a byproduct resource. A typical use is to convert Ore/Rock into the locally available resources and Slag.
- The Rockhound template now uses the new WBIAsteroidProcessor to convert Ore and/ore Rock.

1.3.0
- Updated to KSP 1.1.3
- Introduced IOpsView to enable command and control of parts from the Operations Manager.
- Refactored the WBIMultiConverter to use a template selector similar to the WBIConvertibleStorage.
- WBIConvertibleStorage/WBIMulticonverter will show you all the templates that a part can use, but templates that you haven't researched yet will be grayed out.

1.2.9
- Removed Dirt from the USI LifeSupport template.
- Added icons for USI-LS templates.
- Added the WBIModuleDecoupler part module that can switch between a decoupler and a separator.

1.2.8
- Fixed an issue where converter text and experiment manifest text wasn't showing up in the VAB/SPH.
- Fixed an issue where you'd see crew portraits in a nearby vessel even though you're focused upon a different vessel.
- Fixed an issue where a science experiment would be run when transferring the experiment out of a lab, even though it hasn't met all the requirements.
- Fixed an issue where a science lab could not transmit data back to KSC when RemoteTech is installed. NOTE: This is a pretty simplistic fix; future updates will account for packet transmission rates etc.

1.2.7
- Fixed issues with USI-LS.

1.2.6
- Added new props

1.2.5
- Improved GUI for selecting resources
- You can now click on the laptop prop's monitor to change the image.

1.2.4
- More Input is NULL error fixes.

1.2.3
- More Input is NULL error fixes.

1.2.2
- Fixed NREs and Input Is NULL errors.

1.2.1
- Minor bug fixes

1.2.0

Science System
- Added a new science system that lets you load experiments into the Coach containers, fly them to your stations and transfer them into a MOLE lab, and once completed, load them back into a Coach for transport back to Kerbin. The experiments have little to no transmit value, encouraging you to bring them home (or if you prefer, load them into an MPL). The new experiments can have many requirements such as orbiting specific planets at specific altitudes, various resources, minimum crew, and required parts. To give players an added challenge, you can optionally specify the percentage chance that an experiment will succeed. You even have the ability to run a specific part module once an experiment has met the prerequisites- that gives you the ability to provide custom benefits. Consult the wiki for more details.

1.1.5
- Adjusted Ore and XenonGas capacities to reflect stock resource volumes.

---LICENSE---
Some resource definitions courtesy of Community Resource Pack. License: CC-BY-NC-SA 4.0

Refinery icon by Goran tek-en License: CC-BY-NC-SA 4.0

Art Assets, including .mu, .mbm, and .dds files are copyright 2014-2016 by Michael Billard, All Rights Reserved.

Wild Blue Industries is trademarked by Michael Billard. All rights reserved.
Note that Wild Blue Industries is a ficticious entity 
created for entertainment purposes. It is in no way meant to represent a real entity.
Any similarity to a real entity is purely coincidental.

Source code copyright 2014-2016 by Michael Billard (Angel-125)

    This source code is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.  If not, see <http://www.gnu.org/licenses/>.