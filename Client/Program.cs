using A2A;

// Discover agent
var cardResolver = new A2ACardResolver(new Uri("http://localhost:5000/"));
var agentCard = await cardResolver.GetAgentCardAsync();

// Create client using agent's endpoint
var client = new A2AClient(new Uri(agentCard.SupportedInterfaces[0].Url));

// Send message
var response = await client.SendMessageAsync(new SendMessageRequest
{
    Message = new Message
    {
        MessageId = Guid.NewGuid().ToString("N"),
        Role = Role.User,
        Parts = [Part.FromText("Hello!")]
    }
});

// Handle response
switch (response.PayloadCase)
{
    case SendMessageResponseCase.Message:
        Console.WriteLine(response.Message!.Parts[0].Text);
        break;
    case SendMessageResponseCase.Task:
        Console.WriteLine($"Task created: {response.Task!.Id}");
        break;
}