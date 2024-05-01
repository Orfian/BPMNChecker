
from xml_scanner import create_xml_scanner
from cmof_classes import *


xmi_uri = "http://schema.omg.org/spec/XMI/2.1"
cmof_uri = "http://schema.omg.org/spec/MOF/2.0/cmof.xml"

ns_xmi = '{' + xmi_uri + '}'
ns_cmof = '{' + cmof_uri + '}'

xmi_id = ns_xmi + 'id'
xmi_type = ns_xmi + 'type'


def parse_cmof(tree, err_output):
    p = create_xml_scanner(tree, err_output)
    return parse_top_node(p)


def parse_top_node(p):
    p.check_cur_tag(ns_xmi + 'XMI')
    version = p.get_attr_required(ns_xmi + 'version')
    p.check_attrs_end()
    
    p.check_tag (ns_cmof + 'Package')
    p.push()
    package = parse_Package(p)
    p.pop()

    tags = []

    tag_Tag = ns_cmof + 'Tag'

    while p.tag == tag_Tag:
        p.push()
        tag_elem = parse_Tag(p)
        p.pop()

        tags.append(tag_elem)

    p.check_end()

    return CMOF_top_node (version, package, tags)


def parse_Package(p):
    p.check_cur_tag(ns_cmof + 'Package')

    id = p.get_attr_required(xmi_id)
    name = p.get_attr_required('name')
    uri = p.get_attr_required('uri')
    p.check_attrs_end()

    imports = []

    while p.tag == 'packageImport':
        p.push()
        imp = parse_packageImport(p)
        p.pop()
        imports.append(imp)

    members = []

    while p.tag == 'ownedMember':
        p.push()
        member = parse_Member(p)
        p.pop()
        members.append(member)

    p.check_end()

    return CMOF_Package(id, name, uri, imports, members)


def check_xmi_type(p, xtype, expected, tag):
    if xtype == expected:
        return
    p.error("<" + tag + "> expects xmi:type = " + repr(expected) + 
            " and got " + repr(xtype))


def read_type_id_name(p):
    xtype = p.get_attr_required(xmi_type)
    xid = p.get_attr_required(xmi_id)
    name = p.get_attr_required('name')
    return (xtype, xid, name)


def read_type_href(p):
    xtype = p.get_attr_required(xmi_type)
    href = p.get_attr_required('href')
    return (xtype, href)


def parse_packageImport(p):
    p.check_cur_tag('packageImport')

    xtype = p.get_attr_required(xmi_type)
    check_xmi_type(p, xtype, 'cmof:PackageImport', 'packageImport')
    xid = p.get_attr_required(xmi_id)
    importingNamespace = p.get_attr_required('importingNamespace')
    p.check_attrs_end()

    p.check_tag('importedPackage')
    p.push()
    importedPackage = parse_importedPackage(p)
    p.pop()

    p.check_end()
    return CMOF_PackageImport(xtype, xid, importingNamespace, importedPackage)


def parse_Member(p):
    p.check_cur_tag('ownedMember')

    tin = read_type_id_name(p)
    xmi_type = tin[0]

    match xmi_type:

        case 'cmof:Class':
            return parse_Class(p, tin)

        case 'cmof:DataType':
            return parse_DataType(p, tin)

        case 'cmof:PrimitiveType':
            return parse_PrimitiveType(p, tin)

        case 'cmof:Enumeration':
            return parse_Enumeration(p, tin)

        case 'cmof:Association':
            return parse_Association(p, tin)

        case _:
            p.error("Unknown <ownedMember> type " + repr(xmi_type))

    assert False


def parse_Class(p, tin):
    isAbstract = p.get_attr_opt('isAbstract')
    superClass = p.get_attr_opt('superClass')
    p.check_attrs_end()

    rules = parse_rules(p)
    attributes = parse_class_attributes(p)

    superClass2 = None
    if p.tag == 'superClass':
        p.push()
        superClass2 = parse_superClass(p)
        p.pop()

    p.check_end()
    return CMOF_Class(tin, isAbstract, superClass, rules, attributes, superClass2)


def parse_DataType(p, tin):
    p.check_attrs_end()

    rules = parse_rules(p)
    attributes = parse_datatype_attributes(p)

    p.check_end()
    return CMOF_DataType(tin, rules, attributes)


def parse_rules(p):
    rules = []

    while p.tag == 'ownedRule':
        p.push()
        rule = parse_Rule(p)
        p.pop()
        rules.append(rule)

    return rules


def parse_class_attributes(p):
    attributes = []

    while p.tag == 'ownedAttribute':
        p.push()
        attribute = parse_Attribute(p)
        p.pop()
        attributes.append(attribute)

    return attributes


def parse_datatype_attributes(p):
    attributes = []

    while p.tag == 'ownedAttribute':
        p.push()
        attribute = parse_DataType_Attribute(p)
        p.pop()
        attributes.append(attribute)

    return attributes


def parse_PrimitiveType(p, sup):
    p.check_attrs_end()
    p.check_end()
    return CMOF_PrimitiveType(sup)



def parse_Enumeration(p, sup):
    p.check_attrs_end()

    literals = []

    while p.tag == "ownedLiteral":
        p.push()
        literal = parse_Literal(p)
        p.pop()
        literals.append(literal)

    p.check_end()
    return CMOF_Enumeration(sup, literals)


def parse_Literal(p):
    p.check_cur_tag('ownedLiteral')

    tin = read_type_id_name(p)
    xmi_type = tin[0]
    check_xmi_type(p, xmi_type, "cmof:EnumerationLiteral", "ownedLiteral")

    classifier = p.get_attr_required('classifier')
    enumeration = p.get_attr_required('enumeration')
    p.check_attrs_end()

    p.check_end()
    return CMOF_Literal(tin, classifier, enumeration)


def parse_Association(p, tin):
    memberEnd = p.get_attr_required('memberEnd')
    visibility = read_visibility_required(p)
    p.check_attrs_end()

    end = None
    if p.tag == 'ownedEnd':
        p.push()
        end = parse_End(p)
        p.pop()

    p.check_end()
    return CMOF_Association(tin, memberEnd, visibility, end)


def read_type_card_vis (p):
    type = p.get_attr_opt('type')
    cardinality = read_cardinality(p)
    visibility = read_visibility_opt(p)
    return (type, cardinality, visibility)


def parse_DataType_Attribute(p):
    p.check_cur_tag('ownedAttribute')

    tin = read_type_id_name(p)
    xtype = tin[0]
    check_xmi_type(p, xtype, "cmof:Property", "ownedAttribute")
    tcv = read_type_card_vis(p)
    default = p.get_attr_opt('default')
    datatype = p.get_attr_required('datatype')
    p.check_attrs_end()

    p.check_end()
    return CMOF_DataType_Attribute(tin, tcv, datatype, default)



def parse_Attribute(p):
    p.check_cur_tag('ownedAttribute')

    tin = read_type_id_name(p)
    xtype = tin[0]
    check_xmi_type(p, xtype, "cmof:Property", "ownedAttribute")
    tcv = read_type_card_vis(p)
    bp = read_attr_bool_props(p)
    default = p.get_attr_opt('default')
    subsettedProperty = p.get_attr_opt('subsettedProperty')
    association = p.get_attr_opt('association')
    p.check_attrs_end()

    type_href = None
    if p.tag == 'type':
        p.push()
        type_href = parse_type(p)
        p.pop()

    propeties = parse_properties(p)

    p.check_end()
    return CMOF_Attribute(tin, tcv, bp, default, subsettedProperty, association,
                          type_href, propeties)
    

def read_attr_bool_props(p):
    bp = Attr_bool_props()
    bp.isComposite = p.get_attr_opt('isComposite')
    bp.isReadOnly = p.get_attr_opt('isReadOnly')
    bp.isDerived = p.get_attr_opt('isDerived')
    bp.isDerivedUnion = p.get_attr_opt('isDerivedUnion')
    bp.isOrdered = p.get_attr_opt('isOrdered')
    bp.isUnique = p.get_attr_opt('isUnique')
    return bp


def parse_End(p):
    p.check_cur_tag('ownedEnd')

    tin = read_type_id_name(p)
    xtype = tin[0]
    check_xmi_type(p, xtype, "cmof:Property", "ownedEnd")
    tcv = read_type_card_vis(p)

    owningAssociation = p.get_attr_required('owningAssociation')
    association = p.get_attr_required('association')
    bp = read_end_bool_props(p)
    subsettedProperty = p.get_attr_opt('subsettedProperty')
    p.check_attrs_end()
    
    propeties = parse_properties(p)

    p.check_end()
    return CMOF_End(tin, tcv, owningAssociation, association, bp,
                    subsettedProperty, propeties)


def read_end_bool_props(p):
    bp = End_bool_props()
    bp.isReadOnly = p.get_attr_opt('isReadOnly')
    bp.isDerived = p.get_attr_opt('isDerived')
    bp.isDerivedUnion = p.get_attr_opt('isDerivedUnion')
    return bp



def parse_properties(p):
    redefinedProperty = None
    if p.tag == 'redefinedProperty':
        p.push()
        redefinedProperty = parse_redefinedProperty(p)
        p.pop()

    subsettedProperty = None
    if p.tag == 'subsettedProperty':
        p.push()
        subsettedProperty = parse_subsettedProperty(p)
        p.pop()

    return (redefinedProperty, subsettedProperty)


def parse_Rule(p):
    p.check_cur_tag('ownedRule')

    tin = read_type_id_name(p)
    xtype = tin[0]
    check_xmi_type(p, xtype, "cmof:Constraint", "ownedRule")
    constrainedElement = p.get_attr_required('constrainedElement')
    namespace = p.get_attr_required('namespace')
    p.check_attrs_end()

    p.check_tag('specification')
    p.push()
    specification = parse_rule_specification(p)
    p.pop()

    p.check_end()
    return CMOF_Rule(tin, constrainedElement, namespace, specification)


def parse_rule_specification(p):
    p.check_cur_tag('specification')
    return None


def parse_importedPackage(p):
    p.check_cur_tag('importedPackage')

    xtype, href = read_type_href(p)
    check_xmi_type(p, xtype, 'cmof:Package', 'importedPackage')
    p.check_attrs_end()

    p.check_end()
    return CMOF_ImportedPackage(xtype, href)



def parse_type(p):
    p.check_cur_tag('type')

    xtype, href = read_type_href(p)
    p.check_attrs_end()

    p.check_end()
    return CMOF_type(xtype, href)


def parse_superClass(p):
    p.check_cur_tag('superClass')

    xtype, href = read_type_href(p)
    check_xmi_type(p, xtype, 'cmof:Class', 'superClass')
    p.check_attrs_end()

    p.check_end()
    return CMOF_superClass(xtype, href)


def parse_redefinedProperty(p):
    p.check_cur_tag('redefinedProperty')

    xtype, href = read_type_href(p)
    check_xmi_type(p, xtype, 'cmof:Property', 'redefinedProperty')
    p.check_attrs_end()

    p.check_end()
    return CMOF_redefinedProperty(xtype, href)


def parse_subsettedProperty(p):
    p.check_cur_tag('subsettedProperty')

    xtype, href = read_type_href(p)
    check_xmi_type(p, xtype, 'cmof:Property', 'subsettedProperty')
    p.check_attrs_end()

    p.check_end()
    return CMOF_subsettedProperty(xtype, href)



def parse_Tag(p):
    p.check_cur_tag(ns_cmof + 'Tag')
    id = p.get_attr_required(xmi_id)
    name = p.get_attr_required('name')
    value = p.get_attr_required('value')
    element = p.get_attr_required('element')
    p.check_attrs_end()
    p.check_end()
    return CMOF_Tag(id, name, value, element)


def read_cardinality(p):
    lower = p.get_attr_opt('lower')
    upper = p.get_attr_opt('upper')
    if lower is None and upper is None:
        return None
    return (lower, upper)

def read_visibility_required(p):
    visibility = read_visibility_opt(p)
    if p is None:
        p.error("Required attribute 'visibility' is missing")
    return visibility

def read_visibility_opt(p):
    visibility = p.get_attr_opt('visibility')
    return visibility
