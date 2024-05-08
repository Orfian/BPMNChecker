
from cmof_parsed_classes import *


def print_all(out, c):
    assert isinstance(c, P_top_node)
    out.write("XMI {").nl()
    out.inc()
    out.write("xmi:version: " + repr(c.version)).nl()
    out.nl()
    print_Package(out, c.package)
    out.nl()
    print_array(out, "tags", c.tags, print_Tag)
    out.dec()
    out.write("}").nl()


def print_Tag (out, c):
    assert isinstance(c, P_Tag)
    out.write("Tag (id = " + repr(c.xmi_id) + ") {").nl()
    out.inc()
    out.write("   name: " + repr(c.name)).nl()
    out.write("  value: " + repr(c.value)).nl()
    out.write("element: " + repr(c.element)).nl()
    out.dec()
    out.write("}").nl()


def print_Package (out, c):
    assert isinstance(c, P_Package)
    out.write("Package " + repr(c.name) + " " +
        "(id = " + repr(c.xmi_id) + ") {").nl()
    out.inc()

    out.nl()
    out.write('uri: ' + repr(c.uri)).nl()
    if len(c.imports) > 0:
        out.nl()
        print_array(out, "imports", c.imports, print_PackageImport)
    out.nl()
    print_array(out, "members", c.members, print_Member)

    out.dec()
    out.write("}").nl()


def print_PackageImport (out, c):
    assert isinstance(c, P_PackageImport)
    out.write("PackageImport (id = " + repr(c.xmi_id) + ") {").nl()
    out.inc()
    # out.write("xmi:type: " + repr(self.xmi_type)).nl()
    assert c.xmi_type == 'cmof:PackageImport'
    out.write("importingNamespace: " + repr(c.importingNamespace)).nl()
    out.nl()
    print_ImportedPackage(out, c.importedPackage)
    out.dec()
    out.write("}").nl()


def print_Member(out, c):
    assert isinstance(c, P_Member)
    v = Member_Visitor(out)
    c.visit(v)


class Member_Visitor (object):

    __slots__ = [ 'out' ]

    def __init__(self, out):
        self.out = out

    def visit_Class(self, c):
        assert isinstance(c, P_Class)
        print_Class(self.out, c)

    def visit_DataType(self, c):
        assert isinstance(c, P_DataType)
        print_DataType(self.out, c)

    def visit_PrimitiveType(self, c):
        assert isinstance(c, P_PrimitiveType)
        print_PrimitiveType(self.out, c)

    def visit_Enumeration(self, c):
        assert isinstance(c, P_Enumeration)
        print_Enumeration(self.out, c)

    def visit_Association(self, c):
        assert isinstance(c, P_Association)
        print_Association(self.out, c)



def print_Class(out, c):
    assert isinstance(c, P_Class)
    out.write("Class " + name_id_str(c))
    if c.superClass is not None:
        out.write(" extends " + repr(c.superClass))
    out.write(" {").nl()
    out.inc()
    out.inc()
    if c.isAbstract is not None:
        out.write("isAbstract: " + repr(c.isAbstract)).nl()

    if c.superClass2 is not None:
        out.nl()
        print_superClass(out, c.superClass2)

    par = c.xmi_id

    print_array_raw_par(out, c.attributes, print_Attribute, par)

    if (len(c.rules) > 0):
        print_array_raw_par(out, c.rules, print_Rule, par)

    out.dec()
    out.dec()
    out.write("}").nl()


def print_DataType(out, c):
    assert isinstance(c, P_DataType)
    out.write("DataType " + name_id_str(c) + " {").nl()
    out.inc()
    out.inc()

    par = c.xmi_id

    print_array_raw_par(out, c.attributes, print_DataType_Attribute, par)

    if (len(c.rules) > 0):
        print_array_raw_par(out, c.rules, print_Rule, par)


    out.dec()
    out.dec()
    out.write("}").nl()



def print_PrimitiveType(out, c):
    assert isinstance(c, P_PrimitiveType)
    out.write("PrimitiveType " + name_id_str(c) + " {}").nl()


def print_Enumeration(out, c):
    assert isinstance(c, P_Enumeration)
    out.write("Enumeration " + name_id_str(c) + " {").nl()
    out.inc()

    print_array_raw_par(out, c.literals, print_Literal, c.xmi_id)

    out.dec()
    out.write("}").nl()


def print_Literal (out, c, par):
    assert isinstance(c, P_Literal)
    out.write("Literal " + name_id_str_par(c, par))
    if c.classifier == par and c.enumeration == par:
        out.nl()
    else:
        out.write(" {").nl()
        out.inc()
        out.write("classifier: " + repr(c.classifier)).nl()
        out.write("enumeration: " + repr(c.enumeration)).nl()
        out.dec()
        out.write("}").nl()



def print_Association(out, c):
    assert isinstance(c, P_Association)
    out.write("Association " + name_id_str(c) + " {").nl()
    out.inc()

    out.write("visibility ==> " + repr(c.visibility)).nl()
    out.write("memberEnd ==> " + repr(c.memberEnd)).nl()

    if c.end is not None:
        out.nl()
        print_End(out, c.end, c.xmi_id)

    out.dec()
    out.write("}").nl()


def print_property_header(out, title, c, par):
    assert isinstance(c, P_Property)
    out.write(title + " " + name_id_str_par(c, par))
    if c.type is not None:
        out.write(" : " + repr(c.type))
        if c.cardinality is not None:
            out.write(" " + cardinality_to_str(c.cardinality))
    out.write(" {")


def print_property_visibility(out, c):
    assert isinstance(c, P_Property)
    if c.visibility is not None:
        out.write("visibility ==> " + repr(c.visibility)).nl()


def print_DataType_Attribute (out, c, par):
    assert isinstance(c, P_DataType_Attribute)
    print_property_header(out, "Attribute", c, par)
    out.nl()
    out.inc()

    print_property_visibility(out, c)
    if c.default is not None:
        out.write("default ==> " + repr(c.default)).nl()
    if c.datatype != par:
        out.write("datatype ==> " + repr(c.datatype)).nl()

    out.dec()
    out.write("}").nl()



def print_Attribute (out, c, par):
    assert isinstance(c, P_Attribute)
    print_property_header(out, "Attribute", c, par)
    out.nl()
    out.inc()
    if c.type_href is not None:
        print_type(out, c.type_href)
    if c.type is None and c.cardinality is not None:
        out.write("cardinality ==> " + cardinality_to_str(c.cardinality)).nl()
    print_property_visibility(out, c)
    print_attr_bool_props(out, c.bool_props)
    if c.default is not None:
        out.write("default ==> " + repr(c.default)).nl()
    if c.subsettedProperty is not None:
        out.write("subsettedProperty ==> " + repr(c.subsettedProperty)).nl()
    if c.association is not None:
        out.write("association ==> " + repr(c.association)).nl()

    print_properties(out, c.properties)

    out.dec()
    out.write("}").nl()


def print_bool_prop(out, b, name):
    if b is not None:
        out.write(name + " ==> " + repr(b)).nl()


def print_attr_bool_props(out, bp):
    print_bool_prop(out, bp.isComposite, "isComposite")
    print_bool_prop(out, bp.isReadOnly, "isReadOnly")
    print_bool_prop(out, bp.isDerived, "isDerived")
    print_bool_prop(out, bp.isDerivedUnion, "isDerivedUnion")
    print_bool_prop(out, bp.isOrdered, "isOrdered")
    print_bool_prop(out, bp.isUnique, "isUnique")


def print_End (out, c, par):
    assert isinstance(c, P_End)
    print_property_header(out, "End", c, par)
    out.nl()
    out.inc()
    if c.association != par:
        if (c.owningAssociation == c.association):
            out.write("association (= owningAssociation): " + repr(c.association)).nl()
        else:
            out.write("owningAssociation: " + repr(c.owningAssociation)).nl()
            out.write("      association: " + repr(c.association)).nl()

    print_property_visibility(out, c)

    print_end_bool_props(out, c.bool_props)
    if c.subsettedProperty is not None:
        out.write("subsettedProperty ==> " + repr(c.subsettedProperty)).nl()

    print_properties(out, c.properties)

    out.dec()
    out.write("}").nl()


def print_end_bool_props(out, bp):
    print_bool_prop(out, bp.isReadOnly, "isReadOnly")
    print_bool_prop(out, bp.isDerived, "isDerived")
    print_bool_prop(out, bp.isDerivedUnion, "isDerivedUnion")



def print_properties(out, properties):
    redefinedProperty, subsettedProperty = properties
    if redefinedProperty is not None:
        out.nl()
        print_redefinedProperty(out, redefinedProperty)

    if subsettedProperty is not None:
        out.nl()
        print_subsettedProperty(out, subsettedProperty)



def print_href_object(out, c, class_name, expected_type):
    assert isinstance(c, P_HRef_Object)
    assert c.xmi_type == expected_type
    out.write(class_name + " {").nl()
    out.inc()
    out.write("href: " + repr(c.href)).nl()
    out.dec()
    out.write("}").nl()


def print_redefinedProperty (out, c):
    assert isinstance(c, P_redefinedProperty)
    print_href_object(out, c, 'redefinedProperty', 'cmof:Property')

def print_subsettedProperty (out, c):
    assert isinstance(c, P_subsettedProperty)
    print_href_object(out, c, 'subsettedProperty', 'cmof:Property')

def print_ImportedPackage (out, c):
    assert isinstance(c, P_ImportedPackage)
    print_href_object(out, c, 'ImportedPackage', 'cmof:Package')

def print_superClass (out, c):
    assert isinstance(c, P_superClass)
    print_href_object(out, c, 'superClass', 'cmof:Class')

def print_type (out, c):
    assert isinstance(c, P_type)
    out.write("type (" + repr(c.xmi_type) + ") {").nl()
    out.inc()
    out.write("href: " + repr(c.href)).nl()
    out.dec()
    out.write("}").nl()



def print_Rule (out, c, par):
    assert isinstance(c, P_Rule)
    out.write("Rule " + name_id_str_par(c, par) + " {").nl()
    out.inc()
    out.write("constrainedElement: " + repr(c.constrainedElement)).nl()
    out.write("         namespace: " + repr(c.namespace)).nl()
    out.dec()
    out.write("}").nl()


def cardinality_to_str(c):
    lower, upper = c
    s = "["
    if lower is None:
        s += '_'
    else:
        s += lower
    s += ' .. '
    if upper is None:
        s += '_'
    else:
        s += upper
    s += ']'
    return s





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


def print_array(out, name, arr, pr):
    out.write(name + " = [")
    if len(arr) == 0:
        out.write("]").nl()
    else:
        out.nl()
        out.inc()
        print_array_raw(out, arr, pr)
        out.dec()
        out.write("]").nl()


def print_array_raw(out, arr, pr):
    if len(arr) == 0:
        return
    for x in arr:
        out.nl()
        pr(out, x)


def print_array_raw_par(out, arr, pr, par):
    if len(arr) == 0:
        return
    for x in arr:
        out.nl()
        pr(out, x, par)


def name_id_str(c):
    assert isinstance(c, P_NamedObject)
    name = c.name
    id = c.xmi_id
    if name == id:
        return repr(name)
    else:
        return repr(name) + " (id=" + repr(id) + ")"


def name_id_str_par(c, par):
    assert isinstance(c, P_NamedObject)
    name = c.name
    id = c.xmi_id
    s = par + '-' + name
    if id == s:
        return repr(name)
    else:
        return repr(name) + " (id=" + repr(id) + ")"


