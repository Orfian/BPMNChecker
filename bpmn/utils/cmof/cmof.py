#!/usr/bin/env python3

import cmof_parser, cmof_print_all, cmof_model_builder, cmof_model_print

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
        cmof_print_all.print_all(out, t)
        return

    print ("Building model...")

    model = cmof_model_builder.build_model(t, err)
    cmof_model_print.print_model(out, model)


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
        'print_parsed'
    ]

    def __init__(self):
        self.filename = None
        self.print_raw = False
        self.print_parsed = True



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
        # elif s == '-p':
        #     opts.print_parsed = True
        elif s == '-m':
            opts.print_parsed = False
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
    print ("     -r  - print raw XML")
    # print ("     -p  - print parsed CMOF")
    print ("     -m  - print constructed model")
    sys.exit(1)


class Output(object):

    __slots__ = [
        'out',
        'indent',
        'start_line'
    ]

    def __init__(self, f):
        self.out = f
        self.indent = 0
        self.start_line = True

    def write(self, s):
        if self.start_line:
            self.start_line = False
            self.out.write(' ' * self.indent * 4)
        self.out.write(s)
        return self

    def nl(self):
        self.out.write('\n')
        self.start_line = True
        return self
    
    def inc(self):
        self.indent += 1
        
    def dec(self):
        assert (self.indent > 0)
        self.indent -= 1


class ErrOutput (object):

    __slots__ = [ 'out' ]
    
    def __init__(self, out):
        self.out = out

    def syntax_error(self, msg):
        self.out.write('\n')
        self.out.write("SYNTAX ERROR: ")
        self.out.write (msg)
        self.out.write('\n\n')
        sys.exit(1)

    def error(self, msg):
        self.out.write('\n')
        self.out.write("ERROR: ")
        self.out.write (msg)
        self.out.write('\n\n')
        sys.exit(1)

    def warning(self, msg):
        self.out.write('\n')
        self.out.write("WARNING: ")
        self.out.write (msg)
        self.out.write('\n\n')



if __name__ == '__main__':
    main()

