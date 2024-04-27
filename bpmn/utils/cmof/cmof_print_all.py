
from cmof_classes import *


def print_all(out, c):
    assert isinstance(c, CMOF_top_node)
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
    assert isinstance(c, CMOF_Tag)
    out.write("Tag (id = " + repr(c.xmi_id) + ") {").nl()
    out.inc()
    out.write("   name: " + repr(c.name)).nl()
    out.write("  value: " + repr(c.value)).nl()
    out.write("element: " + repr(c.element)).nl()
    out.dec()
    out.write("}").nl()


def print_Package (out, c):
    assert isinstance(c, CMOF_Package)
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
    assert isinstance(c, CMOF_PackageImport)
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
    assert isinstance(c, CMOF_Member)
    v = Member_Visitor(out)
    c.visit(v)


class Member_Visitor (object):

    __slots__ = [ 'out' ]

    def __init__(self, out):
        self.out = out

    def visit_Class(self, c):
        assert isinstance(c, CMOF_Class)
        print_Class(self.out, c)

    def visit_DataType(self, c):
        assert isinstance(c, CMOF_DataType)
        print_DataType(self.out, c)

    def visit_PrimitiveType(self, c):
        assert isinstance(c, CMOF_PrimitiveType)
        print_PrimitiveType(self.out, c)

    def visit_Enumeration(self, c):
        assert isinstance(c, CMOF_Enumeration)
        print_Enumeration(self.out, c)

    def visit_Association(self, c):
        assert isinstance(c, CMOF_Association)
        print_Association(self.out, c)



def print_Class(out, c):
    assert isinstance(c, CMOF_Class)
    out.write("Class " + c.name_id_str())
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

    print_attributes_and_rules(out, c.attributes, c.rules, c.xmi_id)

    out.dec()
    out.dec()
    out.write("}").nl()


def print_DataType(out, c):
    assert isinstance(c, CMOF_DataType)
    out.write("DataType " + c.name_id_str() + " {").nl()
    out.inc()
    out.inc()

    print_attributes_and_rules(out, c.attributes, c.rules, c.xmi_id)

    out.dec()
    out.dec()
    out.write("}").nl()


def print_attributes_and_rules(out, attributes, rules, par):
    print_array_raw_par(out, attributes, print_Attribute, par)

    if (len(rules) > 0):
        print_array_raw_par(out, rules, print_Rule, par)



def print_PrimitiveType(out, c):
    assert isinstance(c, CMOF_PrimitiveType)
    out.write("PrimitiveType " + c.name_id_str() + " {}").nl()


def print_Enumeration(out, c):
    assert isinstance(c, CMOF_Enumeration)
    out.write("Enumeration " + c.name_id_str() + " {").nl()
    out.inc()

    print_array_raw_par(out, c.literals, print_Literal, c.xmi_id)

    out.dec()
    out.write("}").nl()


def print_Literal (out, c, par):
    assert isinstance(c, CMOF_Literal)
    out.write("Literal " + c.name_id_str_par(par))
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
    assert isinstance(c, CMOF_Association)
    out.write("Association " + c.name_id_str() + " {").nl()
    out.inc()

    out.write("visibility ==> " + repr(c.visibility)).nl()
    out.write("memberEnd ==> " + repr(c.memberEnd)).nl()

    if c.end is not None:
        out.nl()
        print_End(out, c.end, c.xmi_id)

    out.dec()
    out.write("}").nl()



def print_Attribute (out, c, par):
    assert isinstance(c, CMOF_Attribute)
    out.write("Attribute " + c.name_id_str_par(par))
    if c.type is not None:
        out.write(" : " + repr(c.type))
        if c.cardinality is not None:
            out.write(" " + cardinality_to_str(c.cardinality))
    out.write(" {").nl()
    out.inc()
    if c.type_href is not None:
        print_type(out, c.type_href)
    if c.type is None and c.cardinality is not None:
        out.write("cardinality ==> " + cardinality_to_str(c.cardinality)).nl()
    if c.visibility is not None:
        out.write("visibility ==> " + repr(c.visibility)).nl()
    if c.isComposite is not None:
        out.write("isComposite ==> " + repr(c.isComposite)).nl()
    if c.association is not None:
        out.write("association ==> " + repr(c.association)).nl()
    if len(c.attrs) > 0:
        print_attrs(out, c.attrs)

    print_properties(out, c.properties)

    out.dec()
    out.write("}").nl()


def print_End (out, c, par):
    assert isinstance(c, CMOF_End)
    out.write("End " + c.name_id_str_par(par) + " {").nl()
    out.inc()
    out.write("type: " + repr(c.type))
    if c.cardinality is not None:
        out.write(" " + cardinality_to_str(c.cardinality))
    out.nl()
    if c.association != par:
        if (c.owningAssociation == c.association):
            out.write("association (= owningAssociation): " + repr(c.association)).nl()
        else:
            out.write("owningAssociation: " + repr(c.owningAssociation)).nl()
            out.write("      association: " + repr(c.association)).nl()

    if c.visibility is not None:
        out.write("visibility ==> " + repr(c.visibility)).nl()

    assert len(c.attrs) >= 0
    if (len(c.attrs) > 0):
        print_attrs(out, c.attrs)

    print_properties(out, c.properties)

    out.dec()
    out.write("}").nl()



def print_properties(out, properties):
    redefinedProperty, subsettedProperty = properties
    if redefinedProperty is not None:
        out.nl()
        print_redefinedProperty(out, redefinedProperty)

    if subsettedProperty is not None:
        out.nl()
        print_subsettedProperty(out, subsettedProperty)



def print_href_object(out, c, class_name, expected_type):
    assert isinstance(c, CMOF_HRef_Object)
    assert c.xmi_type == expected_type
    out.write(class_name + " {").nl()
    out.inc()
    out.write("href: " + repr(c.href)).nl()
    out.dec()
    out.write("}").nl()


def print_redefinedProperty (out, c):
    assert isinstance(c, CMOF_redefinedProperty)
    print_href_object(out, c, 'redefinedProperty', 'cmof:Property')

def print_subsettedProperty (out, c):
    assert isinstance(c, CMOF_subsettedProperty)
    print_href_object(out, c, 'subsettedProperty', 'cmof:Property')

def print_ImportedPackage (out, c):
    assert isinstance(c, CMOF_ImportedPackage)
    print_href_object(out, c, 'ImportedPackage', 'cmof:Package')

def print_superClass (out, c):
    assert isinstance(c, CMOF_superClass)
    print_href_object(out, c, 'superClass', 'cmof:Class')

def print_type (out, c):
    assert isinstance(c, CMOF_type)
    out.write("type_href (" + repr(c.xmi_type) + ") {").nl()
    out.inc()
    out.write("href: " + repr(c.href)).nl()
    out.dec()
    out.write("}").nl()



def print_Rule (out, c, par):
    assert isinstance(c, CMOF_Rule)
    out.write("Rule " + c.name_id_str_par(par) + " {").nl()
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

