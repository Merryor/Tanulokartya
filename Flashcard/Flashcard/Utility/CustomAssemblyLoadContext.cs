using System;
using System.Reflection;
using System.Runtime.Loader;

namespace Flashcard.Utility
{
    /// <summary>
    /// This class represents the runtime's concept of a scope for assembly loading. 
    /// This helps to load the dll needed to generate the pdf.
    /// </summary>
    internal class CustomAssemblyLoadContext : AssemblyLoadContext
    {
        public IntPtr LoadUnmanagedLibrary(string absolutePath)
        {
            return LoadUnmanagedDll(absolutePath);
        }
        protected override IntPtr LoadUnmanagedDll(string unmanagedDllName)
        {
            return LoadUnmanagedDllFromPath(unmanagedDllName);
        }

        protected override Assembly Load(AssemblyName assemblyName)
        {
            throw new NotImplementedException();
        }
    }
}
