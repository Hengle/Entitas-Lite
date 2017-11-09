/*
 *	Entitas-Lite is a helper extension of Entitas(ECS framework for c# and Unity).
 *	Entitas-Lite focusses on easy development WITHOUT CodeGenerator of original Entitas.
 *	https://github.com/rocwood/Entitas-Lite
 */


using System;
using System.Collections.Generic;
using System.Linq;


namespace Entitas
{

	[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
	public class FeatureAttribute : Attribute
	{
		public readonly string featureName;
		public readonly int priority;

		public FeatureAttribute(string name="", int prior = 0) { featureName = name; priority = prior; }
	}
	

#if (!ENTITAS_DISABLE_VISUAL_DEBUGGING && UNITY_EDITOR)

	public class Feature : VisualDebugging.Unity.DebugSystems
	{
		public Feature(string name) : base(name)
		{
			FeatureHelper.CollectSystems(name, this);
		}
	}

#else

	public class Feature : Systems 
	{
		public Feature(string name) 
		{
			FeatureHelper.CollectSystems(name, this);
		}
	}

#endif

	/// Collect all matched System in current domain, then add them to Feature ordered by priority
	public class FeatureHelper
	{
		private class SystemProxy : IComparable<SystemProxy>
		{
			public ISystem system;
			public int priority;

			public SystemProxy(ISystem s, int prior) { system = s; priority = prior; }
			public int CompareTo(SystemProxy other) { return other.priority - priority; } // in descending order
		}

		public static void CollectSystems(string name, Systems feature)
		{
			var sysType = typeof(ISystem);
			var ssType = typeof(Systems);

			var types = AppDomain.CurrentDomain.GetAssemblies()
								.SelectMany(s => s.GetTypes())
								.Where(p => p.IsClass && p.IsPublic && !p.IsAbstract 
										&& sysType.IsAssignableFrom(p) 
										&& !ssType.IsAssignableFrom(p));

			var attribType = typeof(FeatureAttribute);
			var c = new List<SystemProxy>();

			foreach (var p in types)
			{
				var attribs = p.GetCustomAttributes(attribType, false);

				string n = string.Empty;
				int w = 0;

				if (attribs != null && attribs.Length > 0)
				{
					var attrib = (FeatureAttribute)attribs[0];
					n = attrib.featureName;
					w = attrib.priority;
				}
				
				if (n != name)
					continue;

				var system = (ISystem)Activator.CreateInstance(p);
				c.Add(new SystemProxy(system, w));
			}

			c.Sort();

			int count = c.Count;
			for (int i = 0; i < count; i++)
			{
				feature.Add(c[i].system);
			}
		}
	}
}