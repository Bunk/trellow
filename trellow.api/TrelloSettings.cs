﻿using JetBrains.Annotations;

namespace trellow.api
{
    public interface ITrelloApiSettings
    {
        string AppName { get; }

        string MemberId { get; set; }

        string Username { get; set; }

        string Fullname { get; set; }

        string AvatarHash { get; set; }

        string ApiRoot { get; }

        string ApiConsumerKey { get; }

        string ApiConsumerSecret { get; }

        string OAuthScope { get; }

        string OAuthExpiration { get; }

        OAuthToken AccessToken { get; set; }
    }

    [UsedImplicitly]
    public class TrelloSettings : AppSettingsBase, ITrelloApiSettings
    {
        public string AppName
        {
            get { return "Trellow"; }
        }

        public string MemberId
        {
            get { return GetOrDefault<string>("MemberId"); }
            set { Set("MemberId", value); }
        }

        public string Username
        {
            get { return GetOrDefault<string>("Username"); }
            set { Set("Username", value); }
        }

        public string Fullname
        {
            get { return GetOrDefault<string>("Fullname"); }
            set { Set("Fullname", value); }
        }

        public string AvatarHash
        {
            get { return GetOrDefault<string>("AvatarHash"); }
            set { Set("AvatarHash", value);}
        }

        public string ApiRoot
        {
            get { return "https://trello.com/1"; }
        }

        public string ApiConsumerKey
        {
            get { return "69d9b907713f98fce88b772243734ee1"; }
        }

        public string ApiConsumerSecret
        {
            get { return "1fb29637cc712d8622aeac07fccf2e5caf1a713029032e96b9db2527ab14d65b"; }
        }

        public string OAuthScope
        {
            get { return "read,write,account"; }
        }

        public string OAuthExpiration
        {
            get { return "never"; }
        }

        public OAuthToken AccessToken
        {
            get { return GetOrDefault<OAuthToken>("AccessToken"); }
            set { Set("AccessToken", value); }
        }
    }
}