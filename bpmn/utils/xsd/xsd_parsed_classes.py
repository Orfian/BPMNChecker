
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


class P_Element (object):

    __slots__ = [
        'name',
        'attrs'
    ]

    def __init__(self, name, attrs):
        self.name = name
        self.attrs = attrs

    def visit(self, v):
        v.visit_Element(self)


class P_ComplexType (object):

    __slots__ = [
        'name',
        'attrs'
    ]

    def __init__(self, name, attrs):
        self.name = name
        self.attrs = attrs

    def visit(self, v):
        v.visit_ComplexType(self)


class P_SimpleType (object):

    __slots__ = [
        'name',
        'attrs'
    ]

    def __init__(self, name, attrs):
        self.name = name
        self.attrs = attrs

    def visit(self, v):
        v.visit_SimpleType(self)
