#!/usr/bin/python3

import sys
import os
import shutil
from subprocess import call

fullVersion = "1.5.2"
commonInstaller = "COMMONInstaller-" + fullVersion + ".aip"
commonPrerequisitesInstaller = "COMMONPrerequisitesInstaller-" + fullVersion + ".aip"
ai = "C:/Program Files (x86)/Caphyon/Advanced Installer 16.0/bin/x86/AdvancedInstaller.com"
installers = {
	"common": commonInstaller,
	"commonpre": commonPrerequisitesInstaller,
}

def main(argv):
	if len(argv) < 2:
		show_usage()
		return

	args = []
	version = ""
	for i in range(1, len(argv)):
		arg = argv[i];
		# print (arg)
		if arg == "-v":
			version = argv[i + 1]
		elif arg in installers:
			args.append(arg)
		elif arg == "all":
			args.extend(installers.keys())

	if len(args) == 0:
		print ("No installers defined")
	else:
		for arg in args:
			build(arg, version, fullVersion)

def build(arg, version, fullVersion):
	print("--------------------")
	print("Building: " + arg + " -- " + fullVersion + "." + version)
	# We can edit the version of the product, but only the first 3 (x.y.z) are used
	# so this doesn't really have any effect. That's a Windows Installer limitation:
	# https://docs.microsoft.com/en-us/windows/desktop/Msi/productversion
	# call([ai, "/edit", installers[arg], "/SetVersion", fullVersion])
	call([ai, "/rebuild", installers[arg]])
	# consolidate(installers[arg])

def show_usage():
	print ("Usage:")
	print (" build.py [apps] [-v version]")
	print ("  where [apps] is one or more of the following")
	for installer in installers.keys():
		print ("   " + installer)
	print (" or")
	print ("  all -- to build everything")
	print (" version is the build version")

main(sys.argv)