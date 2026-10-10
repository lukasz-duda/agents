using A2A;
using A2A.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Register the echo agent with DI — this sets up A2AServer, ITaskStore, and all middleware
builder.Services.AddA2AAgent<EchoAgent>(EchoAgent.GetAgentCard("http://localhost:5000/echo"));

var app = builder.Build();

// Map JSON-RPC endpoint for A2A communication
app.MapA2A("/echo");

// Map well-known agent card for discovery
var card = app.Services.GetRequiredService<AgentCard>();
app.MapWellKnownAgentCard(card);

app.Run();

// Define the agent — implement IAgentHandler
public sealed class EchoAgent : IAgentHandler
{
    public async Task ExecuteAsync(
        RequestContext context,
        AgentEventQueue eventQueue,
        CancellationToken cancellationToken)
    {
        var responder = new MessageResponder(eventQueue, context.ContextId);
        await responder.ReplyAsync($"Echo: {context.UserText}", cancellationToken);
    }

    public static AgentCard GetAgentCard(string url) => new()
    {
        Name = "Echo Agent",
        Description = "Echoes messages back to the user",
        Version = "1.0.0",
        SupportedInterfaces = [new AgentInterface
        {
            Url = url,
            ProtocolBinding = "JSONRPC",
            ProtocolVersion = "1.0"
        }],
        DefaultInputModes = ["text/plain"],
        DefaultOutputModes = ["text/plain"],
        Capabilities = new AgentCapabilities { Streaming = false },
        Skills = [new AgentSkill
        {
            Id = "echo",
            Name = "Echo",
            Description = "Echoes back user messages",
            Tags = ["echo"]
        }],
    };
}