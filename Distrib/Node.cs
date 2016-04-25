using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace Distrib
{
  public class Node
  {
    private NodeId _id;
    private NodeController _nodeController;
    private int _pIdCounter = 0;
    private object _lock = new object();

    public Node(string host, int port)
    {
      _id = new NodeId(host,port);
      _nodeController = new NodeController(_id);
    }

    public void Init(Action action)
    {
      int pid = GetNewProcessId();
      var p = new Process(_id,pid,action);
      if (!_nodeController.AddProcess(p))
      {
        throw new Exception("Could not add Process to ProcessGroup");
      }

      p.StartProcess();
    }

    public void ShutDown()
    {
      _nodeController.ShutDown();
    }


    private int GetNewProcessId()
    {
      int pid;
      lock (_lock)
      {
        pid = _pIdCounter;
        _pIdCounter++;
      }

      return pid;
    }
  }

  public class NodeId
  {
    public string Host {get; set;}
    public int Port {get; set;}

    public NodeId (string host, int port)
    {
      this.Host = host;
      this.Port = port;
    }
  }

  public class NodeController
  {
    private readonly ConcurrentDictionary<ProcessId, Process> _processGroup;
    private readonly List<TcpClient> _connections;
    private readonly TcpListener _listener;
    private readonly NodeId _id;
    private bool _alive;

    public NodeController(NodeId id)
    {
      _processGroup = new ConcurrentDictionary<ProcessId,Process>();
      _connections = new List<TcpClient>();
      this._id = id;
      _alive = true;

      try
      {
        _listener = StartTcp();
        Thread messageReceiver = new Thread(WaitForConnections);
        messageReceiver.Start();
      }
      catch(Exception e)
      {
        Console.WriteLine(e.ToString());
      }
    }

    private void WaitForConnections()
    {
      while (_alive)
      {
        Console.WriteLine("Start waiting for connections");
        TcpClient client = _listener.AcceptTcpClient();
        _connections.Add(client);
        Console.WriteLine("Accepted a connection");
        Task t = new Task(() => HandleConnection(client));
        t.Start();
      }
    }

    private void HandleConnection(TcpClient client)
    {
      NetworkStream stream = client.GetStream();

      using(stream)
      {
        IFormatter formatter = new BinaryFormatter();
        while(client.Connected) //Keep receiving messages as long as the connection remains
        {
          Message msg = (Message) formatter.Deserialize(stream);
          Process p = _processGroup[msg.ProcessId];
          p.MailBox.Add(msg.Msg); //Add the message to the mailbox of the corresponding process
        }
        _connections.Remove(client); //Connection is closed
      }


    }

    private TcpListener StartTcp()
    {
      IPAddress ipAddress = IPAddress.Parse(_id.Host);
      TcpListener listener = new TcpListener(ipAddress,_id.Port);
      listener.Start();
      return listener;
    }

    public bool AddProcess(Process p)
    {
      return _processGroup.TryAdd(p.ProcessId, p);
    }

    public void ShutDown()
    {
      _alive = false; //Stop the listener loop
      _connections.ForEach(x => x.Close()); //Shutdown all connections
      _listener.Stop(); //Stop the listener itself
    }

    //Force shutdown??

  }
}