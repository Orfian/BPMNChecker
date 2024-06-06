
from cmof_model import *
from utils import Output

extracted = {
	"Activity": ([], ["id", "name", "isForCompensation", "startQuantity", "completionQuantity", "default"], ["documentation", "extensionElements", "auditing", "monitoring", "categoryValueRef", "incoming", "outgoing", "ioSpecification", "property", "dataInputAssociation", "dataOutputAssociation", "resourceRole", "loopCharacteristics"]),
	"AdHocSubProcess": ([], ["id", "name", "isForCompensation", "startQuantity", "completionQuantity", "default", "triggeredByEvent", "cancelRemainingInstances", "ordering"], ["documentation", "extensionElements", "auditing", "monitoring", "categoryValueRef", "incoming", "outgoing", "ioSpecification", "property", "dataInputAssociation", "dataOutputAssociation", "resourceRole", "loopCharacteristics", "laneSet", "flowElement", "artifact", "completionCondition"]),
	"Artifact": ([], ["id"], ["documentation", "extensionElements"]),
	"Assignment": ([], ["id"], ["documentation", "extensionElements", "from", "to"]),
	"Association": (["sourceRef", "targetRef"], ["id", "associationDirection"], ["documentation", "extensionElements"]),
	"Auditing": ([], ["id"], ["documentation", "extensionElements"]),
	"BaseElement": ([], ["id"], ["documentation", "extensionElements"]),
	"BaseElementWithMixedContent": ([], ["id"], ["documentation", "extensionElements"]),
	"BoundaryEvent": (["attachedToRef"], ["id", "name", "parallelMultiple", "cancelActivity"], ["documentation", "extensionElements", "auditing", "monitoring", "categoryValueRef", "incoming", "outgoing", "property", "dataOutput", "dataOutputAssociation", "outputSet", "eventDefinition", "eventDefinitionRef"]),
	"BusinessRuleTask": ([], ["id", "name", "isForCompensation", "startQuantity", "completionQuantity", "default", "implementation"], ["documentation", "extensionElements", "auditing", "monitoring", "categoryValueRef", "incoming", "outgoing", "ioSpecification", "property", "dataInputAssociation", "dataOutputAssociation", "resourceRole", "loopCharacteristics"]),
	"CallableElement": ([], ["id", "name"], ["documentation", "extensionElements", "supportedInterfaceRef", "ioSpecification", "ioBinding"]),
	"CallActivity": ([], ["id", "name", "isForCompensation", "startQuantity", "completionQuantity", "default", "calledElement"], ["documentation", "extensionElements", "auditing", "monitoring", "categoryValueRef", "incoming", "outgoing", "ioSpecification", "property", "dataInputAssociation", "dataOutputAssociation", "resourceRole", "loopCharacteristics"]),
	"CallChoreography": (["initiatingParticipantRef"], ["id", "name", "loopType", "calledChoreographyRef"], ["documentation", "extensionElements", "auditing", "monitoring", "categoryValueRef", "incoming", "outgoing", "participantRef", "correlationKey", "participantAssociation"]),
	"CallConversation": ([], ["id", "name", "calledCollaborationRef"], ["documentation", "extensionElements", "participantRef", "messageFlowRef", "correlationKey", "participantAssociation"]),
	"CancelEventDefinition": ([], ["id"], ["documentation", "extensionElements"]),
	"CatchEvent": ([], ["id", "name", "parallelMultiple"], ["documentation", "extensionElements", "auditing", "monitoring", "categoryValueRef", "incoming", "outgoing", "property", "dataOutput", "dataOutputAssociation", "outputSet", "eventDefinition", "eventDefinitionRef"]),
	"Category": ([], ["id", "name"], ["documentation", "extensionElements", "categoryValue"]),
	"CategoryValue": ([], ["id", "value"], ["documentation", "extensionElements"]),
	"Choreography": ([], ["id", "name", "isClosed"], ["documentation", "extensionElements", "participant", "messageFlow", "artifact", "conversationNode", "conversationAssociation", "participantAssociation", "messageFlowAssociation", "correlationKey", "choreographyRef", "conversationLink", "flowElement"]),
	"ChoreographyActivity": (["initiatingParticipantRef"], ["id", "name", "loopType"], ["documentation", "extensionElements", "auditing", "monitoring", "categoryValueRef", "incoming", "outgoing", "participantRef", "correlationKey"]),
	"ChoreographyTask": (["initiatingParticipantRef"], ["id", "name", "loopType"], ["documentation", "extensionElements", "auditing", "monitoring", "categoryValueRef", "incoming", "outgoing", "participantRef", "correlationKey", "messageFlowRef"]),
	"Collaboration": ([], ["id", "name", "isClosed"], ["documentation", "extensionElements", "participant", "messageFlow", "artifact", "conversationNode", "conversationAssociation", "participantAssociation", "messageFlowAssociation", "correlationKey", "choreographyRef", "conversationLink"]),
	"CompensateEventDefinition": ([], ["id", "waitForCompletion", "activityRef"], ["documentation", "extensionElements"]),
	"ComplexBehaviorDefinition": ([], ["id"], ["documentation", "extensionElements", "condition", "event"]),
	"ComplexGateway": ([], ["id", "name", "gatewayDirection", "default"], ["documentation", "extensionElements", "auditing", "monitoring", "categoryValueRef", "incoming", "outgoing", "activationCondition"]),
	"ConditionalEventDefinition": ([], ["id"], ["documentation", "extensionElements", "condition"]),
	"Conversation": ([], ["id", "name"], ["documentation", "extensionElements", "participantRef", "messageFlowRef", "correlationKey"]),
	"ConversationAssociation": (["innerConversationNodeRef", "outerConversationNodeRef"], ["id"], ["documentation", "extensionElements"]),
	"ConversationLink": (["sourceRef", "targetRef"], ["id", "name"], ["documentation", "extensionElements"]),
	"ConversationNode": ([], ["id", "name"], ["documentation", "extensionElements", "participantRef", "messageFlowRef", "correlationKey"]),
	"CorrelationKey": ([], ["id", "name"], ["documentation", "extensionElements", "correlationPropertyRef"]),
	"CorrelationProperty": ([], ["id", "name", "type"], ["documentation", "extensionElements", "correlationPropertyRetrievalExpression"]),
	"CorrelationPropertyBinding": (["correlationPropertyRef"], ["id"], ["documentation", "extensionElements", "dataPath"]),
	"CorrelationPropertyRetrievalExpression": (["messageRef"], ["id"], ["documentation", "extensionElements", "messagePath"]),
	"CorrelationSubscription": (["correlationKeyRef"], ["id"], ["documentation", "extensionElements", "correlationPropertyBinding"]),
	"DataAssociation": ([], ["id"], ["documentation", "extensionElements", "sourceRef", "targetRef", "transformation", "assignment"]),
	"DataInput": ([], ["id", "name", "itemSubjectRef", "isCollection"], ["documentation", "extensionElements", "dataState"]),
	"DataInputAssociation": ([], ["id"], ["documentation", "extensionElements", "sourceRef", "targetRef", "transformation", "assignment"]),
	"DataObject": ([], ["id", "name", "itemSubjectRef", "isCollection"], ["documentation", "extensionElements", "auditing", "monitoring", "categoryValueRef", "dataState"]),
	"DataObjectReference": ([], ["id", "name", "itemSubjectRef", "dataObjectRef"], ["documentation", "extensionElements", "auditing", "monitoring", "categoryValueRef", "dataState"]),
	"DataOutput": ([], ["id", "name", "itemSubjectRef", "isCollection"], ["documentation", "extensionElements", "dataState"]),
	"DataOutputAssociation": ([], ["id"], ["documentation", "extensionElements", "sourceRef", "targetRef", "transformation", "assignment"]),
	"DataState": ([], ["id", "name"], ["documentation", "extensionElements"]),
	"DataStore": ([], ["id", "name", "capacity", "isUnlimited", "itemSubjectRef"], ["documentation", "extensionElements", "dataState"]),
	"DataStoreReference": ([], ["id", "name", "itemSubjectRef", "dataStoreRef"], ["documentation", "extensionElements", "auditing", "monitoring", "categoryValueRef", "dataState"]),
	"Documentation": ([], ["id", "textFormat"], ["any"]),
	"EndEvent": ([], ["id", "name"], ["documentation", "extensionElements", "auditing", "monitoring", "categoryValueRef", "incoming", "outgoing", "property", "dataInput", "dataInputAssociation", "inputSet", "eventDefinition", "eventDefinitionRef"]),
	"EndPoint": ([], ["id"], ["documentation", "extensionElements"]),
	"Error": ([], ["id", "name", "errorCode", "structureRef"], ["documentation", "extensionElements"]),
	"ErrorEventDefinition": ([], ["id", "errorRef"], ["documentation", "extensionElements"]),
	"Escalation": ([], ["id", "name", "escalationCode", "structureRef"], ["documentation", "extensionElements"]),
	"EscalationEventDefinition": ([], ["id", "escalationRef"], ["documentation", "extensionElements"]),
	"Event": ([], ["id", "name"], ["documentation", "extensionElements", "auditing", "monitoring", "categoryValueRef", "incoming", "outgoing", "property"]),
	"EventBasedGateway": ([], ["id", "name", "gatewayDirection", "instantiate", "eventGatewayType"], ["documentation", "extensionElements", "auditing", "monitoring", "categoryValueRef", "incoming", "outgoing"]),
	"EventDefinition": ([], ["id"], ["documentation", "extensionElements"]),
	"ExclusiveGateway": ([], ["id", "name", "gatewayDirection", "default"], ["documentation", "extensionElements", "auditing", "monitoring", "categoryValueRef", "incoming", "outgoing"]),
	"Expression": ([], ["id"], ["documentation", "extensionElements"]),
	"Extension": ([], ["definition", "mustUnderstand"], ["documentation"]),
	"ExtensionElements": ([], [], ["any"]),
	"FlowElement": ([], ["id", "name"], ["documentation", "extensionElements", "auditing", "monitoring", "categoryValueRef"]),
	"FlowNode": ([], ["id", "name"], ["documentation", "extensionElements", "auditing", "monitoring", "categoryValueRef", "incoming", "outgoing"]),
	"FormalExpression": ([], ["id", "language", "evaluatesToTypeRef"], ["documentation", "extensionElements"]),
	"Gateway": ([], ["id", "name", "gatewayDirection"], ["documentation", "extensionElements", "auditing", "monitoring", "categoryValueRef", "incoming", "outgoing"]),
	"GlobalBusinessRuleTask": ([], ["id", "name", "implementation"], ["documentation", "extensionElements", "supportedInterfaceRef", "ioSpecification", "ioBinding", "resourceRole"]),
	"GlobalChoreographyTask": ([], ["id", "name", "isClosed", "initiatingParticipantRef"], ["documentation", "extensionElements", "participant", "messageFlow", "artifact", "conversationNode", "conversationAssociation", "participantAssociation", "messageFlowAssociation", "correlationKey", "choreographyRef", "conversationLink", "flowElement"]),
	"GlobalConversation": ([], ["id", "name", "isClosed"], ["documentation", "extensionElements", "participant", "messageFlow", "artifact", "conversationNode", "conversationAssociation", "participantAssociation", "messageFlowAssociation", "correlationKey", "choreographyRef", "conversationLink"]),
	"GlobalManualTask": ([], ["id", "name"], ["documentation", "extensionElements", "supportedInterfaceRef", "ioSpecification", "ioBinding", "resourceRole"]),
	"GlobalScriptTask": ([], ["id", "name", "scriptLanguage"], ["documentation", "extensionElements", "supportedInterfaceRef", "ioSpecification", "ioBinding", "resourceRole", "script"]),
	"GlobalTask": ([], ["id", "name"], ["documentation", "extensionElements", "supportedInterfaceRef", "ioSpecification", "ioBinding", "resourceRole"]),
	"GlobalUserTask": ([], ["id", "name", "implementation"], ["documentation", "extensionElements", "supportedInterfaceRef", "ioSpecification", "ioBinding", "resourceRole", "rendering"]),
	"Group": ([], ["id", "categoryValueRef"], ["documentation", "extensionElements"]),
	"HumanPerformer": ([], ["id", "name"], ["documentation", "extensionElements", "resourceRef", "resourceParameterBinding", "resourceAssignmentExpression"]),
	"ImplicitThrowEvent": ([], ["id", "name"], ["documentation", "extensionElements", "auditing", "monitoring", "categoryValueRef", "incoming", "outgoing", "property", "dataInput", "dataInputAssociation", "inputSet", "eventDefinition", "eventDefinitionRef"]),
	"InclusiveGateway": ([], ["id", "name", "gatewayDirection", "default"], ["documentation", "extensionElements", "auditing", "monitoring", "categoryValueRef", "incoming", "outgoing"]),
	"InputSet": ([], ["id", "name"], ["documentation", "extensionElements", "dataInputRefs", "optionalInputRefs", "whileExecutingInputRefs", "outputSetRefs"]),
	"Interface": (["name"], ["id", "implementationRef"], ["documentation", "extensionElements", "operation"]),
	"IntermediateCatchEvent": ([], ["id", "name", "parallelMultiple"], ["documentation", "extensionElements", "auditing", "monitoring", "categoryValueRef", "incoming", "outgoing", "property", "dataOutput", "dataOutputAssociation", "outputSet", "eventDefinition", "eventDefinitionRef"]),
	"IntermediateThrowEvent": ([], ["id", "name"], ["documentation", "extensionElements", "auditing", "monitoring", "categoryValueRef", "incoming", "outgoing", "property", "dataInput", "dataInputAssociation", "inputSet", "eventDefinition", "eventDefinitionRef"]),
	"InputOutputBinding": (["operationRef", "inputDataRef", "outputDataRef"], ["id"], ["documentation", "extensionElements"]),
	"InputOutputSpecification": ([], ["id"], ["documentation", "extensionElements", "dataInput", "dataOutput", "inputSet", "outputSet"]),
	"ItemDefinition": ([], ["id", "structureRef", "isCollection", "itemKind"], ["documentation", "extensionElements"]),
	"Lane": ([], ["id", "name", "partitionElementRef"], ["documentation", "extensionElements", "partitionElement", "flowNodeRef", "childLaneSet"]),
	"LaneSet": ([], ["id", "name"], ["documentation", "extensionElements", "lane"]),
	"LinkEventDefinition": (["name"], ["id"], ["documentation", "extensionElements", "source", "target"]),
	"LoopCharacteristics": ([], ["id"], ["documentation", "extensionElements"]),
	"ManualTask": ([], ["id", "name", "isForCompensation", "startQuantity", "completionQuantity", "default"], ["documentation", "extensionElements", "auditing", "monitoring", "categoryValueRef", "incoming", "outgoing", "ioSpecification", "property", "dataInputAssociation", "dataOutputAssociation", "resourceRole", "loopCharacteristics"]),
	"Message": ([], ["id", "name", "itemRef"], ["documentation", "extensionElements"]),
	"MessageEventDefinition": ([], ["id", "messageRef"], ["documentation", "extensionElements", "operationRef"]),
	"MessageFlow": (["sourceRef", "targetRef"], ["id", "name", "messageRef"], ["documentation", "extensionElements"]),
	"MessageFlowAssociation": (["innerMessageFlowRef", "outerMessageFlowRef"], ["id"], ["documentation", "extensionElements"]),
	"Monitoring": ([], ["id"], ["documentation", "extensionElements"]),
	"MultiInstanceLoopCharacteristics": ([], ["id", "isSequential", "behavior", "oneBehaviorEventRef", "noneBehaviorEventRef"], ["documentation", "extensionElements", "loopCardinality", "loopDataInputRef", "loopDataOutputRef", "inputDataItem", "outputDataItem", "complexBehaviorDefinition", "completionCondition"]),
	"Operation": (["name"], ["id", "implementationRef"], ["documentation", "extensionElements", "inMessageRef", "outMessageRef", "errorRef"]),
	"OutputSet": ([], ["id", "name"], ["documentation", "extensionElements", "dataOutputRefs", "optionalOutputRefs", "whileExecutingOutputRefs", "inputSetRefs"]),
	"ParallelGateway": ([], ["id", "name", "gatewayDirection"], ["documentation", "extensionElements", "auditing", "monitoring", "categoryValueRef", "incoming", "outgoing"]),
	"Participant": ([], ["id", "name", "processRef"], ["documentation", "extensionElements", "interfaceRef", "endPointRef", "participantMultiplicity"]),
	"ParticipantAssociation": ([], ["id"], ["documentation", "extensionElements", "innerParticipantRef", "outerParticipantRef"]),
	"ParticipantMultiplicity": ([], ["id", "minimum", "maximum"], ["documentation", "extensionElements"]),
	"PartnerEntity": ([], ["id", "name"], ["documentation", "extensionElements", "participantRef"]),
	"PartnerRole": ([], ["id", "name"], ["documentation", "extensionElements", "participantRef"]),
	"Performer": ([], ["id", "name"], ["documentation", "extensionElements", "resourceRef", "resourceParameterBinding", "resourceAssignmentExpression"]),
	"PotentialOwner": ([], ["id", "name"], ["documentation", "extensionElements", "resourceRef", "resourceParameterBinding", "resourceAssignmentExpression"]),
	"Process": ([], ["id", "name", "processType", "isClosed", "isExecutable", "definitionalCollaborationRef"], ["documentation", "extensionElements", "supportedInterfaceRef", "ioSpecification", "ioBinding", "auditing", "monitoring", "property", "laneSet", "flowElement", "artifact", "resourceRole", "correlationSubscription", "supports"]),
	"Property": ([], ["id", "name", "itemSubjectRef"], ["documentation", "extensionElements", "dataState"]),
	"ReceiveTask": ([], ["id", "name", "isForCompensation", "startQuantity", "completionQuantity", "default", "implementation", "instantiate", "messageRef", "operationRef"], ["documentation", "extensionElements", "auditing", "monitoring", "categoryValueRef", "incoming", "outgoing", "ioSpecification", "property", "dataInputAssociation", "dataOutputAssociation", "resourceRole", "loopCharacteristics"]),
	"Relationship": (["type"], ["id", "direction"], ["documentation", "extensionElements", "source", "target"]),
	"Rendering": ([], ["id"], ["documentation", "extensionElements"]),
	"Resource": (["name"], ["id"], ["documentation", "extensionElements", "resourceParameter"]),
	"ResourceAssignmentExpression": ([], ["id"], ["documentation", "extensionElements", "expression"]),
	"ResourceParameter": ([], ["id", "name", "type", "isRequired"], ["documentation", "extensionElements"]),
	"ResourceParameterBinding": (["parameterRef"], ["id"], ["documentation", "extensionElements", "expression"]),
	"ResourceRole": ([], ["id", "name"], ["documentation", "extensionElements", "resourceRef", "resourceParameterBinding", "resourceAssignmentExpression"]),
	"RootElement": ([], ["id"], ["documentation", "extensionElements"]),
	"ScriptTask": ([], ["id", "name", "isForCompensation", "startQuantity", "completionQuantity", "default", "scriptFormat"], ["documentation", "extensionElements", "auditing", "monitoring", "categoryValueRef", "incoming", "outgoing", "ioSpecification", "property", "dataInputAssociation", "dataOutputAssociation", "resourceRole", "loopCharacteristics", "script"]),
	"Script": ([], [], ["any"]),
	"SendTask": ([], ["id", "name", "isForCompensation", "startQuantity", "completionQuantity", "default", "implementation", "messageRef", "operationRef"], ["documentation", "extensionElements", "auditing", "monitoring", "categoryValueRef", "incoming", "outgoing", "ioSpecification", "property", "dataInputAssociation", "dataOutputAssociation", "resourceRole", "loopCharacteristics"]),
	"SequenceFlow": (["sourceRef", "targetRef"], ["id", "name", "isImmediate"], ["documentation", "extensionElements", "auditing", "monitoring", "categoryValueRef", "conditionExpression"]),
	"ServiceTask": ([], ["id", "name", "isForCompensation", "startQuantity", "completionQuantity", "default", "implementation", "operationRef"], ["documentation", "extensionElements", "auditing", "monitoring", "categoryValueRef", "incoming", "outgoing", "ioSpecification", "property", "dataInputAssociation", "dataOutputAssociation", "resourceRole", "loopCharacteristics"]),
	"Signal": ([], ["id", "name", "structureRef"], ["documentation", "extensionElements"]),
	"SignalEventDefinition": ([], ["id", "signalRef"], ["documentation", "extensionElements"]),
	"StandardLoopCharacteristics": ([], ["id", "testBefore", "loopMaximum"], ["documentation", "extensionElements", "loopCondition"]),
	"StartEvent": ([], ["id", "name", "parallelMultiple", "isInterrupting"], ["documentation", "extensionElements", "auditing", "monitoring", "categoryValueRef", "incoming", "outgoing", "property", "dataOutput", "dataOutputAssociation", "outputSet", "eventDefinition", "eventDefinitionRef"]),
	"SubChoreography": (["initiatingParticipantRef"], ["id", "name", "loopType"], ["documentation", "extensionElements", "auditing", "monitoring", "categoryValueRef", "incoming", "outgoing", "participantRef", "correlationKey", "flowElement", "artifact"]),
	"SubConversation": ([], ["id", "name"], ["documentation", "extensionElements", "participantRef", "messageFlowRef", "correlationKey", "conversationNode"]),
	"SubProcess": ([], ["id", "name", "isForCompensation", "startQuantity", "completionQuantity", "default", "triggeredByEvent"], ["documentation", "extensionElements", "auditing", "monitoring", "categoryValueRef", "incoming", "outgoing", "ioSpecification", "property", "dataInputAssociation", "dataOutputAssociation", "resourceRole", "loopCharacteristics", "laneSet", "flowElement", "artifact"]),
	"Task": ([], ["id", "name", "isForCompensation", "startQuantity", "completionQuantity", "default"], ["documentation", "extensionElements", "auditing", "monitoring", "categoryValueRef", "incoming", "outgoing", "ioSpecification", "property", "dataInputAssociation", "dataOutputAssociation", "resourceRole", "loopCharacteristics"]),
	"TerminateEventDefinition": ([], ["id"], ["documentation", "extensionElements"]),
	"TextAnnotation": ([], ["id", "textFormat"], ["documentation", "extensionElements", "text"]),
	"Text": ([], [], ["any"]),
	"ThrowEvent": ([], ["id", "name"], ["documentation", "extensionElements", "auditing", "monitoring", "categoryValueRef", "incoming", "outgoing", "property", "dataInput", "dataInputAssociation", "inputSet", "eventDefinition", "eventDefinitionRef"]),
	"TimerEventDefinition": ([], ["id"], ["documentation", "extensionElements", "timeDate", "timeDuration", "timeCycle"]),
	"Transaction": ([], ["id", "name", "isForCompensation", "startQuantity", "completionQuantity", "default", "triggeredByEvent", "method"], ["documentation", "extensionElements", "auditing", "monitoring", "categoryValueRef", "incoming", "outgoing", "ioSpecification", "property", "dataInputAssociation", "dataOutputAssociation", "resourceRole", "loopCharacteristics", "laneSet", "flowElement", "artifact"]),
	"UserTask": ([], ["id", "name", "isForCompensation", "startQuantity", "completionQuantity", "default", "implementation"], ["documentation", "extensionElements", "auditing", "monitoring", "categoryValueRef", "incoming", "outgoing", "ioSpecification", "property", "dataInputAssociation", "dataOutputAssociation", "resourceRole", "loopCharacteristics", "rendering"]),
	"Definitions": (["targetNamespace"], ["id", "name", "expressionLanguage", "typeLanguage", "exporter", "exporterVersion"], ["import", "extension", "rootElement", "relationship"]),
	"Import": (["namespace", "location", "importType"], [], []),
	}

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

enumNames = []

def capitalize_first_letter(s):
    if not s:
        return s
    return s[0].upper() + s[1:]

def print_mapping(mappings, model_names):
    mappingFile = r"d:\mapping.txt"
    out = Output(open(mappingFile, "w"))
    out.write("Mapping:").nl()
    out.inc()
    for key,(fromXMLtoCMOF, fromCMOFtoXML) in mappings.items():
        out.write(key).nl()
        out.inc()
        for xmlAtt, cmofAtt in fromXMLtoCMOF.items():
            out.write("\tXML: "+xmlAtt+" -> "+cmofAtt).nl()
        out.nl()
        for cmofAtt, xmlAtt in fromCMOFtoXML.items():
            out.write("\tCMOF: "+cmofAtt+" -> "+xmlAtt).nl()
        out.nl()
        out.dec()
    out.dec()        
    out.nl()
    out.write("In CMOF but not in XSD").nl()
    out.inc()
    selectedCmof = [name for name in model_names if name not in mappings]
    for name in selectedCmof:
        out.write(name)
        if name in interfaces:
            out.write(" (interface)")
        out.nl()
    out.dec()
    out.nl()
    out.write("In XSD but not in CMOF").nl()
    out.inc()
    selectedXsd = [key for key in extracted if key not in mappings]
    for name in selectedXsd:
        out.write(name)
        if name in interfaces:
            out.write(" (interface)")
        out.nl()
    

def print_model(path, model):

    assert isinstance(model, M_Model)

    print(path)
    mappings = build_mapping(model.get_classes())
    
    #print_mapping(mappings, [c.name for c in model.get_classes()])

    classFile = path + r"\File.cs"
    out = Output(open(classFile, "w"))

    #out.write("using Serilog;").nl()
    #out.nl()
    out.write("namespace BPMNModel.Model").nl()
    out.write("{").nl()
    out.inc()
    print_classes(out, model.get_classes(), mappings)
    print_enumerations(out, model.get_enumerations())
    out.dec()
    out.write("}").nl()
    
    out = Output(open(path + r"\Factory.cs", "w"))
    out.write("using Utility;").nl()
    out.nl()
    out.write("namespace BPMNModel.Model").nl()
    out.write("{").nl()
    out.inc()
    out.write("public partial class Factory").nl()
    out.write("{").nl()
    out.inc()
    print_factories(out, model.get_classes(), mappings)
    out.dec()
    out.write("}").nl()
    out.dec()
    out.write("}").nl()

def build_mapping(classes):
    n = len(classes)
    if n == 0: return
    result = {}
    for c in classes:
        fromXMLtoCMOF = {}
        fromCMOFtoXML = {}
        assert isinstance(c, M_Class)
        if not c.name in extracted:
            continue;
        (reqiredFromXSD, optionalFormXSD, elementsFromXSD) = extracted[c.name]
        allCMOFAtributes = get_all_attributes_names(c)
        allXMLAttributes = reqiredFromXSD+optionalFormXSD+elementsFromXSD
        #print(c.name + " : "+ ("" if parentClass is None else parentClass) + " , " + ("" if implementedInterface is None else implementedInterface.name))
        #print("\t"+ ", ".join(allXMLAttributes))
        #print("\t"+", ".join(allCMOFAtributes))
        for xmlAtt in allXMLAttributes:
            matches = [cmofAtt for cmofAtt in allCMOFAtributes if remove_trailing_s(xmlAtt) == remove_trailing_s(cmofAtt)]
            if len(matches) == 1 :
                fromXMLtoCMOF[xmlAtt] = matches[0]
                fromCMOFtoXML[matches[0]] = xmlAtt
                allCMOFAtributes.remove(matches[0])
            else:
                assert len(matches) == 0
        result[c.name] = (fromXMLtoCMOF, fromCMOFtoXML)
    return result


def print_factories(out, classes, mappings):
    n = len(classes)
    if n == 0: return
    out.write("#region Factories").nl()
    for c in classes:
        out.nl()
        print_factory(out, c, mappings)
    out.write("#endregion").nl().nl()
 
def print_classes(out, classes, mapping):
    n = len(classes)
    if n == 0: return
    out.write("#region Classes (%d items)" % n).nl()
    for c in classes:
        out.nl()
        print_Class(out, c, mapping)
    out.write("#endregion").nl().nl()


def print_enumerations(out, enums):
    n = len(enums)
    if n == 0: return
    out.write("#region Enumerations (%d items)" % n).nl()
    for c in enums:
        print_Enumeration(out, c)
        out.nl()
    out.write("#endregion").nl()
    
def get_arity(l, h):
    result = " ("+str(l)+", "
    if h < 0:
        return result + "*)"
    else:
        return result + str(h) + ")"

def print_factory(out, c, mappings):
    assert isinstance(c, M_Class)
    isInterface = c.name in interfaces
    if isInterface == False and c.is_abstract == False and c.name in extracted:
        out.write("private "+c.name+ " Load"+c.name+ "(XmlParserComplexNode node)").nl()
        out.write("{").nl()
        out.inc()
        (requiredXmlNames, optionalXmlNames, elementXmlNames) = extracted[c.name]
        allAttributes = get_all_attributes_with_Xml_names(c, mappings)

        out.write(" var result = GetOrCreate<"+c.name+">(node);").nl()
        out.nl()

        processed = []
        processedXML = []
        for (requiredName, (requiredType, requiredXMLName,l,h,_)) in allAttributes.items():
            if requiredXMLName in requiredXmlNames:
                assert l <= 1 and l>=0 and h <= 1 and h >=0
                out.write("// required: " + requiredXMLName + " -> " + requiredType+ " " + requiredName + get_arity(l,h)).nl()
                out.write("var _"+requiredName+"Attribute = node.Attributes[\""+requiredXMLName+"\"]?.ProcessedValue;").nl()
                out.write("if (_"+requiredName+"Attribute is null) throw new BPMNCheckerExceptions($\"Node {node.ID} ({(string.IsNullOrWhiteSpace(node.Type?.Name) ? node.Type?.Name : \"\")}) is missing required attribute "+requiredXMLName+"\");").nl()
                if requiredType == "string" or requiredType == "int" or requiredType == "bool":
                    out.write("result."+ capitalize_first_letter(requiredName) + " = ("+requiredType+ ")_"+requiredName+"Attribute;").nl()
                elif requiredType in enumNames:
                    out.write("result."+ capitalize_first_letter(requiredName) +" = CreateEnum<"+requiredType+ ">((string)_"+requiredName+"Attribute);").nl()
                else:
                    out.write("result."+ capitalize_first_letter(requiredName) + " = Load<"+requiredType+ ">((XmlParserComplexNode)_"+requiredName+"Attribute);").nl()
                out.nl()
                processed.append(requiredName)
                processedXML.append(requiredXMLName)

        for (optionalName, (optionalType, optionalXmlName, l, h,_)) in allAttributes.items():
            if optionalXmlName in optionalXmlNames:
                assert l <= 1 and l>=0 and h <= 1 and h >=0
                out.write("// optional: "+ optionalXmlName + " -> " +optionalType + " "+ optionalName + get_arity(l,h)).nl()
                out.write("var _"+optionalName+"Attribute = node.Attributes.ContainsKey(\"" + optionalXmlName +"\") ? node.Attributes[\""+optionalXmlName+"\"].ProcessedValue : null;").nl()
                out.write("if (_"+optionalName+"Attribute is not null) ")
                if optionalType == "string" or optionalType == "int" or optionalType == "bool":
                    out.write("result."+ capitalize_first_letter(optionalName) +" = ("+optionalType+ ")_"+optionalName+"Attribute;").nl()
                elif optionalType in enumNames:
                    out.write("result."+ capitalize_first_letter(optionalName) +" = CreateEnum<"+optionalType+ ">((string)_"+optionalName+"Attribute);").nl()
                else:
                    out.write("result."+ capitalize_first_letter(optionalName) +" = Load<"+optionalType+ ">((XmlParserComplexNode)_"+optionalName+"Attribute);").nl()   
                out.nl()
                processed.append(optionalName)
                processedXML.append(optionalXmlName)

        for (elementName, (elementType, elementXmlName, l, h,_)) in allAttributes.items():
            if elementXmlName in elementXmlNames:
                out.write("// element: "+ elementXmlName + " -> " +elementType + " "+ elementName + get_arity(l,h)).nl()
                if h == 1:
                    out.write("result." + capitalize_first_letter(elementName) + " = FillElement<")
                    out.write(elementType)
                    out.write(">(node.ChildNodes[\"" + elementXmlName + "\"]);").nl()
                    out.nl()
                else:
                    out.write("FillElements(node.ChildNodes[\"" + elementXmlName + "\"], result.")
                    out.write(capitalize_first_letter(elementName))
                    out.write(");").nl()
                    out.nl()

                processed.append(elementName)
                processedXML.append(elementXmlName)

        for (itemName, (itemType, itemXmlName,l, h, attr)) in allAttributes.items():
            #TODO: Solve known attributes
            if itemName not in processed + ["diagrams", "extensionValues", "extensionDefinitions", "documentation"]:
                #expecting just unlinked attributes
                if itemXmlName is not None:
                    raise "Expecting no xml name, but have:"+ itemXmlName

                assert isinstance(attr, M_Attribute)
                association = attr.association
                if association is None:
                    out.write("// missing: "+ itemType + " "+ itemName + get_arity(l,h)).nl()
                elif association.is_one_way():
                    assert isinstance(association, M_One_Way_Association)
                    out.write("// empty: "+ itemType + " "+ itemName + get_arity(l,h)).nl()
                else:          
                    #
                    assert h <0               
                    assert isinstance(association, M_Two_Way_Association)
                    out.write("// link back: "+ itemType + " "+ itemName + get_arity(l,h)).nl()
                    out.write("// "+ attr.name + " " + str(attr.assoc_index) + " " + str(attr.association)).nl()
                    target = attr.association.get_other_attr(attr.assoc_index)
                    s = target.parent.name + '.' + target.name
                    out.write("// target: "+s).nl()
                out.nl()

        for  xmlName in requiredXmlNames + optionalXmlNames + elementXmlNames:
            #TODO: Solve known attributes
            if xmlName not in processedXML + [ "extensionElements", "id", "any", "documentation"]:
                raise "Error: not attached: "+ xmlName
        out.write("return result;").nl()
        out.dec()
        out.write("}").nl().nl()

def remove_trailing_s(string):
    if string == "calledElementRef":
        return "calledElement"
    if string.lower().endswith('node'):
        return string[:-4]
    if string.lower().endswith('nodes'):
        return string[:-5]
    if string.lower().endswith('role'):
        return string[:-4]
    if string.lower()=="resources":
        return "resource"
    if string.lower()=="properties":
        return "property"
    if string.lower().endswith('s'):
        return string[:-1]
    return string

def print_Class(out, c, mappings):
    assert isinstance(c, M_Class)
    mapping = None
    if c.name in mappings:    
        (mapping, _) = mappings[c.name]
    requiredXMLnames = []
    if c.name in extracted and mapping is not None:
        (originalRequired,_,_) = extracted[c.name]
        requiredXMLnames = [name for name in originalRequired]

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

    for attr in c.attributes:
        print_Attribute(out, attr,isInterface)

    if implementedInterface != None:
        out.nl();
        out.write("#region Implementing: "+implementedInterface.name).nl()
        for attr in implementedInterface.attributes:
            print_Attribute(out, attr,False)
        out.write("#endregion").nl()
        out.nl();
    
    if not isInterface:
                
        out.write("public "+c.name+"()").nl()
        out.write("{").nl()
        out.write("}").nl().nl()
    out.dec()
    out.write("}").nl()

# def get_all_attributes_with_mapping(currentClass, mappings):
#     assert isinstance(currentClass, M_Class)
#     result = []
#     if currentClass not in interfaces:
#         parentClasses = [sc for sc in currentClass.superclasses]
#         for parentClass in parentClasses:
#             result += get_all_attributes_with_mapping(parentClass, mappings) 
#     if currentClass.name in mappings:    
#         (fromXmlToCmof, fromCmofToXml) = mappings[currentClass.name]
#     attributesWithMapping = []
#     if currentClass.name in extracted and fromXmlToCmof is not None:
#         (originalRequired,originalOptional,originalElements) = extracted[currentClass.name]
#         for name in originalRequired + originalOptional + originalElements:
#             if name in fromXmlToCmof:
#                 attributesWithMapping.append(fromXmlToCmof[name])
#             #elif name == "id" and "id" not in optional :
#             #    optional.append("id")
#     for attr in currentClass.attributes:
#         if attr.name in attributesWithMapping:
#             card = attr.cardinality
#             assert isinstance(card, M_Cardinality)
#             lower = card.lower
#             upper = card.upper
#             result.append((attr.name, print_csharp_type(attr.type), fromCmofToXml[attr.name], lower, upper))
#     print("Solving " + currentClass.name)            
#     print(result)
#     return result


def get_all_attributes_names(currentClass):
    assert isinstance(currentClass, M_Class)
    result = []
    parentClasses = [sc for sc in currentClass.superclasses]
    for parentClass in parentClasses:
        parentResult = get_all_attributes_names(parentClass) 
        for newAttributeName in parentResult:
            if newAttributeName not in result:
                result.append(newAttributeName)

    for attr in currentClass.attributes:
        if attr.name not in result:
            result.append(attr.name)
    return result

def get_all_attributes(currentClass):
    assert isinstance(currentClass, M_Class)
    result = {}
    parentClasses = [sc for sc in currentClass.superclasses]
    for parentClass in parentClasses:
        parentResult = get_all_attributes(parentClass) 
        for newAttributeName in parentResult:
            if newAttributeName not in result:
                result[newAttributeName] = parentResult[newAttributeName]

    for attr in currentClass.attributes:
        if attr.name not in result:
            card = attr.cardinality
            assert isinstance(card, M_Cardinality)
            lower = card.lower
            upper = card.upper
            result[attr.name] = (print_csharp_type(attr.type), lower, upper, attr)
    return result

def get_all_attributes_with_Xml_names(currentClass, mappings):
    assert isinstance(currentClass, M_Class)
    result = {}
    attributes = get_all_attributes(currentClass)
    fromCmofToXml = None
    if currentClass.name in mappings:    
        (_, fromCmofToXml) = mappings[currentClass.name]

    for (attName,(attType, l, u, attr)) in attributes.items():
        xmlName = None
        if fromCmofToXml is not None and attName in fromCmofToXml:
            xmlName = fromCmofToXml[attName]
        result[attName] = (attType,xmlName, l, u, attr)
    return result

# def get_all_required_attributes(currentClass, mappings):
#     assert isinstance(currentClass, M_Class)
#     result = []
#     parentClasses = [sc for sc in currentClass.superclasses if not(sc.name in interfaces)]
#     parentClass = parentClasses[0] if parentClasses else None
#     if parentClass != None:
#         result += get_all_required_attributes(parentClass, mappings) 
#     if currentClass.name in mappings:    
#         (fromXmlToCmof, fromCmofToXml, _) = mappings[currentClass.name]
#     required = []
#     if currentClass.name in extracted and fromXmlToCmof is not None:
#         (originalRequired,_,_) = extracted[currentClass.name]
#         required = [fromXmlToCmof[name] for name in originalRequired]
#     for attr in currentClass.attributes:
#         if attr.name in required:
#             result.append((attr.name, print_csharp_type(attr.type), fromCmofToXml[attr.name]))
#     return result

# def get_all_non_required_attributes(currentClass, mappings):
#     assert isinstance(currentClass, M_Class)
#     result = []
#     if currentClass.name not in interfaces:
#         parentClasses = [sc for sc in currentClass.superclasses]
#         for parentClass in parentClasses:
#             result += get_all_non_required_attributes(parentClass, mappings) 

#     if currentClass.name in mappings:    
#         (fromXmlToCmof, fromCmofToXml, _) = mappings[currentClass.name]
#     optional = []
#     if currentClass.name in extracted and fromXmlToCmof is not None:
#         (_,originalOptional,_) = extracted[currentClass.name]
#         for name in originalOptional:
#             if name in fromXmlToCmof:
#                 optional.append(fromXmlToCmof[name])
#             elif name == "id" and "id" not in optional :
#                 optional.append("id")

#     for attr in currentClass.attributes:
#         if attr.name in optional:
#             result.append((attr.name, print_csharp_type(attr.type), fromCmofToXml[attr.name]))
#     return result



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

    realName = capitalize_first_letter(c.name)
    card = c.cardinality
    assert isinstance(card, M_Cardinality)
    lower = card.lower
    upper = card.upper
    if isInInterface:
        if upper == 1:
            out.write(print_csharp_type(c.type)+"? " + realName + " { get; set; }")
        else:
            out.write("List<"+print_csharp_type(c.type)+"> "+ realName + " { get; }")
    else:
        if upper == 1:
            out.write("public "+print_csharp_type(c.type)+"? " + realName + " { get; set; }")
        else:
            out.write("public List<"+print_csharp_type(c.type)+"> "+ realName + " { get; } = new();")
    out.nl()

def print_Enumeration(out, c):
    out.write("public enum " + c.name).nl().write("{").nl()

    enumNames.append(c.name)

    out.inc()
    for item in c.literals:
        assert item.enum is c
        out.write(item.name).write(",").nl()
    out.dec()
    out.write("}").nl()

