﻿using System;
using System.Reflection;

namespace Cubic.Core.Runtime
{
  public class WeakAction
  {
    private Action _staticAction;

    /// <summary>
    /// Gets or sets the <see cref="MethodInfo" /> corresponding to this WeakAction's
    /// method passed in the constructor.
    /// </summary>
    protected MethodInfo Method
    {
      get;
      set;
    }

    /// <summary>
    /// Gets the name of the method that this WeakAction represents.
    /// </summary>
    public virtual string MethodName
    {
      get
      {
        if (_staticAction != null)
        {
          return _staticAction.Method.Name;
        }

        return Method.Name;
      }
    }

    /// <summary>
    /// Gets or sets a WeakReference to this WeakAction's action's target.
    /// This is not necessarily the same as
    /// <see cref="Reference" />, for example if the
    /// method is anonymous.
    /// </summary>
    protected WeakReference ActionReference
    {
      get;
      set;
    }

    /// <summary>
    /// Saves the <see cref="ActionReference"/> as a hard reference. This is
    /// used in relation with this instance's constructor and only if
    /// the constructor's keepTargetAlive parameter is true.
    /// </summary>
    protected object LiveReference
    {
      get;
      set;
    }

    /// <summary>
    /// Gets or sets a WeakReference to the target passed when constructing
    /// the WeakAction. This is not necessarily the same as
    /// <see cref="ActionReference" />, for example if the
    /// method is anonymous.
    /// </summary>
    protected WeakReference Reference
    {
      get;
      set;
    }

    /// <summary>
    /// Gets a value indicating whether the WeakAction is static or not.
    /// </summary>
    public bool IsStatic
    {
      get
      {
        return _staticAction != null;
      }
    }

    /// <summary>
    /// Initializes an empty instance of the <see cref="WeakAction" /> class.
    /// </summary>
    protected WeakAction()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="WeakAction" /> class.
    /// </summary>
    /// <param name="action">The action that will be associated to this instance.</param>
    /// <param name="keepTargetAlive">If true, the target of the Action will
    /// be kept as a hard reference, which might cause a memory leak. You should only set this
    /// parameter to true if the action is using closures. See
    public WeakAction(Action action, bool keepTargetAlive = false)
        : this(action == null ? null : action.Target, action, keepTargetAlive)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="WeakAction" /> class.
    /// </summary>
    /// <param name="target">The action's owner.</param>
    /// <param name="action">The action that will be associated to this instance.</param>
    /// <param name="keepTargetAlive">If true, the target of the Action will
    /// be kept as a hard reference, which might cause a memory leak. You should only set this
    /// parameter to true if the action is using closures. See
    public WeakAction(object target, Action action, bool keepTargetAlive = false)
    {
      if (action.Method.IsStatic)
      {
        _staticAction = action;

        if (target != null)
        {
          // Keep a reference to the target to control the
          // WeakAction's lifetime.
          Reference = new WeakReference(target);
        }

        return;
      }

      Method = action.Method;
      ActionReference = new WeakReference(action.Target);

      LiveReference = keepTargetAlive ? action.Target : null;
      Reference = new WeakReference(target);

#if DEBUG
      if (ActionReference != null
          && ActionReference.Target != null
          && !keepTargetAlive)
      {
        var type = ActionReference.Target.GetType();

        if (type.Name.StartsWith("<>")
            && type.Name.Contains("DisplayClass"))
        {
          System.Diagnostics.Debug.WriteLine(
              "You are attempting to register a lambda with a closure without using keepTargetAlive. Are you sure? Check http://galasoft.ch/s/mvvmweakaction for more info.");
        }
      }
#endif
    }

    /// <summary>
    /// Gets a value indicating whether the Action's owner is still alive, or if it was collected
    /// by the Garbage Collector already.
    /// </summary>
    public virtual bool IsAlive
    {
      get
      {
        if (_staticAction == null
            && Reference == null
            && LiveReference == null)
        {
          return false;
        }

        if (_staticAction != null)
        {
          if (Reference != null)
          {
            return Reference.IsAlive;
          }

          return true;
        }

        // Non static action

        if (LiveReference != null)
        {
          return true;
        }

        if (Reference != null)
        {
          return Reference.IsAlive;
        }

        return false;
      }
    }

    /// <summary>
    /// Gets the Action's owner. This object is stored as a 
    /// <see cref="WeakReference" />.
    /// </summary>
    public object Target
    {
      get
      {
        if (Reference == null)
        {
          return null;
        }

        return Reference.Target;
      }
    }

    /// <summary>
    /// The target of the weak reference.
    /// </summary>
    protected object ActionTarget
    {
      get
      {
        if (LiveReference != null)
        {
          return LiveReference;
        }

        if (ActionReference == null)
        {
          return null;
        }

        return ActionReference.Target;
      }
    }

    /// <summary>
    /// Executes the action. This only happens if the action's owner
    /// is still alive.
    /// </summary>
    public void Execute()
    {
      if (_staticAction != null)
      {
        _staticAction();
        return;
      }

      var actionTarget = ActionTarget;

      if (IsAlive)
      {
        if (Method != null
            && (LiveReference != null
                || ActionReference != null)
            && actionTarget != null)
        {
          Method.Invoke(actionTarget, null);

          // ReSharper disable RedundantJumpStatement
          return;
          // ReSharper restore RedundantJumpStatement
        }

      }
    }

    /// <summary>
    /// Sets the reference that this instance stores to null.
    /// </summary>
    public void MarkForDeletion()
    {
      Reference = null;
      ActionReference = null;
      LiveReference = null;
      Method = null;
      _staticAction = null;

    }
  }
}