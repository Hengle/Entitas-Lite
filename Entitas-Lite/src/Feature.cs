/*
 *	Entitas-Lite is a helper extension of Entitas(ECS framework for c# and Unity).
 *	Entitas-Lite focusses on easy development WITHOUT CodeGenerator of original Entitas.
 *	https://github.com/rocwood/Entitas-Lite
 */


namespace Entitas
{
	/// Simple Systems with automatic collections. Nested is not supported
	public class Feature : Systems
	{
		public Feature()
		{
			Init(DefaultFeature.Name);
		}

		public Feature(string name)
		{
			Init(name);
		}

		private void Init(string name)
		{
			FeatureHelper.CollectSystems(name, this);
		}
	}
}
