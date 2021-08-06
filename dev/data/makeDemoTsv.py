"""
Create a thin TSV file for testing from randomly sampled
lines from a full-size TSV input file.
"""

from os import linesep
import random
import pathlib

PATH_HERE = pathlib.Path(__file__).parent.resolve()

if __name__ == "__main__":
    inLines = PATH_HERE.joinpath("abfs.tsv").read_text().splitlines()
    randomLineIndexes = [random.randint(1, len(inLines)) for x in range(1234)]
    outLines = [inLines[x] for x in set(randomLineIndexes)]
    outLines.insert(0, inLines[0])
    PATH_HERE.joinpath("demo.tsv").write_text("\n".join(outLines))
