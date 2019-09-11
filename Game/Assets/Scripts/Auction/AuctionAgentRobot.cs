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

	public float balanceRight = 0.8f;
	public float balanceWrong = 0.2f;
	public float balanceWeight = 3;

	public float preferPermanent = 0.85f;
	public float preferTemporary = 0.15f;
	public int preferPermanentThreshold = 3;
	public int preferTemporaryThreshold = 6;
	public float preferWeight = 1;

	Player player;
	RobotModel robotModel;

	public AuctionAgentRobot(Player player) : base() {
		this.player = player;
	}

	public override float GetInterest(object obj, object agent, bool isSelf) {
		UpgradeBox upgradeBox = (UpgradeBox)obj;
		Upgrade upgrade = Upgrades.permanent[upgradeBox.level][upgradeBox.ID];

		Player player = (Player)agent;
		if (!robotModel) {
			GameObject model = GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/Robots/" + player.robotName + "/" + player.robotName));
			model.SetActive(false);
			robotModel = model.GetComponent<RobotModel>();
		}
		if (player.upgradesBalance == UpgradesBalance.notSet || player.upgradesPrefer == UpgradesPrefer.notSet) {
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
		float balance = 0;
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
		}

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

		float result = (level * levelWeight + compatibility * compatibilityWeight_ + partUsed * partUsedWeight_ + balance * balanceWeight_ + prefer * preferWeight_) /
			(levelWeight + compatibilityWeight_ + partUsedWeight_ + balanceWeight_ + preferWeight_);
		if (isSelf) {
			Debug.Log(player.name + "'s bid: " + level * levelWeight + ", " + compatibility * compatibilityWeight_ + ", " + partUsed * partUsedWeight_ + ", " + balance * balanceWeight_ + ", " + prefer * preferWeight_ + " -> " + result);
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
		if (balanced == 0) {
			return UpgradesBalance.specialized;
		} else if (specialized == 0) {
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
}
