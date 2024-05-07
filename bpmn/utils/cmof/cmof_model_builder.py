
from cmof_classes import *
from cmof_model import *
from utils import is_ident



def build_model(t, err):
    b = ModelBuilder (err)
    process_top_level_node(b, t)
    # show_ids(b)
    process_classes(b)
    model = b.build()
    return model


def process_top_level_node(b, t):
    package = t.package

    v = Build_Visitor(b)

    for c in package.members:
        assert isinstance(c, CMOF_Member)
        c.visit(v)


class Build_Visitor (object):

    __slots__ = [ 'builder' ]

    def __init__(self, builder):
        self.builder = builder

    def visit_Class(self, c):
        assert isinstance(c, CMOF_Class)
        process_Class(self.builder, c)

    def visit_DataType(self, c):
        assert isinstance(c, CMOF_DataType)
        process_DataType(self.builder, c)

    def visit_PrimitiveType(self, c):
        assert isinstance(c, CMOF_PrimitiveType)
        process_PrimitiveType(self.builder, c)

    def visit_Enumeration(self, c):
        assert isinstance(c, CMOF_Enumeration)
        process_Enumeration(self.builder, c)

    def visit_Association(self, c):
        assert isinstance(c, CMOF_Association)
        process_Association(self.builder, c)


def check_name_is_ident(b, name, s):
    if is_ident(name): return
    b.error("Name of "  + s + " " + repr(name) + " is not an identifier.")

def check_id(b, xid, expected_id, s, name):
    if xid == expected_id: return
    b.error(s + " " + repr(name) + " (id = " + repr(xid) +
        ") --- id is not as expected (" + repr(expected_id) + ")")

def check_id_not_used(b, xid, s, name):
    tmp = b.get_obj_by_id_opt(xid)
    if tmp is None: return
    b.error(s + " " + repr(name) + " --- id " + repr(xid) + " is already used.")


def process_member_id(b, c, s):
    name = c.name
    check_name_is_ident(b, name, s)
    xid = c.xmi_id
    check_id(b, xid, name, s, name)
    check_id_not_used(b, xid, s, name)
    return name

def process_item_id(b, par_id, c, s):
    name = c.name
    check_name_is_ident(b, name, s + "in " + repr(par_id))
    xid = c.xmi_id
    item_name = par_id + '.' + name
    check_id(b, xid, par_id + '-' + name, s, item_name)
    check_id_not_used(b, xid, s, item_name)
    return name


def class_descr(c, s):
    return "Class " + repr(c.name) + " --- " + s


def process_Class(b, c):
    # print ("Processing Class " + repr(c.xmi_id))
    assert isinstance(c, CMOF_Class)
    assert c.xmi_type == 'cmof:Class'
    name = process_member_id(b, c, "Class")
    is_abstract = read_bool_true(b, c.isAbstract, class_descr(c, "isAbstract"))
    obj = M_Class(name, is_abstract)
    tmp = Tmp_Class(c, obj)

    for attr in c.attributes:
        preprocess_Attribute(b, tmp, attr)

    b.add_Class(tmp)


def preprocess_Attribute(b, tmp_cl, c):
    assert isinstance(tmp_cl, Tmp_Class)
    assert isinstance(c, CMOF_Attribute)
    assert c.xmi_type == 'cmof:Property'
    par_name = tmp_cl.get_id()
    name = process_item_id(b, par_name, c, "Attribute")
    typ = read_class_attr_type(b, tmp_cl, c)
    tmp = Tmp_Attribute(tmp_cl, name, typ)
    tmp_cl.add_attribute(tmp)
    b.add_Attribute(tmp)


def read_class_attr_type(b, tmp_cl, c):
    t = c.type
    if t is not None:
        assert isinstance(t, str)
        typ = Tmp_TypeRef_name(t)
    else:
        typ = None
    return typ


def class_attr_name(cl, attr):
    return cl.get_name() + "." + attr.get_name()


def process_classes(b):
    for cl in b.get_classes():
        process_one_class(b, cl)


def process_one_class(b, cl):
    attrs = []
    for attr in cl.get_attributes():
        obj = process_class_attribute(b, cl, attr)
        attrs.append(obj)
    cl.set_obj_attributes(attrs)


def process_class_attribute(b, cl, attr):
    name = attr.get_name()
    typ = find_class_attribute_type(b, cl, attr)
    obj = M_Attribute(name, typ)
    return obj


def find_class_attribute_type(b, cl, attr):
    t = attr.get_type()
    assert isinstance(t, Tmp_TypeRef) or t is None
    if t is None or t.is_href():
        typ = None
    else:
        assert isinstance(t, Tmp_TypeRef_name)
        tname = t.name
        typ = b.find_type_by_name(tname)
        if typ is None:
            b.error("Attribute " + class_attr_name(cl, attr) + " --- " +
            "type " + repr(tname) + " not found")
    return typ


def process_DataType(b, c):
    print ("Processing DataType " + repr(c.xmi_id))
    assert isinstance(c, CMOF_DataType)
    assert c.xmi_type == 'cmof:DataType'


def process_Enumeration(b, c):
    # print ("Processing Enumeration " + repr(c.xmi_id))
    assert isinstance(c, CMOF_Enumeration)
    assert c.xmi_type == 'cmof:Enumeration'
    name = process_member_id(b, c, "Enumeration")
    tmp = Tmp_Enumeration(name)

    for literal in c.literals:
        process_Literal(b, tmp, literal)

    test_enumeration_duplicates(b, tmp)

    b.add_Enumeration(tmp)
    
    obj = M_Enumeration (tmp.get_name(), tmp.get_literal_names())
    tmp.set_obj(obj)


def test_enumeration_duplicates(b, tmp):
    assert isinstance(tmp, Tmp_Enumeration)
    h = {}
    for name in tmp.get_literal_names():
        if name in h:
            b.error("Enum " + tmp.get_name() + " --- duplicate item " + repr(name))
        h[name] = name


def process_Literal(b, enum, c):
    assert isinstance(enum, Tmp_Enumeration)
    assert isinstance(c, CMOF_Literal)
    assert c.xmi_type == 'cmof:EnumerationLiteral'
    par_name = enum.get_id()
    name = process_item_id(b, par_name, c, "Enumeration Literal")

    if c.classifier != par_name:
        b.error("Enum Literal \'" + name + "\' in \'" + par_name + '\': ' +
            "classifier " + repr(c.classifier) + " is not as expected (\'" +
            par_name + "\')"
        )

    if c.enumeration != par_name:
        b.error("Enum Literal \'" + name + "\' in \'" + par_name + '\': ' +
            "enumeration " + repr(c.enumeration) + " is not as expected (\'" +
            par_name + "\')"
        )

    tmp = enum.add_literal(name)
    b.add_Literal(tmp)


def process_PrimitiveType(b, c):
    # print ("Processing PrimitiveType " + repr(c.xmi_id))
    assert isinstance(c, CMOF_PrimitiveType)
    assert c.xmi_type == 'cmof:PrimitiveType'
    name = process_member_id(b, c, "PrimitiveType")
    obj = M_PrimitiveType(name)
    tmp = Tmp_PrimitiveType(obj)
    b.add_PrimitiveType(tmp)


def process_Association(b, c):
    # print ("Processing Association " + repr(c.xmi_id))
    assert isinstance(c, CMOF_Association)
    assert c.xmi_type == 'cmof:Association'
    name = process_member_id(b, c, "Association")
    # if c.end is None:
    #     obj = process_two_way_Association(name, b, c)
    # else:
    #     obj = process_one_way_Association(name, b, c)


def show_ids(b):
    h = b.get_id_table()
    print ("Id table:")
    print ("=========")
    for xid in h:
        tmp = h[xid]
        print ("   " + repr(xid) + " ==> " + tmp.get_short_descr())


def read_bool_true(b, v, s):
    if v is None: return False
    if v == 'true': return True
    b.error(s + ": \'true\' expected, got " + repr(v))
    return False


class ModelBuilder (object):

    __slots__ = [
        'err',
        '__id_table',
        '__classes',
        '__enumerations',
        '__primitive_types',
        '__type_table',
        '__associations',
        '__assoc_table'
    ]

    def __init__(self, err):
        self.err = err
        self.__id_table = {}
        self.__classes = []
        self.__enumerations = []
        self.__primitive_types = []
        self.__type_table = {}
        self.__associations = []
        self.__assoc_table = {}

    def warning(self, msg):
        self.err.warning(msg)

    def error(self, msg):
        self.err.error(msg)

    def get_id_table(self):
        return self.__id_table

    def __add_to_id_table(self, xid, tmp):
        h = self.__id_table
        assert xid not in h
        h[xid] = tmp

    def __add_to_type_table(self, xid, tmp):
        # print ("Adding to type table: " + repr(xid) + " ==> " + 
        #         tmp.get_short_descr())
        h = self.__type_table
        assert xid not in h
        h[xid] = tmp

    def find_type_by_name(self, name):
        # print ("Find type by name: " + repr(name))
        h = self.__type_table
        if name not in h: return None
        # print ("      ... found")
        tmp = h[name]
        return tmp.get_type_obj()

    def get_obj_by_id_opt(self, xid):
        h = self.__id_table
        if xid in h:
            return h[xid]
        else:
            return None

    def add_Class(self, tmp):
        xid = tmp.get_id()
        name = tmp.get_name()
        assert xid == name
        self.__classes.append(tmp)
        self.__add_to_type_table(xid, tmp)
        self.__add_to_id_table(xid, tmp)

    def add_Attribute(self, tmp):
        assert isinstance(tmp, Tmp_Attribute)
        xid = tmp.get_id()
        self.__add_to_id_table(xid, tmp)

    def add_Enumeration(self, tmp):
        assert isinstance(tmp, Tmp_Enumeration)
        xid = tmp.get_id()
        name = tmp.get_name()
        assert xid == name
        self.__enumerations.append(tmp)
        self.__add_to_type_table(xid, tmp)
        self.__add_to_id_table(xid, tmp)
        return tmp

    def add_Literal(self, tmp):
        assert isinstance(tmp, Tmp_Literal)
        xid = tmp.get_id()
        self.__add_to_id_table(xid, tmp)

    def add_PrimitiveType(self, tmp):
        assert isinstance(tmp, Tmp_PrimitiveType)
        xid = tmp.get_id()
        name = tmp.get_name()
        assert xid == name
        self.__primitive_types.append(tmp)
        self.__add_to_type_table(xid, tmp)
        self.__add_to_id_table(xid, tmp)


    def add_Association(self, c, obj):
        name = obj.name
        xid = c.xmi_id
        assert xid == name
        tmp = Tmp_Association(c, obj)
        self.__associations.append(tmp)
        self.__add_to_assoc_table(xid, tmp)
        self.__add_to_id_table(xid, tmp)
        return tmp

    def get_classes(self):
        return self.__classes
        
    def build (self):
        classes = [ tmp.get_obj() for tmp in self.__classes ]
        enums = [ tmp.get_obj() for tmp in self.__enumerations ]
        prim_types = [ tmp.get_obj() for tmp in self.__primitive_types ]
        return M_Model(classes, enums, prim_types)



class Tmp_Object (object):

    def __init__(self):
        pass

    def get_id(self):
        assert False

    def get_short_descr(self):
        assert False


class Tmp_Class (Tmp_Object):

    __slots__ = [ '__orig', '__obj', '__attributes' ]

    def __init__(self, orig, obj):
        self.__orig = orig
        self.__obj = obj
        self.__attributes = []

    def get_id(self):
        return self.__orig.xmi_id

    def get_name(self):
        return self.__obj.name

    def get_short_descr(self):
        return "class " + self.get_name()

    def add_attribute(self, attr):
        assert isinstance(attr, Tmp_Attribute)
        assert attr.get_parent() is self
        self.__attributes.append(attr)

    def get_attributes(self):
        return self.__attributes

    def set_obj_attributes(self, attrs):
        obj = self.__obj
        obj.set_attributes(attrs)

    def get_obj(self):
        return self.__obj

    def get_type_obj(self):
        return self.get_obj()


class Tmp_Attribute(Tmp_Object):

    __slots__ = [ '__parent', '__name', '__type' ]

    def __init__(self, parent, name, typ):
        assert isinstance(parent, Tmp_Class)
        assert isinstance(name, str)
        self.__parent = parent
        self.__name = name
        self.__type = typ

    def get_parent(self):
        return self.__parent

    def get_id(self):
        return self.__parent.get_id() + '-' + self.get_name()

    def get_name(self):
        return self.__name

    def get_long_name(self):
        return self.__parent.get_name() + '.' + self.get_name()

    def get_type(self):
        return self.__type

    def get_short_descr(self):
        return "attribute " + self.get_long_name()




class Tmp_Enumeration (Tmp_Object):

    __slots__ = [ '__name', '__literals', '__obj' ]

    def __init__(self, name):
        assert isinstance(name, str)
        self.__name = name
        self.__literals = []

    def get_id(self):
        return self.__name

    def get_name(self):
        return self.__name

    def get_short_descr(self):
        return "enum " + self.get_name()

    def add_literal(self, name):
        assert isinstance(name, str)
        literal = Tmp_Literal(self, name)
        self.__literals.append(literal)
        return literal

    def get_literal_names(self):
        return [ lit.get_name() for lit in self.__literals ]

    def set_obj(self, obj):
        self.__obj = obj

    def get_obj(self):
        return self.__obj

    def get_type_obj(self):
        return self.get_obj()


class Tmp_Literal (Tmp_Object):

    __slots__ = [ '__enum', '__name' ]

    def __init__(self, enum, name):
        assert isinstance(enum, Tmp_Enumeration)
        assert isinstance(name, str)
        self.__enum = enum
        self.__name = name

    def get_id(self):
        par = self.__enum
        return par.get_id() + '-' + self.__name

    def get_name(self):
        return self.__name

    def get_long_name(self):
        return self.__enum.get_name() + '.' + self.get_name()

    def get_short_descr(self):
        return "literal " + self.get_long_name()


class Tmp_PrimitiveType(Tmp_Object):

    __slots__ = [ '__obj' ]

    def __init__(self, obj):
        assert isinstance(obj, M_PrimitiveType)
        self.__obj = obj

    def get_id(self):
        return self.__obj.name

    def get_name(self):
        return self.__obj.name

    def get_obj(self):
        return self.__obj

    def get_short_descr(self):
        return "primitive type " + self.get_name()


class Tmp_OneWay_Association(Tmp_Object):

    pass


class Tmp_TypeRef(object):
    
    pass


class Tmp_TypeRef_name(Tmp_TypeRef):

    __slots__ = [ 'name' ]

    def __init__(self, name):
        self.name = name


    def is_href(self):
        return False
