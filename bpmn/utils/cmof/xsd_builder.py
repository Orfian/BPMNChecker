
from xsd_parsed_classes import *
from xsd_model import *
from utils import is_ident, is_int, to_upper


def create_builder(err):
    builder = XSD_Builder(err)
    return builder


def process_parsed_xsd(b, t):
    assert isinstance(t, P_top_node)
    
    visitor = Member_Visitor(b)

    for c in t.members:
        c.visit(visitor)


class Member_Visitor (object):

    __slots__ = [ 'builder' ]

    def __init__(self, builder):
        self.builder = builder

    def visit_Element(self, c):
        assert isinstance(c, P_Element)
        self.builder.add_Element(c)

    def visit_ComplexType(self, c):
        assert isinstance(c, P_ComplexType)
        self.builder.add_ComplexType(c)

    def visit_SimpleType(self, c):
        assert isinstance(c, P_SimpleType)
        self.builder.add_SimpleType(c)



def build_model(b):
    classes = create_elem_type_pairs(b)
    return XSD_Model(classes)


def create_elem_type_pairs(b):
    elements = b.get_elements()
    types = b.get_complex_types()

    h = {}
    for t in types:
        t.marked = False
        name = t.name
        if name in h:
            b.error("Duplicate ComplexType " + repr(name))
        h[name] = t

    pairs = []

    for e in elements:
        name = e.name
        type_name = e.type

        if type_name not in h:
            b.error("Type " + repr(type_name) + " for element " +
                    repr(name) + " not found")

        typ = h[type_name]
        assert typ.name == type_name

        if typ.marked:
            b.error("Type " + repr(type_name) + " used by two elements")
        else:
            typ.marked = True

        pair = create_elem_type_pair(e, typ)

        pairs.append(pair)

    for t in types:
        type_name = t.name
        if not t.marked:
            b.error("Type " + repr(type_name) + " does not have a corresponding elemement")

    return pairs


def create_elem_type_pair(e, t):
    assert isinstance(e, P_Element)
    assert isinstance(t, P_ComplexType)
    ename = e.name
    tname = t.name

    # print ("Creating pair " + repr(ename) + " --- " + repr(tname))
    # tname2 = 't' + to_upper(ename[0]) + ename[1:]
    # if tname2 != tname:
    #     print ("Unmatched pair " + repr(e.name) + " --- " + repr(t.name))

    substitutionGroup = e.substitutionGroup
    element_abstract = e.abstract
    type_abstract = t.abstract
    mixed = t.mixed
    base_name = t.base
    children = t.children
    attrs = t.attrs
    any_attr = t.any_attr

    return XSD_Class(ename, tname, substitutionGroup,
                     element_abstract, type_abstract, mixed, base_name,
                     children, attrs, any_attr)
    



class XSD_Builder (object):

    __slots__ = [
        '__err',
        '__elements',
        '__complex_types',
        '__simple_types'
    ]

    def __init__(self, err):
        self.__err = err
        self.__elements = []
        self.__complex_types = []
        self.__simple_types = []

    def warning(self, msg):
        self.__err.warning(msg)

    def error(self, msg):
        self.__err.error(msg)

    def add_Element(self, c):
        self.__elements.append(c)

    def add_ComplexType(self, c):
        self.__complex_types.append(c)

    def add_SimpleType(self, c):
        self.__simple_types.append(c)

    def get_elements(self):
        return self.__elements

    def get_complex_types(self):
        return self.__complex_types

