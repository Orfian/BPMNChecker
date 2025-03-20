using System.Xml.Linq;
using BPMNChecker;
using BPMNModel;
using BPMNModel.Camunda;
using BPMNModel.Model;
using BPMNModel.XMLElements;
using BPMNModel.XMLParser;

using Serilog;
using Serilog.Events;
using Serilog.Formatting.Json;
using Serilog.Sinks.File;
using Utility;

namespace UnitTests;

[TestClass]
public class DiagramLoading
{
    private static ILogger? _logger;
    private static Generator? _generator;

    private static ILogger GetLogger()
    {
        if (_logger == null)
        {
            _logger = new LoggerConfiguration()
                .WriteTo.Console(restrictedToMinimumLevel: LogEventLevel.Debug)
                .WriteTo.File(new JsonFormatter(), "important.json", LogEventLevel.Warning)
                .WriteTo.File("all.logs",
                    restrictedToMinimumLevel: LogEventLevel.Verbose,
                    rollingInterval: RollingInterval.Day)
                .MinimumLevel.Verbose()
                .CreateLogger();
        }
        return _logger;
    }

    private static Generator GetGenerator()
    {
        if (_generator == null)
        {
            var logger = GetLogger();
            _generator = Generator.CreateGenerator(logger);
            CamundaExtensions.EnrichGenerator(logger, _generator, true);
        }
        return _generator;
    }

    public static ModelRoot LoadBpmnModel(string resourcePath)
    {
        var logger = GetLogger();
        var generator = GetGenerator();

        XDocument doc = ResourcesUtility.LoadResourceAsXDocument(resourcePath);
        var parser = XmlParser.Parse(logger, generator, doc, false, true);

        if (parser.HasErrors)
        {
            foreach (var error in parser.Errors)
            {
                logger.Error(error);
            }
        }

        return Factory.ProcessModel(logger, parser);
    }

    [TestMethod]
    public void CallActivity()
    {
        var modelRoot = LoadBpmnModel(@"diagrams/ModelsAndDiagrams/CallActivity.bpmn");
        Assert.IsNotNull(modelRoot);

        // Ověření hlavního procesu
        var mainProcess = modelRoot.AllObjectsWithIds["_6"] as Process;
        Assert.IsNotNull(mainProcess);
        Assert.AreEqual("_6", mainProcess.Id);
        Assert.IsFalse(mainProcess.IsExecutable);

        // Ověření CallActivity
        var callActivity = mainProcess.FlowElements.OfType<CallActivity>().FirstOrDefault();
        Assert.IsNotNull(callActivity);
        Assert.AreEqual("SubProcessApproveOrder", callActivity.Id);
        Assert.AreEqual("Approve Order", callActivity.Name);
        Assert.AreEqual("_0", callActivity.CalledElementRef?.Id);
        Assert.AreEqual(1, callActivity.Incoming.Count);
        Assert.AreEqual(1, callActivity.Outgoing.Count);
        Assert.AreEqual("_6-470", callActivity.Incoming.First().Id);
        Assert.AreEqual("_6-500", callActivity.Outgoing.First().Id);

        // Ověření volaného procesu
        var calledProcess = modelRoot.AllObjectsWithIds["_0"] as Process;
        Assert.IsNotNull(calledProcess);
        Assert.AreEqual("_0", calledProcess.Id);
        Assert.IsFalse(calledProcess.IsExecutable);

        // Ověření elementů volaného procesu
        var calledProcessElements = calledProcess.FlowElements;
        Assert.AreEqual(7, calledProcessElements.Count); // 2 userTask + 1 startEvent + 1 endEvent + 3 sequenceFlow

        // Ověření UserTasks ve volaném procesu
        var userTasks = calledProcessElements.OfType<UserTask>().ToList();
        Assert.AreEqual(2, userTasks.Count);
        Assert.IsTrue(userTasks.Any(t => t.Id == "TaskApproveCustomer" && t.Name == "Approve Customer"));
        Assert.IsTrue(userTasks.Any(t => t.Id == "TaskApproveProduct" && t.Name == "Approve Product"));

        // Ověření vlastností UserTasks ve volaném procesu
        var approveCustomerTask = userTasks.First(t => t.Id == "TaskApproveCustomer");
        Assert.AreEqual("##unspecified", approveCustomerTask.Implementation);
        Assert.AreEqual(1, approveCustomerTask.CompletionQuantity);
        Assert.AreEqual(1, approveCustomerTask.StartQuantity);
        Assert.IsFalse(approveCustomerTask.IsForCompensation);
        Assert.AreEqual(1, approveCustomerTask.Incoming.Count);
        Assert.AreEqual(1, approveCustomerTask.Outgoing.Count);
        Assert.AreEqual("_0-131", approveCustomerTask.Incoming.First().Id);
        Assert.AreEqual("_0-133", approveCustomerTask.Outgoing.First().Id);

        var approveProductTask = userTasks.First(t => t.Id == "TaskApproveProduct");
        Assert.AreEqual("##unspecified", approveProductTask.Implementation);
        Assert.AreEqual(1, approveProductTask.CompletionQuantity);
        Assert.AreEqual(1, approveProductTask.StartQuantity);
        Assert.IsFalse(approveProductTask.IsForCompensation);
        Assert.AreEqual(1, approveProductTask.Incoming.Count);
        Assert.AreEqual(1, approveProductTask.Outgoing.Count);
        Assert.AreEqual("_0-133", approveProductTask.Incoming.First().Id);
        Assert.AreEqual("_0-135", approveProductTask.Outgoing.First().Id);

        // Ověření StartEvent ve volaném procesu
        var subProcessStart = calledProcessElements.OfType<StartEvent>().FirstOrDefault();
        Assert.IsNotNull(subProcessStart);
        Assert.AreEqual("SubProcessStart", subProcessStart.Id);
        Assert.AreEqual("", subProcessStart.Name);
        Assert.AreEqual(0, subProcessStart.Incoming.Count);
        Assert.AreEqual(1, subProcessStart.Outgoing.Count);
        Assert.AreEqual("_0-131", subProcessStart.Outgoing.First().Id);

        // Ověření EndEvent ve volaném procesu
        var subProcessEnd = calledProcessElements.OfType<EndEvent>().FirstOrDefault();
        Assert.IsNotNull(subProcessEnd);
        Assert.AreEqual("SubProcessEnd", subProcessEnd.Id);
        Assert.AreEqual("", subProcessEnd.Name);
        Assert.AreEqual(1, subProcessEnd.Incoming.Count);
        Assert.AreEqual(0, subProcessEnd.Outgoing.Count);
        Assert.AreEqual("_0-135", subProcessEnd.Incoming.First().Id);

        // Ověření SequenceFlows ve volaném procesu
        var calledProcessSequenceFlows = calledProcessElements.OfType<SequenceFlow>().ToList();
        Assert.AreEqual(3, calledProcessSequenceFlows.Count);

        var startToCustomer = calledProcessSequenceFlows.First(f => f.Id == "_0-131");
        Assert.AreEqual("", startToCustomer.Name);
        Assert.AreEqual("SubProcessStart", startToCustomer.SourceRef?.Id);
        Assert.AreEqual("TaskApproveCustomer", startToCustomer.TargetRef?.Id);

        var customerToProduct = calledProcessSequenceFlows.First(f => f.Id == "_0-133");
        Assert.AreEqual("", customerToProduct.Name);
        Assert.AreEqual("TaskApproveCustomer", customerToProduct.SourceRef?.Id);
        Assert.AreEqual("TaskApproveProduct", customerToProduct.TargetRef?.Id);

        var productToEnd = calledProcessSequenceFlows.First(f => f.Id == "_0-135");
        Assert.AreEqual("", productToEnd.Name);
        Assert.AreEqual("TaskApproveProduct", productToEnd.SourceRef?.Id);
        Assert.AreEqual("SubProcessEnd", productToEnd.TargetRef?.Id);

        // Ověření hlavních elementů hlavního procesu
        var mainProcessElements = mainProcess.FlowElements;

        // StartEvent
        var startEvent = mainProcessElements.OfType<StartEvent>().FirstOrDefault();
        Assert.IsNotNull(startEvent);
        Assert.AreEqual("StartProcess", startEvent.Id);
        Assert.AreEqual("", startEvent.Name);
        Assert.AreEqual(0, startEvent.Incoming.Count);
        Assert.AreEqual(1, startEvent.Outgoing.Count);
        Assert.AreEqual("_6-468", startEvent.Outgoing.First().Id);

        // Task (Quotation Handling)
        var quotationTask = mainProcessElements.OfType<BPMNModel.Model.Task>().FirstOrDefault();
        Assert.IsNotNull(quotationTask);
        Assert.AreEqual("TaskQuotationHandling", quotationTask.Id);
        Assert.AreEqual("Quotation Handling", quotationTask.Name);
        Assert.AreEqual(1, quotationTask.CompletionQuantity);
        Assert.AreEqual(1, quotationTask.StartQuantity);
        Assert.IsFalse(quotationTask.IsForCompensation);
        Assert.AreEqual(1, quotationTask.Incoming.Count);
        Assert.AreEqual(1, quotationTask.Outgoing.Count);
        Assert.AreEqual("_6-468", quotationTask.Incoming.First().Id);
        Assert.AreEqual("_6-470", quotationTask.Outgoing.First().Id);

        // ExclusiveGateway
        var exclusiveGateway = mainProcessElements.OfType<ExclusiveGateway>().FirstOrDefault();
        Assert.IsNotNull(exclusiveGateway);
        Assert.AreEqual("GatewayOrderApprovedDecision", exclusiveGateway.Id);
        Assert.AreEqual("", exclusiveGateway.Name);
        Assert.AreEqual(GatewayDirection.Diverging, exclusiveGateway.GatewayDirection);
        Assert.AreEqual(1, exclusiveGateway.Incoming.Count);
        Assert.AreEqual(2, exclusiveGateway.Outgoing.Count);
        Assert.AreEqual("_6-500", exclusiveGateway.Incoming.First().Id);
        Assert.IsTrue(exclusiveGateway.Outgoing.Any(o => o.Id == "_6-502"));
        Assert.IsTrue(exclusiveGateway.Outgoing.Any(o => o.Id == "_6-552"));

        // ParallelGateways
        var parallelGateways = mainProcessElements.OfType<ParallelGateway>().ToList();
        Assert.AreEqual(2, parallelGateways.Count);

        var splitGateway = parallelGateways.First(g => g.Id == "ParaSplitOrderAndShipment");
        Assert.AreEqual("", splitGateway.Name);
        Assert.AreEqual(GatewayDirection.Diverging, splitGateway.GatewayDirection);
        Assert.AreEqual(1, splitGateway.Incoming.Count);
        Assert.AreEqual(2, splitGateway.Outgoing.Count);
        Assert.AreEqual("_6-502", splitGateway.Incoming.First().Id);
        Assert.IsTrue(splitGateway.Outgoing.Any(o => o.Id == "_6-504"));
        Assert.IsTrue(splitGateway.Outgoing.Any(o => o.Id == "_6-506"));

        var joinGateway = parallelGateways.First(g => g.Id == "ParaJoinOderAndShipment");
        Assert.AreEqual("", joinGateway.Name);
        Assert.AreEqual(GatewayDirection.Converging, joinGateway.GatewayDirection);
        Assert.AreEqual(2, joinGateway.Incoming.Count);
        Assert.AreEqual(1, joinGateway.Outgoing.Count);
        Assert.IsTrue(joinGateway.Incoming.Any(i => i.Id == "_6-508"));
        Assert.IsTrue(joinGateway.Incoming.Any(i => i.Id == "_6-532"));
        Assert.AreEqual("_6-534", joinGateway.Outgoing.First().Id);

        // UserTask (Review Order)
        var reviewTask = mainProcessElements.OfType<UserTask>().FirstOrDefault();
        Assert.IsNotNull(reviewTask);
        Assert.AreEqual("TaskReviewOrder", reviewTask.Id);
        Assert.AreEqual("Review Order", reviewTask.Name);
        Assert.AreEqual(1, reviewTask.CompletionQuantity);
        Assert.AreEqual(1, reviewTask.StartQuantity);
        Assert.IsFalse(reviewTask.IsForCompensation);
        Assert.AreEqual("##unspecified", reviewTask.Implementation);
        Assert.AreEqual(1, reviewTask.Incoming.Count);
        Assert.AreEqual(1, reviewTask.Outgoing.Count);
        Assert.AreEqual("_6-534", reviewTask.Incoming.First().Id);
        Assert.AreEqual("_6-536", reviewTask.Outgoing.First().Id);

        // EndEvents
        var endEvents = mainProcessElements.OfType<EndEvent>().ToList();
        Assert.AreEqual(2, endEvents.Count);

        var normalEndEvent = endEvents.First(e => e.Id == "EndProcess");
        Assert.AreEqual("", normalEndEvent.Name);
        Assert.AreEqual(1, normalEndEvent.Incoming.Count);
        Assert.AreEqual(0, normalEndEvent.Outgoing.Count);
        Assert.AreEqual("_6-536", normalEndEvent.Incoming.First().Id);

        var terminateEndEvent = endEvents.First(e => e.Id == "TerminateProcess");
        Assert.AreEqual("", terminateEndEvent.Name);
        Assert.IsTrue(terminateEndEvent.EventDefinitions.Any(e => e is TerminateEventDefinition));
        Assert.AreEqual(1, terminateEndEvent.Incoming.Count);
        Assert.AreEqual(0, terminateEndEvent.Outgoing.Count);
        Assert.AreEqual("_6-552", terminateEndEvent.Incoming.First().Id);

        // SequenceFlows
        var sequenceFlows = mainProcessElements.OfType<SequenceFlow>().ToList();
        Assert.AreEqual(11, sequenceFlows.Count);

        // Ověření všech sequence flows
        var startToQuotation = sequenceFlows.First(f => f.Id == "_6-468");
        Assert.AreEqual("", startToQuotation.Name);
        Assert.AreEqual("StartProcess", startToQuotation.SourceRef?.Id);
        Assert.AreEqual("TaskQuotationHandling", startToQuotation.TargetRef?.Id);

        var quotationToCallActivity = sequenceFlows.First(f => f.Id == "_6-470");
        Assert.AreEqual("", quotationToCallActivity.Name);
        Assert.AreEqual("TaskQuotationHandling", quotationToCallActivity.SourceRef?.Id);
        Assert.AreEqual("SubProcessApproveOrder", quotationToCallActivity.TargetRef?.Id);

        var callActivityToGateway = sequenceFlows.First(f => f.Id == "_6-500");
        Assert.AreEqual("", callActivityToGateway.Name);
        Assert.AreEqual("SubProcessApproveOrder", callActivityToGateway.SourceRef?.Id);
        Assert.AreEqual("GatewayOrderApprovedDecision", callActivityToGateway.TargetRef?.Id);

        var gatewayToSplit = sequenceFlows.First(f => f.Id == "_6-502");
        Assert.AreEqual("Approved", gatewayToSplit.Name);
        Assert.AreEqual("GatewayOrderApprovedDecision", gatewayToSplit.SourceRef?.Id);
        Assert.AreEqual("ParaSplitOrderAndShipment", gatewayToSplit.TargetRef?.Id);

        var splitToOrder = sequenceFlows.First(f => f.Id == "_6-504");
        Assert.AreEqual("", splitToOrder.Name);
        Assert.AreEqual("ParaSplitOrderAndShipment", splitToOrder.SourceRef?.Id);
        Assert.AreEqual("_6-190", splitToOrder.TargetRef?.Id);

        var splitToShipping = sequenceFlows.First(f => f.Id == "_6-506");
        Assert.AreEqual("", splitToShipping.Name);
        Assert.AreEqual("ParaSplitOrderAndShipment", splitToShipping.SourceRef?.Id);
        Assert.AreEqual("_6-241", splitToShipping.TargetRef?.Id);

        var orderToJoin = sequenceFlows.First(f => f.Id == "_6-508");
        Assert.AreEqual("", orderToJoin.Name);
        Assert.AreEqual("_6-190", orderToJoin.SourceRef?.Id);
        Assert.AreEqual("ParaJoinOderAndShipment", orderToJoin.TargetRef?.Id);

        var shippingToJoin = sequenceFlows.First(f => f.Id == "_6-532");
        Assert.AreEqual("", shippingToJoin.Name);
        Assert.AreEqual("_6-241", shippingToJoin.SourceRef?.Id);
        Assert.AreEqual("ParaJoinOderAndShipment", shippingToJoin.TargetRef?.Id);

        var joinToReview = sequenceFlows.First(f => f.Id == "_6-534");
        Assert.AreEqual("", joinToReview.Name);
        Assert.AreEqual("ParaJoinOderAndShipment", joinToReview.SourceRef?.Id);
        Assert.AreEqual("TaskReviewOrder", joinToReview.TargetRef?.Id);

        var reviewToEnd = sequenceFlows.First(f => f.Id == "_6-536");
        Assert.AreEqual("", reviewToEnd.Name);
        Assert.AreEqual("TaskReviewOrder", reviewToEnd.SourceRef?.Id);
        Assert.AreEqual("EndProcess", reviewToEnd.TargetRef?.Id);

        var gatewayToTerminate = sequenceFlows.First(f => f.Id == "_6-552");
        Assert.AreEqual("", gatewayToTerminate.Name);
        Assert.AreEqual("GatewayOrderApprovedDecision", gatewayToTerminate.SourceRef?.Id);
        Assert.AreEqual("TerminateProcess", gatewayToTerminate.TargetRef?.Id);
    }

    [TestMethod]
    public void PrefixTest()
    {
        // otestovat tagy type <semantic:element> a <semantic:complexType>
    }
}

