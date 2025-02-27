// -------------------------------------------------------
// Do not modify directly, this file was generated.
// -------------------------------------------------------
// While generating Camunda classes following types were used as if they were abstract:
// camunda:PotentialStarter
// camunda:Assignable
// camunda:CallActivity
// camunda:ServiceTaskLike
// camunda:DmnCapable
// camunda:ExternalCapable
// camunda:TaskPriorized
// (Just extending attributes were generated for base types.)
// -------------------------------------------------------
using BPMNModel.Model;

namespace BPMNModel.Camunda
{
	public class CamundaConnector : ICamundaLoaderBase, ICamundaBaseElement
	{
		// Allowed: camunda:ServiceTaskLike

		// Extension Elements:
		public CamundaInputOutput? InputOutput { get; set; } 
		public string? ConnectorId { get; set; } 
		public  void Load(XmlParserCamundaNode node)
		{
			//Loading element: InputOutput inputOutput - camunda:inputOutput
			if (node.ChildNodes["camunda:inputOutput"].Count==1) InputOutput = (CamundaInputOutput)CamundaFactory.Load((XmlParserCamundaNode)node.ChildNodes["camunda:inputOutput"][0]);

			//Loading element: String connectorId - camunda:connectorId
			if (node.ChildNodes["camunda:connectorId"].Count==1) ConnectorId = ((XmlParserStringNode)node.ChildNodes["camunda:connectorId"][0]).Value;

		}
	}

	public class CamundaConstraint : ICamundaLoaderBase
	{

		// Attributes:
		public string? Name {get; set;} 
		public string? Config {get; set;} 

		public  void Load(XmlParserCamundaNode node)
		{
			//Loading attribute: String name
			if (node.Attributes.ContainsKey("name")) Name = node.Attributes["name"].Value;

			//Loading attribute: String config
			if (node.Attributes.ContainsKey("config")) Config = node.Attributes["config"].Value;

		}
	}

	public class CamundaEntry : ICamundaLoaderBase
	{

		// Attributes:
		public string? Key {get; set;} 


		// Extension Elements:
		public string? Value { get; set; } 
		public CamundaInputOutputParameterDefinition? Definition { get; set; } 
		public  void Load(XmlParserCamundaNode node)
		{
			//Loading attribute: String key
			if (node.Attributes.ContainsKey("key")) Key = node.Attributes["key"].Value;

			//Loading element: String value - camunda:value
			if (node.ChildNodes.ContainsKey("value") && node.ChildNodes["value"].Count==1) Value = ((XmlParserStringNode)node.ChildNodes["value"][0]).Value;

			//Loading element: InputOutputParameterDefinition definition - camunda:definition
			if (node.ChildNodes["camunda:definition"].Count==1) Definition = (CamundaInputOutputParameterDefinition)CamundaFactory.Load((XmlParserCamundaNode)node.ChildNodes["camunda:definition"][0]);

		}
	}

	public class CamundaErrorEventDefinition : ErrorEventDefinition, ICamundaLoaderBase, ICamundaBaseElement
	{
		// Allowed: bpmn:ServiceTask

		// Attributes:
		public string? Expression {get; set;} 

		public  void Load(XmlParserCamundaNode node)
		{
			throw new Utility.BPMNCheckerExceptions("Do not know how to solve this - need to implement it in the generator.");
		}
	}

	public class CamundaExecutionListener : ICamundaLoaderBase, ICamundaBaseElement
	{
		// Allowed: bpmn:Task, bpmn:ServiceTask, bpmn:UserTask, bpmn:BusinessRuleTask, bpmn:ScriptTask, bpmn:ReceiveTask, bpmn:ManualTask, bpmn:ExclusiveGateway, bpmn:SequenceFlow, bpmn:ParallelGateway, bpmn:InclusiveGateway, bpmn:EventBasedGateway, bpmn:StartEvent, bpmn:IntermediateCatchEvent, bpmn:IntermediateThrowEvent, bpmn:EndEvent, bpmn:BoundaryEvent, bpmn:CallActivity, bpmn:SubProcess, bpmn:Process

		// Attributes:
		public string? Expression {get; set;} 
		public string? Class {get; set;} 
		public string? DelegateExpression {get; set;} 
		public string? Event {get; set;} 


		// Extension Elements:
		public CamundaScript? Script { get; set; } 
		public List<CamundaField> Fields { get; } = new();
		public  void Load(XmlParserCamundaNode node)
		{
			//Loading attribute: String expression
			if (node.Attributes.ContainsKey("expression")) Expression = node.Attributes["expression"].Value;

			//Loading attribute: String class
			if (node.Attributes.ContainsKey("class")) Class = node.Attributes["class"].Value;

			//Loading attribute: String delegateExpression
			if (node.Attributes.ContainsKey("delegateExpression")) DelegateExpression = node.Attributes["delegateExpression"].Value;

			//Loading attribute: String event
			if (node.Attributes.ContainsKey("event")) Event = node.Attributes["event"].Value;

			//Loading element: Script script - camunda:script
			if (node.ChildNodes["camunda:script"].Count==1) Script = (CamundaScript)CamundaFactory.Load((XmlParserCamundaNode)node.ChildNodes["camunda:script"][0]);

			//Loading element: Field* fields - camunda:fields
			if (node.ChildNodes["camunda:fields"].Count>0) CamundaFactory.LoadElements<CamundaField>(Fields, node.ChildNodes["camunda:fields"]);

		}
	}

	public class CamundaFailedJobRetryTimeCycle : ICamundaLoaderBase, ICamundaBaseElement
	{
		// Allowed: camunda:AsyncCapable, bpmn:MultiInstanceLoopCharacteristics

		// Extension Elements:
		public string? Body { get; set; } 
		public  void Load(XmlParserCamundaNode node)
		{
			//Loading element: String body - camunda:body
			if (node.ChildNodes.ContainsKey("body") && node.ChildNodes["body"].Count==1) Body = ((XmlParserStringNode)node.ChildNodes["body"][0]).Value;

		}
	}

	public class CamundaField : ICamundaLoaderBase, ICamundaBaseElement
	{
		// Allowed: camunda:ServiceTaskLike, camunda:ExecutionListener, camunda:TaskListener

		// Attributes:
		public string? Name {get; set;} 
		public string? StringValue {get; set;} 


		// Extension Elements:
		public string? Expression { get; set; } 
		public string? String { get; set; } 
		public  void Load(XmlParserCamundaNode node)
		{
			//Loading attribute: String name
			if (node.Attributes.ContainsKey("name")) Name = node.Attributes["name"].Value;

			//Loading attribute: String stringValue
			if (node.Attributes.ContainsKey("stringValue")) StringValue = node.Attributes["stringValue"].Value;

			//Loading element: String expression - camunda:expression
			if (node.ChildNodes["camunda:expression"].Count==1) Expression = ((XmlParserStringNode)node.ChildNodes["camunda:expression"][0]).Value;

			//Loading element: String string - camunda:string
			if (node.ChildNodes["camunda:string"].Count==1) String = ((XmlParserStringNode)node.ChildNodes["camunda:string"][0]).Value;

		}
	}

	public class CamundaFormData : ICamundaLoaderBase, ICamundaBaseElement
	{
		// Allowed: bpmn:StartEvent, bpmn:UserTask

		// Attributes:
		public string? BusinessKey {get; set;} 


		// Extension Elements:
		public List<CamundaFormField> Fields { get; } = new();
		public  void Load(XmlParserCamundaNode node)
		{
			//Loading attribute: String businessKey
			if (node.Attributes.ContainsKey("businessKey")) BusinessKey = node.Attributes["businessKey"].Value;

			//Loading element: FormField* fields - camunda:fields
			if (node.ChildNodes["camunda:fields"].Count>0) CamundaFactory.LoadElements<CamundaFormField>(Fields, node.ChildNodes["camunda:fields"]);

		}
	}

	public class CamundaFormField : ICamundaLoaderBase
	{

		// Attributes:
		public string? Id {get; set;} 
		public string? Label {get; set;} 
		public string? Type {get; set;} 
		public string? DatePattern {get; set;} 
		public string? DefaultValue {get; set;} 


		// Extension Elements:
		public CamundaProperties? Properties { get; set; } 
		public CamundaValidation? Validation { get; set; } 
		public List<CamundaValue> Values { get; } = new();
		public  void Load(XmlParserCamundaNode node)
		{
			//Loading attribute: String id
			if (node.Attributes.ContainsKey("id")) Id = node.Attributes["id"].Value;

			//Loading attribute: String label
			if (node.Attributes.ContainsKey("label")) Label = node.Attributes["label"].Value;

			//Loading attribute: String type
			if (node.Attributes.ContainsKey("type")) Type = node.Attributes["type"].Value;

			//Loading attribute: String datePattern
			if (node.Attributes.ContainsKey("datePattern")) DatePattern = node.Attributes["datePattern"].Value;

			//Loading attribute: String defaultValue
			if (node.Attributes.ContainsKey("defaultValue")) DefaultValue = node.Attributes["defaultValue"].Value;

			//Loading element: Properties properties - camunda:properties
			if (node.ChildNodes["camunda:properties"].Count==1) Properties = (CamundaProperties)CamundaFactory.Load((XmlParserCamundaNode)node.ChildNodes["camunda:properties"][0]);

			//Loading element: Validation validation - camunda:validation
			if (node.ChildNodes["camunda:validation"].Count==1) Validation = (CamundaValidation)CamundaFactory.Load((XmlParserCamundaNode)node.ChildNodes["camunda:validation"][0]);

			//Loading element: Value* values - camunda:values
			if (node.ChildNodes["camunda:values"].Count>0) CamundaFactory.LoadElements<CamundaValue>(Values, node.ChildNodes["camunda:values"]);

		}
	}

	public class CamundaFormProperty : ICamundaLoaderBase, ICamundaBaseElement
	{
		// Allowed: bpmn:StartEvent, bpmn:UserTask

		// Attributes:
		public string? Id {get; set;} 
		public string? Name {get; set;} 
		public string? Type {get; set;} 
		public string? Required {get; set;} 
		public string? Readable {get; set;} 
		public string? Writable {get; set;} 
		public string? Variable {get; set;} 
		public string? Expression {get; set;} 
		public string? DatePattern {get; set;} 
		public string? Default {get; set;} 


		// Extension Elements:
		public List<CamundaValue> Values { get; } = new();
		public  void Load(XmlParserCamundaNode node)
		{
			//Loading attribute: String id
			if (node.Attributes.ContainsKey("id")) Id = node.Attributes["id"].Value;

			//Loading attribute: String name
			if (node.Attributes.ContainsKey("name")) Name = node.Attributes["name"].Value;

			//Loading attribute: String type
			if (node.Attributes.ContainsKey("type")) Type = node.Attributes["type"].Value;

			//Loading attribute: String required
			if (node.Attributes.ContainsKey("required")) Required = node.Attributes["required"].Value;

			//Loading attribute: String readable
			if (node.Attributes.ContainsKey("readable")) Readable = node.Attributes["readable"].Value;

			//Loading attribute: String writable
			if (node.Attributes.ContainsKey("writable")) Writable = node.Attributes["writable"].Value;

			//Loading attribute: String variable
			if (node.Attributes.ContainsKey("variable")) Variable = node.Attributes["variable"].Value;

			//Loading attribute: String expression
			if (node.Attributes.ContainsKey("expression")) Expression = node.Attributes["expression"].Value;

			//Loading attribute: String datePattern
			if (node.Attributes.ContainsKey("datePattern")) DatePattern = node.Attributes["datePattern"].Value;

			//Loading attribute: String default
			if (node.Attributes.ContainsKey("default")) Default = node.Attributes["default"].Value;

			//Loading element: Value* values - camunda:values
			if (node.ChildNodes["camunda:values"].Count>0) CamundaFactory.LoadElements<CamundaValue>(Values, node.ChildNodes["camunda:values"]);

		}
	}

	public class CamundaIn : CamundaInOutBinding, ICamundaLoaderBase, ICamundaBaseElement
	{
		// Allowed: bpmn:CallActivity, bpmn:SignalEventDefinition

		public new void Load(XmlParserCamundaNode node)
		{
			base.Load(node);
		}
	}

	public abstract class CamundaInOutBinding : ICamundaLoaderBase
	{

		// Attributes:
		public string? Source {get; set;} 
		public string? SourceExpression {get; set;} 
		public string? Target {get; set;} 
		public string? BusinessKey {get; set;} 
		public bool? Local {get; set;}  = false;
		public string? Variables {get; set;} 

		public  void Load(XmlParserCamundaNode node)
		{
			//Loading attribute: String source
			if (node.Attributes.ContainsKey("source")) Source = node.Attributes["source"].Value;

			//Loading attribute: String sourceExpression
			if (node.Attributes.ContainsKey("sourceExpression")) SourceExpression = node.Attributes["sourceExpression"].Value;

			//Loading attribute: String target
			if (node.Attributes.ContainsKey("target")) Target = node.Attributes["target"].Value;

			//Loading attribute: String businessKey
			if (node.Attributes.ContainsKey("businessKey")) BusinessKey = node.Attributes["businessKey"].Value;

			//Loading attribute: Boolean local(False)
			if (node.Attributes.ContainsKey("local")) Local = string.Equals(node.Attributes["local"].Value, "true");
			else Local = false;

			//Loading attribute: String variables
			if (node.Attributes.ContainsKey("variables")) Variables = node.Attributes["variables"].Value;

		}
	}

	public class CamundaInputOutput : ICamundaLoaderBase, ICamundaBaseElement
	{
		// Allowed: bpmn:FlowNode, camunda:Connector

		// Extension Elements:
		public CamundaInputOutput? InputOutput { get; set; } 
		public string? ConnectorId { get; set; } 
		public List<CamundaInputParameter> InputParameters { get; } = new();
		public List<CamundaOutputParameter> OutputParameters { get; } = new();
		public  void Load(XmlParserCamundaNode node)
		{
			//Loading element: InputOutput inputOutput - camunda:inputOutput
			if (node.ChildNodes["camunda:inputOutput"].Count==1) InputOutput = (CamundaInputOutput)CamundaFactory.Load((XmlParserCamundaNode)node.ChildNodes["camunda:inputOutput"][0]);

			//Loading element: String connectorId - camunda:connectorId
			if (node.ChildNodes["camunda:connectorId"].Count==1) ConnectorId = ((XmlParserStringNode)node.ChildNodes["camunda:connectorId"][0]).Value;

			//Loading element: InputParameter* inputParameters - camunda:inputParameters
			if (node.ChildNodes["camunda:inputParameters"].Count>0) CamundaFactory.LoadElements<CamundaInputParameter>(InputParameters, node.ChildNodes["camunda:inputParameters"]);

			//Loading element: OutputParameter* outputParameters - camunda:outputParameters
			if (node.ChildNodes["camunda:outputParameters"].Count>0) CamundaFactory.LoadElements<CamundaOutputParameter>(OutputParameters, node.ChildNodes["camunda:outputParameters"]);

		}
	}

	public class CamundaInputOutputParameter : ICamundaLoaderBase
	{

		// Attributes:
		public string? Name {get; set;} 


		// Extension Elements:
		public string? Value { get; set; } 
		public CamundaInputOutputParameterDefinition? Definition { get; set; } 
		public  void Load(XmlParserCamundaNode node)
		{
			//Loading attribute: String name
			if (node.Attributes.ContainsKey("name")) Name = node.Attributes["name"].Value;

			//Loading element: String value - camunda:value
			if (node.ChildNodes.ContainsKey("value") && node.ChildNodes["value"].Count==1) Value = ((XmlParserStringNode)node.ChildNodes["value"][0]).Value;

			//Loading element: InputOutputParameterDefinition definition - camunda:definition
			if (node.ChildNodes["camunda:definition"].Count==1) Definition = (CamundaInputOutputParameterDefinition)CamundaFactory.Load((XmlParserCamundaNode)node.ChildNodes["camunda:definition"][0]);

		}
	}

	public abstract class CamundaInputOutputParameterDefinition : ICamundaLoaderBase
	{

		public  void Load(XmlParserCamundaNode node)
		{
		}
	}

	public class CamundaInputParameter : CamundaInputOutputParameter, ICamundaLoaderBase
	{

		public new void Load(XmlParserCamundaNode node)
		{
			base.Load(node);
		}
	}

	public class CamundaList : CamundaInputOutputParameterDefinition, ICamundaLoaderBase
	{

		// Extension Elements:
		public List<CamundaInputOutputParameterDefinition> Items { get; } = new();
		public new void Load(XmlParserCamundaNode node)
		{
			base.Load(node);
			//Loading element: InputOutputParameterDefinition* items - camunda:items
			if (node.ChildNodes["camunda:items"].Count>0) CamundaFactory.LoadElements<CamundaInputOutputParameterDefinition>(Items, node.ChildNodes["camunda:items"]);

		}
	}

	public class CamundaMap : CamundaInputOutputParameterDefinition, ICamundaLoaderBase
	{

		// Extension Elements:
		public List<CamundaEntry> Entries { get; } = new();
		public new void Load(XmlParserCamundaNode node)
		{
			base.Load(node);
			//Loading element: Entry* entries - camunda:entries
			if (node.ChildNodes["camunda:entries"].Count>0) CamundaFactory.LoadElements<CamundaEntry>(Entries, node.ChildNodes["camunda:entries"]);

		}
	}

	public class CamundaOut : CamundaInOutBinding, ICamundaLoaderBase, ICamundaBaseElement
	{
		// Allowed: bpmn:CallActivity

		public new void Load(XmlParserCamundaNode node)
		{
			base.Load(node);
		}
	}

	public class CamundaOutputParameter : CamundaInputOutputParameter, ICamundaLoaderBase
	{

		public new void Load(XmlParserCamundaNode node)
		{
			base.Load(node);
		}
	}

	public class CamundaProperties : ICamundaLoaderBase, ICamundaBaseElement
	{
		// Allowed: *

		// Extension Elements:
		public List<CamundaProperty> Values { get; } = new();
		public  void Load(XmlParserCamundaNode node)
		{
			//Loading element: Property* values - camunda:values
			if (node.ChildNodes["camunda:values"].Count>0) CamundaFactory.LoadElements<CamundaProperty>(Values, node.ChildNodes["camunda:values"]);

		}
	}

	public class CamundaProperty : ICamundaLoaderBase
	{

		// Attributes:
		public string? Id {get; set;} 
		public string? Name {get; set;} 
		public string? Value {get; set;} 

		public  void Load(XmlParserCamundaNode node)
		{
			//Loading attribute: String id
			if (node.Attributes.ContainsKey("id")) Id = node.Attributes["id"].Value;

			//Loading attribute: String name
			if (node.Attributes.ContainsKey("name")) Name = node.Attributes["name"].Value;

			//Loading attribute: String value
			if (node.Attributes.ContainsKey("value")) Value = node.Attributes["value"].Value;

		}
	}

	public class CamundaScript : CamundaInputOutputParameterDefinition, ICamundaLoaderBase
	{

		// Attributes:
		public string? ScriptFormat {get; set;} 
		public string? Resource {get; set;} 


		// Extension Elements:
		public string? Value { get; set; } 
		public new void Load(XmlParserCamundaNode node)
		{
			base.Load(node);
			//Loading attribute: String scriptFormat
			if (node.Attributes.ContainsKey("scriptFormat")) ScriptFormat = node.Attributes["scriptFormat"].Value;

			//Loading attribute: String resource
			if (node.Attributes.ContainsKey("resource")) Resource = node.Attributes["resource"].Value;

			//Loading element: String value - camunda:value
			if (node.ChildNodes.ContainsKey("value") && node.ChildNodes["value"].Count==1) Value = ((XmlParserStringNode)node.ChildNodes["value"][0]).Value;

		}
	}

	public class CamundaTaskListener : ICamundaLoaderBase, ICamundaBaseElement
	{
		// Allowed: bpmn:UserTask

		// Attributes:
		public string? Expression {get; set;} 
		public string? Class {get; set;} 
		public string? DelegateExpression {get; set;} 
		public string? Event {get; set;} 
		public string? Id {get; set;} 


		// Extension Elements:
		public CamundaScript? Script { get; set; } 
		public List<CamundaField> Fields { get; } = new();
		public List<TimerEventDefinition> EventDefinitions { get; } = new();
		public  void Load(XmlParserCamundaNode node)
		{
			//Loading attribute: String expression
			if (node.Attributes.ContainsKey("expression")) Expression = node.Attributes["expression"].Value;

			//Loading attribute: String class
			if (node.Attributes.ContainsKey("class")) Class = node.Attributes["class"].Value;

			//Loading attribute: String delegateExpression
			if (node.Attributes.ContainsKey("delegateExpression")) DelegateExpression = node.Attributes["delegateExpression"].Value;

			//Loading attribute: String event
			if (node.Attributes.ContainsKey("event")) Event = node.Attributes["event"].Value;

			//Loading attribute: String id
			if (node.Attributes.ContainsKey("id")) Id = node.Attributes["id"].Value;

			//Loading element: Script script - camunda:script
			if (node.ChildNodes["camunda:script"].Count==1) Script = (CamundaScript)CamundaFactory.Load((XmlParserCamundaNode)node.ChildNodes["camunda:script"][0]);

			//Loading element: Field* fields - camunda:fields
			if (node.ChildNodes["camunda:fields"].Count>0) CamundaFactory.LoadElements<CamundaField>(Fields, node.ChildNodes["camunda:fields"]);

			//Loading element: bpmn:TimerEventDefinition* eventDefinitions - camunda:eventDefinitions
			if (node.ChildNodes["camunda:eventDefinitions"].Count>0) CamundaFactory.LoadElements<TimerEventDefinition>(EventDefinitions, node.ChildNodes["camunda:eventDefinitions"]);

		}
	}

	public class CamundaValidation : ICamundaLoaderBase
	{

		// Extension Elements:
		public List<CamundaConstraint> Constraints { get; } = new();
		public  void Load(XmlParserCamundaNode node)
		{
			//Loading element: Constraint* constraints - camunda:constraints
			if (node.ChildNodes["camunda:constraints"].Count>0) CamundaFactory.LoadElements<CamundaConstraint>(Constraints, node.ChildNodes["camunda:constraints"]);

		}
	}

	public class CamundaValue : CamundaInputOutputParameterDefinition, ICamundaLoaderBase
	{

		// Attributes:
		public string? Id {get; set;} 
		public string? Name {get; set;} 


		// Extension Elements:
		public string? Value { get; set; } 
		public new void Load(XmlParserCamundaNode node)
		{
			base.Load(node);
			//Loading attribute: String id
			if (node.Attributes.ContainsKey("id")) Id = node.Attributes["id"].Value;

			//Loading attribute: String name
			if (node.Attributes.ContainsKey("name")) Name = node.Attributes["name"].Value;

			//Loading element: String value - camunda:value
			if (node.ChildNodes.ContainsKey("value") && node.ChildNodes["value"].Count==1) Value = ((XmlParserStringNode)node.ChildNodes["value"][0]).Value;

		}
	}

}
