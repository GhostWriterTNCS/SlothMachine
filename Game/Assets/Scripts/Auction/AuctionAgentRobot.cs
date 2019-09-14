using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum UpgradesBalance {
	notSet,
	balanced,
	specialized,
	mixed
}

public enum UpgradesPrefer {
	notSet,
	permanent,
	temporary,
	mixed
}

public class AuctionAgentRobot : AuctionAgent {
	public float interestPerLevel = 0.3f;
	public float levelWeight = 1;

	public float partUsed = 0.1f;
	public float partUsedWeight = 1;

	public float compatibilityRight = 0.95f;
	public float compatibilityWrong = 0.15f;
	public float compatibilityWeight = 1;

	/*public float balanceRight = 0.8f;
	public float balanceWrong = 0.2f;
	public float balanceWeight = 3;*/

	public float preferPermanent = 0.85f;
	public float preferTemporary = 0.15f;
	public int preferPermanentThreshold = 3;
	public int preferTemporaryThreshold = 6;
	public float preferWeight = 1;

	public float favoriteThreshold = 0.6f;
	public float notFavoriteThreshold = 0.4f;
	public float favoriteIncrease = 1.33f;
	public float favoriteDecrease = 0.75f;
	public float favoriteUpgradeBought = 0.25f;
	public float favoriteWeight = 1;

	Player player;

	public AuctionAgentRobot(Player player) : base() {
		this.player = player;
	}

	public override float GetInterest(object obj, object agent, bool isSelf) {
		UpgradeBox upgradeBox = (UpgradeBox)obj;
		Upgrade upgrade = Upgrades.permanent[upgradeBox.level][upgradeBox.ID];

		Player player = (Player)agent;
		GameObject model = GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/Robots/" + player.robotName + "/" + player.robotName));
		model.SetActive(false);
		RobotModel robotModel = model.GetComponent<RobotModel>();

		if (isSelf && (/*player.upgradesBalance == UpgradesBalance.notSet ||*/ player.upgradesPrefer == UpgradesPrefer.notSet)) {
			player.CmdSetupAuctionAgent();
		}

		// Check if the body part is already used.
		float partUsedWeight_ = partUsedWeight;
		Pair p = player.upgrades[(int)upgrade.type];
		if (p) {
			if (p.value1 >= upgradeBox.level) {
				// The robot already has a better upgrade for the same part.
				return 0;
			}
		} else {
			// The body part is free.
			partUsedWeight_ = 0;
		}

		// Evaluate the upgrade level.
		float level = upgradeBox.level * interestPerLevel;

		// Evaluate the upgrade compatibility.
		float compatibility = 0;
		float compatibilityWeight_ = compatibilityWeight;
		switch (robotModel.robotStyle) {
			case RobotStyle.Arms:
				switch (upgrade.type) {
					case UpgradeTypes.Hands:
						compatibility = compatibilityRight;
						break;
					case UpgradeTypes.Feet:
						compatibility = compatibilityWrong;
						break;
					default:
						compatibilityWeight_ = 0;
						break;
				}
				break;
			case RobotStyle.Legs:
				switch (upgrade.type) {
					case UpgradeTypes.Hands:
						compatibility = compatibilityWrong;
						break;
					case UpgradeTypes.Feet:
						compatibility = compatibilityRight;
						break;
					default:
						compatibilityWeight_ = 0;
						break;
				}
				break;
			default:
				compatibilityWeight_ = 0;
				break;
		}

		// Evaluate the upgrade balance.
		/*float balance = 0;
		float balanceWeight_ = balanceWeight;
		RobotStats[] strenghts = GetRobotStrenghts(robotModel);
		UpgradesBalance upgradesBalance = player.upgradesBalance;
		if (!isSelf) {
			upgradesBalance = DetectBalance(player);
		}
		switch (upgradesBalance) {
			case UpgradesBalance.balanced:
				if (strenghts.Contains(upgrade.robotStats)) {
					balance = balanceWrong;
				} else {
					balance = balanceRight;
				}
				break;
			case UpgradesBalance.specialized:
				if (strenghts.Contains(upgrade.robotStats)) {
					balance = balanceRight;
				} else {
					balance = balanceWrong;
				}
				break;
			default:
				balanceWeight_ = 0;
				break;
		}*/

		// Evaluate the upgrade preference.
		float prefer = 0;
		float preferWeight_ = preferWeight;
		UpgradesPrefer upgradesPrefer = player.upgradesPrefer;
		if (!isSelf) {
			upgradesPrefer = DetectPrefer(player);
		}
		switch (upgradesPrefer) {
			case UpgradesPrefer.permanent:
				prefer = preferPermanent;
				break;
			case UpgradesPrefer.temporary:
				prefer = preferTemporary;
				break;
			default:
				preferWeight_ = 0;
				break;
		}

		// Evaluate the favorite stats.
		float favorite = 0;
		if (isSelf) {
			Debug.Log(string.Join(",", player.favorites));
			favorite = player.favorites[(int)upgrade.robotStats - 1];
		} else {
			if (!this.player.expectedFavorites.ContainsKey(player)) {
				this.player.expectedFavorites.Add(player, new float[] { 0.5f, 0.5f, 0.5f, 0.5f });
			}
			favorite = this.player.expectedFavorites[player][(int)upgrade.robotStats - 1];
		}

		float result = (level * levelWeight + compatibility * compatibilityWeight_ + partUsed * partUsedWeight_ + prefer * preferWeight_ + favorite * favoriteWeight) /
			(levelWeight + compatibilityWeight_ + partUsedWeight_ + preferWeight_ + favoriteWeight);
		if (isSelf) {
			Debug.Log(player.name + "'s bid for " + upgrade.name + ": " + level * levelWeight + ", " + compatibility * compatibilityWeight_ + ", " + partUsed * partUsedWeight_ + ", " + prefer * preferWeight_ + ", " + favorite * favoriteWeight + " -> " + result);
		}
		return result;
	}

	public override float GetExpectedMoney(object other) {
		Player otherPlayer = (Player)other;
		if (!player.expectedMoney.ContainsKey(otherPlayer)) {
			player.expectedMoney.Add(otherPlayer, Player.defaultScraps);
		}
		//Debug.Log(player.name + " expects " + (player.expectedMoney[otherPlayer] + otherPlayer.score / 26) + " for " + otherPlayer.name);
		return player.expectedMoney[otherPlayer] + otherPlayer.score / 26;
	}

	RobotStats[] GetRobotStrenghts(RobotModel robotModel) {
		List<RobotStats> stats = new List<RobotStats>();
		float mean = (robotModel.health + robotModel.attack + robotModel.defense + robotModel.speed) / 4;
		if (robotModel.health > mean) {
			stats.Add(RobotStats.health);
		}
		if (robotModel.attack > mean) {
			stats.Add(RobotStats.attack);
		}
		if (robotModel.defense > mean) {
			stats.Add(RobotStats.defense);
		}
		if (robotModel.speed > mean) {
			stats.Add(RobotStats.speed);
		}
		return stats.ToArray();
	}

	UpgradesBalance DetectBalance(Player player) {
		GameObject model = GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/Robots/" + player.robotName + "/" + player.robotName));
		model.SetActive(false);
		RobotModel robotModel = model.GetComponent<RobotModel>();
		RobotStats[] strenghts = GetRobotStrenghts(robotModel);
		int balanced = 0;
		int specialized = 0;
		foreach (Pair u in player.upgrades) {
			if (u) {
				if (strenghts.Contains(Upgrades.permanent[u.value1][u.value2].robotStats)) {
					specialized++;
				} else {
					balanced++;
				}
			}
		}
		if (balanced == 0 && specialized > 1) {
			return UpgradesBalance.specialized;
		} else if (specialized == 0 && balanced > 1) {
			return UpgradesBalance.balanced;
		} else {
			return UpgradesBalance.mixed;
		}
	}

	UpgradesPrefer DetectPrefer(Player otherPlayer) {
		if (!player.temporaryUpgradesCount.ContainsKey(otherPlayer)) {
			return UpgradesPrefer.mixed;
		}
		if (player.temporaryUpgradesCount[otherPlayer] < preferPermanentThreshold / (float)MatchManager.singleton.roundCounter) {
			return UpgradesPrefer.permanent;
		} else if (player.temporaryUpgradesCount[otherPlayer] > preferTemporaryThreshold / (float)MatchManager.singleton.roundCounter) {
			return UpgradesPrefer.temporary;
		}
		return UpgradesPrefer.mixed;
	}

	public void EvaluateBid(Player otherPlayer, UpgradeBox upgradeBox, int bid) {
		float interest = bid / GetExpectedMoney(otherPlayer);
		Upgrade upgrade = Upgrades.permanent[upgradeBox.level][upgradeBox.ID];
		GameObject model = GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/Robots/" + otherPlayer.robotName + "/" + otherPlayer.robotName));
		model.SetActive(false);
		RobotModel robotModel = model.GetComponent<RobotModel>();

		if (!player.expectedFavorites.ContainsKey(otherPlayer)) {
			player.expectedFavorites.Add(otherPlayer, new float[] { 0.5f, 0.5f, 0.5f, 0.5f });
		}

		float favorite = interest;

		Pair p = otherPlayer.upgrades[(int)upgrade.type];
		if (p) {
			if (p.value1 < upgradeBox.level) {
				// The robot already has an upgrade for the same part.
				favorite /= partUsed;
			}
		}

		favorite /= interestPerLevel;

		switch (robotModel.robotStyle) {
			case RobotStyle.Arms:
				switch (upgrade.type) {
					case UpgradeTypes.Hands:
						favorite /= compatibilityRight;
						break;
					case UpgradeTypes.Feet:
						favorite /= compatibilityWrong;
						break;
				}
				break;
			case RobotStyle.Legs:
				switch (upgrade.type) {
					case UpgradeTypes.Hands:
						favorite /= compatibilityWrong;
						break;
					case UpgradeTypes.Feet:
						favorite /= compatibilityRight;
						break;
				}
				break;
		}

		UpgradesPrefer upgradesPrefer = DetectPrefer(otherPlayer);
		switch (upgradesPrefer) {
			case UpgradesPrefer.permanent:
				favorite /= preferPermanent;
				break;
			case UpgradesPrefer.temporary:
				favorite /= preferTemporary;
				break;
		}

		if (favorite > favoriteThreshold) {
			favorite = favoriteIncrease;
		} else if (favorite < notFavoriteThreshold) {
			favorite = favoriteDecrease;
		} else {
			return;
		}
		player.expectedFavorites[otherPlayer][(int)upgrade.robotStats - 1] *= favorite;
	}
}
