using Utility;

namespace BPMNModel.Model
{
    public partial class Factory
    {
        #region Factories

        private Interface LoadInterface(XmlParserComplexNode node)
        {
             var result = GetOrCreate<Interface>(node);

            // required: name -> string name (1, 1)
            var _nameAttribute = node.Attributes["name"]?.ProcessedValue;
            if (_nameAttribute is null) throw new BPMNCheckerExceptions($"Node {node.ID} ({(string.IsNullOrWhiteSpace(node.Type?.Name) ? node.Type?.Name : "")}) is missing required attribute name");
            result.Name = (string)_nameAttribute;

            // optional: id -> string id (1, 1)
            var _idAttribute = node.Attributes.ContainsKey("id") ? node.Attributes["id"].ProcessedValue : null;
            if (_idAttribute is not null) result.Id = (string)_idAttribute;

            // optional: implementationRef -> Element implementationRef (0, 1)
            var _implementationRefAttribute = node.Attributes.ContainsKey("implementationRef") ? node.Attributes["implementationRef"].ProcessedValue : null;
            if (_implementationRefAttribute is not null) result.ImplementationRef = Load<Element>((XmlParserComplexNode)_implementationRefAttribute);

            // element: documentation -> Documentation documentation (0, *)
            FillElements(node.ChildNodes["documentation"], result.Documentation);

            // element: operation -> Operation operations (1, *)
            FillElements(node.ChildNodes["operation"], result.Operations);

            return result;
        }


        private Operation LoadOperation(XmlParserComplexNode node)
        {
             var result = GetOrCreate<Operation>(node);

            // required: name -> string name (1, 1)
            var _nameAttribute = node.Attributes["name"]?.ProcessedValue;
            if (_nameAttribute is null) throw new BPMNCheckerExceptions($"Node {node.ID} ({(string.IsNullOrWhiteSpace(node.Type?.Name) ? node.Type?.Name : "")}) is missing required attribute name");
            result.Name = (string)_nameAttribute;

            // optional: id -> string id (1, 1)
            var _idAttribute = node.Attributes.ContainsKey("id") ? node.Attributes["id"].ProcessedValue : null;
            if (_idAttribute is not null) result.Id = (string)_idAttribute;

            // optional: implementationRef -> Element implementationRef (0, 1)
            var _implementationRefAttribute = node.Attributes.ContainsKey("implementationRef") ? node.Attributes["implementationRef"].ProcessedValue : null;
            if (_implementationRefAttribute is not null) result.ImplementationRef = Load<Element>((XmlParserComplexNode)_implementationRefAttribute);

            // element: documentation -> Documentation documentation (0, *)
            FillElements(node.ChildNodes["documentation"], result.Documentation);

            // element: inMessageRef -> Message inMessageRef (1, 1)
            result.InMessageRef = FillElement<Message>(node.ChildNodes["inMessageRef"]);

            // element: outMessageRef -> Message outMessageRef (0, 1)
            result.OutMessageRef = FillElement<Message>(node.ChildNodes["outMessageRef"]);

            // element: errorRef -> Error errorRefs (0, *)
            FillElements(node.ChildNodes["errorRef"], result.ErrorRefs);

            return result;
        }


        private EndPoint LoadEndPoint(XmlParserComplexNode node)
        {
             var result = GetOrCreate<EndPoint>(node);

            // optional: id -> string id (1, 1)
            var _idAttribute = node.Attributes.ContainsKey("id") ? node.Attributes["id"].ProcessedValue : null;
            if (_idAttribute is not null) result.Id = (string)_idAttribute;

            // element: documentation -> Documentation documentation (0, *)
            FillElements(node.ChildNodes["documentation"], result.Documentation);

            return result;
        }


        private Auditing LoadAuditing(XmlParserComplexNode node)
        {
             var result = GetOrCreate<Auditing>(node);

            // optional: id -> string id (1, 1)
            var _idAttribute = node.Attributes.ContainsKey("id") ? node.Attributes["id"].ProcessedValue : null;
            if (_idAttribute is not null) result.Id = (string)_idAttribute;

            // element: documentation -> Documentation documentation (0, *)
            FillElements(node.ChildNodes["documentation"], result.Documentation);

            return result;
        }


        private GlobalTask LoadGlobalTask(XmlParserComplexNode node)
        {
             var result = GetOrCreate<GlobalTask>(node);

            // optional: id -> string id (1, 1)
            var _idAttribute = node.Attributes.ContainsKey("id") ? node.Attributes["id"].ProcessedValue : null;
            if (_idAttribute is not null) result.Id = (string)_idAttribute;

            // optional: name -> string name (1, 1)
            var _nameAttribute = node.Attributes.ContainsKey("name") ? node.Attributes["name"].ProcessedValue : null;
            if (_nameAttribute is not null) result.Name = (string)_nameAttribute;

            // element: documentation -> Documentation documentation (0, *)
            FillElements(node.ChildNodes["documentation"], result.Documentation);

            // element: ioSpecification -> InputOutputSpecification ioSpecification (0, 1)
            result.IoSpecification = FillElement<InputOutputSpecification>(node.ChildNodes["ioSpecification"]);

            // element: supportedInterfaceRef -> Interface supportedInterfaceRefs (0, *)
            FillElements(node.ChildNodes["supportedInterfaceRef"], result.SupportedInterfaceRefs);

            // element: ioBinding -> InputOutputBinding ioBinding (0, *)
            FillElements(node.ChildNodes["ioBinding"], result.IoBinding);

            // element: resourceRole -> ResourceRole resources (0, *)
            FillElements(node.ChildNodes["resourceRole"], result.Resources);

            return result;
        }


        private Monitoring LoadMonitoring(XmlParserComplexNode node)
        {
             var result = GetOrCreate<Monitoring>(node);

            // optional: id -> string id (1, 1)
            var _idAttribute = node.Attributes.ContainsKey("id") ? node.Attributes["id"].ProcessedValue : null;
            if (_idAttribute is not null) result.Id = (string)_idAttribute;

            // element: documentation -> Documentation documentation (0, *)
            FillElements(node.ChildNodes["documentation"], result.Documentation);

            return result;
        }


        private Performer LoadPerformer(XmlParserComplexNode node)
        {
             var result = GetOrCreate<Performer>(node);

            // optional: id -> string id (1, 1)
            var _idAttribute = node.Attributes.ContainsKey("id") ? node.Attributes["id"].ProcessedValue : null;
            if (_idAttribute is not null) result.Id = (string)_idAttribute;

            // optional: name -> string name (1, 1)
            var _nameAttribute = node.Attributes.ContainsKey("name") ? node.Attributes["name"].ProcessedValue : null;
            if (_nameAttribute is not null) result.Name = (string)_nameAttribute;

            // element: documentation -> Documentation documentation (0, *)
            FillElements(node.ChildNodes["documentation"], result.Documentation);

            // element: resourceRef -> Resource resourceRef (0, 1)
            result.ResourceRef = FillElement<Resource>(node.ChildNodes["resourceRef"]);

            // element: resourceParameterBinding -> ResourceParameterBinding resourceParameterBindings (0, *)
            FillElements(node.ChildNodes["resourceParameterBinding"], result.ResourceParameterBindings);

            // element: resourceAssignmentExpression -> ResourceAssignmentExpression resourceAssignmentExpression (0, 1)
            result.ResourceAssignmentExpression = FillElement<ResourceAssignmentExpression>(node.ChildNodes["resourceAssignmentExpression"]);

            return result;
        }


        private Process LoadProcess(XmlParserComplexNode node)
        {
             var result = GetOrCreate<Process>(node);

            // optional: id -> string id (1, 1)
            var _idAttribute = node.Attributes.ContainsKey("id") ? node.Attributes["id"].ProcessedValue : null;
            if (_idAttribute is not null) result.Id = (string)_idAttribute;

            // optional: name -> string name (1, 1)
            var _nameAttribute = node.Attributes.ContainsKey("name") ? node.Attributes["name"].ProcessedValue : null;
            if (_nameAttribute is not null) result.Name = (string)_nameAttribute;

            // optional: processType -> ProcessType processType (1, 1)
            var _processTypeAttribute = node.Attributes.ContainsKey("processType") ? node.Attributes["processType"].ProcessedValue : null;
            if (_processTypeAttribute is not null) result.ProcessType = CreateEnum<ProcessType>((string)_processTypeAttribute);

            // optional: isClosed -> bool isClosed (1, 1)
            var _isClosedAttribute = node.Attributes.ContainsKey("isClosed") ? node.Attributes["isClosed"].ProcessedValue : null;
            if (_isClosedAttribute is not null) result.IsClosed = (bool)_isClosedAttribute;

            // optional: definitionalCollaborationRef -> Collaboration definitionalCollaborationRef (0, 1)
            var _definitionalCollaborationRefAttribute = node.Attributes.ContainsKey("definitionalCollaborationRef") ? node.Attributes["definitionalCollaborationRef"].ProcessedValue : null;
            if (_definitionalCollaborationRefAttribute is not null) result.DefinitionalCollaborationRef = Load<Collaboration>((XmlParserComplexNode)_definitionalCollaborationRefAttribute);

            // optional: isExecutable -> bool isExecutable (1, 1)
            var _isExecutableAttribute = node.Attributes.ContainsKey("isExecutable") ? node.Attributes["isExecutable"].ProcessedValue : null;
            if (_isExecutableAttribute is not null) result.IsExecutable = (bool)_isExecutableAttribute;

            // element: documentation -> Documentation documentation (0, *)
            FillElements(node.ChildNodes["documentation"], result.Documentation);

            // element: flowElement -> FlowElement flowElements (0, *)
            FillElements(node.ChildNodes["flowElement"], result.FlowElements);

            // element: laneSet -> LaneSet laneSets (0, *)
            FillElements(node.ChildNodes["laneSet"], result.LaneSets);

            // element: ioSpecification -> InputOutputSpecification ioSpecification (0, 1)
            result.IoSpecification = FillElement<InputOutputSpecification>(node.ChildNodes["ioSpecification"]);

            // element: supportedInterfaceRef -> Interface supportedInterfaceRefs (0, *)
            FillElements(node.ChildNodes["supportedInterfaceRef"], result.SupportedInterfaceRefs);

            // element: ioBinding -> InputOutputBinding ioBinding (0, *)
            FillElements(node.ChildNodes["ioBinding"], result.IoBinding);

            // element: auditing -> Auditing auditing (0, 1)
            result.Auditing = FillElement<Auditing>(node.ChildNodes["auditing"]);

            // element: monitoring -> Monitoring monitoring (0, 1)
            result.Monitoring = FillElement<Monitoring>(node.ChildNodes["monitoring"]);

            // element: property -> Property properties (0, *)
            FillElements(node.ChildNodes["property"], result.Properties);

            // element: supports -> Process supports (0, *)
            FillElements(node.ChildNodes["supports"], result.Supports);

            // element: resourceRole -> ResourceRole resources (0, *)
            FillElements(node.ChildNodes["resourceRole"], result.Resources);

            // element: artifact -> Artifact artifacts (0, *)
            FillElements(node.ChildNodes["artifact"], result.Artifacts);

            // element: correlationSubscription -> CorrelationSubscription correlationSubscriptions (0, *)
            FillElements(node.ChildNodes["correlationSubscription"], result.CorrelationSubscriptions);

            return result;
        }


        private LaneSet LoadLaneSet(XmlParserComplexNode node)
        {
             var result = GetOrCreate<LaneSet>(node);

            // optional: id -> string id (1, 1)
            var _idAttribute = node.Attributes.ContainsKey("id") ? node.Attributes["id"].ProcessedValue : null;
            if (_idAttribute is not null) result.Id = (string)_idAttribute;

            // optional: name -> string name (0, 1)
            var _nameAttribute = node.Attributes.ContainsKey("name") ? node.Attributes["name"].ProcessedValue : null;
            if (_nameAttribute is not null) result.Name = (string)_nameAttribute;

            // element: documentation -> Documentation documentation (0, *)
            FillElements(node.ChildNodes["documentation"], result.Documentation);

            // element: lane -> Lane lanes (0, *)
            FillElements(node.ChildNodes["lane"], result.Lanes);

            return result;
        }


        private Lane LoadLane(XmlParserComplexNode node)
        {
             var result = GetOrCreate<Lane>(node);

            // optional: id -> string id (1, 1)
            var _idAttribute = node.Attributes.ContainsKey("id") ? node.Attributes["id"].ProcessedValue : null;
            if (_idAttribute is not null) result.Id = (string)_idAttribute;

            // optional: name -> string name (1, 1)
            var _nameAttribute = node.Attributes.ContainsKey("name") ? node.Attributes["name"].ProcessedValue : null;
            if (_nameAttribute is not null) result.Name = (string)_nameAttribute;

            // optional: partitionElementRef -> BaseElement partitionElementRef (0, 1)
            var _partitionElementRefAttribute = node.Attributes.ContainsKey("partitionElementRef") ? node.Attributes["partitionElementRef"].ProcessedValue : null;
            if (_partitionElementRefAttribute is not null) result.PartitionElementRef = Load<BaseElement>((XmlParserComplexNode)_partitionElementRefAttribute);

            // element: documentation -> Documentation documentation (0, *)
            FillElements(node.ChildNodes["documentation"], result.Documentation);

            // element: childLaneSet -> LaneSet childLaneSet (0, 1)
            result.ChildLaneSet = FillElement<LaneSet>(node.ChildNodes["childLaneSet"]);

            // element: flowNodeRef -> FlowNode flowNodeRefs (0, *)
            FillElements(node.ChildNodes["flowNodeRef"], result.FlowNodeRefs);

            // element: partitionElement -> BaseElement partitionElement (0, 1)
            result.PartitionElement = FillElement<BaseElement>(node.ChildNodes["partitionElement"]);

            return result;
        }


        private GlobalManualTask LoadGlobalManualTask(XmlParserComplexNode node)
        {
             var result = GetOrCreate<GlobalManualTask>(node);

            // optional: id -> string id (1, 1)
            var _idAttribute = node.Attributes.ContainsKey("id") ? node.Attributes["id"].ProcessedValue : null;
            if (_idAttribute is not null) result.Id = (string)_idAttribute;

            // optional: name -> string name (1, 1)
            var _nameAttribute = node.Attributes.ContainsKey("name") ? node.Attributes["name"].ProcessedValue : null;
            if (_nameAttribute is not null) result.Name = (string)_nameAttribute;

            // element: documentation -> Documentation documentation (0, *)
            FillElements(node.ChildNodes["documentation"], result.Documentation);

            // element: ioSpecification -> InputOutputSpecification ioSpecification (0, 1)
            result.IoSpecification = FillElement<InputOutputSpecification>(node.ChildNodes["ioSpecification"]);

            // element: supportedInterfaceRef -> Interface supportedInterfaceRefs (0, *)
            FillElements(node.ChildNodes["supportedInterfaceRef"], result.SupportedInterfaceRefs);

            // element: ioBinding -> InputOutputBinding ioBinding (0, *)
            FillElements(node.ChildNodes["ioBinding"], result.IoBinding);

            // element: resourceRole -> ResourceRole resources (0, *)
            FillElements(node.ChildNodes["resourceRole"], result.Resources);

            return result;
        }


        private ManualTask LoadManualTask(XmlParserComplexNode node)
        {
             var result = GetOrCreate<ManualTask>(node);

            // optional: id -> string id (1, 1)
            var _idAttribute = node.Attributes.ContainsKey("id") ? node.Attributes["id"].ProcessedValue : null;
            if (_idAttribute is not null) result.Id = (string)_idAttribute;

            // optional: name -> string name (1, 1)
            var _nameAttribute = node.Attributes.ContainsKey("name") ? node.Attributes["name"].ProcessedValue : null;
            if (_nameAttribute is not null) result.Name = (string)_nameAttribute;

            // optional: isForCompensation -> bool isForCompensation (1, 1)
            var _isForCompensationAttribute = node.Attributes.ContainsKey("isForCompensation") ? node.Attributes["isForCompensation"].ProcessedValue : null;
            if (_isForCompensationAttribute is not null) result.IsForCompensation = (bool)_isForCompensationAttribute;

            // optional: default -> SequenceFlow default (0, 1)
            var _defaultAttribute = node.Attributes.ContainsKey("default") ? node.Attributes["default"].ProcessedValue : null;
            if (_defaultAttribute is not null) result.Default = Load<SequenceFlow>((XmlParserComplexNode)_defaultAttribute);

            // optional: startQuantity -> int startQuantity (1, 1)
            var _startQuantityAttribute = node.Attributes.ContainsKey("startQuantity") ? node.Attributes["startQuantity"].ProcessedValue : null;
            if (_startQuantityAttribute is not null) result.StartQuantity = (int)_startQuantityAttribute;

            // optional: completionQuantity -> int completionQuantity (1, 1)
            var _completionQuantityAttribute = node.Attributes.ContainsKey("completionQuantity") ? node.Attributes["completionQuantity"].ProcessedValue : null;
            if (_completionQuantityAttribute is not null) result.CompletionQuantity = (int)_completionQuantityAttribute;

            // element: documentation -> Documentation documentation (0, *)
            FillElements(node.ChildNodes["documentation"], result.Documentation);

            // element: auditing -> Auditing auditing (0, 1)
            result.Auditing = FillElement<Auditing>(node.ChildNodes["auditing"]);

            // element: monitoring -> Monitoring monitoring (0, 1)
            result.Monitoring = FillElement<Monitoring>(node.ChildNodes["monitoring"]);

            // element: categoryValueRef -> CategoryValue categoryValueRef (0, *)
            FillElements(node.ChildNodes["categoryValueRef"], result.CategoryValueRef);

            // element: outgoing -> SequenceFlow outgoing (0, *)
            FillElements(node.ChildNodes["outgoing"], result.Outgoing);

            // element: incoming -> SequenceFlow incoming (0, *)
            FillElements(node.ChildNodes["incoming"], result.Incoming);

            // element: loopCharacteristics -> LoopCharacteristics loopCharacteristics (0, 1)
            result.LoopCharacteristics = FillElement<LoopCharacteristics>(node.ChildNodes["loopCharacteristics"]);

            // element: resourceRole -> ResourceRole resources (0, *)
            FillElements(node.ChildNodes["resourceRole"], result.Resources);

            // element: property -> Property properties (0, *)
            FillElements(node.ChildNodes["property"], result.Properties);

            // element: ioSpecification -> InputOutputSpecification ioSpecification (0, 1)
            result.IoSpecification = FillElement<InputOutputSpecification>(node.ChildNodes["ioSpecification"]);

            // element: dataInputAssociation -> DataInputAssociation dataInputAssociations (0, *)
            FillElements(node.ChildNodes["dataInputAssociation"], result.DataInputAssociations);

            // element: dataOutputAssociation -> DataOutputAssociation dataOutputAssociations (0, *)
            FillElements(node.ChildNodes["dataOutputAssociation"], result.DataOutputAssociations);

            // link back: Lane lanes (0, *)
            // lanes 1 <cmof_model.M_Two_Way_Association object at 0x000002941E4AD800>
            // target: Lane.flowNodeRefs

            // link back: BoundaryEvent boundaryEventRefs (0, *)
            // boundaryEventRefs 0 <cmof_model.M_Two_Way_Association object at 0x000002941E4ADF40>
            // target: BoundaryEvent.attachedToRef

            // link back: ConversationLink incomingConversationLinks (0, *)
            // incomingConversationLinks 1 <cmof_model.M_Two_Way_Association object at 0x000002941E4ADD40>
            // target: ConversationLink.targetRef

            // link back: ConversationLink outgoingConversationLinks (0, *)
            // outgoingConversationLinks 1 <cmof_model.M_Two_Way_Association object at 0x000002941E4ADCC0>
            // target: ConversationLink.sourceRef

            return result;
        }


        private UserTask LoadUserTask(XmlParserComplexNode node)
        {
             var result = GetOrCreate<UserTask>(node);

            // optional: id -> string id (1, 1)
            var _idAttribute = node.Attributes.ContainsKey("id") ? node.Attributes["id"].ProcessedValue : null;
            if (_idAttribute is not null) result.Id = (string)_idAttribute;

            // optional: name -> string name (1, 1)
            var _nameAttribute = node.Attributes.ContainsKey("name") ? node.Attributes["name"].ProcessedValue : null;
            if (_nameAttribute is not null) result.Name = (string)_nameAttribute;

            // optional: isForCompensation -> bool isForCompensation (1, 1)
            var _isForCompensationAttribute = node.Attributes.ContainsKey("isForCompensation") ? node.Attributes["isForCompensation"].ProcessedValue : null;
            if (_isForCompensationAttribute is not null) result.IsForCompensation = (bool)_isForCompensationAttribute;

            // optional: default -> SequenceFlow default (0, 1)
            var _defaultAttribute = node.Attributes.ContainsKey("default") ? node.Attributes["default"].ProcessedValue : null;
            if (_defaultAttribute is not null) result.Default = Load<SequenceFlow>((XmlParserComplexNode)_defaultAttribute);

            // optional: startQuantity -> int startQuantity (1, 1)
            var _startQuantityAttribute = node.Attributes.ContainsKey("startQuantity") ? node.Attributes["startQuantity"].ProcessedValue : null;
            if (_startQuantityAttribute is not null) result.StartQuantity = (int)_startQuantityAttribute;

            // optional: completionQuantity -> int completionQuantity (1, 1)
            var _completionQuantityAttribute = node.Attributes.ContainsKey("completionQuantity") ? node.Attributes["completionQuantity"].ProcessedValue : null;
            if (_completionQuantityAttribute is not null) result.CompletionQuantity = (int)_completionQuantityAttribute;

            // optional: implementation -> string implementation (1, 1)
            var _implementationAttribute = node.Attributes.ContainsKey("implementation") ? node.Attributes["implementation"].ProcessedValue : null;
            if (_implementationAttribute is not null) result.Implementation = (string)_implementationAttribute;

            // element: documentation -> Documentation documentation (0, *)
            FillElements(node.ChildNodes["documentation"], result.Documentation);

            // element: auditing -> Auditing auditing (0, 1)
            result.Auditing = FillElement<Auditing>(node.ChildNodes["auditing"]);

            // element: monitoring -> Monitoring monitoring (0, 1)
            result.Monitoring = FillElement<Monitoring>(node.ChildNodes["monitoring"]);

            // element: categoryValueRef -> CategoryValue categoryValueRef (0, *)
            FillElements(node.ChildNodes["categoryValueRef"], result.CategoryValueRef);

            // element: outgoing -> SequenceFlow outgoing (0, *)
            FillElements(node.ChildNodes["outgoing"], result.Outgoing);

            // element: incoming -> SequenceFlow incoming (0, *)
            FillElements(node.ChildNodes["incoming"], result.Incoming);

            // element: loopCharacteristics -> LoopCharacteristics loopCharacteristics (0, 1)
            result.LoopCharacteristics = FillElement<LoopCharacteristics>(node.ChildNodes["loopCharacteristics"]);

            // element: resourceRole -> ResourceRole resources (0, *)
            FillElements(node.ChildNodes["resourceRole"], result.Resources);

            // element: property -> Property properties (0, *)
            FillElements(node.ChildNodes["property"], result.Properties);

            // element: ioSpecification -> InputOutputSpecification ioSpecification (0, 1)
            result.IoSpecification = FillElement<InputOutputSpecification>(node.ChildNodes["ioSpecification"]);

            // element: dataInputAssociation -> DataInputAssociation dataInputAssociations (0, *)
            FillElements(node.ChildNodes["dataInputAssociation"], result.DataInputAssociations);

            // element: dataOutputAssociation -> DataOutputAssociation dataOutputAssociations (0, *)
            FillElements(node.ChildNodes["dataOutputAssociation"], result.DataOutputAssociations);

            // element: rendering -> Rendering renderings (0, *)
            FillElements(node.ChildNodes["rendering"], result.Renderings);

            // link back: Lane lanes (0, *)
            // lanes 1 <cmof_model.M_Two_Way_Association object at 0x000002941E4AD800>
            // target: Lane.flowNodeRefs

            // link back: BoundaryEvent boundaryEventRefs (0, *)
            // boundaryEventRefs 0 <cmof_model.M_Two_Way_Association object at 0x000002941E4ADF40>
            // target: BoundaryEvent.attachedToRef

            // link back: ConversationLink incomingConversationLinks (0, *)
            // incomingConversationLinks 1 <cmof_model.M_Two_Way_Association object at 0x000002941E4ADD40>
            // target: ConversationLink.targetRef

            // link back: ConversationLink outgoingConversationLinks (0, *)
            // outgoingConversationLinks 1 <cmof_model.M_Two_Way_Association object at 0x000002941E4ADCC0>
            // target: ConversationLink.sourceRef

            return result;
        }


        private Rendering LoadRendering(XmlParserComplexNode node)
        {
             var result = GetOrCreate<Rendering>(node);

            // optional: id -> string id (1, 1)
            var _idAttribute = node.Attributes.ContainsKey("id") ? node.Attributes["id"].ProcessedValue : null;
            if (_idAttribute is not null) result.Id = (string)_idAttribute;

            // element: documentation -> Documentation documentation (0, *)
            FillElements(node.ChildNodes["documentation"], result.Documentation);

            return result;
        }


        private HumanPerformer LoadHumanPerformer(XmlParserComplexNode node)
        {
             var result = GetOrCreate<HumanPerformer>(node);

            // optional: id -> string id (1, 1)
            var _idAttribute = node.Attributes.ContainsKey("id") ? node.Attributes["id"].ProcessedValue : null;
            if (_idAttribute is not null) result.Id = (string)_idAttribute;

            // optional: name -> string name (1, 1)
            var _nameAttribute = node.Attributes.ContainsKey("name") ? node.Attributes["name"].ProcessedValue : null;
            if (_nameAttribute is not null) result.Name = (string)_nameAttribute;

            // element: documentation -> Documentation documentation (0, *)
            FillElements(node.ChildNodes["documentation"], result.Documentation);

            // element: resourceRef -> Resource resourceRef (0, 1)
            result.ResourceRef = FillElement<Resource>(node.ChildNodes["resourceRef"]);

            // element: resourceParameterBinding -> ResourceParameterBinding resourceParameterBindings (0, *)
            FillElements(node.ChildNodes["resourceParameterBinding"], result.ResourceParameterBindings);

            // element: resourceAssignmentExpression -> ResourceAssignmentExpression resourceAssignmentExpression (0, 1)
            result.ResourceAssignmentExpression = FillElement<ResourceAssignmentExpression>(node.ChildNodes["resourceAssignmentExpression"]);

            return result;
        }


        private PotentialOwner LoadPotentialOwner(XmlParserComplexNode node)
        {
             var result = GetOrCreate<PotentialOwner>(node);

            // optional: id -> string id (1, 1)
            var _idAttribute = node.Attributes.ContainsKey("id") ? node.Attributes["id"].ProcessedValue : null;
            if (_idAttribute is not null) result.Id = (string)_idAttribute;

            // optional: name -> string name (1, 1)
            var _nameAttribute = node.Attributes.ContainsKey("name") ? node.Attributes["name"].ProcessedValue : null;
            if (_nameAttribute is not null) result.Name = (string)_nameAttribute;

            // element: documentation -> Documentation documentation (0, *)
            FillElements(node.ChildNodes["documentation"], result.Documentation);

            // element: resourceRef -> Resource resourceRef (0, 1)
            result.ResourceRef = FillElement<Resource>(node.ChildNodes["resourceRef"]);

            // element: resourceParameterBinding -> ResourceParameterBinding resourceParameterBindings (0, *)
            FillElements(node.ChildNodes["resourceParameterBinding"], result.ResourceParameterBindings);

            // element: resourceAssignmentExpression -> ResourceAssignmentExpression resourceAssignmentExpression (0, 1)
            result.ResourceAssignmentExpression = FillElement<ResourceAssignmentExpression>(node.ChildNodes["resourceAssignmentExpression"]);

            return result;
        }


        private GlobalUserTask LoadGlobalUserTask(XmlParserComplexNode node)
        {
             var result = GetOrCreate<GlobalUserTask>(node);

            // optional: id -> string id (1, 1)
            var _idAttribute = node.Attributes.ContainsKey("id") ? node.Attributes["id"].ProcessedValue : null;
            if (_idAttribute is not null) result.Id = (string)_idAttribute;

            // optional: name -> string name (1, 1)
            var _nameAttribute = node.Attributes.ContainsKey("name") ? node.Attributes["name"].ProcessedValue : null;
            if (_nameAttribute is not null) result.Name = (string)_nameAttribute;

            // optional: implementation -> string implementation (1, 1)
            var _implementationAttribute = node.Attributes.ContainsKey("implementation") ? node.Attributes["implementation"].ProcessedValue : null;
            if (_implementationAttribute is not null) result.Implementation = (string)_implementationAttribute;

            // element: documentation -> Documentation documentation (0, *)
            FillElements(node.ChildNodes["documentation"], result.Documentation);

            // element: ioSpecification -> InputOutputSpecification ioSpecification (0, 1)
            result.IoSpecification = FillElement<InputOutputSpecification>(node.ChildNodes["ioSpecification"]);

            // element: supportedInterfaceRef -> Interface supportedInterfaceRefs (0, *)
            FillElements(node.ChildNodes["supportedInterfaceRef"], result.SupportedInterfaceRefs);

            // element: ioBinding -> InputOutputBinding ioBinding (0, *)
            FillElements(node.ChildNodes["ioBinding"], result.IoBinding);

            // element: resourceRole -> ResourceRole resources (0, *)
            FillElements(node.ChildNodes["resourceRole"], result.Resources);

            // element: rendering -> Rendering renderings (0, *)
            FillElements(node.ChildNodes["rendering"], result.Renderings);

            return result;
        }



        private EventBasedGateway LoadEventBasedGateway(XmlParserComplexNode node)
        {
             var result = GetOrCreate<EventBasedGateway>(node);

            // optional: id -> string id (1, 1)
            var _idAttribute = node.Attributes.ContainsKey("id") ? node.Attributes["id"].ProcessedValue : null;
            if (_idAttribute is not null) result.Id = (string)_idAttribute;

            // optional: name -> string name (1, 1)
            var _nameAttribute = node.Attributes.ContainsKey("name") ? node.Attributes["name"].ProcessedValue : null;
            if (_nameAttribute is not null) result.Name = (string)_nameAttribute;

            // optional: gatewayDirection -> GatewayDirection gatewayDirection (1, 1)
            var _gatewayDirectionAttribute = node.Attributes.ContainsKey("gatewayDirection") ? node.Attributes["gatewayDirection"].ProcessedValue : null;
            if (_gatewayDirectionAttribute is not null) result.GatewayDirection = CreateEnum<GatewayDirection>((string)_gatewayDirectionAttribute);

            // optional: instantiate -> bool instantiate (1, 1)
            var _instantiateAttribute = node.Attributes.ContainsKey("instantiate") ? node.Attributes["instantiate"].ProcessedValue : null;
            if (_instantiateAttribute is not null) result.Instantiate = (bool)_instantiateAttribute;

            // optional: eventGatewayType -> EventBasedGatewayType eventGatewayType (1, 1)
            var _eventGatewayTypeAttribute = node.Attributes.ContainsKey("eventGatewayType") ? node.Attributes["eventGatewayType"].ProcessedValue : null;
            if (_eventGatewayTypeAttribute is not null) result.EventGatewayType = CreateEnum<EventBasedGatewayType>((string)_eventGatewayTypeAttribute);

            // element: documentation -> Documentation documentation (0, *)
            FillElements(node.ChildNodes["documentation"], result.Documentation);

            // element: auditing -> Auditing auditing (0, 1)
            result.Auditing = FillElement<Auditing>(node.ChildNodes["auditing"]);

            // element: monitoring -> Monitoring monitoring (0, 1)
            result.Monitoring = FillElement<Monitoring>(node.ChildNodes["monitoring"]);

            // element: categoryValueRef -> CategoryValue categoryValueRef (0, *)
            FillElements(node.ChildNodes["categoryValueRef"], result.CategoryValueRef);

            // element: outgoing -> SequenceFlow outgoing (0, *)
            FillElements(node.ChildNodes["outgoing"], result.Outgoing);

            // element: incoming -> SequenceFlow incoming (0, *)
            FillElements(node.ChildNodes["incoming"], result.Incoming);

            // link back: Lane lanes (0, *)
            // lanes 1 <cmof_model.M_Two_Way_Association object at 0x000002941E4AD800>
            // target: Lane.flowNodeRefs

            return result;
        }


        private ComplexGateway LoadComplexGateway(XmlParserComplexNode node)
        {
             var result = GetOrCreate<ComplexGateway>(node);

            // optional: id -> string id (1, 1)
            var _idAttribute = node.Attributes.ContainsKey("id") ? node.Attributes["id"].ProcessedValue : null;
            if (_idAttribute is not null) result.Id = (string)_idAttribute;

            // optional: name -> string name (1, 1)
            var _nameAttribute = node.Attributes.ContainsKey("name") ? node.Attributes["name"].ProcessedValue : null;
            if (_nameAttribute is not null) result.Name = (string)_nameAttribute;

            // optional: gatewayDirection -> GatewayDirection gatewayDirection (1, 1)
            var _gatewayDirectionAttribute = node.Attributes.ContainsKey("gatewayDirection") ? node.Attributes["gatewayDirection"].ProcessedValue : null;
            if (_gatewayDirectionAttribute is not null) result.GatewayDirection = CreateEnum<GatewayDirection>((string)_gatewayDirectionAttribute);

            // optional: default -> SequenceFlow default (0, 1)
            var _defaultAttribute = node.Attributes.ContainsKey("default") ? node.Attributes["default"].ProcessedValue : null;
            if (_defaultAttribute is not null) result.Default = Load<SequenceFlow>((XmlParserComplexNode)_defaultAttribute);

            // element: documentation -> Documentation documentation (0, *)
            FillElements(node.ChildNodes["documentation"], result.Documentation);

            // element: auditing -> Auditing auditing (0, 1)
            result.Auditing = FillElement<Auditing>(node.ChildNodes["auditing"]);

            // element: monitoring -> Monitoring monitoring (0, 1)
            result.Monitoring = FillElement<Monitoring>(node.ChildNodes["monitoring"]);

            // element: categoryValueRef -> CategoryValue categoryValueRef (0, *)
            FillElements(node.ChildNodes["categoryValueRef"], result.CategoryValueRef);

            // element: outgoing -> SequenceFlow outgoing (0, *)
            FillElements(node.ChildNodes["outgoing"], result.Outgoing);

            // element: incoming -> SequenceFlow incoming (0, *)
            FillElements(node.ChildNodes["incoming"], result.Incoming);

            // element: activationCondition -> Expression activationCondition (0, 1)
            result.ActivationCondition = FillElement<Expression>(node.ChildNodes["activationCondition"]);

            // link back: Lane lanes (0, *)
            // lanes 1 <cmof_model.M_Two_Way_Association object at 0x000002941E4AD800>
            // target: Lane.flowNodeRefs

            return result;
        }


        private ExclusiveGateway LoadExclusiveGateway(XmlParserComplexNode node)
        {
             var result = GetOrCreate<ExclusiveGateway>(node);

            // optional: id -> string id (1, 1)
            var _idAttribute = node.Attributes.ContainsKey("id") ? node.Attributes["id"].ProcessedValue : null;
            if (_idAttribute is not null) result.Id = (string)_idAttribute;

            // optional: name -> string name (1, 1)
            var _nameAttribute = node.Attributes.ContainsKey("name") ? node.Attributes["name"].ProcessedValue : null;
            if (_nameAttribute is not null) result.Name = (string)_nameAttribute;

            // optional: gatewayDirection -> GatewayDirection gatewayDirection (1, 1)
            var _gatewayDirectionAttribute = node.Attributes.ContainsKey("gatewayDirection") ? node.Attributes["gatewayDirection"].ProcessedValue : null;
            if (_gatewayDirectionAttribute is not null) result.GatewayDirection = CreateEnum<GatewayDirection>((string)_gatewayDirectionAttribute);

            // optional: default -> SequenceFlow default (0, 1)
            var _defaultAttribute = node.Attributes.ContainsKey("default") ? node.Attributes["default"].ProcessedValue : null;
            if (_defaultAttribute is not null) result.Default = Load<SequenceFlow>((XmlParserComplexNode)_defaultAttribute);

            // element: documentation -> Documentation documentation (0, *)
            FillElements(node.ChildNodes["documentation"], result.Documentation);

            // element: auditing -> Auditing auditing (0, 1)
            result.Auditing = FillElement<Auditing>(node.ChildNodes["auditing"]);

            // element: monitoring -> Monitoring monitoring (0, 1)
            result.Monitoring = FillElement<Monitoring>(node.ChildNodes["monitoring"]);

            // element: categoryValueRef -> CategoryValue categoryValueRef (0, *)
            FillElements(node.ChildNodes["categoryValueRef"], result.CategoryValueRef);

            // element: outgoing -> SequenceFlow outgoing (0, *)
            FillElements(node.ChildNodes["outgoing"], result.Outgoing);

            // element: incoming -> SequenceFlow incoming (0, *)
            FillElements(node.ChildNodes["incoming"], result.Incoming);

            // link back: Lane lanes (0, *)
            // lanes 1 <cmof_model.M_Two_Way_Association object at 0x000002941E4AD800>
            // target: Lane.flowNodeRefs

            return result;
        }


        private InclusiveGateway LoadInclusiveGateway(XmlParserComplexNode node)
        {
             var result = GetOrCreate<InclusiveGateway>(node);

            // optional: id -> string id (1, 1)
            var _idAttribute = node.Attributes.ContainsKey("id") ? node.Attributes["id"].ProcessedValue : null;
            if (_idAttribute is not null) result.Id = (string)_idAttribute;

            // optional: name -> string name (1, 1)
            var _nameAttribute = node.Attributes.ContainsKey("name") ? node.Attributes["name"].ProcessedValue : null;
            if (_nameAttribute is not null) result.Name = (string)_nameAttribute;

            // optional: gatewayDirection -> GatewayDirection gatewayDirection (1, 1)
            var _gatewayDirectionAttribute = node.Attributes.ContainsKey("gatewayDirection") ? node.Attributes["gatewayDirection"].ProcessedValue : null;
            if (_gatewayDirectionAttribute is not null) result.GatewayDirection = CreateEnum<GatewayDirection>((string)_gatewayDirectionAttribute);

            // optional: default -> SequenceFlow default (0, 1)
            var _defaultAttribute = node.Attributes.ContainsKey("default") ? node.Attributes["default"].ProcessedValue : null;
            if (_defaultAttribute is not null) result.Default = Load<SequenceFlow>((XmlParserComplexNode)_defaultAttribute);

            // element: documentation -> Documentation documentation (0, *)
            FillElements(node.ChildNodes["documentation"], result.Documentation);

            // element: auditing -> Auditing auditing (0, 1)
            result.Auditing = FillElement<Auditing>(node.ChildNodes["auditing"]);

            // element: monitoring -> Monitoring monitoring (0, 1)
            result.Monitoring = FillElement<Monitoring>(node.ChildNodes["monitoring"]);

            // element: categoryValueRef -> CategoryValue categoryValueRef (0, *)
            FillElements(node.ChildNodes["categoryValueRef"], result.CategoryValueRef);

            // element: outgoing -> SequenceFlow outgoing (0, *)
            FillElements(node.ChildNodes["outgoing"], result.Outgoing);

            // element: incoming -> SequenceFlow incoming (0, *)
            FillElements(node.ChildNodes["incoming"], result.Incoming);

            // link back: Lane lanes (0, *)
            // lanes 1 <cmof_model.M_Two_Way_Association object at 0x000002941E4AD800>
            // target: Lane.flowNodeRefs

            return result;
        }


        private ParallelGateway LoadParallelGateway(XmlParserComplexNode node)
        {
             var result = GetOrCreate<ParallelGateway>(node);

            // optional: id -> string id (1, 1)
            var _idAttribute = node.Attributes.ContainsKey("id") ? node.Attributes["id"].ProcessedValue : null;
            if (_idAttribute is not null) result.Id = (string)_idAttribute;

            // optional: name -> string name (1, 1)
            var _nameAttribute = node.Attributes.ContainsKey("name") ? node.Attributes["name"].ProcessedValue : null;
            if (_nameAttribute is not null) result.Name = (string)_nameAttribute;

            // optional: gatewayDirection -> GatewayDirection gatewayDirection (1, 1)
            var _gatewayDirectionAttribute = node.Attributes.ContainsKey("gatewayDirection") ? node.Attributes["gatewayDirection"].ProcessedValue : null;
            if (_gatewayDirectionAttribute is not null) result.GatewayDirection = CreateEnum<GatewayDirection>((string)_gatewayDirectionAttribute);

            // element: documentation -> Documentation documentation (0, *)
            FillElements(node.ChildNodes["documentation"], result.Documentation);

            // element: auditing -> Auditing auditing (0, 1)
            result.Auditing = FillElement<Auditing>(node.ChildNodes["auditing"]);

            // element: monitoring -> Monitoring monitoring (0, 1)
            result.Monitoring = FillElement<Monitoring>(node.ChildNodes["monitoring"]);

            // element: categoryValueRef -> CategoryValue categoryValueRef (0, *)
            FillElements(node.ChildNodes["categoryValueRef"], result.CategoryValueRef);

            // element: outgoing -> SequenceFlow outgoing (0, *)
            FillElements(node.ChildNodes["outgoing"], result.Outgoing);

            // element: incoming -> SequenceFlow incoming (0, *)
            FillElements(node.ChildNodes["incoming"], result.Incoming);

            // link back: Lane lanes (0, *)
            // lanes 1 <cmof_model.M_Two_Way_Association object at 0x000002941E4AD800>
            // target: Lane.flowNodeRefs

            return result;
        }



        private Relationship LoadRelationship(XmlParserComplexNode node)
        {
             var result = GetOrCreate<Relationship>(node);

            // required: type -> string type (1, 1)
            var _typeAttribute = node.Attributes["type"]?.ProcessedValue;
            if (_typeAttribute is null) throw new BPMNCheckerExceptions($"Node {node.ID} ({(string.IsNullOrWhiteSpace(node.Type?.Name) ? node.Type?.Name : "")}) is missing required attribute type");
            result.Type = (string)_typeAttribute;

            // optional: id -> string id (1, 1)
            var _idAttribute = node.Attributes.ContainsKey("id") ? node.Attributes["id"].ProcessedValue : null;
            if (_idAttribute is not null) result.Id = (string)_idAttribute;

            // optional: direction -> RelationshipDirection direction (1, 1)
            var _directionAttribute = node.Attributes.ContainsKey("direction") ? node.Attributes["direction"].ProcessedValue : null;
            if (_directionAttribute is not null) result.Direction = CreateEnum<RelationshipDirection>((string)_directionAttribute);

            // element: documentation -> Documentation documentation (0, *)
            FillElements(node.ChildNodes["documentation"], result.Documentation);

            // element: source -> Element sources (1, *)
            FillElements(node.ChildNodes["source"], result.Sources);

            // element: target -> Element targets (1, *)
            FillElements(node.ChildNodes["target"], result.Targets);

            return result;
        }



        private Extension LoadExtension(XmlParserComplexNode node)
        {
             var result = GetOrCreate<Extension>(node);

            // optional: mustUnderstand -> bool mustUnderstand (1, 1)
            var _mustUnderstandAttribute = node.Attributes.ContainsKey("mustUnderstand") ? node.Attributes["mustUnderstand"].ProcessedValue : null;
            if (_mustUnderstandAttribute is not null) result.MustUnderstand = (bool)_mustUnderstandAttribute;

            // optional: definition -> ExtensionDefinition definition (1, 1)
            var _definitionAttribute = node.Attributes.ContainsKey("definition") ? node.Attributes["definition"].ProcessedValue : null;
            if (_definitionAttribute is not null) result.Definition = Load<ExtensionDefinition>((XmlParserComplexNode)_definitionAttribute);

            return result;
        }





        private Documentation LoadDocumentation(XmlParserComplexNode node)
        {
             var result = GetOrCreate<Documentation>(node);

            // optional: id -> string id (1, 1)
            var _idAttribute = node.Attributes.ContainsKey("id") ? node.Attributes["id"].ProcessedValue : null;
            if (_idAttribute is not null) result.Id = (string)_idAttribute;

            // optional: textFormat -> string textFormat (1, 1)
            var _textFormatAttribute = node.Attributes.ContainsKey("textFormat") ? node.Attributes["textFormat"].ProcessedValue : null;
            if (_textFormatAttribute is not null) result.TextFormat = (string)_textFormatAttribute;

            // missing: string text (1, 1)

            return result;
        }



        private IntermediateCatchEvent LoadIntermediateCatchEvent(XmlParserComplexNode node)
        {
             var result = GetOrCreate<IntermediateCatchEvent>(node);

            // optional: id -> string id (1, 1)
            var _idAttribute = node.Attributes.ContainsKey("id") ? node.Attributes["id"].ProcessedValue : null;
            if (_idAttribute is not null) result.Id = (string)_idAttribute;

            // optional: name -> string name (1, 1)
            var _nameAttribute = node.Attributes.ContainsKey("name") ? node.Attributes["name"].ProcessedValue : null;
            if (_nameAttribute is not null) result.Name = (string)_nameAttribute;

            // optional: parallelMultiple -> bool parallelMultiple (1, 1)
            var _parallelMultipleAttribute = node.Attributes.ContainsKey("parallelMultiple") ? node.Attributes["parallelMultiple"].ProcessedValue : null;
            if (_parallelMultipleAttribute is not null) result.ParallelMultiple = (bool)_parallelMultipleAttribute;

            // element: documentation -> Documentation documentation (0, *)
            FillElements(node.ChildNodes["documentation"], result.Documentation);

            // element: auditing -> Auditing auditing (0, 1)
            result.Auditing = FillElement<Auditing>(node.ChildNodes["auditing"]);

            // element: monitoring -> Monitoring monitoring (0, 1)
            result.Monitoring = FillElement<Monitoring>(node.ChildNodes["monitoring"]);

            // element: categoryValueRef -> CategoryValue categoryValueRef (0, *)
            FillElements(node.ChildNodes["categoryValueRef"], result.CategoryValueRef);

            // element: outgoing -> SequenceFlow outgoing (0, *)
            FillElements(node.ChildNodes["outgoing"], result.Outgoing);

            // element: incoming -> SequenceFlow incoming (0, *)
            FillElements(node.ChildNodes["incoming"], result.Incoming);

            // element: property -> Property properties (0, *)
            FillElements(node.ChildNodes["property"], result.Properties);

            // element: outputSet -> OutputSet outputSet (0, 1)
            result.OutputSet = FillElement<OutputSet>(node.ChildNodes["outputSet"]);

            // element: eventDefinitionRef -> EventDefinition eventDefinitionRefs (0, *)
            FillElements(node.ChildNodes["eventDefinitionRef"], result.EventDefinitionRefs);

            // element: dataOutputAssociation -> DataOutputAssociation dataOutputAssociation (0, *)
            FillElements(node.ChildNodes["dataOutputAssociation"], result.DataOutputAssociation);

            // element: dataOutput -> DataOutput dataOutputs (0, *)
            FillElements(node.ChildNodes["dataOutput"], result.DataOutputs);

            // element: eventDefinition -> EventDefinition eventDefinitions (0, *)
            FillElements(node.ChildNodes["eventDefinition"], result.EventDefinitions);

            // link back: Lane lanes (0, *)
            // lanes 1 <cmof_model.M_Two_Way_Association object at 0x000002941E4AD800>
            // target: Lane.flowNodeRefs

            // link back: ConversationLink incomingConversationLinks (0, *)
            // incomingConversationLinks 1 <cmof_model.M_Two_Way_Association object at 0x000002941E4ADD40>
            // target: ConversationLink.targetRef

            // link back: ConversationLink outgoingConversationLinks (0, *)
            // outgoingConversationLinks 1 <cmof_model.M_Two_Way_Association object at 0x000002941E4ADCC0>
            // target: ConversationLink.sourceRef

            return result;
        }


        private IntermediateThrowEvent LoadIntermediateThrowEvent(XmlParserComplexNode node)
        {
             var result = GetOrCreate<IntermediateThrowEvent>(node);

            // optional: id -> string id (1, 1)
            var _idAttribute = node.Attributes.ContainsKey("id") ? node.Attributes["id"].ProcessedValue : null;
            if (_idAttribute is not null) result.Id = (string)_idAttribute;

            // optional: name -> string name (1, 1)
            var _nameAttribute = node.Attributes.ContainsKey("name") ? node.Attributes["name"].ProcessedValue : null;
            if (_nameAttribute is not null) result.Name = (string)_nameAttribute;

            // element: documentation -> Documentation documentation (0, *)
            FillElements(node.ChildNodes["documentation"], result.Documentation);

            // element: auditing -> Auditing auditing (0, 1)
            result.Auditing = FillElement<Auditing>(node.ChildNodes["auditing"]);

            // element: monitoring -> Monitoring monitoring (0, 1)
            result.Monitoring = FillElement<Monitoring>(node.ChildNodes["monitoring"]);

            // element: categoryValueRef -> CategoryValue categoryValueRef (0, *)
            FillElements(node.ChildNodes["categoryValueRef"], result.CategoryValueRef);

            // element: outgoing -> SequenceFlow outgoing (0, *)
            FillElements(node.ChildNodes["outgoing"], result.Outgoing);

            // element: incoming -> SequenceFlow incoming (0, *)
            FillElements(node.ChildNodes["incoming"], result.Incoming);

            // element: property -> Property properties (0, *)
            FillElements(node.ChildNodes["property"], result.Properties);

            // element: inputSet -> InputSet inputSet (0, 1)
            result.InputSet = FillElement<InputSet>(node.ChildNodes["inputSet"]);

            // element: eventDefinitionRef -> EventDefinition eventDefinitionRefs (0, *)
            FillElements(node.ChildNodes["eventDefinitionRef"], result.EventDefinitionRefs);

            // element: dataInputAssociation -> DataInputAssociation dataInputAssociation (0, *)
            FillElements(node.ChildNodes["dataInputAssociation"], result.DataInputAssociation);

            // element: dataInput -> DataInput dataInputs (0, *)
            FillElements(node.ChildNodes["dataInput"], result.DataInputs);

            // element: eventDefinition -> EventDefinition eventDefinitions (0, *)
            FillElements(node.ChildNodes["eventDefinition"], result.EventDefinitions);

            // link back: Lane lanes (0, *)
            // lanes 1 <cmof_model.M_Two_Way_Association object at 0x000002941E4AD800>
            // target: Lane.flowNodeRefs

            // link back: ConversationLink incomingConversationLinks (0, *)
            // incomingConversationLinks 1 <cmof_model.M_Two_Way_Association object at 0x000002941E4ADD40>
            // target: ConversationLink.targetRef

            // link back: ConversationLink outgoingConversationLinks (0, *)
            // outgoingConversationLinks 1 <cmof_model.M_Two_Way_Association object at 0x000002941E4ADCC0>
            // target: ConversationLink.sourceRef

            return result;
        }


        private EndEvent LoadEndEvent(XmlParserComplexNode node)
        {
             var result = GetOrCreate<EndEvent>(node);

            // optional: id -> string id (1, 1)
            var _idAttribute = node.Attributes.ContainsKey("id") ? node.Attributes["id"].ProcessedValue : null;
            if (_idAttribute is not null) result.Id = (string)_idAttribute;

            // optional: name -> string name (1, 1)
            var _nameAttribute = node.Attributes.ContainsKey("name") ? node.Attributes["name"].ProcessedValue : null;
            if (_nameAttribute is not null) result.Name = (string)_nameAttribute;

            // element: documentation -> Documentation documentation (0, *)
            FillElements(node.ChildNodes["documentation"], result.Documentation);

            // element: auditing -> Auditing auditing (0, 1)
            result.Auditing = FillElement<Auditing>(node.ChildNodes["auditing"]);

            // element: monitoring -> Monitoring monitoring (0, 1)
            result.Monitoring = FillElement<Monitoring>(node.ChildNodes["monitoring"]);

            // element: categoryValueRef -> CategoryValue categoryValueRef (0, *)
            FillElements(node.ChildNodes["categoryValueRef"], result.CategoryValueRef);

            // element: outgoing -> SequenceFlow outgoing (0, *)
            FillElements(node.ChildNodes["outgoing"], result.Outgoing);

            // element: incoming -> SequenceFlow incoming (0, *)
            FillElements(node.ChildNodes["incoming"], result.Incoming);

            // element: property -> Property properties (0, *)
            FillElements(node.ChildNodes["property"], result.Properties);

            // element: inputSet -> InputSet inputSet (0, 1)
            result.InputSet = FillElement<InputSet>(node.ChildNodes["inputSet"]);

            // element: eventDefinitionRef -> EventDefinition eventDefinitionRefs (0, *)
            FillElements(node.ChildNodes["eventDefinitionRef"], result.EventDefinitionRefs);

            // element: dataInputAssociation -> DataInputAssociation dataInputAssociation (0, *)
            FillElements(node.ChildNodes["dataInputAssociation"], result.DataInputAssociation);

            // element: dataInput -> DataInput dataInputs (0, *)
            FillElements(node.ChildNodes["dataInput"], result.DataInputs);

            // element: eventDefinition -> EventDefinition eventDefinitions (0, *)
            FillElements(node.ChildNodes["eventDefinition"], result.EventDefinitions);

            // link back: Lane lanes (0, *)
            // lanes 1 <cmof_model.M_Two_Way_Association object at 0x000002941E4AD800>
            // target: Lane.flowNodeRefs

            // link back: ConversationLink incomingConversationLinks (0, *)
            // incomingConversationLinks 1 <cmof_model.M_Two_Way_Association object at 0x000002941E4ADD40>
            // target: ConversationLink.targetRef

            // link back: ConversationLink outgoingConversationLinks (0, *)
            // outgoingConversationLinks 1 <cmof_model.M_Two_Way_Association object at 0x000002941E4ADCC0>
            // target: ConversationLink.sourceRef

            return result;
        }


        private StartEvent LoadStartEvent(XmlParserComplexNode node)
        {
             var result = GetOrCreate<StartEvent>(node);

            // optional: id -> string id (1, 1)
            var _idAttribute = node.Attributes.ContainsKey("id") ? node.Attributes["id"].ProcessedValue : null;
            if (_idAttribute is not null) result.Id = (string)_idAttribute;

            // optional: name -> string name (1, 1)
            var _nameAttribute = node.Attributes.ContainsKey("name") ? node.Attributes["name"].ProcessedValue : null;
            if (_nameAttribute is not null) result.Name = (string)_nameAttribute;

            // optional: parallelMultiple -> bool parallelMultiple (1, 1)
            var _parallelMultipleAttribute = node.Attributes.ContainsKey("parallelMultiple") ? node.Attributes["parallelMultiple"].ProcessedValue : null;
            if (_parallelMultipleAttribute is not null) result.ParallelMultiple = (bool)_parallelMultipleAttribute;

            // optional: isInterrupting -> bool isInterrupting (1, 1)
            var _isInterruptingAttribute = node.Attributes.ContainsKey("isInterrupting") ? node.Attributes["isInterrupting"].ProcessedValue : null;
            if (_isInterruptingAttribute is not null) result.IsInterrupting = (bool)_isInterruptingAttribute;

            // element: documentation -> Documentation documentation (0, *)
            FillElements(node.ChildNodes["documentation"], result.Documentation);

            // element: auditing -> Auditing auditing (0, 1)
            result.Auditing = FillElement<Auditing>(node.ChildNodes["auditing"]);

            // element: monitoring -> Monitoring monitoring (0, 1)
            result.Monitoring = FillElement<Monitoring>(node.ChildNodes["monitoring"]);

            // element: categoryValueRef -> CategoryValue categoryValueRef (0, *)
            FillElements(node.ChildNodes["categoryValueRef"], result.CategoryValueRef);

            // element: outgoing -> SequenceFlow outgoing (0, *)
            FillElements(node.ChildNodes["outgoing"], result.Outgoing);

            // element: incoming -> SequenceFlow incoming (0, *)
            FillElements(node.ChildNodes["incoming"], result.Incoming);

            // element: property -> Property properties (0, *)
            FillElements(node.ChildNodes["property"], result.Properties);

            // element: outputSet -> OutputSet outputSet (0, 1)
            result.OutputSet = FillElement<OutputSet>(node.ChildNodes["outputSet"]);

            // element: eventDefinitionRef -> EventDefinition eventDefinitionRefs (0, *)
            FillElements(node.ChildNodes["eventDefinitionRef"], result.EventDefinitionRefs);

            // element: dataOutputAssociation -> DataOutputAssociation dataOutputAssociation (0, *)
            FillElements(node.ChildNodes["dataOutputAssociation"], result.DataOutputAssociation);

            // element: dataOutput -> DataOutput dataOutputs (0, *)
            FillElements(node.ChildNodes["dataOutput"], result.DataOutputs);

            // element: eventDefinition -> EventDefinition eventDefinitions (0, *)
            FillElements(node.ChildNodes["eventDefinition"], result.EventDefinitions);

            // link back: Lane lanes (0, *)
            // lanes 1 <cmof_model.M_Two_Way_Association object at 0x000002941E4AD800>
            // target: Lane.flowNodeRefs

            // link back: ConversationLink incomingConversationLinks (0, *)
            // incomingConversationLinks 1 <cmof_model.M_Two_Way_Association object at 0x000002941E4ADD40>
            // target: ConversationLink.targetRef

            // link back: ConversationLink outgoingConversationLinks (0, *)
            // outgoingConversationLinks 1 <cmof_model.M_Two_Way_Association object at 0x000002941E4ADCC0>
            // target: ConversationLink.sourceRef

            return result;
        }




        private BoundaryEvent LoadBoundaryEvent(XmlParserComplexNode node)
        {
             var result = GetOrCreate<BoundaryEvent>(node);

            // required: attachedToRef -> Activity attachedToRef (1, 1)
            var _attachedToRefAttribute = node.Attributes["attachedToRef"]?.ProcessedValue;
            if (_attachedToRefAttribute is null) throw new BPMNCheckerExceptions($"Node {node.ID} ({(string.IsNullOrWhiteSpace(node.Type?.Name) ? node.Type?.Name : "")}) is missing required attribute attachedToRef");
            result.AttachedToRef = Load<Activity>((XmlParserComplexNode)_attachedToRefAttribute);

            // optional: id -> string id (1, 1)
            var _idAttribute = node.Attributes.ContainsKey("id") ? node.Attributes["id"].ProcessedValue : null;
            if (_idAttribute is not null) result.Id = (string)_idAttribute;

            // optional: name -> string name (1, 1)
            var _nameAttribute = node.Attributes.ContainsKey("name") ? node.Attributes["name"].ProcessedValue : null;
            if (_nameAttribute is not null) result.Name = (string)_nameAttribute;

            // optional: parallelMultiple -> bool parallelMultiple (1, 1)
            var _parallelMultipleAttribute = node.Attributes.ContainsKey("parallelMultiple") ? node.Attributes["parallelMultiple"].ProcessedValue : null;
            if (_parallelMultipleAttribute is not null) result.ParallelMultiple = (bool)_parallelMultipleAttribute;

            // optional: cancelActivity -> bool cancelActivity (1, 1)
            var _cancelActivityAttribute = node.Attributes.ContainsKey("cancelActivity") ? node.Attributes["cancelActivity"].ProcessedValue : null;
            if (_cancelActivityAttribute is not null) result.CancelActivity = (bool)_cancelActivityAttribute;

            // element: documentation -> Documentation documentation (0, *)
            FillElements(node.ChildNodes["documentation"], result.Documentation);

            // element: auditing -> Auditing auditing (0, 1)
            result.Auditing = FillElement<Auditing>(node.ChildNodes["auditing"]);

            // element: monitoring -> Monitoring monitoring (0, 1)
            result.Monitoring = FillElement<Monitoring>(node.ChildNodes["monitoring"]);

            // element: categoryValueRef -> CategoryValue categoryValueRef (0, *)
            FillElements(node.ChildNodes["categoryValueRef"], result.CategoryValueRef);

            // element: outgoing -> SequenceFlow outgoing (0, *)
            FillElements(node.ChildNodes["outgoing"], result.Outgoing);

            // element: incoming -> SequenceFlow incoming (0, *)
            FillElements(node.ChildNodes["incoming"], result.Incoming);

            // element: property -> Property properties (0, *)
            FillElements(node.ChildNodes["property"], result.Properties);

            // element: outputSet -> OutputSet outputSet (0, 1)
            result.OutputSet = FillElement<OutputSet>(node.ChildNodes["outputSet"]);

            // element: eventDefinitionRef -> EventDefinition eventDefinitionRefs (0, *)
            FillElements(node.ChildNodes["eventDefinitionRef"], result.EventDefinitionRefs);

            // element: dataOutputAssociation -> DataOutputAssociation dataOutputAssociation (0, *)
            FillElements(node.ChildNodes["dataOutputAssociation"], result.DataOutputAssociation);

            // element: dataOutput -> DataOutput dataOutputs (0, *)
            FillElements(node.ChildNodes["dataOutput"], result.DataOutputs);

            // element: eventDefinition -> EventDefinition eventDefinitions (0, *)
            FillElements(node.ChildNodes["eventDefinition"], result.EventDefinitions);

            // link back: Lane lanes (0, *)
            // lanes 1 <cmof_model.M_Two_Way_Association object at 0x000002941E4AD800>
            // target: Lane.flowNodeRefs

            // link back: ConversationLink incomingConversationLinks (0, *)
            // incomingConversationLinks 1 <cmof_model.M_Two_Way_Association object at 0x000002941E4ADD40>
            // target: ConversationLink.targetRef

            // link back: ConversationLink outgoingConversationLinks (0, *)
            // outgoingConversationLinks 1 <cmof_model.M_Two_Way_Association object at 0x000002941E4ADCC0>
            // target: ConversationLink.sourceRef

            return result;
        }



        private CancelEventDefinition LoadCancelEventDefinition(XmlParserComplexNode node)
        {
             var result = GetOrCreate<CancelEventDefinition>(node);

            // optional: id -> string id (1, 1)
            var _idAttribute = node.Attributes.ContainsKey("id") ? node.Attributes["id"].ProcessedValue : null;
            if (_idAttribute is not null) result.Id = (string)_idAttribute;

            // element: documentation -> Documentation documentation (0, *)
            FillElements(node.ChildNodes["documentation"], result.Documentation);

            return result;
        }


        private ErrorEventDefinition LoadErrorEventDefinition(XmlParserComplexNode node)
        {
             var result = GetOrCreate<ErrorEventDefinition>(node);

            // optional: id -> string id (1, 1)
            var _idAttribute = node.Attributes.ContainsKey("id") ? node.Attributes["id"].ProcessedValue : null;
            if (_idAttribute is not null) result.Id = (string)_idAttribute;

            // optional: errorRef -> Error errorRef (0, 1)
            var _errorRefAttribute = node.Attributes.ContainsKey("errorRef") ? node.Attributes["errorRef"].ProcessedValue : null;
            if (_errorRefAttribute is not null) result.ErrorRef = Load<Error>((XmlParserComplexNode)_errorRefAttribute);

            // element: documentation -> Documentation documentation (0, *)
            FillElements(node.ChildNodes["documentation"], result.Documentation);

            return result;
        }


        private TerminateEventDefinition LoadTerminateEventDefinition(XmlParserComplexNode node)
        {
             var result = GetOrCreate<TerminateEventDefinition>(node);

            // optional: id -> string id (1, 1)
            var _idAttribute = node.Attributes.ContainsKey("id") ? node.Attributes["id"].ProcessedValue : null;
            if (_idAttribute is not null) result.Id = (string)_idAttribute;

            // element: documentation -> Documentation documentation (0, *)
            FillElements(node.ChildNodes["documentation"], result.Documentation);

            return result;
        }


        private EscalationEventDefinition LoadEscalationEventDefinition(XmlParserComplexNode node)
        {
             var result = GetOrCreate<EscalationEventDefinition>(node);

            // optional: id -> string id (1, 1)
            var _idAttribute = node.Attributes.ContainsKey("id") ? node.Attributes["id"].ProcessedValue : null;
            if (_idAttribute is not null) result.Id = (string)_idAttribute;

            // optional: escalationRef -> Escalation escalationRef (0, 1)
            var _escalationRefAttribute = node.Attributes.ContainsKey("escalationRef") ? node.Attributes["escalationRef"].ProcessedValue : null;
            if (_escalationRefAttribute is not null) result.EscalationRef = Load<Escalation>((XmlParserComplexNode)_escalationRefAttribute);

            // element: documentation -> Documentation documentation (0, *)
            FillElements(node.ChildNodes["documentation"], result.Documentation);

            return result;
        }


        private Escalation LoadEscalation(XmlParserComplexNode node)
        {
             var result = GetOrCreate<Escalation>(node);

            // optional: structureRef -> ItemDefinition structureRef (0, 1)
            var _structureRefAttribute = node.Attributes.ContainsKey("structureRef") ? node.Attributes["structureRef"].ProcessedValue : null;
            if (_structureRefAttribute is not null) result.StructureRef = Load<ItemDefinition>((XmlParserComplexNode)_structureRefAttribute);

            // optional: name -> string name (1, 1)
            var _nameAttribute = node.Attributes.ContainsKey("name") ? node.Attributes["name"].ProcessedValue : null;
            if (_nameAttribute is not null) result.Name = (string)_nameAttribute;

            // optional: escalationCode -> string escalationCode (1, 1)
            var _escalationCodeAttribute = node.Attributes.ContainsKey("escalationCode") ? node.Attributes["escalationCode"].ProcessedValue : null;
            if (_escalationCodeAttribute is not null) result.EscalationCode = (string)_escalationCodeAttribute;

            return result;
        }


        private CompensateEventDefinition LoadCompensateEventDefinition(XmlParserComplexNode node)
        {
             var result = GetOrCreate<CompensateEventDefinition>(node);

            // optional: id -> string id (1, 1)
            var _idAttribute = node.Attributes.ContainsKey("id") ? node.Attributes["id"].ProcessedValue : null;
            if (_idAttribute is not null) result.Id = (string)_idAttribute;

            // optional: waitForCompletion -> bool waitForCompletion (1, 1)
            var _waitForCompletionAttribute = node.Attributes.ContainsKey("waitForCompletion") ? node.Attributes["waitForCompletion"].ProcessedValue : null;
            if (_waitForCompletionAttribute is not null) result.WaitForCompletion = (bool)_waitForCompletionAttribute;

            // optional: activityRef -> Activity activityRef (0, 1)
            var _activityRefAttribute = node.Attributes.ContainsKey("activityRef") ? node.Attributes["activityRef"].ProcessedValue : null;
            if (_activityRefAttribute is not null) result.ActivityRef = Load<Activity>((XmlParserComplexNode)_activityRefAttribute);

            // element: documentation -> Documentation documentation (0, *)
            FillElements(node.ChildNodes["documentation"], result.Documentation);

            return result;
        }


        private TimerEventDefinition LoadTimerEventDefinition(XmlParserComplexNode node)
        {
             var result = GetOrCreate<TimerEventDefinition>(node);

            // optional: id -> string id (1, 1)
            var _idAttribute = node.Attributes.ContainsKey("id") ? node.Attributes["id"].ProcessedValue : null;
            if (_idAttribute is not null) result.Id = (string)_idAttribute;

            // element: documentation -> Documentation documentation (0, *)
            FillElements(node.ChildNodes["documentation"], result.Documentation);

            // element: timeDate -> Expression timeDate (0, 1)
            result.TimeDate = FillElement<Expression>(node.ChildNodes["timeDate"]);

            // element: timeCycle -> Expression timeCycle (0, 1)
            result.TimeCycle = FillElement<Expression>(node.ChildNodes["timeCycle"]);

            // element: timeDuration -> Expression timeDuration (0, 1)
            result.TimeDuration = FillElement<Expression>(node.ChildNodes["timeDuration"]);

            return result;
        }


        private LinkEventDefinition LoadLinkEventDefinition(XmlParserComplexNode node)
        {
             var result = GetOrCreate<LinkEventDefinition>(node);

            // required: name -> string name (1, 1)
            var _nameAttribute = node.Attributes["name"]?.ProcessedValue;
            if (_nameAttribute is null) throw new BPMNCheckerExceptions($"Node {node.ID} ({(string.IsNullOrWhiteSpace(node.Type?.Name) ? node.Type?.Name : "")}) is missing required attribute name");
            result.Name = (string)_nameAttribute;

            // optional: id -> string id (1, 1)
            var _idAttribute = node.Attributes.ContainsKey("id") ? node.Attributes["id"].ProcessedValue : null;
            if (_idAttribute is not null) result.Id = (string)_idAttribute;

            // element: documentation -> Documentation documentation (0, *)
            FillElements(node.ChildNodes["documentation"], result.Documentation);

            // element: target -> LinkEventDefinition target (0, 1)
            result.Target = FillElement<LinkEventDefinition>(node.ChildNodes["target"]);

            // element: source -> LinkEventDefinition source (0, *)
            FillElements(node.ChildNodes["source"], result.Source);

            return result;
        }


        private MessageEventDefinition LoadMessageEventDefinition(XmlParserComplexNode node)
        {
             var result = GetOrCreate<MessageEventDefinition>(node);

            // optional: id -> string id (1, 1)
            var _idAttribute = node.Attributes.ContainsKey("id") ? node.Attributes["id"].ProcessedValue : null;
            if (_idAttribute is not null) result.Id = (string)_idAttribute;

            // optional: messageRef -> Message messageRef (0, 1)
            var _messageRefAttribute = node.Attributes.ContainsKey("messageRef") ? node.Attributes["messageRef"].ProcessedValue : null;
            if (_messageRefAttribute is not null) result.MessageRef = Load<Message>((XmlParserComplexNode)_messageRefAttribute);

            // element: documentation -> Documentation documentation (0, *)
            FillElements(node.ChildNodes["documentation"], result.Documentation);

            // element: operationRef -> Operation operationRef (0, 1)
            result.OperationRef = FillElement<Operation>(node.ChildNodes["operationRef"]);

            return result;
        }


        private ConditionalEventDefinition LoadConditionalEventDefinition(XmlParserComplexNode node)
        {
             var result = GetOrCreate<ConditionalEventDefinition>(node);

            // optional: id -> string id (1, 1)
            var _idAttribute = node.Attributes.ContainsKey("id") ? node.Attributes["id"].ProcessedValue : null;
            if (_idAttribute is not null) result.Id = (string)_idAttribute;

            // element: documentation -> Documentation documentation (0, *)
            FillElements(node.ChildNodes["documentation"], result.Documentation);

            // element: condition -> Expression condition (1, 1)
            result.Condition = FillElement<Expression>(node.ChildNodes["condition"]);

            return result;
        }


        private SignalEventDefinition LoadSignalEventDefinition(XmlParserComplexNode node)
        {
             var result = GetOrCreate<SignalEventDefinition>(node);

            // optional: id -> string id (1, 1)
            var _idAttribute = node.Attributes.ContainsKey("id") ? node.Attributes["id"].ProcessedValue : null;
            if (_idAttribute is not null) result.Id = (string)_idAttribute;

            // optional: signalRef -> Signal signalRef (0, 1)
            var _signalRefAttribute = node.Attributes.ContainsKey("signalRef") ? node.Attributes["signalRef"].ProcessedValue : null;
            if (_signalRefAttribute is not null) result.SignalRef = Load<Signal>((XmlParserComplexNode)_signalRefAttribute);

            // element: documentation -> Documentation documentation (0, *)
            FillElements(node.ChildNodes["documentation"], result.Documentation);

            return result;
        }


        private Signal LoadSignal(XmlParserComplexNode node)
        {
             var result = GetOrCreate<Signal>(node);

            // optional: id -> string id (1, 1)
            var _idAttribute = node.Attributes.ContainsKey("id") ? node.Attributes["id"].ProcessedValue : null;
            if (_idAttribute is not null) result.Id = (string)_idAttribute;

            // optional: structureRef -> ItemDefinition structureRef (0, 1)
            var _structureRefAttribute = node.Attributes.ContainsKey("structureRef") ? node.Attributes["structureRef"].ProcessedValue : null;
            if (_structureRefAttribute is not null) result.StructureRef = Load<ItemDefinition>((XmlParserComplexNode)_structureRefAttribute);

            // optional: name -> string name (1, 1)
            var _nameAttribute = node.Attributes.ContainsKey("name") ? node.Attributes["name"].ProcessedValue : null;
            if (_nameAttribute is not null) result.Name = (string)_nameAttribute;

            // element: documentation -> Documentation documentation (0, *)
            FillElements(node.ChildNodes["documentation"], result.Documentation);

            return result;
        }


        private ImplicitThrowEvent LoadImplicitThrowEvent(XmlParserComplexNode node)
        {
             var result = GetOrCreate<ImplicitThrowEvent>(node);

            // optional: id -> string id (1, 1)
            var _idAttribute = node.Attributes.ContainsKey("id") ? node.Attributes["id"].ProcessedValue : null;
            if (_idAttribute is not null) result.Id = (string)_idAttribute;

            // optional: name -> string name (1, 1)
            var _nameAttribute = node.Attributes.ContainsKey("name") ? node.Attributes["name"].ProcessedValue : null;
            if (_nameAttribute is not null) result.Name = (string)_nameAttribute;

            // element: documentation -> Documentation documentation (0, *)
            FillElements(node.ChildNodes["documentation"], result.Documentation);

            // element: auditing -> Auditing auditing (0, 1)
            result.Auditing = FillElement<Auditing>(node.ChildNodes["auditing"]);

            // element: monitoring -> Monitoring monitoring (0, 1)
            result.Monitoring = FillElement<Monitoring>(node.ChildNodes["monitoring"]);

            // element: categoryValueRef -> CategoryValue categoryValueRef (0, *)
            FillElements(node.ChildNodes["categoryValueRef"], result.CategoryValueRef);

            // element: outgoing -> SequenceFlow outgoing (0, *)
            FillElements(node.ChildNodes["outgoing"], result.Outgoing);

            // element: incoming -> SequenceFlow incoming (0, *)
            FillElements(node.ChildNodes["incoming"], result.Incoming);

            // element: property -> Property properties (0, *)
            FillElements(node.ChildNodes["property"], result.Properties);

            // element: inputSet -> InputSet inputSet (0, 1)
            result.InputSet = FillElement<InputSet>(node.ChildNodes["inputSet"]);

            // element: eventDefinitionRef -> EventDefinition eventDefinitionRefs (0, *)
            FillElements(node.ChildNodes["eventDefinitionRef"], result.EventDefinitionRefs);

            // element: dataInputAssociation -> DataInputAssociation dataInputAssociation (0, *)
            FillElements(node.ChildNodes["dataInputAssociation"], result.DataInputAssociation);

            // element: dataInput -> DataInput dataInputs (0, *)
            FillElements(node.ChildNodes["dataInput"], result.DataInputs);

            // element: eventDefinition -> EventDefinition eventDefinitions (0, *)
            FillElements(node.ChildNodes["eventDefinition"], result.EventDefinitions);

            // link back: Lane lanes (0, *)
            // lanes 1 <cmof_model.M_Two_Way_Association object at 0x000002941E4AD800>
            // target: Lane.flowNodeRefs

            // link back: ConversationLink incomingConversationLinks (0, *)
            // incomingConversationLinks 1 <cmof_model.M_Two_Way_Association object at 0x000002941E4ADD40>
            // target: ConversationLink.targetRef

            // link back: ConversationLink outgoingConversationLinks (0, *)
            // outgoingConversationLinks 1 <cmof_model.M_Two_Way_Association object at 0x000002941E4ADCC0>
            // target: ConversationLink.sourceRef

            return result;
        }


        private DataState LoadDataState(XmlParserComplexNode node)
        {
             var result = GetOrCreate<DataState>(node);

            // optional: id -> string id (1, 1)
            var _idAttribute = node.Attributes.ContainsKey("id") ? node.Attributes["id"].ProcessedValue : null;
            if (_idAttribute is not null) result.Id = (string)_idAttribute;

            // optional: name -> string name (1, 1)
            var _nameAttribute = node.Attributes.ContainsKey("name") ? node.Attributes["name"].ProcessedValue : null;
            if (_nameAttribute is not null) result.Name = (string)_nameAttribute;

            // element: documentation -> Documentation documentation (0, *)
            FillElements(node.ChildNodes["documentation"], result.Documentation);

            return result;
        }



        private DataAssociation LoadDataAssociation(XmlParserComplexNode node)
        {
             var result = GetOrCreate<DataAssociation>(node);

            // optional: id -> string id (1, 1)
            var _idAttribute = node.Attributes.ContainsKey("id") ? node.Attributes["id"].ProcessedValue : null;
            if (_idAttribute is not null) result.Id = (string)_idAttribute;

            // element: documentation -> Documentation documentation (0, *)
            FillElements(node.ChildNodes["documentation"], result.Documentation);

            // element: transformation -> FormalExpression transformation (0, 1)
            result.Transformation = FillElement<FormalExpression>(node.ChildNodes["transformation"]);

            // element: assignment -> Assignment assignment (0, *)
            FillElements(node.ChildNodes["assignment"], result.Assignment);

            // element: targetRef -> ItemAwareElement targetRef (1, 1)
            result.TargetRef = FillElement<ItemAwareElement>(node.ChildNodes["targetRef"]);

            // element: sourceRef -> ItemAwareElement sourceRef (0, *)
            FillElements(node.ChildNodes["sourceRef"], result.SourceRef);

            return result;
        }


        private DataInput LoadDataInput(XmlParserComplexNode node)
        {
             var result = GetOrCreate<DataInput>(node);

            // optional: id -> string id (1, 1)
            var _idAttribute = node.Attributes.ContainsKey("id") ? node.Attributes["id"].ProcessedValue : null;
            if (_idAttribute is not null) result.Id = (string)_idAttribute;

            // optional: itemSubjectRef -> ItemDefinition itemSubjectRef (0, 1)
            var _itemSubjectRefAttribute = node.Attributes.ContainsKey("itemSubjectRef") ? node.Attributes["itemSubjectRef"].ProcessedValue : null;
            if (_itemSubjectRefAttribute is not null) result.ItemSubjectRef = Load<ItemDefinition>((XmlParserComplexNode)_itemSubjectRefAttribute);

            // optional: name -> string name (0, 1)
            var _nameAttribute = node.Attributes.ContainsKey("name") ? node.Attributes["name"].ProcessedValue : null;
            if (_nameAttribute is not null) result.Name = (string)_nameAttribute;

            // optional: isCollection -> bool isCollection (1, 1)
            var _isCollectionAttribute = node.Attributes.ContainsKey("isCollection") ? node.Attributes["isCollection"].ProcessedValue : null;
            if (_isCollectionAttribute is not null) result.IsCollection = (bool)_isCollectionAttribute;

            // element: documentation -> Documentation documentation (0, *)
            FillElements(node.ChildNodes["documentation"], result.Documentation);

            // element: dataState -> DataState dataState (0, 1)
            result.DataState = FillElement<DataState>(node.ChildNodes["dataState"]);

            // link back: InputSet inputSetRefs (1, *)
            // inputSetRefs 1 <cmof_model.M_Two_Way_Association object at 0x000002941E4AD940>
            // target: InputSet.dataInputRefs

            // link back: InputSet inputSetWithOptional (0, *)
            // inputSetWithOptional 1 <cmof_model.M_Two_Way_Association object at 0x000002941E4ADB40>
            // target: InputSet.optionalInputRefs

            // link back: InputSet inputSetWithWhileExecuting (0, *)
            // inputSetWithWhileExecuting 1 <cmof_model.M_Two_Way_Association object at 0x000002941E4ADAC0>
            // target: InputSet.whileExecutingInputRefs

            return result;
        }


        private DataOutput LoadDataOutput(XmlParserComplexNode node)
        {
             var result = GetOrCreate<DataOutput>(node);

            // optional: id -> string id (1, 1)
            var _idAttribute = node.Attributes.ContainsKey("id") ? node.Attributes["id"].ProcessedValue : null;
            if (_idAttribute is not null) result.Id = (string)_idAttribute;

            // optional: itemSubjectRef -> ItemDefinition itemSubjectRef (0, 1)
            var _itemSubjectRefAttribute = node.Attributes.ContainsKey("itemSubjectRef") ? node.Attributes["itemSubjectRef"].ProcessedValue : null;
            if (_itemSubjectRefAttribute is not null) result.ItemSubjectRef = Load<ItemDefinition>((XmlParserComplexNode)_itemSubjectRefAttribute);

            // optional: name -> string name (0, 1)
            var _nameAttribute = node.Attributes.ContainsKey("name") ? node.Attributes["name"].ProcessedValue : null;
            if (_nameAttribute is not null) result.Name = (string)_nameAttribute;

            // optional: isCollection -> bool isCollection (1, 1)
            var _isCollectionAttribute = node.Attributes.ContainsKey("isCollection") ? node.Attributes["isCollection"].ProcessedValue : null;
            if (_isCollectionAttribute is not null) result.IsCollection = (bool)_isCollectionAttribute;

            // element: documentation -> Documentation documentation (0, *)
            FillElements(node.ChildNodes["documentation"], result.Documentation);

            // element: dataState -> DataState dataState (0, 1)
            result.DataState = FillElement<DataState>(node.ChildNodes["dataState"]);

            // link back: OutputSet outputSetRefs (1, *)
            // outputSetRefs 1 <cmof_model.M_Two_Way_Association object at 0x000002941E4AD9C0>
            // target: OutputSet.dataOutputRefs

            // link back: OutputSet outputSetWithOptional (0, *)
            // outputSetWithOptional 0 <cmof_model.M_Two_Way_Association object at 0x000002941E4ADBC0>
            // target: OutputSet.optionalOutputRefs

            // link back: OutputSet outputSetWithWhileExecuting (0, *)
            // outputSetWithWhileExecuting 0 <cmof_model.M_Two_Way_Association object at 0x000002941E4ADC40>
            // target: OutputSet.whileExecutingOutputRefs

            return result;
        }


        private InputSet LoadInputSet(XmlParserComplexNode node)
        {
             var result = GetOrCreate<InputSet>(node);

            // optional: id -> string id (1, 1)
            var _idAttribute = node.Attributes.ContainsKey("id") ? node.Attributes["id"].ProcessedValue : null;
            if (_idAttribute is not null) result.Id = (string)_idAttribute;

            // optional: name -> string name (1, 1)
            var _nameAttribute = node.Attributes.ContainsKey("name") ? node.Attributes["name"].ProcessedValue : null;
            if (_nameAttribute is not null) result.Name = (string)_nameAttribute;

            // element: documentation -> Documentation documentation (0, *)
            FillElements(node.ChildNodes["documentation"], result.Documentation);

            // element: dataInputRefs -> DataInput dataInputRefs (0, *)
            FillElements(node.ChildNodes["dataInputRefs"], result.DataInputRefs);

            // element: optionalInputRefs -> DataInput optionalInputRefs (0, *)
            FillElements(node.ChildNodes["optionalInputRefs"], result.OptionalInputRefs);

            // element: whileExecutingInputRefs -> DataInput whileExecutingInputRefs (0, *)
            FillElements(node.ChildNodes["whileExecutingInputRefs"], result.WhileExecutingInputRefs);

            // element: outputSetRefs -> OutputSet outputSetRefs (0, *)
            FillElements(node.ChildNodes["outputSetRefs"], result.OutputSetRefs);

            return result;
        }


        private OutputSet LoadOutputSet(XmlParserComplexNode node)
        {
             var result = GetOrCreate<OutputSet>(node);

            // optional: id -> string id (1, 1)
            var _idAttribute = node.Attributes.ContainsKey("id") ? node.Attributes["id"].ProcessedValue : null;
            if (_idAttribute is not null) result.Id = (string)_idAttribute;

            // optional: name -> string name (1, 1)
            var _nameAttribute = node.Attributes.ContainsKey("name") ? node.Attributes["name"].ProcessedValue : null;
            if (_nameAttribute is not null) result.Name = (string)_nameAttribute;

            // element: documentation -> Documentation documentation (0, *)
            FillElements(node.ChildNodes["documentation"], result.Documentation);

            // element: dataOutputRefs -> DataOutput dataOutputRefs (0, *)
            FillElements(node.ChildNodes["dataOutputRefs"], result.DataOutputRefs);

            // element: inputSetRefs -> InputSet inputSetRefs (0, *)
            FillElements(node.ChildNodes["inputSetRefs"], result.InputSetRefs);

            // element: optionalOutputRefs -> DataOutput optionalOutputRefs (0, *)
            FillElements(node.ChildNodes["optionalOutputRefs"], result.OptionalOutputRefs);

            // element: whileExecutingOutputRefs -> DataOutput whileExecutingOutputRefs (0, *)
            FillElements(node.ChildNodes["whileExecutingOutputRefs"], result.WhileExecutingOutputRefs);

            return result;
        }


        private Property LoadProperty(XmlParserComplexNode node)
        {
             var result = GetOrCreate<Property>(node);

            // optional: id -> string id (1, 1)
            var _idAttribute = node.Attributes.ContainsKey("id") ? node.Attributes["id"].ProcessedValue : null;
            if (_idAttribute is not null) result.Id = (string)_idAttribute;

            // optional: itemSubjectRef -> ItemDefinition itemSubjectRef (0, 1)
            var _itemSubjectRefAttribute = node.Attributes.ContainsKey("itemSubjectRef") ? node.Attributes["itemSubjectRef"].ProcessedValue : null;
            if (_itemSubjectRefAttribute is not null) result.ItemSubjectRef = Load<ItemDefinition>((XmlParserComplexNode)_itemSubjectRefAttribute);

            // optional: name -> string name (1, 1)
            var _nameAttribute = node.Attributes.ContainsKey("name") ? node.Attributes["name"].ProcessedValue : null;
            if (_nameAttribute is not null) result.Name = (string)_nameAttribute;

            // element: documentation -> Documentation documentation (0, *)
            FillElements(node.ChildNodes["documentation"], result.Documentation);

            // element: dataState -> DataState dataState (0, 1)
            result.DataState = FillElement<DataState>(node.ChildNodes["dataState"]);

            return result;
        }


        private DataInputAssociation LoadDataInputAssociation(XmlParserComplexNode node)
        {
             var result = GetOrCreate<DataInputAssociation>(node);

            // optional: id -> string id (1, 1)
            var _idAttribute = node.Attributes.ContainsKey("id") ? node.Attributes["id"].ProcessedValue : null;
            if (_idAttribute is not null) result.Id = (string)_idAttribute;

            // element: documentation -> Documentation documentation (0, *)
            FillElements(node.ChildNodes["documentation"], result.Documentation);

            // element: transformation -> FormalExpression transformation (0, 1)
            result.Transformation = FillElement<FormalExpression>(node.ChildNodes["transformation"]);

            // element: assignment -> Assignment assignment (0, *)
            FillElements(node.ChildNodes["assignment"], result.Assignment);

            // element: targetRef -> ItemAwareElement targetRef (1, 1)
            result.TargetRef = FillElement<ItemAwareElement>(node.ChildNodes["targetRef"]);

            // element: sourceRef -> ItemAwareElement sourceRef (0, *)
            FillElements(node.ChildNodes["sourceRef"], result.SourceRef);

            return result;
        }


        private DataOutputAssociation LoadDataOutputAssociation(XmlParserComplexNode node)
        {
             var result = GetOrCreate<DataOutputAssociation>(node);

            // optional: id -> string id (1, 1)
            var _idAttribute = node.Attributes.ContainsKey("id") ? node.Attributes["id"].ProcessedValue : null;
            if (_idAttribute is not null) result.Id = (string)_idAttribute;

            // element: documentation -> Documentation documentation (0, *)
            FillElements(node.ChildNodes["documentation"], result.Documentation);

            // element: transformation -> FormalExpression transformation (0, 1)
            result.Transformation = FillElement<FormalExpression>(node.ChildNodes["transformation"]);

            // element: assignment -> Assignment assignment (0, *)
            FillElements(node.ChildNodes["assignment"], result.Assignment);

            // element: targetRef -> ItemAwareElement targetRef (1, 1)
            result.TargetRef = FillElement<ItemAwareElement>(node.ChildNodes["targetRef"]);

            // element: sourceRef -> ItemAwareElement sourceRef (0, *)
            FillElements(node.ChildNodes["sourceRef"], result.SourceRef);

            return result;
        }


        private InputOutputSpecification LoadInputOutputSpecification(XmlParserComplexNode node)
        {
             var result = GetOrCreate<InputOutputSpecification>(node);

            // optional: id -> string id (1, 1)
            var _idAttribute = node.Attributes.ContainsKey("id") ? node.Attributes["id"].ProcessedValue : null;
            if (_idAttribute is not null) result.Id = (string)_idAttribute;

            // element: documentation -> Documentation documentation (0, *)
            FillElements(node.ChildNodes["documentation"], result.Documentation);

            // element: inputSet -> InputSet inputSets (1, *)
            FillElements(node.ChildNodes["inputSet"], result.InputSets);

            // element: outputSet -> OutputSet outputSets (1, *)
            FillElements(node.ChildNodes["outputSet"], result.OutputSets);

            // element: dataInput -> DataInput dataInputs (0, *)
            FillElements(node.ChildNodes["dataInput"], result.DataInputs);

            // element: dataOutput -> DataOutput dataOutputs (0, *)
            FillElements(node.ChildNodes["dataOutput"], result.DataOutputs);

            return result;
        }


        private DataObject LoadDataObject(XmlParserComplexNode node)
        {
             var result = GetOrCreate<DataObject>(node);

            // optional: id -> string id (1, 1)
            var _idAttribute = node.Attributes.ContainsKey("id") ? node.Attributes["id"].ProcessedValue : null;
            if (_idAttribute is not null) result.Id = (string)_idAttribute;

            // optional: name -> string name (1, 1)
            var _nameAttribute = node.Attributes.ContainsKey("name") ? node.Attributes["name"].ProcessedValue : null;
            if (_nameAttribute is not null) result.Name = (string)_nameAttribute;

            // optional: itemSubjectRef -> ItemDefinition itemSubjectRef (0, 1)
            var _itemSubjectRefAttribute = node.Attributes.ContainsKey("itemSubjectRef") ? node.Attributes["itemSubjectRef"].ProcessedValue : null;
            if (_itemSubjectRefAttribute is not null) result.ItemSubjectRef = Load<ItemDefinition>((XmlParserComplexNode)_itemSubjectRefAttribute);

            // optional: isCollection -> bool isCollection (1, 1)
            var _isCollectionAttribute = node.Attributes.ContainsKey("isCollection") ? node.Attributes["isCollection"].ProcessedValue : null;
            if (_isCollectionAttribute is not null) result.IsCollection = (bool)_isCollectionAttribute;

            // element: documentation -> Documentation documentation (0, *)
            FillElements(node.ChildNodes["documentation"], result.Documentation);

            // element: auditing -> Auditing auditing (0, 1)
            result.Auditing = FillElement<Auditing>(node.ChildNodes["auditing"]);

            // element: monitoring -> Monitoring monitoring (0, 1)
            result.Monitoring = FillElement<Monitoring>(node.ChildNodes["monitoring"]);

            // element: categoryValueRef -> CategoryValue categoryValueRef (0, *)
            FillElements(node.ChildNodes["categoryValueRef"], result.CategoryValueRef);

            // element: dataState -> DataState dataState (0, 1)
            result.DataState = FillElement<DataState>(node.ChildNodes["dataState"]);

            return result;
        }


        private InputOutputBinding LoadInputOutputBinding(XmlParserComplexNode node)
        {
             var result = GetOrCreate<InputOutputBinding>(node);

            // required: inputDataRef -> InputSet inputDataRef (1, 1)
            var _inputDataRefAttribute = node.Attributes["inputDataRef"]?.ProcessedValue;
            if (_inputDataRefAttribute is null) throw new BPMNCheckerExceptions($"Node {node.ID} ({(string.IsNullOrWhiteSpace(node.Type?.Name) ? node.Type?.Name : "")}) is missing required attribute inputDataRef");
            result.InputDataRef = Load<InputSet>((XmlParserComplexNode)_inputDataRefAttribute);

            // required: outputDataRef -> OutputSet outputDataRef (1, 1)
            var _outputDataRefAttribute = node.Attributes["outputDataRef"]?.ProcessedValue;
            if (_outputDataRefAttribute is null) throw new BPMNCheckerExceptions($"Node {node.ID} ({(string.IsNullOrWhiteSpace(node.Type?.Name) ? node.Type?.Name : "")}) is missing required attribute outputDataRef");
            result.OutputDataRef = Load<OutputSet>((XmlParserComplexNode)_outputDataRefAttribute);

            // required: operationRef -> Operation operationRef (1, 1)
            var _operationRefAttribute = node.Attributes["operationRef"]?.ProcessedValue;
            if (_operationRefAttribute is null) throw new BPMNCheckerExceptions($"Node {node.ID} ({(string.IsNullOrWhiteSpace(node.Type?.Name) ? node.Type?.Name : "")}) is missing required attribute operationRef");
            result.OperationRef = Load<Operation>((XmlParserComplexNode)_operationRefAttribute);

            return result;
        }


        private Assignment LoadAssignment(XmlParserComplexNode node)
        {
             var result = GetOrCreate<Assignment>(node);

            // optional: id -> string id (1, 1)
            var _idAttribute = node.Attributes.ContainsKey("id") ? node.Attributes["id"].ProcessedValue : null;
            if (_idAttribute is not null) result.Id = (string)_idAttribute;

            // element: documentation -> Documentation documentation (0, *)
            FillElements(node.ChildNodes["documentation"], result.Documentation);

            // element: from -> Expression from (1, 1)
            result.From = FillElement<Expression>(node.ChildNodes["from"]);

            // element: to -> Expression to (1, 1)
            result.To = FillElement<Expression>(node.ChildNodes["to"]);

            return result;
        }


        private DataStore LoadDataStore(XmlParserComplexNode node)
        {
             var result = GetOrCreate<DataStore>(node);

            // optional: id -> string id (1, 1)
            var _idAttribute = node.Attributes.ContainsKey("id") ? node.Attributes["id"].ProcessedValue : null;
            if (_idAttribute is not null) result.Id = (string)_idAttribute;

            // optional: itemSubjectRef -> ItemDefinition itemSubjectRef (0, 1)
            var _itemSubjectRefAttribute = node.Attributes.ContainsKey("itemSubjectRef") ? node.Attributes["itemSubjectRef"].ProcessedValue : null;
            if (_itemSubjectRefAttribute is not null) result.ItemSubjectRef = Load<ItemDefinition>((XmlParserComplexNode)_itemSubjectRefAttribute);

            // optional: name -> string name (1, 1)
            var _nameAttribute = node.Attributes.ContainsKey("name") ? node.Attributes["name"].ProcessedValue : null;
            if (_nameAttribute is not null) result.Name = (string)_nameAttribute;

            // optional: capacity -> int capacity (1, 1)
            var _capacityAttribute = node.Attributes.ContainsKey("capacity") ? node.Attributes["capacity"].ProcessedValue : null;
            if (_capacityAttribute is not null) result.Capacity = (int)_capacityAttribute;

            // optional: isUnlimited -> bool isUnlimited (1, 1)
            var _isUnlimitedAttribute = node.Attributes.ContainsKey("isUnlimited") ? node.Attributes["isUnlimited"].ProcessedValue : null;
            if (_isUnlimitedAttribute is not null) result.IsUnlimited = (bool)_isUnlimitedAttribute;

            // element: documentation -> Documentation documentation (0, *)
            FillElements(node.ChildNodes["documentation"], result.Documentation);

            // element: dataState -> DataState dataState (0, 1)
            result.DataState = FillElement<DataState>(node.ChildNodes["dataState"]);

            return result;
        }


        private DataStoreReference LoadDataStoreReference(XmlParserComplexNode node)
        {
             var result = GetOrCreate<DataStoreReference>(node);

            // optional: id -> string id (1, 1)
            var _idAttribute = node.Attributes.ContainsKey("id") ? node.Attributes["id"].ProcessedValue : null;
            if (_idAttribute is not null) result.Id = (string)_idAttribute;

            // optional: itemSubjectRef -> ItemDefinition itemSubjectRef (0, 1)
            var _itemSubjectRefAttribute = node.Attributes.ContainsKey("itemSubjectRef") ? node.Attributes["itemSubjectRef"].ProcessedValue : null;
            if (_itemSubjectRefAttribute is not null) result.ItemSubjectRef = Load<ItemDefinition>((XmlParserComplexNode)_itemSubjectRefAttribute);

            // optional: name -> string name (1, 1)
            var _nameAttribute = node.Attributes.ContainsKey("name") ? node.Attributes["name"].ProcessedValue : null;
            if (_nameAttribute is not null) result.Name = (string)_nameAttribute;

            // optional: dataStoreRef -> DataStore dataStoreRef (0, 1)
            var _dataStoreRefAttribute = node.Attributes.ContainsKey("dataStoreRef") ? node.Attributes["dataStoreRef"].ProcessedValue : null;
            if (_dataStoreRefAttribute is not null) result.DataStoreRef = Load<DataStore>((XmlParserComplexNode)_dataStoreRefAttribute);

            // element: documentation -> Documentation documentation (0, *)
            FillElements(node.ChildNodes["documentation"], result.Documentation);

            // element: dataState -> DataState dataState (0, 1)
            result.DataState = FillElement<DataState>(node.ChildNodes["dataState"]);

            // element: auditing -> Auditing auditing (0, 1)
            result.Auditing = FillElement<Auditing>(node.ChildNodes["auditing"]);

            // element: monitoring -> Monitoring monitoring (0, 1)
            result.Monitoring = FillElement<Monitoring>(node.ChildNodes["monitoring"]);

            // element: categoryValueRef -> CategoryValue categoryValueRef (0, *)
            FillElements(node.ChildNodes["categoryValueRef"], result.CategoryValueRef);

            return result;
        }


        private DataObjectReference LoadDataObjectReference(XmlParserComplexNode node)
        {
             var result = GetOrCreate<DataObjectReference>(node);

            // optional: id -> string id (1, 1)
            var _idAttribute = node.Attributes.ContainsKey("id") ? node.Attributes["id"].ProcessedValue : null;
            if (_idAttribute is not null) result.Id = (string)_idAttribute;

            // optional: itemSubjectRef -> ItemDefinition itemSubjectRef (0, 1)
            var _itemSubjectRefAttribute = node.Attributes.ContainsKey("itemSubjectRef") ? node.Attributes["itemSubjectRef"].ProcessedValue : null;
            if (_itemSubjectRefAttribute is not null) result.ItemSubjectRef = Load<ItemDefinition>((XmlParserComplexNode)_itemSubjectRefAttribute);

            // optional: name -> string name (1, 1)
            var _nameAttribute = node.Attributes.ContainsKey("name") ? node.Attributes["name"].ProcessedValue : null;
            if (_nameAttribute is not null) result.Name = (string)_nameAttribute;

            // optional: dataObjectRef -> DataObject dataObjectRef (1, 1)
            var _dataObjectRefAttribute = node.Attributes.ContainsKey("dataObjectRef") ? node.Attributes["dataObjectRef"].ProcessedValue : null;
            if (_dataObjectRefAttribute is not null) result.DataObjectRef = Load<DataObject>((XmlParserComplexNode)_dataObjectRefAttribute);

            // element: documentation -> Documentation documentation (0, *)
            FillElements(node.ChildNodes["documentation"], result.Documentation);

            // element: dataState -> DataState dataState (0, 1)
            result.DataState = FillElement<DataState>(node.ChildNodes["dataState"]);

            // element: auditing -> Auditing auditing (0, 1)
            result.Auditing = FillElement<Auditing>(node.ChildNodes["auditing"]);

            // element: monitoring -> Monitoring monitoring (0, 1)
            result.Monitoring = FillElement<Monitoring>(node.ChildNodes["monitoring"]);

            // element: categoryValueRef -> CategoryValue categoryValueRef (0, *)
            FillElements(node.ChildNodes["categoryValueRef"], result.CategoryValueRef);

            return result;
        }


        private ConversationLink LoadConversationLink(XmlParserComplexNode node)
        {
             var result = GetOrCreate<ConversationLink>(node);

            // required: sourceRef -> InteractionNode sourceRef (1, 1)
            var _sourceRefAttribute = node.Attributes["sourceRef"]?.ProcessedValue;
            if (_sourceRefAttribute is null) throw new BPMNCheckerExceptions($"Node {node.ID} ({(string.IsNullOrWhiteSpace(node.Type?.Name) ? node.Type?.Name : "")}) is missing required attribute sourceRef");
            result.SourceRef = Load<InteractionNode>((XmlParserComplexNode)_sourceRefAttribute);

            // required: targetRef -> InteractionNode targetRef (1, 1)
            var _targetRefAttribute = node.Attributes["targetRef"]?.ProcessedValue;
            if (_targetRefAttribute is null) throw new BPMNCheckerExceptions($"Node {node.ID} ({(string.IsNullOrWhiteSpace(node.Type?.Name) ? node.Type?.Name : "")}) is missing required attribute targetRef");
            result.TargetRef = Load<InteractionNode>((XmlParserComplexNode)_targetRefAttribute);

            // optional: id -> string id (1, 1)
            var _idAttribute = node.Attributes.ContainsKey("id") ? node.Attributes["id"].ProcessedValue : null;
            if (_idAttribute is not null) result.Id = (string)_idAttribute;

            // optional: name -> string name (0, 1)
            var _nameAttribute = node.Attributes.ContainsKey("name") ? node.Attributes["name"].ProcessedValue : null;
            if (_nameAttribute is not null) result.Name = (string)_nameAttribute;

            // element: documentation -> Documentation documentation (0, *)
            FillElements(node.ChildNodes["documentation"], result.Documentation);

            return result;
        }


        private ConversationAssociation LoadConversationAssociation(XmlParserComplexNode node)
        {
             var result = GetOrCreate<ConversationAssociation>(node);

            // required: innerConversationNodeRef -> ConversationNode innerConversationNodeRef (1, 1)
            var _innerConversationNodeRefAttribute = node.Attributes["innerConversationNodeRef"]?.ProcessedValue;
            if (_innerConversationNodeRefAttribute is null) throw new BPMNCheckerExceptions($"Node {node.ID} ({(string.IsNullOrWhiteSpace(node.Type?.Name) ? node.Type?.Name : "")}) is missing required attribute innerConversationNodeRef");
            result.InnerConversationNodeRef = Load<ConversationNode>((XmlParserComplexNode)_innerConversationNodeRefAttribute);

            // required: outerConversationNodeRef -> ConversationNode outerConversationNodeRef (1, 1)
            var _outerConversationNodeRefAttribute = node.Attributes["outerConversationNodeRef"]?.ProcessedValue;
            if (_outerConversationNodeRefAttribute is null) throw new BPMNCheckerExceptions($"Node {node.ID} ({(string.IsNullOrWhiteSpace(node.Type?.Name) ? node.Type?.Name : "")}) is missing required attribute outerConversationNodeRef");
            result.OuterConversationNodeRef = Load<ConversationNode>((XmlParserComplexNode)_outerConversationNodeRefAttribute);

            // optional: id -> string id (1, 1)
            var _idAttribute = node.Attributes.ContainsKey("id") ? node.Attributes["id"].ProcessedValue : null;
            if (_idAttribute is not null) result.Id = (string)_idAttribute;

            // element: documentation -> Documentation documentation (0, *)
            FillElements(node.ChildNodes["documentation"], result.Documentation);

            return result;
        }


        private CallConversation LoadCallConversation(XmlParserComplexNode node)
        {
             var result = GetOrCreate<CallConversation>(node);

            // optional: id -> string id (1, 1)
            var _idAttribute = node.Attributes.ContainsKey("id") ? node.Attributes["id"].ProcessedValue : null;
            if (_idAttribute is not null) result.Id = (string)_idAttribute;

            // optional: name -> string name (1, 1)
            var _nameAttribute = node.Attributes.ContainsKey("name") ? node.Attributes["name"].ProcessedValue : null;
            if (_nameAttribute is not null) result.Name = (string)_nameAttribute;

            // optional: calledCollaborationRef -> Collaboration calledCollaborationRef (0, 1)
            var _calledCollaborationRefAttribute = node.Attributes.ContainsKey("calledCollaborationRef") ? node.Attributes["calledCollaborationRef"].ProcessedValue : null;
            if (_calledCollaborationRefAttribute is not null) result.CalledCollaborationRef = Load<Collaboration>((XmlParserComplexNode)_calledCollaborationRefAttribute);

            // element: documentation -> Documentation documentation (0, *)
            FillElements(node.ChildNodes["documentation"], result.Documentation);

            // element: participantRef -> Participant participantRefs (2, *)
            FillElements(node.ChildNodes["participantRef"], result.ParticipantRefs);

            // element: messageFlowRef -> MessageFlow messageFlowRefs (0, *)
            FillElements(node.ChildNodes["messageFlowRef"], result.MessageFlowRefs);

            // element: correlationKey -> CorrelationKey correlationKeys (0, *)
            FillElements(node.ChildNodes["correlationKey"], result.CorrelationKeys);

            // element: participantAssociation -> ParticipantAssociation participantAssociations (0, *)
            FillElements(node.ChildNodes["participantAssociation"], result.ParticipantAssociations);

            // link back: ConversationLink incomingConversationLinks (0, *)
            // incomingConversationLinks 1 <cmof_model.M_Two_Way_Association object at 0x000002941E4ADD40>
            // target: ConversationLink.targetRef

            // link back: ConversationLink outgoingConversationLinks (0, *)
            // outgoingConversationLinks 1 <cmof_model.M_Two_Way_Association object at 0x000002941E4ADCC0>
            // target: ConversationLink.sourceRef

            return result;
        }


        private Conversation LoadConversation(XmlParserComplexNode node)
        {
             var result = GetOrCreate<Conversation>(node);

            // optional: id -> string id (1, 1)
            var _idAttribute = node.Attributes.ContainsKey("id") ? node.Attributes["id"].ProcessedValue : null;
            if (_idAttribute is not null) result.Id = (string)_idAttribute;

            // optional: name -> string name (1, 1)
            var _nameAttribute = node.Attributes.ContainsKey("name") ? node.Attributes["name"].ProcessedValue : null;
            if (_nameAttribute is not null) result.Name = (string)_nameAttribute;

            // element: documentation -> Documentation documentation (0, *)
            FillElements(node.ChildNodes["documentation"], result.Documentation);

            // element: participantRef -> Participant participantRefs (2, *)
            FillElements(node.ChildNodes["participantRef"], result.ParticipantRefs);

            // element: messageFlowRef -> MessageFlow messageFlowRefs (0, *)
            FillElements(node.ChildNodes["messageFlowRef"], result.MessageFlowRefs);

            // element: correlationKey -> CorrelationKey correlationKeys (0, *)
            FillElements(node.ChildNodes["correlationKey"], result.CorrelationKeys);

            // link back: ConversationLink incomingConversationLinks (0, *)
            // incomingConversationLinks 1 <cmof_model.M_Two_Way_Association object at 0x000002941E4ADD40>
            // target: ConversationLink.targetRef

            // link back: ConversationLink outgoingConversationLinks (0, *)
            // outgoingConversationLinks 1 <cmof_model.M_Two_Way_Association object at 0x000002941E4ADCC0>
            // target: ConversationLink.sourceRef

            return result;
        }


        private SubConversation LoadSubConversation(XmlParserComplexNode node)
        {
             var result = GetOrCreate<SubConversation>(node);

            // optional: id -> string id (1, 1)
            var _idAttribute = node.Attributes.ContainsKey("id") ? node.Attributes["id"].ProcessedValue : null;
            if (_idAttribute is not null) result.Id = (string)_idAttribute;

            // optional: name -> string name (1, 1)
            var _nameAttribute = node.Attributes.ContainsKey("name") ? node.Attributes["name"].ProcessedValue : null;
            if (_nameAttribute is not null) result.Name = (string)_nameAttribute;

            // element: documentation -> Documentation documentation (0, *)
            FillElements(node.ChildNodes["documentation"], result.Documentation);

            // element: participantRef -> Participant participantRefs (2, *)
            FillElements(node.ChildNodes["participantRef"], result.ParticipantRefs);

            // element: messageFlowRef -> MessageFlow messageFlowRefs (0, *)
            FillElements(node.ChildNodes["messageFlowRef"], result.MessageFlowRefs);

            // element: correlationKey -> CorrelationKey correlationKeys (0, *)
            FillElements(node.ChildNodes["correlationKey"], result.CorrelationKeys);

            // element: conversationNode -> ConversationNode conversationNodes (0, *)
            FillElements(node.ChildNodes["conversationNode"], result.ConversationNodes);

            // link back: ConversationLink incomingConversationLinks (0, *)
            // incomingConversationLinks 1 <cmof_model.M_Two_Way_Association object at 0x000002941E4ADD40>
            // target: ConversationLink.targetRef

            // link back: ConversationLink outgoingConversationLinks (0, *)
            // outgoingConversationLinks 1 <cmof_model.M_Two_Way_Association object at 0x000002941E4ADCC0>
            // target: ConversationLink.sourceRef

            return result;
        }



        private GlobalConversation LoadGlobalConversation(XmlParserComplexNode node)
        {
             var result = GetOrCreate<GlobalConversation>(node);

            // optional: id -> string id (1, 1)
            var _idAttribute = node.Attributes.ContainsKey("id") ? node.Attributes["id"].ProcessedValue : null;
            if (_idAttribute is not null) result.Id = (string)_idAttribute;

            // optional: name -> string name (1, 1)
            var _nameAttribute = node.Attributes.ContainsKey("name") ? node.Attributes["name"].ProcessedValue : null;
            if (_nameAttribute is not null) result.Name = (string)_nameAttribute;

            // optional: isClosed -> bool isClosed (1, 1)
            var _isClosedAttribute = node.Attributes.ContainsKey("isClosed") ? node.Attributes["isClosed"].ProcessedValue : null;
            if (_isClosedAttribute is not null) result.IsClosed = (bool)_isClosedAttribute;

            // element: documentation -> Documentation documentation (0, *)
            FillElements(node.ChildNodes["documentation"], result.Documentation);

            // element: choreographyRef -> Choreography choreographyRef (0, *)
            FillElements(node.ChildNodes["choreographyRef"], result.ChoreographyRef);

            // element: artifact -> Artifact artifacts (0, *)
            FillElements(node.ChildNodes["artifact"], result.Artifacts);

            // element: participantAssociation -> ParticipantAssociation participantAssociations (0, *)
            FillElements(node.ChildNodes["participantAssociation"], result.ParticipantAssociations);

            // element: messageFlowAssociation -> MessageFlowAssociation messageFlowAssociations (0, *)
            FillElements(node.ChildNodes["messageFlowAssociation"], result.MessageFlowAssociations);

            // element: conversationAssociation -> ConversationAssociation conversationAssociations (1, 1)
            result.ConversationAssociations = FillElement<ConversationAssociation>(node.ChildNodes["conversationAssociation"]);

            // element: participant -> Participant participants (0, *)
            FillElements(node.ChildNodes["participant"], result.Participants);

            // element: messageFlow -> MessageFlow messageFlows (0, *)
            FillElements(node.ChildNodes["messageFlow"], result.MessageFlows);

            // element: correlationKey -> CorrelationKey correlationKeys (0, *)
            FillElements(node.ChildNodes["correlationKey"], result.CorrelationKeys);

            // element: conversationNode -> ConversationNode conversations (0, *)
            FillElements(node.ChildNodes["conversationNode"], result.Conversations);

            // element: conversationLink -> ConversationLink conversationLinks (0, *)
            FillElements(node.ChildNodes["conversationLink"], result.ConversationLinks);

            return result;
        }


        private PartnerEntity LoadPartnerEntity(XmlParserComplexNode node)
        {
             var result = GetOrCreate<PartnerEntity>(node);

            // optional: id -> string id (1, 1)
            var _idAttribute = node.Attributes.ContainsKey("id") ? node.Attributes["id"].ProcessedValue : null;
            if (_idAttribute is not null) result.Id = (string)_idAttribute;

            // optional: name -> string name (1, 1)
            var _nameAttribute = node.Attributes.ContainsKey("name") ? node.Attributes["name"].ProcessedValue : null;
            if (_nameAttribute is not null) result.Name = (string)_nameAttribute;

            // element: documentation -> Documentation documentation (0, *)
            FillElements(node.ChildNodes["documentation"], result.Documentation);

            // element: participantRef -> Participant participantRef (0, *)
            FillElements(node.ChildNodes["participantRef"], result.ParticipantRef);

            return result;
        }


        private PartnerRole LoadPartnerRole(XmlParserComplexNode node)
        {
             var result = GetOrCreate<PartnerRole>(node);

            // optional: id -> string id (1, 1)
            var _idAttribute = node.Attributes.ContainsKey("id") ? node.Attributes["id"].ProcessedValue : null;
            if (_idAttribute is not null) result.Id = (string)_idAttribute;

            // optional: name -> string name (1, 1)
            var _nameAttribute = node.Attributes.ContainsKey("name") ? node.Attributes["name"].ProcessedValue : null;
            if (_nameAttribute is not null) result.Name = (string)_nameAttribute;

            // element: documentation -> Documentation documentation (0, *)
            FillElements(node.ChildNodes["documentation"], result.Documentation);

            // element: participantRef -> Participant participantRef (0, *)
            FillElements(node.ChildNodes["participantRef"], result.ParticipantRef);

            return result;
        }


        private CorrelationProperty LoadCorrelationProperty(XmlParserComplexNode node)
        {
             var result = GetOrCreate<CorrelationProperty>(node);

            // optional: id -> string id (1, 1)
            var _idAttribute = node.Attributes.ContainsKey("id") ? node.Attributes["id"].ProcessedValue : null;
            if (_idAttribute is not null) result.Id = (string)_idAttribute;

            // optional: name -> string name (1, 1)
            var _nameAttribute = node.Attributes.ContainsKey("name") ? node.Attributes["name"].ProcessedValue : null;
            if (_nameAttribute is not null) result.Name = (string)_nameAttribute;

            // optional: type -> ItemDefinition type (0, 1)
            var _typeAttribute = node.Attributes.ContainsKey("type") ? node.Attributes["type"].ProcessedValue : null;
            if (_typeAttribute is not null) result.Type = Load<ItemDefinition>((XmlParserComplexNode)_typeAttribute);

            // element: documentation -> Documentation documentation (0, *)
            FillElements(node.ChildNodes["documentation"], result.Documentation);

            // element: correlationPropertyRetrievalExpression -> CorrelationPropertyRetrievalExpression correlationPropertyRetrievalExpression (1, *)
            FillElements(node.ChildNodes["correlationPropertyRetrievalExpression"], result.CorrelationPropertyRetrievalExpression);

            return result;
        }


        private Error LoadError(XmlParserComplexNode node)
        {
             var result = GetOrCreate<Error>(node);

            // optional: id -> string id (1, 1)
            var _idAttribute = node.Attributes.ContainsKey("id") ? node.Attributes["id"].ProcessedValue : null;
            if (_idAttribute is not null) result.Id = (string)_idAttribute;

            // optional: structureRef -> ItemDefinition structureRef (0, 1)
            var _structureRefAttribute = node.Attributes.ContainsKey("structureRef") ? node.Attributes["structureRef"].ProcessedValue : null;
            if (_structureRefAttribute is not null) result.StructureRef = Load<ItemDefinition>((XmlParserComplexNode)_structureRefAttribute);

            // optional: name -> string name (1, 1)
            var _nameAttribute = node.Attributes.ContainsKey("name") ? node.Attributes["name"].ProcessedValue : null;
            if (_nameAttribute is not null) result.Name = (string)_nameAttribute;

            // optional: errorCode -> string errorCode (1, 1)
            var _errorCodeAttribute = node.Attributes.ContainsKey("errorCode") ? node.Attributes["errorCode"].ProcessedValue : null;
            if (_errorCodeAttribute is not null) result.ErrorCode = (string)_errorCodeAttribute;

            // element: documentation -> Documentation documentation (0, *)
            FillElements(node.ChildNodes["documentation"], result.Documentation);

            return result;
        }


        private CorrelationKey LoadCorrelationKey(XmlParserComplexNode node)
        {
             var result = GetOrCreate<CorrelationKey>(node);

            // optional: id -> string id (1, 1)
            var _idAttribute = node.Attributes.ContainsKey("id") ? node.Attributes["id"].ProcessedValue : null;
            if (_idAttribute is not null) result.Id = (string)_idAttribute;

            // optional: name -> string name (1, 1)
            var _nameAttribute = node.Attributes.ContainsKey("name") ? node.Attributes["name"].ProcessedValue : null;
            if (_nameAttribute is not null) result.Name = (string)_nameAttribute;

            // element: documentation -> Documentation documentation (0, *)
            FillElements(node.ChildNodes["documentation"], result.Documentation);

            // element: correlationPropertyRef -> CorrelationProperty correlationPropertyRef (0, *)
            FillElements(node.ChildNodes["correlationPropertyRef"], result.CorrelationPropertyRef);

            return result;
        }


        private Expression LoadExpression(XmlParserComplexNode node)
        {
             var result = GetOrCreate<Expression>(node);

            // optional: id -> string id (1, 1)
            var _idAttribute = node.Attributes.ContainsKey("id") ? node.Attributes["id"].ProcessedValue : null;
            if (_idAttribute is not null) result.Id = (string)_idAttribute;

            // element: documentation -> Documentation documentation (0, *)
            FillElements(node.ChildNodes["documentation"], result.Documentation);

            return result;
        }


        private FormalExpression LoadFormalExpression(XmlParserComplexNode node)
        {
             var result = GetOrCreate<FormalExpression>(node);

            // optional: id -> string id (1, 1)
            var _idAttribute = node.Attributes.ContainsKey("id") ? node.Attributes["id"].ProcessedValue : null;
            if (_idAttribute is not null) result.Id = (string)_idAttribute;

            // optional: language -> string language (1, 1)
            var _languageAttribute = node.Attributes.ContainsKey("language") ? node.Attributes["language"].ProcessedValue : null;
            if (_languageAttribute is not null) result.Language = (string)_languageAttribute;

            // optional: evaluatesToTypeRef -> ItemDefinition evaluatesToTypeRef (1, 1)
            var _evaluatesToTypeRefAttribute = node.Attributes.ContainsKey("evaluatesToTypeRef") ? node.Attributes["evaluatesToTypeRef"].ProcessedValue : null;
            if (_evaluatesToTypeRefAttribute is not null) result.EvaluatesToTypeRef = Load<ItemDefinition>((XmlParserComplexNode)_evaluatesToTypeRefAttribute);

            // element: documentation -> Documentation documentation (0, *)
            FillElements(node.ChildNodes["documentation"], result.Documentation);

            // missing: Element body (1, 1)

            return result;
        }


        private Message LoadMessage(XmlParserComplexNode node)
        {
             var result = GetOrCreate<Message>(node);

            // optional: id -> string id (1, 1)
            var _idAttribute = node.Attributes.ContainsKey("id") ? node.Attributes["id"].ProcessedValue : null;
            if (_idAttribute is not null) result.Id = (string)_idAttribute;

            // optional: name -> string name (1, 1)
            var _nameAttribute = node.Attributes.ContainsKey("name") ? node.Attributes["name"].ProcessedValue : null;
            if (_nameAttribute is not null) result.Name = (string)_nameAttribute;

            // optional: itemRef -> ItemDefinition itemRef (0, 1)
            var _itemRefAttribute = node.Attributes.ContainsKey("itemRef") ? node.Attributes["itemRef"].ProcessedValue : null;
            if (_itemRefAttribute is not null) result.ItemRef = Load<ItemDefinition>((XmlParserComplexNode)_itemRefAttribute);

            // element: documentation -> Documentation documentation (0, *)
            FillElements(node.ChildNodes["documentation"], result.Documentation);

            return result;
        }


        private ItemDefinition LoadItemDefinition(XmlParserComplexNode node)
        {
             var result = GetOrCreate<ItemDefinition>(node);

            // optional: id -> string id (1, 1)
            var _idAttribute = node.Attributes.ContainsKey("id") ? node.Attributes["id"].ProcessedValue : null;
            if (_idAttribute is not null) result.Id = (string)_idAttribute;

            // optional: itemKind -> ItemKind itemKind (1, 1)
            var _itemKindAttribute = node.Attributes.ContainsKey("itemKind") ? node.Attributes["itemKind"].ProcessedValue : null;
            if (_itemKindAttribute is not null) result.ItemKind = CreateEnum<ItemKind>((string)_itemKindAttribute);

            // optional: structureRef -> Element structureRef (1, 1)
            var _structureRefAttribute = node.Attributes.ContainsKey("structureRef") ? node.Attributes["structureRef"].ProcessedValue : null;
            if (_structureRefAttribute is not null) result.StructureRef = Load<Element>((XmlParserComplexNode)_structureRefAttribute);

            // optional: isCollection -> bool isCollection (1, 1)
            var _isCollectionAttribute = node.Attributes.ContainsKey("isCollection") ? node.Attributes["isCollection"].ProcessedValue : null;
            if (_isCollectionAttribute is not null) result.IsCollection = (bool)_isCollectionAttribute;

            // element: documentation -> Documentation documentation (0, *)
            FillElements(node.ChildNodes["documentation"], result.Documentation);

            // empty: Import import (0, 1)

            return result;
        }



        private SequenceFlow LoadSequenceFlow(XmlParserComplexNode node)
        {
             var result = GetOrCreate<SequenceFlow>(node);

            // required: sourceRef -> FlowNode sourceRef (1, 1)
            var _sourceRefAttribute = node.Attributes["sourceRef"]?.ProcessedValue;
            if (_sourceRefAttribute is null) throw new BPMNCheckerExceptions($"Node {node.ID} ({(string.IsNullOrWhiteSpace(node.Type?.Name) ? node.Type?.Name : "")}) is missing required attribute sourceRef");
            result.SourceRef = Load<FlowNode>((XmlParserComplexNode)_sourceRefAttribute);

            // required: targetRef -> FlowNode targetRef (1, 1)
            var _targetRefAttribute = node.Attributes["targetRef"]?.ProcessedValue;
            if (_targetRefAttribute is null) throw new BPMNCheckerExceptions($"Node {node.ID} ({(string.IsNullOrWhiteSpace(node.Type?.Name) ? node.Type?.Name : "")}) is missing required attribute targetRef");
            result.TargetRef = Load<FlowNode>((XmlParserComplexNode)_targetRefAttribute);

            // optional: id -> string id (1, 1)
            var _idAttribute = node.Attributes.ContainsKey("id") ? node.Attributes["id"].ProcessedValue : null;
            if (_idAttribute is not null) result.Id = (string)_idAttribute;

            // optional: name -> string name (1, 1)
            var _nameAttribute = node.Attributes.ContainsKey("name") ? node.Attributes["name"].ProcessedValue : null;
            if (_nameAttribute is not null) result.Name = (string)_nameAttribute;

            // optional: isImmediate -> bool isImmediate (0, 1)
            var _isImmediateAttribute = node.Attributes.ContainsKey("isImmediate") ? node.Attributes["isImmediate"].ProcessedValue : null;
            if (_isImmediateAttribute is not null) result.IsImmediate = (bool)_isImmediateAttribute;

            // element: documentation -> Documentation documentation (0, *)
            FillElements(node.ChildNodes["documentation"], result.Documentation);

            // element: auditing -> Auditing auditing (0, 1)
            result.Auditing = FillElement<Auditing>(node.ChildNodes["auditing"]);

            // element: monitoring -> Monitoring monitoring (0, 1)
            result.Monitoring = FillElement<Monitoring>(node.ChildNodes["monitoring"]);

            // element: categoryValueRef -> CategoryValue categoryValueRef (0, *)
            FillElements(node.ChildNodes["categoryValueRef"], result.CategoryValueRef);

            // element: conditionExpression -> Expression conditionExpression (0, 1)
            result.ConditionExpression = FillElement<Expression>(node.ChildNodes["conditionExpression"]);

            return result;
        }





        private CorrelationPropertyRetrievalExpression LoadCorrelationPropertyRetrievalExpression(XmlParserComplexNode node)
        {
             var result = GetOrCreate<CorrelationPropertyRetrievalExpression>(node);

            // required: messageRef -> Message messageRef (1, 1)
            var _messageRefAttribute = node.Attributes["messageRef"]?.ProcessedValue;
            if (_messageRefAttribute is null) throw new BPMNCheckerExceptions($"Node {node.ID} ({(string.IsNullOrWhiteSpace(node.Type?.Name) ? node.Type?.Name : "")}) is missing required attribute messageRef");
            result.MessageRef = Load<Message>((XmlParserComplexNode)_messageRefAttribute);

            // optional: id -> string id (1, 1)
            var _idAttribute = node.Attributes.ContainsKey("id") ? node.Attributes["id"].ProcessedValue : null;
            if (_idAttribute is not null) result.Id = (string)_idAttribute;

            // element: documentation -> Documentation documentation (0, *)
            FillElements(node.ChildNodes["documentation"], result.Documentation);

            // element: messagePath -> FormalExpression messagePath (1, 1)
            result.MessagePath = FillElement<FormalExpression>(node.ChildNodes["messagePath"]);

            return result;
        }


        private CorrelationPropertyBinding LoadCorrelationPropertyBinding(XmlParserComplexNode node)
        {
             var result = GetOrCreate<CorrelationPropertyBinding>(node);

            // required: correlationPropertyRef -> CorrelationProperty correlationPropertyRef (1, 1)
            var _correlationPropertyRefAttribute = node.Attributes["correlationPropertyRef"]?.ProcessedValue;
            if (_correlationPropertyRefAttribute is null) throw new BPMNCheckerExceptions($"Node {node.ID} ({(string.IsNullOrWhiteSpace(node.Type?.Name) ? node.Type?.Name : "")}) is missing required attribute correlationPropertyRef");
            result.CorrelationPropertyRef = Load<CorrelationProperty>((XmlParserComplexNode)_correlationPropertyRefAttribute);

            // optional: id -> string id (1, 1)
            var _idAttribute = node.Attributes.ContainsKey("id") ? node.Attributes["id"].ProcessedValue : null;
            if (_idAttribute is not null) result.Id = (string)_idAttribute;

            // element: documentation -> Documentation documentation (0, *)
            FillElements(node.ChildNodes["documentation"], result.Documentation);

            // element: dataPath -> FormalExpression dataPath (1, 1)
            result.DataPath = FillElement<FormalExpression>(node.ChildNodes["dataPath"]);

            return result;
        }


        private Resource LoadResource(XmlParserComplexNode node)
        {
             var result = GetOrCreate<Resource>(node);

            // required: name -> string name (1, 1)
            var _nameAttribute = node.Attributes["name"]?.ProcessedValue;
            if (_nameAttribute is null) throw new BPMNCheckerExceptions($"Node {node.ID} ({(string.IsNullOrWhiteSpace(node.Type?.Name) ? node.Type?.Name : "")}) is missing required attribute name");
            result.Name = (string)_nameAttribute;

            // optional: id -> string id (1, 1)
            var _idAttribute = node.Attributes.ContainsKey("id") ? node.Attributes["id"].ProcessedValue : null;
            if (_idAttribute is not null) result.Id = (string)_idAttribute;

            // element: documentation -> Documentation documentation (0, *)
            FillElements(node.ChildNodes["documentation"], result.Documentation);

            // element: resourceParameter -> ResourceParameter resourceParameters (0, *)
            FillElements(node.ChildNodes["resourceParameter"], result.ResourceParameters);

            return result;
        }


        private ResourceParameter LoadResourceParameter(XmlParserComplexNode node)
        {
             var result = GetOrCreate<ResourceParameter>(node);

            // optional: id -> string id (1, 1)
            var _idAttribute = node.Attributes.ContainsKey("id") ? node.Attributes["id"].ProcessedValue : null;
            if (_idAttribute is not null) result.Id = (string)_idAttribute;

            // optional: name -> string name (1, 1)
            var _nameAttribute = node.Attributes.ContainsKey("name") ? node.Attributes["name"].ProcessedValue : null;
            if (_nameAttribute is not null) result.Name = (string)_nameAttribute;

            // optional: isRequired -> bool isRequired (1, 1)
            var _isRequiredAttribute = node.Attributes.ContainsKey("isRequired") ? node.Attributes["isRequired"].ProcessedValue : null;
            if (_isRequiredAttribute is not null) result.IsRequired = (bool)_isRequiredAttribute;

            // optional: type -> ItemDefinition type (0, 1)
            var _typeAttribute = node.Attributes.ContainsKey("type") ? node.Attributes["type"].ProcessedValue : null;
            if (_typeAttribute is not null) result.Type = Load<ItemDefinition>((XmlParserComplexNode)_typeAttribute);

            // element: documentation -> Documentation documentation (0, *)
            FillElements(node.ChildNodes["documentation"], result.Documentation);

            return result;
        }


        private CorrelationSubscription LoadCorrelationSubscription(XmlParserComplexNode node)
        {
             var result = GetOrCreate<CorrelationSubscription>(node);

            // required: correlationKeyRef -> CorrelationKey correlationKeyRef (1, 1)
            var _correlationKeyRefAttribute = node.Attributes["correlationKeyRef"]?.ProcessedValue;
            if (_correlationKeyRefAttribute is null) throw new BPMNCheckerExceptions($"Node {node.ID} ({(string.IsNullOrWhiteSpace(node.Type?.Name) ? node.Type?.Name : "")}) is missing required attribute correlationKeyRef");
            result.CorrelationKeyRef = Load<CorrelationKey>((XmlParserComplexNode)_correlationKeyRefAttribute);

            // optional: id -> string id (1, 1)
            var _idAttribute = node.Attributes.ContainsKey("id") ? node.Attributes["id"].ProcessedValue : null;
            if (_idAttribute is not null) result.Id = (string)_idAttribute;

            // element: documentation -> Documentation documentation (0, *)
            FillElements(node.ChildNodes["documentation"], result.Documentation);

            // element: correlationPropertyBinding -> CorrelationPropertyBinding correlationPropertyBinding (0, *)
            FillElements(node.ChildNodes["correlationPropertyBinding"], result.CorrelationPropertyBinding);

            return result;
        }


        private MessageFlow LoadMessageFlow(XmlParserComplexNode node)
        {
             var result = GetOrCreate<MessageFlow>(node);

            // required: sourceRef -> InteractionNode sourceRef (1, 1)
            var _sourceRefAttribute = node.Attributes["sourceRef"]?.ProcessedValue;
            if (_sourceRefAttribute is null) throw new BPMNCheckerExceptions($"Node {node.ID} ({(string.IsNullOrWhiteSpace(node.Type?.Name) ? node.Type?.Name : "")}) is missing required attribute sourceRef");
            result.SourceRef = Load<InteractionNode>((XmlParserComplexNode)_sourceRefAttribute);

            // required: targetRef -> InteractionNode targetRef (1, 1)
            var _targetRefAttribute = node.Attributes["targetRef"]?.ProcessedValue;
            if (_targetRefAttribute is null) throw new BPMNCheckerExceptions($"Node {node.ID} ({(string.IsNullOrWhiteSpace(node.Type?.Name) ? node.Type?.Name : "")}) is missing required attribute targetRef");
            result.TargetRef = Load<InteractionNode>((XmlParserComplexNode)_targetRefAttribute);

            // optional: id -> string id (1, 1)
            var _idAttribute = node.Attributes.ContainsKey("id") ? node.Attributes["id"].ProcessedValue : null;
            if (_idAttribute is not null) result.Id = (string)_idAttribute;

            // optional: name -> string name (1, 1)
            var _nameAttribute = node.Attributes.ContainsKey("name") ? node.Attributes["name"].ProcessedValue : null;
            if (_nameAttribute is not null) result.Name = (string)_nameAttribute;

            // optional: messageRef -> Message messageRef (0, 1)
            var _messageRefAttribute = node.Attributes.ContainsKey("messageRef") ? node.Attributes["messageRef"].ProcessedValue : null;
            if (_messageRefAttribute is not null) result.MessageRef = Load<Message>((XmlParserComplexNode)_messageRefAttribute);

            // element: documentation -> Documentation documentation (0, *)
            FillElements(node.ChildNodes["documentation"], result.Documentation);

            return result;
        }


        private MessageFlowAssociation LoadMessageFlowAssociation(XmlParserComplexNode node)
        {
             var result = GetOrCreate<MessageFlowAssociation>(node);

            // required: innerMessageFlowRef -> MessageFlow innerMessageFlowRef (1, 1)
            var _innerMessageFlowRefAttribute = node.Attributes["innerMessageFlowRef"]?.ProcessedValue;
            if (_innerMessageFlowRefAttribute is null) throw new BPMNCheckerExceptions($"Node {node.ID} ({(string.IsNullOrWhiteSpace(node.Type?.Name) ? node.Type?.Name : "")}) is missing required attribute innerMessageFlowRef");
            result.InnerMessageFlowRef = Load<MessageFlow>((XmlParserComplexNode)_innerMessageFlowRefAttribute);

            // required: outerMessageFlowRef -> MessageFlow outerMessageFlowRef (1, 1)
            var _outerMessageFlowRefAttribute = node.Attributes["outerMessageFlowRef"]?.ProcessedValue;
            if (_outerMessageFlowRefAttribute is null) throw new BPMNCheckerExceptions($"Node {node.ID} ({(string.IsNullOrWhiteSpace(node.Type?.Name) ? node.Type?.Name : "")}) is missing required attribute outerMessageFlowRef");
            result.OuterMessageFlowRef = Load<MessageFlow>((XmlParserComplexNode)_outerMessageFlowRefAttribute);

            // optional: id -> string id (1, 1)
            var _idAttribute = node.Attributes.ContainsKey("id") ? node.Attributes["id"].ProcessedValue : null;
            if (_idAttribute is not null) result.Id = (string)_idAttribute;

            // element: documentation -> Documentation documentation (0, *)
            FillElements(node.ChildNodes["documentation"], result.Documentation);

            return result;
        }



        private Participant LoadParticipant(XmlParserComplexNode node)
        {
             var result = GetOrCreate<Participant>(node);

            // optional: id -> string id (1, 1)
            var _idAttribute = node.Attributes.ContainsKey("id") ? node.Attributes["id"].ProcessedValue : null;
            if (_idAttribute is not null) result.Id = (string)_idAttribute;

            // optional: name -> string name (1, 1)
            var _nameAttribute = node.Attributes.ContainsKey("name") ? node.Attributes["name"].ProcessedValue : null;
            if (_nameAttribute is not null) result.Name = (string)_nameAttribute;

            // optional: processRef -> Process processRef (0, 1)
            var _processRefAttribute = node.Attributes.ContainsKey("processRef") ? node.Attributes["processRef"].ProcessedValue : null;
            if (_processRefAttribute is not null) result.ProcessRef = Load<Process>((XmlParserComplexNode)_processRefAttribute);

            // element: documentation -> Documentation documentation (0, *)
            FillElements(node.ChildNodes["documentation"], result.Documentation);

            // element: interfaceRef -> Interface interfaceRefs (0, *)
            FillElements(node.ChildNodes["interfaceRef"], result.InterfaceRefs);

            // element: participantMultiplicity -> ParticipantMultiplicity participantMultiplicity (0, 1)
            result.ParticipantMultiplicity = FillElement<ParticipantMultiplicity>(node.ChildNodes["participantMultiplicity"]);

            // element: endPointRef -> EndPoint endPointRefs (0, *)
            FillElements(node.ChildNodes["endPointRef"], result.EndPointRefs);

            // link back: ConversationLink incomingConversationLinks (0, *)
            // incomingConversationLinks 1 <cmof_model.M_Two_Way_Association object at 0x000002941E4ADD40>
            // target: ConversationLink.targetRef

            // link back: ConversationLink outgoingConversationLinks (0, *)
            // outgoingConversationLinks 1 <cmof_model.M_Two_Way_Association object at 0x000002941E4ADCC0>
            // target: ConversationLink.sourceRef

            return result;
        }


        private ParticipantAssociation LoadParticipantAssociation(XmlParserComplexNode node)
        {
             var result = GetOrCreate<ParticipantAssociation>(node);

            // optional: id -> string id (1, 1)
            var _idAttribute = node.Attributes.ContainsKey("id") ? node.Attributes["id"].ProcessedValue : null;
            if (_idAttribute is not null) result.Id = (string)_idAttribute;

            // element: documentation -> Documentation documentation (0, *)
            FillElements(node.ChildNodes["documentation"], result.Documentation);

            // element: innerParticipantRef -> Participant innerParticipantRef (1, 1)
            result.InnerParticipantRef = FillElement<Participant>(node.ChildNodes["innerParticipantRef"]);

            // element: outerParticipantRef -> Participant outerParticipantRef (1, 1)
            result.OuterParticipantRef = FillElement<Participant>(node.ChildNodes["outerParticipantRef"]);

            return result;
        }


        private ParticipantMultiplicity LoadParticipantMultiplicity(XmlParserComplexNode node)
        {
             var result = GetOrCreate<ParticipantMultiplicity>(node);

            // optional: minimum -> int minimum (1, 1)
            var _minimumAttribute = node.Attributes.ContainsKey("minimum") ? node.Attributes["minimum"].ProcessedValue : null;
            if (_minimumAttribute is not null) result.Minimum = (int)_minimumAttribute;

            // optional: maximum -> int maximum (0, 1)
            var _maximumAttribute = node.Attributes.ContainsKey("maximum") ? node.Attributes["maximum"].ProcessedValue : null;
            if (_maximumAttribute is not null) result.Maximum = (int)_maximumAttribute;

            return result;
        }


        private Collaboration LoadCollaboration(XmlParserComplexNode node)
        {
             var result = GetOrCreate<Collaboration>(node);

            // optional: id -> string id (1, 1)
            var _idAttribute = node.Attributes.ContainsKey("id") ? node.Attributes["id"].ProcessedValue : null;
            if (_idAttribute is not null) result.Id = (string)_idAttribute;

            // optional: name -> string name (1, 1)
            var _nameAttribute = node.Attributes.ContainsKey("name") ? node.Attributes["name"].ProcessedValue : null;
            if (_nameAttribute is not null) result.Name = (string)_nameAttribute;

            // optional: isClosed -> bool isClosed (1, 1)
            var _isClosedAttribute = node.Attributes.ContainsKey("isClosed") ? node.Attributes["isClosed"].ProcessedValue : null;
            if (_isClosedAttribute is not null) result.IsClosed = (bool)_isClosedAttribute;

            // element: documentation -> Documentation documentation (0, *)
            FillElements(node.ChildNodes["documentation"], result.Documentation);

            // element: choreographyRef -> Choreography choreographyRef (0, *)
            FillElements(node.ChildNodes["choreographyRef"], result.ChoreographyRef);

            // element: artifact -> Artifact artifacts (0, *)
            FillElements(node.ChildNodes["artifact"], result.Artifacts);

            // element: participantAssociation -> ParticipantAssociation participantAssociations (0, *)
            FillElements(node.ChildNodes["participantAssociation"], result.ParticipantAssociations);

            // element: messageFlowAssociation -> MessageFlowAssociation messageFlowAssociations (0, *)
            FillElements(node.ChildNodes["messageFlowAssociation"], result.MessageFlowAssociations);

            // element: conversationAssociation -> ConversationAssociation conversationAssociations (1, 1)
            result.ConversationAssociations = FillElement<ConversationAssociation>(node.ChildNodes["conversationAssociation"]);

            // element: participant -> Participant participants (0, *)
            FillElements(node.ChildNodes["participant"], result.Participants);

            // element: messageFlow -> MessageFlow messageFlows (0, *)
            FillElements(node.ChildNodes["messageFlow"], result.MessageFlows);

            // element: correlationKey -> CorrelationKey correlationKeys (0, *)
            FillElements(node.ChildNodes["correlationKey"], result.CorrelationKeys);

            // element: conversationNode -> ConversationNode conversations (0, *)
            FillElements(node.ChildNodes["conversationNode"], result.Conversations);

            // element: conversationLink -> ConversationLink conversationLinks (0, *)
            FillElements(node.ChildNodes["conversationLink"], result.ConversationLinks);

            return result;
        }



        private CallChoreography LoadCallChoreography(XmlParserComplexNode node)
        {
             var result = GetOrCreate<CallChoreography>(node);

            // required: initiatingParticipantRef -> Participant initiatingParticipantRef (1, 1)
            var _initiatingParticipantRefAttribute = node.Attributes["initiatingParticipantRef"]?.ProcessedValue;
            if (_initiatingParticipantRefAttribute is null) throw new BPMNCheckerExceptions($"Node {node.ID} ({(string.IsNullOrWhiteSpace(node.Type?.Name) ? node.Type?.Name : "")}) is missing required attribute initiatingParticipantRef");
            result.InitiatingParticipantRef = Load<Participant>((XmlParserComplexNode)_initiatingParticipantRefAttribute);

            // optional: id -> string id (1, 1)
            var _idAttribute = node.Attributes.ContainsKey("id") ? node.Attributes["id"].ProcessedValue : null;
            if (_idAttribute is not null) result.Id = (string)_idAttribute;

            // optional: name -> string name (1, 1)
            var _nameAttribute = node.Attributes.ContainsKey("name") ? node.Attributes["name"].ProcessedValue : null;
            if (_nameAttribute is not null) result.Name = (string)_nameAttribute;

            // optional: loopType -> ChoreographyLoopType loopType (1, 1)
            var _loopTypeAttribute = node.Attributes.ContainsKey("loopType") ? node.Attributes["loopType"].ProcessedValue : null;
            if (_loopTypeAttribute is not null) result.LoopType = CreateEnum<ChoreographyLoopType>((string)_loopTypeAttribute);

            // optional: calledChoreographyRef -> Choreography calledChoreographyRef (0, 1)
            var _calledChoreographyRefAttribute = node.Attributes.ContainsKey("calledChoreographyRef") ? node.Attributes["calledChoreographyRef"].ProcessedValue : null;
            if (_calledChoreographyRefAttribute is not null) result.CalledChoreographyRef = Load<Choreography>((XmlParserComplexNode)_calledChoreographyRefAttribute);

            // element: documentation -> Documentation documentation (0, *)
            FillElements(node.ChildNodes["documentation"], result.Documentation);

            // element: auditing -> Auditing auditing (0, 1)
            result.Auditing = FillElement<Auditing>(node.ChildNodes["auditing"]);

            // element: monitoring -> Monitoring monitoring (0, 1)
            result.Monitoring = FillElement<Monitoring>(node.ChildNodes["monitoring"]);

            // element: categoryValueRef -> CategoryValue categoryValueRef (0, *)
            FillElements(node.ChildNodes["categoryValueRef"], result.CategoryValueRef);

            // element: outgoing -> SequenceFlow outgoing (0, *)
            FillElements(node.ChildNodes["outgoing"], result.Outgoing);

            // element: incoming -> SequenceFlow incoming (0, *)
            FillElements(node.ChildNodes["incoming"], result.Incoming);

            // element: participantRef -> Participant participantRefs (2, *)
            FillElements(node.ChildNodes["participantRef"], result.ParticipantRefs);

            // element: correlationKey -> CorrelationKey correlationKeys (0, *)
            FillElements(node.ChildNodes["correlationKey"], result.CorrelationKeys);

            // element: participantAssociation -> ParticipantAssociation participantAssociations (0, *)
            FillElements(node.ChildNodes["participantAssociation"], result.ParticipantAssociations);

            // link back: Lane lanes (0, *)
            // lanes 1 <cmof_model.M_Two_Way_Association object at 0x000002941E4AD800>
            // target: Lane.flowNodeRefs

            return result;
        }


        private SubChoreography LoadSubChoreography(XmlParserComplexNode node)
        {
             var result = GetOrCreate<SubChoreography>(node);

            // required: initiatingParticipantRef -> Participant initiatingParticipantRef (1, 1)
            var _initiatingParticipantRefAttribute = node.Attributes["initiatingParticipantRef"]?.ProcessedValue;
            if (_initiatingParticipantRefAttribute is null) throw new BPMNCheckerExceptions($"Node {node.ID} ({(string.IsNullOrWhiteSpace(node.Type?.Name) ? node.Type?.Name : "")}) is missing required attribute initiatingParticipantRef");
            result.InitiatingParticipantRef = Load<Participant>((XmlParserComplexNode)_initiatingParticipantRefAttribute);

            // optional: id -> string id (1, 1)
            var _idAttribute = node.Attributes.ContainsKey("id") ? node.Attributes["id"].ProcessedValue : null;
            if (_idAttribute is not null) result.Id = (string)_idAttribute;

            // optional: name -> string name (1, 1)
            var _nameAttribute = node.Attributes.ContainsKey("name") ? node.Attributes["name"].ProcessedValue : null;
            if (_nameAttribute is not null) result.Name = (string)_nameAttribute;

            // optional: loopType -> ChoreographyLoopType loopType (1, 1)
            var _loopTypeAttribute = node.Attributes.ContainsKey("loopType") ? node.Attributes["loopType"].ProcessedValue : null;
            if (_loopTypeAttribute is not null) result.LoopType = CreateEnum<ChoreographyLoopType>((string)_loopTypeAttribute);

            // element: documentation -> Documentation documentation (0, *)
            FillElements(node.ChildNodes["documentation"], result.Documentation);

            // element: auditing -> Auditing auditing (0, 1)
            result.Auditing = FillElement<Auditing>(node.ChildNodes["auditing"]);

            // element: monitoring -> Monitoring monitoring (0, 1)
            result.Monitoring = FillElement<Monitoring>(node.ChildNodes["monitoring"]);

            // element: categoryValueRef -> CategoryValue categoryValueRef (0, *)
            FillElements(node.ChildNodes["categoryValueRef"], result.CategoryValueRef);

            // element: outgoing -> SequenceFlow outgoing (0, *)
            FillElements(node.ChildNodes["outgoing"], result.Outgoing);

            // element: incoming -> SequenceFlow incoming (0, *)
            FillElements(node.ChildNodes["incoming"], result.Incoming);

            // element: participantRef -> Participant participantRefs (2, *)
            FillElements(node.ChildNodes["participantRef"], result.ParticipantRefs);

            // element: correlationKey -> CorrelationKey correlationKeys (0, *)
            FillElements(node.ChildNodes["correlationKey"], result.CorrelationKeys);

            // element: flowElement -> FlowElement flowElements (0, *)
            FillElements(node.ChildNodes["flowElement"], result.FlowElements);

            // element: artifact -> Artifact artifacts (0, *)
            FillElements(node.ChildNodes["artifact"], result.Artifacts);

            // link back: Lane lanes (0, *)
            // lanes 1 <cmof_model.M_Two_Way_Association object at 0x000002941E4AD800>
            // target: Lane.flowNodeRefs

            // empty: LaneSet laneSets (0, *)

            return result;
        }


        private ChoreographyTask LoadChoreographyTask(XmlParserComplexNode node)
        {
             var result = GetOrCreate<ChoreographyTask>(node);

            // required: initiatingParticipantRef -> Participant initiatingParticipantRef (1, 1)
            var _initiatingParticipantRefAttribute = node.Attributes["initiatingParticipantRef"]?.ProcessedValue;
            if (_initiatingParticipantRefAttribute is null) throw new BPMNCheckerExceptions($"Node {node.ID} ({(string.IsNullOrWhiteSpace(node.Type?.Name) ? node.Type?.Name : "")}) is missing required attribute initiatingParticipantRef");
            result.InitiatingParticipantRef = Load<Participant>((XmlParserComplexNode)_initiatingParticipantRefAttribute);

            // optional: id -> string id (1, 1)
            var _idAttribute = node.Attributes.ContainsKey("id") ? node.Attributes["id"].ProcessedValue : null;
            if (_idAttribute is not null) result.Id = (string)_idAttribute;

            // optional: name -> string name (1, 1)
            var _nameAttribute = node.Attributes.ContainsKey("name") ? node.Attributes["name"].ProcessedValue : null;
            if (_nameAttribute is not null) result.Name = (string)_nameAttribute;

            // optional: loopType -> ChoreographyLoopType loopType (1, 1)
            var _loopTypeAttribute = node.Attributes.ContainsKey("loopType") ? node.Attributes["loopType"].ProcessedValue : null;
            if (_loopTypeAttribute is not null) result.LoopType = CreateEnum<ChoreographyLoopType>((string)_loopTypeAttribute);

            // element: documentation -> Documentation documentation (0, *)
            FillElements(node.ChildNodes["documentation"], result.Documentation);

            // element: auditing -> Auditing auditing (0, 1)
            result.Auditing = FillElement<Auditing>(node.ChildNodes["auditing"]);

            // element: monitoring -> Monitoring monitoring (0, 1)
            result.Monitoring = FillElement<Monitoring>(node.ChildNodes["monitoring"]);

            // element: categoryValueRef -> CategoryValue categoryValueRef (0, *)
            FillElements(node.ChildNodes["categoryValueRef"], result.CategoryValueRef);

            // element: outgoing -> SequenceFlow outgoing (0, *)
            FillElements(node.ChildNodes["outgoing"], result.Outgoing);

            // element: incoming -> SequenceFlow incoming (0, *)
            FillElements(node.ChildNodes["incoming"], result.Incoming);

            // element: participantRef -> Participant participantRefs (2, *)
            FillElements(node.ChildNodes["participantRef"], result.ParticipantRefs);

            // element: correlationKey -> CorrelationKey correlationKeys (0, *)
            FillElements(node.ChildNodes["correlationKey"], result.CorrelationKeys);

            // element: messageFlowRef -> MessageFlow messageFlowRef (1, 2)
            FillElements(node.ChildNodes["messageFlowRef"], result.MessageFlowRef);

            // link back: Lane lanes (0, *)
            // lanes 1 <cmof_model.M_Two_Way_Association object at 0x000002941E4AD800>
            // target: Lane.flowNodeRefs

            return result;
        }


        private Choreography LoadChoreography(XmlParserComplexNode node)
        {
             var result = GetOrCreate<Choreography>(node);

            // optional: id -> string id (1, 1)
            var _idAttribute = node.Attributes.ContainsKey("id") ? node.Attributes["id"].ProcessedValue : null;
            if (_idAttribute is not null) result.Id = (string)_idAttribute;

            // optional: name -> string name (1, 1)
            var _nameAttribute = node.Attributes.ContainsKey("name") ? node.Attributes["name"].ProcessedValue : null;
            if (_nameAttribute is not null) result.Name = (string)_nameAttribute;

            // optional: isClosed -> bool isClosed (1, 1)
            var _isClosedAttribute = node.Attributes.ContainsKey("isClosed") ? node.Attributes["isClosed"].ProcessedValue : null;
            if (_isClosedAttribute is not null) result.IsClosed = (bool)_isClosedAttribute;

            // element: documentation -> Documentation documentation (0, *)
            FillElements(node.ChildNodes["documentation"], result.Documentation);

            // element: flowElement -> FlowElement flowElements (0, *)
            FillElements(node.ChildNodes["flowElement"], result.FlowElements);

            // element: choreographyRef -> Choreography choreographyRef (0, *)
            FillElements(node.ChildNodes["choreographyRef"], result.ChoreographyRef);

            // element: artifact -> Artifact artifacts (0, *)
            FillElements(node.ChildNodes["artifact"], result.Artifacts);

            // element: participantAssociation -> ParticipantAssociation participantAssociations (0, *)
            FillElements(node.ChildNodes["participantAssociation"], result.ParticipantAssociations);

            // element: messageFlowAssociation -> MessageFlowAssociation messageFlowAssociations (0, *)
            FillElements(node.ChildNodes["messageFlowAssociation"], result.MessageFlowAssociations);

            // element: conversationAssociation -> ConversationAssociation conversationAssociations (1, 1)
            result.ConversationAssociations = FillElement<ConversationAssociation>(node.ChildNodes["conversationAssociation"]);

            // element: participant -> Participant participants (0, *)
            FillElements(node.ChildNodes["participant"], result.Participants);

            // element: messageFlow -> MessageFlow messageFlows (0, *)
            FillElements(node.ChildNodes["messageFlow"], result.MessageFlows);

            // element: correlationKey -> CorrelationKey correlationKeys (0, *)
            FillElements(node.ChildNodes["correlationKey"], result.CorrelationKeys);

            // element: conversationNode -> ConversationNode conversations (0, *)
            FillElements(node.ChildNodes["conversationNode"], result.Conversations);

            // element: conversationLink -> ConversationLink conversationLinks (0, *)
            FillElements(node.ChildNodes["conversationLink"], result.ConversationLinks);

            // empty: LaneSet laneSets (0, *)

            return result;
        }


        private GlobalChoreographyTask LoadGlobalChoreographyTask(XmlParserComplexNode node)
        {
             var result = GetOrCreate<GlobalChoreographyTask>(node);

            // optional: id -> string id (1, 1)
            var _idAttribute = node.Attributes.ContainsKey("id") ? node.Attributes["id"].ProcessedValue : null;
            if (_idAttribute is not null) result.Id = (string)_idAttribute;

            // optional: name -> string name (1, 1)
            var _nameAttribute = node.Attributes.ContainsKey("name") ? node.Attributes["name"].ProcessedValue : null;
            if (_nameAttribute is not null) result.Name = (string)_nameAttribute;

            // optional: isClosed -> bool isClosed (1, 1)
            var _isClosedAttribute = node.Attributes.ContainsKey("isClosed") ? node.Attributes["isClosed"].ProcessedValue : null;
            if (_isClosedAttribute is not null) result.IsClosed = (bool)_isClosedAttribute;

            // optional: initiatingParticipantRef -> Participant initiatingParticipantRef (1, 1)
            var _initiatingParticipantRefAttribute = node.Attributes.ContainsKey("initiatingParticipantRef") ? node.Attributes["initiatingParticipantRef"].ProcessedValue : null;
            if (_initiatingParticipantRefAttribute is not null) result.InitiatingParticipantRef = Load<Participant>((XmlParserComplexNode)_initiatingParticipantRefAttribute);

            // element: documentation -> Documentation documentation (0, *)
            FillElements(node.ChildNodes["documentation"], result.Documentation);

            // element: flowElement -> FlowElement flowElements (0, *)
            FillElements(node.ChildNodes["flowElement"], result.FlowElements);

            // element: choreographyRef -> Choreography choreographyRef (0, *)
            FillElements(node.ChildNodes["choreographyRef"], result.ChoreographyRef);

            // element: artifact -> Artifact artifacts (0, *)
            FillElements(node.ChildNodes["artifact"], result.Artifacts);

            // element: participantAssociation -> ParticipantAssociation participantAssociations (0, *)
            FillElements(node.ChildNodes["participantAssociation"], result.ParticipantAssociations);

            // element: messageFlowAssociation -> MessageFlowAssociation messageFlowAssociations (0, *)
            FillElements(node.ChildNodes["messageFlowAssociation"], result.MessageFlowAssociations);

            // element: conversationAssociation -> ConversationAssociation conversationAssociations (1, 1)
            result.ConversationAssociations = FillElement<ConversationAssociation>(node.ChildNodes["conversationAssociation"]);

            // element: participant -> Participant participants (0, *)
            FillElements(node.ChildNodes["participant"], result.Participants);

            // element: messageFlow -> MessageFlow messageFlows (0, *)
            FillElements(node.ChildNodes["messageFlow"], result.MessageFlows);

            // element: correlationKey -> CorrelationKey correlationKeys (0, *)
            FillElements(node.ChildNodes["correlationKey"], result.CorrelationKeys);

            // element: conversationNode -> ConversationNode conversations (0, *)
            FillElements(node.ChildNodes["conversationNode"], result.Conversations);

            // element: conversationLink -> ConversationLink conversationLinks (0, *)
            FillElements(node.ChildNodes["conversationLink"], result.ConversationLinks);

            // empty: LaneSet laneSets (0, *)

            return result;
        }


        private TextAnnotation LoadTextAnnotation(XmlParserComplexNode node)
        {
             var result = GetOrCreate<TextAnnotation>(node);

            // optional: id -> string id (1, 1)
            var _idAttribute = node.Attributes.ContainsKey("id") ? node.Attributes["id"].ProcessedValue : null;
            if (_idAttribute is not null) result.Id = (string)_idAttribute;

            // optional: textFormat -> string textFormat (1, 1)
            var _textFormatAttribute = node.Attributes.ContainsKey("textFormat") ? node.Attributes["textFormat"].ProcessedValue : null;
            if (_textFormatAttribute is not null) result.TextFormat = (string)_textFormatAttribute;

            // element: documentation -> Documentation documentation (0, *)
            FillElements(node.ChildNodes["documentation"], result.Documentation);

            // element: text -> string text (1, 1)
            result.Text = FillElement<string>(node.ChildNodes["text"]);

            return result;
        }


        private Group LoadGroup(XmlParserComplexNode node)
        {
             var result = GetOrCreate<Group>(node);

            // optional: id -> string id (1, 1)
            var _idAttribute = node.Attributes.ContainsKey("id") ? node.Attributes["id"].ProcessedValue : null;
            if (_idAttribute is not null) result.Id = (string)_idAttribute;

            // optional: categoryValueRef -> CategoryValue categoryValueRef (0, 1)
            var _categoryValueRefAttribute = node.Attributes.ContainsKey("categoryValueRef") ? node.Attributes["categoryValueRef"].ProcessedValue : null;
            if (_categoryValueRefAttribute is not null) result.CategoryValueRef = Load<CategoryValue>((XmlParserComplexNode)_categoryValueRefAttribute);

            // element: documentation -> Documentation documentation (0, *)
            FillElements(node.ChildNodes["documentation"], result.Documentation);

            return result;
        }


        private Association LoadAssociation(XmlParserComplexNode node)
        {
             var result = GetOrCreate<Association>(node);

            // required: sourceRef -> BaseElement sourceRef (1, 1)
            var _sourceRefAttribute = node.Attributes["sourceRef"]?.ProcessedValue;
            if (_sourceRefAttribute is null) throw new BPMNCheckerExceptions($"Node {node.ID} ({(string.IsNullOrWhiteSpace(node.Type?.Name) ? node.Type?.Name : "")}) is missing required attribute sourceRef");
            result.SourceRef = Load<BaseElement>((XmlParserComplexNode)_sourceRefAttribute);

            // required: targetRef -> BaseElement targetRef (1, 1)
            var _targetRefAttribute = node.Attributes["targetRef"]?.ProcessedValue;
            if (_targetRefAttribute is null) throw new BPMNCheckerExceptions($"Node {node.ID} ({(string.IsNullOrWhiteSpace(node.Type?.Name) ? node.Type?.Name : "")}) is missing required attribute targetRef");
            result.TargetRef = Load<BaseElement>((XmlParserComplexNode)_targetRefAttribute);

            // optional: id -> string id (1, 1)
            var _idAttribute = node.Attributes.ContainsKey("id") ? node.Attributes["id"].ProcessedValue : null;
            if (_idAttribute is not null) result.Id = (string)_idAttribute;

            // optional: associationDirection -> AssociationDirection associationDirection (1, 1)
            var _associationDirectionAttribute = node.Attributes.ContainsKey("associationDirection") ? node.Attributes["associationDirection"].ProcessedValue : null;
            if (_associationDirectionAttribute is not null) result.AssociationDirection = CreateEnum<AssociationDirection>((string)_associationDirectionAttribute);

            // element: documentation -> Documentation documentation (0, *)
            FillElements(node.ChildNodes["documentation"], result.Documentation);

            return result;
        }


        private Category LoadCategory(XmlParserComplexNode node)
        {
             var result = GetOrCreate<Category>(node);

            // optional: id -> string id (1, 1)
            var _idAttribute = node.Attributes.ContainsKey("id") ? node.Attributes["id"].ProcessedValue : null;
            if (_idAttribute is not null) result.Id = (string)_idAttribute;

            // optional: name -> string name (1, 1)
            var _nameAttribute = node.Attributes.ContainsKey("name") ? node.Attributes["name"].ProcessedValue : null;
            if (_nameAttribute is not null) result.Name = (string)_nameAttribute;

            // element: documentation -> Documentation documentation (0, *)
            FillElements(node.ChildNodes["documentation"], result.Documentation);

            // element: categoryValue -> CategoryValue categoryValue (0, *)
            FillElements(node.ChildNodes["categoryValue"], result.CategoryValue);

            return result;
        }



        private CategoryValue LoadCategoryValue(XmlParserComplexNode node)
        {
             var result = GetOrCreate<CategoryValue>(node);

            // optional: id -> string id (1, 1)
            var _idAttribute = node.Attributes.ContainsKey("id") ? node.Attributes["id"].ProcessedValue : null;
            if (_idAttribute is not null) result.Id = (string)_idAttribute;

            // optional: value -> string value (1, 1)
            var _valueAttribute = node.Attributes.ContainsKey("value") ? node.Attributes["value"].ProcessedValue : null;
            if (_valueAttribute is not null) result.Value = (string)_valueAttribute;

            // element: documentation -> Documentation documentation (0, *)
            FillElements(node.ChildNodes["documentation"], result.Documentation);

            // link back: FlowElement categorizedFlowElements (0, *)
            // categorizedFlowElements 0 <cmof_model.M_Two_Way_Association object at 0x000002941E4ADEC0>
            // target: FlowElement.categoryValueRef

            return result;
        }



        private ServiceTask LoadServiceTask(XmlParserComplexNode node)
        {
             var result = GetOrCreate<ServiceTask>(node);

            // optional: id -> string id (1, 1)
            var _idAttribute = node.Attributes.ContainsKey("id") ? node.Attributes["id"].ProcessedValue : null;
            if (_idAttribute is not null) result.Id = (string)_idAttribute;

            // optional: name -> string name (1, 1)
            var _nameAttribute = node.Attributes.ContainsKey("name") ? node.Attributes["name"].ProcessedValue : null;
            if (_nameAttribute is not null) result.Name = (string)_nameAttribute;

            // optional: isForCompensation -> bool isForCompensation (1, 1)
            var _isForCompensationAttribute = node.Attributes.ContainsKey("isForCompensation") ? node.Attributes["isForCompensation"].ProcessedValue : null;
            if (_isForCompensationAttribute is not null) result.IsForCompensation = (bool)_isForCompensationAttribute;

            // optional: default -> SequenceFlow default (0, 1)
            var _defaultAttribute = node.Attributes.ContainsKey("default") ? node.Attributes["default"].ProcessedValue : null;
            if (_defaultAttribute is not null) result.Default = Load<SequenceFlow>((XmlParserComplexNode)_defaultAttribute);

            // optional: startQuantity -> int startQuantity (1, 1)
            var _startQuantityAttribute = node.Attributes.ContainsKey("startQuantity") ? node.Attributes["startQuantity"].ProcessedValue : null;
            if (_startQuantityAttribute is not null) result.StartQuantity = (int)_startQuantityAttribute;

            // optional: completionQuantity -> int completionQuantity (1, 1)
            var _completionQuantityAttribute = node.Attributes.ContainsKey("completionQuantity") ? node.Attributes["completionQuantity"].ProcessedValue : null;
            if (_completionQuantityAttribute is not null) result.CompletionQuantity = (int)_completionQuantityAttribute;

            // optional: implementation -> string implementation (1, 1)
            var _implementationAttribute = node.Attributes.ContainsKey("implementation") ? node.Attributes["implementation"].ProcessedValue : null;
            if (_implementationAttribute is not null) result.Implementation = (string)_implementationAttribute;

            // optional: operationRef -> Operation operationRef (0, 1)
            var _operationRefAttribute = node.Attributes.ContainsKey("operationRef") ? node.Attributes["operationRef"].ProcessedValue : null;
            if (_operationRefAttribute is not null) result.OperationRef = Load<Operation>((XmlParserComplexNode)_operationRefAttribute);

            // element: documentation -> Documentation documentation (0, *)
            FillElements(node.ChildNodes["documentation"], result.Documentation);

            // element: auditing -> Auditing auditing (0, 1)
            result.Auditing = FillElement<Auditing>(node.ChildNodes["auditing"]);

            // element: monitoring -> Monitoring monitoring (0, 1)
            result.Monitoring = FillElement<Monitoring>(node.ChildNodes["monitoring"]);

            // element: categoryValueRef -> CategoryValue categoryValueRef (0, *)
            FillElements(node.ChildNodes["categoryValueRef"], result.CategoryValueRef);

            // element: outgoing -> SequenceFlow outgoing (0, *)
            FillElements(node.ChildNodes["outgoing"], result.Outgoing);

            // element: incoming -> SequenceFlow incoming (0, *)
            FillElements(node.ChildNodes["incoming"], result.Incoming);

            // element: loopCharacteristics -> LoopCharacteristics loopCharacteristics (0, 1)
            result.LoopCharacteristics = FillElement<LoopCharacteristics>(node.ChildNodes["loopCharacteristics"]);

            // element: resourceRole -> ResourceRole resources (0, *)
            FillElements(node.ChildNodes["resourceRole"], result.Resources);

            // element: property -> Property properties (0, *)
            FillElements(node.ChildNodes["property"], result.Properties);

            // element: ioSpecification -> InputOutputSpecification ioSpecification (0, 1)
            result.IoSpecification = FillElement<InputOutputSpecification>(node.ChildNodes["ioSpecification"]);

            // element: dataInputAssociation -> DataInputAssociation dataInputAssociations (0, *)
            FillElements(node.ChildNodes["dataInputAssociation"], result.DataInputAssociations);

            // element: dataOutputAssociation -> DataOutputAssociation dataOutputAssociations (0, *)
            FillElements(node.ChildNodes["dataOutputAssociation"], result.DataOutputAssociations);

            // link back: Lane lanes (0, *)
            // lanes 1 <cmof_model.M_Two_Way_Association object at 0x000002941E4AD800>
            // target: Lane.flowNodeRefs

            // link back: BoundaryEvent boundaryEventRefs (0, *)
            // boundaryEventRefs 0 <cmof_model.M_Two_Way_Association object at 0x000002941E4ADF40>
            // target: BoundaryEvent.attachedToRef

            // link back: ConversationLink incomingConversationLinks (0, *)
            // incomingConversationLinks 1 <cmof_model.M_Two_Way_Association object at 0x000002941E4ADD40>
            // target: ConversationLink.targetRef

            // link back: ConversationLink outgoingConversationLinks (0, *)
            // outgoingConversationLinks 1 <cmof_model.M_Two_Way_Association object at 0x000002941E4ADCC0>
            // target: ConversationLink.sourceRef

            return result;
        }


        private SubProcess LoadSubProcess(XmlParserComplexNode node)
        {
             var result = GetOrCreate<SubProcess>(node);

            // optional: id -> string id (1, 1)
            var _idAttribute = node.Attributes.ContainsKey("id") ? node.Attributes["id"].ProcessedValue : null;
            if (_idAttribute is not null) result.Id = (string)_idAttribute;

            // optional: name -> string name (1, 1)
            var _nameAttribute = node.Attributes.ContainsKey("name") ? node.Attributes["name"].ProcessedValue : null;
            if (_nameAttribute is not null) result.Name = (string)_nameAttribute;

            // optional: isForCompensation -> bool isForCompensation (1, 1)
            var _isForCompensationAttribute = node.Attributes.ContainsKey("isForCompensation") ? node.Attributes["isForCompensation"].ProcessedValue : null;
            if (_isForCompensationAttribute is not null) result.IsForCompensation = (bool)_isForCompensationAttribute;

            // optional: default -> SequenceFlow default (0, 1)
            var _defaultAttribute = node.Attributes.ContainsKey("default") ? node.Attributes["default"].ProcessedValue : null;
            if (_defaultAttribute is not null) result.Default = Load<SequenceFlow>((XmlParserComplexNode)_defaultAttribute);

            // optional: startQuantity -> int startQuantity (1, 1)
            var _startQuantityAttribute = node.Attributes.ContainsKey("startQuantity") ? node.Attributes["startQuantity"].ProcessedValue : null;
            if (_startQuantityAttribute is not null) result.StartQuantity = (int)_startQuantityAttribute;

            // optional: completionQuantity -> int completionQuantity (1, 1)
            var _completionQuantityAttribute = node.Attributes.ContainsKey("completionQuantity") ? node.Attributes["completionQuantity"].ProcessedValue : null;
            if (_completionQuantityAttribute is not null) result.CompletionQuantity = (int)_completionQuantityAttribute;

            // optional: triggeredByEvent -> bool triggeredByEvent (1, 1)
            var _triggeredByEventAttribute = node.Attributes.ContainsKey("triggeredByEvent") ? node.Attributes["triggeredByEvent"].ProcessedValue : null;
            if (_triggeredByEventAttribute is not null) result.TriggeredByEvent = (bool)_triggeredByEventAttribute;

            // element: documentation -> Documentation documentation (0, *)
            FillElements(node.ChildNodes["documentation"], result.Documentation);

            // element: auditing -> Auditing auditing (0, 1)
            result.Auditing = FillElement<Auditing>(node.ChildNodes["auditing"]);

            // element: monitoring -> Monitoring monitoring (0, 1)
            result.Monitoring = FillElement<Monitoring>(node.ChildNodes["monitoring"]);

            // element: categoryValueRef -> CategoryValue categoryValueRef (0, *)
            FillElements(node.ChildNodes["categoryValueRef"], result.CategoryValueRef);

            // element: outgoing -> SequenceFlow outgoing (0, *)
            FillElements(node.ChildNodes["outgoing"], result.Outgoing);

            // element: incoming -> SequenceFlow incoming (0, *)
            FillElements(node.ChildNodes["incoming"], result.Incoming);

            // element: loopCharacteristics -> LoopCharacteristics loopCharacteristics (0, 1)
            result.LoopCharacteristics = FillElement<LoopCharacteristics>(node.ChildNodes["loopCharacteristics"]);

            // element: resourceRole -> ResourceRole resources (0, *)
            FillElements(node.ChildNodes["resourceRole"], result.Resources);

            // element: property -> Property properties (0, *)
            FillElements(node.ChildNodes["property"], result.Properties);

            // element: ioSpecification -> InputOutputSpecification ioSpecification (0, 1)
            result.IoSpecification = FillElement<InputOutputSpecification>(node.ChildNodes["ioSpecification"]);

            // element: dataInputAssociation -> DataInputAssociation dataInputAssociations (0, *)
            FillElements(node.ChildNodes["dataInputAssociation"], result.DataInputAssociations);

            // element: dataOutputAssociation -> DataOutputAssociation dataOutputAssociations (0, *)
            FillElements(node.ChildNodes["dataOutputAssociation"], result.DataOutputAssociations);

            // element: flowElement -> FlowElement flowElements (0, *)
            FillElements(node.ChildNodes["flowElement"], result.FlowElements);

            // element: laneSet -> LaneSet laneSets (0, *)
            FillElements(node.ChildNodes["laneSet"], result.LaneSets);

            // element: artifact -> Artifact artifacts (0, *)
            FillElements(node.ChildNodes["artifact"], result.Artifacts);

            // link back: Lane lanes (0, *)
            // lanes 1 <cmof_model.M_Two_Way_Association object at 0x000002941E4AD800>
            // target: Lane.flowNodeRefs

            // link back: BoundaryEvent boundaryEventRefs (0, *)
            // boundaryEventRefs 0 <cmof_model.M_Two_Way_Association object at 0x000002941E4ADF40>
            // target: BoundaryEvent.attachedToRef

            return result;
        }



        private MultiInstanceLoopCharacteristics LoadMultiInstanceLoopCharacteristics(XmlParserComplexNode node)
        {
             var result = GetOrCreate<MultiInstanceLoopCharacteristics>(node);

            // optional: id -> string id (1, 1)
            var _idAttribute = node.Attributes.ContainsKey("id") ? node.Attributes["id"].ProcessedValue : null;
            if (_idAttribute is not null) result.Id = (string)_idAttribute;

            // optional: isSequential -> bool isSequential (1, 1)
            var _isSequentialAttribute = node.Attributes.ContainsKey("isSequential") ? node.Attributes["isSequential"].ProcessedValue : null;
            if (_isSequentialAttribute is not null) result.IsSequential = (bool)_isSequentialAttribute;

            // optional: behavior -> MultiInstanceBehavior behavior (1, 1)
            var _behaviorAttribute = node.Attributes.ContainsKey("behavior") ? node.Attributes["behavior"].ProcessedValue : null;
            if (_behaviorAttribute is not null) result.Behavior = CreateEnum<MultiInstanceBehavior>((string)_behaviorAttribute);

            // optional: oneBehaviorEventRef -> EventDefinition oneBehaviorEventRef (0, 1)
            var _oneBehaviorEventRefAttribute = node.Attributes.ContainsKey("oneBehaviorEventRef") ? node.Attributes["oneBehaviorEventRef"].ProcessedValue : null;
            if (_oneBehaviorEventRefAttribute is not null) result.OneBehaviorEventRef = Load<EventDefinition>((XmlParserComplexNode)_oneBehaviorEventRefAttribute);

            // optional: noneBehaviorEventRef -> EventDefinition noneBehaviorEventRef (0, 1)
            var _noneBehaviorEventRefAttribute = node.Attributes.ContainsKey("noneBehaviorEventRef") ? node.Attributes["noneBehaviorEventRef"].ProcessedValue : null;
            if (_noneBehaviorEventRefAttribute is not null) result.NoneBehaviorEventRef = Load<EventDefinition>((XmlParserComplexNode)_noneBehaviorEventRefAttribute);

            // element: documentation -> Documentation documentation (0, *)
            FillElements(node.ChildNodes["documentation"], result.Documentation);

            // element: loopCardinality -> Expression loopCardinality (0, 1)
            result.LoopCardinality = FillElement<Expression>(node.ChildNodes["loopCardinality"]);

            // element: loopDataInputRef -> ItemAwareElement loopDataInputRef (0, 1)
            result.LoopDataInputRef = FillElement<ItemAwareElement>(node.ChildNodes["loopDataInputRef"]);

            // element: loopDataOutputRef -> ItemAwareElement loopDataOutputRef (0, 1)
            result.LoopDataOutputRef = FillElement<ItemAwareElement>(node.ChildNodes["loopDataOutputRef"]);

            // element: inputDataItem -> DataInput inputDataItem (0, 1)
            result.InputDataItem = FillElement<DataInput>(node.ChildNodes["inputDataItem"]);

            // element: outputDataItem -> DataOutput outputDataItem (0, 1)
            result.OutputDataItem = FillElement<DataOutput>(node.ChildNodes["outputDataItem"]);

            // element: completionCondition -> Expression completionCondition (0, 1)
            result.CompletionCondition = FillElement<Expression>(node.ChildNodes["completionCondition"]);

            // element: complexBehaviorDefinition -> ComplexBehaviorDefinition complexBehaviorDefinition (0, *)
            FillElements(node.ChildNodes["complexBehaviorDefinition"], result.ComplexBehaviorDefinition);

            return result;
        }


        private StandardLoopCharacteristics LoadStandardLoopCharacteristics(XmlParserComplexNode node)
        {
             var result = GetOrCreate<StandardLoopCharacteristics>(node);

            // optional: id -> string id (1, 1)
            var _idAttribute = node.Attributes.ContainsKey("id") ? node.Attributes["id"].ProcessedValue : null;
            if (_idAttribute is not null) result.Id = (string)_idAttribute;

            // optional: testBefore -> bool testBefore (1, 1)
            var _testBeforeAttribute = node.Attributes.ContainsKey("testBefore") ? node.Attributes["testBefore"].ProcessedValue : null;
            if (_testBeforeAttribute is not null) result.TestBefore = (bool)_testBeforeAttribute;

            // optional: loopMaximum -> Expression loopMaximum (0, 1)
            var _loopMaximumAttribute = node.Attributes.ContainsKey("loopMaximum") ? node.Attributes["loopMaximum"].ProcessedValue : null;
            if (_loopMaximumAttribute is not null) result.LoopMaximum = Load<Expression>((XmlParserComplexNode)_loopMaximumAttribute);

            // element: documentation -> Documentation documentation (0, *)
            FillElements(node.ChildNodes["documentation"], result.Documentation);

            // element: loopCondition -> Expression loopCondition (0, 1)
            result.LoopCondition = FillElement<Expression>(node.ChildNodes["loopCondition"]);

            return result;
        }


        private CallActivity LoadCallActivity(XmlParserComplexNode node)
        {
             var result = GetOrCreate<CallActivity>(node);

            // optional: id -> string id (1, 1)
            var _idAttribute = node.Attributes.ContainsKey("id") ? node.Attributes["id"].ProcessedValue : null;
            if (_idAttribute is not null) result.Id = (string)_idAttribute;

            // optional: name -> string name (1, 1)
            var _nameAttribute = node.Attributes.ContainsKey("name") ? node.Attributes["name"].ProcessedValue : null;
            if (_nameAttribute is not null) result.Name = (string)_nameAttribute;

            // optional: isForCompensation -> bool isForCompensation (1, 1)
            var _isForCompensationAttribute = node.Attributes.ContainsKey("isForCompensation") ? node.Attributes["isForCompensation"].ProcessedValue : null;
            if (_isForCompensationAttribute is not null) result.IsForCompensation = (bool)_isForCompensationAttribute;

            // optional: default -> SequenceFlow default (0, 1)
            var _defaultAttribute = node.Attributes.ContainsKey("default") ? node.Attributes["default"].ProcessedValue : null;
            if (_defaultAttribute is not null) result.Default = Load<SequenceFlow>((XmlParserComplexNode)_defaultAttribute);

            // optional: startQuantity -> int startQuantity (1, 1)
            var _startQuantityAttribute = node.Attributes.ContainsKey("startQuantity") ? node.Attributes["startQuantity"].ProcessedValue : null;
            if (_startQuantityAttribute is not null) result.StartQuantity = (int)_startQuantityAttribute;

            // optional: completionQuantity -> int completionQuantity (1, 1)
            var _completionQuantityAttribute = node.Attributes.ContainsKey("completionQuantity") ? node.Attributes["completionQuantity"].ProcessedValue : null;
            if (_completionQuantityAttribute is not null) result.CompletionQuantity = (int)_completionQuantityAttribute;

            // optional: calledElement -> CallableElement calledElementRef (0, 1)
            var _calledElementRefAttribute = node.Attributes.ContainsKey("calledElement") ? node.Attributes["calledElement"].ProcessedValue : null;
            if (_calledElementRefAttribute is not null) result.CalledElementRef = Load<CallableElement>((XmlParserComplexNode)_calledElementRefAttribute);

            // element: documentation -> Documentation documentation (0, *)
            FillElements(node.ChildNodes["documentation"], result.Documentation);

            // element: auditing -> Auditing auditing (0, 1)
            result.Auditing = FillElement<Auditing>(node.ChildNodes["auditing"]);

            // element: monitoring -> Monitoring monitoring (0, 1)
            result.Monitoring = FillElement<Monitoring>(node.ChildNodes["monitoring"]);

            // element: categoryValueRef -> CategoryValue categoryValueRef (0, *)
            FillElements(node.ChildNodes["categoryValueRef"], result.CategoryValueRef);

            // element: outgoing -> SequenceFlow outgoing (0, *)
            FillElements(node.ChildNodes["outgoing"], result.Outgoing);

            // element: incoming -> SequenceFlow incoming (0, *)
            FillElements(node.ChildNodes["incoming"], result.Incoming);

            // element: loopCharacteristics -> LoopCharacteristics loopCharacteristics (0, 1)
            result.LoopCharacteristics = FillElement<LoopCharacteristics>(node.ChildNodes["loopCharacteristics"]);

            // element: resourceRole -> ResourceRole resources (0, *)
            FillElements(node.ChildNodes["resourceRole"], result.Resources);

            // element: property -> Property properties (0, *)
            FillElements(node.ChildNodes["property"], result.Properties);

            // element: ioSpecification -> InputOutputSpecification ioSpecification (0, 1)
            result.IoSpecification = FillElement<InputOutputSpecification>(node.ChildNodes["ioSpecification"]);

            // element: dataInputAssociation -> DataInputAssociation dataInputAssociations (0, *)
            FillElements(node.ChildNodes["dataInputAssociation"], result.DataInputAssociations);

            // element: dataOutputAssociation -> DataOutputAssociation dataOutputAssociations (0, *)
            FillElements(node.ChildNodes["dataOutputAssociation"], result.DataOutputAssociations);

            // link back: Lane lanes (0, *)
            // lanes 1 <cmof_model.M_Two_Way_Association object at 0x000002941E4AD800>
            // target: Lane.flowNodeRefs

            // link back: BoundaryEvent boundaryEventRefs (0, *)
            // boundaryEventRefs 0 <cmof_model.M_Two_Way_Association object at 0x000002941E4ADF40>
            // target: BoundaryEvent.attachedToRef

            return result;
        }


        private Task LoadTask(XmlParserComplexNode node)
        {
             var result = GetOrCreate<Task>(node);

            // optional: id -> string id (1, 1)
            var _idAttribute = node.Attributes.ContainsKey("id") ? node.Attributes["id"].ProcessedValue : null;
            if (_idAttribute is not null) result.Id = (string)_idAttribute;

            // optional: name -> string name (1, 1)
            var _nameAttribute = node.Attributes.ContainsKey("name") ? node.Attributes["name"].ProcessedValue : null;
            if (_nameAttribute is not null) result.Name = (string)_nameAttribute;

            // optional: isForCompensation -> bool isForCompensation (1, 1)
            var _isForCompensationAttribute = node.Attributes.ContainsKey("isForCompensation") ? node.Attributes["isForCompensation"].ProcessedValue : null;
            if (_isForCompensationAttribute is not null) result.IsForCompensation = (bool)_isForCompensationAttribute;

            // optional: default -> SequenceFlow default (0, 1)
            var _defaultAttribute = node.Attributes.ContainsKey("default") ? node.Attributes["default"].ProcessedValue : null;
            if (_defaultAttribute is not null) result.Default = Load<SequenceFlow>((XmlParserComplexNode)_defaultAttribute);

            // optional: startQuantity -> int startQuantity (1, 1)
            var _startQuantityAttribute = node.Attributes.ContainsKey("startQuantity") ? node.Attributes["startQuantity"].ProcessedValue : null;
            if (_startQuantityAttribute is not null) result.StartQuantity = (int)_startQuantityAttribute;

            // optional: completionQuantity -> int completionQuantity (1, 1)
            var _completionQuantityAttribute = node.Attributes.ContainsKey("completionQuantity") ? node.Attributes["completionQuantity"].ProcessedValue : null;
            if (_completionQuantityAttribute is not null) result.CompletionQuantity = (int)_completionQuantityAttribute;

            // element: documentation -> Documentation documentation (0, *)
            FillElements(node.ChildNodes["documentation"], result.Documentation);

            // element: auditing -> Auditing auditing (0, 1)
            result.Auditing = FillElement<Auditing>(node.ChildNodes["auditing"]);

            // element: monitoring -> Monitoring monitoring (0, 1)
            result.Monitoring = FillElement<Monitoring>(node.ChildNodes["monitoring"]);

            // element: categoryValueRef -> CategoryValue categoryValueRef (0, *)
            FillElements(node.ChildNodes["categoryValueRef"], result.CategoryValueRef);

            // element: outgoing -> SequenceFlow outgoing (0, *)
            FillElements(node.ChildNodes["outgoing"], result.Outgoing);

            // element: incoming -> SequenceFlow incoming (0, *)
            FillElements(node.ChildNodes["incoming"], result.Incoming);

            // element: loopCharacteristics -> LoopCharacteristics loopCharacteristics (0, 1)
            result.LoopCharacteristics = FillElement<LoopCharacteristics>(node.ChildNodes["loopCharacteristics"]);

            // element: resourceRole -> ResourceRole resources (0, *)
            FillElements(node.ChildNodes["resourceRole"], result.Resources);

            // element: property -> Property properties (0, *)
            FillElements(node.ChildNodes["property"], result.Properties);

            // element: ioSpecification -> InputOutputSpecification ioSpecification (0, 1)
            result.IoSpecification = FillElement<InputOutputSpecification>(node.ChildNodes["ioSpecification"]);

            // element: dataInputAssociation -> DataInputAssociation dataInputAssociations (0, *)
            FillElements(node.ChildNodes["dataInputAssociation"], result.DataInputAssociations);

            // element: dataOutputAssociation -> DataOutputAssociation dataOutputAssociations (0, *)
            FillElements(node.ChildNodes["dataOutputAssociation"], result.DataOutputAssociations);

            // link back: Lane lanes (0, *)
            // lanes 1 <cmof_model.M_Two_Way_Association object at 0x000002941E4AD800>
            // target: Lane.flowNodeRefs

            // link back: BoundaryEvent boundaryEventRefs (0, *)
            // boundaryEventRefs 0 <cmof_model.M_Two_Way_Association object at 0x000002941E4ADF40>
            // target: BoundaryEvent.attachedToRef

            // link back: ConversationLink incomingConversationLinks (0, *)
            // incomingConversationLinks 1 <cmof_model.M_Two_Way_Association object at 0x000002941E4ADD40>
            // target: ConversationLink.targetRef

            // link back: ConversationLink outgoingConversationLinks (0, *)
            // outgoingConversationLinks 1 <cmof_model.M_Two_Way_Association object at 0x000002941E4ADCC0>
            // target: ConversationLink.sourceRef

            return result;
        }


        private SendTask LoadSendTask(XmlParserComplexNode node)
        {
             var result = GetOrCreate<SendTask>(node);

            // optional: id -> string id (1, 1)
            var _idAttribute = node.Attributes.ContainsKey("id") ? node.Attributes["id"].ProcessedValue : null;
            if (_idAttribute is not null) result.Id = (string)_idAttribute;

            // optional: name -> string name (1, 1)
            var _nameAttribute = node.Attributes.ContainsKey("name") ? node.Attributes["name"].ProcessedValue : null;
            if (_nameAttribute is not null) result.Name = (string)_nameAttribute;

            // optional: isForCompensation -> bool isForCompensation (1, 1)
            var _isForCompensationAttribute = node.Attributes.ContainsKey("isForCompensation") ? node.Attributes["isForCompensation"].ProcessedValue : null;
            if (_isForCompensationAttribute is not null) result.IsForCompensation = (bool)_isForCompensationAttribute;

            // optional: default -> SequenceFlow default (0, 1)
            var _defaultAttribute = node.Attributes.ContainsKey("default") ? node.Attributes["default"].ProcessedValue : null;
            if (_defaultAttribute is not null) result.Default = Load<SequenceFlow>((XmlParserComplexNode)_defaultAttribute);

            // optional: startQuantity -> int startQuantity (1, 1)
            var _startQuantityAttribute = node.Attributes.ContainsKey("startQuantity") ? node.Attributes["startQuantity"].ProcessedValue : null;
            if (_startQuantityAttribute is not null) result.StartQuantity = (int)_startQuantityAttribute;

            // optional: completionQuantity -> int completionQuantity (1, 1)
            var _completionQuantityAttribute = node.Attributes.ContainsKey("completionQuantity") ? node.Attributes["completionQuantity"].ProcessedValue : null;
            if (_completionQuantityAttribute is not null) result.CompletionQuantity = (int)_completionQuantityAttribute;

            // optional: implementation -> string implementation (1, 1)
            var _implementationAttribute = node.Attributes.ContainsKey("implementation") ? node.Attributes["implementation"].ProcessedValue : null;
            if (_implementationAttribute is not null) result.Implementation = (string)_implementationAttribute;

            // optional: operationRef -> Operation operationRef (0, 1)
            var _operationRefAttribute = node.Attributes.ContainsKey("operationRef") ? node.Attributes["operationRef"].ProcessedValue : null;
            if (_operationRefAttribute is not null) result.OperationRef = Load<Operation>((XmlParserComplexNode)_operationRefAttribute);

            // optional: messageRef -> Message messageRef (0, 1)
            var _messageRefAttribute = node.Attributes.ContainsKey("messageRef") ? node.Attributes["messageRef"].ProcessedValue : null;
            if (_messageRefAttribute is not null) result.MessageRef = Load<Message>((XmlParserComplexNode)_messageRefAttribute);

            // element: documentation -> Documentation documentation (0, *)
            FillElements(node.ChildNodes["documentation"], result.Documentation);

            // element: auditing -> Auditing auditing (0, 1)
            result.Auditing = FillElement<Auditing>(node.ChildNodes["auditing"]);

            // element: monitoring -> Monitoring monitoring (0, 1)
            result.Monitoring = FillElement<Monitoring>(node.ChildNodes["monitoring"]);

            // element: categoryValueRef -> CategoryValue categoryValueRef (0, *)
            FillElements(node.ChildNodes["categoryValueRef"], result.CategoryValueRef);

            // element: outgoing -> SequenceFlow outgoing (0, *)
            FillElements(node.ChildNodes["outgoing"], result.Outgoing);

            // element: incoming -> SequenceFlow incoming (0, *)
            FillElements(node.ChildNodes["incoming"], result.Incoming);

            // element: loopCharacteristics -> LoopCharacteristics loopCharacteristics (0, 1)
            result.LoopCharacteristics = FillElement<LoopCharacteristics>(node.ChildNodes["loopCharacteristics"]);

            // element: resourceRole -> ResourceRole resources (0, *)
            FillElements(node.ChildNodes["resourceRole"], result.Resources);

            // element: property -> Property properties (0, *)
            FillElements(node.ChildNodes["property"], result.Properties);

            // element: ioSpecification -> InputOutputSpecification ioSpecification (0, 1)
            result.IoSpecification = FillElement<InputOutputSpecification>(node.ChildNodes["ioSpecification"]);

            // element: dataInputAssociation -> DataInputAssociation dataInputAssociations (0, *)
            FillElements(node.ChildNodes["dataInputAssociation"], result.DataInputAssociations);

            // element: dataOutputAssociation -> DataOutputAssociation dataOutputAssociations (0, *)
            FillElements(node.ChildNodes["dataOutputAssociation"], result.DataOutputAssociations);

            // link back: Lane lanes (0, *)
            // lanes 1 <cmof_model.M_Two_Way_Association object at 0x000002941E4AD800>
            // target: Lane.flowNodeRefs

            // link back: BoundaryEvent boundaryEventRefs (0, *)
            // boundaryEventRefs 0 <cmof_model.M_Two_Way_Association object at 0x000002941E4ADF40>
            // target: BoundaryEvent.attachedToRef

            // link back: ConversationLink incomingConversationLinks (0, *)
            // incomingConversationLinks 1 <cmof_model.M_Two_Way_Association object at 0x000002941E4ADD40>
            // target: ConversationLink.targetRef

            // link back: ConversationLink outgoingConversationLinks (0, *)
            // outgoingConversationLinks 1 <cmof_model.M_Two_Way_Association object at 0x000002941E4ADCC0>
            // target: ConversationLink.sourceRef

            return result;
        }


        private ReceiveTask LoadReceiveTask(XmlParserComplexNode node)
        {
             var result = GetOrCreate<ReceiveTask>(node);

            // optional: id -> string id (1, 1)
            var _idAttribute = node.Attributes.ContainsKey("id") ? node.Attributes["id"].ProcessedValue : null;
            if (_idAttribute is not null) result.Id = (string)_idAttribute;

            // optional: name -> string name (1, 1)
            var _nameAttribute = node.Attributes.ContainsKey("name") ? node.Attributes["name"].ProcessedValue : null;
            if (_nameAttribute is not null) result.Name = (string)_nameAttribute;

            // optional: isForCompensation -> bool isForCompensation (1, 1)
            var _isForCompensationAttribute = node.Attributes.ContainsKey("isForCompensation") ? node.Attributes["isForCompensation"].ProcessedValue : null;
            if (_isForCompensationAttribute is not null) result.IsForCompensation = (bool)_isForCompensationAttribute;

            // optional: default -> SequenceFlow default (0, 1)
            var _defaultAttribute = node.Attributes.ContainsKey("default") ? node.Attributes["default"].ProcessedValue : null;
            if (_defaultAttribute is not null) result.Default = Load<SequenceFlow>((XmlParserComplexNode)_defaultAttribute);

            // optional: startQuantity -> int startQuantity (1, 1)
            var _startQuantityAttribute = node.Attributes.ContainsKey("startQuantity") ? node.Attributes["startQuantity"].ProcessedValue : null;
            if (_startQuantityAttribute is not null) result.StartQuantity = (int)_startQuantityAttribute;

            // optional: completionQuantity -> int completionQuantity (1, 1)
            var _completionQuantityAttribute = node.Attributes.ContainsKey("completionQuantity") ? node.Attributes["completionQuantity"].ProcessedValue : null;
            if (_completionQuantityAttribute is not null) result.CompletionQuantity = (int)_completionQuantityAttribute;

            // optional: implementation -> string implementation (1, 1)
            var _implementationAttribute = node.Attributes.ContainsKey("implementation") ? node.Attributes["implementation"].ProcessedValue : null;
            if (_implementationAttribute is not null) result.Implementation = (string)_implementationAttribute;

            // optional: instantiate -> bool instantiate (1, 1)
            var _instantiateAttribute = node.Attributes.ContainsKey("instantiate") ? node.Attributes["instantiate"].ProcessedValue : null;
            if (_instantiateAttribute is not null) result.Instantiate = (bool)_instantiateAttribute;

            // optional: operationRef -> Operation operationRef (0, 1)
            var _operationRefAttribute = node.Attributes.ContainsKey("operationRef") ? node.Attributes["operationRef"].ProcessedValue : null;
            if (_operationRefAttribute is not null) result.OperationRef = Load<Operation>((XmlParserComplexNode)_operationRefAttribute);

            // optional: messageRef -> Message messageRef (0, 1)
            var _messageRefAttribute = node.Attributes.ContainsKey("messageRef") ? node.Attributes["messageRef"].ProcessedValue : null;
            if (_messageRefAttribute is not null) result.MessageRef = Load<Message>((XmlParserComplexNode)_messageRefAttribute);

            // element: documentation -> Documentation documentation (0, *)
            FillElements(node.ChildNodes["documentation"], result.Documentation);

            // element: auditing -> Auditing auditing (0, 1)
            result.Auditing = FillElement<Auditing>(node.ChildNodes["auditing"]);

            // element: monitoring -> Monitoring monitoring (0, 1)
            result.Monitoring = FillElement<Monitoring>(node.ChildNodes["monitoring"]);

            // element: categoryValueRef -> CategoryValue categoryValueRef (0, *)
            FillElements(node.ChildNodes["categoryValueRef"], result.CategoryValueRef);

            // element: outgoing -> SequenceFlow outgoing (0, *)
            FillElements(node.ChildNodes["outgoing"], result.Outgoing);

            // element: incoming -> SequenceFlow incoming (0, *)
            FillElements(node.ChildNodes["incoming"], result.Incoming);

            // element: loopCharacteristics -> LoopCharacteristics loopCharacteristics (0, 1)
            result.LoopCharacteristics = FillElement<LoopCharacteristics>(node.ChildNodes["loopCharacteristics"]);

            // element: resourceRole -> ResourceRole resources (0, *)
            FillElements(node.ChildNodes["resourceRole"], result.Resources);

            // element: property -> Property properties (0, *)
            FillElements(node.ChildNodes["property"], result.Properties);

            // element: ioSpecification -> InputOutputSpecification ioSpecification (0, 1)
            result.IoSpecification = FillElement<InputOutputSpecification>(node.ChildNodes["ioSpecification"]);

            // element: dataInputAssociation -> DataInputAssociation dataInputAssociations (0, *)
            FillElements(node.ChildNodes["dataInputAssociation"], result.DataInputAssociations);

            // element: dataOutputAssociation -> DataOutputAssociation dataOutputAssociations (0, *)
            FillElements(node.ChildNodes["dataOutputAssociation"], result.DataOutputAssociations);

            // link back: Lane lanes (0, *)
            // lanes 1 <cmof_model.M_Two_Way_Association object at 0x000002941E4AD800>
            // target: Lane.flowNodeRefs

            // link back: BoundaryEvent boundaryEventRefs (0, *)
            // boundaryEventRefs 0 <cmof_model.M_Two_Way_Association object at 0x000002941E4ADF40>
            // target: BoundaryEvent.attachedToRef

            // link back: ConversationLink incomingConversationLinks (0, *)
            // incomingConversationLinks 1 <cmof_model.M_Two_Way_Association object at 0x000002941E4ADD40>
            // target: ConversationLink.targetRef

            // link back: ConversationLink outgoingConversationLinks (0, *)
            // outgoingConversationLinks 1 <cmof_model.M_Two_Way_Association object at 0x000002941E4ADCC0>
            // target: ConversationLink.sourceRef

            return result;
        }


        private ScriptTask LoadScriptTask(XmlParserComplexNode node)
        {
             var result = GetOrCreate<ScriptTask>(node);

            // optional: id -> string id (1, 1)
            var _idAttribute = node.Attributes.ContainsKey("id") ? node.Attributes["id"].ProcessedValue : null;
            if (_idAttribute is not null) result.Id = (string)_idAttribute;

            // optional: name -> string name (1, 1)
            var _nameAttribute = node.Attributes.ContainsKey("name") ? node.Attributes["name"].ProcessedValue : null;
            if (_nameAttribute is not null) result.Name = (string)_nameAttribute;

            // optional: isForCompensation -> bool isForCompensation (1, 1)
            var _isForCompensationAttribute = node.Attributes.ContainsKey("isForCompensation") ? node.Attributes["isForCompensation"].ProcessedValue : null;
            if (_isForCompensationAttribute is not null) result.IsForCompensation = (bool)_isForCompensationAttribute;

            // optional: default -> SequenceFlow default (0, 1)
            var _defaultAttribute = node.Attributes.ContainsKey("default") ? node.Attributes["default"].ProcessedValue : null;
            if (_defaultAttribute is not null) result.Default = Load<SequenceFlow>((XmlParserComplexNode)_defaultAttribute);

            // optional: startQuantity -> int startQuantity (1, 1)
            var _startQuantityAttribute = node.Attributes.ContainsKey("startQuantity") ? node.Attributes["startQuantity"].ProcessedValue : null;
            if (_startQuantityAttribute is not null) result.StartQuantity = (int)_startQuantityAttribute;

            // optional: completionQuantity -> int completionQuantity (1, 1)
            var _completionQuantityAttribute = node.Attributes.ContainsKey("completionQuantity") ? node.Attributes["completionQuantity"].ProcessedValue : null;
            if (_completionQuantityAttribute is not null) result.CompletionQuantity = (int)_completionQuantityAttribute;

            // optional: scriptFormat -> string scriptFormat (1, 1)
            var _scriptFormatAttribute = node.Attributes.ContainsKey("scriptFormat") ? node.Attributes["scriptFormat"].ProcessedValue : null;
            if (_scriptFormatAttribute is not null) result.ScriptFormat = (string)_scriptFormatAttribute;

            // element: documentation -> Documentation documentation (0, *)
            FillElements(node.ChildNodes["documentation"], result.Documentation);

            // element: auditing -> Auditing auditing (0, 1)
            result.Auditing = FillElement<Auditing>(node.ChildNodes["auditing"]);

            // element: monitoring -> Monitoring monitoring (0, 1)
            result.Monitoring = FillElement<Monitoring>(node.ChildNodes["monitoring"]);

            // element: categoryValueRef -> CategoryValue categoryValueRef (0, *)
            FillElements(node.ChildNodes["categoryValueRef"], result.CategoryValueRef);

            // element: outgoing -> SequenceFlow outgoing (0, *)
            FillElements(node.ChildNodes["outgoing"], result.Outgoing);

            // element: incoming -> SequenceFlow incoming (0, *)
            FillElements(node.ChildNodes["incoming"], result.Incoming);

            // element: loopCharacteristics -> LoopCharacteristics loopCharacteristics (0, 1)
            result.LoopCharacteristics = FillElement<LoopCharacteristics>(node.ChildNodes["loopCharacteristics"]);

            // element: resourceRole -> ResourceRole resources (0, *)
            FillElements(node.ChildNodes["resourceRole"], result.Resources);

            // element: property -> Property properties (0, *)
            FillElements(node.ChildNodes["property"], result.Properties);

            // element: ioSpecification -> InputOutputSpecification ioSpecification (0, 1)
            result.IoSpecification = FillElement<InputOutputSpecification>(node.ChildNodes["ioSpecification"]);

            // element: dataInputAssociation -> DataInputAssociation dataInputAssociations (0, *)
            FillElements(node.ChildNodes["dataInputAssociation"], result.DataInputAssociations);

            // element: dataOutputAssociation -> DataOutputAssociation dataOutputAssociations (0, *)
            FillElements(node.ChildNodes["dataOutputAssociation"], result.DataOutputAssociations);

            // element: script -> string script (1, 1)
            result.Script = FillElement<string>(node.ChildNodes["script"]);

            // link back: Lane lanes (0, *)
            // lanes 1 <cmof_model.M_Two_Way_Association object at 0x000002941E4AD800>
            // target: Lane.flowNodeRefs

            // link back: BoundaryEvent boundaryEventRefs (0, *)
            // boundaryEventRefs 0 <cmof_model.M_Two_Way_Association object at 0x000002941E4ADF40>
            // target: BoundaryEvent.attachedToRef

            // link back: ConversationLink incomingConversationLinks (0, *)
            // incomingConversationLinks 1 <cmof_model.M_Two_Way_Association object at 0x000002941E4ADD40>
            // target: ConversationLink.targetRef

            // link back: ConversationLink outgoingConversationLinks (0, *)
            // outgoingConversationLinks 1 <cmof_model.M_Two_Way_Association object at 0x000002941E4ADCC0>
            // target: ConversationLink.sourceRef

            return result;
        }


        private BusinessRuleTask LoadBusinessRuleTask(XmlParserComplexNode node)
        {
             var result = GetOrCreate<BusinessRuleTask>(node);

            // optional: id -> string id (1, 1)
            var _idAttribute = node.Attributes.ContainsKey("id") ? node.Attributes["id"].ProcessedValue : null;
            if (_idAttribute is not null) result.Id = (string)_idAttribute;

            // optional: name -> string name (1, 1)
            var _nameAttribute = node.Attributes.ContainsKey("name") ? node.Attributes["name"].ProcessedValue : null;
            if (_nameAttribute is not null) result.Name = (string)_nameAttribute;

            // optional: isForCompensation -> bool isForCompensation (1, 1)
            var _isForCompensationAttribute = node.Attributes.ContainsKey("isForCompensation") ? node.Attributes["isForCompensation"].ProcessedValue : null;
            if (_isForCompensationAttribute is not null) result.IsForCompensation = (bool)_isForCompensationAttribute;

            // optional: default -> SequenceFlow default (0, 1)
            var _defaultAttribute = node.Attributes.ContainsKey("default") ? node.Attributes["default"].ProcessedValue : null;
            if (_defaultAttribute is not null) result.Default = Load<SequenceFlow>((XmlParserComplexNode)_defaultAttribute);

            // optional: startQuantity -> int startQuantity (1, 1)
            var _startQuantityAttribute = node.Attributes.ContainsKey("startQuantity") ? node.Attributes["startQuantity"].ProcessedValue : null;
            if (_startQuantityAttribute is not null) result.StartQuantity = (int)_startQuantityAttribute;

            // optional: completionQuantity -> int completionQuantity (1, 1)
            var _completionQuantityAttribute = node.Attributes.ContainsKey("completionQuantity") ? node.Attributes["completionQuantity"].ProcessedValue : null;
            if (_completionQuantityAttribute is not null) result.CompletionQuantity = (int)_completionQuantityAttribute;

            // optional: implementation -> string implementation (1, 1)
            var _implementationAttribute = node.Attributes.ContainsKey("implementation") ? node.Attributes["implementation"].ProcessedValue : null;
            if (_implementationAttribute is not null) result.Implementation = (string)_implementationAttribute;

            // element: documentation -> Documentation documentation (0, *)
            FillElements(node.ChildNodes["documentation"], result.Documentation);

            // element: auditing -> Auditing auditing (0, 1)
            result.Auditing = FillElement<Auditing>(node.ChildNodes["auditing"]);

            // element: monitoring -> Monitoring monitoring (0, 1)
            result.Monitoring = FillElement<Monitoring>(node.ChildNodes["monitoring"]);

            // element: categoryValueRef -> CategoryValue categoryValueRef (0, *)
            FillElements(node.ChildNodes["categoryValueRef"], result.CategoryValueRef);

            // element: outgoing -> SequenceFlow outgoing (0, *)
            FillElements(node.ChildNodes["outgoing"], result.Outgoing);

            // element: incoming -> SequenceFlow incoming (0, *)
            FillElements(node.ChildNodes["incoming"], result.Incoming);

            // element: loopCharacteristics -> LoopCharacteristics loopCharacteristics (0, 1)
            result.LoopCharacteristics = FillElement<LoopCharacteristics>(node.ChildNodes["loopCharacteristics"]);

            // element: resourceRole -> ResourceRole resources (0, *)
            FillElements(node.ChildNodes["resourceRole"], result.Resources);

            // element: property -> Property properties (0, *)
            FillElements(node.ChildNodes["property"], result.Properties);

            // element: ioSpecification -> InputOutputSpecification ioSpecification (0, 1)
            result.IoSpecification = FillElement<InputOutputSpecification>(node.ChildNodes["ioSpecification"]);

            // element: dataInputAssociation -> DataInputAssociation dataInputAssociations (0, *)
            FillElements(node.ChildNodes["dataInputAssociation"], result.DataInputAssociations);

            // element: dataOutputAssociation -> DataOutputAssociation dataOutputAssociations (0, *)
            FillElements(node.ChildNodes["dataOutputAssociation"], result.DataOutputAssociations);

            // link back: Lane lanes (0, *)
            // lanes 1 <cmof_model.M_Two_Way_Association object at 0x000002941E4AD800>
            // target: Lane.flowNodeRefs

            // link back: BoundaryEvent boundaryEventRefs (0, *)
            // boundaryEventRefs 0 <cmof_model.M_Two_Way_Association object at 0x000002941E4ADF40>
            // target: BoundaryEvent.attachedToRef

            // link back: ConversationLink incomingConversationLinks (0, *)
            // incomingConversationLinks 1 <cmof_model.M_Two_Way_Association object at 0x000002941E4ADD40>
            // target: ConversationLink.targetRef

            // link back: ConversationLink outgoingConversationLinks (0, *)
            // outgoingConversationLinks 1 <cmof_model.M_Two_Way_Association object at 0x000002941E4ADCC0>
            // target: ConversationLink.sourceRef

            return result;
        }


        private AdHocSubProcess LoadAdHocSubProcess(XmlParserComplexNode node)
        {
             var result = GetOrCreate<AdHocSubProcess>(node);

            // optional: id -> string id (1, 1)
            var _idAttribute = node.Attributes.ContainsKey("id") ? node.Attributes["id"].ProcessedValue : null;
            if (_idAttribute is not null) result.Id = (string)_idAttribute;

            // optional: name -> string name (1, 1)
            var _nameAttribute = node.Attributes.ContainsKey("name") ? node.Attributes["name"].ProcessedValue : null;
            if (_nameAttribute is not null) result.Name = (string)_nameAttribute;

            // optional: isForCompensation -> bool isForCompensation (1, 1)
            var _isForCompensationAttribute = node.Attributes.ContainsKey("isForCompensation") ? node.Attributes["isForCompensation"].ProcessedValue : null;
            if (_isForCompensationAttribute is not null) result.IsForCompensation = (bool)_isForCompensationAttribute;

            // optional: default -> SequenceFlow default (0, 1)
            var _defaultAttribute = node.Attributes.ContainsKey("default") ? node.Attributes["default"].ProcessedValue : null;
            if (_defaultAttribute is not null) result.Default = Load<SequenceFlow>((XmlParserComplexNode)_defaultAttribute);

            // optional: startQuantity -> int startQuantity (1, 1)
            var _startQuantityAttribute = node.Attributes.ContainsKey("startQuantity") ? node.Attributes["startQuantity"].ProcessedValue : null;
            if (_startQuantityAttribute is not null) result.StartQuantity = (int)_startQuantityAttribute;

            // optional: completionQuantity -> int completionQuantity (1, 1)
            var _completionQuantityAttribute = node.Attributes.ContainsKey("completionQuantity") ? node.Attributes["completionQuantity"].ProcessedValue : null;
            if (_completionQuantityAttribute is not null) result.CompletionQuantity = (int)_completionQuantityAttribute;

            // optional: triggeredByEvent -> bool triggeredByEvent (1, 1)
            var _triggeredByEventAttribute = node.Attributes.ContainsKey("triggeredByEvent") ? node.Attributes["triggeredByEvent"].ProcessedValue : null;
            if (_triggeredByEventAttribute is not null) result.TriggeredByEvent = (bool)_triggeredByEventAttribute;

            // optional: ordering -> AdHocOrdering ordering (1, 1)
            var _orderingAttribute = node.Attributes.ContainsKey("ordering") ? node.Attributes["ordering"].ProcessedValue : null;
            if (_orderingAttribute is not null) result.Ordering = CreateEnum<AdHocOrdering>((string)_orderingAttribute);

            // optional: cancelRemainingInstances -> bool cancelRemainingInstances (1, 1)
            var _cancelRemainingInstancesAttribute = node.Attributes.ContainsKey("cancelRemainingInstances") ? node.Attributes["cancelRemainingInstances"].ProcessedValue : null;
            if (_cancelRemainingInstancesAttribute is not null) result.CancelRemainingInstances = (bool)_cancelRemainingInstancesAttribute;

            // element: documentation -> Documentation documentation (0, *)
            FillElements(node.ChildNodes["documentation"], result.Documentation);

            // element: auditing -> Auditing auditing (0, 1)
            result.Auditing = FillElement<Auditing>(node.ChildNodes["auditing"]);

            // element: monitoring -> Monitoring monitoring (0, 1)
            result.Monitoring = FillElement<Monitoring>(node.ChildNodes["monitoring"]);

            // element: categoryValueRef -> CategoryValue categoryValueRef (0, *)
            FillElements(node.ChildNodes["categoryValueRef"], result.CategoryValueRef);

            // element: outgoing -> SequenceFlow outgoing (0, *)
            FillElements(node.ChildNodes["outgoing"], result.Outgoing);

            // element: incoming -> SequenceFlow incoming (0, *)
            FillElements(node.ChildNodes["incoming"], result.Incoming);

            // element: loopCharacteristics -> LoopCharacteristics loopCharacteristics (0, 1)
            result.LoopCharacteristics = FillElement<LoopCharacteristics>(node.ChildNodes["loopCharacteristics"]);

            // element: resourceRole -> ResourceRole resources (0, *)
            FillElements(node.ChildNodes["resourceRole"], result.Resources);

            // element: property -> Property properties (0, *)
            FillElements(node.ChildNodes["property"], result.Properties);

            // element: ioSpecification -> InputOutputSpecification ioSpecification (0, 1)
            result.IoSpecification = FillElement<InputOutputSpecification>(node.ChildNodes["ioSpecification"]);

            // element: dataInputAssociation -> DataInputAssociation dataInputAssociations (0, *)
            FillElements(node.ChildNodes["dataInputAssociation"], result.DataInputAssociations);

            // element: dataOutputAssociation -> DataOutputAssociation dataOutputAssociations (0, *)
            FillElements(node.ChildNodes["dataOutputAssociation"], result.DataOutputAssociations);

            // element: flowElement -> FlowElement flowElements (0, *)
            FillElements(node.ChildNodes["flowElement"], result.FlowElements);

            // element: laneSet -> LaneSet laneSets (0, *)
            FillElements(node.ChildNodes["laneSet"], result.LaneSets);

            // element: artifact -> Artifact artifacts (0, *)
            FillElements(node.ChildNodes["artifact"], result.Artifacts);

            // element: completionCondition -> Expression completionCondition (1, 1)
            result.CompletionCondition = FillElement<Expression>(node.ChildNodes["completionCondition"]);

            // link back: Lane lanes (0, *)
            // lanes 1 <cmof_model.M_Two_Way_Association object at 0x000002941E4AD800>
            // target: Lane.flowNodeRefs

            // link back: BoundaryEvent boundaryEventRefs (0, *)
            // boundaryEventRefs 0 <cmof_model.M_Two_Way_Association object at 0x000002941E4ADF40>
            // target: BoundaryEvent.attachedToRef

            return result;
        }


        private Transaction LoadTransaction(XmlParserComplexNode node)
        {
             var result = GetOrCreate<Transaction>(node);

            // optional: id -> string id (1, 1)
            var _idAttribute = node.Attributes.ContainsKey("id") ? node.Attributes["id"].ProcessedValue : null;
            if (_idAttribute is not null) result.Id = (string)_idAttribute;

            // optional: name -> string name (1, 1)
            var _nameAttribute = node.Attributes.ContainsKey("name") ? node.Attributes["name"].ProcessedValue : null;
            if (_nameAttribute is not null) result.Name = (string)_nameAttribute;

            // optional: isForCompensation -> bool isForCompensation (1, 1)
            var _isForCompensationAttribute = node.Attributes.ContainsKey("isForCompensation") ? node.Attributes["isForCompensation"].ProcessedValue : null;
            if (_isForCompensationAttribute is not null) result.IsForCompensation = (bool)_isForCompensationAttribute;

            // optional: default -> SequenceFlow default (0, 1)
            var _defaultAttribute = node.Attributes.ContainsKey("default") ? node.Attributes["default"].ProcessedValue : null;
            if (_defaultAttribute is not null) result.Default = Load<SequenceFlow>((XmlParserComplexNode)_defaultAttribute);

            // optional: startQuantity -> int startQuantity (1, 1)
            var _startQuantityAttribute = node.Attributes.ContainsKey("startQuantity") ? node.Attributes["startQuantity"].ProcessedValue : null;
            if (_startQuantityAttribute is not null) result.StartQuantity = (int)_startQuantityAttribute;

            // optional: completionQuantity -> int completionQuantity (1, 1)
            var _completionQuantityAttribute = node.Attributes.ContainsKey("completionQuantity") ? node.Attributes["completionQuantity"].ProcessedValue : null;
            if (_completionQuantityAttribute is not null) result.CompletionQuantity = (int)_completionQuantityAttribute;

            // optional: triggeredByEvent -> bool triggeredByEvent (1, 1)
            var _triggeredByEventAttribute = node.Attributes.ContainsKey("triggeredByEvent") ? node.Attributes["triggeredByEvent"].ProcessedValue : null;
            if (_triggeredByEventAttribute is not null) result.TriggeredByEvent = (bool)_triggeredByEventAttribute;

            // optional: method -> string method (1, 1)
            var _methodAttribute = node.Attributes.ContainsKey("method") ? node.Attributes["method"].ProcessedValue : null;
            if (_methodAttribute is not null) result.Method = (string)_methodAttribute;

            // element: documentation -> Documentation documentation (0, *)
            FillElements(node.ChildNodes["documentation"], result.Documentation);

            // element: auditing -> Auditing auditing (0, 1)
            result.Auditing = FillElement<Auditing>(node.ChildNodes["auditing"]);

            // element: monitoring -> Monitoring monitoring (0, 1)
            result.Monitoring = FillElement<Monitoring>(node.ChildNodes["monitoring"]);

            // element: categoryValueRef -> CategoryValue categoryValueRef (0, *)
            FillElements(node.ChildNodes["categoryValueRef"], result.CategoryValueRef);

            // element: outgoing -> SequenceFlow outgoing (0, *)
            FillElements(node.ChildNodes["outgoing"], result.Outgoing);

            // element: incoming -> SequenceFlow incoming (0, *)
            FillElements(node.ChildNodes["incoming"], result.Incoming);

            // element: loopCharacteristics -> LoopCharacteristics loopCharacteristics (0, 1)
            result.LoopCharacteristics = FillElement<LoopCharacteristics>(node.ChildNodes["loopCharacteristics"]);

            // element: resourceRole -> ResourceRole resources (0, *)
            FillElements(node.ChildNodes["resourceRole"], result.Resources);

            // element: property -> Property properties (0, *)
            FillElements(node.ChildNodes["property"], result.Properties);

            // element: ioSpecification -> InputOutputSpecification ioSpecification (0, 1)
            result.IoSpecification = FillElement<InputOutputSpecification>(node.ChildNodes["ioSpecification"]);

            // element: dataInputAssociation -> DataInputAssociation dataInputAssociations (0, *)
            FillElements(node.ChildNodes["dataInputAssociation"], result.DataInputAssociations);

            // element: dataOutputAssociation -> DataOutputAssociation dataOutputAssociations (0, *)
            FillElements(node.ChildNodes["dataOutputAssociation"], result.DataOutputAssociations);

            // element: flowElement -> FlowElement flowElements (0, *)
            FillElements(node.ChildNodes["flowElement"], result.FlowElements);

            // element: laneSet -> LaneSet laneSets (0, *)
            FillElements(node.ChildNodes["laneSet"], result.LaneSets);

            // element: artifact -> Artifact artifacts (0, *)
            FillElements(node.ChildNodes["artifact"], result.Artifacts);

            // link back: Lane lanes (0, *)
            // lanes 1 <cmof_model.M_Two_Way_Association object at 0x000002941E4AD800>
            // target: Lane.flowNodeRefs

            // link back: BoundaryEvent boundaryEventRefs (0, *)
            // boundaryEventRefs 0 <cmof_model.M_Two_Way_Association object at 0x000002941E4ADF40>
            // target: BoundaryEvent.attachedToRef

            // missing: string protocol (0, 1)

            return result;
        }


        private GlobalScriptTask LoadGlobalScriptTask(XmlParserComplexNode node)
        {
             var result = GetOrCreate<GlobalScriptTask>(node);

            // optional: id -> string id (1, 1)
            var _idAttribute = node.Attributes.ContainsKey("id") ? node.Attributes["id"].ProcessedValue : null;
            if (_idAttribute is not null) result.Id = (string)_idAttribute;

            // optional: name -> string name (1, 1)
            var _nameAttribute = node.Attributes.ContainsKey("name") ? node.Attributes["name"].ProcessedValue : null;
            if (_nameAttribute is not null) result.Name = (string)_nameAttribute;

            // optional: scriptLanguage -> string scriptLanguage (1, 1)
            var _scriptLanguageAttribute = node.Attributes.ContainsKey("scriptLanguage") ? node.Attributes["scriptLanguage"].ProcessedValue : null;
            if (_scriptLanguageAttribute is not null) result.ScriptLanguage = (string)_scriptLanguageAttribute;

            // element: documentation -> Documentation documentation (0, *)
            FillElements(node.ChildNodes["documentation"], result.Documentation);

            // element: ioSpecification -> InputOutputSpecification ioSpecification (0, 1)
            result.IoSpecification = FillElement<InputOutputSpecification>(node.ChildNodes["ioSpecification"]);

            // element: supportedInterfaceRef -> Interface supportedInterfaceRefs (0, *)
            FillElements(node.ChildNodes["supportedInterfaceRef"], result.SupportedInterfaceRefs);

            // element: ioBinding -> InputOutputBinding ioBinding (0, *)
            FillElements(node.ChildNodes["ioBinding"], result.IoBinding);

            // element: resourceRole -> ResourceRole resources (0, *)
            FillElements(node.ChildNodes["resourceRole"], result.Resources);

            // element: script -> string script (1, 1)
            result.Script = FillElement<string>(node.ChildNodes["script"]);

            return result;
        }


        private GlobalBusinessRuleTask LoadGlobalBusinessRuleTask(XmlParserComplexNode node)
        {
             var result = GetOrCreate<GlobalBusinessRuleTask>(node);

            // optional: id -> string id (1, 1)
            var _idAttribute = node.Attributes.ContainsKey("id") ? node.Attributes["id"].ProcessedValue : null;
            if (_idAttribute is not null) result.Id = (string)_idAttribute;

            // optional: name -> string name (1, 1)
            var _nameAttribute = node.Attributes.ContainsKey("name") ? node.Attributes["name"].ProcessedValue : null;
            if (_nameAttribute is not null) result.Name = (string)_nameAttribute;

            // optional: implementation -> string implementation (1, 1)
            var _implementationAttribute = node.Attributes.ContainsKey("implementation") ? node.Attributes["implementation"].ProcessedValue : null;
            if (_implementationAttribute is not null) result.Implementation = (string)_implementationAttribute;

            // element: documentation -> Documentation documentation (0, *)
            FillElements(node.ChildNodes["documentation"], result.Documentation);

            // element: ioSpecification -> InputOutputSpecification ioSpecification (0, 1)
            result.IoSpecification = FillElement<InputOutputSpecification>(node.ChildNodes["ioSpecification"]);

            // element: supportedInterfaceRef -> Interface supportedInterfaceRefs (0, *)
            FillElements(node.ChildNodes["supportedInterfaceRef"], result.SupportedInterfaceRefs);

            // element: ioBinding -> InputOutputBinding ioBinding (0, *)
            FillElements(node.ChildNodes["ioBinding"], result.IoBinding);

            // element: resourceRole -> ResourceRole resources (0, *)
            FillElements(node.ChildNodes["resourceRole"], result.Resources);

            return result;
        }


        private ComplexBehaviorDefinition LoadComplexBehaviorDefinition(XmlParserComplexNode node)
        {
             var result = GetOrCreate<ComplexBehaviorDefinition>(node);

            // optional: id -> string id (1, 1)
            var _idAttribute = node.Attributes.ContainsKey("id") ? node.Attributes["id"].ProcessedValue : null;
            if (_idAttribute is not null) result.Id = (string)_idAttribute;

            // element: documentation -> Documentation documentation (0, *)
            FillElements(node.ChildNodes["documentation"], result.Documentation);

            // element: condition -> FormalExpression condition (1, 1)
            result.Condition = FillElement<FormalExpression>(node.ChildNodes["condition"]);

            // element: event -> ImplicitThrowEvent event (0, 1)
            result.Event = FillElement<ImplicitThrowEvent>(node.ChildNodes["event"]);

            return result;
        }


        private ResourceRole LoadResourceRole(XmlParserComplexNode node)
        {
             var result = GetOrCreate<ResourceRole>(node);

            // optional: id -> string id (1, 1)
            var _idAttribute = node.Attributes.ContainsKey("id") ? node.Attributes["id"].ProcessedValue : null;
            if (_idAttribute is not null) result.Id = (string)_idAttribute;

            // optional: name -> string name (1, 1)
            var _nameAttribute = node.Attributes.ContainsKey("name") ? node.Attributes["name"].ProcessedValue : null;
            if (_nameAttribute is not null) result.Name = (string)_nameAttribute;

            // element: documentation -> Documentation documentation (0, *)
            FillElements(node.ChildNodes["documentation"], result.Documentation);

            // element: resourceRef -> Resource resourceRef (0, 1)
            result.ResourceRef = FillElement<Resource>(node.ChildNodes["resourceRef"]);

            // element: resourceParameterBinding -> ResourceParameterBinding resourceParameterBindings (0, *)
            FillElements(node.ChildNodes["resourceParameterBinding"], result.ResourceParameterBindings);

            // element: resourceAssignmentExpression -> ResourceAssignmentExpression resourceAssignmentExpression (0, 1)
            result.ResourceAssignmentExpression = FillElement<ResourceAssignmentExpression>(node.ChildNodes["resourceAssignmentExpression"]);

            return result;
        }


        private ResourceParameterBinding LoadResourceParameterBinding(XmlParserComplexNode node)
        {
             var result = GetOrCreate<ResourceParameterBinding>(node);

            // required: parameterRef -> ResourceParameter parameterRef (1, 1)
            var _parameterRefAttribute = node.Attributes["parameterRef"]?.ProcessedValue;
            if (_parameterRefAttribute is null) throw new BPMNCheckerExceptions($"Node {node.ID} ({(string.IsNullOrWhiteSpace(node.Type?.Name) ? node.Type?.Name : "")}) is missing required attribute parameterRef");
            result.ParameterRef = Load<ResourceParameter>((XmlParserComplexNode)_parameterRefAttribute);

            // element: expression -> Expression expression (1, 1)
            result.Expression = FillElement<Expression>(node.ChildNodes["expression"]);

            return result;
        }


        private ResourceAssignmentExpression LoadResourceAssignmentExpression(XmlParserComplexNode node)
        {
             var result = GetOrCreate<ResourceAssignmentExpression>(node);

            // element: expression -> Expression expression (1, 1)
            result.Expression = FillElement<Expression>(node.ChildNodes["expression"]);

            return result;
        }


        private Import LoadImport(XmlParserComplexNode node)
        {
             var result = GetOrCreate<Import>(node);

            // required: importType -> string importType (1, 1)
            var _importTypeAttribute = node.Attributes["importType"]?.ProcessedValue;
            if (_importTypeAttribute is null) throw new BPMNCheckerExceptions($"Node {node.ID} ({(string.IsNullOrWhiteSpace(node.Type?.Name) ? node.Type?.Name : "")}) is missing required attribute importType");
            result.ImportType = (string)_importTypeAttribute;

            // required: location -> string location (1, 1)
            var _locationAttribute = node.Attributes["location"]?.ProcessedValue;
            if (_locationAttribute is null) throw new BPMNCheckerExceptions($"Node {node.ID} ({(string.IsNullOrWhiteSpace(node.Type?.Name) ? node.Type?.Name : "")}) is missing required attribute location");
            result.Location = (string)_locationAttribute;

            // required: namespace -> string namespace (1, 1)
            var _namespaceAttribute = node.Attributes["namespace"]?.ProcessedValue;
            if (_namespaceAttribute is null) throw new BPMNCheckerExceptions($"Node {node.ID} ({(string.IsNullOrWhiteSpace(node.Type?.Name) ? node.Type?.Name : "")}) is missing required attribute namespace");
            result.Namespace = (string)_namespaceAttribute;

            return result;
        }


        private Definitions LoadDefinitions(XmlParserComplexNode node)
        {
             var result = GetOrCreate<Definitions>(node);

            // required: targetNamespace -> string targetNamespace (1, 1)
            var _targetNamespaceAttribute = node.Attributes["targetNamespace"]?.ProcessedValue;
            if (_targetNamespaceAttribute is null) throw new BPMNCheckerExceptions($"Node {node.ID} ({(string.IsNullOrWhiteSpace(node.Type?.Name) ? node.Type?.Name : "")}) is missing required attribute targetNamespace");
            result.TargetNamespace = (string)_targetNamespaceAttribute;

            // optional: id -> string id (1, 1)
            var _idAttribute = node.Attributes.ContainsKey("id") ? node.Attributes["id"].ProcessedValue : null;
            if (_idAttribute is not null) result.Id = (string)_idAttribute;

            // optional: name -> string name (1, 1)
            var _nameAttribute = node.Attributes.ContainsKey("name") ? node.Attributes["name"].ProcessedValue : null;
            if (_nameAttribute is not null) result.Name = (string)_nameAttribute;

            // optional: expressionLanguage -> string expressionLanguage (1, 1)
            var _expressionLanguageAttribute = node.Attributes.ContainsKey("expressionLanguage") ? node.Attributes["expressionLanguage"].ProcessedValue : null;
            if (_expressionLanguageAttribute is not null) result.ExpressionLanguage = (string)_expressionLanguageAttribute;

            // optional: typeLanguage -> string typeLanguage (1, 1)
            var _typeLanguageAttribute = node.Attributes.ContainsKey("typeLanguage") ? node.Attributes["typeLanguage"].ProcessedValue : null;
            if (_typeLanguageAttribute is not null) result.TypeLanguage = (string)_typeLanguageAttribute;

            // optional: exporter -> string exporter (1, 1)
            var _exporterAttribute = node.Attributes.ContainsKey("exporter") ? node.Attributes["exporter"].ProcessedValue : null;
            if (_exporterAttribute is not null) result.Exporter = (string)_exporterAttribute;

            // optional: exporterVersion -> string exporterVersion (1, 1)
            var _exporterVersionAttribute = node.Attributes.ContainsKey("exporterVersion") ? node.Attributes["exporterVersion"].ProcessedValue : null;
            if (_exporterVersionAttribute is not null) result.ExporterVersion = (string)_exporterVersionAttribute;

            // element: import -> Import imports (0, *)
            FillElements(node.ChildNodes["import"], result.Imports);

            // element: extension -> Extension extensions (0, *)
            FillElements(node.ChildNodes["extension"], result.Extensions);

            // element: relationship -> Relationship relationships (0, *)
            FillElements(node.ChildNodes["relationship"], result.Relationships);

            // element: rootElement -> RootElement rootElements (0, *)
            FillElements(node.ChildNodes["rootElement"], result.RootElements);

            return result;
        }

        #endregion

    }
}
