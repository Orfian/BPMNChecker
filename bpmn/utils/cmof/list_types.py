
class Package (object):

    __slots__ = [
        'name', 'info', 'types'
    ]

    def __init__(self, name, info, types):
        self.name = name
        self.info = info
        self.types = types

    def get_name(self):
        return self.name

    def get_info(self):
        return self.info

    def get_types(self):
        return self.types
