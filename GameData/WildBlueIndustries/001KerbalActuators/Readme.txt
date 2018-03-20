KerbalActuators

A KSP mod that provides several part modules to manipulate various mesh transforms in a part in order to animate it. Modules include:

WBIMagnetController: Primarily used by robot arms to grab parts and move them around.
WBILightController: Controls a light.
WBIRotationController: Rotates mesh transforms to help animate a robot arm.
WBIServoManager: Added after all of the above part modules in a config file, the servo manager coordinates all the controllers. It provides GUI controls to manipulate the controllers. You can create, load, and save a series of servo snapshots into a sequence and then play back the snapshots to animate a part.

WBIHoverController: Provides hover management to engines.
WBIPropSpinner: Spins props for propeller-driven engines.
WBIVTOLManager: Added after all of the above, the VTOL Manager handles hover management and can provide GUI controls for WBIRotationController.

WBIAirParkController: Enables a vessel to "part" in mid air and be treated as if landed. It's finicky but it works...

For a description of the API, go to https://github.com/Angel-125/KerbalActuators/wiki

---INSTALLATION---

Copy the contents of the mod's GameData directory into your GameData folder.

---REVISION HISORY---

1.2.0
- Added WBIMagnetController and WBILightController.
- Added XML document comments to all the modules.

---ACKNOWLEDGEMENTS---

WBIMagnetController incorporates code by sirkut. MUCH appreciated, sirkut! :)

---LICENSE---
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