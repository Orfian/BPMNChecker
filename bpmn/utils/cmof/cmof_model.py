
from utils import is_ident


class M_Model(object):

    __slots__ = [
        '__classes',
        '__enumerations',
        '__primitive_types'
    ]

    def __init__(self, classes, enumerations, prim_types):
        self.__classes = classes
        self.__enumerations = enumerations
        self.__primitive_types = prim_types

    def get_classes(self):
        return self.__classes

    def get_enumerations(self):
        return self.__enumerations

    def get_primitive_types(self):
        return self.__primitive_types


class M_Type(object):

    def __init__(self):
        pass


class M_Class(M_Type):

    __slots__ = [
        'name'
    ]

    def __init__(self, name):
        assert is_ident(name)
        self.name = name

    def set_attributes(self, attrs):
        for attr in attrs:
            attr.set_parent(self)
        self.attributes = attrs



class M_Attribute(object):

    __slots__ = [
        'parent',
        'name'
    ]

    def __init__(self, name):
        assert is_ident(name)
        self.name = name

    def set_parent(self, parent):
        self.parent = parent



class M_Enumeration(M_Type):

    __slots__ = [
        'name', 
        'literals'
    ]

    def __init__(self, name, literals):
        assert is_ident(name)
        self.name = name
        self.literals = [ M_Literal(self, lit) for lit in literals ]



class M_Literal(object):

    __slots__ = [
        'enum',
        'name'
    ]

    def __init__(self, enum, name):
        assert isinstance(enum, M_Enumeration)
        assert isinstance(name, str)
        assert is_ident(name)
        self.enum = enum
        self.name = name


class M_PrimitiveType(M_Type):

    __slots__ = [
        'name'
    ]

    def __init__(self, name):
        assert is_ident(name)
        self.name = name
