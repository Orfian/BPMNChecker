
from cmof_classes import *
from cmof_model import *


def is_letter(c):
    x = ord(c)
    return (ord('A') <= x <= ord('Z')) or (ord('a') <= x <= ord('z'))

def is_digit(c):
    x = ord(c)
    return ord('0') <= x <= ord('9')

def is_first_ident_char(c):
    return is_letter(c) or c == '_'

def is_next_ident_char(c):
    return is_letter(c) or is_digit(c) or c == '_'

def is_ident(s):
    if len(s) < 1: return False
    c = s[0]
    if not is_first_ident_char(c): return False
    for c in s[1:]:
        if not is_next_ident_char(c): return False
    return True



def build_model(t, err):
    b = ModelBuilder (err)
    process_top_level_node(b, t)
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


def check_name(b, name, s):
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
    check_name(b, name, s)
    xid = c.xmi_id
    check_id(b, xid, name, s, name)
    check_id_not_used(b, xid, s, name)
    return name


def process_Class(b, c):
    # print ("Processing Class " + repr(c.xmi_id))
    assert isinstance(c, CMOF_Class)
    assert c.xmi_type == 'cmof:Class'
    name = process_member_id(b, c, "Class")
    obj = M_Class(name)
    tmp = b.add_Class(c, obj)


def process_DataType(b, c):
    print ("Processing DataType " + repr(c.xmi_id))
    assert isinstance(c, CMOF_DataType)
    assert c.xmi_type == 'cmof:DataType'


def process_Enumeration(b, c):
    # print ("Processing Enumeration " + repr(c.xmi_id))
    assert isinstance(c, CMOF_Enumeration)
    assert c.xmi_type == 'cmof:Enumeration'
    name = process_member_id(b, c, "Enumeration")
    obj = M_Enumeration(name)
    tmp = b.add_Enumeration(c, obj)


def process_PrimitiveType(b, c):
    print ("Processing PrimitiveType " + repr(c.xmi_id))
    assert isinstance(c, CMOF_PrimitiveType)
    assert c.xmi_type == 'cmof:PrimitiveType'


def process_Association(b, c):
    print ("Processing Association " + repr(c.xmi_id))
    assert isinstance(c, CMOF_Association)
    assert c.xmi_type == 'cmof:Association'




class ModelBuilder (object):

    __slots__ = [
        'err',
        '__id_table',
        '__classes',
        '__enumerations',
        '__type_table',
    ]

    def __init__(self, err):
        self.err = err
        self.__id_table = {}
        self.__classes = []
        self.__enumerations = []
        self.__type_table = {}

    def warning(self, msg):
        self.err.warning(msg)

    def error(self, msg):
        self.err.error(msg)

    def __add_to_id_table(self, xid, tmp):
        h = self.__id_table
        assert xid not in h
        h[xid] = tmp

    def __add_to_type_table(self, xid, tmp):
        h = self.__type_table
        assert xid not in h
        h[xid] = tmp


    def get_obj_by_id_opt(self, xid):
        h = self.__id_table
        if xid in h:
            return h[xid]
        else:
            return None

    def add_Class(self, c, obj):
        name = obj.name
        xid = c.xmi_id
        assert xid == name
        tmp = Tmp_Class(c, obj)
        self.__classes.append(tmp)
        self.__add_to_type_table(xid, tmp)
        self.__add_to_id_table(xid, tmp)
        return tmp

    def add_Enumeration(self, c, obj):
        name = obj.name
        xid = c.xmi_id
        assert xid == name
        tmp = Tmp_Enumeration(c, obj)
        self.__enumerations.append(tmp)
        self.__add_to_type_table(xid, tmp)
        self.__add_to_id_table(xid, tmp)
        return tmp

    def build (self):
        classes = [ tmp.new for tmp in self.__classes ]
        enums = [ tmp.new for tmp in self.__enumerations]
        return M_Model(classes, enums)



class Tmp_Object (object):

    def __init__(self):
        pass


class Tmp_Class (Tmp_Object):

    __slots__ = [ 'old', 'new' ]

    def __init__(self, old, new):
        self.old = old
        self.new = new


class Tmp_Enumeration (Tmp_Object):

    __slots__ = [ 'old', 'new' ]

    def __init__(self, old, new):
        self.old = old
        self.new = new

