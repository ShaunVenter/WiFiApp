using System;

namespace AlwaysOn.Objects
{
    public class InfoButton
    {
        private string _title;
        private string _url;
        public string Title { get { return string.IsNullOrEmpty(_title) ? "About\r\nAlwaysOn" : _title; } set { _title = value; } }
        public string Url { get { return string.IsNullOrEmpty(_url) ? "https://www.alwayson.co.za/AboutAlwaysOn.html" : _url; } set { _url = value; } }
    }
}

