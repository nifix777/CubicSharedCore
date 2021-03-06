﻿using System;
using System.Threading;

namespace Cubic.Core.Console
{
  public class Spinner : IDisposable
  {
    private readonly IConsole _console;
    private const string Sequence = @"/-\|";
    private int _counter;
    private readonly int _delay;
    private bool _active;
    private Thread _thread;

    public Spinner(IConsole console)
    {
      _console = console;
      _delay = 100;
      _thread = new Thread(Spin);
    }

    public void Start()
    {
      _console.CursorVisible = false;
      _active = true;
      if (_thread.IsAlive)
        return;
      _thread.Abort();
      _thread = new Thread(Spin);
      _thread.Start();
    }

    public void Stop()
    {
      _active = false;
      Draw(' ');
      _console.CursorVisible = true;
    }

    private void Spin()
    {
      while (_active)
      {
        Turn();
        Thread.Sleep(_delay);
      }
    }

    private void Draw(char c)
    {
      try
      {
        _console.SetCursorPosition(_console.CursorLeft <= 0 ? 0 : _console.CursorLeft - 1, _console.CursorTop);
        _console.Write(c);
      }
      catch (Exception e)
      {
        _console.Write(e);
      }
    }

    private void Turn()
    {
      Draw(Sequence[++_counter % Sequence.Length]);
    }

    public void Dispose()
    {
      Stop();
    }
  }
}