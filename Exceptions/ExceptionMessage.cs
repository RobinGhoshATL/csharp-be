using Newtonsoft.Json;


namespace ProSuite.Support.WebAPI.Exceptions
{
    public class ExceptionMessage
    {
        #region "Variables"
        public string _message { get; set; }
        #endregion "Variables"

        #region "Constructor"
        public ExceptionMessage() { }

        public ExceptionMessage(string message)
        {
            _message = message;
        }
        #endregion "Constructor"

        #region "Public Methods"
        public override string ToString()
        {
            return JsonConvert.SerializeObject(new { message = new string(_message) });
        }
        #endregion "Public Methods"
    }
}
