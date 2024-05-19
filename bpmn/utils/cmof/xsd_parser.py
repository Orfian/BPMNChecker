
from xml_scanner import create_xml_scanner
from xsd_parsed_classes import *


xsd_uri = "http://www.w3.org/2001/XMLSchema"

ns_xsd = '{' + xsd_uri + '}'

xsd_element        = ns_xsd + 'element'
xsd_complexType    = ns_xsd + 'complexType'
xsd_simpleType     = ns_xsd + 'simpleType'
xsd_import         = ns_xsd + 'import'
xsd_include        = ns_xsd + 'include'
xsd_complexContent = ns_xsd + 'complexContent'
xsd_extension      = ns_xsd + 'extension'
xsd_sequence       = ns_xsd + 'sequence'
xsd_any            = ns_xsd + 'any'
xsd_choice         = ns_xsd + 'choice'
xsd_attribute      = ns_xsd + 'attribute'
xsd_anyAttribute   = ns_xsd + 'anyAttribute'
xsd_restriction    = ns_xsd + 'restriction'
xsd_enumeration    = ns_xsd + 'enumeration'
xsd_union          = ns_xsd + 'union'


def parse_xsd(tree, err_output):
    p = create_xml_scanner(tree, err_output)
    return parse_top_node(p)


def parse_top_node(p):
    p.check_cur_tag(ns_xsd + 'schema')
    elementFormDefault = p.get_attr_required('elementFormDefault')
    attributeFormDefault = p.get_attr_required('attributeFormDefault')
    targetNamespace = p.get_attr_required('targetNamespace')
    p.check_attrs_end()

    imports = []

    while p.tag == xsd_import:
        p.push()
        imp = parse_Import(p)
        p.pop()
        imports.append(imp)

    includes = []

    while p.tag == xsd_include:
        p.push()
        incl = parse_Include(p)
        p.pop()
        includes.append(incl)

    members = []

    while p.tag is not None:

        tag = p.tag

        if tag == xsd_element:
            p.push()
            e = parse_Element(p)
            p.pop()
            member = e

        elif tag == xsd_complexType:
            p.push()
            t = parse_ComplexType(p)
            p.pop()
            member = t

        elif tag == xsd_simpleType:
            p.push()
            t = parse_SimpleType(p)
            p.pop()
            member = t

        else:
            p.error("Unknown tag " + repr(tag))

        members.append(member)

    p.check_end()

    return P_top_node (elementFormDefault, attributeFormDefault,
                       targetNamespace, imports, includes, members)


def parse_Import(p):
    p.check_cur_tag(xsd_import)
    namespace = p.get_attr_required("namespace")
    schemaLocation = p.get_attr_required("schemaLocation")
    p.check_attrs_end()
    p.check_end()
    return P_Import(namespace, schemaLocation)


def parse_Include(p):
    p.check_cur_tag(xsd_include)
    schemaLocation = p.get_attr_required("schemaLocation")
    p.check_attrs_end()
    p.check_end()
    return P_Include(schemaLocation)


def parse_Element(p):
    p.check_cur_tag(xsd_element)

    name = p.get_attr_required("name")
    typ = p.get_attr_required("type")
    subst_group = p.get_attr_opt("substitutionGroup")
    abstract = p.get_attr_opt("abstract")
    p.check_attrs_end()
    
    p.check_end()
    return P_Element(name, typ, subst_group, abstract)


def parse_ComplexType(p):
    p.check_cur_tag(xsd_complexType)

    name = p.get_attr_required('name')
    abstract = p.get_attr_opt('abstract')
    mixed = p.get_attr_opt('mixed')
    p.check_attrs_end()

    # print ("Parsing ComplextType " + repr(name))

    if p.tag == xsd_complexContent:
        p.push()
        base, expr_attrs = parse_ComplexContent(p)
        p.pop()
    else:
        base = None
        expr_attrs = parse_expr_and_attributes(p)

    p.check_end()

    children = expr_attrs.expr
    attrs = expr_attrs.attributes
    any_attr = expr_attrs.any_attribute

    return P_ComplexType(name, abstract, mixed, base,
                         children, attrs, any_attr)


class Expr_and_attributes(object):

    __slots__ = [
        'expr',
        'attributes',
        'any_attribute'
    ]

    def __init__(self, expr, attributes, any_attr):
        self.expr = expr
        self.attributes = attributes
        self.any_attribute = any_attr



def parse_ComplexContent(p):
    p.check_cur_tag(xsd_complexContent)
    p.check_attrs_end()

    p.check_tag(xsd_extension)

    p.push()
    p.check_cur_tag(xsd_extension)

    base = p.get_attr_required("base")
    p.check_attrs_end()

    expr_attrs = parse_expr_and_attributes(p)

    p.check_end()
    p.pop()

    p.check_end()
    return (base, expr_attrs)



def parse_expr_and_attributes(p):
    # print("parse_expr_and_attributes(1): " + repr(p.tag))
    if p.tag == xsd_sequence:
        p.push()
        children = parse_Seq(p)
        p.pop()

    elif p.tag == xsd_choice:
        p.push()
        children = parse_GenExpr(p)
        p.pop()

    else:
        children = None

    attributes = []

    # print("parse_expr_and_attributes(2): " + repr(p.tag))
    while p.tag == xsd_attribute:
        p.push()
        a = parse_Attribute(p)
        p.pop()
        attributes.append(a)

    if p.tag == xsd_anyAttribute:
        p.push()
        any_attr = parse_AnyAttribute(p)
        p.pop()
    else:
        any_attr = None

    return Expr_and_attributes(children, attributes, any_attr)


def parse_Seq(p):
    p.check_cur_tag(xsd_sequence)
    p.check_attrs_end()

    if p.tag == xsd_any:
        p.push()
        res = parse_LocalAny(p)
        p.pop()

    else:
        elements = []

        while p.tag == xsd_element:
            p.push()
            e = parse_LocalElement(p)
            p.pop()
            elements.append(e)

        res = P_Seq(elements)

    p.check_end()
    return res


def parse_GenExpr(p):
    e = parse_expr(p)
    return P_GenExpr(e)


def parse_expr(p):
    if p.tag == xsd_sequence:
        p.push()
        e = parse_expr_seq(p)
        p.pop()
        return e

    elif p.tag == xsd_choice:
        p.push()
        e = parse_expr_choice(p)
        p.pop()
        return e

    elif p.tag == xsd_element:
        p.push()
        e = parse_expr_element(p)
        p.pop()
        return e

    else:
        p.error("<sequence> or <choice> expected")


def parse_expr_element(p):
    p.check_cur_tag(xsd_element)
    e = parse_LocalElement(p)
    return P_Expr_Element(e)


def parse_expr_seq(p):
    p.check_cur_tag(xsd_sequence)
    p.check_attrs_end()

    exprs = []

    while p.tag is not None:
        e = parse_expr(p)
        exprs.append(e)

    p.check_end()
    return P_Expr_Seq(exprs)


def parse_expr_choice(p):
    p.check_cur_tag(xsd_choice)
    p.check_attrs_end()

    exprs = []

    while p.tag is not None:
        e = parse_expr(p)
        exprs.append(e)

    p.check_end()
    return P_Expr_Choice(exprs)


def parse_Sequence(p):
    p.check_cur_tag(xsd_sequence)

    p.check_attrs_end()

    elements = []

    while p.tag == xsd_element:
        p.push()
        e = parse_LocalElement(p)
        p.pop()
        elements.append(e)

    if p.tag == xsd_any:
        p.push()
        any_e = parse_LocalAny(p)
        p.pop()
    else:
        any_e = None

    p.check_end()
    return P_Sequence(elements, any_e)


def parse_LocalElement(p):
    p.check_cur_tag(xsd_element)

    card = read_card(p)

    ref = p.get_attr_opt('ref')
    if ref is not None:
        p.check_attrs_end()
        res = P_LocalElementRef(ref, card)

    else:
        name = p.get_attr_required('name')
        typ = p.get_attr_opt('type')
        p.check_attrs_end()

        if typ is not None:
            res = P_LocalElementDef(name, typ, card)
        else:
            p.check_tag(xsd_complexType)
            p.push()
            typ = parse_Local_ComplexType(p)
            p.pop()

            res = P_LocalElementDefType(name, typ, card)

    p.check_end()
    return res


def parse_Local_ComplexType (p):
    p.check_cur_tag(xsd_complexType)

    p.check_attrs_end()
    expr_attrs = parse_expr_and_attributes(p)

    p.check_end()
    return P_Local_ComplexType(expr_attrs)


def parse_LocalAny(p):
    p.check_cur_tag(xsd_any)

    namespace = p.get_attr_required('namespace')
    processContents = p.get_attr_opt('processContents')
    card = read_card(p)
    p.check_attrs_end()

    p.check_end()
    return P_LocalAny(namespace, processContents, card)


def parse_Attribute(p):
    p.check_cur_tag(xsd_attribute)

    name = p.get_attr_required("name")
    typ = p.get_attr_required("type")
    use = p.get_attr_opt("use")
    default = p.get_attr_opt("default")
    p.check_attrs_end()

    p.check_end()
    return P_Attribute(name, typ, use, default)


def parse_AnyAttribute(p):
    p.check_cur_tag(xsd_anyAttribute)

    namespace = p.get_attr_required('namespace')
    processContents = p.get_attr_required('processContents')
    p.check_attrs_end()

    p.check_end()
    return P_AnyAttribute(namespace, processContents)


def parse_SimpleType(p):
    p.check_cur_tag(xsd_simpleType)
    name = p.get_attr_required('name')
    p.check_attrs_end()

    if p.tag == xsd_restriction:
        p.push()
        st = parse_Restriction(p)
        p.pop()
    elif p.tag == xsd_union:
        p.push()
        st = parse_Union(p)
        p.pop()
    else:
        p.error("<restriction> or <union> expected")

    p.check_end()
    return P_SimpleType(name, st)


def parse_Restriction(p):
    p.check_cur_tag(xsd_restriction)

    base = p.get_attr_required('base')
    p.check_attrs_end()

    enums = []

    while p.tag == xsd_enumeration:
        p.push()
        e = parse_Enumeration(p)
        p.pop()
        enums.append(e)

    p.check_end()
    return P_Restriction(base, enums)


def parse_Enumeration(p):
    p.check_cur_tag(xsd_enumeration)

    value = p.get_attr_required('value')
    p.check_attrs_end()

    p.check_end()
    return P_Enumeration(value)


def parse_Union(p):
    p.check_cur_tag(xsd_union)

    memberTypes = p.get_attr_required('memberTypes')
    p.check_attrs_end()

    p.check_tag(xsd_simpleType)
    p.push()
    simple_type = parse_Local_SimpleType(p)
    p.pop()

    p.check_end()
    return P_Union(memberTypes, simple_type)


def parse_Local_SimpleType(p):
    p.check_cur_tag(xsd_simpleType)
    p.check_attrs_end()

    p.check_tag(xsd_restriction)
    p.push()
    restriction = parse_Restriction(p)
    p.pop()

    p.check_end()
    return P_Local_SimpleType(restriction)


def read_card(p):
    min_occurs = p.get_attr_opt('minOccurs')
    max_occurs = p.get_attr_opt('maxOccurs')
    return (min_occurs, max_occurs)

