

class XSD_Model (object):

    __slots__ = [
        '__classes'
    ]

    def __init__(self, classes):
        self.__classes = classes

    def get_classes(self):
        return self.__classes


class XSD_Class (object):

    __slots__ = [
        'cmof_class',
        'element_name',
        'type_name',
        'substitutionGroup',
        'element_abstract',
        'type_abstract',
        'mixed',
        'base_name',
        'children',
        'attrs',
        'any_attr',
    ]

    def __init__(self, element_name, type_name,
                 substitutionGroup, element_abstract, type_abstract,
                 mixed, base_name, children, attrs, any_attr):
        self.element_name = element_name
        self.type_name = type_name
        self.substitutionGroup = substitutionGroup
        self.element_abstract = element_abstract
        self.type_abstract = type_abstract
        self.mixed = mixed
        self.base_name = base_name
        self.children = children
        self.attrs = attrs
        self.any_attr = any_attr

