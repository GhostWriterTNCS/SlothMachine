using System;

public struct Upgrade {
	public string name;
	public UpgradeTypes type;
	public int price;
	public string description;
	public Action<Robot> OnAdd;
	public Action<Robot> OnRemove;
	public RobotStats robotStats;

	public Upgrade(string name, UpgradeTypes type, int price, string description, Action<Robot> OnAdd, Action<Robot> OnRemove, RobotStats robotStats = RobotStats.nothing) {
		this.name = name;
		this.type = type;
		this.price = price;
		this.description = description;
		this.OnAdd = OnAdd;
		this.OnRemove = OnRemove;
		this.robotStats = robotStats;
	}
}

public enum UpgradeTypes {
	Hands,
	Feet,
	Armor,
	Core,
	Consumable // only for temporary upgrades
}
public enum UpgradeElements {
	Fire,
	Ice,
	Lightning,
	Sonic
}
public enum RobotStats {
	nothing,
	health,
	attack,
	defense,
	speed
}

public class Upgrades {
	public static Upgrade[][] permanent = {
		new Upgrade[] { }, // Upgrade levels start from 1.
		new Upgrade[] {
			new Upgrade("",UpgradeTypes.Core, 0, "", null, null), // Upgrade IDs start from 1.
			new Upgrade("Hammer", UpgradeTypes.Hands, 0, "A light hammer that slightly increases the attack.", (Robot r) => { r.attackBonus += 3; }, (Robot r) => {r.attackBonus -= 3; }, RobotStats.attack),
			new Upgrade("Spike ball", UpgradeTypes.Feet, 0, "A spiked ball that slightly increases the attack.", (Robot r) => {r.attackBonus += 3; }, (Robot r) => {r.attackBonus -= 3; }, RobotStats.attack),
			new Upgrade("Advanced engine Mk I", UpgradeTypes.Core, 0, "A more powerful engine that slightly increases the attack.", (Robot r) => {r.attackBonus += 2; }, (Robot r) => {r.attackBonus -= 2; }, RobotStats.attack),
			new Upgrade("Advanced armor Mk I", UpgradeTypes.Armor, 0, "A more resistent material for the armor that slightly increases the defense.", (Robot r) => {r.defenseBonus += 2; }, (Robot r) => {r.defenseBonus -= 2; }, RobotStats.defense),
			new Upgrade("Advanced core Mk I", UpgradeTypes.Core, 0, "A more powerful core that slightly increases the maximum health.", (Robot r) => {r.healthBonus += 2; }, (Robot r) => {r.healthBonus -= 2; }, RobotStats.health),
			new Upgrade("Advanced connectors Mk I", UpgradeTypes.Core, 0, "More efficient connectors that slightly increase the speed.", (Robot r) => {r.speedBonus += 2; }, (Robot r) => {r.speedBonus -= 2; }, RobotStats.speed),
		},
		new Upgrade[] {
			new Upgrade("", UpgradeTypes.Core, 0, "", null, null), // Upgrade IDs start from 1.
			new Upgrade("Spike armor", UpgradeTypes.Armor, 0, "A spiked armor that increases the defense.", (Robot r) => {r.defenseBonus += 5; }, (Robot r) => { r.defenseBonus -= 5;}),
			new Upgrade("Advanced core Mk II", UpgradeTypes.Core, 0, "A more powerful core that increases the maximum health.", (Robot r) => { r.healthBonus += 4; }, (Robot r) => {r.healthBonus -= 4; }),
			new Upgrade("Drill", UpgradeTypes.Hands, 0, "A sharp drill that increases the attack.", (Robot r) => { r.attackBonus += 5; }, (Robot r) => {r.healthBonus -= 5; }),
			new Upgrade("Advanced engine Mk II", UpgradeTypes.Core, 0, "A more powerful engine that increases the attack.", (Robot r) => {r.attackBonus += 4; }, (Robot r) => {r.attackBonus -= 4; }),
			new Upgrade("Advanced armor Mk II", UpgradeTypes.Armor, 0, "A more resistent material for the armor that increases the defense.", (Robot r) => {r.defenseBonus += 4; }, (Robot r) => {r.defenseBonus -= 4; }),
			new Upgrade("Advanced connectors Mk II", UpgradeTypes.Core, 0, "More efficient connectors that increase the speed.", (Robot r) => {r.speedBonus += 4; }, (Robot r) => {r.speedBonus -= 4; }),
		},
		new Upgrade[] {
			new Upgrade("", UpgradeTypes.Core, 0, "", null, null), // Upgrade IDs start from 1.
			new Upgrade("Advanced core Mk III", UpgradeTypes.Core, 0, "A more powerful core that considerably increases the maximum health.", (Robot r) => { r.healthBonus += 6; }, (Robot r) => {r.healthBonus -= 6; }),
			new Upgrade("Advanced engine Mk III", UpgradeTypes.Core, 0, "A more powerful engine that considerably increases the attack.", (Robot r) => {r.attackBonus += 6; }, (Robot r) => {r.attackBonus -= 6; }),
			new Upgrade("Advanced armor Mk III", UpgradeTypes.Armor, 0, "A more resistent material for the armor that considerably increases the defense.", (Robot r) => {r.defenseBonus += 6; }, (Robot r) => {r.defenseBonus -= 6; }),
			new Upgrade("Advanced connectors Mk III", UpgradeTypes.Core, 0, "More efficient connectors that considerably increase the speed.", (Robot r) => {r.speedBonus += 6; }, (Robot r) => {r.speedBonus -= 64; }),
		}
	};

	public static Upgrade[] temporary = {
		new Upgrade("", UpgradeTypes.Core, 0, "", null, null), // Upgrade IDs start from 1.
		new Upgrade("Burning arms", UpgradeTypes.Hands, 8, "Adds fire effect to the fists.", (Robot r) => { r.RefreshUpgradeParticle(UpgradeElements.Fire, Robot.BodyPart.Hands); }, (Robot r) => {}),
		new Upgrade("Electrified arms", UpgradeTypes.Hands, 8, "Adds lightning effect to the fists.", (Robot r) => { r.RefreshUpgradeParticle(UpgradeElements.Lightning, Robot.BodyPart.Hands); }, (Robot r) => {}),
		new Upgrade("Burning legs", UpgradeTypes.Feet, 8, "Adds fire effect to the kicks.", (Robot r) => { r.RefreshUpgradeParticle(UpgradeElements.Fire, Robot.BodyPart.Feet); }, (Robot r) => {}),
		new Upgrade("Electrified legs", UpgradeTypes.Feet, 8, "Adds lightning effect to the kicks.", (Robot r) => { r.RefreshUpgradeParticle(UpgradeElements.Lightning, Robot.BodyPart.Feet); }, (Robot r) => {}),
		new Upgrade("Repair kit", UpgradeTypes.Consumable, 12, "Recover 50 health.", (Robot r) => { r.UpdateHealth(50); r.CmdPlayClip(Robot.AudioClips.Repair); }, (Robot r) => {}),
		new Upgrade("Advanced repair kit", UpgradeTypes.Consumable, 20, "Recover 100 health.", (Robot r) => { r.UpdateHealth(100); r.CmdPlayClip(Robot.AudioClips.Repair); }, (Robot r) => {}),
		new Upgrade("Ultimate repair kit", UpgradeTypes.Consumable, 28, "Recover your full health.", (Robot r) => { r.UpdateHealth(r.healthMax); r.CmdPlayClip(Robot.AudioClips.Repair); }, (Robot r) => {}),
		new Upgrade("Burning armor", UpgradeTypes.Armor, 12, "Adds fire effect to the armor.", (Robot r) => { r.RefreshUpgradeParticle(UpgradeElements.Fire, Robot.BodyPart.Body); r.defenseBonus += 2; }, (Robot r) => {}),
		new Upgrade("Electrified armor", UpgradeTypes.Armor, 12, "Adds lightning effect to the armor.", (Robot r) => { r.RefreshUpgradeParticle(UpgradeElements.Lightning, Robot.BodyPart.Body); r.defenseBonus += 2; }, (Robot r) => {}),
		new Upgrade("Freezing arms", UpgradeTypes.Hands, 8, "Adds ice effect to the fists.", (Robot r) => { r.RefreshUpgradeParticle(UpgradeElements.Ice, Robot.BodyPart.Hands); }, (Robot r) => {}),
		new Upgrade("Freezing legs", UpgradeTypes.Feet, 8, "Adds ice effect to the kicks.", (Robot r) => { r.RefreshUpgradeParticle(UpgradeElements.Ice, Robot.BodyPart.Feet); }, (Robot r) => {}),
		new Upgrade("Freezing armor", UpgradeTypes.Armor, 12, "Adds ice effect to the armor.", (Robot r) => { r.RefreshUpgradeParticle(UpgradeElements.Ice, Robot.BodyPart.Body); r.defenseBonus += 2; }, (Robot r) => {}),
		new Upgrade("Sonic arms", UpgradeTypes.Hands, 8, "Adds sonic effect to the fists.", (Robot r) => { r.RefreshUpgradeParticle(UpgradeElements.Sonic, Robot.BodyPart.Hands); }, (Robot r) => {}),
		new Upgrade("Sonic legs", UpgradeTypes.Feet, 8, "Adds sonic effect to the kicks.", (Robot r) => { r.RefreshUpgradeParticle(UpgradeElements.Sonic, Robot.BodyPart.Feet); }, (Robot r) => {}),
		new Upgrade("Sonic armor", UpgradeTypes.Armor, 12, "Adds sonic effect to the armor.", (Robot r) => { r.RefreshUpgradeParticle(UpgradeElements.Sonic, Robot.BodyPart.Body); r.defenseBonus += 2; }, (Robot r) => {}),
	};
}
