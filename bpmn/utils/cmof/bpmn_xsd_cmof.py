#!/usr/bin/env python3

import xsd_parser, xsd_print_parsed, xsd_builder, xsd_print_model, \
       cmof_parser, cmof_model_builder, cmof_print_model, \
       find_matching, print_matching

from utils import Output, ErrOutput

import sys
import xml.etree.ElementTree as ET


def main():
    options = parse_options(sys.argv)

    do_work(options)

    print ('')
    print ("O.K.")


def do_work(options):
    out = Output(sys.stdout)
    err = ErrOutput(sys.stdout)
    print_opts = print_options_from_options(options)

    xsd_model = read_xsd_model(err)

    if options.print_xsd:
        xsd_print_model.print_model(out, xsd_model)
        return

    cmof_model = read_cmof_model(err)

    if options.print_cmof:
        cmof_print_model.print_model(out, cmof_model, print_opts)
        return

    matching = find_matching.match_all(xsd_model, cmof_model, err)
    print_matching.print_xsd_cmof_matching(out, matching)



def read_xsd_model(err):
    b = xsd_builder.create_builder(err)

    files = [
        'BPMN20.xsd', 'Semantic.xsd'
    ]

    for f in files:
        tree = read_xml(f)
        t = xsd_parser.parse_xsd(tree, err)
        xsd_builder.process_parsed_xsd(b, t)

    return xsd_builder.build_model(b)


def read_cmof_model(err):
    filename = 'BPMN20.cmof'
    tree = read_xml(filename)

    t = cmof_parser.parse_cmof(tree, err)

    return cmof_model_builder.build_model(t, err)


def read_xml(filename):
    tree = ET.parse(filename)
    return tree



def print_options_from_options(options):
    print_props = options.print_properties
    print_assoc = options.print_associations
    opts = cmof_print_model.create_print_options(print_props, print_assoc)
    return opts



class Options (object):

    __slots__ = [
        'print_xsd',
        'print_cmof',
        'print_properties',
        'print_associations'
    ]

    def __init__(self):
        self.print_xsd = False
        self.print_cmof = False
        self.print_properties = False
        self.print_associations = False



def parse_options(argv):
    opts = Options()
    i = 1
    n = len(argv)
    while i < n:
        s = argv[i]
        i += 1
        if s in ['-?', '-h']:
            usage()
        elif s == '-xsd':
            opts.print_xsd = True
        elif s == '-cmof':
            opts.print_cmof = True
        elif s == '-a':
            opts.print_associations = True
        elif s == '-b':
            opts.print_properties = True
        else:
            print ("Error: unknown option " + repr(s))
            usage()

    return opts


def usage():
    print ("Usage: bpmn_xsd_cmof.py [<options>]")
    print ("   options:")
    print ("     -xsd    - print XSD model")
    print ("     -cmof   - print CMOF model")
    print ("     -b      - display also properties of attributes in CMOF model")
    print ("     -a      - display also associations in CMOF model")
    sys.exit(1)


if __name__ == '__main__':
    main()

