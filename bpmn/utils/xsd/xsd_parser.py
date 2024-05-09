
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
        # print ("Tag: " + repr(tag))

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
    abstract = p.get_attr_opt("abstract")
    mixed = p.get_attr_opt("mixed")
    p.check_attrs_end()

    if p.tag == xsd_complexContent:
        p.push()
        complexContent = parse_ComplexContent(p)
        p.pop()

    else:
        complexContent = None

        print ("parse_ComplexType(1): " + repr(p.tag))

        while p.tag is not None:
            p.next()
            print ("   parse_ComplexType(2): " + repr(p.tag))

    p.check_end()
    return P_ComplexType(name, abstract, mixed, complexContent)


def parse_ComplexContent(p):
    p.check_cur_tag(xsd_complexContent)
    p.check_attrs_end()

    p.check_tag(xsd_extension)
    p.push()
    extension = parse_Extension(p)
    p.pop()

    # print ("parse_ComplexContent(1): " + repr(p.tag))

    # while p.tag is not None:
    #     p.next()
    #     print ("   parse_ComplexContent(2): " + repr(p.tag))

    p.check_end()
    return P_ComplexContent(extension)


def parse_Extension(p):
    p.check_cur_tag(xsd_extension)

    base = p.get_attr_required("base")
    p.check_attrs_end()

    print ("parse_Extension(1): " + repr(p.tag))

    while p.tag is not None:
        p.next()
        print ("   parse_Extension(2): " + repr(p.tag))

    p.check_end()
    return P_Extension(base)


def parse_SimpleType(p):
    p.check_cur_tag(xsd_simpleType)
    name = p.get_attr_required('name')
    p.check_attrs_end()

    print ("parse_SimpleType(1): " + repr(p.tag))

    while p.tag is not None:
        p.next()
        print ("   parse_SimpleType(2): " + repr(p.tag))


    return P_SimpleType(name)

