using Elsa.Expressions.Models;
using Elsa.Workflows.Core.Models;

namespace Elsa.Workflows.Core;

public static class ExpressionExecutionContextExtensions
{
    public static readonly object WorkflowExecutionContextKey = new();
    public static readonly object ActivityExecutionContextKey = new();
    public static readonly object InputKey = new();
    public static readonly object WorkflowKey = new();

    public static IDictionary<object, object> CreateActivityExecutionContextPropertiesFrom(WorkflowExecutionContext workflowExecutionContext, IDictionary<string, object> input) =>
        new Dictionary<object, object>
        {
            [WorkflowExecutionContextKey] = workflowExecutionContext,
            [InputKey] = input,
            [WorkflowKey] = workflowExecutionContext.Workflow,
        };
    
    public static IDictionary<object, object> CreateTriggerIndexingPropertiesFrom(Workflow workflow, IDictionary<string, object> input) =>
        new Dictionary<object, object>
        {
            [WorkflowKey] = workflow,
            [InputKey] = input
        };

    public static WorkflowExecutionContext GetWorkflowExecutionContext(this ExpressionExecutionContext context) => (WorkflowExecutionContext)context.TransientProperties[WorkflowExecutionContextKey];
    public static ActivityExecutionContext GetActivityExecutionContext(this ExpressionExecutionContext context) => (ActivityExecutionContext)context.TransientProperties[ActivityExecutionContextKey];
    public static IDictionary<string, object> GetInput(this ExpressionExecutionContext context) => (IDictionary<string, object>)context.TransientProperties[InputKey];

    public static T? Get<T>(this ExpressionExecutionContext context, Input<T>? input) => input != null ? (T?)context.GetBlock(input.MemoryBlockReference).Value : default;
    public static T? Get<T>(this ExpressionExecutionContext context, Output output) => (T?)context.GetBlock(output.MemoryBlockReference).Value;
    public static object? Get(this ExpressionExecutionContext context, Output output) => context.GetBlock(output.MemoryBlockReference).Value;
    public static T? GetVariable<T>(this ExpressionExecutionContext context, string name) => (T?)context.GetVariable(name);
    public static T? GetVariable<T>(this ExpressionExecutionContext context) => (T?)context.GetVariable(typeof(T).Name);
    public static object? GetVariable(this ExpressionExecutionContext context, string name) => new Variable(name).Get(context);
    public static Variable SetVariable<T>(this ExpressionExecutionContext context, T? value) => context.SetVariable(typeof(T).Name, value);
    public static Variable SetVariable<T>(this ExpressionExecutionContext context, string name, T? value) => context.SetVariable(name, (object?)value);

    public static Variable SetVariable(this ExpressionExecutionContext context, string name, object? value)
    {
        var variable = new Variable(name, value);
        context.Set(variable, value);
        return variable;
    }

    public static void Set(this ExpressionExecutionContext context, Output? output, object? value)
    {
        if(output != null) context.Set(output.MemoryBlockReference(), value);
    }
}