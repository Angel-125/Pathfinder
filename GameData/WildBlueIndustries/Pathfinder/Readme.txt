Pathfinder

A KSP mod that blazes the trail for more permanent installations. Geoscience for better resource extraction.

---INSTALLATION---

Copy the contents of the mod's GameData directory into your KSP's GameData folder.

---REVISION HISTORY---

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
Community Resource Pack by RoverDude, Nertea, and the KSP community
Portions of this codebase include source by Snjo and Swamp-IG, used under CC BY-NC SA 4.0 license
Icons by icons8: https://icons8.com/license/
Eve: Order Zero graphic courtesy of Kuzztler and used with permission.

---LICENSE---

Source code copyrighgt 2015, by Michael Billard (Angel-125)
License: CC BY-NC-SA 4.0
License URL: https://creativecommons.org/licenses/by-nc-sa/4.0/
Wild Blue Industries is trademarked by Michael Billard and may be used for non-commercial purposes. All other rights reserved.
Note that Wild Blue Industries is a ficticious entity 
created for entertainment purposes. It is in no way meant to represent a real entity.
Any similarity to a real entity is purely coincidental.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.