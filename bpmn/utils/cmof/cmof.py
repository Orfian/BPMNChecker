#!/usr/bin/env python3

import cmof_parser, cmof_print_parsed, cmof_model_builder, cmof_print_model, cmof_print_csharp
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
        print_tree_raw(tree)
        return

    t = cmof_parser.parse_cmof(tree, err)

    if options.print_parsed:
        cmof_print_parsed.print_all(out, t)
        return

    print ("Building model...")

    model = cmof_model_builder.build_model(t, err)

    if options.print_csharp: 
        cmof_print_csharp.print_model(out, model)
    else: 
        cmof_print_model.print_model(out, model)


def read_xml(filename):
    tree = ET.parse(filename)
    return tree


def print_tree_raw(tree):
    root = tree.getroot()
    print_subtree (root, 0)


def print_subtree(node, level):
    sp = ' ' * (level * 4)
    print (sp + "Node " + repr(node.tag) + " {")
    sp2 = sp + "    "
    # # print (sp2 + "attrs = " + repr(node.attrib))
    print (sp2 +"Attrs:")
    for attr in node.attrib:
        print (sp2 + "   " + repr(attr) + ": " + repr(node.attrib[attr]))
    # print (sp2 + "text = " + repr(node.text))
    for child in node:
        print ('')
        print_subtree(child, level + 1)
    print (sp + "}")
    # print (sp + "tail = " + repr(node.tail))




class Options (object):

    __slots__ = [
        'filename',
        'print_raw',
        'print_parsed',
        'print_csharp'
    ]

    def __init__(self):
        self.filename = None
        self.print_raw = False
        self.print_parsed = False
        self.print_csharp = False


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
        elif s == '-csharp':
             opts.print_csharp = True
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
    print ("Usage: cmof.py [<options>] <filename>")
    print ("   options:")
    print ("     -r         - print raw XML")
    print ("     -p         - print parsed CMOF")
    print ("     -csharp    - print constructed model in C#")
    sys.exit(1)



if __name__ == '__main__':
    main()

