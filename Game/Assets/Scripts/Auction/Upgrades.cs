using System;

public struct Upgrade {
	public string name;
	public UpgradeTypes type;
	public string description;
	public Action<Robot> OnAdd;
	public Action<Robot> OnRemove;

	public Upgrade(string name, UpgradeTypes type, string description, Action<Robot> OnAdd, Action<Robot> OnRemove) {
		this.name = name;
		this.type = type;
		this.description = description;
		this.OnAdd = OnAdd;
		this.OnRemove = OnRemove;
	}
}

public enum UpgradeTypes {
	Hands,
	Feet,
	Armor,
	Core
}
public class Upgrades {
	public static Upgrade[][] list = {
		new Upgrade[] { }, // Upgrade levels start from 1.
		new Upgrade[] {
			new Upgrade("",UpgradeTypes.Core, "", null, null), // Upgrade IDs start from 1.
			new Upgrade("Hammer", UpgradeTypes.Hands, "A light hammer.", (Robot r) => { r.attackBonus += 1; }, (Robot r) => {r.attackBonus -= 1; }),
			new Upgrade("Spike ball", UpgradeTypes.Feet, "A spiked ball.", (Robot r) => {r.attackBonus += 1; }, (Robot r) => {r.attackBonus -= 1; })
		},
		new Upgrade[] {
			new Upgrade("", UpgradeTypes.Core, "", null, null), // Upgrade IDs start from 1.
			new Upgrade("Spike armor", UpgradeTypes.Armor, "A spiked armor.", (Robot r) => {r.defenseBonus += 1; }, (Robot r) => { r.defenseBonus -= 1;}),
			new Upgrade("Core Mk 2", UpgradeTypes.Core, "A more powerful core.", (Robot r) => { r.healthBonus += 1; }, (Robot r) => {r.healthBonus -= 1; })
		}
	};
}
