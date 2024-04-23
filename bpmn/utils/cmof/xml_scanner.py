

def create_xml_scanner (tree, err_output):
    return XML_Scanner(tree, err_output)


class XML_Scanner (object):

    __slots__ = [
        'tree',
        'err',
        'stack',
        'current',
        'pos',
        'count',
        'child',
        'tag',
        'attrs',
        'text'
    ]

    def __init__(self, tree, err_output):
        self.tree = tree
        self.err = err_output
        self.stack = []
        root = tree.getroot()
        self.__start_node(root)

    def error(self, msg):
        self.err.error("Syntax error: " + msg)

    def has_next(self):
        return (self.child is not None)

    def __start_node(self, node):
        self.current = node
        self.pos = 0
        self.count = len(node)
        self.attrs = Attr_Scanner(node.attrib.copy())
        self.text = node.text
        self.__next_child()

    def next(self):
        assert (self.has_next())
        self.text = self.child.tail
        self.__next_child()

    def __next_child(self):
        assert (self.pos <= self.count)
        if self.pos == self.count:
            self.child = None
            self.tag = None
        else:
            child = self.current[self.pos]
            self.child = child
            self.tag = child.tag
            self.pos += 1
        
    def get_cur_tag(self):
        return self.current.tag

    def get_attrs(self):
        return self.attrs.get_items_remove()

    def get_text(self):
        return self.text

    def push(self):
        assert (self.has_next())
        item = XML_Stack_item(
                    self.current,
                    self.pos,
                    self.count,
                    self.attrs
               )
        self.stack.append(item)
        self.__start_node(self.child)


    def __pop(self):
        cur = self.current
        item = self.stack.pop()
        self.current = item.node
        self.pos = item.pos
        self.count = item.count
        self.attrs = item.attrs
        self.child = cur

    def pop(self):
        self.__pop()
        self.next()


    def check_cur_tag(self, tag):
        self.__check_tag(self.current.tag, tag)

    def check_tag(self, tag):
        self.__check_tag(self.tag, tag)

    def __check_tag(self, cur, tag):
        if cur != tag:
            self.error("<" + tag + "> expected " + "(got " + repr(cur) + ")")

    def check_end(self):
        if self.tag is not None:
            self.error("Unexpected tag " + repr(self.tag))

    def get_attr_opt(self, name):
        return self.attrs.get_attr_opt(name)

    def get_attr_required(self, name):
        v = self.get_attr_opt(name)
        if v is None:
            cur_tag = self.get_cur_tag()
            self.error("Required attribute " + repr(name) + " missing in tag <" + cur_tag + ">")
        return v

    def check_attrs_end(self):
        attrs = self.attrs
        if not attrs.is_empty():
            names = attrs.get_names()
            s = ', '.join([ repr(name) for name in names ])
            cur_tag = self.get_cur_tag()
            if len(names) > 1:
                s2 = "s"
            else:
                s2 = ""
            self.error("Unused attribute" + s2 + " in tag <" + cur_tag + "> (" + s + ")")



class XML_Stack_item (object):

    __slots__ = [
        'node',
        'pos',
        'count',
        'attrs'
    ]

    def __init__(self, node, pos, count, attrs):
        self.node = node
        self.pos = pos
        self.count = count
        self.attrs = attrs


class Attr_Scanner (object):

    __slots__ = [
        'dict'
    ]

    def __init__(self, d):
        self.dict = d

    def get_items(self):
        return self.dict.items()

    def get_items_remove(self):
        d = self.dict
        a = list(self.get_items())
        for (name, v) in a:
            del d[name]
        return a

    def get_names(self):
        return self.dict.keys()

    def is_empty(self):
        return len(self.dict) == 0

    def get_attr_opt(self, name):
        d = self.dict
        if name in d:
            v = d.pop(name)
        else:
            v = None
        return v
        
