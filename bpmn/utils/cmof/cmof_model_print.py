
from cmof_model import *


def print_model(out, model):
    assert isinstance(model, M_Model)
    out.write("Model {").nl()
    out.inc()

    out.write('-' * 60).nl().nl()

    out.write("    (" + str(len(model.classes)) + " classes)").nl().nl()
    for c in model.classes:
        print_Class(out, c)

    out.nl()
    out.write('-' * 60).nl().nl()

    for c in model.enumerations:
        print_Enumeration(out, c)

    out.dec()
    out.write("}").nl()


def print_Class(out, c):
    out.write("Class \"" + c.name + "\"").nl()

def print_Enumeration(out, c):
    out.write("Enumeration \"" + c.name + "\"").nl()
