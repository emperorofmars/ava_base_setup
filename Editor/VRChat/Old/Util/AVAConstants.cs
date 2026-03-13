
using System.Collections.Generic;

namespace com.squirrelbite.ava_base_setup.vrchat.oooold
{
	public static class AVAConstantsVRC
	{
		public const string OUTPUT_PATH = "Packages/com.squirrelbite.ava_base_setup/Output/";
		public const string ASSET_PATH = "Packages/com.squirrelbite.ava_base_setup/Assets/";
		public static Dictionary<string, int> ControllerTypeToIndex = new()
		{
			{"Base", 0},
			{"Additive", 1},
			{"Gesture", 2},
			{"Action", 3},
			{"FX", 4},
		};
	}
}
