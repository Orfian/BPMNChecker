
from cmof_model import *


class Print_Options (object):

    __slots__ = [
        'print_props',
        'print_associations'
    ]

    def __init__(self, print_props, print_assoc):
        self.print_props = print_props
        self.print_associations = print_assoc


def create_print_options(print_props, print_assoc):
    return Print_Options(print_props, print_assoc)



def print_model(out, model, opts):
    assert isinstance(model, M_Model)
    out.nl().write("Model " + model.get_package_name() + " {").nl()
    out.inc()

    print_external_types(out, model.get_external_types())
    print_classes(out, model.get_classes(), opts)
    print_enumerations(out, model.get_enumerations(), opts)
    print_primitive_types(out, model.get_primitive_types())

    sep_line_last(out)
    out.dec()
    out.write("}").nl()


def print_classes(out, classes, opts):
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
        print_Class(out, c, opts)
    out.nl()


def print_enumerations(out, enums, opts):
    n = len(enums)
    if n == 0: return
    sep_line(out)
    out.write("    (%d enumerations)" % n).nl().nl()
    for c in enums:
        print_Enumeration(out, c, opts)
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



def print_Class(out, c, opts):
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
        print_Attribute(out, attr, opts)
    out.dec()
    out.write("}").nl()


def print_Attribute(out, c, opts):
    out.write(c.name)
    typ = c.type
    assert isinstance(typ, M_Type)
    out.write(" : " + typ.name)

    s = cardinality_to_str(c.cardinality)
    if s is not None:
        out.write(" " + s)

    if opts.print_props:
        s = attr_props_to_str(c.visibility, c.props)
        if s is not None:
            out.write(" ==> (" + s + ")")

    if opts.print_associations:
        print_attr_assoc(out, c.association, c.assoc_index)

    out.nl()


def print_assoc_end(out, c):
    out.write(c.name)
    typ = c.type
    assert isinstance(typ, M_Type)
    out.write(" : " + typ.name)

    s = cardinality_to_str(c.cardinality)
    if s is not None:
        out.write(" " + s)


def attr_props_to_str(visibility, props):
    assert isinstance(props, M_Attr_props)
    a = []

    s = visibility_to_str(visibility)
    if s is not None:
        a.append(s)

    if props.isComposite:
        a.append("composite")

    if props.isReadOnly:
        a.append("read-only")

    if props.isDerived:
        a.append("derived")

    if props.isDerivedUnion:
        a.append("derived union")

    if props.isOrdered:
        a.append("ordered")

    if not props.isUnique:
        a.append("not unique")

    if props.default is not None:
        a.append("default = " + repr(props.default))

    if len(a) == 0: return None
    return ", ".join(a)


def print_attr_assoc(out, assoc, index):
    if assoc is None: return

    if assoc.is_one_way():
        print_one_way_assoc(out, assoc)
    else:
        print_two_way_assoc(out, assoc, index)


def print_one_way_assoc(out, assoc):
    out.write(" -----> ")
    out.write("[ ")
    print_assoc_end(out, assoc.end)
    out.write(" ]")


def print_two_way_assoc(out, assoc, index):
    out.write(" <-----> ")
    out.write("[ ")
    # out.write("two-way: " + repr(assoc.name))
    attr = assoc.get_other_attr(index)
    s = attr.parent.name + '.' + attr.name
    # out.write(" --- attr: ")
    out.write(s)
    out.write(" ]")



def print_Enumeration(out, c, opts):
    out.write("enum " + c.name + " :=").nl()
    out.inc()
    for item in c.literals:
        assert item.enum is c
        out.write("| " + item.name).nl()
    out.dec()


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


def visibility_to_str(vis):
    assert isinstance(vis, Visibility)

    match vis:
        case Visibility.Unknown:
            return None

        case Visibility.Public:
            return "public"

        case Visibility.Private:
            return "private"

        case _:
            assert False


def sep_line(out):
    out.write('-' * 60).nl().nl()

def sep_line_last(out):
    out.write('-' * 60).nl()
