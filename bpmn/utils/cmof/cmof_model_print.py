
from cmof_model import *


def print_model(out, model):
    assert isinstance(model, M_Model)
    out.nl().write("Model {").nl()
    out.inc()

    print_classes(out, model.get_classes())
    print_enumerations(out, model.get_enumerations())
    print_primitive_types(out, model.get_primitive_types())

    sep_line_last(out)
    out.dec()
    out.write("}").nl()


def print_classes(out, classes):
    n = len(classes)
    if n == 0: return
    sep_line(out)
    out.write("    (%d classes)" % n).nl().nl()
    first = True
    for c in classes:
        if first:
            first = False
        else:
            out.nl()
        print_Class(out, c)
    out.nl()


def print_enumerations(out, enums):
    n = len(enums)
    if n == 0: return
    sep_line(out)
    out.write("    (%d enumerations)" % n).nl().nl()
    for c in enums:
        print_Enumeration(out, c)
        out.nl()


def print_primitive_types(out, prim_types):
    n = len(prim_types)
    if n == 0: return
    sep_line(out)
    out.write("    (%d primitive types)" % n).nl().nl()
    for c in prim_types:
        print_PrimitiveType(out, c)
    out.nl()



def print_Class(out, c):
    assert isinstance(c, M_Class)
    if c.is_abstract:
        out.write ("abstract ")
    out.write("class " + c.name + " {").nl()
    out.inc()
    for attr in c.attributes:
        print_Attribute(out, attr)
    out.dec()
    out.write("}").nl()


def print_Attribute(out, c):
    out.write(c.name)
    typ = c.type
    if typ is not None:
        assert isinstance(typ, M_Type)
        out.write(" : " + typ.name)
    out.nl()


def print_Enumeration(out, c):
    out.write("enum " + c.name + " {").nl()
    out.inc()
    for item in c.literals:
        assert item.enum is c
        out.write(item.name).nl()
    out.dec()
    out.write("}").nl()

def print_PrimitiveType(out, c):
    out.write("primitive type " + c.name).nl()


def sep_line(out):
    out.write('-' * 60).nl().nl()

def sep_line_last(out):
    out.write('-' * 60).nl()
