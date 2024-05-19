
from matching_classes import *


def print_xsd_cmof_matching(out, matching):
    assert isinstance(matching, XSD_CMOF_Matching)

    out.nl()
    print_title(out, "MATCHED CLASSES")
    print_matched_classes(out, matching.matched_classes)

    out.nl()
    print_title(out, "UNMATCHED CMOF CLASSES")
    print_unmatched_cmof_classes(out, matching.unmatched_cmof_classes)

    out.nl()
    print_title(out, "UNMATCHED XSD CLASSES")
    print_unmatched_xsd_classes(out, matching.unmatched_xsd_classes)



def print_title(out, s):
    out.write("=" * 78).nl()
    out.write("     " + s).nl()
    out.write("=" * 78).nl().nl()



def print_matched_classes(out, pairs):
    for p in pairs:
        print_class_match_pair(out, p)
        out.nl()


def print_class_match_pair(out, p):
    xsd_class = p.xsd_class
    cmof_class = p.cmof_class
    s1 = "Class " + cmof_class.name
    s2 = xsd_class_name(xsd_class)
    out.write("%40s  <-->  %s" % (s1, s2)).nl()


def print_unmatched_cmof_classes(out, classes):
    for c in classes:
        out.write("  - " + c.name).nl().nl()


def print_unmatched_xsd_classes(out, classes):
    for c in classes:
        out.write("  - " + xsd_class_name(c)).nl().nl()


def xsd_class_name(c):
    return "XSD(" + c.element_name + " : " + c.type_name + ")"
