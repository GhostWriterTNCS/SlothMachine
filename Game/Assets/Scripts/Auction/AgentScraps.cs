using UnityEngine;

public class AgentScraps : MonoBehaviour {
	[Range(0, 1)]
	public float variability = 0.1f;

	AuctionAgent auctionAgent = new AuctionRobot();
	AuctionManager auctionManager;
	UpgradeBox currentUpgrade;

	void Start() {
		auctionManager = FindObjectOfType<AuctionManager>();
	}

	public short CalculateAgentBid() {
		currentUpgrade = auctionManager.currentUpgrade;
		auctionAgent.variability = variability;
		auctionAgent.moneyAvailable = 100;
		return (short)auctionAgent.GetBid(currentUpgrade, null, true);
	}
}
