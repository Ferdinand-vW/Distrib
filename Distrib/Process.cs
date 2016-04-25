using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Distrib
{
  public class Process
  {
    public ProcessId ProcessId {get;set;}
    public MailBox MailBox {get;set;}

    public static Process Current
      {
        get {
          if(!Task.CurrentId.HasValue)
          {
            throw new Exception("Cannot access Registry from outside of a Task");
          }
          else
          {
            return Registry.Processes[Task.CurrentId.Value];
          }
        }
      }

    private Task _internalProcess;
    private Action _action;

    public Process(NodeId nId, int ident, Action action)
    {
      this.ProcessId = new ProcessId(nId,ident);
      this._action = action;
    }

    public Process(NodeId nId, int ident, Action<Process> pAction)
    {
      this.ProcessId = new ProcessId(nId,ident);
    }

    internal void StartProcess()
    {
        Console.WriteLine("Start new task");
        _internalProcess = new Task(() => _action.Invoke());
        Registry.RegisterProcess(_internalProcess.Id,this); //Make the process accessible in the executing Task
        _internalProcess.Start();
        Console.WriteLine("Started task");
    }

    //ProcessId
    //MailBox
    //terminate
    //kill
    //send
    //receive
    //receiveTimeOut
    //whereIs
    //spawnProcess
    //spawnLocalProcess
    //monitor
    //unmonitor
    //register
    //unregister
    //relay
    //unrelay
    //link
    //unlink
  }

  public class ProcessId
  {
    public NodeId NodeId {get;set;}
    public int Pid {get; set;}

    public ProcessId(NodeId nId, int pId)
    {
      this.NodeId = nId;
      this.Pid = pId;
    }
  }

  public class MailBox
  {
    LinkedList<object> messages = new LinkedList<object>();

    public void Add(object msg)
    {
      messages.AddLast(msg);
    }

    public object Read()
    {
      object msg = messages.First;
      messages.RemoveFirst();
      return msg;
    }
  }
}