using System.Text.RegularExpressions;

namespace GodotPCKExplorer
{
    public struct PCKVersion
    {
        const string version_string_pattern = @"^([0-9]{1,2})\.([0-9]{1,2})\.([0-9]{1,2})\.([0-9]{1,2})$";
        static readonly Regex VersionStringRegEx = new Regex(version_string_pattern);

        public int Pack { get; set; }
        public int Major { get; set; }
        public int Minor { get; set; }
        public int Revision { get; set; }

        public PCKVersion(int pckVersion, int major, int minor, int revision)
        {
            Pack = pckVersion;
            Major = major;
            Minor = minor;
            Revision = revision;
        }

        public PCKVersion(string version)
        {
            var tmpCheck = VersionStringRegEx.Match(version.Trim());
            if (tmpCheck.Success)
            {
                var digits = tmpCheck.Value.Split('.');
                Pack = int.Parse(digits[0]);
                Major = int.Parse(digits[1]);
                Minor = int.Parse(digits[2]);
                Revision = int.Parse(digits[3]);
            }
            else
            {
                Pack = -1;
                Major = -1;
                Minor = -1;
                Revision = -1;
            }
        }

        public override readonly string ToString()
        {
            return $"{Pack}.{Major}.{Minor}.{Revision}";
        }

        public readonly bool IsValid()
        {
            return Pack >= 0 && Major >= 0 && Minor >= 0 && Revision >= 0;
        }

        public override readonly bool Equals(object obj)
        {
            return obj is PCKVersion v && this == v;
        }

        public override readonly int GetHashCode()
        {
            return Pack.GetHashCode() ^ Major.GetHashCode() ^ Minor.GetHashCode() ^ Revision.GetHashCode();
        }

        public static bool operator ==(PCKVersion a, PCKVersion b)
        {
            return a.Pack == b.Pack && a.Major == b.Major && a.Minor == b.Minor && a.Revision == b.Revision;
        }

        public static bool operator !=(PCKVersion a, PCKVersion b)
        {
            return !(a == b);
        }
    }
}
