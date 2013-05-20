using System;
using Caliburn.Micro;

namespace Gemini.Framework
{
	public abstract class Window : PropertyChangedBase, IWindow
	{
	    public void Activate()
	    {
	    }

	    public bool IsActive { get; private set; }
	    public event EventHandler<ActivationEventArgs> Activated;
	    public void Deactivate(bool close)
	    {
	    }

	    public event EventHandler<DeactivationEventArgs> AttemptingDeactivation;
	    public event EventHandler<DeactivationEventArgs> Deactivated;
	    public void TryClose()
	    {
	    }

	    public bool IsVisible { get; set; }
	}
}