
def print_tree_raw(out, tree):
    root = tree.getroot()
    print_subtree (out, root)


def print_subtree(out, node):
    out.write("Node " + repr(node.tag) + " {").nl()
    out.inc()
    out.write("Attrs:").nl()
    out.inc()
    for attr in node.attrib:
        out.write(repr(attr) + ": " + repr(node.attrib[attr])).nl()
    out.dec()
    # out.write("text = " + repr(node.text))
    for child in node:
        out.nl()
        print_subtree(out, child)
    out.dec()
    out.write("}").nl()
    # out.write("tail = " + repr(node.tail))

