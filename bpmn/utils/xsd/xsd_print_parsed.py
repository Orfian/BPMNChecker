
from xsd_parsed_classes import *


def print_all(out, c):
    assert isinstance(c, P_top_node)
    out.write("XMLSchema {").nl()
    out.inc()
    out.write("elementFormDefault: " + repr(c.elementFormDefault)).nl()
    out.write("attributeFormDefault: " + repr(c.attributeFormDefault)).nl()
    out.write("targetNamespace: " + repr(c.targetNamespace)).nl()
    out.nl()

    for imp in c.imports:
        out.write(repr(imp)).nl()

    for incl in c.includes:
        out.write(repr(incl)).nl()

    for member in c.members:
        out.nl()
        print_Member(out, member)

    out.dec()
    out.write("}").nl()


def print_Member(out, c):
    # assert isinstance(c, P_Member)
    v = Member_Visitor(out)
    c.visit(v)


class Member_Visitor (object):

    __slots__ = [ 'out' ]

    def __init__(self, out):
        self.out = out

    def visit_Element(self, c):
        assert isinstance(c, P_Element)
        print_Element(self.out, c)

    def visit_ComplexType(self, c):
        assert isinstance(c, P_ComplexType)
        print_ComplexType(self.out, c)

    def visit_SimpleType(self, c):
        assert isinstance(c, P_SimpleType)
        print_SimpleType(self.out, c)


def print_Element(out, c):
    assert isinstance(c, P_Element)
    out.write("Element " + repr(c.name) + " {").nl()
    out.inc()
    print_attrs(out, c.attrs)
    out.dec()
    out.write("}").nl()


def print_ComplexType(out, c):
    assert isinstance(c, P_ComplexType)
    out.write("ComplexType " + repr(c.name) + " {").nl()
    out.inc()
    print_attrs(out, c.attrs)
    out.dec()
    out.write("}").nl()


def print_SimpleType(out, c):
    assert isinstance(c, P_SimpleType)
    out.write("SimpleType " + repr(c.name) + " {").nl()
    out.inc()
    print_attrs(out, c.attrs)
    out.dec()
    out.write("}").nl()



def print_attrs(out, attrs):
    out.write('attrs:')
    if len(attrs) == 0:
        out.write(" ---").nl()
    else:
        out.nl()
        out.inc()
        for name, val in attrs:
            out.write(repr(name) + " ==> " + repr(val)).nl()
        out.dec()


