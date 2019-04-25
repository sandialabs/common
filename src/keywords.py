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
excluded_file_extensions = [".exe", ".dll", ".pdf", ".pdb", ".png", ".jpg", ".jpeg", ".ttf", ".woff", ".woff2", ".cache", ".resources", ".lut", ".baml", ".ico", ".pxm", ".js", ".user", ".map"]
excluded_directories = [".git", ".nuget", ".vs", "bin", "obj", "node_modules", "bower_components"]

# Make sure we don't search in this file itself since it contains all the keywords we report on.
# We'll use this as we're checking the filenames in the directories
keywords_file = Path("keywords.txt").resolve()

def recurse_path(value, file_count):
    path = Path(value)

    for item in path.iterdir():
        current = item.resolve()
  
        if current.is_dir() and not any(filter(lambda dir: current.name == dir, excluded_directories)):
            recurse_path(current, file_count)
        elif current.is_file() and not any(filter(lambda ext: current.suffix == ext, excluded_file_extensions)) and current != keywords_file:
            results = get_keywords_in_file(current)

            if len(results) > 0:
                print("Found " + str(results) + " in " + str(current))
                file_count += 1

def get_keywords_in_file(file):
    results = []
    try:
        with file.open() as source:
            text = source.read().casefold()
            results = list(filter(lambda r: r != False, map(lambda keyword: keyword if keyword in text else False, keywords)))    ### changed this from None to False
    except:
        pass

    return results

def load_keywords():
    if keywords_file.exists():
        with open(keywords_file) as k:
            lines = k.readlines()
            global keywords
            keywords = [line.rstrip('\n').casefold() for line in lines]
    else:
        print("Keyword file " + str(keywords_file) + " doesn't exist")
        exit(1)

def main():
    load_keywords()

    file_count = 0
    recurse_path(".", file_count)

    if file_count > 0:
        exit(1)

main()
