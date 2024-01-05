using JDE2.Managers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace JDE2.Utils
{
    public static class ReflectionTool
    {

        public static List<Type> GetTypesFromInterface(List<Assembly> assemblies, string interfaceName)
        {
            List<Type> allTypes = new List<Type>();
            foreach (Assembly assembly in assemblies)
            {
                allTypes.AddRange(GetTypesFromInterface(assembly, interfaceName));
            }
            return allTypes;
            
        }

        public static List<Type> GetTypesFromInterface(List<Assembly> assemblies, Type typeInterface)
        {
            List<Type> allTypes = new List<Type>();
            foreach (Assembly assembly in assemblies)
            {
                allTypes.AddRange(GetTypesFromInterface(assembly, typeInterface));
            }
            return allTypes;
        }

        public static List<Type> GetTypesFromInterface<T>(List<Assembly> assemblies)
        {
            return GetTypesFromInterface(assemblies, typeof(T));
        }

        public static List<Type> GetTypesFromInterface(Assembly assembly, string interfaceName)
        {
            List<Type> allTypes = new List<Type>();
            Type[] types;
            try
            {
                types = assembly.GetTypes();
            }
            catch (ReflectionTypeLoadException e)
            {
                types = e.Types;
            }
            foreach (Type type in types.Where(t => t != null))
            {
                if (type.GetInterface(interfaceName) != null)
                {
                    allTypes.Add(type);
                }
            }
            return allTypes;
        }

        public static List<Type> GetTypesFromInterface(Assembly assembly, Type typeInterface)
        {
            List<Type> allTypes = new List<Type>();
            Type[] types;
            try
            {
                types = assembly.GetTypes();
            }
            catch (ReflectionTypeLoadException e)
            {
                types = e.Types;
            }
            foreach (Type type in types.Where(t => t != null))
            {
                if (type.GetInterface(typeInterface.Name) != null)
                {
                    allTypes.Add(type);
                }
            }
            return allTypes;
        }

        public static List<Type> GetTypesFromInterface<T>(Assembly assembly)
        {
            return GetTypesFromInterface(assembly, typeof(T));
        }

        public static Dictionary<string, string> GetAssembliesFromDirectory(string directory, string extension = "*.dll")
        {
            Dictionary<string, string> l = new Dictionary<string, string>();
            IEnumerable<FileInfo> libraries = new DirectoryInfo(directory).GetFiles(extension, SearchOption.AllDirectories);
            foreach (FileInfo library in libraries)
            {
                try
                {
                    AssemblyName name = AssemblyName.GetAssemblyName(library.FullName);
                    l.Add(name.FullName, library.FullName);
                }
                catch { }
            }
            return l;
        }

        /// <summary>
        /// Replacement for GetAssembliesFromDirectory using AssemblyName as key rather than string.
        /// </summary>
        private static Dictionary<AssemblyName, string> FindAssembliesInDirectory(string directory)
        {
            Dictionary<AssemblyName, string> l = new Dictionary<AssemblyName, string>();
            IEnumerable<FileInfo> libraries = new DirectoryInfo(directory).GetFiles("*.dll", SearchOption.AllDirectories);
            foreach (FileInfo library in libraries)
            {
                try
                {
                    AssemblyName name = AssemblyName.GetAssemblyName(library.FullName);
                    l.Add(name, library.FullName);
                }
                catch { }
            }
            return l;
        }

        public static List<Assembly> LoadAssembliesFromDirectory(string directory, string extension = "*.dll")
        {
            List<Assembly> assemblies = new List<Assembly>();
            IEnumerable<FileInfo> pluginsLibraries = new DirectoryInfo(directory).GetFiles(extension, SearchOption.AllDirectories);

            foreach (FileInfo library in pluginsLibraries)
            {
                try
                {
                    var assem = Assembly.LoadFile(library.FullName);
                    assemblies.Add(assem);
                }
                catch
                {

                }
            }
            return assemblies;
        }

        public const BindingFlags ReflectionFlags = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;

        public static FieldInfo GetField<T>(string path)
        {
            FieldInfo obj = null;
            Type type = typeof(T);
            if (path.Contains('.'))
            {
                string[] portions = path.Split('.');

                for (int i = 0; i < portions.Length; i++)
                {
                    string text = portions[i];
                    FieldInfo field = type.GetField(text, ReflectionFlags);
                    if (field == null) return obj;
                    if (i == portions.Length - 1)
                    {
                        obj = field;
                    }
                    else
                    {
                        type = field.FieldType;
                    }
                }
            }
            else
            {
                obj = type.GetField(path, ReflectionFlags);
            }
            return obj;
        }


    }
}

