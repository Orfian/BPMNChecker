
from cmof_model import *


def create_package(fname, model):
    name       = model.get_package_name()
    classes    = model.get_classes()
    enums      = model.get_enumerations()
    prim_types = model.get_primitive_types()
    data_types = model.get_data_types()
    ext_types  = model.get_external_types()

    package = M_Package(name, fname, classes, enums, prim_types,
                         data_types, ext_types)

    return package


def create_model(packages):
    model = M_Combined_Model(packages)

    for p in model.get_packages():
        for c in p.get_external_types():
            process_ext_type(model, c)

    return model


def process_ext_type(model, c):
    p = c.package
    # print ("Processing " + p.get_package_name() + "." + c.name)
    fname, name = decompose_href(c.href)
    # print ("  fname=" + repr(fname) + ", name=" + repr(name))
    t = model.find_type_in_file(fname, name)
    if t is not None:
        # print ("   FOUND: " + t.get_full_name())
        c.set_type(t)


def decompose_href(href):
    r = href.split('#')
    assert (len(r) == 2)
    fname = r[0]
    name = r[1]
    return (fname, name)

