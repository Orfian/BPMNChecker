
def print_attrs(out, attrs):
    out.write('attrs:')
    if len(attrs) == 0:
        out.write(" <None>").nl()
    else:
        out.nl()
        out.inc()
        for name, val in attrs:
            out.write(repr(name) + " ==> " + repr(val)).nl()
        out.dec()


def print_array(out, name, arr):
    out.write(name + " = [")
    if len(arr) == 0:
        out.write("]").nl()
    else:
        out.nl().nl()
        out.inc()
        first = True
        for x in arr:
            if first:
                first = False
            else:
                out.nl()
            x.print(out)
        out.dec()
        out.write("]").nl()




class CMOF_top_node (object):

    __slots__ = [
        'version',
        'package',
        'tags'
    ]

    def __init__(self, version, package, tags):
        self.version = version
        self.package = package
        self.tags = tags

    def print (self, out):
        out.write("XMI {").nl()
        out.inc()
        out.write("xmi:version: " + repr(self.version)).nl()
        out.nl()
        self.package.print(out)
        out.nl()
        print_array(out, "tags", self.tags)
        out.dec()
        out.write("}").nl()


class CMOF_Package (object):

    __slots__ = [
        'xmi_id',
        'name',
        'uri',
        'attrs',
        'imports',
        'members'
    ]

    def __init__(self, id, name, uri, imports, members):
        self.xmi_id = id
        self.name = name
        self.uri = uri
        self.imports = imports
        self.members = members

    def print (self, out):
        out.write("Package " + repr(self.name) + " " +
            "(xmi:id = " + repr(self.xmi_id) + ") {").nl()
        out.inc()

        out.nl()
        out.write('uri: ' + repr(self.uri)).nl()
        if len(self.imports) > 0:
            out.nl()
            print_array(out, "imports", self.imports)
        out.nl()
        print_array(out, "members", self.members)

        out.dec()
        out.write("}").nl()



class CMOF_Object (object):

    __slots__ = [
        'xmi_type'
    ]

    def __init__(self, xmi_type):
        self.xmi_type = xmi_type



class CMOF_packageImport (CMOF_Object):

    __slots__ = [
        'xmi_id',
        'importingNamespace',
        'importedPackage'
    ]

    def __init__(self, xmi_type, xmi_id, importingNamespace, importedPackage):
        super().__init__(xmi_type)
        assert xmi_type == 'cmof:PackageImport'
        self.xmi_id = xmi_id
        self.importingNamespace = importingNamespace
        self.importedPackage = importedPackage

    def print (self, out):
        out.write("PackageImport (id = " + repr(self.xmi_id) + ") {").nl()
        out.inc()
        # out.write("xmi:type: " + repr(self.xmi_type)).nl()
        out.write("importingNamespace: " + repr(self.importingNamespace)).nl()
        out.nl()
        self.importedPackage.print(out)
        out.dec()
        out.write("}").nl()



class CMOF_NamedObject (CMOF_Object):
    __slots__ = [
        'xmi_id',
        'name',
    ]

    def __init__(self, tin):
        xtype, xid, name = tin
        super().__init__(xtype)
        self.xmi_id = xid
        self.name = name

    def name_id_str(self):
        name = self.name
        id = self.xmi_id
        if name == id:
            return repr(name)
        else:
            return repr(name) + " (id=" + repr(id) + ")"



class CMOF_Member (CMOF_NamedObject):

    __slots__ = [ ]

    def __init__(self, tin):
        super().__init__(tin)



class CMOF_Class (CMOF_Member):

    __slots__ = [
        'isAbstract',
        'superClass',
        'rules',
        'attributes',
        'superClass2'
    ]

    def __init__(self, tin, isAbstract, superClass, rules, attributes, superClass2):
        super().__init__(tin)
        self.isAbstract = isAbstract
        self.superClass = superClass
        self.rules = rules
        self.attributes = attributes
        self.superClass2 = superClass2

    def print(self, out):
        out.write("Class " + self.name_id_str() + " {").nl()
        out.inc()
        if self.isAbstract is not None:
            out.write("isAbstract: " + repr(self.isAbstract)).nl()
        if self.superClass is not None:
            out.write("superClass: " + repr(self.superClass)).nl()

        if self.superClass2 is not None:
            out.nl()
            self.superClass2.print(out)

        out.nl()
        print_array(out, "attributes", self.attributes)

        if (len(self.rules) > 0):
            out.nl()
            print_array(out, "rules", self.rules)

        out.dec()
        out.write("}").nl()


class CMOF_DataType (CMOF_Member):

    __slots__ = [
        'rules',
        'attributes'
    ]

    def __init__(self, tin, rules, attributes):
        super().__init__(tin)
        self.rules = rules
        self.attributes = attributes

    def print(self, out):
        out.write("DataType " + self.name_id_str() + " {").nl()
        out.inc()

        if (len(self.rules) > 0):
            out.nl()
            print_array(out, "rules", self.rules)

        out.nl()
        print_array(out, "attributes", self.attributes)

        out.dec()
        out.write("}").nl()


class CMOF_PrimitiveType (CMOF_Member):

    __slots__ = [ ]

    def __init__(self, tin):
        super().__init__(tin)

    def print(self, out):
        out.write("PrimitiveType " + self.name_id_str() + " {}").nl()



class CMOF_Enumeration (CMOF_Member):

    __slots__ = [
        'literals'
    ]

    def __init__(self, tin, literals):
        super().__init__(tin)
        self.literals = literals

    def print(self, out):
        out.write("Enumeration " + self.name_id_str() + " {").nl()
        out.inc()

        print_array(out, "literals", self.literals)

        out.dec()
        out.write("}").nl()


class CMOF_Literal (CMOF_NamedObject):
    
    __slots__ = [
        'classifier',
        'enumeration',
    ]

    def __init__(self, tin, classifier, enumeration):
        super().__init__(tin)
        xtype = tin[0]
        assert xtype == 'cmof:EnumerationLiteral'
        assert classifier == enumeration
        self.classifier = classifier
        self.enumeration = enumeration

    def print (self, out):
        out.write("Literal " + self.name_id_str() + " {").nl()
        out.inc()
        out.write("classifier: " + repr(self.classifier)).nl()
        out.write("enumeration: " + repr(self.enumeration)).nl()
        out.dec()
        out.write("}").nl()



class CMOF_Association (CMOF_Member):

    __slots__ = [
        'attrs',
        'end'
    ]

    def __init__(self, tin, attrs, end):
        super().__init__(tin)
        self.attrs = attrs
        self.end = end

    def print(self, out):
        out.write("Association " + self.name_id_str() + " {").nl()
        out.inc()

        print_attrs(out, self.attrs)

        if self.end is not None:
            out.nl()
            self.end.print(out)

        out.dec()
        out.write("}").nl()



class CMOF_Attribute (CMOF_NamedObject):
    
    __slots__ = [
        'attrs',
        'type',
        'properties'
    ]

    def __init__(self, tin, attrs, type, properties):
        super().__init__(tin)
        xtype = tin[0]
        assert xtype == "cmof:Property"
        self.attrs = attrs
        self.type = type
        self.properties = properties

    def print (self, out):
        out.write("Attribute " + self.name_id_str() + " {").nl()
        out.inc()
        print_attrs(out, self.attrs)

        if self.type is not None:
            out.nl()
            self.type.print(out)

        print_properties(out, self.properties)

        out.dec()
        out.write("}").nl()


class CMOF_End (CMOF_NamedObject):
    
    __slots__ = [
        'attrs',
        'properties'
    ]

    def __init__(self, tin, attrs, properties):
        super().__init__(tin)
        self.attrs = attrs
        self.properties = properties

    def print (self, out):
        out.write("End " + self.name_id_str() + " {").nl()
        out.inc()
        print_attrs(out, self.attrs)

        print_properties(out, self.properties)

        out.dec()
        out.write("}").nl()


def print_properties(out, properties):
    redefinedProperty, subsettedProperty = properties
    if redefinedProperty is not None:
        out.nl()
        redefinedProperty.print(out)

    if subsettedProperty is not None:
        out.nl()
        subsettedProperty.print(out)



class CMOF_ImportedPackage (object):

    __slots__ = [
        'xmi_type',
        'href'
    ]

    def __init__(self, xtype, href):
        self.xmi_type = xtype
        assert xtype == 'cmof:Package'
        self.href = href

    def print (self, out):
        out.write("ImportedPackage {").nl()
        out.inc()
        # out.write("xmi:type: " + repr(self.xmi_type)).nl()
        out.write("href: " + repr(self.href)).nl()
        out.dec()
        out.write("}").nl()



class CMOF_type (object):
    
    __slots__ = [
        'xmi_type', 
        'href'
    ]

    def __init__(self, xtype, href):
        self.xmi_type = xtype
        # assert xtype == 'cmof:Class'
        self.href = href

    def print (self, out):
        out.write("type {").nl()
        out.inc()
        out.write("xmi:type: " + repr(self.xmi_type)).nl()
        out.write("href: " + repr(self.href)).nl()
        out.dec()
        out.write("}").nl()


class CMOF_superClass (object):
    
    __slots__ = [
        'xmi_type', 
        'href'
    ]

    def __init__(self, xtype, href):
        self.xmi_type = xtype
        assert xtype == 'cmof:Class'
        self.href = href

    def print (self, out):
        out.write("superClass {").nl()
        out.inc()
        # out.write("xmi:type: " + repr(self.xmi_type)).nl()
        out.write("href: " + repr(self.href)).nl()
        out.dec()
        out.write("}").nl()


class CMOF_redefinedProperty (object):
    
    __slots__ = [
        'attrs'
    ]

    def __init__(self, attrs):
        self.attrs = attrs

    def print (self, out):
        out.write("redefinedProperty {").nl()
        out.inc()
        print_attrs(out, self.attrs)
        out.dec()
        out.write("}").nl()


class CMOF_subsettedProperty (object):
    
    __slots__ = [
        'attrs'
    ]

    def __init__(self, attrs):
        self.attrs = attrs

    def print (self, out):
        out.write("subsettedProperty {").nl()
        out.inc()
        print_attrs(out, self.attrs)
        out.dec()
        out.write("}").nl()


class CMOF_Rule (object):
    
    __slots__ = [
        'attrs'
    ]

    def __init__(self, attrs):
        self.attrs = attrs

    def print (self, out):
        out.write("Rule {").nl()
        out.inc()
        print_attrs(out, self.attrs)
        out.dec()
        out.write("}").nl()





class CMOF_Tag (object):

    __slots__ = [
        'xmi_id',
        'name',
        'value',
        'element',
    ]

    def __init__(self, id, name, value, element):
        self.xmi_id = id
        self.name = name
        self.value = value
        self.element = element

    def print (self, out):
        out.write("Tag (xmi:id = " + repr(self.xmi_id) + ") {").nl()
        out.inc()
        out.write("   name: " + repr(self.name)).nl()
        out.write("  value: " + repr(self.value)).nl()
        out.write("element: " + repr(self.element)).nl()
        out.dec()
        out.write("}").nl()
