
from matching_classes import *


def match_all(xsd_model, cmof_model, err):
    b  = Matching_Builder(err)

    x_classes = xsd_model.get_classes()
    m_classes = cmof_model.get_classes()

    matching = match_classes(b, x_classes, m_classes)

    return matching

    
def match_classes(b, x_classes, m_classes):
    h = {}
    for x in x_classes:
        x.cmof_class = None
        tname = x.type_name
        name = xsd_name_to_cmof_name(tname)
        assert name not in h
        h[name] = x
        
    for c in m_classes:
        c.xsd_class = None
        name = c.name
        if name in h:
            x = h[name]
            assert x.cmof_class is None
            c.xsd_class = x
            x.cmof_class = c
            b.add_matched_classes(x, c)
        else:
            b.add_unmatch_cmof_class(c)

    rest = [ x for x in x_classes if x.cmof_class is None ]

    for x in rest:
        b.add_unmatched_xsd_class(x)

    return b.build()



def xsd_name_to_cmof_name(tname):
    assert tname[0] == 't'
    return tname[1:]



class Matching_Builder (object):

    __slots__ = [
        '__err',
        '__matched_classes',
        '__unmatched_xsd_classes',
        '__unmatched_cmof_classes'
    ]

    def __init__(self, err):
        self.__err = err
        self.__matched_classes = []
        self.__unmatched_xsd_classes = []
        self.__unmatched_cmof_classes = []

    def warning(self, msg):
        self.__err.warning(msg)

    def error(self, msg):
        self.__err.error(msg)

    def add_matched_classes(self, xsd_class, cmof_class):
        pair = Class_Match_Pair(xsd_class, cmof_class)
        self.__matched_classes.append(pair)

    def add_unmatched_xsd_class(self, xsd_class):
        self.__unmatched_xsd_classes.append(xsd_class)

    def add_unmatch_cmof_class(self, cmof_class):
        self.__unmatched_cmof_classes.append(cmof_class)


    def build(self):
        matching = XSD_CMOF_Matching(self.__matched_classes,
                                     self.__unmatched_xsd_classes,
                                     self.__unmatched_cmof_classes)
        return matching
