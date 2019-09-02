using System;
using System.Collections.Generic;
using System.Linq;

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
	public float veryInterestedThreshold = 0.7f;
	/// <summary>
	/// The minimum amount of interest between its interest and the highest expected interest for the opponents.
	/// The value must be between 0 (no margin) and 1 (always bid all the money).
	/// To be effective, it should be bigger than variability.
	/// </summary>
	public float safeMargin = 0.2f;

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
	/// <returns>The bid for the object.</returns>
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
	/// <returns>The bid for the object.</returns>
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
	/// <returns>The bid for the object.</returns>
	public float GetRefinedBid(object obj, object agent, object[] others) {
		float interest = GetInterest(obj, agent, true);
		List<float> othersBids = new List<float>();
		foreach (object other in others) {
			float otherBid = GetBid(obj, other, false);
			othersBids.Add(otherBid);
		}
		othersBids = othersBids.OrderByDescending(f => f).ToList();
		float maxInterest = othersBids[0] / moneyAvailable;
		if (maxInterest < interest - safeMargin) {
			interest = maxInterest;
		} else if (interest > veryInterestedThreshold && maxInterest > interest) {
			interest = maxInterest + safeMargin;
		}
		return GetBid(interest, moneyAvailable);
	}
}
