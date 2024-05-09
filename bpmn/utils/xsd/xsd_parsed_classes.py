
class P_top_node (object):

    __slots__ = [
        'elementFormDefault',
        'attributeFormDefault',
        'targetNamespace',
        'imports',
        'includes',
        'members'
    ]

    def __init__(self, elementFormDefault, attributeFormDefault,
                 targetNamespace, imports, includes, members):
        self.elementFormDefault = elementFormDefault
        self.attributeFormDefault = attributeFormDefault
        self.targetNamespace = targetNamespace
        self.imports = imports
        self.includes = includes
        self.members = members


class P_Import (object):

    __slots__ = [
        'namespace',
        'schemaLocation',
    ]

    def __init__(self, namespace, schemaLocation):
        self.namespace = namespace
        self.schemaLocation = schemaLocation


class P_Include (object):

    __slots__ = [
        'schemaLocation',
    ]

    def __init__(self, schemaLocation):
        self.schemaLocation = schemaLocation


class P_Element (object):

    __slots__ = [
        'name',
        'type',
        'substitutionGroup',
        'abstract',
    ]

    def __init__(self, name, typ, subst_group, abstract):
        self.name = name
        self.type = typ
        self.substitutionGroup = subst_group
        self.abstract = abstract

    def visit(self, v):
        v.visit_Element(self)


class P_ComplexType (object):

    __slots__ = [
        'name',
        'abstract',
        'mixed',
        'complexContent'
    ]

    def __init__(self, name, abstract, mixed, complexContent):
        self.name = name
        self.abstract = abstract
        self.mixed = mixed
        self.complexContent = complexContent

    def visit(self, v):
        v.visit_ComplexType(self)


class P_ComplexContent (object):

    __slots__ = [
        'extension'
    ]

    def __init__(self, extension):
        self.extension = extension


class P_Extension (object):

    __slots__ = [
        'base',
    ]

    def __init__(self, base):
        self.base = base



class P_SimpleType (object):

    __slots__ = [
        'name',
    ]

    def __init__(self, name):
        self.name = name

    def visit(self, v):
        v.visit_SimpleType(self)


