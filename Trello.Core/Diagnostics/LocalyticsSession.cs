using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.IO.IsolatedStorage;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Windows;
using Microsoft.Phone.Info;

public class LocalyticsSession
{
    #region library constants

    private const int maxStoredSessions = 10;
    private const int maxNameLength = 100;
    private const string libraryVersion = "windowsphone_2.2";
    private const string directoryName = "localytics";
    private const string sessionFilePrefix = "s_";
    private const string uploadFilePrefix = "u_";
    private const string metaFileName = "m_meta";

    private const string serviceURLBase = "http://analytics.localytics.com/api/v2/applications/";

    #endregion

    #region private members

    private readonly string appKey;
    private string sessionUuid;
    private string sessionFilename;
    private bool isSessionOpen;
    private bool isSessionClosed;
    private double sessionStartTime;

    #endregion

    #region static members

    private static bool isUploading;
    private static IsolatedStorageFile localStorage;

    #endregion

    #region private methods

    #region Storage

    /// <summary>
    /// Caches the reference to the app's isolated storage
    /// </summary>
    private static IsolatedStorageFile getStore()
    {
        if (localStorage == null)
        {
            localStorage = IsolatedStorageFile.GetUserStoreForApplication();
        }

        return localStorage;
    }

    /// <summary>
    /// Tallies up the number of files whose name starts w/ sessionFilePrefix in the localytics dir
    /// </summary>
    private static int getNumberOfStoredSessions()
    {
        var store = getStore();
        if (store.DirectoryExists(directoryName) == false)
        {
            return 0;
        }
        return store.GetFileNames(directoryName + @"\" + sessionFilePrefix + "*").Length;
    }

    /// <summary>
    /// Gets a stream pointing to the requested file.  If the file does not exist it is created. 
    /// If the file does exist the stream points to the end of the file
    /// </summary>
    /// <param name="filename">Name of the file (w/o directory) to create</param>
    private static IsolatedStorageFileStream getStreamForFile(string filename)
    {
        var store = getStore();
        store.CreateDirectory(directoryName); // does nothing if dir exists
        return new IsolatedStorageFileStream(directoryName + @"\" + filename, FileMode.Append, store);
    }

    /// <summary>
    /// Appends a string to the end of a text file
    /// </summary>
    /// <param name="text">Text to append</param>
    /// <param name="filename">Name of file to append to</param>
    private static void appendTextToFile(string text, string filename)
    {
        var file = getStreamForFile(filename);
        TextWriter writer = new StreamWriter(file);
        writer.Write(text);
        writer.Close();
        file.Close();
    }

    /// <summary>
    /// Reads a file and returns its contents as a string
    /// </summary>
    /// <param name="filename">file to read (w/o directory prefix)</param>
    /// <returns>the contents of the file</returns>
    private static string GetFileContents(string filename)
    {
        var store = getStore();
        var file = store.OpenFile(directoryName + @"\" + filename, FileMode.Open);
        TextReader reader = new StreamReader(file);
        var contents = reader.ReadToEnd();
        reader.Close();
        file.Close();
        return contents;
    }

    #endregion

    #region upload

    /// <summary>
    /// Goes through all the upload files and collects their contents for upload
    /// </summary>
    /// <returns>A string containing the concatenated </returns>
    private static string GetUploadContents()
    {
        var contents = new StringBuilder();
        var store = getStore();

        if (store.DirectoryExists(directoryName))
        {
            var files = store.GetFileNames(directoryName + @"\" + uploadFilePrefix + "*");
            foreach (var file in files)
            {
                if (file.StartsWith(uploadFilePrefix)) // workaround for GetFileNames bug
                {
                    contents.Append(GetFileContents(file));
                }
            }
        }
        return contents.ToString();
    }

    /// <summary>
    /// loops through all the files in the directory deleting the upload files
    /// </summary>
    private static void DeleteUploadFiles()
    {
        var store = getStore();
        if (store.DirectoryExists(directoryName))
        {
            var files = store.GetFileNames(directoryName + @"\" + uploadFilePrefix + "*");
            foreach (var file in files)
            {
                if (file.StartsWith(uploadFilePrefix)) // workaround for GetfileNames returning extra files
                {
                    store.DeleteFile(directoryName + @"\" + file);
                }
            }
        }
    }

    /// <summary>
    /// Rename any open session files. This way events recorded during uploaded get written safely to disk
    /// and threading difficulties are missed.
    /// </summary>
    private void renameOrAppendSessionFiles()
    {
        var store = getStore();
        if (store.DirectoryExists(directoryName))
        {
            var files = store.GetFileNames(directoryName + @"\" + sessionFilePrefix + "*");
            var destinationFilename = uploadFilePrefix + Guid.NewGuid().ToString();

            var addedHeader = false;
            foreach (var file in files)
            {
                if (file.StartsWith(sessionFilePrefix)) // work around for GetFileNames returning extra files
                {
                    // Any time sessions are appended, an upload header should be added. But only one is needed regardless of number of files added
                    if (!addedHeader)
                    {
                        appendTextToFile(GetBlobHeader(), destinationFilename);
                        addedHeader = true;
                    }

                    appendTextToFile(GetFileContents(file), destinationFilename);
                    store.DeleteFile(directoryName + @"\" + file);
                }
            }
        }
    }

    /// <summary>
    /// Runs on a seperate thread and is responsible for renaming and uploading files as appropriate
    /// </summary>
    private async void BeginUpload()
    {
        LogMessage("Beginning upload.");

        try
        {
            renameOrAppendSessionFiles();
            // begin the upload
            var url = serviceURLBase + this.appKey + "/uploads";
            LogMessage("Uploading to: " + url);

            var client = new HttpClient();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            HttpContent content = new StringContent(GetUploadContents(), Encoding.UTF8, "application/json");
            var response = await client.PostAsync(url, content);
            //HttpResponseMessage response = await client.PostAsync(url, content).ConfigureAwait(false);
            LogMessage("Upload complete. Response: " + response.Content.ReadAsStringAsync());
            DeleteUploadFiles();
        }
        catch (Exception e)
        {
            LogMessage("Swallowing exception: " + e.Message);
        }
        finally
        {
            isUploading = false;
        }
    }

    #endregion

    #region Data Looklups

    /// <summary>
    /// Retreives a unique identifier for this device.  According to Microsoft, this identifier is
    /// unique across all carriers and devices
    /// </summary>
    private static string GetDeviceId()
    {
        var id = (byte[]) DeviceExtendedProperties.GetValue("DeviceUniqueId");
        return Convert.ToBase64String(id);
    }

    /// <summary>
    /// Gets the sequence number for the next upload blob. 
    /// </summary>
    /// <returns>Sequence number as a string</returns>
    private static string GetSequenceNumber()
    {
        // open the meta file and read the next sequence number.
        var store = getStore();
        var metaFile = directoryName + @"\" + metaFileName;
        if (!store.FileExists(metaFile))
        {
            SetNextSequenceNumber("1");
            return "1";
        }

        var file = store.OpenFile(directoryName + @"\" + metaFileName, FileMode.Open);
        TextReader reader = new StreamReader(file);
        var installID = reader.ReadLine();
        var sequenceNumber = reader.ReadLine();
        reader.Close();
        file.Close();
        return sequenceNumber;
    }

    /// <summary>
    /// Sets the next sequence number in the metadata file. Creates the file if its not already there
    /// </summary>
    /// <param name="number">Next sequence number</param>
    private static void SetNextSequenceNumber(string number)
    {
        var store = getStore();
        var metaFile = directoryName + @"\" + metaFileName;
        if (!store.FileExists(metaFile))
        {
            // Create a new metadata file consisting of a unique installation ID and a sequence number
            appendTextToFile(Guid.NewGuid().ToString() + Environment.NewLine + number, metaFileName);
        }
        else
        {
            var fileIn = store.OpenFile(metaFile, FileMode.Open);
            TextReader reader = new StreamReader(fileIn);
            var installId = reader.ReadLine();
            reader.Close();
            fileIn.Close();

            // overwite the file w/ the old install ID and the new sequence number
            var fileOut = store.OpenFile(metaFile, FileMode.Truncate);
            TextWriter writer = new StreamWriter(fileOut);
            writer.WriteLine(installId);
            writer.Write(number);
            writer.Close();
            fileOut.Close();
        }
    }

    /// <summary>
    /// Gets the timestamp of the storage file containing the sequence numbers. This allows processing to
    /// ignore duplicates or identify missing uploads
    /// </summary>
    /// <returns>A string containing a Unixtime</returns>
    private static string GetPersistStoreCreateTime()
    {
        var store = getStore();
        var metaFile = directoryName + @"\" + metaFileName;
        if (!store.FileExists(metaFile))
        {
            SetNextSequenceNumber("1");
        }

        var dto = store.GetCreationTime(metaFile);
        var secondsSinceUnixEpoch =
            (int) Math.Round((dto.DateTime - new DateTime(1970, 1, 1).ToLocalTime()).TotalSeconds);
        return secondsSinceUnixEpoch.ToString();
    }

    /// <summary>
    /// Gets the Installation ID out of the metadata file
    /// </summary>
    private static string GetInstallId()
    {
        var store = getStore();
        var file = store.OpenFile(directoryName + @"\" + metaFileName, FileMode.Open);
        TextReader reader = new StreamReader(file);
        var installID = reader.ReadLine();
        reader.Close();
        file.Close();

        return installID;
    }

    private static string _version;

    /// <summary>
    /// Retreives the Application Version from teh WMAppManifest.xml file
    /// </summary>
    /// <returns>User generated app version</returns>
    public static string GetAppVersion()
    {
        if (!string.IsNullOrEmpty(_version))
            return _version;

        var manifest = new Uri("WMAppManifest.xml", UriKind.Relative);
        var si = Application.GetResourceStream(manifest);
        if (si != null)
        {
            using (var sr = new StreamReader(si.Stream))
            {
                var haveApp = false;
                while (!sr.EndOfStream)
                {
                    var line = sr.ReadLine();
                    if (!haveApp)
                    {
                        var i = line.IndexOf("AppPlatformVersion=\"", StringComparison.InvariantCulture);
                        if (i >= 0)
                        {
                            haveApp = true;
                            line = line.Substring(i + 20);
                        }
                    }

                    var y = line.IndexOf("Version=\"", StringComparison.InvariantCulture);
                    if (y >= 0)
                    {
                        var z = line.IndexOf("\"", y + 9, StringComparison.InvariantCulture);
                        if (z >= 0)
                        {
                            // We have the version, no need to read on.
                            _version = line.Substring(y + 9, z - y - 9);
                            break;
                        }
                    }
                }
            }
        }
        else
        {
            _version = "Unknown";
        }

        return _version;
    }

    /// <summary>
    /// Gets the current date/time as a string which can be inserted in the DB
    /// </summary>
    /// <returns>A formatted string with date, time, and timezone information</returns>
    private static string GetDatestring()
    {
        var dt = DateTime.Now.ToUniversalTime();

        // reformat the time to: YYYY-MM-DDTHH:MM:SS
        // use a StringBuilder to avoid creating multiple 
        var datestring = new StringBuilder();
        datestring.Append(dt.Year);
        datestring.Append("-");
        datestring.Append(dt.Month.ToString("D2"));
        datestring.Append("-");
        datestring.Append(dt.Day.ToString("D2"));
        datestring.Append("T");
        datestring.Append(dt.Hour.ToString("D2"));
        datestring.Append(":");
        datestring.Append(dt.Minute.ToString("D2"));
        datestring.Append(":");
        datestring.Append(dt.Second.ToString("D2"));

        return datestring.ToString();
    }

    /// <summary>
    /// Gets the current time in unixtime
    /// </summary>
    /// <returns>The current time in unixtime</returns>
    private static double GetTimeInUnixTime()
    {
        return Math.Round(((DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0)).TotalSeconds), 0);
    }

    #endregion

    /// <summary>
    /// Constructs a blob header for uploading
    /// </summary>
    /// <returns>A string containing a blob header</returns>
    private string GetBlobHeader()
    {
        var blobString = new StringBuilder();

        //{ "dt":"h",  // data type, h for header
        //  "pa": int, // persistent store created at
        //  "seq": int,  // blob sequence number, incremented on each new blob, 
        //               // remembered in the persistent store
        //  "u": string, // A unique ID for the blob. Must be the same if the blob is re-uploaded!
        //  "attrs": {
        //    "dt": "a" // data type, a for attributes
        //    "au":string // Localytics Application Id
        //    "du":string // Device UUID
        //    "s":boolean // Whether the app has been stolen (optional)
        //    "j":boolean // Whether the device has been jailbroken (optional)
        //    "lv":string // Library version
        //    "av":string // Application version
        //    "dp":string // Device Platform
        //    "dll":string // Locale Language (optional)
        //    "dlc":string // Locale Country (optional)
        //    "nc":string // Network Country (iso code) (optional)
        //    "dc":string // Device Country (iso code) (optional)
        //    "dma":string // Device Manufacturer (optional)
        //    "dmo":string // Device Model
        //    "dov":string // Device OS Version
        //    "nca":string // Network Carrier (optional)
        //    "dac":string // Data Connection Type (optional)
        //    "mnc":int // mobile network code (optional)
        //    "mcc":int // mobile country code (optional)
        //    "tdid":string // Telephony Device Id (meid or imei) (optional)
        //    "wmac":string // hashed wifi mac address (optional)
        //    "emac":string // hashed ethernet mac address (optional)
        //    "bmac":string // hashed bluetooth mac address (optional)
        //    "iu":string // install id
        //    "udid":string } } // client side hashed version of the udid
        blobString.Append("{\"dt\":\"h\",");
        blobString.Append("\"pa\":" + GetPersistStoreCreateTime() + ",");

        var sequenceNumber = GetSequenceNumber();
        blobString.Append("\"seq\":" + sequenceNumber + ",");
        SetNextSequenceNumber((int.Parse(sequenceNumber) + 1).ToString());

        blobString.Append("\"u\":\"" + Guid.NewGuid().ToString() + "\",");
        blobString.Append("\"attrs\":");
        blobString.Append("{\"dt\":\"a\",");
        blobString.Append("\"au\":\"" + this.appKey + "\",");
        blobString.Append("\"du\":\"" + GetDeviceId() + "\",");
        blobString.Append("\"lv\":\"" + libraryVersion + "\",");
        blobString.Append("\"av\":\"" + GetAppVersion() + "\",");
        blobString.Append("\"dp\":\"Windows Phone\",");
        blobString.Append("\"dll\":\"" + CultureInfo.CurrentCulture.TwoLetterISOLanguageName + "\",");
        blobString.Append("\"dma\":\"" + DeviceStatus.DeviceManufacturer + "\",");
        blobString.Append("\"dmo\":\"" + DeviceStatus.DeviceName + "\",");
        blobString.Append("\"dov\":\"" + Environment.OSVersion.Version.Build.ToString() + "\",");
        blobString.Append("\"iu\":\"" + GetInstallId() + "\"");

        blobString.Append("}}");
        blobString.Append(Environment.NewLine);

        return blobString.ToString();
    }

    /// <summary>
    /// Formats an input string for YAML
    /// </summary>       
    /// <returns>string sorrounded in quotes, with dangerous characters escaped</returns>
    private static string EscapeString(string input)
    {
        if (input == null)
            input = string.Empty;

        var escapedSlahes = input.Replace("\\", "\\\\");
        return "\"" + escapedSlahes.Replace("\"", "\\\"") + "\"";
    }

    /// <summary>
    /// Outputs a message to the debug console
    /// </summary>
    private static void LogMessage(string msg)
    {
        Debug.WriteLine("(localytics) " + msg);
    }

    #endregion

    #region public methods

    /// <summary>
    /// Creates a Localytics Session object
    /// </summary>
    /// <param name="appKey"> The key unique for each application generated at www.localytics.com</param>
    public LocalyticsSession(string appKey)
    {
        this.appKey = appKey;

        // Store the time and sequence number 
    }

    /// <summary>
    /// Opens or resumes the Localytics session.
    /// </summary>
    public void Open()
    {
        if (this.isSessionOpen || this.isSessionClosed)
        {
            LogMessage("Session is already opened or closed.");
            return;
        }

        try
        {
            if (getNumberOfStoredSessions() > maxStoredSessions)
            {
                LogMessage("Local stored session count exceeded.");
                return;
            }

            this.sessionUuid = Guid.NewGuid().ToString();
            this.sessionFilename = sessionFilePrefix + this.sessionUuid;
            this.sessionStartTime = GetTimeInUnixTime();

            // Format of an open session:
            //{ "dt":"s",       // This is a session blob
            //  "ct": long,     // seconds since Unix epoch
            //  "u": string     // A unique ID attached to this session 
            //  "nth": int,     // This is the nth session on the device. (not required)
            //  "new": boolean, // New vs returning (not required)
            //  "sl": long,     // seconds since last session (not required)
            //  "lat": double,  // latitude (not required)
            //  "lng": double,  // longitude (not required)
            //  "c0" : string,  // custom dimensions (not required)
            //  "c1" : string,
            //  "c2" : string,
            //  "c3" : string }

            var openstring = new StringBuilder();
            openstring.Append("{\"dt\":\"s\",");
            openstring.Append("\"ct\":" + GetTimeInUnixTime().ToString() + ",");
            openstring.Append("\"u\":\"" + this.sessionUuid + "\"");
            openstring.Append("}");
            openstring.Append(Environment.NewLine);

            appendTextToFile(openstring.ToString(), this.sessionFilename);

            this.isSessionOpen = true;
            LogMessage("Session opened.");
        }
        catch (Exception e)
        {
            LogMessage("Swallowing exception: " + e.Message);
        }
    }

    /// <summary>
    /// Closes the Localytics session.
    /// </summary>
    public void Close()
    {
        if (this.isSessionOpen == false || this.isSessionClosed)
        {
            LogMessage("Session not closed b/c it is either not open or already closed.");
            return;
        }

        try
        {
            //{ "dt":"c", // close data type
            //  "u":"abec86047d-ae51", // unique id for teh close
            //  "ss": session_start_time, // time the session was started
            //  "su":"696c44ebf6f",   // session uuid
            //  "ct":1302559195,  // client time
            //  "ctl":114,  // session length (optional)
            //  "cta":60, // active time length (optional)
            //  "fl":["1","2","3","4","5","6","7","8","9"], // Flows (optional)
            //  "lat": double,  // lat (optional)
            //  "lng": double,  // lng (optional)
            //  "c0" : string,  // custom dimensions (otpinal)
            //  "c1" : string,
            //  "c2" : string,
            //  "c3" : string }

            var closeString = new StringBuilder();
            closeString.Append("{\"dt\":\"c\",");
            closeString.Append("\"u\":\"" + Guid.NewGuid().ToString() + "\",");
            closeString.Append("\"ss\":" + this.sessionStartTime.ToString() + ",");
            closeString.Append("\"su\":\"" + this.sessionUuid + "\",");
            closeString.Append("\"ct\":" + GetTimeInUnixTime().ToString());
            closeString.Append("}");
            closeString.Append(Environment.NewLine);
            appendTextToFile(closeString.ToString(), this.sessionFilename); // the close blob

            this.isSessionOpen = false;
            this.isSessionClosed = true;
            LogMessage("Session closed.");
        }
        catch (Exception e)
        {
            LogMessage("Swallowing exception: " + e.Message);
        }
    }

    /// <summary>
    /// Creates a new thread which collects any files and uploads them. Returns immediately if an upload
    /// is already happenning.
    /// </summary>
    public void Upload()
    {
        if (isUploading)
        {
            return;
        }

        isUploading = true;

        try
        {
            // Do all the upload work on a seperate thread.
            ThreadStart uploadJob = BeginUpload;
            var uploadThread = new Thread(uploadJob);
            uploadThread.Start();
        }
        catch (Exception e)
        {
            LogMessage("Swallowing exception: " + e.Message);
        }
    }

    /// <summary>
    /// Records a specific event as having occured and optionally records some attributes associated with this event.
    /// This should not be called inside a loop. It should not be used to record personally identifiable information
    /// and it is best to define all your event names rather than generate them programatically.
    /// </summary>
    /// <param name="eventName">The name of the event which occured. E.G. 'button pressed'</param>
    /// <param name="attributes">Key value pairs that record data relevant to the event.</param>
    public void TagEvent(string eventName, Dictionary<string, string> attributes = null)
    {
        if (this.isSessionOpen == false)
        {
            LogMessage("Event not tagged because session is not open.");
            return;
        }

        //{ "dt":"e",  // event data time
        //  "ct":1302559181,   // client time
        //  "u":"48afd8beebd3",   // unique id
        //  "su":"696c44ebf6f",   // session id
        //  "n":"Button Clicked",  // event name
        //  "lat": double,   // lat (optional)
        //  "lng": double,   // lng (optional)
        //  "attrs":   // event attributes (optional)
        //  {
        //      "Button Type":"Round"
        //  },
        //  "c0" : string, // custom dimensions (optional)
        //  "c1" : string,
        //  "c2" : string,
        //  "c3" : string }

        try
        {
            var eventString = new StringBuilder();
            eventString.Append("{\"dt\":\"e\",");
            eventString.Append("\"ct\":" + GetTimeInUnixTime().ToString() + ",");
            eventString.Append("\"u\":\"" + Guid.NewGuid().ToString() + "\",");
            eventString.Append("\"su\":\"" + this.sessionUuid + "\",");
            eventString.Append("\"n\":" + EscapeString(eventName));

            if (attributes != null)
            {
                eventString.Append(",\"attrs\": {");
                var first = true;
                foreach (var key in attributes.Keys)
                {
                    if (!first)
                    {
                        eventString.Append(",");
                    }
                    eventString.Append(EscapeString(key) + ":" + EscapeString(attributes[key]));
                    first = false;
                }
                eventString.Append("}");
            }
            eventString.Append("}");
            eventString.Append(Environment.NewLine);

            appendTextToFile(eventString.ToString(), this.sessionFilename); // the close blob
            LogMessage("Tagged event: " + EscapeString(eventName));
        }
        catch (Exception e)
        {
            LogMessage("Swallowing exception: " + e.Message);
        }
    }

    #endregion
}