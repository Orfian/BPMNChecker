
from cmof_model import *
from utils import Output

extracted = {
	"Font": ([], ["name", "size", "isBold", "isItalic", "isUnderline", "isStrikeThrough"], []),
	"Point": (["x", "y"], [], []),
	"Bounds": (["x", "y", "width", "height"], [], []),
	"Extension": ([], [], ["any"]),
	"DiagramElement": ([], ["id"], ["Extension"]),
	"Diagram": ([], ["name", "documentation", "resolution", "id"], []),
	"Node": ([], ["id"], ["Extension"]),
	"Edge": ([], ["id"], ["Extension", "waypoint"]),
	"LabeledEdge": ([], ["id"], ["Extension", "waypoint"]),
	"Shape": ([], ["id"], ["Extension", "Bounds"]),
	"LabeledShape": ([], ["id"], ["Extension", "Bounds"]),
	"Label": ([], ["id"], ["Extension", "Bounds"]),
	"Plane": ([], ["id"], ["Extension", "DiagramElement"]),
	"Style": ([], ["id"], []),
	"BPMNDiagram": ([], ["name", "documentation", "resolution", "id"], ["BPMNPlane", "BPMNLabelStyle"]),
	"BPMNPlane": ([], ["id", "bpmnElement"], ["Extension", "DiagramElement"]),
	"BPMNEdge": ([], ["id", "bpmnElement", "sourceElement", "targetElement", "messageVisibleKind"], ["Extension", "waypoint", "BPMNLabel"]),
	"BPMNShape": ([], ["id", "bpmnElement", "isHorizontal", "isExpanded", "isMarkerVisible", "isMessageVisible", "participantBandKind", "choreographyActivityShape"], ["Extension", "Bounds", "BPMNLabel"]),
	"BPMNLabel": ([], ["id", "labelStyle"], ["Extension", "Bounds"]),
	"BPMNLabelStyle": ([], ["id"], ["Font"]),
	"Activity": ([], ["id", "name", "camunda:modelerTemplate", "camunda:modelerTemplateVersion", "isForCompensation", "startQuantity", "completionQuantity", "default", "camunda:async", "camunda:asyncBefore", "camunda:asyncAfter", "camunda:exclusive", "camunda:jobPriority"], ["documentation", "extensionElements", "auditing", "monitoring", "categoryValueRef", "incoming", "outgoing", "ioSpecification", "property", "dataInputAssociation", "dataOutputAssociation", "resourceRole", "loopCharacteristics"]),
	"AdHocSubProcess": ([], ["id", "name", "camunda:modelerTemplate", "camunda:modelerTemplateVersion", "isForCompensation", "startQuantity", "completionQuantity", "default", "camunda:async", "camunda:asyncBefore", "camunda:asyncAfter", "camunda:exclusive", "camunda:jobPriority", "triggeredByEvent", "cancelRemainingInstances", "ordering"], ["documentation", "extensionElements", "auditing", "monitoring", "categoryValueRef", "incoming", "outgoing", "ioSpecification", "property", "dataInputAssociation", "dataOutputAssociation", "resourceRole", "loopCharacteristics", "laneSet", "flowElement", "artifact", "completionCondition"]),
	"Artifact": ([], ["id"], ["documentation", "extensionElements"]),
	"Assignment": ([], ["id"], ["documentation", "extensionElements", "from", "to"]),
	"Association": (["sourceRef", "targetRef"], ["id", "associationDirection"], ["documentation", "extensionElements"]),
	"Auditing": ([], ["id"], ["documentation", "extensionElements"]),
	"BaseElement": ([], ["id"], ["documentation", "extensionElements"]),
	"BaseElementWithMixedContent": ([], ["id"], ["documentation", "extensionElements"]),
	"BoundaryEvent": (["attachedToRef"], ["id", "name", "camunda:modelerTemplate", "camunda:modelerTemplateVersion", "camunda:async", "camunda:asyncBefore", "camunda:asyncAfter", "camunda:exclusive", "camunda:jobPriority", "parallelMultiple", "cancelActivity"], ["documentation", "extensionElements", "auditing", "monitoring", "categoryValueRef", "incoming", "outgoing", "property", "dataOutput", "dataOutputAssociation", "outputSet", "eventDefinition", "eventDefinitionRef"]),
	"BusinessRuleTask": ([], ["id", "name", "camunda:modelerTemplate", "camunda:modelerTemplateVersion", "isForCompensation", "startQuantity", "completionQuantity", "default", "camunda:async", "camunda:asyncBefore", "camunda:asyncAfter", "camunda:exclusive", "camunda:jobPriority", "implementation", "camunda:expression", "camunda:class", "camunda:delegateExpression", "camunda:resultVariable", "camunda:type", "camunda:topic", "camunda:taskPriority", "camunda:decisionRef", "camunda:decisionRefBinding", "camunda:decisionRefVersion", "camunda:mapDecisionResult", "camunda:decisionRefTenantId"], ["documentation", "extensionElements", "auditing", "monitoring", "categoryValueRef", "incoming", "outgoing", "ioSpecification", "property", "dataInputAssociation", "dataOutputAssociation", "resourceRole", "loopCharacteristics"]),
	"CallableElement": ([], ["id", "name"], ["documentation", "extensionElements", "supportedInterfaceRef", "ioSpecification", "ioBinding"]),
	"CallActivity": ([], ["id", "name", "camunda:modelerTemplate", "camunda:modelerTemplateVersion", "isForCompensation", "startQuantity", "completionQuantity", "default", "camunda:async", "camunda:asyncBefore", "camunda:asyncAfter", "camunda:exclusive", "camunda:jobPriority", "calledElement", "camunda:calledElementBinding", "camunda:calledElementVersion", "camunda:calledElementVersionTag", "camunda:calledElementTenantId", "camunda:caseRef", "camunda:caseBinding", "camunda:caseVersion", "camunda:caseTenantId", "camunda:variableMappingClass", "camunda:variableMappingDelegateExpression"], ["documentation", "extensionElements", "auditing", "monitoring", "categoryValueRef", "incoming", "outgoing", "ioSpecification", "property", "dataInputAssociation", "dataOutputAssociation", "resourceRole", "loopCharacteristics"]),
	"CallChoreography": (["initiatingParticipantRef"], ["id", "name", "camunda:modelerTemplate", "camunda:modelerTemplateVersion", "loopType", "calledChoreographyRef"], ["documentation", "extensionElements", "auditing", "monitoring", "categoryValueRef", "incoming", "outgoing", "participantRef", "correlationKey", "participantAssociation"]),
	"CallConversation": ([], ["id", "name", "calledCollaborationRef"], ["documentation", "extensionElements", "participantRef", "messageFlowRef", "correlationKey", "participantAssociation"]),
	"CancelEventDefinition": ([], ["id"], ["documentation", "extensionElements"]),
	"CatchEvent": ([], ["id", "name", "camunda:modelerTemplate", "camunda:modelerTemplateVersion", "camunda:async", "camunda:asyncBefore", "camunda:asyncAfter", "camunda:exclusive", "camunda:jobPriority", "parallelMultiple"], ["documentation", "extensionElements", "auditing", "monitoring", "categoryValueRef", "incoming", "outgoing", "property", "dataOutput", "dataOutputAssociation", "outputSet", "eventDefinition", "eventDefinitionRef"]),
	"Category": ([], ["id", "name"], ["documentation", "extensionElements", "categoryValue"]),
	"CategoryValue": ([], ["id", "value"], ["documentation", "extensionElements"]),
	"Choreography": ([], ["id", "name", "isClosed", "camunda:modelerTemplate", "camunda:modelerTemplateVersion"], ["documentation", "extensionElements", "participant", "messageFlow", "artifact", "conversationNode", "conversationAssociation", "participantAssociation", "messageFlowAssociation", "correlationKey", "choreographyRef", "conversationLink", "flowElement"]),
	"ChoreographyActivity": (["initiatingParticipantRef"], ["id", "name", "camunda:modelerTemplate", "camunda:modelerTemplateVersion", "loopType"], ["documentation", "extensionElements", "auditing", "monitoring", "categoryValueRef", "incoming", "outgoing", "participantRef", "correlationKey"]),
	"ChoreographyTask": (["initiatingParticipantRef"], ["id", "name", "camunda:modelerTemplate", "camunda:modelerTemplateVersion", "loopType"], ["documentation", "extensionElements", "auditing", "monitoring", "categoryValueRef", "incoming", "outgoing", "participantRef", "correlationKey", "messageFlowRef"]),
	"Collaboration": ([], ["id", "name", "isClosed", "camunda:modelerTemplate", "camunda:modelerTemplateVersion"], ["documentation", "extensionElements", "participant", "messageFlow", "artifact", "conversationNode", "conversationAssociation", "participantAssociation", "messageFlowAssociation", "correlationKey", "choreographyRef", "conversationLink"]),
	"CompensateEventDefinition": ([], ["id", "waitForCompletion", "activityRef"], ["documentation", "extensionElements"]),
	"ComplexBehaviorDefinition": ([], ["id"], ["documentation", "extensionElements", "condition", "event"]),
	"ComplexGateway": ([], ["id", "name", "camunda:modelerTemplate", "camunda:modelerTemplateVersion", "gatewayDirection", "camunda:async", "camunda:asyncBefore", "camunda:asyncAfter", "camunda:exclusive", "camunda:jobPriority", "default"], ["documentation", "extensionElements", "auditing", "monitoring", "categoryValueRef", "incoming", "outgoing", "activationCondition"]),
	"ConditionalEventDefinition": ([], ["id", "camunda:variableName", "camunda:variableEvents"], ["documentation", "extensionElements", "condition"]),
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
	"DataObject": ([], ["id", "name", "camunda:modelerTemplate", "camunda:modelerTemplateVersion", "itemSubjectRef", "isCollection"], ["documentation", "extensionElements", "auditing", "monitoring", "categoryValueRef", "dataState"]),
	"DataObjectReference": ([], ["id", "name", "camunda:modelerTemplate", "camunda:modelerTemplateVersion", "itemSubjectRef", "dataObjectRef"], ["documentation", "extensionElements", "auditing", "monitoring", "categoryValueRef", "dataState"]),
	"DataOutput": ([], ["id", "name", "itemSubjectRef", "isCollection"], ["documentation", "extensionElements", "dataState"]),
	"DataOutputAssociation": ([], ["id"], ["documentation", "extensionElements", "sourceRef", "targetRef", "transformation", "assignment"]),
	"DataState": ([], ["id", "name"], ["documentation", "extensionElements"]),
	"DataStore": ([], ["id", "name", "capacity", "isUnlimited", "itemSubjectRef"], ["documentation", "extensionElements", "dataState"]),
	"DataStoreReference": ([], ["id", "name", "camunda:modelerTemplate", "camunda:modelerTemplateVersion", "itemSubjectRef", "dataStoreRef"], ["documentation", "extensionElements", "auditing", "monitoring", "categoryValueRef", "dataState"]),
	"Documentation": ([], ["id", "textFormat"], ["any"]),
	"EndEvent": ([], ["id", "name", "camunda:modelerTemplate", "camunda:modelerTemplateVersion", "camunda:async", "camunda:asyncBefore", "camunda:asyncAfter", "camunda:exclusive", "camunda:jobPriority"], ["documentation", "extensionElements", "auditing", "monitoring", "categoryValueRef", "incoming", "outgoing", "property", "dataInput", "dataInputAssociation", "inputSet", "eventDefinition", "eventDefinitionRef"]),
	"EndPoint": ([], ["id"], ["documentation", "extensionElements"]),
	"Error": ([], ["id", "name", "errorCode", "structureRef", "camunda:errorMessage"], ["documentation", "extensionElements"]),
	"ErrorEventDefinition": ([], ["id", "errorRef", "camunda:errorCodeVariable", "camunda:errorMessageVariable"], ["documentation", "extensionElements"]),
	"Escalation": ([], ["id", "name", "escalationCode", "structureRef"], ["documentation", "extensionElements"]),
	"EscalationEventDefinition": ([], ["id", "escalationRef", "camunda:escalationCodeVariable"], ["documentation", "extensionElements"]),
	"Event": ([], ["id", "name", "camunda:modelerTemplate", "camunda:modelerTemplateVersion", "camunda:async", "camunda:asyncBefore", "camunda:asyncAfter", "camunda:exclusive", "camunda:jobPriority"], ["documentation", "extensionElements", "auditing", "monitoring", "categoryValueRef", "incoming", "outgoing", "property"]),
	"EventBasedGateway": ([], ["id", "name", "camunda:modelerTemplate", "camunda:modelerTemplateVersion", "gatewayDirection", "camunda:async", "camunda:asyncBefore", "camunda:asyncAfter", "camunda:exclusive", "camunda:jobPriority", "instantiate", "eventGatewayType"], ["documentation", "extensionElements", "auditing", "monitoring", "categoryValueRef", "incoming", "outgoing"]),
	"EventDefinition": ([], ["id"], ["documentation", "extensionElements"]),
	"ExclusiveGateway": ([], ["id", "name", "camunda:modelerTemplate", "camunda:modelerTemplateVersion", "gatewayDirection", "camunda:async", "camunda:asyncBefore", "camunda:asyncAfter", "camunda:exclusive", "camunda:jobPriority", "default"], ["documentation", "extensionElements", "auditing", "monitoring", "categoryValueRef", "incoming", "outgoing"]),
	"Expression": ([], ["id"], ["documentation", "extensionElements"]),
	"Extension": ([], ["definition", "mustUnderstand"], ["documentation"]),
	"ExtensionElements": ([], [], ["any"]),
	"FlowElement": ([], ["id", "name", "camunda:modelerTemplate", "camunda:modelerTemplateVersion"], ["documentation", "extensionElements", "auditing", "monitoring", "categoryValueRef"]),
	"FlowNode": ([], ["id", "name", "camunda:modelerTemplate", "camunda:modelerTemplateVersion"], ["documentation", "extensionElements", "auditing", "monitoring", "categoryValueRef", "incoming", "outgoing"]),
	"FormalExpression": ([], ["id", "language", "evaluatesToTypeRef", "camunda:resource"], ["documentation", "extensionElements"]),
	"Gateway": ([], ["id", "name", "camunda:modelerTemplate", "camunda:modelerTemplateVersion", "gatewayDirection", "camunda:async", "camunda:asyncBefore", "camunda:asyncAfter", "camunda:exclusive", "camunda:jobPriority"], ["documentation", "extensionElements", "auditing", "monitoring", "categoryValueRef", "incoming", "outgoing"]),
	"GlobalBusinessRuleTask": ([], ["id", "name", "implementation"], ["documentation", "extensionElements", "supportedInterfaceRef", "ioSpecification", "ioBinding", "resourceRole"]),
	"GlobalChoreographyTask": ([], ["id", "name", "isClosed", "camunda:modelerTemplate", "camunda:modelerTemplateVersion", "initiatingParticipantRef"], ["documentation", "extensionElements", "participant", "messageFlow", "artifact", "conversationNode", "conversationAssociation", "participantAssociation", "messageFlowAssociation", "correlationKey", "choreographyRef", "conversationLink", "flowElement"]),
	"GlobalConversation": ([], ["id", "name", "isClosed", "camunda:modelerTemplate", "camunda:modelerTemplateVersion"], ["documentation", "extensionElements", "participant", "messageFlow", "artifact", "conversationNode", "conversationAssociation", "participantAssociation", "messageFlowAssociation", "correlationKey", "choreographyRef", "conversationLink"]),
	"GlobalManualTask": ([], ["id", "name"], ["documentation", "extensionElements", "supportedInterfaceRef", "ioSpecification", "ioBinding", "resourceRole"]),
	"GlobalScriptTask": ([], ["id", "name", "scriptLanguage"], ["documentation", "extensionElements", "supportedInterfaceRef", "ioSpecification", "ioBinding", "resourceRole", "script"]),
	"GlobalTask": ([], ["id", "name"], ["documentation", "extensionElements", "supportedInterfaceRef", "ioSpecification", "ioBinding", "resourceRole"]),
	"GlobalUserTask": ([], ["id", "name", "implementation"], ["documentation", "extensionElements", "supportedInterfaceRef", "ioSpecification", "ioBinding", "resourceRole", "rendering"]),
	"Group": ([], ["id", "categoryValueRef"], ["documentation", "extensionElements"]),
	"HumanPerformer": ([], ["id", "name"], ["documentation", "extensionElements", "resourceRef", "resourceParameterBinding", "resourceAssignmentExpression"]),
	"ImplicitThrowEvent": ([], ["id", "name", "camunda:modelerTemplate", "camunda:modelerTemplateVersion", "camunda:async", "camunda:asyncBefore", "camunda:asyncAfter", "camunda:exclusive", "camunda:jobPriority"], ["documentation", "extensionElements", "auditing", "monitoring", "categoryValueRef", "incoming", "outgoing", "property", "dataInput", "dataInputAssociation", "inputSet", "eventDefinition", "eventDefinitionRef"]),
	"InclusiveGateway": ([], ["id", "name", "camunda:modelerTemplate", "camunda:modelerTemplateVersion", "gatewayDirection", "camunda:async", "camunda:asyncBefore", "camunda:asyncAfter", "camunda:exclusive", "camunda:jobPriority", "default"], ["documentation", "extensionElements", "auditing", "monitoring", "categoryValueRef", "incoming", "outgoing"]),
	"InputSet": ([], ["id", "name"], ["documentation", "extensionElements", "dataInputRefs", "optionalInputRefs", "whileExecutingInputRefs", "outputSetRefs"]),
	"Interface": (["name"], ["id", "implementationRef"], ["documentation", "extensionElements", "operation"]),
	"IntermediateCatchEvent": ([], ["id", "name", "camunda:modelerTemplate", "camunda:modelerTemplateVersion", "camunda:async", "camunda:asyncBefore", "camunda:asyncAfter", "camunda:exclusive", "camunda:jobPriority", "parallelMultiple"], ["documentation", "extensionElements", "auditing", "monitoring", "categoryValueRef", "incoming", "outgoing", "property", "dataOutput", "dataOutputAssociation", "outputSet", "eventDefinition", "eventDefinitionRef"]),
	"IntermediateThrowEvent": ([], ["id", "name", "camunda:modelerTemplate", "camunda:modelerTemplateVersion", "camunda:async", "camunda:asyncBefore", "camunda:asyncAfter", "camunda:exclusive", "camunda:jobPriority"], ["documentation", "extensionElements", "auditing", "monitoring", "categoryValueRef", "incoming", "outgoing", "property", "dataInput", "dataInputAssociation", "inputSet", "eventDefinition", "eventDefinitionRef"]),
	"InputOutputBinding": (["operationRef", "inputDataRef", "outputDataRef"], ["id"], ["documentation", "extensionElements"]),
	"InputOutputSpecification": ([], ["id"], ["documentation", "extensionElements", "dataInput", "dataOutput", "inputSet", "outputSet"]),
	"ItemDefinition": ([], ["id", "structureRef", "isCollection", "itemKind"], ["documentation", "extensionElements"]),
	"Lane": ([], ["id", "name", "partitionElementRef"], ["documentation", "extensionElements", "partitionElement", "flowNodeRef", "childLaneSet"]),
	"LaneSet": ([], ["id", "name"], ["documentation", "extensionElements", "lane"]),
	"LinkEventDefinition": (["name"], ["id"], ["documentation", "extensionElements", "source", "target"]),
	"LoopCharacteristics": ([], ["id"], ["documentation", "extensionElements"]),
	"ManualTask": ([], ["id", "name", "camunda:modelerTemplate", "camunda:modelerTemplateVersion", "isForCompensation", "startQuantity", "completionQuantity", "default", "camunda:async", "camunda:asyncBefore", "camunda:asyncAfter", "camunda:exclusive", "camunda:jobPriority"], ["documentation", "extensionElements", "auditing", "monitoring", "categoryValueRef", "incoming", "outgoing", "ioSpecification", "property", "dataInputAssociation", "dataOutputAssociation", "resourceRole", "loopCharacteristics"]),
	"Message": ([], ["id", "name", "itemRef"], ["documentation", "extensionElements"]),
	"MessageEventDefinition": ([], ["id", "messageRef", "camunda:expression", "camunda:class", "camunda:delegateExpression", "camunda:resultVariable", "camunda:type", "camunda:topic", "camunda:taskPriority"], ["documentation", "extensionElements", "operationRef"]),
	"MessageFlow": (["sourceRef", "targetRef"], ["id", "name", "messageRef"], ["documentation", "extensionElements"]),
	"MessageFlowAssociation": (["innerMessageFlowRef", "outerMessageFlowRef"], ["id"], ["documentation", "extensionElements"]),
	"Monitoring": ([], ["id"], ["documentation", "extensionElements"]),
	"MultiInstanceLoopCharacteristics": ([], ["id", "isSequential", "behavior", "oneBehaviorEventRef", "noneBehaviorEventRef", "camunda:async", "camunda:asyncBefore", "camunda:asyncAfter", "camunda:exclusive", "camunda:collection", "camunda:elementVariable"], ["documentation", "extensionElements", "loopCardinality", "loopDataInputRef", "loopDataOutputRef", "inputDataItem", "outputDataItem", "complexBehaviorDefinition", "completionCondition"]),
	"Operation": (["name"], ["id", "implementationRef"], ["documentation", "extensionElements", "inMessageRef", "outMessageRef", "errorRef"]),
	"OutputSet": ([], ["id", "name"], ["documentation", "extensionElements", "dataOutputRefs", "optionalOutputRefs", "whileExecutingOutputRefs", "inputSetRefs"]),
	"ParallelGateway": ([], ["id", "name", "camunda:modelerTemplate", "camunda:modelerTemplateVersion", "gatewayDirection", "camunda:async", "camunda:asyncBefore", "camunda:asyncAfter", "camunda:exclusive", "camunda:jobPriority"], ["documentation", "extensionElements", "auditing", "monitoring", "categoryValueRef", "incoming", "outgoing"]),
	"Participant": ([], ["id", "name", "processRef"], ["documentation", "extensionElements", "interfaceRef", "endPointRef", "participantMultiplicity"]),
	"ParticipantAssociation": ([], ["id"], ["documentation", "extensionElements", "innerParticipantRef", "outerParticipantRef"]),
	"ParticipantMultiplicity": ([], ["id", "minimum", "maximum"], ["documentation", "extensionElements"]),
	"PartnerEntity": ([], ["id", "name"], ["documentation", "extensionElements", "participantRef"]),
	"PartnerRole": ([], ["id", "name"], ["documentation", "extensionElements", "participantRef"]),
	"Performer": ([], ["id", "name"], ["documentation", "extensionElements", "resourceRef", "resourceParameterBinding", "resourceAssignmentExpression"]),
	"PotentialOwner": ([], ["id", "name"], ["documentation", "extensionElements", "resourceRef", "resourceParameterBinding", "resourceAssignmentExpression"]),
	"Process": ([], ["id", "name", "processType", "isClosed", "isExecutable", "definitionalCollaborationRef", "camunda:jobPriority", "camunda:modelerTemplate", "camunda:modelerTemplateVersion", "camunda:candidateStarterGroups", "camunda:candidateStarterUsers", "camunda:versionTag", "camunda:historyTimeToLive", "camunda:isStartableInTasklist", "camunda:taskPriority"], ["documentation", "extensionElements", "supportedInterfaceRef", "ioSpecification", "ioBinding", "auditing", "monitoring", "property", "laneSet", "flowElement", "artifact", "resourceRole", "correlationSubscription", "supports"]),
	"Property": ([], ["id", "name", "itemSubjectRef"], ["documentation", "extensionElements", "dataState"]),
	"ReceiveTask": ([], ["id", "name", "camunda:modelerTemplate", "camunda:modelerTemplateVersion", "isForCompensation", "startQuantity", "completionQuantity", "default", "camunda:async", "camunda:asyncBefore", "camunda:asyncAfter", "camunda:exclusive", "camunda:jobPriority", "implementation", "instantiate", "messageRef", "operationRef"], ["documentation", "extensionElements", "auditing", "monitoring", "categoryValueRef", "incoming", "outgoing", "ioSpecification", "property", "dataInputAssociation", "dataOutputAssociation", "resourceRole", "loopCharacteristics"]),
	"Relationship": (["type"], ["id", "direction"], ["documentation", "extensionElements", "source", "target"]),
	"Rendering": ([], ["id"], ["documentation", "extensionElements"]),
	"Resource": (["name"], ["id"], ["documentation", "extensionElements", "resourceParameter"]),
	"ResourceAssignmentExpression": ([], ["id"], ["documentation", "extensionElements", "expression"]),
	"ResourceParameter": ([], ["id", "name", "type", "isRequired"], ["documentation", "extensionElements"]),
	"ResourceParameterBinding": (["parameterRef"], ["id"], ["documentation", "extensionElements", "expression"]),
	"ResourceRole": ([], ["id", "name"], ["documentation", "extensionElements", "resourceRef", "resourceParameterBinding", "resourceAssignmentExpression"]),
	"RootElement": ([], ["id"], ["documentation", "extensionElements"]),
	"ScriptTask": ([], ["id", "name", "camunda:modelerTemplate", "camunda:modelerTemplateVersion", "isForCompensation", "startQuantity", "completionQuantity", "default", "camunda:async", "camunda:asyncBefore", "camunda:asyncAfter", "camunda:exclusive", "camunda:jobPriority", "scriptFormat", "camunda:resultVariable", "camunda:resource"], ["documentation", "extensionElements", "auditing", "monitoring", "categoryValueRef", "incoming", "outgoing", "ioSpecification", "property", "dataInputAssociation", "dataOutputAssociation", "resourceRole", "loopCharacteristics", "script"]),
	"Script": ([], [], ["any"]),
	"SendTask": ([], ["id", "name", "camunda:modelerTemplate", "camunda:modelerTemplateVersion", "isForCompensation", "startQuantity", "completionQuantity", "default", "camunda:async", "camunda:asyncBefore", "camunda:asyncAfter", "camunda:exclusive", "camunda:jobPriority", "implementation", "messageRef", "operationRef", "camunda:expression", "camunda:class", "camunda:delegateExpression", "camunda:resultVariable", "camunda:type", "camunda:topic", "camunda:taskPriority"], ["documentation", "extensionElements", "auditing", "monitoring", "categoryValueRef", "incoming", "outgoing", "ioSpecification", "property", "dataInputAssociation", "dataOutputAssociation", "resourceRole", "loopCharacteristics"]),
	"SequenceFlow": (["sourceRef", "targetRef"], ["id", "name", "camunda:modelerTemplate", "camunda:modelerTemplateVersion", "isImmediate"], ["documentation", "extensionElements", "auditing", "monitoring", "categoryValueRef", "conditionExpression"]),
	"ServiceTask": ([], ["id", "name", "camunda:modelerTemplate", "camunda:modelerTemplateVersion", "isForCompensation", "startQuantity", "completionQuantity", "default", "camunda:async", "camunda:asyncBefore", "camunda:asyncAfter", "camunda:exclusive", "camunda:jobPriority", "implementation", "operationRef", "camunda:expression", "camunda:class", "camunda:delegateExpression", "camunda:resultVariable", "camunda:type", "camunda:topic", "camunda:taskPriority"], ["documentation", "extensionElements", "auditing", "monitoring", "categoryValueRef", "incoming", "outgoing", "ioSpecification", "property", "dataInputAssociation", "dataOutputAssociation", "resourceRole", "loopCharacteristics"]),
	"Signal": ([], ["id", "name", "structureRef"], ["documentation", "extensionElements"]),
	"SignalEventDefinition": ([], ["id", "signalRef", "camunda:async"], ["documentation", "extensionElements"]),
	"StandardLoopCharacteristics": ([], ["id", "testBefore", "loopMaximum"], ["documentation", "extensionElements", "loopCondition"]),
	"StartEvent": ([], ["id", "name", "camunda:modelerTemplate", "camunda:modelerTemplateVersion", "camunda:async", "camunda:asyncBefore", "camunda:asyncAfter", "camunda:exclusive", "camunda:jobPriority", "parallelMultiple", "isInterrupting", "camunda:formHandlerClass", "camunda:formKey", "camunda:formRef", "camunda:formRefBinding", "camunda:formRefVersion", "camunda:initiator"], ["documentation", "extensionElements", "auditing", "monitoring", "categoryValueRef", "incoming", "outgoing", "property", "dataOutput", "dataOutputAssociation", "outputSet", "eventDefinition", "eventDefinitionRef"]),
	"SubChoreography": (["initiatingParticipantRef"], ["id", "name", "camunda:modelerTemplate", "camunda:modelerTemplateVersion", "loopType"], ["documentation", "extensionElements", "auditing", "monitoring", "categoryValueRef", "incoming", "outgoing", "participantRef", "correlationKey", "flowElement", "artifact"]),
	"SubConversation": ([], ["id", "name"], ["documentation", "extensionElements", "participantRef", "messageFlowRef", "correlationKey", "conversationNode"]),
	"SubProcess": ([], ["id", "name", "camunda:modelerTemplate", "camunda:modelerTemplateVersion", "isForCompensation", "startQuantity", "completionQuantity", "default", "camunda:async", "camunda:asyncBefore", "camunda:asyncAfter", "camunda:exclusive", "camunda:jobPriority", "triggeredByEvent"], ["documentation", "extensionElements", "auditing", "monitoring", "categoryValueRef", "incoming", "outgoing", "ioSpecification", "property", "dataInputAssociation", "dataOutputAssociation", "resourceRole", "loopCharacteristics", "laneSet", "flowElement", "artifact"]),
	"Task": ([], ["id", "name", "camunda:modelerTemplate", "camunda:modelerTemplateVersion", "isForCompensation", "startQuantity", "completionQuantity", "default", "camunda:async", "camunda:asyncBefore", "camunda:asyncAfter", "camunda:exclusive", "camunda:jobPriority"], ["documentation", "extensionElements", "auditing", "monitoring", "categoryValueRef", "incoming", "outgoing", "ioSpecification", "property", "dataInputAssociation", "dataOutputAssociation", "resourceRole", "loopCharacteristics"]),
	"TerminateEventDefinition": ([], ["id"], ["documentation", "extensionElements"]),
	"TextAnnotation": ([], ["id", "textFormat"], ["documentation", "extensionElements", "text"]),
	"Text": ([], [], ["any"]),
	"ThrowEvent": ([], ["id", "name", "camunda:modelerTemplate", "camunda:modelerTemplateVersion", "camunda:async", "camunda:asyncBefore", "camunda:asyncAfter", "camunda:exclusive", "camunda:jobPriority"], ["documentation", "extensionElements", "auditing", "monitoring", "categoryValueRef", "incoming", "outgoing", "property", "dataInput", "dataInputAssociation", "inputSet", "eventDefinition", "eventDefinitionRef"]),
	"TimerEventDefinition": ([], ["id"], ["documentation", "extensionElements", "timeDate", "timeDuration", "timeCycle"]),
	"Transaction": ([], ["id", "name", "camunda:modelerTemplate", "camunda:modelerTemplateVersion", "isForCompensation", "startQuantity", "completionQuantity", "default", "camunda:async", "camunda:asyncBefore", "camunda:asyncAfter", "camunda:exclusive", "camunda:jobPriority", "triggeredByEvent", "method"], ["documentation", "extensionElements", "auditing", "monitoring", "categoryValueRef", "incoming", "outgoing", "ioSpecification", "property", "dataInputAssociation", "dataOutputAssociation", "resourceRole", "loopCharacteristics", "laneSet", "flowElement", "artifact"]),
	"UserTask": ([], ["id", "name", "camunda:modelerTemplate", "camunda:modelerTemplateVersion", "isForCompensation", "startQuantity", "completionQuantity", "default", "camunda:async", "camunda:asyncBefore", "camunda:asyncAfter", "camunda:exclusive", "camunda:jobPriority", "implementation", "camunda:formHandlerClass", "camunda:formKey", "camunda:formRef", "camunda:formRefBinding", "camunda:formRefVersion", "camunda:assignee", "camunda:candidateUsers", "camunda:candidateGroups", "camunda:dueDate", "camunda:followUpDate", "camunda:priority"], ["documentation", "extensionElements", "auditing", "monitoring", "categoryValueRef", "incoming", "outgoing", "ioSpecification", "property", "dataInputAssociation", "dataOutputAssociation", "resourceRole", "loopCharacteristics", "rendering"]),
	"Definitions": (["targetNamespace"], ["id", "name", "expressionLanguage", "typeLanguage", "exporter", "exporterVersion", "camunda:diagramRelationId"], ["import", "extension", "rootElement", "BPMNDiagram", "relationship"]),
	"Import": (["namespace", "location", "importType"], [], []),
	}

camundaAttributes = {
  "Definitions" : [("String","camunda:diagramRelationId")],
  "Activity" : [("Boolean","camunda:async"), ("Boolean","camunda:asyncBefore"), ("Boolean","camunda:asyncAfter"), ("Boolean","camunda:exclusive"), ("String","camunda:jobPriority")],
  "Gateway" : [("Boolean","camunda:async"), ("Boolean","camunda:asyncBefore"), ("Boolean","camunda:asyncAfter"), ("Boolean","camunda:exclusive"), ("String","camunda:jobPriority")],
  "Event" : [("Boolean","camunda:async"), ("Boolean","camunda:asyncBefore"), ("Boolean","camunda:asyncAfter"), ("Boolean","camunda:exclusive"), ("String","camunda:jobPriority")],
  "Process" : [("String","camunda:jobPriority"), ("String","camunda:modelerTemplate"), ("Integer","camunda:modelerTemplateVersion"), ("String","camunda:candidateStarterGroups"), ("String","camunda:candidateStarterUsers"), ("String","camunda:versionTag"), ("String","camunda:historyTimeToLive"), ("Boolean","camunda:isStartableInTasklist"), ("String","camunda:taskPriority")],
  "SignalEventDefinition" : [("Boolean","camunda:async")],
  "ErrorEventDefinition" : [("String","camunda:errorCodeVariable"), ("String","camunda:errorMessageVariable")],
  "Error" : [("String","camunda:errorMessage")],
  "StartEvent" : [("String","camunda:formHandlerClass"), ("String","camunda:formKey"), ("String","camunda:formRef"), ("String","camunda:formRefBinding"), ("String","camunda:formRefVersion"), ("String","camunda:initiator")],
  "UserTask" : [("String","camunda:formHandlerClass"), ("String","camunda:formKey"), ("String","camunda:formRef"), ("String","camunda:formRefBinding"), ("String","camunda:formRefVersion"), ("String","camunda:assignee"), ("String","camunda:candidateUsers"), ("String","camunda:candidateGroups"), ("String","camunda:dueDate"), ("String","camunda:followUpDate"), ("String","camunda:priority")],
  "Collaboration" : [("String","camunda:modelerTemplate"), ("Integer","camunda:modelerTemplateVersion")],
  "FlowElement" : [("String","camunda:modelerTemplate"), ("Integer","camunda:modelerTemplateVersion")],
  "ScriptTask" : [("String","camunda:resultVariable"), ("String","camunda:resource")],
  "EscalationEventDefinition" : [("String","camunda:escalationCodeVariable")],
  "FormalExpression" : [("String","camunda:resource")],
  "CallActivity" : [("String","camunda:calledElementBinding"), ("String","camunda:calledElementVersion"), ("String","camunda:calledElementVersionTag"), ("String","camunda:calledElementTenantId"), ("String","camunda:caseRef"), ("String","camunda:caseBinding"), ("String","camunda:caseVersion"), ("String","camunda:caseTenantId"), ("String","camunda:variableMappingClass"), ("String","camunda:variableMappingDelegateExpression")],
  "ServiceTask" : [("String","camunda:expression"), ("String","camunda:class"), ("String","camunda:delegateExpression"), ("String","camunda:resultVariable"), ("String","camunda:type"), ("String","camunda:topic"), ("String","camunda:taskPriority")],
  "BusinessRuleTask" : [("String","camunda:expression"), ("String","camunda:class"), ("String","camunda:delegateExpression"), ("String","camunda:resultVariable"), ("String","camunda:type"), ("String","camunda:topic"), ("String","camunda:taskPriority"), ("String","camunda:decisionRef"), ("String","camunda:decisionRefBinding"), ("String","camunda:decisionRefVersion"), ("String","camunda:mapDecisionResult"), ("String","camunda:decisionRefTenantId")],
  "SendTask" : [("String","camunda:expression"), ("String","camunda:class"), ("String","camunda:delegateExpression"), ("String","camunda:resultVariable"), ("String","camunda:type"), ("String","camunda:topic"), ("String","camunda:taskPriority")],
  "MessageEventDefinition" : [("String","camunda:expression"), ("String","camunda:class"), ("String","camunda:delegateExpression"), ("String","camunda:resultVariable"), ("String","camunda:type"), ("String","camunda:topic"), ("String","camunda:taskPriority")],
  "MultiInstanceLoopCharacteristics" : [("Boolean","camunda:async"), ("Boolean","camunda:asyncBefore"), ("Boolean","camunda:asyncAfter"), ("Boolean","camunda:exclusive"), ("String","camunda:collection"), ("String","camunda:elementVariable")],
  "ConditionalEventDefinition" : [("String","camunda:variableName"), ("String","camunda:variableEvents")],
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
dataClassesNames = []

def capitalize_first_letter(s):
    if not s:
        return s
    if s.startswith("camunda:"):
        return s.replace("camunda:", "Camunda_", 1)
    return s[0].upper() + s[1:]

def print_file_header(out):
    out.write("/// <summary>").nl();
    out.write("/// This file was generated by Python utility - do NOT modify it directly.").nl();
    out.write("/// </summary>").nl()


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

    assert isinstance(model, M_Combined_Model)

    print(path)
    mappings = build_mapping(model.get_all_data_types() + model.get_all_classes())
    
    print_mapping(mappings, [c.name for c in (model.get_all_data_types() + model.get_all_classes())])

    out = Output(open(path + r"\IModelVisitor.cs", "w"))
    print_file_header(out)
    out.write("namespace BPMNModel.Model").nl()
    out.write("{").nl()
    out.inc()
    out.write("public interface IModelVisitor<TResult> : IBaseVisitor<TResult> where TResult : class").nl()
    out.write("{").nl()
    out.inc()
    for c in model.get_all_classes():
        if not (c.name in interfaces):
            out.write("TResult? Visit"+c.name+"([NotNull] "+c.name+" context);").nl()

    out.dec()
    out.write("}").nl()
    out.dec()
    out.write("}").nl()

    out = Output(open(path + r"\BaseModelVisitor.cs", "w"))
    print_file_header(out)
    out.write("namespace BPMNModel.Model").nl()
    out.write("{").nl()
    out.inc()
    out.write("public abstract class BaseModelVisitor<TResult> : AbstractModelVisitor<TResult>, IModelVisitor<TResult> where TResult : class").nl()
    out.write("{").nl()
    out.inc()
    for c in model.get_all_classes():
        if not (c.name in interfaces):
            out.write("public virtual TResult? Visit"+c.name+"([NotNull] "+c.name+" context) { return VisitOnceChildren(context); }").nl()

    out.dec()
    out.write("}").nl()
    out.dec()
    out.write("}").nl()


    out = Output(open(path + r"\File.cs", "w"))
    print_file_header(out)
    out.write("namespace BPMNModel.Model").nl()
    out.write("{").nl()
    out.inc()

    for c in model.get_all_data_types():
        dataClassesNames.append(c.name)

    print_enumerations(out, model.get_all_enumerations())
    print_classes_and_data_types(out, model.get_all_data_types() + model.get_all_classes(), mappings)
    out.dec()
    out.write("}").nl()
   

    out = Output(open(path + r"\Factory.cs", "w"))
    print_file_header(out)
    out.write("namespace BPMNModel.Model").nl()
    out.write("{").nl()
    out.inc()
    out.write("using BPMNModel.XMLParser;").nl()
    out.write("using Utility;").nl()
    out.nl()
    out.write("public partial class Factory").nl()
    out.write("{").nl()
    out.inc()

    postProcessing = print_factories(out, model.get_all_data_types() + model.get_all_classes(), mappings)
    
    out.dec()
    out.write("}").nl()
    out.dec()
    out.write("}").nl()

    post = Output(open(path + r"\PostProcessing.cs", "w"))
    print_file_header(post)
    post.write("namespace BPMNModel.Model").nl()
    post.write("{").nl()
    post.inc()
    post.write("public static class PostProcessing").nl()
    post.write("{").nl()
    post.inc()
    post.write("public static void Process(object item)").nl()
    post.write("{").nl()
    post.inc()

    print_post_processing(post, postProcessing)

    post.dec()
    post.write("}").nl()
    post.dec()
    post.write("}").nl()
    post.dec()
    post.write("}").nl()


def print_post_processing(out, postProcessing):
    postProcessingSet = set(postProcessing)
    sortedData = sorted( list(postProcessingSet))
    counter = 0
    for (setInXmlClass, setInXmlProperty , setInXmlArity, missingInType, missingInListProperty) in sortedData:
        out.write("// If "+ setInXmlClass + " get " + setInXmlProperty);
        if setInXmlArity == 1:
            out.write(" use it like ")
        else:
            out.write(" for all elements used like ")
        out.write(missingInType + ", put THIS into its List " + missingInListProperty ).nl()

        castName = "casted" + str(counter)
        counter+=1
        realSetInXmlProperty =  capitalize_first_letter(setInXmlProperty)
        realMissingInListProperty = capitalize_first_letter(missingInListProperty)            

        if setInXmlArity == 1:
            # if (item is ConversationLink casted1 && casted1.SourceRef is not null) casted1.SourceRef.OutgoingConversationLinks.Add(casted1);
            out.write("if (item is " + setInXmlClass + " " + castName + " && "+castName+"."+realSetInXmlProperty+" is not null) ")
            out.write(castName+"."+ realSetInXmlProperty + "." +realMissingInListProperty +".Add("+castName +");").nl()
        else:
            # if (item is Lane casted2) casted2.FlowNodeRefs.ForEach(x => x.Lanes.Add(casted2));
            out.write("if (item is " + setInXmlClass + " " + castName + ") ")
            out.write(castName + "." + realSetInXmlProperty + ".ForEach(x => x." + realMissingInListProperty+ ".Add("+castName+"));").nl()
        out.nl()

def build_mapping(classes):
    n = len(classes)
    if n == 0: return
    result = {}
    for c in classes:
        print(c.name)
        fromXMLtoCMOF = {}
        fromCMOFtoXML = {}
        assert isinstance(c, M_Class) | isinstance(c, M_DataType)
        if not c.name in extracted:
            continue;
        (reqiredFromXSD, optionalFormXSD, elementsFromXSD) = extracted[c.name]

        if isinstance(c, M_Class):
            allCMOFAtributes = get_all_attributes_names(c)
        else:
            allCMOFAtributes = [attr.name for attr in c.attributes]

        allXMLAttributes = reqiredFromXSD+optionalFormXSD+elementsFromXSD
        #print(c.name + " : "+ ("" if parentClass is None else parentClass) + " , " + ("" if implementedInterface is None else implementedInterface.name))
        print("\t"+ ", ".join(allXMLAttributes))
        print("\t"+", ".join(allCMOFAtributes))
        for xmlAtt in allXMLAttributes:
            matches = [cmofAtt for cmofAtt in allCMOFAtributes if remove_trailing_s(xmlAtt).lower() == remove_trailing_s(cmofAtt).lower()]
            if len(matches) == 1 :
                fromXMLtoCMOF[xmlAtt] = matches[0]
                fromCMOFtoXML[matches[0]] = xmlAtt
                allCMOFAtributes.remove(matches[0])
            else:
                assert len(matches) == 0
        result[c.name] = (fromXMLtoCMOF, fromCMOFtoXML)
    return result


def print_factories(out, classes, mappings):
    postProcessing = []
    n = len(classes)
    if n == 0: return
    out.write("#region Factories").nl()
    for c in classes:
        out.write("// ").write(c.name).nl()
        postProcessing += print_factory(out, c, mappings)
    out.write("#endregion").nl().nl()
    return postProcessing
 
def print_classes_and_data_types(out, all, mapping):
    n = len(all)
    if n == 0: return
    out.write("#region Classes (%d items)" % n).nl()
    for c in all:
        out.nl()
        print_class_or_data_type(out, c, mapping)
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
    postProcessing = []
    assert isinstance(c, M_Class) | isinstance(c, M_DataType)
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
                if requiredType == "string" or requiredType == "long" or requiredType == "bool" or requiredType == "double":
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
                if optionalType == "string" or optionalType == "long" or optionalType == "bool" or optionalType == "double":
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
            if itemName not in processed + ["extensionValues", "extensionDefinitions", "documentation", "owningElement", "owningDiagram", "ownedElement", "rootElement"]:
                #expecting just unlinked attributes
                if itemXmlName is not None:
                    raise Exception("Expecting no xml name, but have:"+ itemXmlName);

                assert isinstance(attr, M_Attribute)
                association = attr.association
                if association is None:
                    out.write("// missing: "+ itemType + " "+ itemName + get_arity(l,h)).nl()
                    out.write("ManualySolve_"+itemName+"_in_"+c.name+"(result, node);")
                elif association.is_one_way():
                    assert isinstance(association, M_One_Way_Association)
                    out.write("// empty: "+ itemType + " "+ itemName + get_arity(l,h)).nl()
                else:          
                    assert h <0               
                    assert isinstance(association, M_Two_Way_Association)
                    target = attr.association.get_other_attr(attr.assoc_index)
                    targetCard = target.cardinality
                    assert isinstance(targetCard, M_Cardinality)
                    targetLower = targetCard.lower
                    targetUpper = targetCard.upper
                    targetParentType = print_csharp_type(target.parent)
                    targetName = target.name
                    targetType = print_csharp_type(target.type)
                    out.write("// two way association: " +c.name +"("+itemType + " "+ itemName + get_arity(l,h)+")")
                    out.write(" <---> "+ targetParentType + "(" + targetType + " " + targetName + " " + get_arity(targetLower, targetUpper)+ ")").nl()
                    out.write("// In "+ targetParentType + " get " + targetName);
                    if targetUpper == 1:
                        out.write(" use it like ")
                    else:
                        out.write(" for all elements used like ")
                    out.write(targetType + ", put THIS into its List " + itemName ).nl()
                    # working with the same type
                    assert targetParentType == itemType
                    #all not set are lists
                    assert h < 0
                    postProcessing.append((targetParentType, targetName , targetUpper, targetType, itemName))
                out.nl()

        for  xmlName in requiredXmlNames + optionalXmlNames + elementXmlNames:
            #TODO: Solve known attributes
            if xmlName not in processedXML + [ "Extension", "extensionElements", "id", "any", "documentation"]:
                raise Exception("Error: not attached: "+ xmlName)
        out.write("return result;").nl()
        out.dec()
        out.write("}").nl().nl()
    return postProcessing

def remove_trailing_s(string):
    if string == "DiagramElement":
        return "planeElement"
    if string == "calledElementRef":
        return "calledElement"
    if string.startswith('BPMN'):
        return string[4:]
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

def print_class_or_data_type(out, c, mappings):
    assert isinstance(c, M_Class) | isinstance(c, M_DataType)
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
        out.write("interface " + c.name + " : ITraversableNode")
        if c.is_abstract:
            out.write (" // abstract ")
    else:
        if c.is_abstract:
            out.write ("abstract ")
        out.write("class " + c.name)

    parentClass = None
    implementedInterface = None

    if isinstance(c, M_Class): 
        scls = c.superclasses
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
        if c.name == "BaseElement":
            out.write(" : CamundaExtensionBaseElement, IElementWithId, ITraversableNode")            
        elif c.name in ["Diagram", "DiagramElement"]:
            out.write(" : IElementWithId, ITraversableNode")
        elif len(scls)==0 and not isInterface:
            out.write(" : ITraversableNode")

    out.nl()
    out.write("{").nl()
    out.inc()

    for attr in c.attributes:
        print_Attribute(out, attr,isInterface)
    out.nl()

    if implementedInterface != None:
        out.nl();
        out.write("#region Implementing: "+implementedInterface.name).nl()
        for attr in implementedInterface.attributes:
            print_Attribute(out, attr,False)
        out.write("#endregion").nl()
        out.nl();
    
    if c.name in camundaAttributes:
        out.write("#region Camunda attributes").nl()
        for (camundaType, camundaName) in camundaAttributes[c.name]:
            out.write("public "+print_csharp_type(camundaType)+"? " + capitalize_first_letter(camundaName) + " { get; set; }").nl()
        out.write("#endregion").nl()
        out.nl();

    if not isInterface:
        out.write("public "+c.name+"()").nl()
        out.write("{").nl()
        out.write("}").nl().nl()
    # Get child nodes method
    if isinstance(c, M_Class) and not isInterface :
        if c.name in ["BaseElement", "Diagram", "DiagramElement"] or len(c.superclasses) == 0:
            out.write("virtual public  List<ITraversableNode> GetChildElements()").nl()
            out.write("{").nl()
            out.inc()
            out.write("var result = new List<ITraversableNode>();").nl()
        else:
            out.write("override public  List<ITraversableNode> GetChildElements()").nl()
            out.write("{").nl()
            out.inc()
            out.write("var result = base.GetChildElements();").nl()
        
        #for attr in c.attributes:

        for attr in c.attributes:
            print_visit_for_attribute(out, attr)
        if implementedInterface != None:
            out.write("// Implementing: "+implementedInterface.name).nl()
            for attr in implementedInterface.attributes:
                print_visit_for_attribute(out, attr)

        if c.name in camundaAttributes:
            out.write("// Camunda attributes").nl()
            for (camundaType, camundaName) in camundaAttributes[c.name]:
                if isComplexType(camundaType):
                    out.write("// TODO - wrong for: ").write(camundaName).nl()

        out.write("return result;").nl()
        out.dec()
        out.write("}").nl()

    else:
        out.write("/* No method to get child elements - it is interface or data class. */").nl()

    # Accept method
    if isinstance(c, M_Class) and not isInterface :
        if c.name in ["BaseElement", "Diagram", "DiagramElement"] or len(c.superclasses) == 0:
            out.write("public virtual TResult? Accept<TResult>(IBaseVisitor<TResult> visitor) where TResult : class").nl()
            out.write("{").nl()
        else:
            out.write("public override TResult? Accept<TResult>(IBaseVisitor<TResult> visitor) where TResult : class").nl()
            out.write("{").nl()
        
        out.inc()
        out.write("IModelVisitor<TResult>? typedVisitor = visitor as IModelVisitor<TResult>;").nl()
        out.write("if (typedVisitor != null) return typedVisitor.Visit"+c.name+"(this);").nl()
        out.write("else return visitor.VisitOnceChildren(this);").nl()
        out.dec()
        out.write("}").nl()

    else:
        out.write("/* No Accept method - it is interface or data class. */").nl()

    out.dec()

    out.write("}").nl()

def get_all_parents_names(referencedObject):
    if (isinstance(referencedObject, M_HRef_Type)) : 
        currentClass = referencedObject.type
        assert isinstance(currentClass, M_Class)
    else:
        currentClass = referencedObject
        assert isinstance(currentClass, M_Class)

    result = []
    if currentClass not in interfaces:
        parentClasses = [sc for sc in currentClass.superclasses]
        for parentClass in parentClasses:
            result += get_all_parents_names(parentClass)
    result.append(currentClass.name)
    return result

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


def get_all_attributes_names(referencedObject):

    if (isinstance(referencedObject, M_HRef_Type)) : 
        currentClass = referencedObject.type
        assert isinstance(currentClass, M_Class)
    else:
        currentClass = referencedObject
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

    if currentClass.name in camundaAttributes:
        for (_, camundaName) in camundaAttributes[currentClass.name]:
            result.append(camundaName)
    return result

def get_all_attributes(referencedObject):

    if (isinstance(referencedObject, M_HRef_Type)) : 
        currentClass = referencedObject.type
        assert isinstance(currentClass, M_Class)
    else:
        currentClass = referencedObject
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

    if currentClass.name in camundaAttributes:
        for (camundaType, camundaName) in camundaAttributes[currentClass.name]:
            result[camundaName] = (print_csharp_type(camundaType), 0, 1, None) 
    return result

def get_all_attributes_for_data_type(referencedObject):
   
    assert isinstance(referencedObject, M_DataType)
    result = {}

    for attr in referencedObject.attributes:
        if attr.name not in result:
            card = attr.cardinality
            assert isinstance(card, M_Cardinality)
            lower = card.lower
            upper = card.upper
            result[attr.name] = (print_csharp_type(attr.type), lower, upper, attr)

    return result

def get_all_attributes_with_Xml_names(classOrDataType, mappings):
    assert isinstance(classOrDataType, M_Class) | isinstance(classOrDataType, M_DataType)
    result = {}

    if isinstance(classOrDataType, M_Class):
        attributes = get_all_attributes(classOrDataType)
    else:
        attributes = get_all_attributes_for_data_type(classOrDataType)

    fromCmofToXml = None
    if classOrDataType.name in mappings:    
        (_, fromCmofToXml) = mappings[classOrDataType.name]

    for (attName,(attType, l, u, attr)) in attributes.items():
        xmlName = None
        if fromCmofToXml is not None and attName in fromCmofToXml:
            xmlName = fromCmofToXml[attName]
        result[attName.replace(":", "_")] = (attType,xmlName, l, u, attr)
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
    if isinstance(type, M_Type):
        convert = type.name
    else: 
        convert = type
    if convert == "String":
        return "string"
    if convert == "Boolean":
        return "bool"
    if convert == "Integer":
        return "long"
    if convert == "Real":
        return "double"
    return convert


def isComplexType(type):
    if isinstance(type, M_Type):
        convert = type.name
    else: 
        convert = type

    if convert == "String" or convert == "Boolean" or convert == "Integer" or convert == "Real":
        return False
    if convert in enumNames:
        return False
    if convert in dataClassesNames:
        return False
    return True

def print_visit_for_attribute(out, c):
    realName = capitalize_first_letter(c.name)
    card = c.cardinality
    assert isinstance(card, M_Cardinality)
    lower = card.lower
    upper = card.upper
    if isComplexType(c.type):
        if upper == 1:
            out.write("if ("+realName+" is not null) result.Add("+ realName + ");")
        else:
            out.write("result.AddRange("+realName + ");")
        out.nl()


def print_Attribute(out, c, isInInterface):
    #TODO Override for diagrams.
    #if c.name == "diagrams" and print_csharp_type(c.type) == "BPMNDiagram":
    #    return []

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

