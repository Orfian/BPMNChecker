
from cmof_model import *


def print_model(out, model):
    assert isinstance(model, M_Model)
    out.write("namespace BPMNModel.Model").nl()
    out.write("{").nl()
    out.inc()

    #print_external_types(out, model.get_external_types())
    print_classes(out, model.get_classes())
    print_enumerations(out, model.get_enumerations())
    print_primitive_types(out, model.get_primitive_types())

    out.dec()
    out.write("}").nl()


def print_classes(out, classes):
    n = len(classes)
    if n == 0: return

    out.write("#region Classes (%d items)" % n).nl()
    for c in classes:
        out.nl()
        print_Class(out, c)
    out.write("#endregion").nl().nl()


def print_enumerations(out, enums):
    n = len(enums)
    if n == 0: return
    out.write("#region Enumerations (%d items)" % n).nl()
    for c in enums:
        print_Enumeration(out, c)
        out.nl()
    out.write("#endregion").nl()
    

def print_primitive_types(out, prim_types):
    n = len(prim_types)
    if n == 0: return

    out.write("    (%d primitive types)" % n).nl().nl()
    for c in prim_types:
        print_PrimitiveType(out, c)
    out.nl()


def print_external_types(out, ext_types):
    n = len(ext_types)
    if n == 0: return

    out.write("    (%d external types)" % n).nl().nl()
    first = True
    for c in ext_types:
        if first:
            first = False
        else:
            out.nl()
        print_external_type(out, c)
    out.nl()



def print_Class(out, c):
    #InteractionNode - no parent
    #FlowElementsContainer (BaseElement) vs 
    #   CallableElement (RootElement (BaseElement))
    #   ChoreographyActivity (FlowNode (FlowElement (BaseElement)))
    #   Collaboration (RootElement (BaseElement))
    #   Activity (FlowNode (FlowElement (BaseElement)))
    #ItemAwareElement (BaseElement) vs
    #   FlowElement (BaseElement)
    #   RootElement (BaseElement)
    interfaces = ["InteractionNode", "FlowElementsContainer", "ItemAwareElement"] 

    assert isinstance(c, M_Class)
    out.write("public ")
    isInterface = c.name in interfaces
    if isInterface:
        out.write("interface " + c.name)
        if c.is_abstract:
            out.write (" // abstract ")
    else:
        if c.is_abstract:
            out.write ("abstract ")
        out.write("class " + c.name)

    scls = c.superclasses
    parentClass = None
    implementedInterface = None

    if len(scls) > 0 and not isInterface:
        out.write(" : ")
        parentClasses = [sc for sc in scls if not(sc.name in interfaces)]
        parentClass = parentClasses[0] if parentClasses else None
        implementedInterfaces = [sc for sc in scls if sc.name in interfaces]
        implementedInterface =  implementedInterfaces[0] if implementedInterfaces else None
        assert len(parentClasses) <=1 and len(implementedInterfaces) <=1
        if parentClass == None and implementedInterface !=None :
            out.write("BaseElement, "+implementedInterface.name)
        else:
            out.write(", ".join([x.name for x in parentClasses] + [x.name for x in implementedInterfaces] ))
    out.nl()
    out.write("{").nl()
    out.inc()
    
    attributesForConstructor = []
    for attr in c.attributes:
        attributesForConstructor+=print_Attribute(out, attr,isInterface)

    if not isInterface:
        baseParameters = []
        if parentClass != None:
            baseParameters = get_all_required_attributes(parentClass, interfaces)
        elif implementedInterface != None:
            baseParameters = [("id", "string")]    

        out.write("public "+c.name+"(")
        realParameters =  baseParameters + attributesForConstructor
        if len(realParameters)>0:
            formatedParameters = [(type+" _"+name) for (name, type) in realParameters] 
            out.write(", ".join(formatedParameters))
        out.write(")").nl()
        if len(baseParameters) > 0:
            out.inc()
            out.write(": base(" +", ".join(["_"+x for (x,_) in baseParameters] )+")" ).nl()
            out.dec();
        out.write("{").nl()
        out.inc()
        for (name,type) in attributesForConstructor:
            out.write("this."+name.capitalize()+" = _" + name+";").nl()
        out.dec()
        out.write("}").nl()
    out.dec()
    out.write("}").nl()

def get_all_required_attributes(currentClass, interfaces):
    assert isinstance(currentClass, M_Class)
    result = []
    parentClasses = [sc for sc in currentClass.superclasses if not(sc.name in interfaces)]
    parentClass = parentClasses[0] if parentClasses else None
    if parentClass != None:
        result += get_all_required_attributes(parentClass, interfaces)
    for attr in currentClass.attributes:
        assert isinstance(attr, M_Attribute)
        card = attr.cardinality
        assert isinstance(card, M_Cardinality)
        if card.lower == 1 and card.upper==1:
            result.append((attr.name, print_csharp_type(attr.type)))
    return result

def print_csharp_type(type):
    assert isinstance(type, M_Type)
    if type.name == "String":
        return "string"
    if type.name == "Boolean":
        return "bool"
    if type.name == "Integer":
        return "int"
    return type.name

def print_Attribute(out, c, isInInterface):
    #TODO Override for diagrams.
    if c.name == "diagrams" and print_csharp_type(c.type) == "BPMNDiagram":
        return []

    typesForConstructor = []

    realName = c.name.capitalize()
    card = c.cardinality
    assert isinstance(card, M_Cardinality)
    lower = card.lower
    upper = card.upper
    if isInInterface:
        if lower == 1 and upper == 1:
            out.write(print_csharp_type(c.type)+ " " + realName + " { get; init; }")
        elif lower == 0 and upper == 1:
            out.write(print_csharp_type(c.type)+"? " + realName + " { get; set; }")
        else:
            out.write("List<"+print_csharp_type(c.type)+"> "+ realName + " { get; }")
    else:
        if lower == 1 and upper == 1:
            realType = print_csharp_type(c.type)
            out.write("public "+realType+ " " + realName + " { get; init; }")
            typesForConstructor.append((c.name, realType))
        elif lower == 0 and upper == 1:
            out.write("public "+print_csharp_type(c.type)+"? " + realName + " { get; set; }")
        else:
            out.write("public List<"+print_csharp_type(c.type)+"> "+ realName + " { get; } = new();")
    out.nl()
    return typesForConstructor


def print_Enumeration(out, c):
    out.write("public enum " + c.name).nl().write("{").nl()
    out.inc()
    for item in c.literals:
        assert item.enum is c
        out.write(item.name).write(",").nl()
    out.dec()
    out.write("}").nl()


def print_PrimitiveType(out, c):
    out.write("primitive type " + c.name).nl()


def print_external_type(out, c):
    out.write("external " + type_kind_to_str(c.kind) + " " +
              c.name + " {").nl()
    out.inc()
    out.write("href = " + repr(c.href)).nl()
    out.dec()
    out.write("}").nl()


def type_kind_to_str(kind):
    assert isinstance(kind, Type_Kind)

    match kind:
        case Type_Kind.Class:
            return "class"

        case Type_Kind.DataType:
            return "datatype"

        case Type_Kind.Enumeration:
            return "enum"

        case Type_Kind.PrimitiveType:
            return "primitive type"

        case _:
            assert False


