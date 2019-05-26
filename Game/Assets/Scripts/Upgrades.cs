using System;

public struct Upgrade {
	public string name;
	public UpgradeTypes type;
	public int price;
	public string description;
	public Action<Robot> OnAdd;
	public Action<Robot> OnRemove;

	public Upgrade(string name, UpgradeTypes type, int price, string description, Action<Robot> OnAdd, Action<Robot> OnRemove) {
		this.name = name;
		this.type = type;
		this.price = price;
		this.description = description;
		this.OnAdd = OnAdd;
		this.OnRemove = OnRemove;
	}
}

public enum UpgradeTypes {
	Hands,
	Feet,
	Armor,
	Core,
	Consumable // only for temporary upgrades
}
public class Upgrades {
	public static Upgrade[][] permanent = {
		new Upgrade[] { }, // Upgrade levels start from 1.
		new Upgrade[] {
			new Upgrade("",UpgradeTypes.Core, 0, "", null, null), // Upgrade IDs start from 1.
			new Upgrade("Hammer", UpgradeTypes.Hands, 0, "A light hammer.", (Robot r) => { r.attackBonus += 1; }, (Robot r) => {r.attackBonus -= 1; }),
			new Upgrade("Spike ball", UpgradeTypes.Feet, 0, "A spiked ball.", (Robot r) => {r.attackBonus += 1; }, (Robot r) => {r.attackBonus -= 1; }),
		},
		new Upgrade[] {
			new Upgrade("", UpgradeTypes.Core, 0, "", null, null), // Upgrade IDs start from 1.
			new Upgrade("Spike armor", UpgradeTypes.Armor, 0, "A spiked armor.", (Robot r) => {r.defenseBonus += 1; }, (Robot r) => { r.defenseBonus -= 1;}),
			new Upgrade("Core Mk 2", UpgradeTypes.Core, 0, "A more powerful core.", (Robot r) => { r.healthBonus += 1; }, (Robot r) => {r.healthBonus -= 1; }),
		}
	};

	public static Upgrade[] temporary = {
		new Upgrade("", UpgradeTypes.Core, 0, "", null, null), // Upgrade IDs start from 1.
		new Upgrade("Burning arms", UpgradeTypes.Hands, 8, "Adds fire effect to the fists.", (Robot r) => { r.SetUpgradeParticle(r.fireParticle, true); }, (Robot r) => {}),
		new Upgrade("Electrified arms", UpgradeTypes.Hands, 8, "Adds lightning effect to the fists.", (Robot r) => { r.SetUpgradeParticle(r.lightningParticle, true); }, (Robot r) => {}),
		new Upgrade("Burning legs", UpgradeTypes.Feet, 8, "Adds fire effect to the kick.", (Robot r) => { r.SetUpgradeParticle(r.fireParticle, false); }, (Robot r) => {}),
		new Upgrade("Electrified legs", UpgradeTypes.Feet, 8, "Adds lightning effect to the kick.", (Robot r) => { r.SetUpgradeParticle(r.lightningParticle, false); }, (Robot r) => {}),
		new Upgrade("Repair kit", UpgradeTypes.Consumable, 12, "Recover 50 health.", (Robot r) => { r.UpdateHealth(50); }, (Robot r) => {}),
		new Upgrade("Advanced repair kit", UpgradeTypes.Consumable, 20, "Recover 100 health.", (Robot r) => { r.UpdateHealth(100); }, (Robot r) => {}),
		new Upgrade("Ultimate repair kit", UpgradeTypes.Consumable, 28, "Recover your full health.", (Robot r) => { r.UpdateHealth(r.healthMax); }, (Robot r) => {}),
		new Upgrade("Burning armor", UpgradeTypes.Hands, 12, "Adds fire effect to the armor.", (Robot r) => { r.defenseBonus += 2; }, (Robot r) => {}),
		new Upgrade("Electrified armor", UpgradeTypes.Hands, 12, "Adds lightning effect to the armor.", (Robot r) => { r.defenseBonus += 2; }, (Robot r) => {}),
	};
}
