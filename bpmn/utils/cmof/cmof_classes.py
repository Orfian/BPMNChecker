

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


class CMOF_Package (object):

    __slots__ = [
        'xmi_id',
        'name',
        'uri',
        'imports',
        'members'
    ]

    def __init__(self, id, name, uri, imports, members):
        self.xmi_id = id
        self.name = name
        self.uri = uri
        self.imports = imports
        self.members = members



class CMOF_Object (object):

    __slots__ = [
        'xmi_type'
    ]

    def __init__(self, xmi_type):
        self.xmi_type = xmi_type



class CMOF_PackageImport (CMOF_Object):

    __slots__ = [
        'xmi_id',
        'importingNamespace',
        'importedPackage'
    ]

    def __init__(self, xmi_type, xmi_id, importingNamespace, importedPackage):
        super().__init__(xmi_type)
        assert self.xmi_type == 'cmof:PackageImport'
        self.xmi_id = xmi_id
        self.importingNamespace = importingNamespace
        self.importedPackage = importedPackage


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

    def name_id_str_par(self, par):
        name = self.name
        id = self.xmi_id
        s = par + '-' + name
        if id == s:
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

    def visit(self, v):
        v.visit_Class(self)


class CMOF_DataType (CMOF_Member):

    __slots__ = [
        'rules',
        'attributes'
    ]

    def __init__(self, tin, rules, attributes):
        super().__init__(tin)
        self.rules = rules
        self.attributes = attributes

    def visit(self, v):
        v.visit_DataType(self)



class CMOF_PrimitiveType (CMOF_Member):

    __slots__ = [ ]

    def __init__(self, tin):
        super().__init__(tin)

    def visit(self, v):
        v.visit_PrimitiveType(self)



class CMOF_Enumeration (CMOF_Member):

    __slots__ = [
        'literals'
    ]

    def __init__(self, tin, literals):
        super().__init__(tin)
        self.literals = literals

    def visit(self, v):
        v.visit_Enumeration(self)


class CMOF_Literal (CMOF_NamedObject):
    
    __slots__ = [
        'classifier',
        'enumeration',
    ]

    def __init__(self, tin, classifier, enumeration):
        super().__init__(tin)
        assert self.xmi_type == 'cmof:EnumerationLiteral'
        assert classifier == enumeration
        self.classifier = classifier
        self.enumeration = enumeration



class CMOF_Association (CMOF_Member):

    __slots__ = [
        'memberEnd',
        'visibility',
        'end'
    ]

    def __init__(self, tin, memberEnd, visibility, end):
        super().__init__(tin)
        self.memberEnd = memberEnd
        self.visibility = visibility
        self.end = end

    def visit(self, v):
        v.visit_Association(self)



class CMOF_Attribute (CMOF_NamedObject):
    
    __slots__ = [
        'type',
        'cardinality',
        'visibility',
        'isComposite',
        'association',
        'attrs',
        'type_href',
        'properties'
    ]

    def __init__(self, tin, type, cardinality, visibility, isComposite, association,
                 attrs, type_href, properties):
        super().__init__(tin)
        assert self.xmi_type == "cmof:Property"
        self.type = type
        self.cardinality = cardinality
        self.visibility = visibility
        self.isComposite = isComposite
        self.association = association
        self.attrs = attrs
        self.type_href = type_href
        self.properties = properties


class CMOF_End (CMOF_NamedObject):
    
    __slots__ = [
        'type',
        'cardinality',
        'visibility',
        'owningAssociation',
        'association',
        'attrs',
        'properties'
    ]

    def __init__(self, tin, type, cardinality, visibility, owningAssociation, association, attrs, properties):
        super().__init__(tin)
        self.type = type
        self.cardinality = cardinality
        self.visibility = visibility
        self.owningAssociation = owningAssociation
        self.association = association
        self.attrs = attrs
        self.properties = properties



class CMOF_Rule (CMOF_NamedObject):

    __slots__ = [
        'constrainedElement',
        'namespace',
        'specification'
    ]

    def __init__(self, tin, constrainedElement, namespace, specification):
        super().__init__(tin)
        assert self.xmi_type == "cmof:Constraint"
        self.constrainedElement = constrainedElement
        self.namespace = namespace
        self.specification = specification



class CMOF_HRef_Object (CMOF_Object):

    __slots__ = [ 'href' ]

    def __init__(self, xtype, href):
        super().__init__(xtype)
        self.href = href


class CMOF_ImportedPackage (CMOF_HRef_Object):

    def __init__(self, xtype, href):
        super().__init__(xtype, href)
        assert self.xmi_type == 'cmof:Package'


class CMOF_superClass (CMOF_HRef_Object):
    
    def __init__(self, xtype, href):
        super().__init__(xtype, href)
        assert xtype == 'cmof:Class'


class CMOF_redefinedProperty (CMOF_HRef_Object):
    
    def __init__(self, xtype, href):
        super().__init__(xtype, href)
        assert xtype == 'cmof:Property'


class CMOF_subsettedProperty (CMOF_HRef_Object):
    
    def __init__(self, xtype, href):
        super().__init__(xtype, href)
        assert xtype == 'cmof:Property'


class CMOF_type (CMOF_HRef_Object):

    def __init__(self, xtype, href):
        super().__init__(xtype, href)
        # assert xtype == 'cmof:Class'


