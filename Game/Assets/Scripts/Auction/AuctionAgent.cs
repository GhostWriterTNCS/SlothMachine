using System;

public abstract class AuctionAgent {
	/// <summary>
	/// The amount money available for the auction.
	/// </summary>
	public float moneyAvailable;
	/// <summary>
	/// The result variability (between 0 and 1).
	/// </summary>
	public float variability = 0.1f;
	/// <summary>
	/// When the agent is willing to increase its bid if someone else has a higher expected bid (between 0.5 and 1).
	/// </summary>
	public float veryInterestedThreshold = 0.75f;
	/// <summary>
	/// When the agent is willing to decrease its bid if someone else has a lower expected bid (between 0 and 0.5).
	/// </summary>
	public float notInterestedThreshold = 0.25f;

	static Random rand = new Random();

	/// <summary>
	/// Calculate how much the agent is interested in the object.
	/// </summary>
	/// <param name="obj">The auction object.</param>
	/// <param name="agent">The agent.</param>
	/// <param name="isSelf">When false, the agent tries to guess the opponent's values.</param>
	/// <returns>A value between 0 (not interested) and 1 (super interested).</returns>
	public abstract float GetInterest(object obj, object agent, bool isSelf);

	/// <summary>
	/// Returns the bid for the object.
	/// The method uses GetInterest(object) and the variability.
	/// </summary>
	/// <param name="obj">The auction object.</param>
	/// <param name="agent">The agent.</param>
	/// <param name="isSelf">When false, the agent tries to guess the opponent's values.</param>
	/// <returns>The bid for the object</returns>
	public float GetBid(object obj, object agent, bool isSelf) {
		float moneyAvailable = this.moneyAvailable;
		if (!isSelf) {
			moneyAvailable = GetExpectedMoney(agent);
		}
		return GetBid(GetInterest(obj, agent, isSelf), moneyAvailable);
	}
	/// <summary>
	/// Returns the bid for the object.
	/// </summary>
	/// <param name="interest">A value between 0 (not interested) and 1 (super interested).</param>
	/// <param name="moneyAvailable">The money available.</param>
	/// <returns>The bid for the object</returns>
	public float GetBid(float interest, float moneyAvailable) {
		float bid = moneyAvailable * interest;
		bid += bid * ((float)rand.NextDouble() * variability * 2 - variability);
		if (bid > moneyAvailable) {
			bid = moneyAvailable;
		} else if (bid < 0) {
			bid = 0;
		}
		return bid;
	}

	/// <summary>
	/// Returns the expected money available for another agent.
	/// </summary>
	/// <param name="other">The agent.</param>
	/// <returns>The expected money available for the agent.</returns>
	public abstract float GetExpectedMoney(object other);

	/// <summary>
	/// Returns the bid for the object taking into consideration the expected bids for the other players.
	/// </summary>
	/// <param name="obj">The auction object.</param>
	/// <param name="agent">The agent.</param>
	/// <param name="others">The other contenders.</param>
	/// <returns>The bid for the object</returns>
	public float GetRefinedBid(object obj, object agent, object[] others) {
		float interest = GetInterest(obj, agent, true);
		float bid = GetBid(interest, moneyAvailable);
		foreach (object other in others) {
			float otherBid = GetBid(obj, other, false);
			if (interest > veryInterestedThreshold && otherBid > bid) {
				bid = otherBid * (interest + 0.5f);
			} else if (interest < notInterestedThreshold && otherBid < bid) {
				bid = otherBid * (interest + 0.5f);
			}
		}
		if (bid > moneyAvailable) {
			bid = moneyAvailable;
		} else if (bid < 0) {
			bid = 0;
		}
		return bid;
	}
}
