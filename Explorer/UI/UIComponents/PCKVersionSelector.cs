namespace GodotPCKExplorer.UI.UIComponents
{
    public partial class PCKVersionSelector : UserControl
    {
        public readonly struct VersionChanges(PCKVersion value, bool isValidityChanged, bool isValid, bool isPackChanged, bool isMajorChanged, bool isMinorChanged, bool isRevisionChanged)
        {
            public readonly PCKVersion Value = value;
            public readonly bool IsValidityChanged = isValidityChanged;
            public readonly bool IsValid = isValid;
            public readonly bool IsPackChanged = isPackChanged;
            public readonly bool IsMajorChanged = isMajorChanged;
            public readonly bool IsMinorChanged = isMinorChanged;
            public readonly bool IsRevisionChanged = isRevisionChanged;
        }

        public event EventHandler<VersionChanges>? VersionChanged;
        public event EventHandler<int>? PackChanged;
        public event EventHandler<int>? MajorChanged;
        public event EventHandler<int>? MinorChanged;
        public event EventHandler<int>? RevisionChanged;

        PCKVersion prev_version;

        bool called_by_code = true;

        public PCKVersionSelector()
        {
            InitializeComponent();

            cb_ver.Items.Clear();

            foreach (var p in PCKUtils.VersionLimits.Keys)
            {
                cb_ver.Items.Add(p.ToString());
            }
        }

        public void SetVersion(int pack, int major, int minor, int revision)
        {
            called_by_code = true;

            cb_ver.Text = pack.ToString();
            nud_major.Value = major;
            nud_minor.Value = minor;
            nud_revision.Value = revision;

            called_by_code = false;

            prev_version = new PCKVersion(pack, major, minor, revision);
            SetInvalidStatusBorder(!PCKUtils.IsVersionInAllowedLimits(prev_version));

            UpdatePackVersionSelection();
            UpdateVersionNudMinMaxes();
        }

        public void SetVersion(PCKVersion ver)
        {
            SetVersion(ver.Pack, ver.Major, ver.Minor, ver.Revision);
        }

        public PCKVersion GetVersion(bool silent = false)
        {
            if (!int.TryParse((string)(cb_ver.SelectedItem ?? ""), out int pack_ver))
            {
                if (!silent)
                    Program.ShowMessage("Incorrect package version format.", "Error", MessageType.Error);

                return new PCKVersion(-1, -1, -1, -1);
            }

            int major = (int)nud_major.Value, minor = (int)nud_minor.Value, revision = (int)nud_revision.Value;

            if (!PCKUtils.IsVersionInAllowedLimits(new(pack_ver, major, minor, revision)))
            {
                if (PCKUtils.VersionLimits.TryGetValue(pack_ver, out PCKUtils.VersionLimitRanges ranges))
                {
                    if (!silent)
                        Program.ShowMessage($"Incorrect PCK version.\nValid versions are in the range (inclusive): {ranges.MinMajor}.{ranges.MinMinor}.{ranges.MinRevision} - {ranges.MaxMajor}.{ranges.MaxMinor}.{ranges.MaxRevision}", "Error", MessageType.Error);
                }
                else
                {
                    if (!silent)
                        Program.ShowMessage("Incorrect PCK version.", "Error", MessageType.Error);
                }

                return new PCKVersion(-1, -1, -1, -1);
            }

            return new PCKVersion(pack_ver, major, minor, revision);
        }

        public PCKVersion GetRawVersion()
        {
            int pack = -1;
            int idx = cb_ver.Items.IndexOf(cb_ver.SelectedItem);
            if (idx != -1)
            {
                pack = int.Parse((string)(cb_ver.Items[idx] ?? "-1"));
            }

            return new PCKVersion(pack, (int)nud_major.Value, (int)nud_minor.Value, (int)nud_revision.Value);
        }

        void SetInvalidStatusBorder(bool invalid)
        {
            if (invalid)
            {
                red_panel.Visible = true;
                tableLayoutPanel1.BackColor = Color.Firebrick;
            }
            else
            {
                red_panel.Visible = false;
                tableLayoutPanel1.BackColor = SystemColors.Control;
            }
        }

        void UpdateVersionNudMinMaxes()
        {
            called_by_code = true;

            if (int.TryParse((string)(cb_ver.SelectedItem ?? ""), out int ver))
            {
                if (PCKUtils.VersionLimits.TryGetValue(ver, out PCKUtils.VersionLimitRanges ranges))
                {
                    nud_major.Minimum = ranges.MinMajor;
                    nud_major.Maximum = Math.Min(ranges.MaxMajor, (ushort)99);

                    nud_minor.Minimum = 0;
                    nud_minor.Maximum = ushort.MaxValue;

                    nud_revision.Minimum = 0;
                    nud_revision.Maximum = ushort.MaxValue;
                }
            }

            called_by_code = false;
        }

        void EmitEvents()
        {
            var cur_ver = GetRawVersion();
            if (cur_ver != prev_version)
            {
                var is_valid = PCKUtils.IsVersionInAllowedLimits(cur_ver);
                SetInvalidStatusBorder(!is_valid);

                var changes = new VersionChanges(cur_ver,
                    is_valid != PCKUtils.IsVersionInAllowedLimits(prev_version),
                    is_valid,
                    cur_ver.Pack != prev_version.Pack,
                    cur_ver.Major != prev_version.Major,
                    cur_ver.Minor != prev_version.Minor,
                    cur_ver.Revision != prev_version.Revision);

                VersionChanged?.Invoke(this, changes);

                if (changes.IsPackChanged)
                    PackChanged?.Invoke(this, cur_ver.Pack);

                if (changes.IsMajorChanged)
                    MajorChanged?.Invoke(this, cur_ver.Major);

                if (changes.IsMinorChanged)
                    MinorChanged?.Invoke(this, cur_ver.Minor);

                if (changes.IsRevisionChanged)
                    RevisionChanged?.Invoke(this, cur_ver.Revision);

                prev_version = cur_ver;
            }
        }

        void UpdatePackVersionSelection()
        {
            called_by_code = true;

            int idx = cb_ver.Items.IndexOf(cb_ver.Text);
            if (idx != -1)
            {
                if (cb_ver.SelectedIndex != idx)
                {
                    cb_ver.SelectedIndex = idx;

                    EmitEvents();
                }
            }
            else
            {
                cb_ver.SelectedIndex = cb_ver.Items.Count - 1;

                EmitEvents();
            }

            called_by_code = false;
        }

        private void cb_ver_Leave(object sender, EventArgs e)
        {
            if (called_by_code)
                return;

            UpdatePackVersionSelection();
            UpdateVersionNudMinMaxes();
        }

        private void cb_ver_SelectionChangeCommitted(object sender, EventArgs e)
        {
            if (called_by_code)
                return;

            UpdateVersionNudMinMaxes();
            EmitEvents();
        }

        private void nud_major_ValueChanged(object sender, EventArgs e)
        {
            if (called_by_code)
                return;

            EmitEvents();
        }

        private void nud_minor_ValueChanged(object sender, EventArgs e)
        {
            if (called_by_code)
                return;

            EmitEvents();
        }

        private void nud_revision_ValueChanged(object sender, EventArgs e)
        {
            if (called_by_code)
                return;

            EmitEvents();
        }
    }
}