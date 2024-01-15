using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;

namespace NativeLibraryLoader
{
    /// <summary>
    /// Enumerates possible library load targets.
    /// </summary>
    public abstract class PathResolver
    {
        /// <summary>
        /// Returns an enumerator which yields possible library load targets, in priority order.
        /// </summary>
        /// <param name="name">The name of the library to load.</param>
        /// <returns>An enumerator yielding load targets.</returns>
        public abstract IEnumerable<string> EnumeratePossibleLibraryLoadTargets(string name);

        /// <summary>
        /// Gets a default path resolver.
        /// </summary>
        public static PathResolver Default { get; } = new DefaultPathResolver();
    }

    /// <summary>
    /// Enumerates possible library load targets. This default implementation returns the following load targets:
    /// First: The library contained in the applications base folder.
    /// Second: The simple name, unchanged.
    /// Third: The library as resolved via the default DependencyContext, in the default nuget package cache folder.
    /// </summary>
    public class DefaultPathResolver : PathResolver
    {
        /// <summary>
        /// Returns an enumerator which yields possible library load targets, in priority order.
        /// </summary>
        /// <param name="name">The name of the library to load.</param>
        /// <returns>An enumerator yielding load targets.</returns>
        public override IEnumerable<string> EnumeratePossibleLibraryLoadTargets(string name)
        {
            if (!string.IsNullOrEmpty(AppContext.BaseDirectory))
            {
                yield return Path.Combine(AppContext.BaseDirectory, name);
            }
            yield return name;
        }

        private string GetUserDirectory()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                return Environment.GetEnvironmentVariable("USERPROFILE");
            }
            else
            {
                return Environment.GetEnvironmentVariable("HOME");
            }
        }
    }
}
