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

public class AuctionRobot : AuctionAgent {
	float interestPerLevel = 0.25f;
	float compatibilityRight = 0.95f;
	float compatibilityWrong = 0.15f;
	float partUsed = 0.05f;
	float partNotUsed = 0.8f;
	float balanceRight = 0.8f;
	float balanceWrong = 0.2f;

	public override float GetInterest(object obj, object agent, bool isSelf) {
		UpgradeBox upgradeBox = (UpgradeBox)obj;
		Upgrade upgrade = Upgrades.permanent[upgradeBox.level][upgradeBox.ID];

		Player player = (Player)agent;
		GameObject model = GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/Robots/" + player.robotName + "/" + player.robotName));
		model.SetActive(false);
		RobotModel robotModel = model.GetComponent<RobotModel>();

		// Check if the body part is already used.
		float partIsUsed = partNotUsed;
		Pair p = player.upgrades[(int)upgrade.type];
		if (p) {
			if (p.value1 < upgradeBox.level) {
				partIsUsed = partUsed;
			} else {
				// The robot already has a better upgrade for the same part.
				return 0;
			}
		}

		// Evaluate the upgrade level.
		float level = upgradeBox.level * interestPerLevel;

		// Evaluate the upgrade compatibility.
		float compatibility = 0.5f;
		switch (robotModel.robotStyle) {
			case RobotStyle.Arms:
				switch (upgrade.type) {
					case UpgradeTypes.Hands:
						compatibility = compatibilityRight;
						break;
					case UpgradeTypes.Feet:
						compatibility = compatibilityWrong;
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
				}
				break;
		}

		// Check if the upgrade is useful for a balanced or specialized robot.
		if (player.upgradesBalance == UpgradesBalance.notSet) {
			player.CmdSetUpgradesBalance(UpgradesBalance.notSet);
		}
		float balance = 0.5f;
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
			case UpgradesBalance.mixed:
				break;
			default:
				throw new ArgumentException(player.upgradeAssigned.ToString() + " is not a valid value.");
		}

		if (isSelf) {
			Debug.LogError(player.robotName + "'s bid: " + level + ", " + compatibility + ", " + partIsUsed + ", " + balance);
		}
		return (level + compatibility + partIsUsed + balance) / 4;
	}

	public override float GetExpectedMoney(object other) {
		Player player = (Player)other;
		return player.score / 30;
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
}
