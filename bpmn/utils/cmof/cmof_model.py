
class M_Model(object):

    __slots__ = [
        'classes',
        'enumerations'
    ]

    def __init__(self, classes, enumerations):
        self.classes = classes
        self.enumerations = enumerations


class M_Type(object):

    def __init__(self):
        pass


class M_Class(M_Type):

    __slots__ = [
        'name'
    ]

    def __init__(self, name):
        self.name = name



class M_Enumeration(M_Type):

    __slots__ = [
        'name'
    ]

    def __init__(self, name):
        self.name = name

