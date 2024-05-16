#!/usr/bin/env python3

import cmof_parser, cmof_print_parsed, cmof_model_builder, cmof_print_model, \
       xml_print_raw, list_classes, cmof_print_csharp
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

    t = cmof_parser.parse_cmof(tree, err)

    if options.print_parsed:
        cmof_print_parsed.print_all(out, t)
        return

    print ("Building model...")

    model = cmof_model_builder.build_model(t, err)

    if options.print_csharp:
        file = Output(open(r"D:\TACR\bpmnchecker\CSharpChecker\BPMNModel\model\File.cs", "w"))
        cmof_print_csharp.print_model(file, model)

    elif options.list_classes:
        pckg_name = model.get_package_name()
        if pckg_name != "BPMN20":
            err.error("Listing of classes requires 'BPMN20', obtained " +
            repr(pckg_name) + ".")
        list_classes.print_list(out, model)
    else:
        cmof_print_model.print_model(out, model)


def read_xml(filename):
    tree = ET.parse(filename)
    return tree




class Options (object):

    __slots__ = [
        'filename',
        'print_raw',
        'print_parsed',
        'list_classes',
        'print_csharp'
    ]

    def __init__(self):
        self.filename = None
        self.print_raw = False
        self.print_parsed = False
        self.list_classes = False
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
        elif s == '-l':
            opts.list_classes = True
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
    print ("     -l  - list of classes")
    print ("     -csharp    - print constructed model in C#")
    sys.exit(1)



if __name__ == '__main__':
    main()

