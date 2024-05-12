
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
    out.write("Element " + repr(c.name) + " : " + repr(c.type))

    l = []

    if c.abstract is not None:
        l.append("abstract = " + repr(c.abstract))

    if c.substitutionGroup is not None:
        l.append("substitution group = " + repr(c.substitutionGroup))

    if len(l) > 0:
        s = ", ".join(l)
        out.write(" (" + s + ")")

    out.nl()


def print_ComplexType(out, c):
    assert isinstance(c, P_ComplexType)
    out.write("ComplexType " + repr(c.name))
    out.inc()

    l = []
    if c.abstract is not None:
        l.append("abstract = " + repr(c.abstract))

    if c.mixed is not None:
        l.append("mixed = " + repr(c.mixed))

    if len(l) > 0:
        s = ", ".join(l)
        out.write(" (" + s + ")")

    out.write(" {").nl()

    if c.complexContent is not None:
        print_ComplexContent(out, c.complexContent)

    if c.expr_attrs is not None:
        print_expr_attrs(out, c.expr_attrs)

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

    if c.expr_attrs is not None:
        print_expr_attrs(out, c.expr_attrs)

    out.dec()
    out.write("}").nl()


def print_expr_attrs(out, c):
    assert isinstance(c, P_expr_and_attributes)

    if c.expr is not None:
        print_expr(out, c.expr)

    attrs = c.attributes

    if len(attrs) > 0:
        if c.expr is not None: out.nl()
        for a in c.attributes:
            print_Attribute(out, a)

    if c.any_attribute is not None:
        out.nl()
        print_AnyAttribute(out, c.any_attribute)


def print_expr(out, c):
    assert isinstance(c, P_Expr)
    v = Expr_Visitor(out)
    c.visit(v)


class Expr_Visitor (object):

    __slots__ = [ 'out' ]

    def __init__(self, out):
        self.out = out

    def visit_Sequence(self, c):
        assert isinstance(c, P_Sequence)
        print_Sequence(self.out, c)

    def visit_Choice(self, c):
        assert isinstance(c, P_Choice)
        print_Choice(self.out, c)

    def visit_Expr_Element(self, c):
        assert isinstance(c, P_Expr_Element)
        print_Expr_Element(self.out, c)


def print_Choice(out, c):
    assert isinstance(c, P_Choice)
    out.write("Choice {").nl()
    out.inc()

    for e in c.exprs:
        print_expr(out, e)

    out.dec()
    out.write("}").nl()


def print_Expr_Element(out, c):
    assert isinstance(c, P_Expr_Element)
    print_LocalElement(out, c.element)


def print_Sequence(out, c):
    assert isinstance(c, P_Sequence)
    out.write("Seq {").nl()
    out.inc()

    for e in c.elements:
        print_LocalElement(out, e)

    if c.any is not None:
        print_LocalAny(out, c.any)

    out.dec()
    out.write("}").nl()


def print_LocalElement(out, c):
    assert isinstance(c, P_LocalElement)
    v = LocalElement_Visitor(out)
    c.visit(v)


class LocalElement_Visitor (object):

    __slots__ = [ 'out' ]

    def __init__(self, out):
        self.out = out

    def visit_LocalElementRef(self, c):
        assert isinstance(c, P_LocalElementRef)
        print_LocalElementRef(self.out, c)

    def visit_LocalElementDef(self, c):
        assert isinstance(c, P_LocalElementDef)
        print_LocalElementDef(self.out, c)

    def visit_LocalElementDefType(self, c):
        assert isinstance(c, P_LocalElementDefType)
        print_LocalElementDefType(self.out, c)


def print_LocalElementRef(out, c):
    out.write("LocalElement ref = " + repr(c.ref))
    s = card_to_str(c.card)
    if s is not None:
        out.write(" " + s)
    out.nl()


def print_LocalElementDef(out, c):
    out.write("LocalElementDef " + repr(c.name) + " : " + repr(c.type))
    s = card_to_str(c.card)
    if s is not None:
        out.write(" " + s)
    out.nl()


def print_LocalElementDefType(out, c):
    assert isinstance(c, P_LocalElementDefType)
    out.write("LocalElementDefType " + repr(c.name))
    s = card_to_str(c.card)
    if s is not None:
        out.write(" " + s)
    out.write(" {").nl()
    out.inc()
    print_Local_ComplexType(out, c.type)
    out.dec()
    out.write("}").nl()


def print_Local_ComplexType(out, c):
    assert isinstance(c, P_Local_ComplexType)
    out.write("Local_ComplexType {").nl()
    out.inc()
    print_expr_attrs(out, c.expr_attrs)
    out.dec()
    out.write("}").nl()


def print_LocalAny(out, c):
    assert isinstance(c, P_LocalAny)
    out.write("LocalAny")
    s = card_to_str(c.card)
    if s is not None:
        out.write(" " + s)
    out.write(" (namespace = " + repr(c.namespace))
    if c.processContents is not None:
        out.write(", processContents = " + repr(c.processContents))
    out.write(")").nl()


def print_Attribute(out, c):
    assert isinstance(c, P_Attribute)

    out.write("Attribute " + repr(c.name) + " ")

    if c.type is not None:
        out.write(": " + repr(c.type))

    l = []

    if c.use is not None:
        l.append("use = " + repr(c.use))

    if c.default is not None:
        l.append("default = " + repr(c.default))

    if len(l) > 0:
        s = ", ".join(l)
        out.write(" (" + s + ")")

    out.nl()


def print_AnyAttribute(out, c):
    assert isinstance(c, P_AnyAttribute)
    out.write("AnyAttribute (")
    out.write("namespace = " + repr(c.namespace) + ", ")
    out.write("processContents = " + repr(c.processContents) + ")").nl()


def print_SimpleType(out, c):
    assert isinstance(c, P_SimpleType)
    out.write("SimpleType " + repr(c.name) + " {").nl()
    out.inc()

    if c.content is not None:
        print_SimpleType_content(out, c.content)

    out.dec()
    out.write("}").nl()


def print_SimpleType_content(out, c):
    v = SimpleType_Visitor(out)
    c.visit(v)


class SimpleType_Visitor (object):

    __slots__ = [ 'out' ]

    def __init__(self, out):
        self.out = out

    def visit_Restriction(self, c):
        assert isinstance(c, P_Restriction)
        print_Restriction(self.out, c)

    def visit_Union(self, c):
        assert isinstance(c, P_Union)
        print_Union(self.out, c)



def print_Restriction(out, c):
    assert isinstance(c, P_Restriction)
    out.write("Restriction " + repr(c.base) + " {").nl()
    out.inc()

    for e in c.enumerations:
        print_Enumeration(out, e)

    out.dec()
    out.write("}").nl()


def print_Enumeration(out, c):
    assert isinstance(c, P_Enumeration)
    out.write("Enumeration " + repr(c.value)).nl()


def print_Union(out, c):
    assert isinstance(c, P_Union)
    out.write("Union (memberTypes = " + repr(c.member_types) + ") {").nl()
    out.inc()
    print_Local_SimpleType(out, c.simple_type)
    out.dec()
    out.write("}").nl()


def print_Local_SimpleType(out, c):
    assert isinstance(c, P_Local_SimpleType)
    out.write("Local_SimpleType {").nl()
    out.inc()
    print_Restriction(out, c.restriction)
    out.dec()
    out.write("}").nl()


def card_value_to_str(s):
    if s is None:
        return '_'
    else:
        return repr(s)


def card_to_str(card):
    lower, upper = card
    if lower is None and upper is None: return None
    s1 = card_value_to_str(lower)
    s2 = card_value_to_str(upper)
    return "[" + s1 + " .. " + s2 + "]"


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


