using System;

public class CreateSessionRequest { public string Name { get; set; } public string HostName { get; set; } }

public class CreateSessionResponse { public Guid SessionId { get; set; } public string HostPlayerId { get; set; } }
