#!/usr/bin/env python3

from cmof_parser import parse_cmof
import cmof_print_all

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

    tree = read_xml(inp)

    if options.print_raw:
        print_tree_raw(tree)
    else:
        t = parse_cmof(tree, err)
        cmof_print_all.print_all(out, t)

    print ('')
    print ("O.K.")


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
        'print_raw'
    ]

    def __init__(self):
        self.filename = None
        self.print_raw = False



def parse_options(argv):
    opts = Options()
    i = 1
    n = len(argv)
    while i < n:
        s = argv[i]
        i += 1
        if s == '-r':
            opts.print_raw = True
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
    print ("Usage: -----")
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

    def error(self, msg):
        self.out.write (msg)
        self.out.write ('\n')
        sys.exit(1)



if __name__ == '__main__':
    main()

