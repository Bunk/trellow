﻿using System;
using System.IO.IsolatedStorage;
using System.Xml.Linq;
using Strilanc.Value;

namespace Trellow.Diagnostics
{
    public static class AppVersion
    {
        private const string VersionKey = "LatestAppVersion";

        private static readonly May<Version> _current = PullVersionFromProperties();

        public static May<Version> Current
        {
            get { return _current; }
        }

        public static May<Version> GetPreviousVersion()
        {
            var settings = IsolatedStorageSettings.ApplicationSettings;
            if (settings.Contains(VersionKey))
            {
                var version = (string) settings[VersionKey];
                return new Version(version);
            }
            return May<Version>.NoValue;
        }

        public static void UpdatePreviousVersion(Version version)
        {
            var settings = IsolatedStorageSettings.ApplicationSettings;
            settings[VersionKey] = version.ToString(4);
        }

        private static May<Version> PullVersionFromProperties()
        {
            var manifest = XElement.Load("WMAppManifest.xml");
            var version = manifest
                .Elements("App").MayFirst()
                .Select(node => node.Attribute("Version"))
                .Select(attr => new Version(attr.Value));

            return version;
        }
    }
}