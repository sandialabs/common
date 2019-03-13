#!/usr/bin/python3

import sys
import os
import shutil
import re
import codecs

av = "[assembly: AssemblyVersion("
afv = "[assembly: AssemblyFileVersion("
ac = "[assembly: AssemblyCopyright(\""

def main(argv):
    if len(argv) < 2:
        show_usage()
        return

    # There's a limit of 65k for each segment of the
    # version string, so go through each segment and
    # mask out everything above that, then reassemble.
    segments = argv[1].split(".")
    for index, segment in enumerate(segments):
        segments[index] = str(int(segment) % 0x0000FFFF)

    version = ""
    for segment in segments:
        if version != "":
            version += "."
        version += segment

    print ("Updating to " + version)

    for root, dirs, files in os.walk("."):
        for file in files:
            if file.endswith("AssemblyInfo.cs"):
                path = os.path.join(root, file)
                # Make sure we don't change anything inside the
                # NuGet packages folder. May want to revisit this
                # to make it more flexible.
                if path.startswith(".\\packages") == False and path.startswith(".\\HTMLRenderer") == False:
                    setVersion(path, version)

def setVersion(file, version):
    print (" - " + file)

    lines = []
    newLines = []
    with codecs.open(file, 'r', encoding="utf-8") as f:
        lines = f.readlines()

    for line in lines:
        # Could use regular expressions here, but this is
        # probably faster and simpler
        if line.startswith(av):
            line = av + "\"" + version + "\")]\r\n"
        elif line.startswith(afv):
            line = afv + "\"" + version + "\")]\r\n"
        elif line.startswith(ac):
            line = u"[assembly: AssemblyCopyright(\"Copyright © Sandia National Laboratories 2019\")]\r\n"
        
        newLines.append(line)

    with codecs.open(file, 'w', encoding="utf-8") as f:
        f.writelines(newLines)

def show_usage():
    print ("python set_version <version>")
    print (" where <version> is like x.y.z[.r]")
    print ("  x is major")
    print ("  y is minor")
    print ("  z is build")
    print ("  r is revision (optional)")

main(sys.argv)