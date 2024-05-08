
from cmof_model import *


def print_model(out, model):
    assert isinstance(model, M_Model)
    out.nl().write("Model " + model.get_package_name() + " {").nl()
    out.inc()

    print_external_types(out, model.get_external_types())
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


def print_external_types(out, ext_types):
    n = len(ext_types)
    if n == 0: return
    sep_line(out)
    out.write("    (%d external types)" % n).nl().nl()
    first = True
    for c in ext_types:
        if first:
            first = False
        else:
            out.nl()
        print_external_type(out, c)
    out.nl()



def print_Class(out, c):
    assert isinstance(c, M_Class)
    if c.is_abstract:
        out.write ("abstract ")
    out.write("class " + c.name)

    scls = c.superclasses
    if len(scls) > 0:
        out.write(" :> ")
        scls_names = [ sc.name for sc in scls ]
        out.write(" +++ ".join(scls_names))

    out.write(" {").nl()
    out.inc()
    for attr in c.attributes:
        print_Attribute(out, attr)
    out.dec()
    out.write("}").nl()


def print_Attribute(out, c):
    out.write(c.name)
    typ = c.type
    assert isinstance(typ, M_Type)
    out.write(" : " + typ.name)
    s = cardinality_to_str(c.cardinality)
    if s is not None:
        out.write(" " + s)
    out.nl()


def print_Enumeration(out, c):
    out.write("enum " + c.name + " :=").nl()
    out.inc()
    for item in c.literals:
        assert item.enum is c
        out.write("| " + item.name).nl()
    out.dec()
    # out.write("}").nl()


def print_PrimitiveType(out, c):
    out.write("primitive type " + c.name).nl()


def print_external_type(out, c):
    out.write("external " + type_kind_to_str(c.kind) + " " +
              c.name + " {").nl()
    out.inc()
    out.write("href = " + repr(c.href)).nl()
    out.dec()
    out.write("}").nl()


def type_kind_to_str(kind):
    assert isinstance(kind, Type_Kind)

    match kind:
        case Type_Kind.Class:
            return "class"

        case Type_Kind.DataType:
            return "datatype"

        case Type_Kind.Enumeration:
            return "enum"

        case Type_Kind.PrimitiveType:
            return "primitive type"

        case _:
            assert False


def cardinality_to_str(card):
    assert isinstance(card, M_Cardinality)
    lower = card.lower
    upper = card.upper
    if lower == 1 and upper == 1:
        return None

    if lower == 0 and upper == 1:
        return "(opt)"

    ls = "%d" % lower
    if upper == -1:
        us = "*"
    else:
        us = "%d" % upper
    return "[" + ls + " .. " + us + "]"


def sep_line(out):
    out.write('-' * 60).nl().nl()

def sep_line_last(out):
    out.write('-' * 60).nl()
