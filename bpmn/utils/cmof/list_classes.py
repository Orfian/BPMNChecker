
import cmof_model, cmof_print_model
import bpmn_classes_overview 


def print_list(out, model, opts):
    t = prepare_table(model)
    overview = bpmn_classes_overview.overview

    out.nl()
    for package in overview:
        print_package(out, t, package, opts)

    remaining = t.get_remaining()
    if len(remaining) > 0:
        print_remaining(out, remaining, opts)

    out.write("=" * 78).nl()


def print_package(out, t, package, opts):
    out.write("=" * 78).nl()
    out.write("   " + package.get_name() + 
        "  (" + package.get_info() + ")").nl()
    out.write("=" * 78).nl().nl()

    for name, info in package.get_types():
        c = t.find(name)
        assert not c.used
        c.used = True
        c.print(out, info, opts)
        # out.nl()

    out.nl()


def print_remaining(out, remaining, opts):
    out.write("=" * 78).nl()
    out.write("     REMAINING TYPES").nl()
    out.write("=" * 78).nl()
    out.nl()
    for c in remaining:
        c.print(out, None, opts)


def print_type_name(out, s, name, info):
    out.write("  #" + "-" * 74).nl()
    out.write("  #   " + s + " " + name)

    if info is not None:
        out.write("  (" + info + ")")

    out.nl()
    out.write("  #" + "-" * 74).nl()


def print_class(out, c, info, opts):
    print_type_name(out, "class", c.get_name(), info)
    out.nl()
    out.inc()
    cmof_print_model.print_Class(out, c.cl, opts)
    out.dec()
    out.nl()


def print_enum(out, e, info, opts):
    print_type_name(out, "enum", e.get_name(), info)
    out.nl()
    out.inc()
    cmof_print_model.print_Enumeration(out, e.enum, opts)
    out.dec()
    out.nl()



def prepare_table(model):
    t = Table()
    for c in model.get_classes():
        t.add_class(c)

    for e in model.get_enumerations():
        t.add_enum(e)

    return t
    

class Table (object):

    __slots__ = [
        '__types', '__a'
    ]

    def __init__(self):
        self.__a = []
        self.__types = {}

    def __add(self, d):
        t = self.__types
        name = d.get_name()
        assert name not in t
        t[name] = d
        self.__a.append(d)
        
    def add_class(self, c):
        cl = T_Class(c)
        self.__add(cl)

    def add_enum(self, e):
        cl = T_Enum(e)
        self.__add(cl)

    def find(self, name):
        return self.__types[name]

    def get_remaining(self):
        r = [ c for c in self.__a if not c.used ]
        return r


class T_Type (object):

    __slots__ = [
        'used'
    ]

    def __init__(self):
        self.used = False


class T_Class (T_Type):

    __slots__ = [ 'cl' ]

    def __init__(self, c):
        super().__init__()
        self.cl = c

    def get_name(self):
        return self.cl.name

    def print(self, out, info, opts):
        print_class(out, self, info, opts)


class T_Enum (T_Type):

    __slots__ = [ 'enum' ]

    def __init__(self, e):
        super().__init__()
        self.enum = e

    def get_name(self):
        return self.enum.name

    def print(self, out, info, opts):
        print_enum(out, self, info, opts)

