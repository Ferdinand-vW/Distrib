namespace Distrib
{
    public class Message
    {
      public ProcessId ProcessId {get; set;}
      public object Msg {get;set;}

      public Message(ProcessId processId, object msg)
      {
        this.ProcessId = processId;
        this.Msg = msg;
      }
    }
}