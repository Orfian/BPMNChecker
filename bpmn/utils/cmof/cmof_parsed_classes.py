

class P_top_node (object):

    __slots__ = [
        'version',
        'package',
        'tags'
    ]

    def __init__(self, version, package, tags):
        self.version = version
        self.package = package
        self.tags = tags


class P_Tag (object):

    __slots__ = [
        'xmi_id',
        'name',
        'value',
        'element',
    ]

    def __init__(self, xid, name, value, element):
        self.xmi_id = xid
        self.name = name
        self.value = value
        self.element = element


class P_Package (object):

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



class P_Object (object):

    __slots__ = [
        'xmi_type'
    ]

    def __init__(self, xmi_type):
        self.xmi_type = xmi_type



class P_PackageImport (P_Object):

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


class P_NamedObject (P_Object):
    __slots__ = [
        'xmi_id',
        'name',
    ]

    def __init__(self, tin):
        xtype, xid, name = tin
        super().__init__(xtype)
        self.xmi_id = xid
        self.name = name


class P_Member (P_NamedObject):

    __slots__ = [ ]

    def __init__(self, tin):
        super().__init__(tin)



class P_Class (P_Member):

    __slots__ = [
        'isAbstract',
        'superClass',
        'rules',
        'attributes',
        'superClass2'
    ]

    def __init__(self, tin, isAbstract, superClass, rules, attributes, superClass2):
        super().__init__(tin)
        assert self.xmi_type == 'cmof:Class'
        self.isAbstract = isAbstract
        self.superClass = superClass
        self.rules = rules
        self.attributes = attributes
        self.superClass2 = superClass2

    def visit(self, v):
        v.visit_Class(self)


class P_DataType (P_Member):

    __slots__ = [
        'rules',
        'attributes'
    ]

    def __init__(self, tin, rules, attributes):
        super().__init__(tin)
        assert self.xmi_type == 'cmof:DataType'
        self.rules = rules
        self.attributes = attributes

    def visit(self, v):
        v.visit_DataType(self)



class P_PrimitiveType (P_Member):

    __slots__ = [ ]

    def __init__(self, tin):
        super().__init__(tin)
        assert self.xmi_type == 'cmof:PrimitiveType'

    def visit(self, v):
        v.visit_PrimitiveType(self)



class P_Enumeration (P_Member):

    __slots__ = [
        'literals'
    ]

    def __init__(self, tin, literals):
        super().__init__(tin)
        assert self.xmi_type == 'cmof:Enumeration'
        self.literals = literals

    def visit(self, v):
        v.visit_Enumeration(self)


class P_Literal (P_NamedObject):
    
    __slots__ = [
        'classifier',
        'enumeration',
    ]

    def __init__(self, tin, classifier, enumeration):
        super().__init__(tin)
        assert self.xmi_type == 'cmof:EnumerationLiteral'
        # assert classifier == enumeration
        self.classifier = classifier
        self.enumeration = enumeration



class P_Association (P_Member):

    __slots__ = [
        'memberEnd',
        'visibility',
        'end'
    ]

    def __init__(self, tin, memberEnd, visibility, end):
        super().__init__(tin)
        assert self.xmi_type == 'cmof:Association'
        self.memberEnd = memberEnd
        self.visibility = visibility
        self.end = end

    def visit(self, v):
        v.visit_Association(self)


class P_Property (P_NamedObject):

    __slots__ = [
        'type',
        'cardinality',
        'visibility',
    ]

    def __init__(self, tin, tcv):
        super().__init__(tin)
        assert self.xmi_type == "cmof:Property"
        type, cardinality, visibility = tcv
        self.type = type
        self.cardinality = cardinality
        self.visibility = visibility


class P_DataType_Attribute (P_Property):

    __slots__ = [
        'datatype',
        'default',
    ]

    def __init__(self, tin, tcv, datatype, default):
        super().__init__(tin, tcv)
        self.datatype = datatype
        self.default = default



class P_Attribute (P_Property):

    __slots__ = [
        'bool_props',
        'default',
        'subsettedProperty',
        'association',
        'type_href',
        'properties'
    ]

    def __init__(self, tin, tcv, bp, default, subsettedProperty,
            association, type_href, properties):
        super().__init__(tin, tcv)
        self.bool_props = bp
        self.default = default
        self.subsettedProperty = subsettedProperty
        self.association = association
        self.type_href = type_href
        self.properties = properties


class Attr_bool_props (object):

    __slots__ = [
        'isComposite',
        'isReadOnly',
        'isDerived',
        'isDerivedUnion',
        'isOrdered',
        'isUnique'
    ]



class P_End (P_Property):
    
    __slots__ = [
        'owningAssociation',
        'association',
        'bool_props',
        'subsettedProperty',
        'properties'
    ]

    def __init__(self, tin, tcv, owningAssociation, association, bool_props,
                 subsettedProperty, properties):
        super().__init__(tin, tcv)
        self.owningAssociation = owningAssociation
        self.association = association
        self.bool_props = bool_props
        self.subsettedProperty = subsettedProperty
        self.properties = properties


class End_bool_props (object):

    __slots__ = [
        'isReadOnly',
        'isDerived',
        'isDerivedUnion'
    ]



class P_Rule (P_NamedObject):

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



class P_HRef_Object (P_Object):

    __slots__ = [ 'href' ]

    def __init__(self, xtype, href):
        super().__init__(xtype)
        self.href = href


class P_ImportedPackage (P_HRef_Object):

    def __init__(self, xtype, href):
        super().__init__(xtype, href)
        assert self.xmi_type == 'cmof:Package'


class P_superClass (P_HRef_Object):

    def __init__(self, xtype, href):
        super().__init__(xtype, href)
        assert xtype == 'cmof:Class'


class P_type (P_HRef_Object):

    def __init__(self, xtype, href):
        super().__init__(xtype, href)
        # assert xtype == 'cmof:Class'


class P_redefinedProperty (P_HRef_Object):

    def __init__(self, xtype, href):
        super().__init__(xtype, href)
        assert xtype == 'cmof:Property'


class P_subsettedProperty (P_HRef_Object):

    def __init__(self, xtype, href):
        super().__init__(xtype, href)
        assert xtype == 'cmof:Property'



