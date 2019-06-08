using System.Drawing;

namespace MediaManager.VPN.SocketCommands
{
	public class VpnSocketResponse
	{
		public enum ChallengeResponseType
		{
			Undefined,
			Fail,
			Success,
		}

		public bool Success { get; protected set; }
		public string ReturnCode { get; protected set; }
		public ChallengeResponseType ChallengeResponse { protected set; get; }

		protected ChallengeResponseType ParseChallenge(string challenge)
		{
			if (challenge.Length <= 0)
			{
				return ChallengeResponseType.Undefined;
			}

			if (challenge.Contains("SUCCESS"))
			{
				return ChallengeResponseType.Success;
			}

			return ChallengeResponseType.Fail;
		}

		public Color GetColor()
		{
			switch (ChallengeResponse)
			{
				default:
					return Color.Yellow;
				case ChallengeResponseType.Fail:
					return Color.Red;
				case ChallengeResponseType.Success:
					return Color.Green;
			}
		}
	}
}
