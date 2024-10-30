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
	public class CamundaConnector : ICamundaBaseElement
	{
		// Allowed: camunda:ServiceTaskLike

		// Extension Elements:
		public CamundaInputOutput? InputOutput { get; set; } 
		public string? ConnectorId { get; set; } 
	}

	public class CamundaConstraint
	{

		// Attributes:
		public string? Name {get; set;} 
		public string? Config {get; set;} 

	}

	public class CamundaEntry
	{

		// Attributes:
		public string? Key {get; set;} 


		// Extension Elements:
		public string? Value { get; set; } 
		public CamundaInputOutputParameterDefinition? Definition { get; set; } 
	}

	public class CamundaErrorEventDefinition : ErrorEventDefinition, ICamundaBaseElement
	{
		// Allowed: bpmn:ServiceTask

		// Attributes:
		public string? Expression {get; set;} 

	}

	public class CamundaExecutionListener : ICamundaBaseElement
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
	}

	public class CamundaFailedJobRetryTimeCycle : ICamundaBaseElement
	{
		// Allowed: camunda:AsyncCapable, bpmn:MultiInstanceLoopCharacteristics

		// Extension Elements:
		public string? Body { get; set; } 
	}

	public class CamundaField : ICamundaBaseElement
	{
		// Allowed: camunda:ServiceTaskLike, camunda:ExecutionListener, camunda:TaskListener

		// Attributes:
		public string? Name {get; set;} 
		public string? StringValue {get; set;} 


		// Extension Elements:
		public string? Expression { get; set; } 
		public string? String { get; set; } 
	}

	public class CamundaFormData : ICamundaBaseElement
	{
		// Allowed: bpmn:StartEvent, bpmn:UserTask

		// Attributes:
		public string? BusinessKey {get; set;} 


		// Extension Elements:
		public List<CamundaFormField> Fields { get; } = new();
	}

	public class CamundaFormField
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
	}

	public class CamundaFormProperty : ICamundaBaseElement
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
	}

	public class CamundaIn : CamundaInOutBinding, ICamundaBaseElement
	{
		// Allowed: bpmn:CallActivity, bpmn:SignalEventDefinition

	}

	public abstract class CamundaInOutBinding
	{

		// Attributes:
		public string? Source {get; set;} 
		public string? SourceExpression {get; set;} 
		public string? Target {get; set;} 
		public string? BusinessKey {get; set;} 
		public bool? Local {get; set;}  = false;
		public string? Variables {get; set;} 

	}

	public class CamundaInputOutput : ICamundaBaseElement
	{
		// Allowed: bpmn:FlowNode, camunda:Connector

		// Extension Elements:
		public CamundaInputOutput? InputOutput { get; set; } 
		public string? ConnectorId { get; set; } 
		public List<CamundaInputParameter> InputParameters { get; } = new();
		public List<CamundaOutputParameter> OutputParameters { get; } = new();
	}

	public class CamundaInputOutputParameter
	{

		// Attributes:
		public string? Name {get; set;} 


		// Extension Elements:
		public string? Value { get; set; } 
		public CamundaInputOutputParameterDefinition? Definition { get; set; } 
	}

	public abstract class CamundaInputOutputParameterDefinition
	{

	}

	public class CamundaInputParameter : CamundaInputOutputParameter
	{

	}

	public class CamundaList : CamundaInputOutputParameterDefinition
	{

		// Extension Elements:
		public List<CamundaInputOutputParameterDefinition> Items { get; } = new();
	}

	public class CamundaMap : CamundaInputOutputParameterDefinition
	{

		// Extension Elements:
		public List<CamundaEntry> Entries { get; } = new();
	}

	public class CamundaOut : CamundaInOutBinding, ICamundaBaseElement
	{
		// Allowed: bpmn:CallActivity

	}

	public class CamundaOutputParameter : CamundaInputOutputParameter
	{

	}

	public class CamundaProperties : ICamundaBaseElement
	{
		// Allowed: *

		// Extension Elements:
		public List<CamundaProperty> Values { get; } = new();
	}

	public class CamundaProperty
	{

		// Attributes:
		public string? Id {get; set;} 
		public string? Name {get; set;} 
		public string? Value {get; set;} 

	}

	public class CamundaScript : CamundaInputOutputParameterDefinition
	{

		// Attributes:
		public string? ScriptFormat {get; set;} 
		public string? Resource {get; set;} 


		// Extension Elements:
		public string? Value { get; set; } 
	}

	public class CamundaTaskListener : ICamundaBaseElement
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
	}

	public class CamundaValidation
	{

		// Extension Elements:
		public List<CamundaConstraint> Constraints { get; } = new();
	}

	public class CamundaValue : CamundaInputOutputParameterDefinition
	{

		// Attributes:
		public string? Id {get; set;} 
		public string? Name {get; set;} 


		// Extension Elements:
		public string? Value { get; set; } 
	}

}
