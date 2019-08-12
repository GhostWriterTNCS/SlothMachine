using System;

public abstract class AuctionAgent {
	/// <summary>
	/// The money available for the acution.
	/// </summary>
	public float moneyAvailable;
	/// <summary>
	/// The result variability (between 0 and 1).
	/// </summary>
	public float variability;

	Random rand = new Random();

	/// <summary>
	/// Calculate how much the agent is interested in the object.
	/// </summary>
	/// <param name="obj">The auction object.</param>
	/// <param name="agent">The agent.</param>
	/// <returns>A value between 0 (not interested) and 1 (super interested).</returns>
	public abstract float GetInterest(object obj, object agent, bool isSelf);

	/// <summary>
	/// Returns the bid for the object.
	/// The method uses GetInterest(object) and the variability.
	/// </summary>
	/// <param name="obj">The auction object.</param>
	/// <param name="agent">The agent.</param>
	/// <param name="moneyAvailable">The money available. If value is less then 0, it uses the moneyAvailable param.</param>
	/// <returns>The bid for the object</returns>
	public float GetBid(object obj, object agent, bool isSelf) {
		float moneyAvailable = this.moneyAvailable;
		if (!isSelf) {
			moneyAvailable = GetExpectedMoney(agent);
		}
		float bid = moneyAvailable * GetInterest(obj, agent, isSelf);
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
		float bid = GetBid(obj, agent, true);
		foreach (object other in others) {
			GetBid(obj, other, false);
		}
		return bid;
	}
}
