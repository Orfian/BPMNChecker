
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
    return M_Combined_Model(packages)

