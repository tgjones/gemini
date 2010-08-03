using System;
using System.Collections.Generic;
using Caliburn.Core;
using Gemini.Framework.Ribbon;
using Gemini.Framework.Services;

namespace Gemini.Framework
{
	public abstract class ModuleBase : IModule
	{
		private IShell _shell;
		private IContainer _container;

		public IShell Shell
		{
			get { return _shell; }
		}

		public IRibbon Ribbon
		{
			get { return _shell.Ribbon; }
		}

		public IContainer Container
		{
			get { return _container; }
		}

		public void Configure(IContainer container)
		{
			_container = container;
			_container.ConfigureWith(GetComponents());
		}

		protected virtual IEnumerable<ComponentInfo> GetComponents()
		{
			yield break;
		}

		public void Start()
		{
			_shell = _container.GetInstance<IShell>();
			Initialize();
		}

		protected virtual void Initialize()
		{

		}

		protected ComponentInfo Singleton<TService, TImplementation>()
				where TImplementation : TService
		{
			return Singleton(typeof(TService), typeof(TImplementation));
		}

		protected ComponentInfo Singleton(Type service, Type implementation)
		{
			return new ComponentInfo
			{
				Service = service,
				Implementation = implementation,
				Lifetime = ComponentLifetime.Singleton
			};
		}

		protected ComponentInfo PerRequest<TService, TImplementation>()
				where TImplementation : TService
		{
			return PerRequest(typeof(TService), typeof(TImplementation));
		}

		protected ComponentInfo PerRequest(Type service, Type implementation)
		{
			return new ComponentInfo
			{
				Service = service,
				Implementation = implementation,
				Lifetime = ComponentLifetime.PerRequest
			};
		}
	}
}