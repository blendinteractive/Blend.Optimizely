using System;

namespace Blend.Optimizely
{
    [Flags]
    public enum LinkOptions
    {
        None = 0,

        /// <summary>
        /// Does not follow internal shortcuts and will render the original link path (usually resulting in a redirect)
        /// </summary>
        IgnoreInternalShortcuts = 1,

        /// <summary>
        /// Will include the domain (as defined by the Site Definition) in the resolved URL. Useful for canonical links.
        /// </summary>
        IncludeDomain = 2
    }
}
