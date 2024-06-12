
from xsd_model import *
import xsd_print_parsed


def print_model(out, model):
    assert isinstance(model, XSD_Model)
    out.write("XSD_MODEL {").nl()
    out.inc()

    for c in model.get_classes():
        out.nl()
        print_Class(out, c)

    out.dec()
    out.write("}").nl()


def print_Class(out, c):
    out.write("XSD_Class " + repr(c.element_name) + " : " + repr(c.type_name))

    out.write(" {").nl()
    out.inc()

    l = []

    if c.base_name is not None:
         l.append("base = " + repr(c.base_name))

    if c.substitutionGroup is not None:
        l.append("substitution group = " + repr(c.substitutionGroup))

    if c.element_abstract is not None:
        l.append("element_abstract = " + repr(c.element_abstract))

    if c.type_abstract is not None:
        l.append("type_abstract = " + repr(c.type_abstract))

    if c.mixed is not None:
        l.append("mixed = " + repr(c.mixed))

    if len(l) > 0:
        s = ", ".join(l)
        out.write("  --- " + s).nl()

    if c.children is not None:
        if len(l) > 0: out.nl()
        xsd_print_parsed.print_children(out, c.children)

    attrs = c.attrs

    if len(attrs) > 0:
        if c.children is not None or len(l) > 0: out.nl()
        for a in attrs:
            xsd_print_parsed.print_Attribute(out, a)

    if c.any_attr is not None:
        out.nl()
        xsd_print_parsed.print_AnyAttribute(out, c.any_attr)

    out.dec()
    out.write("}").nl()

