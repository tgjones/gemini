using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Caliburn.Core;
using Caliburn.StructureMap;
using Gemini.Framework;
using Microsoft.Practices.ServiceLocation;
using StructureMap;

namespace Gemini
{
	public class Registry
	{
		public IServiceLocator CreateContainer()
		{
			var container = new Container(RegisterAll<IModule>);
			return new StructureMapAdapter(container);
		}

		private static void RegisterAll<T>(ConfigurationExpression expression)
		{
			IEnumerable<Assembly> assemblies =
				Directory.EnumerateFiles(AppDomain.CurrentDomain.BaseDirectory, "*.dll")
				.Union(Directory.EnumerateFiles(AppDomain.CurrentDomain.BaseDirectory, "*.exe"))
				.Select(f => Assembly.LoadFrom(f));

			IEnumerable<Type> allTypes = assemblies.SelectMany(a => a.GetExportedTypes())
				.Where(type => typeof(T).IsAssignableFrom(type) && !type.IsAbstract && !type.IsInterface);

			allTypes.Apply(type =>
				expression.For(typeof(T))
				.Singleton()
				.Add(type)
			);
		}
	}
}