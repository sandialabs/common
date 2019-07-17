#!/usr/bin/python3

import sys
import os
import shutil
import re
import codecs
import datetime

av = "[assembly: AssemblyVersion("
afv = "[assembly: AssemblyFileVersion("
ac = "[assembly: AssemblyCopyright("

p = re.compile("\"(\d+\.\d+\.\d+)(.\d+)?\"")
version = ""
revision = None
year = str(datetime.datetime.now().year)

def main(argv):
    if len(argv) < 2:
        show_usage()
        return

    global version, revision

    if argv[1] == "-r" and len(argv) >= 3:
        revision = argv[2]

        print ("Setting revision to " + revision)

    if revision == None:
        # There's a limit of 65k for each segment of the
        # version string, so go through each segment and
        # mask out everything above that, then reassemble.
        segments = argv[1].split(".")
        for index, segment in enumerate(segments):
            segments[index] = str(int(segment) % 0x0000FFFF)

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
                    walkFile(path)

def fixBuild(prefix):
    return prefix + "\"" + version + "\")]\r\n"

def fixRevision(prefix, line):
    m = p.search(line)
    if m:
        v = m.group(1) + "." + revision
        # print ("Changing " + m.group(0) + " to " + v)

        line = prefix + "\"" + v + "\")]\r\n"
    else:
        print ("Didn't match: " + line)

    return line

def walkFile(file):
    print (" - " + file)

    lines = []
    newLines = []
    with codecs.open(file, 'r', encoding="utf-8") as f:
        lines = f.readlines()

    for line in lines:
        # Could use regular expressions here, but this is
        # probably faster and simpler
        if line.startswith(av):
            if revision == None:
                line = fixBuild(av)
            else:
                line = fixRevision(av, line)
        elif line.startswith(afv):
            if revision == None:
                line = fixBuild(afv)
            else:
                line = fixRevision(afv, line)
        elif line.startswith(ac):
            line = u"[assembly: AssemblyCopyright(\"Copyright © Sandia National Laboratories " + year + "\")]\r\n"
        # else:
        #     print ("Didn't match: " + line)

        newLines.append(line)

    with codecs.open(file, 'w', encoding="utf-8") as f:
        f.writelines(newLines)

def show_usage():
    print ("python set_version <version> | [-r <revision>]")
    print (" where <version> is like x.y.z[.r]")
    print ("  x is major")
    print ("  y is minor")
    print ("  z is build")
    print ("  r is revision (optional)")
    print (" or <revision> represents r")
    print ("  the existing x.y.z will be maintained, and will")
    print ("  be followed by .<revision>")

main(sys.argv)