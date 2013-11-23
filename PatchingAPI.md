The SharpTune rom patching command line API is defined by SharpTune.RomMod.RomMod.

The API for mod metadata used in patch creation and application is defined in SharpTune.RomMod.Mod

The API for mod definition creation is defined in SharpTune.RomMod.ModDefinition

A working example can be observed in the MerpMod project's Metadata.c and Definition.c files at https://github.com/Merp/MerpMod.

The SharpTune rom patching API consists of three main functions; Target Rom Selection, Patch file creation, and Patch file application.

A typical workflow using Renesas HEW:
Target Rom Selection
Compile patch in HEW
Patch File creation
Patch File application
HEW Debugging


Target Rom Selection
====================

Description:

Target rom selection is the process of reading definition data from a .map file and normalizing this data for use with the MerpMod source.

Phases:

Receive command line arguments from Target Selection phase of HEW build
EcuMapTool updates target rom's header and linker files using .map input file and idatohew.xml

Details:

Command line arguments are sent to SharpTune using a custom build phase in HEW. This build phase calls TargetSelection.bat and passes the target rom calibration ID using environment variables.
TargetSelection.bat then calls SharpTune.exe ecumaptool and provides the appropriate input filenames.
The target rom's header and map files are loaded, where all of the defines are unmarshalled to objects.
Definitions from the map file are then translated using idatohew.xml. This process converts nomenclature used to describe definitions in the map file to a nomenclature compatible with MerpMod. Users may alter this to suit their naming conventions by adding their own 'ida name' elements, being careful to observe the example below.
Tools are also provided in the SharpTune GUI to create IDC scripts so users may normalize their naming conventions. 
During this process, the HEW output window will display entries of xmltoidc that have not been defined for debugging purposes.
Normalized definition data from map files will override header file defines, but header file defines are otherwise persistent and entries must be removed manually.

Example idatohew.xml entry:
<define name="pFlagsRevLim" type="unsigned char*">
        <ida name="pFlagsRevLimit" />
	<ida name="REVLIM_FUELCUT_FLAGS" priority="2" />
	<ida name="ANOTHER_NAME_FOR_THIS" priority="3" />
</define>
*note: if multiple ida name candidates are provided, priority tags must be set this way! It is a bit crude but it works.

Idatohew.xml also handles the linker script which creates linker sections.
In this case, multiple candidates are not used. The necessary locations (alias names in the xml) must be defined in the map file as they are named in idatohew.xml.


Patch File Creation
===================

Patch file creation consists of the following phases:
Input partial-patch.mot s-record file from HEW
Read metadata section of partial patch
Construct individual patches
Add baseline data to individual patches
Check individual patches
Output complete mod patch file
Read definition metadata from patch file
Create definition files

The metadata API can be easily inferred from the MerpMod project's Metadata.c. Further details are in SharpTune.RomMod.Mod

The definition data API can be easily inferred from the MerpMod project's Definition.c. Further details are in SharpTune.RomMod.ModDefinition

Patch File Application
======================
Check baseline data of patches against device image (ROM)
Apply patches to a copy of the device image
Check patches
Output new device image

