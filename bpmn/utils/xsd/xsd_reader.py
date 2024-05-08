#!/usr/bin/env python3

import xsd_parser, xsd_print_parsed, xml_print_raw
from utils import Output, ErrOutput

import sys
import xml.etree.ElementTree as ET


def main():
    options = parse_options(sys.argv)

    out = Output(sys.stdout)
    err = ErrOutput(sys.stdout)

    fname = options.filename
    if fname is None:
        inp = sys.stdin
    else:
        inp = open(fname)

    process_file(out, err, options, inp)

    print ('')
    print ("O.K.")


def process_file(out, err, options, inp):
    tree = read_xml(inp)

    if options.print_raw:
        xml_print_raw.print_tree_raw(out, tree)
        return

    t = xsd_parser.parse_xsd(tree, err)

    xsd_print_parsed.print_all(out, t)

    # if options.print_parsed:
    #     cmof_print_parsed.print_all(out, t)
    #     return

    # print ("Building model...")

    # model = cmof_model_builder.build_model(t, err)
    # cmof_print_model.print_model(out, model)


def read_xml(filename):
    tree = ET.parse(filename)
    return tree




class Options (object):

    __slots__ = [
        'filename',
        'print_raw',
        'print_parsed'
    ]

    def __init__(self):
        self.filename = None
        self.print_raw = False
        self.print_parsed = False



def parse_options(argv):
    opts = Options()
    i = 1
    n = len(argv)
    while i < n:
        s = argv[i]
        i += 1
        if s in ['-?', '-h']:
            usage()
        elif s == '-r':
            opts.print_raw = True
        elif s == '-p':
            opts.print_parsed = True
        # elif s == '-m':
        #     opts.print_parsed = False
        else:
            if s.startswith('-'):
                print ("Error: unknown option " + repr(s))
                usage()
            else:
                if opts.filename is None:
                    opts.filename = s
                else:
                    print ("Error: only one filename allowed (" + repr(s) + ")")
                    usage()
    return opts


def usage():
    print ("Usage: xsd_reader.py [<options>] <filename>")
    print ("   options:")
    print ("     -r  - print raw XML")
    print ("     -p  - print parsed XSD")
    # print ("     -m  - print constructed model")
    sys.exit(1)



if __name__ == '__main__':
    main()

