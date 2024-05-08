
from xml_scanner import create_xml_scanner
from xsd_parsed_classes import *


xsd_uri = "http://www.w3.org/2001/XMLSchema"

ns_xsd = '{' + xsd_uri + '}'

xsd_element     = ns_xsd + 'element'
xsd_complexType = ns_xsd + 'complexType'
xsd_simpleType  = ns_xsd + 'simpleType'
xsd_import      = ns_xsd + 'import'
xsd_include     = ns_xsd + 'include'


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
        imp = parse_import(p)
        p.pop()
        imports.append(imp)

    includes = []

    while p.tag == xsd_include:
        p.push()
        incl = parse_include(p)
        p.pop()
        includes.append(incl)

    members = []

    while p.tag is not None:

        tag = p.tag
        # print ("Tag: " + repr(tag))

        if tag == xsd_element:
            p.push()
            e = parse_element(p)
            p.pop()
            member = e

        elif tag == xsd_complexType:
            p.push()
            t = parse_complexType(p)
            p.pop()
            member = t

        elif tag == xsd_simpleType:
            p.push()
            t = parse_simpleType(p)
            p.pop()
            member = t

        else:
            p.error("Unknown tag " + repr(tag))

        members.append(member)

    p.check_end()

    return P_top_node (elementFormDefault, attributeFormDefault,
                       targetNamespace, imports, includes, members)


def parse_import(p):
    p.check_cur_tag(xsd_import)
    attrs = p.get_attrs()
    p.check_attrs_end()

    return "Import"


def parse_include(p):
    p.check_cur_tag(xsd_include)
    attrs = p.get_attrs()
    p.check_attrs_end()

    return "Include"


def parse_element(p):
    p.check_cur_tag(xsd_element)
    name = p.get_attr_required('name')
    attrs = p.get_attrs()
    p.check_attrs_end()

    return P_Element(name, attrs)


def parse_complexType(p):
    p.check_cur_tag(xsd_complexType)
    name = p.get_attr_required('name')
    attrs = p.get_attrs()
    p.check_attrs_end()

    return P_ComplexType(name, attrs)


def parse_simpleType(p):
    p.check_cur_tag(xsd_simpleType)
    name = p.get_attr_required('name')
    attrs = p.get_attrs()
    p.check_attrs_end()

    return P_SimpleType(name, attrs)

