
from xsd_parsed_classes import *


def print_all(out, c):
    assert isinstance(c, P_top_node)
    out.write("XMLSchema {").nl()
    out.inc()
    out.write("elementFormDefault: " + repr(c.elementFormDefault)).nl()
    out.write("attributeFormDefault: " + repr(c.attributeFormDefault)).nl()
    out.write("targetNamespace: " + repr(c.targetNamespace)).nl()

    for imp in c.imports:
        out.nl()
        print_Import(out, imp)

    for incl in c.includes:
        out.nl()
        print_Include(out, incl)

    for member in c.members:
        out.nl()
        print_Member(out, member)

    out.dec()
    out.write("}").nl()


def print_Import(out, c):
    assert isinstance(c, P_Import)
    out.write("Import {").nl()
    out.inc()
    out.write("namespace ==> " + repr(c.namespace)).nl()
    out.write("location ==> " + repr(c.schemaLocation)).nl()
    out.dec()
    out.write("}").nl()


def print_Include(out, c):
    assert isinstance(c, P_Include)
    out.write("Include {").nl()
    out.inc()
    out.write("location ==> " + repr(c.schemaLocation)).nl()
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
    out.write("Element " + repr(c.name) + " : " + repr(c.type) + " {")
    out.inc()

    print_nl = False

    if c.substitutionGroup is not None:
        out.nl().write("substitution group ==> " + repr(c.substitutionGroup))
        print_nl = True

    if c.abstract is not None:
        out.nl().write("abstract ==> " + repr(c.abstract))
        print_nl = True

    if print_nl: out.nl()
    out.dec()
    out.write("}").nl()


def print_ComplexType(out, c):
    assert isinstance(c, P_ComplexType)
    out.write("ComplexType " + repr(c.name) + " {").nl()
    out.inc()

    if c.abstract is not None:
        out.write("abstract ==> " + repr(c.abstract)).nl()

    if c.mixed is not None:
        out.write("mixed ==> " + repr(c.mixed)).nl()

    if c.complexContent is not None:
        out.nl()
        print_ComplexContent(out, c.complexContent)

    out.dec()
    out.write("}").nl()


def print_ComplexContent(out, c):
    assert isinstance(c, P_ComplexContent)
    out.write("ComplexContent {").nl()
    out.inc()
    print_Extension(out, c.extension)
    out.dec()
    out.write("}").nl()


def print_Extension(out, c):
    assert isinstance(c, P_Extension)
    out.write("Extension " + repr(c.base) + " {").nl()
    out.inc()
    out.dec()
    out.write("}").nl()


def print_SimpleType(out, c):
    assert isinstance(c, P_SimpleType)
    out.write("SimpleType " + repr(c.name) + " {").nl()
    out.inc()
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


