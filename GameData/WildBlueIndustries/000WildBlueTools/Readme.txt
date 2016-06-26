Wild Blue Tools

A KSP mod that provides common functionality for mods by Wild Blue Industries.

---INSTALLATION---

Copy the contents of the mod's GameData directory into your GameData folder.

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