using System.Text.RegularExpressions;

namespace GodotPCKExplorer
{
    public struct PCKVersion
    {
        const string version_string_pattern = @"^([0-9]{1,2})\.([0-9]{1,2})\.([0-9]{1,2})\.([0-9]{1,2})$";
        static readonly Regex VersionStringRegEx = new Regex(version_string_pattern);

        public int PackVersion { get; set; }
        public int Major { get; set; }
        public int Minor { get; set; }
        public int Revision { get; set; }

        public readonly bool IsValid => PackVersion >= 0 && Major >= 0 && Minor >= 0 && Revision >= 0;

        public PCKVersion(int pck_version, int major, int minor, int revision)
        {
            PackVersion = pck_version;
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
                PackVersion = int.Parse(digits[0]);
                Major = int.Parse(digits[1]);
                Minor = int.Parse(digits[2]);
                Revision = int.Parse(digits[3]);
            }
            else
            {
                PackVersion = -1;
                Major = -1;
                Minor = -1;
                Revision = -1;
            }
        }

        public override readonly string ToString()
        {
            return $"{PackVersion}.{Major}.{Minor}.{Revision}";
        }
    }

}
