
from utils import is_ident
from enum import Enum


class M_Model(object):

    __slots__ = [
        '__name',
        '__classes',
        '__enumerations',
        '__primitive_types',
        '__external_types'
    ]

    def __init__(self, name, classes, enumerations, prim_types, ext_types):
        assert is_ident(name)
        self.__name = name
        self.__classes = classes
        self.__enumerations = enumerations
        self.__primitive_types = prim_types
        self.__external_types = ext_types

    def get_package_name(self):
        return self.__name

    def get_classes(self):
        return self.__classes

    def get_enumerations(self):
        return self.__enumerations

    def get_primitive_types(self):
        return self.__primitive_types

    def get_external_types(self):
        return self.__external_types



class M_Type(object):

    __slots__ = [ 'name' ]

    def __init__(self, name):
        assert is_ident(name)
        self.name = name


class M_Class(M_Type):

    __slots__ = [
        'is_abstract',
        'superclasses',
        'attributes'
    ]

    def __init__(self, name, is_abstract):
        super().__init__(name)
        assert isinstance(is_abstract, bool)
        self.is_abstract = is_abstract

    def set_attributes(self, attrs):
        for attr in attrs:
            attr.set_parent(self)
        self.attributes = attrs

    def set_superclasses(self, superclasses):
        self.superclasses = superclasses


class M_Attribute(object):

    __slots__ = [
        'parent',
        'name',
        'type',
        'cardinality'
    ]

    def __init__(self, name, typ, card):
        assert is_ident(name)
        assert isinstance(typ, M_Type)
        assert isinstance(card, M_Cardinality)
        self.name = name
        self.type = typ
        self.cardinality = card

    def set_parent(self, parent):
        assert isinstance(parent, M_Class)
        self.parent = parent



class M_Enumeration(M_Type):

    __slots__ = [
        'literals'
    ]

    def __init__(self, name, literals):
        super().__init__(name)
        self.literals = [ M_Literal(self, lit) for lit in literals ]



class M_Literal(object):

    __slots__ = [
        'enum',
        'name'
    ]

    def __init__(self, enum, name):
        assert isinstance(enum, M_Enumeration)
        assert is_ident(name)
        self.enum = enum
        self.name = name


class M_PrimitiveType(M_Type):

    __slots__ = [ ]

    def __init__(self, name):
        super().__init__(name)


class M_HRef_Type(M_Type):

    __slots__ = [ 'href', 'kind' ]

    def __init__(self, name, href, kind):
        assert is_ident(name)
        super().__init__(name)
        assert isinstance(href, str)
        assert isinstance(kind, Type_Kind)
        self.href = href
        self.kind = kind


class Type_Kind(Enum):
    Class = 0
    DataType = 1
    Enumeration = 2
    PrimitiveType = 3


class M_Cardinality (object):

    __slots__ = [ 'lower', 'upper' ]

    def __init__(self, lower, upper):
        assert isinstance(lower, int)
        assert isinstance(upper, int)
        assert lower >= 0
        assert upper >= -1
        self.lower = lower
        self.upper = upper

