using System;
using System.Collections.Generic;

namespace Trellow.Diagnostics
{
    public static class Analytics
    {
        public static LocalyticsSession Session { get; set; }

        public static void CreateAndStartAnalyticsSession()
        {
#if DEBUG
            Session = new LocalyticsSession("084443c212918bc1314eed4-c54e6f14-a2ef-11e2-9a95-00c76edb34ae");
#else
            Session = new LocalyticsSession("a100e3d768f37ed322e953f-64164842-a2eb-11e2-f180-0086c15f90fa");
#endif
            Session.Open();
            Session.Upload();
        }

        public static void CloseSession()
        {
            Session.Close();
        }

        public static void LogException(Exception ex, Dictionary<string, string> attributes = null)
        {
            var attr = attributes.Merge(new Dictionary<string, string>
            {
                {"Exception", ex.Message},
                {"Stack", ex.StackTrace}
            });
            Session.TagEvent("Exception", attr);
        }

        public static void LogWarning(Exception ex, Dictionary<string, string> attributes = null)
        {
            var attr = attributes.Merge(new Dictionary<string, string>
            {
                {"Exception", ex.Message},
                {"Stack", ex.StackTrace}
            });
            Session.TagEvent("Warning", attr);
        }

        public static void TagEvent(string tag, Dictionary<string, string> attributes = null)
        {
            Session.TagEvent(tag, attributes);
        }
    }
}