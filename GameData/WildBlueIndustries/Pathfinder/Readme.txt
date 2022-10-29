Pathfinder

A KSP mod that blazes the trail for more permanent installations. Space camping and geoscience!

---INSTALLATION---

Copy the contents of the mod's GameData directory into your KSP's GameData folder.

---REVISION HISTORY---

1.39.1
- Removes Buffalo from the mod install. Use Buffalo 2 instead.

1.39.0
- Removed BARIS dependency dll.

1.38.0
- Sandcastle compatibility update.

1.37.1
- Recompiled for KSP 1.12.2.

1.37
- KSP 1.11 update
- Add stock inventory support to several parts.

KNOWN ISSUES
- Stack mounting the inflatable parts isn't working with the boxed parts using stock EVA construction. You'll need to surface-attach them instead.

1.36.1
- Buffalo update

1.36.0

Thanks to JadeOfMaar:
- Added EPS Transformer to Arc Reactor. This down-converts ElectroPlasma to ElectricCharge in 1:6 ratio.
- Removed resource ignore lists from (Castillo) Storage Depot so that resources added/used by LS mods can be tweakable in it.
- Updated Sunburn Fusion Lab (CRP) template to have FusionPellets production from D-He3 and from H2O.
- Updated Sunburn Fusion Lab (CRP) template's ISRU Chain (Coolant, Glykerol).

1.35.2
- Bug fixes

1.35.0
- Added rack mount node to the Chuckwagon.
- Updated packed volumes for several parts.
- Fixed texture issues on the Castillo parts.

1.34.0
- Updated to KSP 1.8

1.33.5
- Fix drill NREs.

1.33.4
- Fix for drills breaking.
- Fix for kerbal retraining.
- New OmniConverter templates for when Air and Stress are enabled from Snacks.

1.33.2
- Updated Snacks support.

1.33.1
- Fixed crew requirements for experiment labs.
- Fixed inability to reverse-thrust with propeller engines.
- Updated KAS configs for flexible docking tunnels. Thanks Igor! :)

1.33
- KSP 1.7 updates
- Decal updates courtesy of JadeOfMaar

1.32.4
- Removed specialty bonus from the Drilling Rig; it has no crew capacity
- Added WBIEfficiencyMonitor to the Drilling Rig; it will now benefit from productivity gains from efficiency improvement experiments. Don't expect miracles, these gains are small...

1.32.3
- Fixed invalid harvest types in the Prospector.
- Fixed shrunken packing box on the Drilling Rig.
- Fixed overchange of Equipment costs for OmniConverter templates.
- Fixed incorrect display of Equipment costs in the Operations Manager.
- Fixed duplicate OmniConverter and OmniStorage loadouts that happen when you revert a flight back to the editor and create a new part.
- Added KIS volume override for Golddigger.

1.32.2
- Fixed duplicate Extraplanetary Launchpad OmniConverter templates.
- Fixed portraits not updating when Tourists are retrained. NOTE: KIS might not notice the update.
- Fixed KIS-attached inflatables starting off being attached inverted. Be sure to use the KISMount attachment node (the first in the list).
- Fixed ability to reconfigure deflated parts.
- Updated stack nodes on the Patio.
- You can now prospect at any time, but some resources won't be found unless you travel a certain minimum distance (hint: 3km is typical).

1.32.1 Sandcastle Part 2

New Boxed Parts
- Walkway 2x (Advanced Exploration): This boxed part assembles into a walkway that adds space between base parts. It also has a roof rack for attaching accessories.
- Drilling Rig (Composites): Once assembled, this large drilling rig extracts a large number of resources at a time; it's 4x faster than the Claimjumper.

1.32 Sandcastle Part 1
At long last Project Sandcastle is here! Part 1 contains a new line of 12.5-meter diameter parts and associated accessories designed for more permanent bases. The first part, the Castillo, is one you're already familiar with. It has undergone many revisions to find the right look and feel. This release builds upon the Castillo by delivering a factory, storage depot, and observatory- with working telescope if you install Tarsier or CactEye! In addition, there are new support buildings including the Ground Station and A.R.C. Fusion Reactor, as well as a new walkway. All of these parts start out as standard shipping crates that are then assembled with resources- be sure to turn on resource distribution.

New Boxed Parts - These parts all start out as Buckboard-sized packing boxes and require resources to assemble.
- Castillo Depot (Composites): This is a 12.5m diameter warehouse with a 200,000-liter omni storage capacity. You can show or hide the roof to stack additional depots, and you can mesh switch between crew hatches (the default) and solid walls.
- Castillo Observatory (Composites): This 12.5m diameter part takes over the Castillo's science data generation for labs, Pipelines, and prospecting, and GoldStrike prospecting bonus. It also has a working telescope- provided that you have the Tarsier (preferred) or CactEye telescope mod installed.
- Castillo Factory (Composites): This large factory has very efficient omni converters. It takes over the efficiency improvements produced by the Castillo, and it has built-in support for OSE Workshop and EL Workshop. There's even a Christmas bonus for CRP users. NOTE: You're on your own for future CRP support, I've switched over to Classic Stock and do use CRP anymore. CRP has been legacy for some time now...
- Castillo Adapter (Composites): This boxed part is an adapter between 12.5m parts and 3.75m parts.
- Ground Station (Composites): This boxed part is designed to form part of a Deep Space Network on a local planet when you're far away from the KSC. It also generates trajectory data for the Pipeline mass driver. The Ground Station can track objects targeted by the vessel or a random target.
- A.R.C. Fusion Reactor (Specialized Electrics): Unlike the portable prototype fusion reactor found in the Hacienda's Solar Flare, this permanent installation generates a lot more ElectricCharge. Bonus: You can turn off the particle effects via the Part Action Window to improve rendering performance.
- Walkway (Advanced Exploration): This boxed part assembles into a walkway that adds space between base parts. It also has a roof rack for attaching accessories.

Revised Parts
- Remodeled the Pipeline Mass Driver to make it much easier to attach to your base.
- Remodeled the Castillo to use the new look for konkrete parts.
- The Rangeland now has underground storage for RocketParts and can recycle vessels on the pad.

Bug Fixes & Enhancements
- The Pipeline's launch azimuth is now restricted to a few degrees to the left or right of where the barrel points. You can turn this restriction off in KSC's Settings menu (accessed when you press the ESC key).

1.31.1
- Removed ThermalEfficiency and TemperatureModifier nodes from stock drills in the MM_Drills.cfg patch. For reals this time. Thanks haelon! :)
- Removed duplicate Play Mode patches.
- If you have multiple Buffalo Bulldozers on site, any bulldozer currently NOT engaged in a project will improve the build rate of the bulldozers that ARE engaged in a project.

1.31
New Part
- Accessory Mount (Space Exploration). The Accessory Mount can be radially attached and provides a hardpoint for accessories like the Gaslight, Sombrero, and Telegraph.

Bug Fixes & Enhancements
- Recompled for KSP 1.5.X.
- Updated KIS MM patches to KIS 1.16.
- Updated KAS MM patches to KAS 1.1.
- Updated OSE Workshop MM patches to OSE Workshop 1.3.
IMPORTANT NOTE: The latest KAS update is potentially save breaking to Pathfinder. It is Strongly recommended that you de-link any parts attached via pipes and any vessels connected via Mineshaft.
- Added rack mounts to the crew tunnels on the Chuckwagon, Casa, Doc, and Hacienda.
- The Gaslight, Sombrero, and Telegraph can now be surface mounted in addition to stack mounted.
- Removed ThermalEfficiency and TemperatureModifier nodes from stock drills in the MM_Drills.cfg patch. Thanks haelon! :)
- Part decals now default to hidden.
- Renamed the Hogan to the Castillo.
- Added resource distribution and Omni Converters to the Castillo.
- Consolidated the Castillo's Community Center, Dormitory, and Classroom template functionalities into a single template.
- Removed KAS attachment fixtures from a variety of parts. You'll need to use the KAS pipe connector or better yet, Pathfinder's resource distribution.
- Added OmniStorage to the ground-based Pipeline as well as a resource distributor.
- In Classic Stock Play Mode, OSE Workshop now uses Equipment instead of MaterialKits for its default resource.
- Added Classic Stock recipes to OSE Workshop.
- Added new ElectricCharge + Rock = Konkrete OmniConverter to Classic Stock. This is similar to the real-world LavaHive regolith melter.
- Omni converters will automatically shut down if the vessel's Electric Charge falls below 5%.
- Streamlined feedback messages related to using resource distribution to pull resoures needed to assemble parts.
- Fixed some issues around the boxed parts and their deployment.
- Fixed some issues related to the Operations Manager not showing up properly.
- Fixed emitters on the Buckboards producing smoke when they shouldn't.
- Fixed issue where assembling parts with resource costs didn't actually spend the required resources.
- Fixed issue with heavy parts causing physics and collider problems during assembly.

1.30.1
- Fixed duplicate Hogan

1.30
Last release for KSP 1.4.5!

Classic Stock
- Added new Greenhouse OmniCoverter template. It's only available if Snacks is installed. It will produce Snacks after 180 hours. There's a chance that the yield will be higher than normal, lower than normal, or fail completely. It also runs in the background.
- The Haber Process, Composter, Snack Grinder, and Organic Chips Omni Converters will all now convert their resources even when the vessel is unloaded and out of physics range.

Bug Fixes & Enhancements
- Fixed issue where switching Play Modes would cause some files to not be renamed and cause all kinds of fun for players...
NOTE: For the changes to take effect, you'll need to switch your Play Mode to some mode other than the current one, then switch it to the desired mode.

1.29
Hogan
- Rebuilt the part so that the center of the dome is no longer offset with respect to the center of its packing box. This will fix center of mass issues that caused the part
to flip when assembled. BE SURE TO USE RESOURCE DISTRIBUTION TO ASSEMBLE THE PART! Existing Hogans won't be affected.
- Removed KIS attachment port due to adjustments to the 3D model. Existing Hogans won't be affected.
- Added additional tool tip text.

Claimjumper
- Removed Prospector; its functionality is incoprorated into drills.
- Removed drill switcher; its functionality is incorporated into drills.
- Added list of resources mined from the current biome.
Classic Stock
- Fixed issue with OmniConverters incorrectly displaying "missing" status after changing the recipie.
- OmniConverters will now properly prepare their conversion recipie after you change what they convert.
- Fixed OPAL Processor not being able to produce Water when the part lacks the Water resource.

Bug Fixes & Enhancements
- The Ops Manager will again update its button tabs when you change configurations.

1.28
Omni Converters & Storage
- Added search functions to OmniStorage and OmniConverter GUI.
- Play Mode now lists which mods support a particular mode.

Classic Stock
- Added new Classic Stock omni converters: Propellium Distiller, Oxium Distiller, Snack Grinder (requres Snacks), Soil Dehydrator (requires Snacks).
- Changed the default template to OmniShop and OmniWorks for the Casa/Ponderosa and Hacienda, respectively.
- The Mule's default template is now OmniStorage.

1.27.4
- Fixed NRE produced by converters when BARIS isn't installed.
- WBIHoverController now handles hover state updates instead of WBIVTOLManager. This allows multiple craft within physics range of each other to independently hover.

1.27.3
- Fixes Play Mode failing to rename certain files. NOTE: You might need to reset your current play mode. Simply open the WBT app from the Space Center, choose another mode, press OK, and again open the app, selecting your original play mode. Then be sure to restart KSP.

1.27.2
- Removed MM patch adding GoldStrike drills to every resource harvester part found in the game; it was creating issues.

1.27.1
- Fix for resource collection with the Lasso.
- Prospector fixes
- Converters are unavailable in the VAB/SPH.
- Fixes related to resource abundance changes in KSP 1.4.5

1.27 Sandcastle: Bulldozing

Tired of building bases on a slanted slope? This release introduces the ability to create static regolith mounds upon which you can build your bases! These mounds are level to the ground and nice and flat. To build them, all it takes is the latest version of Kerbal Konstructs and the new Buffalo Bulldozer part.

New Part

- Buffalo Bulldozer (Genral Construction): Specially designed for the Buffalo rover system included in Pathfinder, the bulldozer won't be available unless you have Kerbal Konstructs installed. Use this part to create a small, medium, or large regolith mound. These statics take time to make! Once done, you'll be able to place the regolith mound as any other Kerbal Konstructs static.

Bug Fixes

- Fixed missing converters not showing up in the operations manager.
- Fixed operations manager click-through in the editor.
- You can now have multiple Guppy command cabs on the same vessel and they'll work with each other to control the dive.
- The Lasso's numerous intakes have been consolidated.

1.26.7
- Buffalo update
- Bug fixes for OmniConverters and experiments

1.26.6
- WildBlueTools update.
- Water Buffalo update.

1.26.5
- Recompiled for KSP 1.4.4
- Updated support for Extraplanetary Launchpads.

1.26.4
- WBT Update
- Classic Stock templates update - thanks JadeOfMaar! :)
- You now have the option to distribute shared resources with the vessel during each distribution cycle. You can enable it by setting the "Shares resources with vessel" option.

Templates
- Power Distributor: Available for the Chuckwagon and Conestoga, the Power Distributor takes ElectricCharge stored in the part and distributes it throughout the vessel. It works best with resource distribution turned on, the distributor set to consume ElectricCharge, and the "Shares resources with vessel" option set to Yes. This allows a remote power station to generate power and spread it throughout vessels and bases in the vicinity.

1.26.3
- Updated BARIS support dll.
- Gold Strike drills can now extract all resources in the biome as if they had a built-in Prospector.

1.26 Omni Converters
For Classic Stock only, Pathfinder's base components (Casa, Ponderosa, and Hacienda) can now use the new omni converters. Instead of templates like Watney, OPAL, and Nukeworks that have fixed converters, omni converters let you configure your base components however you like. Need 3 Haber processors? No problem. Need the complete resource chain to go from Ore to MaterialKits to Equipment and finally RocketParts? You can do that to. While traditional templates are still around, omni converters offer much more versatility.

In addition to the Omni Converters, the new Omni Storage template lets you store any number of resources desired up to the maximum available storage volume. Omni Storage is even smart enough to handle resource ratios like LFO; no need to fiddle with the storage capacity sliders to get just the right ratio of LiquidFuel/Oxidizer in the tank. As with Omni Converters, the traditional storage templates are still around, but Omni Storage offers much more versatility.

Bug Fixes & Enhancements
- Recompiled for KSP 1.4.3
- Fixed NRE issues with the WBIProspector.
- Classic Stock's Hot Springs has a resource scanner to help locate GeoEnergy.
- Improved resource summary in the geology lab.
- Fixed missing resources in the Classic Stock's GeologyLab.
- Fixed a situation where resource distribution that wouldn't distribute resources.
- Added new templates to Classic Stock Play Mode: OmniShop (Casa/Ponderosa), OmniLab (Doc), and OmniWorks (Hacienda). 
YOUR EXISTING CLASSIC STOCK TEMPLATES ARE SAFE! These are new templates that offer more versatile configurations, but they take more work on your part to set up.
- Added new Omni Storage template to the Classic Stock Play Mode.
- Moved the Recycler Arm to Buffalo. The existing arm has been deprecated.
- Adjusted Classic Stock resource densities to reflect the 5-liter standard used by most stock resources.
- Adjusted Classic Stock storage capacities to reflect the 5-liter standard used by most stock resources. These changes will affect new parts and when you reconfigure an existing part.
- Classic Stock is now the default Play Mode for new installs of WBI mods. Existing games are unchanged.

1.21 Gigawatts
This release focuses on resource harvesting by making quality of life improvements in the Lasso parts and adding the ability to collect exoatmospheric resources via a new part. Thanks for your help, JadeOfMaar and Rock3tman! :)

Lasso
- Cleaned up the available converters. Now there is an Atmosphere Processor to extract all available atmospheric resources from intake Atmosphere, and a Liquid Distiller to do the same for oceanic resources.
- Added a new 3.75m Lasso-300. It can extract both atmospheric and exospheric resources via its Atmosphere Processor in addition to the standard Liquid Distiller. Be sure to start the Exospheric Scoop to gather up resources just above a planet's tangible atmosphere.

Resources Definitions
- Added exospheric definitions for Atmosphere, Water, Minerite, and Nitronite. With enough time and the proper equipment, you can skim the atmosphere of worlds like Kerbin, Laythe, Duna, Eve, and Jool to make LiquidFuel and Oxidizer!

1.25
- Recompiled for KSP 1.4.1

1.20
- WBT update
- Module Manager update

1.19 Hogan's Tourists

Hogan
- The Ranch House has been renamed to the Hogan.
- Adjusted the resource requirements to build the Hogan.
- Retextured the sides to reflect the Konkrete portion of its construction.
- You can configure the Hogan into one of three configurations, all of which retain their greenhouse functionality:
  Community Center: This is the default configuration. It has the same functionality as the original design. At least for now...
  Dormatory: This configuration has life support recyclers, and it attracts Tourists who want to experience life as a colonist for a few days.
  Study Hall: Like the stock MPL, you can train kerbals and increase their ranks. You can also train promising Tourists to become Pilots, Engineers, and Scientists. With the ever-increasing cost to hire new astronauts, the Study Hall gives you an alternative to acquiring new astronauts through rescue contracts.

New Contracts
- Touring: This contract requires you to send 1 or more tourists to a specific vessel and return them home safely after a few days stay.

- Colonial Aspirations: This contract requires you to send 1 or more tourists to a specific vessel equipped with a Hogan configured as a Dormatory, and let them stay there for a few days. The tourists are potential colonists; you get paid to deliver the tourists to the vessel but not to return them home. It's a great way to send tourists to a Hogan configured as a Study Hall to train them into a Pilot, Scientist, or Engineer.

ARP Icons
Added new Alternate Resource Panel icons courtesy of JadeOfMaar. These look great! :)

Bug Fixes & Enhancements
- Re-exported the models for the Hogan, Pipeline, and Rangeland.
- The Orbital Pipeline will now send payloads from orbit to ground- my girlfriend had a good argument for it.

1.18
- Fixed NRE experienced when setting resource distribution in the VAB/SPH

1.17
- Fix for resource distribution not working after you change the distribution settings.

1.16
- Fix for Pipeline NRE
- Hides the Show Resource Requirements buttons for assembled parts.
- Fix for the Ranch House's mass when unassembled.

1.15
- Far Future Technologies support: NuclearSaltWater used in place of Explodium.
- TAC-LS balancing- thanks Space Kadet! :)
- Fixed missing resource icons
- WBT Update
- Boxed parts (Ranch House, Rangeland, Pipeline) now have a button to Show Resource Requirements needed to assemble the part.

1.14
New Part
- Micro ISRU (Advanced Science Tech): This Buckboard-sized ISRU is capable of producing only one resource at a time but it comes with its own built-in advanced solar panel to power the conversion process.

Bug Fixes & Enhancements
- GoldStrike drills now properly catch up after you haven't visited the vessel in awhile.
- Fixed ground-based extraction rates for GoldStrike resource lodes.
- Fixed missing power requirements for the Gold Digger drill.
- Fixed missing power requirements for the Buffalo Drill.
- Added ability to (slowly) 3D print Equipment from MaterialKits in the Blacksmith and Clockworks.
- Adjusted recycling rate of Equipment into MaterialKits in the Blacksmith and Clockworks.
- The Saddle and Switchback 2 now have air park capability to reduce base slippage. USE WITH CAUTION! If you set the parking break, be sure to immediately quick save and reload to make sure the break is set.

1.13.1
- Fix for Ranch House IVA

1.13.0 Ranches And Pipelines
Along with some new parts, this release introduces a new capability: the ability to transfer resources between the active vessel and unloaded vessels, including KIS inventory items! To do this, all you need is to establish a Pipeline at both ends.

New Parts
- Ranch House (Composites): This large structure is an intermediate step between the inflatable temporary structures and the permanent Sandcastle dwellings that will be in a future release. Instead of having multiple configurations, the Ranch House provides production bonuses to all the parts with converters that are a part of the base. Additionally, it generates data that can be distributed to Doc Science Labs, Mobile Processing Labs, and the new Pipeline Mass Driver. That data can also be collected to provide a prospecting bonus when searching for Gold Strike lodes. The amount of bonuses and data generated depends upon how well staffed the Ranch House is. Finally, the part starts out as a Buckboard-sized packing box that requires copious amounts of Equipment to assemble. Unlike the smaller inflatable structures, the Ranch House cannot be disassembled.

- Pipeline Mass Driver (Nanolathing): The Pipeline lets you transfer resources and KIS inventory items to unloaded vessels in range. The transfers can be ground-to-ground or ground-to-orbit. Each destination must have its own Pipeline. Be sure to check the construction requirements; Pipelines are expensive- they need lots of Equipment and Konkrete. Like the Ranch House, the Pipeline starts out as a Buckboard-sized packing box, and cannot be disassembled.

- Orbital Pipeline (Nanolathing): The orbital variant of the Pipeline can perform orbit-to-orbit transfers and receive deliveries from ground-based Pipelines.

- Rangeland Construction Pad (Advanced Construction): Like the Ranch House, the Rangeland starts out as a Buckboard-sized packing box. once assembled, it forms a large pad from which you can build new vessels if you have Extraplanetary Launchpads installed. It also cannot be disassembled.
KNOWN ISSUE: The Rangeland allows vessel construction even when compacted.

- Buffalo Recycler Arm (Advanced Construction): This part can recycle parts that come in contact with its blowtorch. It doesn't require a skilled engineer.

- Lasso Aero/Hydro Scoop (Aerodynamic Systems, Advanced Aerodynamics): This specialized atmospheric/oceanic intake contains filters and processing units to retrieve valuable resources from the atmosphere and oceans of a planet. It comes in three sizes.

Bug Fixes And Enhancements
- Reduced reactor output from the Nukeworks but made it last longer.
- Nukeworks now produces NuclearWaste as part of its output products in Classic Stock play mode.
- Added Nuclear Reprocessor to the Nukeworks.
- WBT update.

1.12.0 Classic Stock

Module Reconfigurations
- To improve playability, all the inflatable parts now require an Engineer to reconfigure or assemble/disassemble (it was a mix of Engineer and Scientist before).

Play Modes
- Play Mode moved to Wild Blue Tools. Look for the "WBT" button on the Space Center screen.
- Play Modes now apply to all Wild Blue mods.
- Added support for Classic Stock play mode. This is a new mode that uses resources inspired by a system proposed by NovaSilisko and HarvestR back in KSP 0.19. YOUR CURRENT GAMES THAT USE CRP ARE SAFE! For the complete list of resources in Classic Stock and to see how the various converters use them, follow this link: https://github.com/Angel-125/Pathfinder/wiki/Classic-Stock-Play-Mode

Multipurpose Colony Modules
- Deprecated the Multipurpose Colony Modules parts found in the extras folder; they're now obsolete and replaced with equivalents in DSEV:
Multipurpose Base Unit -> Mk2 Ground Hub
Multipurpose Colony Module -> Tranquility Mk2 Habitat
Homestead Mk2 -> Homestead Mk3
Stagecoach -> Junction Storage Hub

Bug Fixes & Enhancements
- Various bug fixes.
- CRP is now a separate download.
- Updated to KSP 1.3.1.
- Skill required to recycle parts reduced to level 2.
- Skill required to recycel whole vessels reduce to level 4.

1.11.5
- BARIS is now an optional download as originally intended- just took awhile for me to figure out how to make that work. DO NOT DELETE the 000ABARISBridgeDoNotDelete FOLDER! That plugin is the bridge between this mod and BARIS.

1.11.1
- Minor fix for the Sunburn lab template.

1.11 BARIS: Building A Rocket Isn't Simple.

This release replaces the drill heat mechanic with BARIS: Building A Rocket Isn't Simple. You can think of BARIS (the name is a nod to an old video game) as a highly customizable Dangit-lite. Don't want your drills or converters to break? Don't want the hassle of parts wearing out? Do you prefer not to send Equipment up to repair your parts, or to requrire specific skills to repair them? No problem! All that and more are options. As always, It's Your Game, Your Choice.

BARIS IS OFF BY DEFAULT! You'll have to opt-in via the Settings->Difficulty->BARIS tab.

With BARIS, your drills and base parts now have wear and tear, which is indicated by the part's Quality rating. They're unlikely to break during their normal design life, but they can start to break down after you exceed their design life. If you perform maintenance on them, you can reduce the chance that they'll break. A broken drill or base part won't function, which can be either annoying or life threatening depending upon the mods you have installed. Once you fix the part, it will continue to function, but its design life will be reduced.

Bug Fixes & Enhancements
- Fixed missing textures.
- Fixed issue with geology lab not transmitting bonus science.
- Resource distribution should run a bit more smoothly now.
- Simplified Mode's ISRU now includes a drill.
- Fixed issues with template reconfiguring when no life support mod is installed.

1.10.0

Gold Strike
- You now have an unlimited number of chances to find resources in any given biome on any given planet. You still only get once chance per asteroid though.
- You now only need to drive a minimum of 3km away from the previous prospecting location in order to make another attempt.
- Lowered the chances of finding a lode.
- Lowered the max units of a lode's resource that you can find.
- The Pathfinder geology lab has the ability to prospect for resources.
- Your drills will automatically dig up the lode resource if there's room for it in your vessel; it functions independently of drilling for Ore.
- Prospecting asteroids is now functioning properly.

Bug Fixes & Enhancements.
- You can now carry the Sombrero and Telegraph.
- Drills no longer generate heat when in operation.
- When inflating parts, if you have no active distributors that are sharing the needed Equipment, you'll receive a message to remind you to turn on resource distribution.
- Fixed an issue where the resource distributor cache wouldn't be rebuilt after changing a part's distributor status and/or what resources to distribute.
- Fixed an integration issue with KPBS.

1.9.3
- WBT Update
- When you scrap a part or vessel, you no longer need to have resource distribution turned on. Any loaded vessel within range that can store the recycled resources will store them.

1.9.2
- WBT Update
- Bug fixes for Buffalo.

1.9.1
- Contracts update to include Spyglass as a cupola (Thanks Krakatoa! :) )
- Recompiled Kerbal Actuators.

1.9.0
- Recompiled plugin for KSP 1.3.
- You can now covert Equipment into MaterialKits in the Blacksmith and Clockworks.
- Reduced the price of the economy-breaking Aurum resource.
- Updated Snacks support.

1.8.8
- Fixed Crash To Desktop issues experienced at startup that are associated with Kerbal Actuators.
- Revised Kerbal Actuators GUI.
- You can now scrap parts and distribute the resources while in orbit.
- You can no longer scrap kerbals. That's just mean.
- Fixed NRE issues with the JetWing.

1.8.7
- Made it easier to climb onto the roof of the Buffalo cabins.
- Kerbal Actuators recompiled.

1.8.6

Buffalo
- VTOL Manager has improved hover management during low framerate conditions.
- Fix for IVA screens not remembering what image they were displaying.

1.8.5
Gold Strike
- Some locations now have a higher chance of containing valuable resources than others.

1.8.0
Buffalo MSEV
- Added new Buffalo Wings parts. See the Buffalo readme for details.

New Part
- S.A.F.E.R. : The Safe Affordable Fission Engine with Radiators generates ElectricCharge for your spacecraft needs. It is based upon the real-world SAFE - 400 reactor created by NASA.

Bug Fixes & Enhancements
- Fixed airlock issue on the Chuckwagon.
- Part scrapping now requires a confirmation click (which can be disabled in the options screen).
- With Extraplanetary Launchpads installed, you can turn Equipment into ScrapMetal.
- When Far Future Technologies is installed, FusionPellets are made from LqdHe3 and LqdDeuterium.
- Fixed an issue where Equipment wasn't being recycled after deflating a module.
- If you have insufficient room on your vessel to store Equipment recovered during module deflation, then the remaining amount will be distributed.

1.7.5 Shipbreakers

Part Scrapping

This release introduces the ability to scrap individual parts- and if you're skill is high enough, entire vessels- and salvage some Equipment in the process. You'll need a kerbal with the RepairSkill (Engineers have it) to perform the operation. The amount of Equipment recycled depends upon the mass of the part/vessel and the skill of the kerbal. A level 3 kerbal can scrap individual parts, while a level 5 kerbal can scrap entire vessels. For safety reasons, you  cannot scrap a part or vessel that has crew aboard.

When you scrap parts and vessels, any resources it contains can be distributed to any nearby containers participating in Pathfinder's resource distribution system. The container must be within 50 meters of the part scrapping kerbal, and the participating container must have resources you're interested in set to either Share or Consume. So if you want to recover resources from the part, then be sure to turn on resource distribution for your storage containers. You can always scrap a part regardless of distance to an available container.

Bug Fixes
- Restricted the number of contracts that are offered and/or active.
- Fixed a situation where experiments weren't registering as completed.
- Contracts won't be offered until you've orbited the target world and have unlocked the proper tech tree.
- Contracts that must be returned to the homeworld must be landed or splashed.

1.7.0 Spring Cleaning

Pathfinder Geology Lab
- Geology Lab experiments can now be completed as part of a contract.
- Gold Strike prospecting ability removed from the Pathfinder Geology Lab; you can continue to use the Buffalo's Crew Cab and the Gold Digger drill.

Iron Works
- Removed RocketParts->Equipment converter; this was in place during the transition from RocketParts to Equipment (necessary because, at the time, I thought RocketParts was a 1-liter resource).

IMPORTANT NOTE: Extralplanetary Launchpads is still fully supported; you can smelt Metal, recycle ScrapMetal, and make new RocketParts in the Iron Works when EL is installed. The Spyglass still serves as a survey station, and all inhabited Pathfinder parts provide some production ability.

Watney
- Removed Rainmaker (LFO) converter; it's redundant with the Fuel Cell converter, which also uses LiquidFuel and Oxidizer to generate ElectricCharge and Water.
- Increased LiquidFuel and Oxidizizer inputs on the Fuel Cell, and doubled its ElectricCharge and Water output.
- The Watney can now burn Ore to produce ElectricCharge, just like Buckboards converted to generators.

Bug Fixes & Enhancements
- Repairing broken parts now requires Equipment.
- The OPAL correctly outputs Water instead of using it as input.
- Increased ElectricCharge output of the Nukeworks' nuclear reactor.
- Buckboard generators are now using the correct engineering skills for enahanced efficiency and repairs.

Mod Support
- WBT Update
- Updated the Snacks converters to the latest standards.
- Dropped support for MKS; the mod changes too much to keep Pathfinder's MM patch up to date.
- Removed WBIGreenhouse from the TAC-LS Prairie; now it uses the same ModuleResourceConverter as the TAC-LS C.R.A.P.
- Removed WBIGreenhouse from the TAC-LS Cropworks; now it uses the same ModuleResourceConverter as the TAC-LS C.R.A.P. (with higher production rates).
- Removed WBIGreenhouse from the USI-LS Prairie; now it uses the same ModuleResourceConverter as the TAC-LS C.R.A.P.
- Removed WBIGreenhouse from the USI-LS Cropworks; now it uses the same ModuleResourceConverter as the TAC-LS C.R.A.P. (with higher production rates).

NOTE: The C.R.A.P. greenhouse remains in place for now, but be sure to convert your greenhouses over as it will be removed in a future update.

NOTE: USI-LS is out of date. Since it changes too often, I'm not inclined to update Pathfinder's MM_USILS patch. If you want an up to date version, send me a pull request. :)

1.6.2
- WBT Update

1.6.0

New Parts
- SCP Adapter: Use this to adapt standard crew ports (like the Mineshaft) to 1.25m parts. Thanks for the suggestion, JustJim!
- AKI Power Strip: If you have the Surface Experiment Package (SEP) by CobaltWolf installed, then you can tack one of these power strips onto the sides of your modules or along the base of the Saddle and gain extra AKI plugs.

WheelJack
- The top node of the WheelJack now serves as a docking port. Simply bolt the WheelJack to the ground and attach a stock Clamp-O-Tron Jr to the top (in KIS, cycle through the nodes until you select the "top" node). Then, attach a chassis or other part to the docking port, and continue your rover assembly. When finished, do a quicksave and reload, and finally, decouple the Clamp-O-Tron Jr. Thanks for the investigations, Sudragon!

Ponderosa Inflatable Habitat Module
- Added a probe core for automated vessel control (both Switchbacks already have them).

Pathfinder Geology Lab
- The Pathfinder Geology lab can now serve as a SEP Central Station. Thanks for the config file, Bombaatu!

Saddle, Gaslight, Telegraph, & Sombrero
- Added additional attachment nodes to make it easier to attach things like KAS ports and AKI plugs.
- Adjusted ground attachment nodes to help reduce the chance of parts exploding when bolted to the ground.

KIS Attachment
- To help with base assembly and correctly orienting inflatable modules, simply press "R" to cycle through the attachment nodes, and find the "KISMount" node. Use "KISMount" when attaching inflatable modules to your base.

Bug Fixes
- Fixed an issue where CropWorks was using the Snacks version of the CropWorks when USI-LS was installed. Ditto for the Prairie.
- Updated Buffalo ASET cockpit to use ASET 1.4: https://spacedock.info/mod/1204/ASET%20Props
- Fixed seating in the Doc Science Lab to prevent kerbals from poking through the seats.
- Fixed starter craft file so that it loads properly. Thanks bmaltby!
- Wired up the Switchback and Switchback2 for KerbNet.
- WBT update

1.5.0 Geoscience
This release updates the Pathfinder Geology Lab to use the WBI Experiment system originally developed for the Mark One Laboratory Extensions. The old efficiency studies have been replaced with new experiments that generate science reports just like the stock science parts do (HELP WANTED: science results text. Contribute to the results to improve your game!), and they will either improve or worsen your production efficiencies just like the original studies did. You can try to improve your production efficiencies in the current biome even when the experiments no longer generate science. Features of the new and improved Pathfinder Geology Lab include:
- A new resource called CoreSamples. The Pathfinder Geology Lab produces CoreSamples through basic research along with bonus science.
- Four new science experiments including: Soil Analysis, Metallurgy Analysis, Chemical Analysis, and Extraction Analysis. These are found under the Geoscience button.
- KerbNet access with Resource, Biome, and Terrain views. This access is provided under the new Biome Analysis button.
- Unlocking a biome's abundance summary and the TERRAIN uplink are now found under the new Biome Analysis button.
- Continued support of the Impact mod by tomf: The Pathfinder Geology Lab has a built-in bangometer.
NOTE: The Geoscience button will be unavailable until you unlock the local biome's abundance summary.

Bug Fixes & Enhancements
- The Chuckwagon has a new greenhouse option for USI-LS: the Continually Regenerating Agricultural Product greenhouse.
- Fixed an NRE issue with adding the TERRAIN to a vessel while in the editor.
- Fixed an NRE issue with the Conestoga lights.
- Fixed IVA issue with the Buffalo command cab and ASET props.
- Fixed control issue with the AUXen.
- You can properly configure a part to be a battery by using the ConverterSkill.
- Fixed an issue with IVAs spawning in the editor when inflating parts.
- You can now select the default image for the Plasma Screen in addition to screens in the Screenshots folder.
- Moved the kPad and plasma screens to the Utility tab.
- The Watney Chemistry Lab now produce a small amount of LqdDeuterium in addition to LqdHydrogen and Oxygen.
- Minor MM patches (thanks kerbas-ad-astra! :) )

1.4.0: Gold Strike
Want to get rich? This release of Pathfinder introduces Gold Strike, a mini-game where you travel around the local biome and go prospecting for valuable resources. These resources are found wherever you find Ore, even if the local biome or asteroid doesn't have the resource in any abundance. If you find a lode, drill for Ore, and you can convert that Ore into the valuable resource- as long as you remain in the area, and as long as there are units of the resource remaining. Use the Gold Digger, Buffalo's geology lab, and of course the Pathfinder Geology Lab to go prospecting.

Features include:
- Go prospecting for ExoticMinerals, RareMetals, Karborundrum, and Aurum. If you find something, start drilling for Ore and run the drill's gold strike converter. The converter is available on the Gold Digger, the new Buffalo Drill, the Hacienda's Claimjumper, and the stock drills.
- Limited chances to find a lode in a given biome that can be reset by paying a Science cost in Career/Science Sandbox mode.
- If you don't find a lode, then travel a set distance before trying again.
- Experienced kerbals with the Science Skill (like Scientists) can improve your prospecting chances if they go EVA and run the Gold Digger.
- Geology labs staffed with those with the Science Skill also contribute to your prospecting chances.
- Several configurable options found in Pathfinder's new difficulty settings screen.
- Easily customize the various valuable resources that can be found- just add new GOLDSTRIKE config nodes.
- New Aurum resource.

New Parts
- Buffalo Drill: This inline drill is about half as good as the stock radial drill.
- Buffalo SAS Module: This inline SAS module is as good as the Advanced Inline Stabilizer, and it has a probe core.

Bug Fixes
- Fixed drilling issues with the Gold Digger. NOTE: You still need someone with the ScienceSkill to improve its efficiency.

1.3.6
- Minor KerbNet fixes.
- Added the Oxide Processing Automated Lab (OPAL). Found in the Casa and Ponderosa, this specilized geology lab can convert Ore into Water, Waste, Slag, and Oxygen.
- Simplified the production chain for TAC-LS to use Waste in place of Fertilizer. Minerals and Fertilizer are no longer required.
- Simplified the production chain for Snacks; you no longer need Organics, and Fertilizer is made in the Pigpen from Ore and Minerals.

1.3.5
- WBT update
- The Doc Science Lab now has a data transfer utility.

1.3.2
- KSP 1.2.2 update

1.3.1
- Greenhouse fixes.
- Templates require MaterialKits instead of Equipment if MKS is installed.

1.3.0
- The Spyglass now serves as a habitation multiplier when USI-LS is installed.
- Added a bunch of new parts to the Buffalo. Be sure to read the Buffalo readme for details.

1.2.9
- Cleaned up some logging issues related to missing part modules and textures when supported mods aren't installed.

1.2.8
- Updated to KSP 1.2.1

1.2.7
- Fixed versioning conflicts
- Fixed an issue where deflated modules had resources when pulled from a KIS inventory.

1.2.5
- WBT update
- Fixed graphics issue with the Doc.

1.2.4
- Updated Snacks support
- Updated USI-LS support
- Fix for KPBS greenhouse
- Added Multipurpose Colony Modules (MCM) to the Extras folder.

1.2.3
- Moved the Mule to Space Exploration. It also appears in the Payload tab.
- Moved some Pathfinder game settings to the Wild Blue tab in the Game Difficulty screen.
- Removed deprecated parts. Reminder: If you want spacedocks, the Mk2 Homestead, and more, download Multipurpose Colony Modules for Pathfinder.
- The OSE Workshop templates now recognize KSP's new part categories.
- Rendering performance improvements for the Spyglass.
- The OSE Workshop templates should now recognize KSP's new part categories.
- Rendering performance improvements for the Spyglass.
- For the modules that used to require Engineers, now any class that has the ConverterSkill qualifies to reconfigure modules. For modules that used to require Scientists, now any class with the ScienceSkill qualifies to reconfigure the modules. Ditto for converters.
- The Claimjumper now includes an industrial version of the Geology Lab's Prospector. Simply drill for Ore, and the industrial prospector will sort out all the available resources in the biome. It has a lower Slag output due to its higher quality processing equipment.
- Fixed collider issues with the Doc Science Lab.
- The Telegraph can now function as a relay transmitter.
- The Ponderosa can now level up kerbals.
- The Ponderosa also serves as a probe control point. You need a minimum of two pilots.

Removed Parts
- Removed the Stockyards and Homestead. You have two options if you want these parts. First, the Mark One Laboratory Extensions' Drydocks replace the Stockyards, and MOLE's Bigby Orbital Workshop replaces the Homestead. Second, you could download the Multipurpose Colony Modules for Pathfinder mod extension. In addition to the Stockyards and Homestead, you get an old favorite, the Pioneer Multipurpose Colony Module and Settler Multipurpose Base Unit.

1.1

Last update before KSP 1.2!

New Parts
- Added the Telegraph deployable communications dish.
- Added the Sombrero Solar Array. This dual-axis solar array can point at the sun in two directions.
- Added the Radial Engine Mount. It is particularly helpful for getting rovers onto the ground.

Greenhouses
- Growth time is no longer reduced based upon experienced Scientists. Yield is still affected by experience though.
- Greenhouses now show where they're at in the growth cycle and show up in the Ops Manager.

USI-LS
- Replaced Water usage with Dirt. Apparently Fertilizer is mostly water.
- Adjusted the resource consumption amounts to be comparable with stock USI-LS.
- While Dirt is an extra added resource, it accounts for the possiblility of improved yields.
- The Pigpen now produces Fertilizer from either Ore, Minerals, or Gypsum.
NOTE: expect additional changes once KSP 1.2 is available. It sounds like USI-LS is changing.

Other
- Updated support for CLS.
- The Sunburn lab won't have the ability to produce Coolant unless you have DSEV installed.
- Buffed the Doc Science Lab's data capacity to reflect changes in the stock MPL. It is still optimized for ground operations.
- Added a small nuclear reactor to the Nukeworks for those times when you don't land in a place that has geothermal activity.
- Fixed some KAS and lighting issues with the Gaslight.
- Adjusted tech tree positions of various modules; they'll show up roughly in Tier 6.
 
1.0.0

This release introduces a couple of new parts and refactors some old ones. It also delivers the Chuckwagon IVA.

New Parts
- Added standard width 1u and 2u Solar Flatbeds. If you have parts attached to them then they won't generate ElectricCharge. Thanks for the suggestion, DStaal and Bombaatu! :)

Gaslight, Saddle, Patio & Switchback2
- Adjusted the colliders to help alleviate explosive parts when attaching them to the ground.
- Due to the way that KAS works, the Saddle and Gaslight now only have one pipe connection. The older versions have been deprecated, so your existing bases will be ok.

Chuckwagon
- The Chuckwagon finally has its IVA! :)

Inflatable Modules
- When inflating or reconfiguring modules, you can now pull Equipment from vessels participating in resource distribution. This happens automatically as long as vessels are sharing their Equipment.
- Due to the way that contracts work, inflatable modules will have a crew capacity listed once again. Please don't flat-pack kerbals. I'm looking at ways to auto-kick kerbals out of deflated modules while in the editor.
NOTE: You might have to return to the space center and then go back to your new base in order for contracts to recognize the inflated crew capacity.

USI-LS
- All inflatable modules will now have the capacity to store Supplies.
- Increased the recycling capability of the Ponderosa template to 90%

Doc Science Lab
- The Doc Science Lab template now participates in the WBI Experiment System. You can load up to four experiments into the lab and conduct research, either from a suitable experiment container (Mark One Laboratory Extensions has several) or by generating new experiments from their list of required resources. Coming soon in KSP 1.2: you'll also need a connection back to KSC to receive instructions. Experimental results can be transferred into the mobile processing section, and the experiment lab generates bonus science.

Watney
- Adjusted the Water production for the Rainmaker converters.
- Added a fuel cell converter to produce ElectricCharge from LiquidFuel and Oxidizer.

0.9.39
- You can climb into the Doc Science Lab again.
- Part mass is now correctly calculated.
- Updated formula for producing Glykerol.

0.9.38
- All generators (Buckboards, HotSprings, SolarFlare) now list how much EC they are generating.
- If a part is participating in resource distribution, and the resource isn't required by a converter or on the blacklist, then you can now individually set a resource to be shared, consumed, or ignored.
- Added support for USI-LS to Buffalo parts.

0.9.36

New Part
- Added 2U Slim Chassis.

Flex Fuel Power Pack
- The Flex Fuel Power Pack now uses the standard configurable storage window to select flex fuel types. You still need to go EVA to change it, and it requires an Engineer (unless you turn off the skill requirement).
- The Flex Fuel Power Pack will have the appropriate fuel resource storage for the selected fuel type.

Spyglass
- The Spyglass now counts as a cupola in base building contracts. Thanks for the suggestion, lukeduff! :)

Resources
- Added the Uraninite template. Thansk for the suggestion, lukeduff! :)
- All Buffalo chassis parts can now participate in Pathfinder's resource distribution.
- The Outback now uses the standard resource configuration window.
- You can now change the configration on tanks with symmetrical parts. In the SPH/VAB it will happen automatically when you select a new configuration. After launch, you'll have the option to change symmetrical tanks.

Nukeworks
- The Nukeworks can serve as a radioactive storage container if you have Near Future Electrical installed. Thanks for the suggestion, lukeduff! :)

Bug Fixes
- Fixed an issue where the Buffalo asteroid scanner would not display the results window after scanning an asteroid.
- Fixed an issue where duplicate entries for the commercial science lab were visible in KPBS labs.

0.9.35
- Fixed an issue with the editor locking up when using the M1-A1 Mountain Goat.
- The M1-A1 can now self-destruct in addition to decoupling (both when surface attached and node attached).
- Reduced the ejection force used when the M1-A1 is decoupled.
- The Buffalo Command Cab window lighting now toggles on/off with the headlights.
- If fuel tanks are arrayed symmetrically, you'll no longer be able to reconfigure them. It's either that or let the game explode (ie nothing I can do about it except prevent players from changing symmetrical tanks).

0.9.34

Flex Fuel Power Pack
- Flex Fuel Power Pack can now run on LiquidFuel & IntakeAir. It produces as much ElectricCharge as LiquidFuel/Oxidizer. Thanks for the suggestion, Geschosskopf! :)
- Added exhaust effects.

Buffalo Crew Cabin
- Added a node on the top to help facilitate mounting the Flex Fuel Power Pack.

M1-A1
- you can now decouple the OmniWheel when it is surface attached as well as when node mounted.

Rendering Performance
- Improved performance and reduced memory footprint for resource distribution.
- Improved rendering performancs of the Operations Manager.

0.9.33

Heat Generation
- To help prevent barbequing kerbals and bases, the Claimjumper's and Gold Digger's heat generation has been disabled for the time being until the problem can be sorted out. It appears to be an issue between KSP's FlightIntegrator and ModuleCoreHeat. Not much I can do about that... Currently Pathfinder's other converters don't generate heat.
- Added "maxSkinTemp" to all base building parts.

Resource Distribution
- You can add resources to the new resourceBlacklist and they won't be distributed. Example: ReplacementParts are used by USI-LS to determine module wear, so they go on the blacklist. The blacklist is defined in the part config's MODULE node.
- You can now set the distribution mode to Distributor, Consumer, or Off.
  Distributor: The part's resources will be distributed if the resource isn't required by a converter and it is unlocked.
  Consumer: the part's resources will be filled to capacity if the resource is unlocked.
- Resource distribution is now restricted to vessels/bases that are landed, splashed, or in prelaunch. Thanks for the suggestion, Geschosskopf! :)

0.9.32

Conestoga
- Added lights all around the part.
- Adjusted consumption rate of the MPU to be in line with stock fuel cells and MOLE's MPUs.

Stockyards
- If you have MOLE installed, the Stockyard 250 and 375 won't be available (existing craft won't break, you just won't be able to build new ones with Stockyards); use the ones in MOLE instead.

Bug Fixes
- Fixed the Konkrete and Slag templates so that they'll be named properly and store the correct resources.
- Fixed storage issues for the Cryo Fuel templates.
- Fixed an issue where the Spyglass spotlight animation didn't rotate properly upon reloading the scene.

0.9.31
- Removed deprecated Switchback that is no longer deprecated. This was likely causing the duplicate Switchback issues seen in the tech tree.

0.9.30
- Fixed attachment node issues with the Switchback 2
- Resource distribution will now correctly ignore resources that are locked or that are required by various converters. All resources in a part will be distributed if the distributor is active and the resource isn't required by a converter, and the resource is unlocked.
- Adjusted empty mass of the Hacienda to bring it in line with the other inflatable modules.
- Fixed an issue where the geology lab GUI wasn't showing up.
- Operations Managers now show the correct part name.

0.9.29
- Updated WBT

0.9.28
The Doc and Spyglass finally get their IVAs! Just two more to go...

New Parts
- Added the Buffalo ISRU, built from the stock mini ISRU. It has several configurations to choose from.
- Added the Flex Fuel Power Pack. The Power Pack is a 2U Buffalo chassis unit that has solar panels and also a generator that can burn Ore, MonoPropellant, or LiquidFuel & Oxidizer. Power output varies depending upon the configuration.

IVAs
- Added IVA to the Doc Science Lab.
- Added IVA to the Spyglass.

Tundra & Wagon Parts
- The Tundra 200 and 400, as well as the Wagon, can now become a resource distributor or consumer. As a distributor, the storage part shares its resources. As a consumer, it fills its resources from the distribution system until full.

Hacienda
- Added an industrial greenhouse template to the Hacienda, because pandas chew through a lot of bamboo.

Contracts
- Added a first stab at contracts for the Ponderosa, Casa, Hacienda, Conestoga, Chuckwagon, and Doc.

Bug Fixes
- Fixed an issue where converters weren't benefiting from research projects in the geology lab.

0.9.27 
This release brings Pathfinder closer to its 1.0 release. It introduces the new Local Operations Manager, a screen that lets you control the functionality of all vessels within physics range. It also makes some tweaks to the life support systems, and offers a new way to process resources in order to fix an issue where you could produce RareMetals and ExoticMinerals in biomes/asteroids that didn't have RareMetals and ExoticMinerals. This new method also gives a way to squeeze more resources out of asteroids, and provides a key component to the future making of Konkrete.

Local Operations Manager
- Added the Local Operations Manager (LOM) to the Pathfinder window. You can access it in the Flight app toolbar (the covered wagon icon). With the LOM, you can control the operations of various parts on every vessel within physics range. You can control the functionality of: configurations, converters, lights, cooling towers, drills (including stock drills), and more!

Life Support
- Dirt is no longer a requirement for the greenhouse. Instead, it depends upon your life support mod: TAC-LS, Snacks, Kerbalism (100 Fertilizer); USI-LS (100 Mulch)
- Removed Cultivator since Dirt is no longer required.
- For TAC-LS, moved the Carbon Extractor from the Ponderosa to the Pigpen.

Resources
- Added the Slag resource and associated storage template.
- Added the Konkrete resource and associated storage template.

Geology Lab
- The Prospector can now process Ore into resources found in the current biome. Most of what it produces is Slag.

Homestead
- The Rockhound can now process Rock and/or Ore into resources found in the designated asteroid attached to the vessel. Most of what it produces is Slag.

0.9.26
- Recompiled for KSP 1.1.3

TAC-LS
- Increased the Prairie's crop yield of Food to 125. Remember, a skilled scientist can reduce growth time by up to half, and increase crop yield up to double.
- Added a carbon extractor to the Ponderosa (scrubs CarbonDioxide to produce Oxygen and Waste).

Hacienda
- Added the Nukeworks template. It can process Uraninite into EnrichedUranium, and reprocess DepletedUranium into EnrichedUranium.

Other
- Updated support for OSE Workshop.
- Inflatable modules now list their inflated crew capacity in the part summary panel.
- Refactored the Operations Manager windows to make it easier to select a module configuration and to control the module.
- When selecting a template in Career or Science mode, templates that require tech nodes that you haven't researched yet will be grayed out in the list.
- Added official support for Kerbalism.
- Added resource patches to find Water on Minmus.

Bug Fixes
- Fixed an issue where inflatable modules would show their inflate button in the VAB/SPH when they shouldn't be.
- Added the drill switcher window to the Gold Digger.

0.9.25
- Updated to latest WildBlueTools, which fixes NREs and Input is NULL errors.
- Improved GUI for selecting storage templates.
- Fixed missing texture files in the Hacienda.
- You can now click on the laptop prop's screen and change its image.
- Updated to latest ModuleManager.

0.9.24
- Fixed an issue where the Buffalo crew module was not loading properly and messing up the KIS seat inventories as a result.

0.9.23
- Fixed NREs and Input is NULL errors.
- Fixed an issue where the Buffalo Adapter was not showing up in flight.
- Fixed an attachment node height issue with the M1-A1.
- Increased ElectricCharge storage in Buffalo parts to account for new KSP 1.1 wheel power requirements.
- Added support to the Buffalo for Kerbalism.
- Made some minor updates to the Buffalo Crew Module IVA.
- Fixed an issue in the Saddleback 2 where it would be spawned partly below ground when pulled from a KIS inventory.

0.9.22
- Fixed an issue with the Switchback sliding around after being bolted to the ground.
- The original model for the Switchback is back temporarily to help allieviate some of the issues found.
- All buffalo parts now have drag cubes.

0.9.21
- Fixed an issue with the Gold Digger and Contract Configurator. Thanks for the fix, Enceos! :)
- Fixed an issue where the Ponderosa would not allow entry/exit into the module.

0.9.20
- More bug fixes (sigh)

0.9.19
- Fixed missing PlayModes and MM patches

0.9.18
- Fixed mission icons and decals
- Fixed an issue with the toolbar app
- Added drag cube to the Conestoga

0.9.17
- Fixed missing parts

0.9.16
- Updated to KSP 1.1.2

0.9.15
- updated to KSP 1.1.1
- Increased breaking force of the Saddle and Switchback. Hopefully this will help reduce kraken attacks.
- Moved the Stockyard 250 to Advanced Construction (same as EL Orbital Dock)
- Slight update to the Tunnel Extender

0.9.14

New Part
- Introducing the kPad Air. It's just like the giant plasma TV screens only smaller. The aspect ratio of width to height is 1 : 1.18. Thanks for the suggestion, Parkaboy! :)

Homestead
- The Homestead can now be configured for: Brew Works, Ironworks, Ponderosa, Pathfinder, Pigpen, Solar Flare, Sunburn, Watney, and the new Rockhound.
- Added the Rockhound template. It converts lots of Rock into ExoticMinerals and RareMetals.

Wheels
- Grizzly has moved to the Advanced Motors tech node.
- Retuned the durability and default traction of the Buffalo wheels. Thanks for the testing, Geschosskopf! :)

Conestoga & Switchback
- Increased the crash tolerance on the solar panels- they'll break when the part does.

Inflatable Modules
- IVA overlays won't be shown when the inflatable modules are deflated.
- No more crew slots in the VAB/SPH when the inflatable module is deflated.
- Fixed an issue where the Blacksmith and Clockworks configurations would forget what projects the OSE Workshop was working on.
- Fixed drill switch window; you can drill for different resources again.
- Fixed an issue where you could not transfer crew between modules using the Transfer Crew feature.
- You can get back into and out of the Ponderosa again.
NOTE: Currently the "Transfer Crew" right-click menu button is unavailable for inflatable parts but you can still transfer crew by clicking on an EVA hatch. I'm working on this one... :)

Other
- Fixed (again) the versioning file

KNOWN ISSUES
- You're unable to surface attach parts to the Buffalo Command Cab until you change its orientation after spawning the part. Just change the orientation, and change it back to the desired orientation as a workaround.

0.9.13
- Fixed an issue with the Snacks MM patch
- Fixed ladder and airlock triggers
- By request, the Pigpen and Prairie greenhouse are available even when there is no life support mod installed.

0.9.12 The Perfect Storm

This release updates Pathfinder to KSP 1.1!

eberkain had a request to have a conversion chain similar to Simple Construction’s Ore->Metal->Rocketparts. That got me thinking about dusting off some old plans I had for Pathfinder and rethinking them. Among other things, this update implements those plans. Thanks eberkain! :)

New Parts
- Added the Tunnel Extender, a part designed to put some space between between modules to make it easier to use the airlocks. It also has a solar panel on the roof and can serve as an attachment location for your stuff.
- Added the all new Consestoga Multipurpose Base Module (MBM). This redesign turns the Conestoga into a foundation module for your bases. Simply fly and/or drive it to the desired location, eject the wheels (if you attached the M1A1), and rest the module directly on the ground. The original part is still around! The old Conestoga has been renamed the Mule.

M1A1 Mountain Goat
- M1A1 Mountain Goats now have an eject feature. They're particularly useful for moving a starter base into position before discarding the wheels.

Pathfinder Settings Window
- Added a Play Mode indicator and a button to change the play mode. This is only available at the Space Center. Pathfinder now offers three different play modes: Default, with all the bells and whistles enabled; Simplified, which reduces the number of resources and configurations you need to keep track of (which has a simplified Ore->RocketParts converter among other things); and Pristine, which just gives you nice looking modules to assemble without the hassle of things like resource extraction and conversion. You can find detailed descriptions of these play modes on the Pathfinder Wiki. To use a play mode, simply open the Pathfinder Settings Window from the Space Center, choose the mode you want, and then restart KSP. Pathfinder’s existing parts and plugin code will adjust accordingly. 
Warning: changing Pathfinder’s play mode WILL affect your currently deployed bases. Custom play modes are possible, just consult the wiki.

- Added the ability to evenly redistribute resources to all vessels within physics range. This happens automatically, but you can manually initiate a resource distribution from the Pathfinder Settings window. The Ponderosa, Casa, Doc, Hacienda, Chuckwagon, and Conestoga can all distribute their resources automatically if you opt-in. To enable an individual part, simply right-click on the part and make sure that the Distributor is set to "ON." To enable resource distribution on all parts that have the capability on the currently focused vessel, go to the Pathfinder Settings window and press the Distribution button until it says "ON." If you want to prevent a part's individual resource from being distributed, then be sure to lock the resource.
NOTE: Resource distribution favors parts with converters that have required resources (such as the greenhouse's Dirt). Resources will fill parts that have converters with required resources first before distributing the remainder to other parts. For more information, consult the wiki.

Hacienda
- Added the official IVA. As with the Ponderosa/Casa, you can safely delete the Ponderosa/Spaces folder if you need more system memory/performance. If you do that, you'll have the placeholder IVAs (which will look wonky).
- Fixed issues with the drill not showing up for the Claimjumper and Hot Springs.

Pigpen
- The Pigpen template will only be available if you install a life support mod (Snacks, TAC-LS, or USI-LS).

Doc
- Added minimalist IVA. It will be completed at a later date.

Chuckwagon
- Added minimalist IVA. It will be completed at a later date.
- The ability to convert between storage and a greenhouse will only be available if you install a life support mod (Snacks, TAC-LS, or USI-LS).
- Revised the in-field template switching. Instead of the Next and Previous buttons, now you click on the Reconfigure Storage button.
NOTE: If you'e upgrading from previous versions, you'll lose your stored resources. Be sure to Hyper-edit back what you need.

Buckboards
- Revised the in-field template switching. Instead of the Next and Previous buttons, now you click on the Reconfigure Storage button.
NOTE: If you'e upgrading from previous versions, you'll lose your stored resources. Be sure to Hyper-edit back what you need.

Ironworks
- Fixed the conversion recipies for MetalOre->Metal, Metal->RocketParts & ScrapMetal, RocketParts->MaterialKits and MaterialKits->RocketParts. Thanks RiverRat2800!
- Added the ability to create Equipment from Ore, RareMetals, ExoticMaterials, and ElectricCharge.
- Added converter to go between RocketParts and Equipment.

TERRAIN
- Fixed an issue where you could not scan for resources in certain situations. Thanks for the detailed assessment, nobodyhasthis2! :)

Switchback
- Redesigned the part slightly so that it no longer needs to be attached to a Saddle. Simply drop it onto the ground and you're good to go. It also includes a KAS pipe. The previous design has been deprecated.

Resources
- Added Equipment resource and template.
- Refactored RocketParts storage template to reflect that it’s actually a 5-liter resource instead of a 1-liter resource. Whoops!
- Inflating and reconfiguring modules now uses Equipment instead of RocketParts. Repairs still require RocketParts.

Other
- You can now specify the resource and amount required to switch a template instead of the previously hard-coded requirement for RocketParts.
- Changed the texture on the crew ports for inflatable modules because Parkaboy is awesome. Fantastic idea! :)

0.9.11

- Fixed an issue preventing the Buckboards from allowing resource switching.
- Fixed an issue causing the TERRAIN to fly off into space upon loading the vessel.

0.9.10 The Calm Before The Storm

This update brings some quality of life improvements, bug fixes, and new uses for the Buckboards. It also provides support for Nils77's excellent Kerbal Planetary Base Systems. If you haven't seen KPBS, check it out! The design and art quality is simply stunning.

Switchback
- The attachment nodes now support symmetrical installation of parts.

Buckboard 2000 & 3000
- The Buckboard 2000 & 3000 can now be converted into an ore-powered generator. It consumes Ore to produce ElectricCharge. The generator can break down (unless you opt-out via Pathfinder's Settings screen), and it generates heat. It also benefits from the Pathfinder Geology Lab's Metallurgy research. In career mode, the generator conversion is available after you research Advanced Electrics. Thanks for the suggestion, Geschosskopf! :)

Kerbal Planetary Base Systems
- Added support for Kerbal Planetary Base Systems. While not as efficient as Pathfinder's modules due to their size, they can still contribute to your Pathfinder base, and still benefit from efficiency improvements created in the geology lab.
  * The K&K Planetary Habitat MK2 can be used as part of Pathfinder's habitat wing.
  * The K&K Planetary Laboratory can be used as part of Pathfinder's science wing.
  * The K&K Planetary Greenhouse uses Pathfinder's greenhouse module code.
  * The K&K Rocket Fuel tanks now hold a variety of different resources (and default to LiquidFuel/Oxidizer).

Other
- You can now access the Pathfinder Settings window from the application toolbar.
- You can now dump resources in flight without having to switch templates. Click "Dump Resources" once, then again to confirm.
- The airlocks on the inflatable modules have been offset to make it easier to enter the desired module. NOTE: due to space constraints the Casa now only has one inflatable airlock. A new version of the Casa is planned that won't break existing bases.

Bug Fixes
- The internal screens in the Ponderosa/Casa work better now. Click on a screen to select an image from your Screenshots folder. You can also enable random selection of images; currently the images will change once every 30 seconds if configured to do so, but a future update will let you configure the screen switch time. Thanks for the challenge, Kuzzter! :)
- Fixed attachment nodes on all inflatable modules so that you can properly stack-mount the inflatable modules in the VAB/SPH.
- Fixed a KIS issue with the Buckboard 3000 that prevented it from being carried properly.
- Fixed a KIS issue with the Buckboard 2000 that prevented it from being carried properly.
- Fixed a rare case where the drill modify window would lock up the game.

0.9.9
- Fixed an issue with USI-LS support where the Life Support template wasn't showing up. Also, added Dirt since there was enough room in the template.

0.9.8 Life, don't talk to me about life.
This update overhauls the various module manager patches for Snacks, TAC-LS, and USI-LS. This overhaul takes advantage of new resources available in the Community Resource Pack, and as a result, the converter inputs and outputs for the Prairie and Pigpen make more sense now.

Snacks
- The Pigpen composter now benefits from Scientists, not Engineers.
- The Pigpen Composter now creates Fertilizer (you need ElectricCharge, Organics, Minerals, and Dirt).
- The Pigpen can create Dirt (you need ElectricCharge, Ore, Minerals, and Fertilizer).
- The Prairie greenhouse now consumes ElectricCharge, Water and Fertilizer, and requires Dirt.
- The Watney lab can produce Organics (you need Ore, Water, and ElectricCharge). Go organic chemistry! :)
- Removed the Watney's Snack Grinder.

TAC-LS
- The Pigpen converters now benefit from Scientists, not Engineers.
- The Pigpen Composter now creates Fertilizer (you need ElectricCharge, Water, Waste, and Minerals).
- The Pigpen can create Dirt (you need ElectricCharge, Ore, Minerals, and Fertilizer).
- The Prairie greenhouse now consumes ElectricCharge, Water and Fertilizer, and requires Dirt.

USI-LS
- Overhauled the converters and support for USI-LS to reflect the latest version. See MM_USI-LS.cfg for details. Thanks for the tips on how to configure things, RoverDude! :)
- Modules now have a lifespan and will wear out over time if USI-LS is installed.
- The Pigpen converters now benefit from Scientists, not Engineers.
- The Pigpen Composter now creates Fertilizer (you need ElectricCharge and Mulch).
- The Pigpen can create Dirt (you need ElectricCharge, Organics, Minerals, and Fertilizer).
- The Prairie greenhouse now uses ElectricCharge, Water and Fertilizer, and requires Dirt.
- The Ponderosa habitat template has a life support recycler that supports up to 6 kerbals.
- The Watney lab can produce Organics (you need Ore, Water, Minerals, and ElectricCharge). Go organic chemistry! :)

Templates
- Added Dirt template.
- Added Fertilizer template.

Sunburn Lab
- Added the ability to make Glykerol (you need ElectricCharge, ExoticMinerals, RareMetals, and Coolant) if Deep Freeze is installed.

Old Faithful
- The Old Faithful now has the ability to toggle open cycle cooling. With open cycle cooling, it can shed even more heat at the cost of expending water.

Other
- Moved Module Manager patches to the Pathfinder/ModuleManagerPatches folder. There's a lot of them now...
- Added support for Connected Living Spaces to the Buffalo. Thanks for the assist, Technologicat! :)

Bug Fixes
- The Watney Rainmaker converters will now dump excess ElectricCharge instead of getting stuck.
- Fixed Remote Tech issues with the TERRAIN. Thanks for the investigation and fix, Vaga! :)

0.9.7

Ponderosa/Casa IVA
- You can now click on a screen to change its image, provided that your scene loaded with a kerbal in the part. See KNOWN ISSUES below.

Buffalo Command Cab
- Added support for TAC-LS

Buffalo Crew Cab
- Added support for TAC-LS

Prairie
- To help with part count at the base, the Prairie now produces enough Snacks/Supplies/Food for 3 kerbals for 90 days.

Bug Fixes
- Updated required Organics in the Prairie to match rebalanced max resource storage amounts.
- Fixed an issue where the Prairie wouldn't start running the Snacks greenhouse even though it had a full load of required resources.
- Kerbals won't fall over if trying to strap a Casa/Ponderosa onto their back.
- Ponderosa/Casa won't explode when attached to another part.

KNOWN ISSUES
- The monitors in the Ponderosa/Casa will retain their default image if the scene loads without a kerbal in the part.
WORKAROUND: Put a kerbal in the part, and reload the scene.


0.9.6 Young Feathers
This update brings a bunch of new parts, Pathfinder's first official IVA, and some tweaks. At last the Ponderosa/Casa has its official IVA! The internal view reflects the module's role as the habitation component of Pathfinder's base building (the bottom floor is deliberately empty- for now). For the memory conscious, you can safely delete the Pathfinder/Spaces folder and revert back to the placeholder IVA.

New Parts
- Added the M1A0 Bear Cub. This adorable wheel is used for small rovers.
- Added the WJ400 Jaguar, a micro-jet engine. It's a great booster engine for weighed down JetWings.
- Added a half-sized chassis. It has an integrated solar panel, but no KIS storage.
- Added a quarter-sized chassis. It too has an integrated solar panel and lacks KIS storage.
- Added the ATV Command Seat. It's just like the External Command Seat, except that it can only be stack attached to parts.

Ponderosa/Casa
- Added the official IVA. Remember, you can delete Pathfinder/Spaces if you need more memory.
- If you put an image in the Pathfinder/Spaces/Screens folder then it'll appear on the monitor. Be sure to use an aspect ratio of 1.33:1 (width:height, such as 1024 by 768). See WBIScreenPropHelper module in Ponderosa.cfg and Ponderosa2.cfg for other goodies. Want to add an image? Post it on the forums and I'll give you credit. Special thanks to Kuzzter for allowing me to use his artwork. :)
- If you want to change the static images on the walls, simply navigate to the Pathfinder/Spaces/Assets folder, and replace the StaticImages.png file with one of your own design. Tip: If you change the image size to, say 2048 by 2048, you can increase the picture resolution.

JetWing
- Reduced the engine ISP by half. Transcontinental flights are fun and all but you know, a wee bit OP...

Wheels
- Adjusted traction values to be more in line with stock wheels. This work is ongoing, some of the stock values don't work for these wheels. Many thanks to Taniwha for the scrips that gave me the values. :)
- Removed the interim rover wheel.

Templates
- Renamed the Claw Marks to the Claim Jumper, and Smoke Pipe to Old Faithful- thanks for the names, AdmiralTigerClaw! :)

Bug Fixes
- Fixed an issue where the Geology Lab would be unable to show the resource percentages after reloading the base.
- Fixed an issue preventing the OSE Recycler from working with the latest version of OSE Workshop.
- Fixed an issue with the TERRAIN that let you spam data reviews before starting the geo survey.
- Fixed a bug that would cause a crash when switching to a Terrain satellite from the geology lab.
- Starter craft from wiki now has the proper wheels.
- Fixed a collider issue with the Grizzly. It shouldn't drag its feet now. Thanks for your help Beeks! :)

Other
- Added support for RemoteTech.

0.9.5
JetWing
- When you load the flight scene, the JetWing will look at what fuel you're using and set up the engine mode automatically.
- Improved the jet flame effects for liquid fuel mode.
- You can now re-bind the hover control keys. Simply right-click on the wing, click "Show GUI" to display the on-screen hover controls, and click the gear button to display the hover controls keyboard mapper.
- JetWing's art assets and config files are now in the Buffalo/Parts/JetWing folder. You can safely delete all other folders in the Buffalo/Parts folder as well as the Buffalo/Spaces folder and Buffalo/Assets folder if all you want is the JetWing.

Outback
- The Outback's art assets and part file are now self-contained in the Buffalo/Parts/Outback folder.

0.9.4 Believe it or Not
The Greatest American Hero Theme Song - Believe it or Not 

Now you can fly around any planet that has an atmosphere with a personal jet-powered wing! It works similarly to a command seat; just drop it on the ground (be careful, some terrain is explosive), right-click to board it, then rotate upright. Alternately, bolt it to the side of a craft, board, and decouple. The wing can be used on any planet with an atmosphere thanks to its monopropellant-powered jet engines. Just don't expect terrific range, unless you switch to liquid fuel and fly in an oxygenated atmosphere like Kerbin or Laythe. JetWing is inspired by JetMan Yves Rossy and the defunct KerbolQuest winged jetpack. The JetWing and its accessories work best with Kerbal Inventory System and either GoodSpeed Fuel Pump or TAC Fuel Balancer.

In addition to the Jetwing, this release also introduces the M1A2 Grizzly, a larger wheel than the Mountain Goat that has a wider wheelbase and better traction. This all comes at the expense of not being able to enter and exit Mk3 cargo bays, however.

Finally, there were some big changes to OSE Workshop and the Community Resource Pack recently, and Pathfinder has been updated to reflect the changes. You might find prospecting a lucrative business if you bring stuff home...

NOTE: Due to changes in OSE Workshop, you can no longer switch between MaterialKits and RocketParts. However, the Ironworks still has converters that switch between RocketParts and MaterialKits. Additionally, OSE Workshop's changes now require ExoticMinerals and RareMetals for some parts, so plan accordingly.

New Parts
- Added the M1A2 Grizzly.
- Added the JetWing. 
- Added the JetWing Drop Tank. Works great with Good Speed Fuel Pump or TAC Fuel Balancer.
- Added the JetWing Parachute. Like the Outback, it can be attached to the JetWing.
- Added the JetWing Cargo Pallet. Mount these to the wing's hardpoints and you can surface-attach small items.

JetWing Flight controls
Insert: Toggle VTOL mode
PageUp: Increase vertical speed
PageDown: Decrease vertical speed
Delete: Reset vertical speed to zero
Gear: Toggle kickstand

Outback
- Moved the Outback from Pathfinder to the Buffalo mod.

Storage Templates
- Added ExoticMinerals template.
- Added RareMetals template.

Blacksmith
- Removed ability to switch between RocketParts and MaterialKits.
- Updated production requirements to latest OSE Workshop standards.

Clockworks
- Removed ability to switch between RocketParts and MaterialKits.
- Updated production requirements to latest OSE Workshop standards.
- Added converter to sift Dirt into ExoticMinerals and RareMetals.

Pathfinder Geology Lab
- Added converter to sift Dirt into ExoticMinerals and RareMetals. It isn't as efficient as the converter in the Clockworks but it gets the job done.

Bug Fixes
- The Greenhouse will now properly account for the current crewmember residing in the module.
- If you have Extraplanetary Launchpads installed, when making RocketParts, excess ScrapMetal will be dumped instead of halting RocketPart creation.
- The Smoke Pipe should inflate properly now, and its active cooling can be enabled/disabled.

Other
- Updated to latest CRP
- Updated to latest OSE Workshop.
- Adjusted maximum stored resources by module. Ponderosa/Casa: 2000, Doc: 3000, Hacienda/Homestead: 4000

0.9.3
- Increased the node size on all the base attachment nodes.

Chuckwagon
- Moved center of mass for the Chuckwagon to the center of the dome.
- Adjusted storage capacity after taking a closer look at the mesh's volume. No more Tardis Chuckwagon...

Bug Fixes
- You can now carry the Smoke Pipe.
- Some inflatable modules have resources that are kept even when switching templates. Now, those resources won't be affected by the part being inflated or deflated.

0.9.2

New Part
- Added the Smoke Pipe portable cooling tower. It is good at radiating heat. Be sure to keep it charged with electricity and fill it with water; it needs both.

Gold Digger
- Increased efficiency on the drill, but it now also generates heat. Be sure to plan accordingly.

Hacienda
- Added the Claw Marks Strip Miner. Credit goes to AdmiralTigerclaw for the concept, thanks AdmiralTigerclaw! :)

Hot Springs
- The Hot Springs now generates heat. Be sure to plan accordingly.

Sunburn Lab
- Added Ore, Water, and Minerals resources to help with conversions.

Pigpen
- Added Organics, Ore, and Minerals resources.

Flatbeds
- Removed KIS part mount module; it was only applicable to a small number of parts.
- You can now surface-attach parts to the flatbeds.

Buckboards
- Anybody can now stack Buckboards.

Other
- Added ability to switch between different resources to the stock shuttle and airliner wings.
- Increased breaking torque and breaking force on all base parts.
- Updated to KSP 1.0.5

Bug Fixes
- The TERRAIN satellite will now benefit from Antenna Range if installed.
- Added missing resources to the Brew Works.

0.9.1

This release has some hotfixes.

Watney Chemistry Lab
- Adjusted ElectricCharge output on the Rainmaker.
- Added a Rainmaker variant that creates Water from LiquidFuel and Oxidizer. It also produces ElectricCharge.

Bug Fixes
- Added collider to broken wheel mesh on the M1A1.

0.9.0

The Buffalo gets new wheels! The stock-based RoveMax M1A1 wheels were always interim placeholders, but now the Buffalo has its real wheels. The new M1A1 Mountain Goat is inspired by the wheels found on NASA’s MMSEV but they have been brought inline with the KSP art style. The current stock-based wheel will be going away. They are deprecated in this release (existing craft won’t break but you won’t find the wheels in the catalog) and it will be removed at a later date, so be sure to retrofit or retire your rovers that have the older wheels.

New Parts
- Added the Patio, a 3.75m muncrete slab that's useful for making landing pads and foundations for bases on uneven terrain. You can clip slabs together to form larger areas. To use it, bolt it into the ground NOTE: This is an experimental part, use at your own risk.

- Added the M1A1 Mountain Goat. The M1A1 can switch between a wide wheelbase for normal driving, and a narrow wheelbase to get in and out of cargo bays. And to top it off, it has sound effects if you have the Wheel Sounds mod installed. Thanks for the suggestion, Supermarine! :)

- Added the Tundra 200 and Tundra 400. These are conformal storage tanks for the Buffalo that are intended for ground and aerospace craft. Unlike the Wagon, they do not expand, but you can attach other parts to them.

Gold Digger
- Lowered EC requirements for the Gold Digger to reflect its lower efficiency compared to the stock Drill-O-Matic.

Buffalo Command Cab
- Increased storage slots slightly.

Interim Rover Wheel
- The existing stock-based rover wheel has been deprecated in favor of the new M1A1 Omni Wheel. Be sure to retrofit your rovers.

Other
- Moved the Mineshaft, Capstone, and Switchback to the Structural tab (they are still found under the Pathfinder category as well).
- Moved the part config files to the new Parts directoy, organized by tab. Textures and mesh files remain in the Assets folder.

Bug Fixes
- The greenhouse will now retain its resources when you switch away from the vessel and switch back.
- The Spyglass can now attach KAS pipes.
- The Watney Rainmaker no longer complains about a lack of Oxidizer or MonoPropellant (you might need to transfer some into the Doc).
- Fixed GeoEnergy resource distribution for Eeloo. Thanks for pointing it out, CyberFoxx! :)
- The Switchback's KIS storage capacity is now available from inside your base as well as during EVA.

0.8.9

This update brings some new parts, some more bug fixes, and minor tweaks. By popular request, the orbital construction docks from Multipurpose Colony Modules (MCM) are back as the Stockyard 250 and Stockyard 375. These parts need Extraplanetary Launchpads to function properly. To help with your orbital construction needs, the new Homestead provides the same industrial facilities as the Hacienda- minus the Hot Springs. For now these are the only orbital elements of Pathfinder; I have plans for other modules but they'll take awhile to build properly.

New Parts
- By popular request, added the Stockyard 250 and Stockyard 375. Use these parts to build vessels in orbit or recycle them.
- Added the Homestead, the orbital equivalent to the Hacienda. Unlike the Hacienda though, the Homestead lacks a Hot Springs configuration.

Spyglass
- The spotlight now rotates. :)

Clockworks
- Increased maximum part volume that can be produced to 14,500L, and increased maximum KIS storage to match it. You'll need to switch away from the Clockworks and switch back for the changes to take effect.

Buffalo Crew Cab
- Adjusted the default EVA airlock to the right side, just like the command cab.
- Adjusted the ladder colliders to make it easier to climb up.

Command Cab
- Adjusted the ladder colliders to make it easier to climb up.

Buffalo Chassis
- Increased the Chassis 2u KIS storage to 200L.
- Decreased the Chassis End Unit KIS storage to 80L.

Bug Fixes
- Fixed the rocket parts production in the Ironworks when Extraplanetary Launchpads isn't installed.
- Fixed the rocket parts to material kits production in the Ironwoks when OSE Workshop is installed.
- Fixed CLS issues with the Casa.
- Adjusted the costs of the Hacienda.

Other
- Updated to latest CommunityResourcePack and ModuleManager.

0.8.8

This update has a bunch of bug fixes- gotta reduce that technical debt!

Templates
- Added the XenonGas storage template.

Bug Fixes
- Fixed an issue preventing the repair of the Solar Flare and Hot Springs.
- Fixed an issue with converting the Buckboard MC-1000 and Outback into a battery.
- Fixed an issue with converting the Chuckwagon into a greenhouse.
- You can now reconfigure your modules again.
- Fixed an issue with Snacks where the inflatable modules weren't adjusting their maximum amounts correctly.
- Fixed an issue with the AuxEN that prevented AntennaRange from working correctly. NOTE: You'll need to switch our your resources for this change to take effect.
- The MC-3000 now has the correct amount of KIS storage when storing something other than KIS items. NOTE: You'll need to switch our your resources for this change to take effect.

0.8.7

Dynamically configuring Extraplanetary Launchpads continues to be a challenge. Unfortunately, once the workshops are set up, it takes reloading the scene to make changes. As a result it's not conducive to dynamic template switching, and the Fireworks template isn't going to help. The workaround is to add productivity to the Doc and Hacienda, so that all staffed base modules will contribute to at least some of the production. It also means that the Fireworks is now just a shell template; it will be removed next release, so now is a good time to switch it out for something else.

You'll also find a new part, the Captstone, to close off those unused crew ports. And there are some new alternate textures that don't have the module names on them. If you need to go anonymous during the end of the world, these new textures are for you. :) Speaking of textures, the Gold Digger has a new texture as well. Thanks for the feedback, Kottabos!

The Buffalo also gets some new additions; some mini RCS motors (with sound effects!) and a couple of animated arms (static animations, sorry) to help with asteroid exploration. There's also some new storage for the chassis end unit, and the graphics glitch in the crew cab has been fixed. Sorry, no custom wheels- yet.

Finally, this update changes how bases are built; You'll need Engineers equipped with power tools to assemble your base modules. This was my original intention, but at the time, I didn't know how to do that with KIS. To attach a module, simply drag it out of your inventory, press R to change the node to "mount" or "bottom" instead of surface-attach, and press and hold X to attach the module to the standard crew port. While it takes an engineer to assemble the base, anybody can still stack crates onto flatbeds and to each other (but you need an engineer to attach a Buckboard to the Saddle).

Base Modules
- Removed ModuleKISPartMount from the Casa, Chuckwagon, Doc, Hacienda, Ponderosa, and Saddle. These are no longer necessary, and might help with performance as well as avoid deadly Kraken attacks when you accidentally hit that Release button.

New parts
- Buffalo: Added the 5-Way Mini RCS Thruster. This is particularly helpful for exploration craft maneuvering around large objects like asteroids. It even has sound effects! You can change the volume or disable the sound effects by opening the Settings window. To do that, press Mod B simultaneously (Mod is the Modifier key, which defaults to Alt on Windows).

- Added the Buffalo Grappler Arm. Just like the stock Advanced Grabbing Unit, the Grappler Arm is useful for grabbing things like asteroids.

- Added the Asteroid Resource Composition Scanner (ARCS). This arm works like the stock Surface Scanning Module, but only for asteroids. However, it tells you the entire composition of an asteroid instead of just its Ore content. Simply right-click on the part and click Scan Asteroid to get its composition.

- Added the Capstone, a small part designed to put a cap on the exposed crew transfer ports.

Buffalo Crew Cab & Wagon
- The integrated solar panels are now more durable. If the solar panel breaks, so has your part.

Buffalo Command Cab
- Increased the crash tolerance slightly and made it consistent with the Wagon and Crew Cab.

Chassis End Unit
- Added a storage tank with some storage capacity.

Buckboards
- Buckboards show up in the Buffalo and Pathfinder parts category again.

Doc & Hacienda
- Added vessel productivity (for Extraplanetary Launchpads) to the Doc and Hacienda. As long as you staff them, you'll get worker's benefits.

Outback
- Increased storage capacity to 400L.

Templates
- Recalibrated TAC-LS storage containers to reflect containers native to TAC-LS
- Added the Organics template.

Textures
- Converted textures to dds now that things are stabilizing.
- Retextured the Gold Digger to make it easier to differentiate it from the stock Drill-O-Matic in the parts catalog.
- For the minimalists, you can find textures for the Ponderosa, Doc, and Hacienda without the module names in the Extras/ModuleTexturesNoNames folder (this is outside of the GameData folder).

Bug Fixes
- The Spyglass is now located on the Advanced Construction node.
- The Switchback is now located on the General Construction node.
- The Switchback can now be mounted to the Saddle. Don't forget your power tools...
- Updated part costs that were out of whack due to copying config files during development.
- Updated HotSprings Water requirements and capacity to their correct values.
- Increased the Wagon's storage slot capacity to 8x8.
- All Buckboards can now be mounted to the flatbeds.
- Fixed ModuleManager patches preventing converters from showing up when certain mods are installed.
- You can now see out of both windows in the crew cabin from either seat.

0.8.6

This release fixes a bunch of issues that cropped up in Pathfinder as well as the Buffalo. It also introduces new game mechanics for the Hot Springs geothermal plant- thanks for your input, AdmiralTigerclaw! :)

New Parts
- Added MC-2000 Buckboard and MC-3000 Buckboard. With colliders and glowing resource decals. ;)
- Added the Auxiliary Electronic Navigator (AuxEN). In the Old West, oxen were frequently used to pull wagons. The AuxEN doesn't pull anything, but it does the driving. The probe core is the same size as a Chassis 1u, and supports RemoteTech, AntennaRange, and kOS.
- Added an adapter that tapers from the Buffalo cab form factor to a 1.25m cylinder. 

Buffalo Command cab
- Widened the headlight beams and angled them down slightly for better ground operations.
- You can now toggle the headlights separately from the cabin lights. Both cabin lights and headlights will turn on when you tap on the Lights button.

Buffalo Crew Cab
- Reworked the crew cab to include doors and ladders on its sides.
- Added a small amount of inventory space.

Doc
- Added an ImpactTransform node to the 3D mesh to accomodate drills.

Ponderosa
- Renamed the radially attached Ponderosa to the Casa.
- Increased empty mass of the Ponderosa to 0.35t to reflect its contents.
- Added an ImpactTransform node to the 3D mesh to accomodate drills.

Hacienda
- Increased empty mass of the Hacienda to 0.5t.
- Added an ImpactTransform node to the 3D mesh to accomodate drills.

Clockworks
- Maximum storage will increase to 10k liters when you convert the Hacienda to become a Clockworks. This matches the maximum volume part that you can 3D print.

Fireworks
- Really added EL productivity to the Fireworks, and got rid of ExSurveyStation. There was a mixup when preparing the last release...

Brew Works
- Brought the Autobots in and transformed Xenon into XenonGas.

Hot Springs
- Refactored the power planet to require Water and GeoEnergy as resources before it can operate. The HotSprings has a dedicated "drill" to extract GeoEnergy, called the Geothermal Tap. Right-click on the Hacienda to use the Geothermal Tap. You can also drill for GeoEnergy using the Gold Digger and stock Drill-O-Matic. Simply right-click on the drill and modify the resource that it drills for. Not all planets are ideal for finding GeoEnergy. Check out GeoEnergyResource.cfg to see how this initial pass is set up. Below is a quick rundown:

Geologically Active Worlds
Eve
Kerbin (duh)
Laythe
Eeloo (!) See See http://io9.com/breaking-geologic-activity-has-been-detected-on-the-su-1718055390

Possibly Geologically Active. (% chance per save)
Moho (70%)
Mun (85%) See http://www.space.com/14632-moon-dead-geologic-activity-monitored.html
Duna (70%) See http://astrogeo.oxfordjournals.org/content/44/4/4.16.full
Ike (70%)
Val (70%)
Tylo (60%)

Not geologically active
Sun
Gilly
Minmus - wiki suggests that it's a captured comet
Jool
Bop
Pol
Dres

If a world is not listed, then it has a base 70% chance of having GeoEnergy.

Bug Fixes
- The Switchback's nodes are now in their correct places.
- The Buckboard MC-1000's decal glows again.
- Fixed file paths for several Buffalo parts so they'll show up again.
- Moved Trailer Hitch to Utility.
- Moved Chassis Decoupler to Structural.

0.8.5
- Fixes issues with the Buffalo. When KSP 1.0.5 comes out, the Buffalo will not be included in Pathfinder.

0.8.4
- Hotfix to add collider to the Buckboard.

0.8.3

This update brings with it some more bug fixes, play balance, and some new parts. The existing Ponderosa now has a near twin that is radially attached just like the Hacienda and Doc. Originally I wanted the Ponderosa to work this way, but couldn't figure out how to make the airlocks work properly. Now that I do know how to make them work, it might be time to retire the containerized Ponderosa. Anyway, since a radially attached Ponderosa needs something to attach to, I also created the Switchback, a 4-way base hub that also has a small amount of storage and a solar panel on its roof. Simply attach a Saddle to the ground and place the Switchback onto the Saddle. Due to some constraints on placing the crew ports so they line up with the inflatable modules, the Switchback does not inflate. Instead, it's assembled off-camera when you pull it from storage. Finally, to resolve an issue with Extraplanetary Launchpads, Pathfinder introduces the Spyglass Survey Module. The Spyglass provides a spot to monitor your vessel construction efforts.

Buffalo
The Buffalo is now located in its own dedicated directory under WildBlueIndustries, and has its own category in the parts catalog. While it continues to be bundled with Pathfinder for the time being, the Buffalo MSEV is now available separately as its own mod. You can download it here.

Buckboard
- Gave the MC-1000 Buckboard a facelift; it is also now a common asset for WildBlueTools, which means it's no longer in the Pathfinder category. You can find it under Utilities.

New Parts
- By popular request, added the radially attached Ponderosa. It works just like the existing containerized Ponderosa, but it is radially attached like the Doc and Hacienda.

- Added the Switchback 4-way hub. Unlike other base components, the Switchback is assembled off-camera once the kerbal pulls it out of storage.

- Added the Spyglass Survey Module. Right-click the part to show the Extrplanetary Launchpads UI. Yes, I do have plans to make the light rotate.

Templates
- Reduced output of the greenhouse to feed 2 kerbals for about 90 days. It just wasn't as fun with the greater yield, even though the math does support the higher output.
- Added ExWorkshop module to the Fireworks to improve base productivity, and removed ExSurveyStation since it wasn't responding well to being loaded dynamically.

Bug Fixes
- The Hacienda Operations Manager now correctly labels its operations manager as Hacienda Operations.
- Fixed an issue Where efficiency and extraction monitors were creating NREs while in solar orbit.
- Fixed an issue where the Gold Digger was creating NREs while in solar orbit.
- Fixed an issue where the hints window would keep showing up even though Pathfinder had already shown the hint.
- Fixed an issue where the Extraplanetary Launchpads window wouldn't show up (see Spyglass for the solution).

Other
- By popular request, added support for USI-LS. Thanks for the patch, badsector! :)

0.8.1: William Tell Overture

Buffalo

- Added ElectricCharge to the Buffalo Crew Cab and Wagon.
- The Buffalo Wagon now properly shows up in the Utility tab.

Module Manager Patches

- Added Antenna Range Module Manager patch by badsector. Thanks badsector & linuxgurugamer! :)

Bug Fixes

- The Recycle ScrapMetal converter (which requires Extraplanetary Launchpads) now requires 9.75 units of ScrapMetal to produce 1 unit of Metal. This accounts for conservation of mass and for density differences between Metal and ScrapMetal.

- Fixed an issue in the commercial science lab where it was incorrectly converting the lab's data into Funds and Reputation instead of its stored Science.
- Fixed an issue in the commercial science lab where it would remove the stored science before completing data transmission.
- The commercial science lab will only show the Publish Research and Sell Research buttons when in Career Mode.

0.8.0: Pathfinder Beta: Home On The Range

NOTE: This is a beta release, there are bound to be bugs.

This release Introduces the Buffalo Modular Space Exploration Vehicle (MSEV). It is based upon NASA's real-world Multi-mission Modular Space Exploration vehicle (MMSEV). Just like the NASA vehicle, the Buffalo can be configured as a rover, but it doesn't stop there. (Coming Soon!) The Buffalo can become an asteroid exploration craft, a munar lander, a cargo hauler, and more. This initial offering provides the components needed to build the "space truck" version of the Buffalo; future versions will offer in-space functionality. The best thing is that the Buffalo can be assembled by an engineer via KIS, and the vehicle is sized to fit into a standard 3.75m cargo bay!

The Buffalo components include:

Buffalo Command Cab: A command pod that seats two kerbals, with Stock IVA and ASET Props/Avionics-based IVA (highly recommended). It also has integrated ladders.
Buffalo Crew Cab: A crew cabin that seats two kerbals, also with IVA. It also has a rooftop solar panel.
Buffalo Wagon: A.K.A. the Wagon, this Buffalo Storage Container holds lots of resources when deployed. It is a semi-rigid container that is shaped like the Crew Cab, and it too has solar panels on its roof.
Chassis (1u): The chassis serves as the foundation for the Buffalo. It is sized to the same length and width of a KIS standard SM-62 storage container/Buckboard/Ponderosa. While it is initialy configured as a battery, the Chassis can store a variety of different resources once converted to storage.
Chassis (2u): Same as above, but it's twice as long.
Standard Flatbed (1u): Sized to be as wide as the Buffalo cabin, the flatbed is sized to hold 2 SM-62 containers/Buckboards/Ponderosas. While it takes an engineer to mount the flatbed onto a chassis, anybody can put cargo on the flatbed.
Standard Flatbed (2u): Same as above, but twice as long.
Wide Flatbed (1u): A wider version than the standard flatbed, the wide flatbed can hold 3 SM-62 containers/Buckboards/Ponderosas. It is sized to fit in a standard 3.75m cargo bay.
Wide Flatbed (2u): Same as above, but twice as long.
Trailer Hitch: This specialized "docking port" is used to connect trucks and trailers together. It also has an integrated ladder.
Chassis Decoupler: For those times when you need to mount a Buffalo to a rocket, the chassis decoupler fits the bill.
Interim Wheel: Based on the RoveMate M1, the Interim Wheel provides ground transportation while we wait for KSP 1.1 and its better wheel system.

Bug Fixes
- Fixed an issue with the stack node on the Mineshaft. Now you can use it to close the gap between Buffalo modules separated by trailer hitches!

---ACKNOWLEDGEMENTS

Eve: Order Zero graphic courtesy of Kuzztler and used with permission.

0.7.8

Templates
- Added LqdHydrogen/Oxidizer converter to the Watney.
- Added support for Connected Living Spaces.

Bug Fixes
- Fixed an issue where adding the commercial science lab could cause an issue if the part did not have a science lab. Thanks kerbas_ad_astra!
- Fixed an issue where the agency flag wasn't showing up.

0.7.7: The Last of the Mohicans - Top of The World
Another bug fix, this release fixes a couple of issues with lights and mesh switching and animation that occurred when you reverted back to launch. It also fixes an issue where you'd lose your Snacks when switching templates- if you have the Snacks mod installed. It also adds a new template to the Hacienda.

Templates
- Added the Brew Works ISRU Refinery to the Hacienda. Like the stock ISRU part, the Brew Works converts Ore into LFO, LiquidFuel, Oxidizer, and MonoPropellant. It also can refine Xenon.

Bug Fixes
- Fixed an issue in WBIAnimation and WBILight that would lose their settings when you reverted to launch.
- Fixed an issue where, if you have Snacks! installed and you reconfigure your modules, the modules would lose their Snacks. Kerbals should be happier now.

0.7.6: The Last of the Mohicans - Elk Hunt

This release brings with it some bug fixes (thanks ozraven!) as well as a new part: the Poncho! Think of buying 10 OX-STAT panels and gluing them together, except that they fold out and cost as much as 15 OX-STAT panels. It won't track the sun, but the Poncho will provide a lot of power for your fledgling base. The Sombrero circular solar array is still planned...

New Part
- Added the Poncho, a large folding solar array that cannot track the sun. But hey, you can mount it on the Buckboard, Ponderosa, Saddle, and SM-62.

Bug Fixes
- Fixed an issue where the drill switcher would show an empty window if the biome hasn't been unlocked yet.

Special thanks to ozraven:
- Fixes for reconfigure affordability check.
- Deflating discards resources that cannot be stored.
- Entire vessel's crew now searched for skill modifier calculation.
- Updated inflate/deflate logic to account for deflate confirmation.

0.7.5 The Last of the Mohicans

This update brings the last of Pathfinder's core base modules. Specifically, it updates the Chuckwagon and it adds optional life support templates that will appear if you have a life support mod installed. Pathfinder supports TAC Life Support (the gold standard of life support), and Snacks out of the box. Other life support options are possible; just read up on how to edit a template file on the wiki. You may also want to check out the MM_TACLS.cfg and MM_Snacks.cfg files for examples. Additionally, you can now generate power using the Hot Springs Geothermal Plant. Keep in mind that it's maintennance intensive, a bit finicky, and not 100% efficient on all worlds. You also have the Solar Flare Experimental Fusion Plant and it's also a bit ornery. Together the Hot Springs and Solar Flare represent the upper end of Pathfinder's power options- lower-tier options are in the works. Finaly, the Ponderosa Habitat is now more than just a placeholder... if you have kOS installed.

Chuckwagon
- You can now science the kraken out of the Chuckwagon and convert it into a greenhouse.
- Added front and side attachment points.
- Added temporary IVA. IVAs are on backorder.
- Added airlocks.

Templates
- You can now retool the Blacksmith to use RocketParts instead of MaterialKits, just like the Clockworks.
- More than just a habitat module, the Ponderosa template is also used as a command and control center. The template now provides support for kOS if you have that mod installed and if you've researched Precision Engineering. kOS is great for creating power management scripts; it works best with Action Groups Extended.
- Added the Pigpen Recycler to the Ponderosa. With a life support mod, it has some useful recyclers.
- The Geology Lab now shows the current efficiency improvements for Science, Industry, and Habitation- based resource processors for the current biome.
- Added the Hot Springs Geothermal Plant to the Hacienda. Hot Springs generates lots of power but it is maintennance intensive and can break down. Don't want it to break? No problem, just use the Settings window to make parts unbreakable. It's available after you've researched High-Powered Electrics.
- Added the Solar Flare Experimental Fusion Plant. It's available after you've researched Specialized Electrics. Like the Hot Springs, it too can break down or be set to unbreakable in the Settings window.
- Did a balance pass on the templates. Some weigh less and need less RocketParts than before, others are more expensive. Skilled Engineers give discounts to inflate/reconfigure modules.

ModuleManager Patches
- Relocated the MetallicOre, Metals, and Ironworks MM patches for Extraplanetary Launchpad to a central file called MM_ExtraplanetaryLaunchpads.cfg. This will make it easier to modify Pathfinder to support other production chains than the EL standard MetalOre->Metal->RocketParts.
- Added MM_TACLS.cfg and MM_Snacks.cfg patches to configure Pathfinder to use TACLifeSupport or Snacks, respectively.
- Added MM_CommercialSciLab.txt, an optional MM patch that will enable the Doc Commercial Science Lab's lab functionality on all parts with lab modules. Simply rename MM_CommercialSciLab.txt to MM_CommercialSciLab.cfg if you want to enable this functionality.

Bug Fixes
- Fixed an issue where the module info window would not properly show the template decal.
- Lights should now properly turn themselves off when deflating a part.
- You won't be able to turn on the lights when a module is deflated.
- Resource converter efficiencies are now being calculated properly.
- Fixed an issue where converting storage to a battery was reporting incorrect values.
- If you stack a box of RocketParts onto a Ponderosa and try to inflate it, the Ponderosa will now properly check to make sure it has enough parts.
- Drills will now remember the correct resource that you switched them to.

Recommended Mods
TACLifeSupport: http://forum.kerbalspaceprogram.com/threads/40667-1-0-2-TAC-Life-Support-v0-11-1-20-5Apr
Snacks!: http://forum.kerbalspaceprogram.com/threads/90841-1-0-2-Snacks!-Kerbal-simplified-life-support-v0-3-5
kOS Scriptable Autopilot System: http://forum.kerbalspaceprogram.com/threads/68089-1-0-4-kOS-Scriptable-Autopilot-System-v0-17-3-2015-6-27
Action Groups Extended: http://forum.kerbalspaceprogram.com/threads/74195-1-0-4-%28Jul09-15%29-Action-Groups-Extended-250-Action-Groups-in-flight-editing-Now-kOS-RemoteTech

0.7.0: The Magificent Seven
youtube: Magnificent Seven Theme

NOTE: Please pack up your base before applying this update, there were a lot of changes under the hood.
NOTE: Please be sure to update your OSEWorkshop to the very latest version.

For science! The Gold Digger generates experiment results while taking core samples. Those results are now worth more than just their Science value; you can use them to improve the efficiency of your habitation, science, and industry processors. In other words, creating RocketParts, MaterialKits, ResearchKits, Water, and others will be improved if you have good results when performing your soil analysis, metallurgic analysis, or chemical analysis in the Geology Lab. And with the Geology Lab’s new Biome Analysis, you can either use the research to improve your production abilities within the biome, or transfer the data to the new Doc Commercial Science Lab. It will cost you Science to perform a Biome Analysis, but you stand to gain much more.

New Parts
- Added the Doc Commercial Science Lab. This is a new multipurpose laboratory. With the Doc Commercial Science Lab, developed in partnership with the Rasta Engineering Group (thanks rasta013 for the lab configs!), you get a Mobile Processing Lab equivalent that can do more than just process experiments for Science over time; you can publish the research for Reputation, or sell the research for Funds. You can also reconfigure the lab into the Sunburn pellet lab, and produce FusionPellets and Coolant for your fusion needs. And finally, you can retool it into the Watney Chemistry Lab, named after a fictional character that finds creative survival solutions through chemistry.

- Added the Hacienda Inflatable Multipurpose Factory. The Hacienda meets your industrial needs by providing the Ironworks, where you can smelt Metals from MetallicOre and create RocketParts from Metals. If you have Extraplanetary Launchpads installed, then you'll also get the Fireworks Survey Station, where you can build vessels. Also you smelt Metal from MetalOre, and fabricate RocketParts from Metal instead of using MetallicOre and Metals. Be sure to bring some survey stakes and a mallet. And if you have OSE Workshop installed, then you'll get the Clockworks, which lets you 3D print parts in larger volumes than the Blacksmith, and you can retool the printers to use RocketParts instead of MaterialKits (or vice versa). Additionally, with OSE Workshop installed, the Ironworks lets you produce MaterialKits, and convert between MaterialKits and RocketParts.

Saddle
- Updated config file to reflect the latest KIS.

Storage Templates
- Added templates for Metals and MetallicOre. These will turn into Metal and MetalOre with Extraplanetary Launchpads installed.

Ponderosa & Buckboard
- Now you can stack the Ponderosa and Buckboard on top of each other via convienient stacking nodes.

Ponderosa Templates

- Geology Lab: Added the ability to generate a Biome Analysis. Be sure to carefully read the tooltip.
- The Geology Lab can use core samples to perform a soil analysis, metallurgic analysis, or chemical analysis. A Soil analysis improves production efficiency for life support (coming soon!), a metallurgic analysis improves fabrication processors (Ironworks), and a chemical analysis improves chemical processors (Watney lab, Sunburn lab).
- When unlocking a biome for the first time and generating Science in the process, the Geology Lab will use the stock experiment results dialog to display the fruits of your labor.
- The Geology Lab can uplink to an orbiting T.E.R.R.A.I.N. equipped satellite/station to check on its status and switch to the vessel as desired.

TERRAIN
- Added a "Review Data" button to review the Science collected by the TERRAIN. It will display the stock experiment results dialog.
- Added some feedback fields to the right-click menu.

Gaslight, Ponderosa, Chuckwagon
- Added ability to toggle the lights via Lights action group.

Bug Fixes
- Fixed an issue where you'd see "No need to reconfigure to the same module type" when switching templates.
- Fixed an issue with the Gold Digger's ability to know which tech node activates its ability to drill for resources.
- Fixed an issue preventing the Blacksmith from printing parts. (NOTE: You'll need the latest version of OSEWorkshop as well)

Recommended Mods
Extraplanetary Launchpads: http://forum.kerbalspaceprogram.com/threads/59545-1-0-4-Extraplanetary-Launchpads-v5-2-1

0.1.6: Neon Light

Need a light? The Gaslight Telescopic Lamppost has you covered. Plant it in the ground (if you can run into it and move it, it’s not planted properly), extend the pole, and turn on the lights. It has a small battery built in, but for continual use, be sure to plug it into your base using its built-in KAS ports. It even serves as a short-range omnidirectional antenna- thanks for the suggestion, MeCripp, hope you like the MC-16 communications link. :) Don’t like the color? Are the lights too bright? You can change them in the field through the light’s right-click menu. 

This update supports the OSE Workshop as a new template for the Ponderosa, and adds a new MaterialKits storage template as well. OSEWorkshop lets you 3D print individual parts like hammers, the Mk1 command pod, and even another Ponderosa. If you download OSE Workshop (I recommended the mod), you’ll be able to 3D print parts if you have enough MaterialKits.

In keeping with the mod's spirit of jurry-rigging what you need, you can now re-engineer the Buckboard and Outback into batteries. This is a revision to the battery template added last update. Additionally, you can now reconfigure drills to drill for different resources. You'll see this concept of jurry-rigging in other parts in the future.

Finally, there are some bug fixes, infrastructure changes, and integrated standard KAS ports to help keep your base’s part count down.

NOTE: Please retire your existing TERRAIN scanners (unless you're savy enough to open your save file and replace PhotoSupplies with ResearchKits).

New Parts
- Added the Gaslight Telescopic Lamppost. It comes with a 4-way KAS pipe junction.

Saddle
- Added a pair of integrated KAS pipes.

Outback
- If attached to a command pod that can hold crew, the Outback will have the ability to recharge its EVA Propellant.

Templates
- Added the Blacksmith 3D Print Shop template to the Ponderosa. This will only be available if you have OSE Workshop installed (I recommend the mod).
- Added a new storage template to hold MaterialKits.
- Renamed PhotoSupplies to ResearchKits, and changed the icon.
- Changed the icon for the Ponderosa Habitat.
- The Battery template's EC levels have been nerfed; The Outback holds 154EC, while the Buckboard holds 611EC (comparable to a stack of three Z-200 batteries). Additionally, you'll need RocketParts and an Engineer to convert the Outback/Buckboard into batteries (which of course can be turned off using the Settings window). To convert the Buckboard/Outback into a battery, right-click the part to open the action menu, and press the "Convert to battery" button. You can convert the Buckboard/Outback back into a storage container as well.

Other
- Created a wiki page describing how to add a template to the Ponderosa: https://github.com/Angel-125/Pathfinder/wiki/Anatomy-Of-A-Template

Bug Fixes
- Fixed an issue where the Buckboard would incorrectly show a ToggleAnimation button during EVA.

Recommended Mods
OSE Workshop: http://forum.kerbalspaceprogram.com/threads/108234-1-0-2-OSE-Workshop-MKS-KIS-Addon-%28v0-7-3-2015-06-01%29

0.1.5: Bonanza

Orbiting satellites are great for detecting resources from space, but advanced sensors take a long time to research. With this update, Pathfinder introduces resource scanning tech earlier in the game- assuming it works. Additionally, the Geology Lab can do things that the stock surface scanner can- and more if you staff it right. Finally, the new Outback gives you a handy way to haul small amounts of resources around without lugging the Buckboard.

New Parts
- Added the T.E.R.R.A.I.N. Geo Scanner. If it breaks down, you'll have to repair it. Don't want it to break? You can change it in the settings menu. 
- Added the Outback Extravehicular Support Pack (ESP). It's great for hauling a small amount of resources around, and it's EVA friendly. It can be attached to the exterior of a vessel by pressing the "H" key, and detached using the "G" key. Anybody can use it.

Ponderosa
- Cleaned up the Ponderosa right-click menu and moved a lot of functionality to the Ponderosa Operations window. Access it via the Manage Operations button in the right-click menu.

Pathfinder Geology Lab
- The Geology Lab is now controlled through the Operations window. Simply right-click on the Ponderosa, press the Manage Operations button, and press the Show button to manage the Geology Lab.
- The Geology Lab can now perform a surface analysis of the biome if properly staffed.
- If you have the Impact mod (I recommend it), the Geology Lab can lend its seismometer to the cause if properly staffed.

Templates
- Added the Prime Flux Battery template to the Buckboard and Outback, named after Prime Flux, who suggested the idea. Thanks Prime Flux! :)

ToolTips & Settings
- Added a tooltip to remind players that the requirement to pay for redecorating/inflating the Ponderosa can be turned off.
- The Settings window can now be toggled using the modifier key (which defaults to the Alt key on Windows) plus P instead of being hard coded to the Alt key.

Resources & Storage
- Added the PhotoMaterials resource for taking pictures.
- Adjusted the Buckboard's storage capacity when not being used to store KIS items.
- You can now access the Chuckwagon's inventory from inside the module.

KIS/KAS
- Updated the saddle to account for the latest changes in KIS.
- Thanks to the latest version of KIS, non-engineers can attach the drill to a vessel with the "H" key and grab it with the "G" key.

Recommended Mod
Impact: http://forum.kerbalspaceprogram.com/threads/114087-1-0-Impact!-impact-science-and-contracts-v1-1-0-With-Asteroids-30-6-15

0.1.4: Raw Hide

Developer Notes: When I started working on Multipurpose Colony Modules, I always intended to have the player spend resources to reconfigure the module. Its successor, Pathfinder, finally realizes that vision. Similarly, certain templates require a specific skill to reconfigure the module into its new configuration.

The new requirements add a new challenge to the gameplay without overly complicating the system. But as always, it’s your game, your choice, so you can disable these requirements if you prefer not to play with them. Simply press Alt P to bring up the Pathfinder settings window, and uncheck the box next to “requires resources to reconfigure.” If you uncheck the box, the Ponderosa won’t require resources to inflate and outfit the module either. Similarly, you can disable the skill requirement.

IMPORTANT NOTE: The directory structure has changed. Please delete the WildBlueIndustries folder before installing this update.
NOTE: Please pack up your Ponderosas before applying this patch.

- The Ponderosa now requires RocketParts to inflate the module and to reconfigure it. Be sure to have an ample supply of RocketParts on your vessel. If you prefer to not use this feature, simply press Alt P to bring up the Pathfinder Settings window to disable it.
- Added the Conestoga Multipurpose Logistics Module (MLM). The Conestoga holds a lot more stuff than the Buckboard, but it’s not very hand-portable.
- Added the Mineshaft Portable Crew Tube (PCT). Mount one on each Ponderosa that you want to connect to, and then link them together, just like the KAS pipe.
- Added the Ponderosa Habitat template to the Ponderosa. It will be helpful in the future, but right now it's decorative.

0.1.3: Dead Or Alive

NOTE: Please pack up your base before applying this patch.
- Adjusted the Buckboard storage capacity to something more reasonable and carryable.
- Removed attach_node from the Saddle; I think that's causing the Kraken to take notice.
- You can now access the Ponderosa's inventory from inside the module as well as during EVA.

0.1.1: Blaze Of Glory

Want to camp out? Then take along the Ponderosa Inflatable Crew Module (ICM)! First bolt a Saddle into the ground, then saddle up the Ponderosa. The ICM comes outfitted with the Pathfinder Geology Lab, but other configurations are possible. And if you need supplies, don't forget to bring along the Buckboard shipping container.

- Added the Saddle Mini-Slab. Attach this to the ground.
- Added the Ponderosa Inflatable Crew Module with Pathfinder Geology Lab.
- Added the MC-1000 Buckboard, a container able to hold a variety of different resources.

0.1.0: Save My Soul

NOTE: This is a technology demo As such, parts and functionality are subject to change.

Do you have what it takes to be a prospector? Then step right up and try your luck with the Gold Digger Portable Mini-Drill! Take core samples for science! Analyze the results! Don't eat the samples.

- Introducing the Gold Digger Portable Mini-Drill.

---ACKNOWLEDGEMENTS

Module Manager by saribian
Portions of this codebase include source by Snjo and Swamp-IG, used under CC BY-NC SA 4.0 license
Icons by icons8: https://icons8.com/license/
Eve: Order Zero graphic courtesy of Kuzztler and used with permission.

---LICENSE---
Art Assets, including .mu, .mbm, and .dds files are copyright 2014-2016 by Michael Billard, All Rights Reserved.

Wild Blue Industries is trademarked by Michael Billard. All Rights Reserved.
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