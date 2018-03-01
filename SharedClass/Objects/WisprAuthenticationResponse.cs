using System;

namespace AlwaysOn
{
    public class WisprAuthenticationResponse
    {
        //fields

        private static string xmlFieldMessageType = "MessageType";
        private static string xmlFieldResponseCode = "ResponseCode";
        private static string xmlFieldReplyMessage = "ReplyMessage";
        private static string xmlFieldLoginResultsURL = "LoginResultsURL";
        private static string xmlFieldLogoffURL = "LogoffURL";


        //properties

        public string MessageType { get; private set; }
        public string ResponseCode { get; private set; }
        public string ReplyMessage { get; private set; }
        public string LoginResultsURL { get; private set; }
        public string LogoffURL { get; private set; }

        public bool Success { get; set; }
        public bool Retry { get; private set; }
        public string ResultMessage { get; set; }


        //methods

        public WisprAuthenticationResponse(bool Success, string ErrorMessage)
        {
            this.Success = Success;
            Retry = false;
            ResultMessage = ErrorMessage;
        }

        public WisprAuthenticationResponse(string pageHtml)
        {
            Success = false;
            Retry = false;
            ResultMessage = string.Empty;

            ExtractFields(pageHtml);
            Verify();

            if (ResponseCode == "0")
            {
                ExtractRadiusErrorMessage(pageHtml);
            }
        }

        private void ExtractRadiusErrorMessage(string pageHtml)
        {
            var Value = "";
            try
            {
                var newString = pageHtml.Substring(pageHtml.IndexOf("name=\"error\""));
                Value = newString.Substring(newString.IndexOf("value=\"") + 7, newString.IndexOf("\">") - 20);
            }
            catch (Exception ex)
            {
            }

            if (Value.Length > 0)
            {
                ResultMessage = Value;
            }
        }

        private void ExtractFields(string pageHtml)
        {
            try
            {
                MessageType = GetValue(pageHtml, xmlFieldMessageType);
                ResponseCode = GetValue(pageHtml, xmlFieldResponseCode);
                ReplyMessage = GetValue(pageHtml, xmlFieldReplyMessage);
                LoginResultsURL = GetValue(pageHtml, xmlFieldLoginResultsURL);
                LogoffURL = GetValue(pageHtml, xmlFieldLogoffURL);
            }
            catch (Exception ex)
            {
                throw new Exception("Could not extract WISPr Authentication fields. More details: " + ex.Message);
            }
        }

        private string GetValue(string pageHtml, string fieldName)
        {
            var openingTag = "<" + fieldName + ">";
            var closingTag = "</" + fieldName + ">";

            var startIndex = pageHtml.IndexOf(openingTag);
            var endIndex = pageHtml.IndexOf(closingTag);

            if (startIndex == -1 || endIndex == -1)
            {
                return string.Empty; //reliable tag data not found
            }

            var data = pageHtml.Substring(startIndex, endIndex - startIndex);
            data = data.Replace(openingTag, string.Empty);

            if (string.IsNullOrWhiteSpace(data))
            {
                return string.Empty;
            }

            return data;
        }

        private void Verify()
        {
            if (string.IsNullOrWhiteSpace(ResponseCode))
            {
                Success = false;
                Retry = false;
                ResultMessage = "Internal malfunction of access gateway (could not determine response code)";
                return;
            }

            switch (ResponseCode.Trim())
            {
                case "50":
                    Success = true;
                    Retry = false;
                    ResultMessage = string.Empty;
                    return;

                case "100":
                    Success = false;
                    Retry = false;
                    ResultMessage = "100: Login failed (Access Reject). More details: " + ReplyMessage;
                    return;

                case "102":
                    Success = false;
                    Retry = false;
                    ResultMessage = "102: RADIUS server error or timeout. More details: " + ReplyMessage;
                    return;

                case "201":
                    Success = false;
                    Retry = true;
                    ResultMessage = "201: Authentication pending. More details: " + ReplyMessage;
                    return;

                case "255":
                    Success = false;
                    Retry = false;
                    ResultMessage = "255: Access Gateway internal error. More details: " + ReplyMessage;
                    return;

                default:
                    Success = false;
                    Retry = false;
                    ResultMessage = "Internal malfunction of access gateway (could not determine response code)";
                    return;
            }
        }

    }
}

