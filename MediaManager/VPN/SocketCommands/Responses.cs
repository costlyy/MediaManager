namespace MediaManager.VPN.SocketCommands
{
	public class GetStateResponse : VpnSocketResponse
	{
		public string Unknown { private set; get; }
		public string State { private set; get; }
		public string InternalIp { get; private set; }
		public string ExternalIp { get; private set; }

		public GetStateResponse(string response)
		{
			string[] splitResponse = response.Split(',');

			if (splitResponse.Length < 5)
			{
				ReturnCode = "MALFORMED";
				Success = false;
				return;
			}

			Unknown = splitResponse[0];
			State = splitResponse[1];
			ChallengeResponse = ParseChallenge(splitResponse[2]);
			InternalIp = splitResponse[3];
			ExternalIp = splitResponse[4];

			Success = true;
		}

	}
}
