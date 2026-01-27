using System;
using Wagr;

public class CreateSessionRequest { public string Name { get; set; } public Player HostPlayer { get; set; } public int Wager { get; set; } }
public class CreateSessionResponse { public Guid SessionId { get; set; } }

public class JoinSessionRequest { public Player OtherPlayer { get; set; } }
public class JoinSessionResponse { public bool Status { get; set; } }


public class SessionSummary { public Guid SessionId { get; set; } public string Name { get; set; } public int PlayerCount { get; set; } public int Wager { get; set; } }
public class SessionDetails { public Guid SessionId { get; set; } public Player HostPlayer { get; set; } public Player OtherPlayer { get; set; } public int Wager { get; set; } }

public class StartTicTacToeRequest { public string StartingPlayerId { get; set; } }
public class StartDotsRequest { public int Rows { get; set; } = 1; public int Cols { get; set; } = 1; }

public class MakeMoveRequest { public string PlayerId { get; set; } public object Move { get; set; } }

