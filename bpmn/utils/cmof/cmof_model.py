
from utils import is_ident
from enum import Enum


class M_Model(object):

    __slots__ = [
        '__name',
        '__classes',
        '__enumerations',
        '__primitive_types',
        '__data_types',
        '__external_types'
    ]

    def __init__(self, name, classes, enumerations, prim_types, data_types, ext_types):
        assert is_ident(name)
        self.__name = name
        self.__classes = classes
        self.__enumerations = enumerations
        self.__primitive_types = prim_types
        self.__data_types = data_types
        self.__external_types = ext_types

    def get_package_name(self):
        return self.__name

    def get_classes(self):
        return self.__classes

    def get_enumerations(self):
        return self.__enumerations

    def get_primitive_types(self):
        return self.__primitive_types

    def get_data_types(self):
        return self.__data_types

    def get_external_types(self):
        return self.__external_types


class M_Combined_Model(object):

    __slots__ = [
        '__packages',
        '__table',
        '__fname_table'
    ]

    def __init__(self, packages):
        self.__packages = packages
        t = {}
        ft = {}
        for p in packages:
            name = p.get_package_name()
            assert name not in t
            t[name] = p
            fname = p.get_filename()
            assert fname not in ft
            ft[fname] = p
        self.__table = t
        self.__fname_table = ft

    def get_packages(self):
        return self.__packages

    def find_package(self, name):
        t = self.__table
        if name not in t:
            return None
        else:
            return t[name]

    def find_package_by_filename(self, fname):
        ft = self.__fname_table
        if fname not in ft:
            return None
        else:
            return ft[fname]

    def find_type(self, package_name, name):
        p = self.find_package(package_name)
        if p is None:
            return None
        return p.find_type(name)

    def find_type_in_file(self, fname, name):
        p = self.find_package_by_filename(fname)
        if p is None:
            return None
        return p.find_type(name)


class M_Package(object):

    __slots__ = [
        '__name',
        '__filename',
        '__classes',
        '__enumerations',
        '__primitive_types',
        '__data_types',
        '__external_types',
        '__table'
    ]

    def __init__(self, name, filename, classes, enumerations, prim_types, data_types, ext_types):
        assert is_ident(name)
        self.__name = name
        self.__filename = filename
        self.__table = {}

        for c in classes:
            c.set_package(self)
            self.__add_type(c)
        self.__classes = classes

        for c in enumerations:
            c.set_package(self)
            self.__add_type(c)
        self.__enumerations = enumerations

        for c in prim_types:
            c.set_package(self)
            self.__add_type(c)
        self.__primitive_types = prim_types

        for c in data_types:
            c.set_package(self)
            self.__add_type(c)
        self.__data_types = data_types

        for c in ext_types:
            c.set_package(self)
        self.__external_types = ext_types

    def __add_type(self, c):
        name = c.get_name()
        t = self.__table
        assert name not in t
        t[name] = c

    def get_package_name(self):
        return self.__name

    def get_filename(self):
        return self.__filename

    def get_package_filename(self):
        return self.__filenamename

    def get_classes(self):
        return self.__classes

    def get_enumerations(self):
        return self.__enumerations

    def get_primitive_types(self):
        return self.__primitive_types

    def get_data_types(self):
        return self.__data_types

    def get_external_types(self):
        return self.__external_types

    def find_type(self, name):
        t = self.__table
        if name not in t:
            return None
        else:
            return t[name]


class M_Type(object):

    __slots__ = [ 'name', 'package' ]

    def __init__(self, name):
        assert is_ident(name)
        self.name = name
        self.package = None

    def set_package(self, package):
        assert isinstance(package, M_Package)
        self.package = package

    def get_name(self):
        return self.name

    def get_full_name(self):
        name = self.name
        p = self.package
        if p is None:
            return name
        else:
            pname = p.get_package_name()
            return pname + '.' + name


class M_Class(M_Type):

    __slots__ = [
        'xsd_class',
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


class M_Property (object):

    __slots__ = [
        'name',
        'type',
        'cardinality',
        'visibility',
    ]

    def __init__(self, name, typ, card, visibility):
        assert is_ident(name)
        assert isinstance(typ, M_Type)
        assert isinstance(card, M_Cardinality)
        assert isinstance(visibility, Visibility)
        self.name = name
        self.type = typ
        self.cardinality = card
        self.visibility = visibility



class M_Attribute (M_Property):

    __slots__ = [
        'parent',
        'association',
        'assoc_index',
        'props'
    ]

    def __init__(self, name, typ, card, visibility, props):
        super().__init__(name, typ, card, visibility)
        assert isinstance(props, M_Attr_props)
        self.props = props
        self.association = None
        self.assoc_index = -1

    def set_parent(self, parent):
        assert isinstance(parent, M_Class) or isinstance(parent, M_DataType)
        self.parent = parent

    def set_one_way_assoc(self, assoc):
        assert isinstance(assoc, M_One_Way_Association)
        assert self.association is None
        self.association = assoc
        self.assoc_index = 0

    def set_two_way_assoc(self, assoc, index):
        assert isinstance(assoc, M_Two_Way_Association)
        assert self.association is None
        self.association = assoc
        self.assoc_index = index


class M_End (M_Property):

    __slots__ = [
        'parent',
        'props'
    ]

    def __init__(self, name, typ, card, visibility, props):
        super().__init__(name, typ, card, visibility)
        self.name = name
        self.props = props



class M_Attr_props (object):

    __slots__ = [
        'isComposite',
        'isReadOnly',
        'isDerived',
        'isDerivedUnion',
        'isOrdered',
        'isUnique',
        'default'
    ]

    def __init__(self, isComposite, isReadOnly, isDerived, isDerivedUnion,
                 isOrdered, isUnique, default):
        assert isinstance(isComposite, bool)
        assert isinstance(isReadOnly, bool)
        assert isinstance(isDerived, bool)
        assert isinstance(isDerivedUnion, bool)
        assert isinstance(isOrdered, bool)
        assert isinstance(isUnique, bool)
        self.isComposite = isComposite
        self.isReadOnly = isReadOnly
        self.isDerived = isDerived
        self.isDerivedUnion = isDerivedUnion
        self.isOrdered = isOrdered
        self.isUnique = isUnique
        self.default = default



class M_Enumeration (M_Type):

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


class M_DataType(M_Type):

    __slots__ = [
        'attributes'
    ]

    def __init__(self, name):
        super().__init__(name)

    def set_attributes(self, attrs):
        for attr in attrs:
            attr.set_parent(self)
        self.attributes = attrs



class M_HRef_Type(M_Type):

    __slots__ = [ 'href', 'kind', 'type' ]

    def __init__(self, name, href, kind):
        assert is_ident(name)
        super().__init__(name)
        assert isinstance(href, str)
        assert isinstance(kind, Type_Kind)
        self.href = href
        self.kind = kind
        self.type = None

    def set_type(self, c):
        assert isinstance(c, M_Type)
        self.type = c


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


class Visibility (Enum):
    Unknown = 0
    Public = 1
    Private = 2


class M_Association (object):

    __slots__ = [
        'name',
        'visibility',
    ]

    def __init__(self, name, visibility):
        assert isinstance(name, str)
        assert isinstance(visibility, Visibility)
        self.name = name
        self.visibility = visibility



class M_One_Way_Association (M_Association):

    __slots__ = [
        'end'
    ]

    def __init__(self, name, visibility, end):
        super().__init__(name, visibility)
        assert isinstance(end, M_End)
        self.end = end

    def is_one_way(self):
        return True


class M_Two_Way_Association (M_Association):

    __slots__ = [ 'attrs' ]

    def __init__(self, name, visibility):
        super().__init__(name, visibility)

    def is_one_way(self):
        return False

    def set_attrs(self, attrs):
        assert len(attrs) == 2
        for attr in attrs:
            assert isinstance(attr, M_Attribute)
        self.attrs = attrs

    def get_other_attr(self, index):
        return self.attrs[1 - index]

