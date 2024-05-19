
class P_top_node (object):

    __slots__ = [
        'elementFormDefault',
        'attributeFormDefault',
        'targetNamespace',
        'imports',
        'includes',
        'members'
    ]

    def __init__(self, elementFormDefault, attributeFormDefault,
                 targetNamespace, imports, includes, members):
        self.elementFormDefault = elementFormDefault
        self.attributeFormDefault = attributeFormDefault
        self.targetNamespace = targetNamespace
        self.imports = imports
        self.includes = includes
        self.members = members


class P_Import (object):

    __slots__ = [
        'namespace',
        'schemaLocation',
    ]

    def __init__(self, namespace, schemaLocation):
        self.namespace = namespace
        self.schemaLocation = schemaLocation


class P_Include (object):

    __slots__ = [
        'schemaLocation',
    ]

    def __init__(self, schemaLocation):
        self.schemaLocation = schemaLocation


class P_Element (object):

    __slots__ = [
        'name',
        'type',
        'substitutionGroup',
        'abstract',
    ]

    def __init__(self, name, typ, subst_group, abstract):
        self.name = name
        self.type = typ
        self.substitutionGroup = subst_group
        self.abstract = abstract

    def visit(self, v):
        v.visit_Element(self)

    def short_descr(self):
        return "Element " + repr(self.name)


class P_ComplexType (object):

    __slots__ = [
        'name',
        'abstract',
        'mixed',
        'base',
        'children',
        'attrs',
        'any_attr',
        'marked'
    ]

    def __init__(self, name, abstract, mixed, base, children, attrs, any_attr):
        self.name = name
        self.abstract = abstract
        self.mixed = mixed
        self.base = base
        self.children = children
        self.attrs = attrs
        self.any_attr = any_attr

    def visit(self, v):
        v.visit_ComplexType(self)

    def short_descr(self):
        return "ComplexType " + repr(self.name)


class P_Children (object):

    pass


class P_Seq (P_Children):

    __slots__ = [ 'elements' ]

    def __init__(self, elements):
        self.elements = elements

    def visit(self, v):
        v.visit_Seq(self)


class P_LocalAny(P_Children):

    __slots__ = [
        'namespace',
        'processContents',
        'card',
    ]

    def __init__(self, namespace, processContents, card):
        self.namespace = namespace
        self.processContents = processContents
        self.card = card

    def visit(self, v):
        v.visit_LocalAny(self)


class P_GenExpr (P_Children):

    __slots__ = [ 'expr' ]

    def __init__(self, expr):
        assert isinstance(expr, P_Expr)
        self.expr = expr

    def visit(self, v):
        v.visit_GenExpr(self)


class P_Expr (object):

    pass


class P_Expr_Element (P_Expr):

    ___slots__ = [ 'element' ]

    def __init__(self, element):
        assert isinstance(element, P_LocalElement)
        self.element = element

    def visit(self, v):
        v.visit_Expr_Element(self)


class P_Expr_Seq (P_Expr):

    ___slots__ = [ 'exprs' ]

    def __init__(self, exprs):
        self.exprs = exprs

    def visit(self, v):
        v.visit_Expr_Seq(self)


class P_Expr_Choice (P_Expr):

    ___slots__ = [ 'exprs' ]

    def __init__(self, exprs):
        self.exprs = exprs

    def visit(self, v):
        v.visit_Expr_Choice(self)



class P_LocalElement(object):

    __slots__ = [ 'card' ]

    def __init__(self, card):
        self.card = card


class P_LocalElementRef(P_LocalElement):

    __slots__ = [ 'ref' ]

    def __init__(self, ref, card):
        super().__init__(card)
        self.ref = ref

    def visit(self, v):
        v.visit_LocalElementRef(self)


class P_LocalElementDef(P_LocalElement):

    __slots__ = [ 'name', 'type' ]

    def __init__(self, name, typ, card):
        super().__init__(card)
        self.name = name
        self.type = typ

    def visit(self, v):
        v.visit_LocalElementDef(self)


class P_LocalElementDefType(P_LocalElement):

    __slots__ = [ 'name', 'type' ]

    def __init__(self, name, typ, card):
        super().__init__(card)
        self.name = name
        self.type = typ

    def visit(self, v):
        v.visit_LocalElementDefType(self)


class P_Local_ComplexType(object):

    __slots__ = [ "expr_attrs" ]

    def __init__(self, expr_attrs):
        self.expr_attrs = expr_attrs


class P_Attribute (object):

    __slots__ = [
        'name',
        'type',
        'use',
        'default',
    ]

    def __init__(self, name, typ, use, default):
        self.name = name
        self.type = typ
        self.use = use
        self.default = default


class P_AnyAttribute (object):

    __slots__ = [
        'namespace',
        'processContents',
    ]

    def __init__(self, namespace, processContents):
        self.namespace = namespace
        self.processContents = processContents



class P_SimpleType (object):

    __slots__ = [ 'name', 'content' ]

    def __init__(self, name, content):
        self.name = name
        self.content = content

    def visit(self, v):
        v.visit_SimpleType(self)

    def short_descr(self):
        return "SimpleType " + repr(self.name)


class P_SimpleType_content (object):

    pass


class P_Restriction (P_SimpleType_content):

    __slots__ = [ 'base', 'enumerations' ]

    def __init__(self, base, enums):
        self.base = base
        self.enumerations = enums

    def visit(self, v):
        v.visit_Restriction(self)


class P_Enumeration (object):

    __slots__ = [ 'value' ]

    def __init__(self, value):
        self.value = value


class P_Union (P_SimpleType_content):

    __slots__ = [ 'member_types', 'simple_type' ]

    def __init__(self, member_types, simple_type):
        self.member_types = member_types
        self.simple_type = simple_type

    def visit(self, v):
        v.visit_Union(self)


class P_Local_SimpleType(object):

    __slots__ = [ 'restriction' ]

    def __init__(self, restriction):
        self.restriction = restriction

