

class XSD_CMOF_Matching (object):

    __slots__ = [
        'matched_classes',
        'unmatched_xsd_classes',
        'unmatched_cmof_classes'
    ]

    def __init__(self, matched_classes,
                 unmatched_xsd_classes, unmatched_cmof_classes):
        self.matched_classes = matched_classes
        self.unmatched_xsd_classes = unmatched_xsd_classes
        self.unmatched_cmof_classes = unmatched_cmof_classes



class Class_Match_Pair (object):

    __slots__ = [
        'xsd_class',
        'cmof_class'
    ]

    def __init__(self, xsd_class, cmof_class):
        self.xsd_class = xsd_class
        self.cmof_class = cmof_class


