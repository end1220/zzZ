
using System;



namespace CompileDll
{
	class Program
	{
		static DllCompiler cmp = new DllCompiler();

		static void Main(string[] args)
		{
			cmp.CompileRuntimeDll();

			Console.ReadLine();
		}
	}
}
