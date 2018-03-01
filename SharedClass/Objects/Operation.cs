using System;
using System.ServiceModel.Channels;
using System.Runtime.CompilerServices;

namespace AlwaysOn.Objects
{
	public class Operation
	{
		private string _name;
		private OperationResult _result;
		private string _message;
		private Object _response;


		public string Name 		
		{
			get 
			{
				return _name;
			}
		}

		public OperationResult Result 
		{
			get 
			{
				return _result;
			}
		}

		public string Message 		
		{
			get 
			{
				return _message;
			}
		}

		public Object Response 		
		{
			get 
			{
				return _response;
			}
		}



		public Operation(string operationName)
		{
			_name = operationName;
		}

		public void CreateSuccessfulResult (Object response, string message = "")
		{
			_result = OperationResult.Success;

			_message = string.Format("{0} has completed successfully!", _name);

			if (!string.IsNullOrWhiteSpace(message))
			{
				_message += " More details: " + message;
			}

			_response = response;
		}

		public void CreateFailingResult (string message)
		{
			_result = OperationResult.Failure;

			if (string.IsNullOrWhiteSpace(message))
			{
				_message = string.Format("{0} has failed because we are currently experiencing technical difficulties. Please try again later", _name);
			}
			else
			{
				_message = string.Format("{0} has failed. More details: {1}", _name, message);
			}

			_response = null;
		}

	}

	public enum OperationResult
	{
		Success = 1,
		Failure = 2
	}


}

