#!/usr/bin/python3

# Gets all files below where this script is run, and checks to see if any contain
# the specified keywords. A report is generated at the end.
#
# Files with certain extensions are excluded, and directories with certain names are excluded.
#
# The actual keywords are kept in a separate file so it can be protected. The keywords are
# just put on a single keyword per line in the file.

from pathlib import Path

keywords = []
excludedFileExtensions = [".exe", ".dll", ".pdf", ".pdb", ".png", ".jpg", ".jpeg", ".ttf", ".woff", ".woff2", ".cache", ".resources", ".lut", ".baml", ".ico", ".pxm", ".js", ".user", ".map"]
excludedDirectories = [".git", ".nuget", ".vs", "bin", "obj", "node_modules", "bower_components"]
filesWithKeywords = 0

# Make sure we don't search in this file itself since it contains all the keywords we report on.
# We'll use this as we're checking the filenames in the directories
keywordFile = Path("keywords.txt").resolve()

def recurse_path(value):
    path = Path(value)

    for item in path.iterdir():
        current = item.resolve()
  
        if current.is_dir() and not any(filter(lambda dir: current.name == dir, excludedDirectories)):
            recurse_path(current)
        elif current.is_file() and not any(filter(lambda ext: current.suffix == ext, excludedFileExtensions)) and current != keywordFile:
            results = getKeywordsInFile(current)

            if len(results) > 0:
                print("Found " + str(results) + " in " + str(current))
                global filesWithKeywords
                filesWithKeywords += 1

def getKeywordsInFile(file):
    results = []
    try:
        with file.open() as source:
            text = source.read().casefold()
            results = list(filter(lambda r: r != False, map(lambda keyword: keyword if keyword in text else False, keywords)))    ### changed this from None to False
    except:
        pass

    return results

def loadKeywords():
    if keywordFile.exists():
        with open(keywordFile) as k:
            lines = k.readlines()
            global keywords
            keywords = [line.rstrip('\n').casefold() for line in lines]
    else:
        print("Keyword file " + str(keywordFile) + " doesn't exist")
        exit(1)

def main():
    loadKeywords()

    recurse_path(".")

    if filesWithKeywords > 0:
        exit(1)

main()
